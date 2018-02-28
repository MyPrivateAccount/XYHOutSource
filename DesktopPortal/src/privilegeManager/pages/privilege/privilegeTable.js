import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Layout, Table, Button, Checkbox, Select, Row, Col, Menu, Icon, Input, Popconfirm, message, Tooltip, Spin } from 'antd';
import { privilegeAdd, privilegeEdit, privilegeDelete, privilegeGetList, appListGet, privilegeDialogClose } from '../../actions/actionCreator';
import PrivilegeEditor from './privilegeEdit';
import { ApplicationTypes } from '../../../constants/baseConfig'

const Option = Select.Option;
const { Header, Sider, Content } = Layout;

class PrivilegeTable extends Component {
    state = {
        condition: { privilegeName: '', appID: '' },
        checkList: [],
        selectAppId: '',
        showLoading: false
    }
    componentWillMount() {
        console.log("加载权限列表：", this.props.appList);
        if (this.props.appList.length > 0) {
            let appID = this.props.appList[0].id;
            this.setState({ selectAppId: appID });
            this.props.dispatch(privilegeGetList({ appid: appID, appList: this.props.appList }));
        } else {
            this.props.dispatch(privilegeGetList({ appid: null }));
        }
    }
    componentWillReceiveProps(newProps) {
        this.setState({ showLoading: false });
        if (this.state.selectAppId == '' && this.props.appList.length > 0) {
            let appID = this.props.appList[0].id;
            this.setState({ selectAppId: appID });
        }
    }
    privilegeTableColumns = [
        {
            title: '选择', dataIndex: 'id', key: 'check', render: (text, recored) => (
                <span>
                    <Checkbox defaultChecked={false} onChange={(e) => this.handleCheckChange(e, recored.id)} />
                </span>
            )
        },
        { title: '权限名', dataIndex: 'name', key: 'name' },
        { title: '分组名', dataIndex: 'groups', key: 'groups' },
        {
            title: '对应工具', dataIndex: 'applicationId', key: 'applicationId', render: (text, recored) => {
                let appName = '';
                for (let i in this.props.appList) {
                    if (this.props.appList[i].id == recored.applicationId) {
                        appName = this.props.appList[i].displayName;
                        break;
                    }
                }
                return (<span>{appName}</span>)
            }
        },
        {
            title: '编辑', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <Tooltip title="权限编辑">
                    <Button type='primary' shape='circle' icon='edit' size='small' onClick={(e) => this.handleEditClick(recored)} />
                </Tooltip>
            )
        }
    ];

    handleAppChange = (appID) => {
        console.log('handleAppChange，appID：', appID);
        this.setState({ selectAppId: appID });
    }
    handleEditClick(privilegeInfo) {
        //console.log("edit:" + JSON.stringify(privilegeInfo));
        this.props.dispatch(privilegeEdit(privilegeInfo));
    }
    handleAddClick = (e) => {
        //console.log("add privilege");
        this.props.dispatch(privilegeAdd(this.state.selectAppId));
    }

    confirmDelete = (e) => {
        let deleteIDs = [];
        this.state.checkList.map((item, i) => {
            if (item.status) {
                deleteIDs.push(item.id);
            }
        });
        console.log("确定删除：", deleteIDs);
        this.props.dispatch(privilegeDelete({ delKeys: deleteIDs, searchAppid: this.state.selectAppId }));
    }
    getAppIcon(appInfo) {
        let appIcon = "appstore";
        let appType = appInfo.applicationType;
        let app = ApplicationTypes.find(app => app.key === appType);
        if (app) {
            appIcon = app.icon;
        }
        return appIcon;
    }

    cancelDelete = (e) => {
        console.log("不删除", e);
    }
    handleSearch = (e) => {
        console.log("search", this.state.selectAppId);
        this.setState({ showLoading: true });
        this.props.dispatch(privilegeGetList({ appid: this.state.selectAppId }));
    }
    handleCheckChange = (e, privilegeID) => {
        //console.log("checkbox change：" + JSON.stringify(e.target.checked));
        let checked = e.target.checked;
        let checkList = this.state.checkList.slice();
        var hasValue = false;
        for (var i in checkList) {
            if (checkList[i].id == privilegeID) {
                checkList[i].status = checked;
                hasValue = true;
                break;
            }
        }
        if (!hasValue) {
            checkList.push({ id: privilegeID, status: checked });
        }
        this.setState({ checkList: checkList });
    }

    render() {
        let removeBtnDisabled = this.state.checkList.filter((item) => item.status).length > 0 ? false : true;
        return (
            <div className="relative">
                <Layout>
                    <Header>
                        应用权限列表
                        <Tooltip title="新增权限">
                            &nbsp;<Button type='primary' shape='circle' icon='plus' onClick={this.handleAddClick} />
                        </Tooltip>
                        <Tooltip title="删除权限">
                            <Popconfirm title="确认要删除选中权限项?" onConfirm={this.confirmDelete} onCancel={this.cancelDelete} okText="确定" cancelText="取消">
                                &nbsp;<Button type='primary' shape='circle' icon='delete' disabled={removeBtnDisabled} onClick={this.handleDeleteClick} />
                            </Popconfirm>
                        </Tooltip>
                    </Header>
                    <Content className='content'>
                        <Row style={{ border: 'solid 1px rgba(87, 83, 83, 0.13)', padding: '5px' }}>
                            {/* <Col span={6}>权限名称&nbsp;<Input placeholder='权限名称' maxLength="25" style={{ width: '170px' }} onChange={this.handleSearchNameChange} /></Col> */}
                            <Col span={6}>所属应用&nbsp;
                                <Select style={{ width: 120 }} onChange={this.handleAppChange} placeholder='请选择应用' defaultValue={this.state.selectAppId}>
                                    {
                                        this.props.appList.map((app, i) => <Option key={app.id} value={app.id} ><Icon type={this.getAppIcon(app)} />{app.displayName}</Option>)
                                    }
                                </Select>
                            </Col>
                            <Col span={6}>
                                <Button type="primary" icon="search" onClick={this.handleSearch}>查询</Button>
                            </Col>
                        </Row>
                        <Spin spinning={this.state.showLoading}>
                            <Table rowKey='id' columns={this.privilegeTableColumns} dataSource={this.props.privilegeList} />
                        </Spin>
                        <PrivilegeEditor />
                    </Content>
                </Layout>
            </div>
        )
    }
}

function privilegetableMapStateToProps(state) {
    //console.log("privilegetableMapStateToProps:" + JSON.stringify(state.app));
    return {
        appList: state.app.appList,
        operInfo: state.privilege.operInfo,
        privilegeList: state.privilege.privilegeList
    }
}

function privilegetableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

export default connect(privilegetableMapStateToProps, privilegetableMapDispatchToProps)(PrivilegeTable);