import React, { Component } from 'react'
import { Layout, Button, Spin, Menu, Table, Tooltip, Popconfirm } from 'antd'
import { connect } from 'react-redux'
import FixedTable from '../../components/FixedTable'
import DicGroupDialog from './DicGroupDialog'
import ParDialog from './ParDialog'
import { addDicGroup, editDicGroup, getGroupListAsync, 
    setCurrentGroup, getParListAsync, addDicValue, 
    editDicValue, delDicGroupAsync, delDicValueAsync,
    startDicValueAsync, } from '../actions'


const { Header, Sider, Content } = Layout;

class Dic extends Component {


    componentWillMount = () => {
        this.props.getDicGroupList();
    }
    componentDidMount() {
    }

    changeGroup = ({ key }) => {
        this.props.setCurrentGroup(key);
        this.props.getDicParList(key);
    }
    editGroup = (e, group) => {
        e.preventDefault();
        e.stopPropagation();
        this.props.editDicGroup(group);
    }
    editDicValue = (e, value) => {
        e.preventDefault();
        e.stopPropagation();
        this.props.editDicValue(value);
    }

    render() {
        let { groupList, loadingGroup, currentGroup, parameterList, loadingParameter } = this.props.dic
        let extColumns = [];
        if (currentGroup) {
            if (currentGroup.hasExt1) {
                extColumns.push({ title: currentGroup.ext1Desc, dataIndex: 'ext1', width: '10rem' });
            }
            if (currentGroup.hasExt2) {
                extColumns.push({ title: currentGroup.ext2Desc, dataIndex: 'ext2', width: '10rem' });
            }
        }
        const dicColumns = [
            { title: '名称', dataIndex: 'key', width: '10rem' },
            { title: '值', dataIndex: 'value', width: '10rem' },
            { title: '顺序', dataIndex: 'order', width: '5rem' },
            ...extColumns,
            { title: '是否禁用', dataIndex: 'isDeleted', width: '10rem' },
            { title: '说明', dataIndex: 'desc' },
            {
                title: '操作', className: 'center', dataIndex: '', key: "action", render: (text, record) => {
                    console.log(record, '>>>>>')
                    return (<span>
                        <Tooltip placement="topLeft" title="编辑字典项" arrowPointAtCenter>
                            <Button disabled={record.operating} type="primary" shape="circle" icon="edit" size="small" onClick={(e)=> this.editDicValue(e, record)} />
                        </Tooltip>
                        {/* <Popconfirm title="你确定要删除此字典项么?" onConfirm={() => this.props.delDicValue(record)} onCancel={null} okText="是" cancelText="否">

                            <Button disabled={record.operating} loading={record.operating} type="primary" className="ml-sm" shape="circle" icon="delete" size="small" />

                        </Popconfirm> */}
                         <Popconfirm title={`你确定要${record.isDeleted === '是' ? '启用': '禁用'}此字典项么?`} onCancel={null} okText="是" cancelText="否"
                         onConfirm={() => {record.isDeleted === '是' ? this.props.startDicValue(record) : this.props.delDicValue(record)} }>
                            <Button 
                            disabled={record.operating} 
                            loading={record.operating} 
                            type="primary" 
                            className="ml-sm" 
                            shape="circle" 
                            size="small"
                            icon={record.isDeleted === '是' ? 'play-circle-o' : 'pause-circle-o'} 
                             />
                        </Popconfirm>
                    </span>)
                }, width: '7rem'
            },
        ];

        const pl = parameterList.list.map((item) => {
             item.isDeleted = (item.isDeleted === '是' || item.isDeleted === true) ? '是' : '否'
             return item
        });
        // const pl = parameterList.list.filter((item) => {
        //     item.deleted = item.deleted === undefined || false ? '否' : '是'
        //     return item
        // });
        console.log(pl, '$$$')

        return (
            <div className="inner-page">
                <div className="left-panel">
                    <div className="relative">
                        <Layout>
                            <Header>
                                字典组
                                <Tooltip placement="topLeft" title="添加字典组" arrowPointAtCenter>
                                    <Button className="ml-1" type='primary' shape='circle' icon='plus' size="small" onClick={() => this.props.addDicGroup()} />
                                </Tooltip>
                            </Header>

                            <Content className="overflow">
                                <Spin spinning={loadingGroup}>
                                    <Menu onClick={this.changeGroup} selectedKeys={[(currentGroup || {}).id]}>
                                        {
                                            groupList.filter((item) => {
                                                return item.deleted === undefined || false
                                            }).map((item) => {
                                                return (
                                                    <Menu.Item key={item.id}>
                                                        {(currentGroup || {}).id === item.id ? (
                                                            <span className="right-tool-bar">
                                                                <Tooltip placement="topLeft" title="编辑字典组" arrowPointAtCenter>
                                                                    <Button disabled={item.operating} type="primary" shape="circle" icon="edit" size="small" onClick={(e) => this.editGroup(e, item)} />
                                                                </Tooltip>
                                                                <Popconfirm title="你确定要删除此字典组么?" onConfirm={() => this.props.delDicGroup(item)} onCancel={null} okText="是" cancelText="否">
                                                                    <Button disabled={item.operating} loading={item.operating} type="primary" className="ml-sm" shape="circle" icon="delete" size="small" />
                                                                </Popconfirm>
                                                            </span>) : null
                                                        } <span>{item.name}</span>
                                                    </Menu.Item>
                                                )
                                            })
                                        }

                                    </Menu>
                                </Spin>
                            </Content>
                        </Layout>
                    </div>
                </div>
                <div className="right-panel">
                    <div className="relative">
                        <Layout>
                            <Header>
                                {(currentGroup || {}).name}
                                {currentGroup != null ?
                                    <Tooltip placement="topLeft" title="添加字典项" arrowPointAtCenter>
                                        <Button className="ml-1" type='primary' shape='circle' icon='plus' size="small" onClick={() => this.props.addDicValue()} />
                                    </Tooltip>
                                    : null
                                }

                            </Header>
                            <Content >
                                <div className="full"  >
                                    <FixedTable loading={loadingParameter}
                                        columns={dicColumns}
                                        pagination={false} dataSource={pl} rowKey="value" />
                                </div>
                            </Content>
                        </Layout>
                    </div>
                </div>


                <DicGroupDialog />
                <ParDialog />
            </div>
        )
    }
}

const mapStateToProps = (state, action) => {
    return {
        dic: state.dic
    }
};

const mapDispatchToProps = (dispatch) => {
    return {
        addDicGroup: (...args) => dispatch(addDicGroup(...args)),
        editDicGroup: (...args) => dispatch(editDicGroup(...args)),
        getDicGroupList: (...args) => dispatch(getGroupListAsync(...args)),
        setCurrentGroup: (...args) => dispatch(setCurrentGroup(...args)),
        getDicParList: (...args) => dispatch(getParListAsync(...args)),
        addDicValue: (...args) => dispatch(addDicValue(...args)),
        editDicValue: (...args) => dispatch(editDicValue(...args)),
        delDicGroup: (...args) => dispatch(delDicGroupAsync(...args)),
        delDicValue: (...args) => dispatch(delDicValueAsync(...args)),
        startDicValue: (...args) => dispatch(startDicValueAsync(...args))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Dic);
