import { connect } from 'react-redux';
import { setLoadingVisible, getAuditList } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Icon, Row, Col, Spin, Pagination, Menu } from 'antd';
import AuditRecordItem, { auditType } from './item/auditRecordItem';
import SearchCondition from '../constants/searchCondition';


class MySubmitListPage extends Component {
    state = {
        activeTab: 'auditing',
        pagination: { current: 1, pageSize: 10, total: 0 }
    }

    componentWillMount() {
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(SearchCondition.mySubmit));
    }

    componentWillReceiveProps(newProps) {
        let paginationInfo = {
            pageSize: newProps.mySubmit.auditingList.pageSize,
            current: newProps.mySubmit.auditingList.pageIndex,
            total: newProps.mySubmit.auditingList.totalCount
        };
        if (this.state.activeTab === "passed") {
            paginationInfo = {
                pageSize: newProps.mySubmit.auditedList.pageSize,
                current: newProps.mySubmit.auditedList.pageIndex,
                total: newProps.mySubmit.auditedList.totalCount
            };
        } else if (this.state.activeTab === "rejected") {
            paginationInfo = {
                pageSize: newProps.mySubmit.rejectedList.pageSize,
                current: newProps.mySubmit.rejectedList.pageIndex,
                total: newProps.mySubmit.rejectedList.totalCount
            };
        }
        console.log("分页信息：", newProps);
        this.setState({ pagination: paginationInfo });
    }

    handleMenuChange = (e) => {
        console.log("key", e.key);
        this.setState({ activeTab: e.key });
        let condition = SearchCondition.mySubmit;
        if (e.key === "auditing") {
            condition.examineStatus = [1];
        }
        else if (e.key === "passed") {
            condition.examineStatus = [2];
        } else if (this.state.activeTab === "rejected") {
            condition.examineStatus = [3];
        }
        condition.pageIndex = 0;
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(condition));
    }
    //翻页处理
    handleChangePage = (pageIndex, pageSize) => {
        //console.log("翻页:", pageIndex, pageSize);
        let condition = SearchCondition.mySubmit;
        if (this.state.activeTab === "auditing") {
            condition.examineStatus = [1];
        }
        else if (this.state.activeTab === "passed") {
            condition.examineStatus = [2];
        } else if (this.state.activeTab === "rejected") {
            condition.examineStatus = [3];
        }
        condition.pageIndex = (pageIndex - 1);
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(condition));
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
        let auditList = this.props.mySubmit.auditingList.extension || [];
        if (this.state.activeTab === "passed") {
            auditList = this.props.mySubmit.auditedList.extension || [];
        } else if (this.state.activeTab === "rejected") {
            auditList = this.props.mySubmit.rejectedList.extension || [];
        }
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
                                            <Menu.Item key="auditing">审核中</Menu.Item>
                                            <Menu.Item key="passed">审核通过</Menu.Item>
                                            <Menu.Item key="rejected">审核驳回</Menu.Item>
                                        </Menu>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col>
                                        {
                                            auditList.map(auditItem => <AuditRecordItem key={auditItem.id} auditInfo={auditItem} />)
                                        }</Col>
                                    <Col style={{ marginTop: '20px' }}>
                                        <Pagination {...this.state.pagination} onChange={this.handleChangePage} />
                                    </Col>
                                </Row>
                            </Spin>
                        </div>
                    </div> : <div>
                            {/*** 详细页面*/}
                            <Spin spinning={showLoading}>
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
        mySubmit: state.audit.mySubmit,
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
export default connect(mapStateToProps, mapDispatchToProps)(MySubmitListPage);