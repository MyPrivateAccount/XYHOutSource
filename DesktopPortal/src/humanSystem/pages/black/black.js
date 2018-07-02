import { connect } from 'react-redux';
import { setLoadingVisible, setSearchLoadingVisible, adduserPage, deleteBlackInfo, getBlackList, selBlackList } from '../../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Spin, Popconfirm, Button, notification, Row, Col, Table } from 'antd';
import './search.less';
import Layer, { LayerRouter } from '../../../components/Layer'
import { Route } from 'react-router'
import Addblack from './addblack'
const buttonDef = [
    { buttonID: "addnew", buttonName: "新建", icon: '', type: 'primary', size: 'large', },
    { buttonID: "modify", buttonName: "修改", icon: '', type: 'primary', size: 'large', },
    { buttonID: "delete", buttonName: "删除", icon: '', type: 'primary', size: 'large', },
];


class MainIndex extends Component {
    state = {
        condition: {
            keyWord: "",
            pageIndex: 0,
            pageSize: 0
        },
        checkList: [],//选中列表
        editblackInfo: null//当前编辑黑名单
    }

    componentWillMount() {
        this.handleSearch()
    }
    _getColumns() {
        const columns = [
            { title: '身份证', dataIndex: 'idCard', key: 'idCard', },
            { title: '姓名', dataIndex: 'name', key: 'name' },
            { title: '性别', dataIndex: 'sex', key: 'sex', render: (text, record) => <div>{text == 1 ? "男" : '女'}</div> },
            { title: '电话', dataIndex: 'phone', key: 'phone' },
            { title: 'email', dataIndex: 'email', key: 'email' },
        ];
        return columns;
    }

    gotoSubPage = (path, params) => {
        this.props.history.push(`${this.props.match.url}/${path}`, params || {})
    }

    handleClickFucButton = (e) => {
        if (e.target.id === "addnew") {
            this.props.dispatch(adduserPage({ id: 11, menuID: 'menu_blackaddnew', displayName: '新建黑名单', type: 'item' }));
            this.gotoSubPage('addblack', {})
        } else if (e.target.id === "modify") {
            if (this.state.checkList.length > 0) {
                this.props.dispatch(adduserPage({ id: 12, menuID: 'menu_blackmodify', displayName: '修改黑名单', type: 'item' }));
                this.gotoSubPage('editblack', this.state.checkList[0])
            }
            else {
                notification.error({
                    description: "请选择指定黑名单",
                    duration: 3
                });
            }
        } else if (e.target.id === "delete") {
            if (this.state.checkList.length > 0) {
                this.props.dispatch(deleteBlackInfo(this.state.checkList[this.state.checkList - 1]));
            }
            else {
                notification.error({
                    description: "请选择指定黑名单",
                    duration: 3
                });
            }
        }
    }
    //是否有权限
    hasPermission(buttonInfo) {
        let hasPermission = false;
        if (this.props.judgePermissions && buttonInfo.requirePermission) {
            for (let i = 0; i < buttonInfo.requirePermission.length; i++) {
                if (this.props.judgePermissions.includes(buttonInfo.requirePermission[i])) {
                    hasPermission = true;
                    break;
                }
            }
        } else {
            hasPermission = true;
        }
        return hasPermission;
    }
    handleSearch = () => {
        this.props.dispatch(setSearchLoadingVisible(true));
        this.props.dispatch(getBlackList(this.state.condition));
    }

    render() {
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                console.log("选中行:", selectedRows);
                this.setState({ checkList: selectedRows });
            }
        };
        let showLoading = this.props.showLoading;
        let columns = this._getColumns();
        let blackList = this.props.searchInfoResult.blackList;
        let paginationInfo = { current: blackList.pageIndex, pageSize: blackList.pageSize, total: blackList.totalCount };
        return (
            <Layer >
                {/* <Spin spinning={showLoading}> */}
                {/* <SearchCondition /> */}
                <div className="searchBox">
                    <Row type="flex">
                        <Col span={12}>
                            <Input placeholder={'请输入名称'} onChange={this.handleKeyChangeWord} />
                        </Col>
                        <Col span={8}>
                            <Button type='primary' className='searchButton' onClick={this.handleSearch}>查询</Button>
                        </Col>
                    </Row>
                </div>
                {
                    buttonDef.map((button, i) => {
                        let hasPermission = this.hasPermission(button);
                        if (hasPermission) {
                            if (button.buttonID == 'delete') {
                                return (this.state.checkList.length > 0 ? <Popconfirm title="确认要删除黑名单?" onConfirm={this.handleClickFucButton} okText="是" cancelText="否">
                                    <Button key={i} id={button.buttonID} style={{ marginTop: '10px', marginBottom: '10px', marginRight: '10px', border: 0 }}
                                        icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button>
                                </Popconfirm> : <Button key={i} id={button.buttonID} style={{ marginTop: '10px', marginBottom: '10px', marginRight: '10px', border: 0 }}
                                    onClick={this.handleClickFucButton}
                                    icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button>
                                )
                            } else {
                                return (<Button key={i} id={button.buttonID} style={{ marginTop: '10px', marginBottom: '10px', marginRight: '10px', border: 0 }}
                                    onClick={this.handleClickFucButton}
                                    icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button>)
                            }
                        }
                    })
                }
                {/* <SearchResult /> */}
                <div>
                    {<p style={{ marginBottom: '10px' }}>目前已为你筛选出<b>{this.props.searchInfoResult.blackList.extension.length}</b>条费用信息</p>}
                    <div id="searchResult">
                        <Table id={"table"} rowKey={record => record.key}
                            columns={columns}
                            pagination={paginationInfo}
                            onChange={this.handleChangePage}
                            dataSource={blackList.extension} bordered size="middle"
                            rowSelection={rowSelection} />
                    </div>
                </div>
                {/* </Spin> */}
                <LayerRouter>
                    <Route path={`${this.props.match.url}/addblack`} render={(props) => <Addblack  {...props} />} />
                    <Route path={`${this.props.match.url}/editblack`} render={(props) => <Addblack  {...props} />} />
                </LayerRouter>
            </Layer>
        )
    }
}

function mapStateToProps(state) {
    return {
        selBlacklist: state.basicData.selBlacklist,
        showLoading: state.basicData.showLoading,
        searchInfoResult: state.search,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);