import { connect } from 'react-redux';
import { removeAdjustItem, adjustCustomer, setLoadingVisible, openCustomerAuditDetail, getAuditHistory, getAuditList, getUserByOrg, getCustomerByUserID, changeSourceOrg, changeTargetOrg } from '../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Button, Row, Col, Badge, Menu, Tooltip, Spin, TreeSelect, Select, Card, Checkbox, Pagination } from 'antd'
import SearchCondition from '../constants/searchCondition';
import moment from 'moment';
import getToolComponent from '../../tools';
const TreeNode = TreeSelect.TreeNode;
const Option = Select.Option;
const transferStyle = {
    marginTop: '5px',
    width: '420px',
    height: '300px',
    display: 'inline-block'
}
const itemStyle = {
    itemBorder: {
        height: '80px',
        border: '1px solid #ccc',
        width: '80%',
        margin: '5px 10px',
        padding: '3px'
    },
    img: {
        width: '70px',
        border: '1px solid #ccc',
        borderRadius: '5px',
        verticalAlign: 'middle'
    }
}

class AdjustCustomerAudit extends Component {
    state = {
        selectedMenuKey: 'auditing',
        customerDataSource: [],//数据源
        targetKeys: [],//将显示在右侧的数据
        pagination: { current: 1, pageSize: 10, total: 0 },
        activeAuditInfo: {},//当前点击的审核信息
    }
    componentWillMount() {
        let condition = SearchCondition.mySubmit;
        condition.pageIndex = 0;
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(condition));
        this.props.dispatch(setLoadingVisible(true));
    }
    componentWillReceiveProps(newProps) {
        let paginationInfo = {
            pageSize: newProps.auditList.pageSize,
            current: newProps.auditList.pageIndex,
            total: newProps.auditList.totalCount
        };
        this.setState({ pagination: paginationInfo });
    }
    handleMenuChange = (e) => {
        console.log("菜单切换:", e);
        this.setState({ selectedMenuKey: e.key }, () => {
            let condition = SearchCondition.mySubmit;
            if (e.key === "auditing") {
                condition.examineStatus = [1];
            }
            else if (e.key === "passed") {
                condition.examineStatus = [2];
            } else if (e.key === "rejected") {
                condition.examineStatus = [3];
            }
            condition.pageIndex = 0;
            console.log("condition:", condition);
            this.props.dispatch(setLoadingVisible(true));
            this.props.dispatch(getAuditList(condition));
        });
    }
    // handleLoadAuditList = () => {
    //     //this.props.dispatch(getAuditList());
    // }
    handleOrgChange = (e, type) => {
        console.log("部门切换:", e);
        if (type === "source") {
            this.props.dispatch(changeSourceOrg());
        } else {
            this.props.dispatch(changeTargetOrg());
        }
        this.props.dispatch(getUserByOrg({ organizationIds: [e], pageIndex: 0, pageSize: 10000, type: type }));
    }
    //翻页处理
    handleChangePage = (pageIndex, pageSize) => {
        //console.log("翻页:", pageIndex, pageSize);
        let condition = SearchCondition.mySubmit;
        condition.pageIndex = (pageIndex - 1);
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(condition));
    }
    //业务员切换处理
    handleUserChange = (e, type) => {
        let condition = (type === "source" ? SearchCondition.sourceCondition : SearchCondition.targetCondition);
        condition.userID = e;
        console.log("业务员切换处理:", e, type, condition);
        this.props.dispatch(getCustomerByUserID(condition));
    }
    //选择变更
    handleCustomerChange = (e) => {
        console.log("客户选择变更", e);
    }
    //调客保存
    handleAdjustCustomer = (e) => {

    }
    //调客审核的客户记录删除
    handleRemoveCustomerAudit = (deleteCustomer) => {
        console.log("删除deleteCustomer", deleteCustomer);
        this.props.dispatch(removeAdjustItem(deleteCustomer.id));
    }
    handleAdjustAgain = (e) => {
        let activeAuditHistory = this.props.activeAuditHistory;
        if (activeAuditHistory.content && activeAuditHistory.content.startsWith("{")) {
            try {
                let requestInfo = JSON.parse(activeAuditHistory.content);
                //console.log("再次请求的:", requestInfo);
                this.props.dispatch(setLoadingVisible(true));
                this.props.dispatch(adjustCustomer(requestInfo));
            } catch (e) { }
        }
    }
    //审核详细
    handleAuditClick = (auditInfo) => {
        console.log("审核详细:", auditInfo);
        this.setState({ activeAuditInfo: auditInfo });
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditHistory(auditInfo.id));
        this.props.dispatch(openCustomerAuditDetail(auditInfo));
    }

    getSourceHeader = () => {
        let userList = this.props.sourceUserList || [];
        return (
            <div>
                归属部门：<TreeSelect style={{ width: '100px' }} allowClear
                    dropdownStyle={{ width: 200, maxHeight: 400, overflow: 'auto' }}
                    onChange={(e) => this.handleOrgChange(e, 'source')} treeData={this.props.orgList}>
                </TreeSelect>
                归属业务员：
                <Select style={{ width: '100px' }} onChange={(e) => this.handleUserChange(e, "source")}>
                    {
                        userList.map(user => <Option key={user.id} value={user.id}>{user.userName}</Option>)
                    }
                </Select>
            </div>)
    }
    getTargetHeader = () => {
        let userList = this.props.targetUserList || [];
        return (
            <div>
                接收部门：<TreeSelect style={{ width: '100px' }} allowClear
                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                    onChange={(e) => this.handleOrgChange(e, "target")} treeData={this.props.orgList}>
                </TreeSelect>
                接收业务员：
                <Select style={{ width: '100px' }} onChange={(e) => this.handleUserChange(e, "target")}>
                    {
                        userList.map(user => <Option key={user.id} value={user.id}>{user.userName}</Option>)
                    }
                </Select>
            </div>)
    }
    render() {
        let auditList = this.props.auditList.extension || [];
        let sourceCustomerList = this.props.sourceCustomerList || [];
        let targetCustomerList = this.props.targetCustomerList || [];
        let AuditDetailPage = getToolComponent("customerAuditInfo");
        let activeAuditHistory = this.props.activeAuditHistory;
        console.log("获取到activeAuditHistory:", activeAuditHistory);
        return (
            <div>
                {
                    !this.props.showAuditDetail ?
                        <div>
                            {/* <Row>
                    <Col>
                        <Card title={this.getSourceHeader()} style={transferStyle} noHovering>
                            <Menu mode="vertical" selectable={false}>
                                {
                                    sourceCustomerList.map(customer => <Menu.Item key={customer.id}><Checkbox onChange={this.handleCustomerChange} />{customer.name}</Menu.Item>)
                                }
                            </Menu>
                        </Card>
                        <Button icon="right" size="small" style={{ margin: '0 5px' }} onClick={this.handleAdjustCustomer} />
                        <Card title={this.getTargetHeader()} style={transferStyle} noHovering>
                            <Menu mode="vertical" selectable={false}>
                                {
                                    sourceCustomerList.map(customer => <Menu.Item key={customer.id}><Checkbox onChange={this.handleCustomerChange} />{customer.name}</Menu.Item>)
                                }
                            </Menu>
                        </Card>
                    </Col>
                </Row> */}
                            {/*审核列表*/}
                            <Menu onClick={this.handleMenuChange} selectedKeys={[this.state.selectedMenuKey]} mode="horizontal">
                                <Menu.Item key="auditing">审核中</Menu.Item>
                                <Menu.Item key="passed">审核通过</Menu.Item>
                                <Menu.Item key="rejected">审核驳回</Menu.Item>
                            </Menu>
                            {/*查询列表*/}
                            <Spin spinning={this.props.showLoading}>
                                {auditList.map(auditInfo =>
                                    < div style={itemStyle.itemBorder} key={auditInfo.id}>
                                        <Row>
                                            <Col span={3}>
                                                <Icon type="export" style={{ fontSize: '48px' }} />
                                            </Col>
                                            <Col span={16}>
                                                <Row style={{ marginBottom: '10px', cursor: 'pointer' }}>
                                                    <Col span={20} onClick={(e) => this.handleAuditClick(auditInfo)}><b>{auditInfo.contentName || "未命名"}</b></Col>
                                                </Row>
                                                <Row style={{ marginBottom: '5px', fontSize: '0.9rem' }}>
                                                    <Col >{auditInfo.taskName}</Col>
                                                    <Col >{auditInfo.desc}</Col>
                                                </Row>
                                            </Col>
                                            <Col span={5}>
                                                <Row style={{ marginBottom: '20px', textAlign: 'right' }}>
                                                    <Col>
                                                        {moment(auditInfo.submitTime).format("YYYY-MM-DD HH:mm:ss")}
                                                    </Col>
                                                </Row>
                                            </Col>
                                        </Row>
                                    </div>)
                                }
                                <Row>
                                    <Col style={{ marginTop: '20px' }}>
                                        <Pagination {...this.state.pagination} onChange={this.handleChangePage} />
                                    </Col>
                                </Row>
                            </Spin>
                        </div>
                        : <Spin spinning={this.props.showLoading}><AuditDetailPage contentInfo={activeAuditHistory} removeCallback={this.handleRemoveCustomerAudit} />
                            <div style={{ width: '80%', textAlign: 'center' }}>
                                {
                                    activeAuditHistory.examineStatus === 3 ? <Button type="primary" onClick={this.handleAdjustAgain}>再次发起调客</Button> : null
                                }

                            </div>
                        </Spin>
                }
            </div>
        )
    }

}

function mapStateToProps(state) {
    //console.log("state.basicData.orgInfo:", state.basicData.orgInfo);
    return {
        orgList: state.basicData.orgInfo.orgList,
        userList: state.basicData.userList,
        sourceUserList: state.basicData.sourceUserList,
        targetUserList: state.basicData.targetUserList,
        auditList: state.search.auditList,
        sourceCustomerList: state.search.sourceCustomerList,
        targetCustomerList: state.search.targetCustomerList,
        activeAuditHistory: state.search.activeAuditHistory,
        showLoading: state.search.showLoading,
        showAuditDetail: state.search.showAuditDetail
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AdjustCustomerAudit);