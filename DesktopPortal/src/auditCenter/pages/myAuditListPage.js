import { connect } from 'react-redux';
import { setLoadingVisible, getAuditList } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Icon, Row, Col, Spin, Pagination, Menu } from 'antd';
import AuditRecordItem, { auditType } from './item/auditRecordItem';
import SearchCondition from '../constants/searchCondition';

class MyAuditListPage extends Component {
    state = {
        activeTab: 'waitAudit',
        pagination: { current: 1, pageSize: 10, total: 0 }
    }
    componentWillMount() {
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(SearchCondition.myAudit.waitAuditListCondition));
    }

    componentWillReceiveProps(newProps) {
        let paginationInfo = {};
        if (this.state.activeTab === "audited") {
            paginationInfo = {
                pageSize: newProps.auditedList.pageSize,
                current: newProps.auditedList.pageIndex,
                total: newProps.auditedList.totalCount
            };
        } else {
            paginationInfo = {
                pageSize: newProps.waitAuditList.pageSize,
                current: newProps.waitAuditList.pageIndex,
                total: newProps.waitAuditList.totalCount
            };
        }
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });
    }

    handleMenuChange = (e) => {
        console.log("key", e.key);
        this.setState({ activeTab: e.key });
        let condition = SearchCondition.myAudit.waitAuditListCondition;
        if (e.key === "audited") {
            condition = SearchCondition.myAudit.auditedListCondition;
        }
        condition.pageIndex = 0;
        console.log("翻页search:", condition);
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(condition));
    }
    //翻页处理
    handleChangePage = (pageIndex, pageSize) => {
        //console.log("翻页:", pageIndex, pageSize);
        let searchCondition = SearchCondition.myAudit.waitAuditListCondition;
        if (this.state.activeTab === "audited") {
            searchCondition = SearchCondition.myAudit.auditedListCondition;
        }
        searchCondition.pageIndex = (pageIndex - 1);
        console.log("翻页search:", searchCondition);
        this.props.dispatch(getAuditList(searchCondition));
    }

    getAuditPage(auditInfo) {
        console.log("我发起的:", auditInfo);
        if (auditType[auditInfo.contentType]) {
            return auditType[auditInfo.contentType].component;
        } else {
            return <div>未知的审核类型</div>
        }
    }

    render() {
        const showLoading = this.props.showLoading;
        const navigator = this.props.navigator;
        let auditList = [];
        if (this.state.activeTab === "waitAudit") {
            auditList = this.props.waitAuditList.extension || [];
        } else {
            auditList = this.props.auditedList.extension || [];
        }
        console.log("auditList", auditList);
        return (
            <div>
                {//首页
                    navigator.length === 0 ? <div>
                        <div id='auditList'>
                            <Spin delay={300} spinning={showLoading}>
                                <Row>
                                    <Col>
                                        <Menu onClick={this.handleMenuChange} selectedKeys={[this.state.activeTab]}
                                            mode="horizontal">
                                            <Menu.Item key="waitAudit">待我审批的</Menu.Item>
                                            <Menu.Item key="audited">我参与的</Menu.Item>
                                        </Menu>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col>
                                        {
                                            auditList.map((auditItem, i) => <AuditRecordItem key={auditItem.id + i} auditInfo={auditItem} />)
                                        }</Col>
                                    <Col style={{ marginTop: '20px' }}>
                                        <Pagination {...this.state.pagination} onChange={this.handleChangePage} />
                                    </Col>
                                </Row>
                            </Spin>
                        </div>
                    </div> : <div>
                            <Spin spinning={showLoading}>
                                {/*** 详细页面*/}
                                {
                                    this.getAuditPage(navigator[0])
                                }
                            </Spin>
                        </div>
                }
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        waitAuditList: state.audit.myAudit.waitAuditList,//待审核列表
        auditedList: state.audit.myAudit.auditedList,//已审核
        showLoading: state.audit.showLoading,
        navigator: state.audit.navigator,
        activeMenu: state.audit.activeMenu,
        searchCondition: state.audit.searchCondition//查询条件
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MyAuditListPage);