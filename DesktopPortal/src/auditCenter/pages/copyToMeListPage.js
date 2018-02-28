import { connect } from 'react-redux';
import { setLoadingVisible, getAuditList, getNoReadCount } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Icon, Row, Col, Spin, Pagination, Menu, Badge } from 'antd';
import AuditRecordItem, { auditType } from './item/auditRecordItem';
import SearchCondition from '../constants/searchCondition';

class CopyToMeListPage extends Component {
    state = {
        activeTab: 'all',
        pagination: { current: 1, pageSize: 10, total: 0 }
    }
    componentWillMount() {
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(SearchCondition.copyToMe));
        this.props.dispatch(getNoReadCount());
    }
    componentWillReceiveProps(newProps) {
        let paginationInfo = {
            pageSize: newProps.copyToMe.allList.pageSize,
            current: newProps.copyToMe.allList.pageIndex,
            total: newProps.copyToMe.allList.totalCount
        };
        if (this.state.activeTab === "noRead") {
            paginationInfo = {
                pageSize: newProps.copyToMe.noReadList.pageSize,
                current: newProps.copyToMe.noReadList.pageIndex,
                total: newProps.copyToMe.noReadList.totalCount
            };
        }
        console.log("分页信息：", newProps);
        this.setState({ pagination: paginationInfo });
    }

    handleMenuChange = (e) => {
        console.log("key", e.key);
        this.setState({ activeTab: e.key });
        let condition = SearchCondition.copyToMe;
        if (e.key === "all") {
            condition.status = [];
        } else if (e.key === "noRead") {
            condition.status = [1];
        }
        SearchCondition.copyToMe.pageIndex = 0;
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(SearchCondition.copyToMe));
    }
    //翻页处理
    handleChangePage = (pageIndex, pageSize) => {
        //console.log("翻页:", pageIndex, pageSize);
        let condition = SearchCondition.copyToMe;
        if (this.state.activeTab === "all") {
            condition.status = [];
        } else if (this.state.activeTab === "noRead") {
            condition.status = [1];
        }
        condition.pageIndex = (pageIndex - 1);
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(condition));
    }

    getAuditPage(auditInfo) {
        if (auditType[auditInfo.contentType]) {
            return auditType[auditInfo.contentType].component;
        } else {
            return <div>未知的审核类型</div>
        }
    }

    render() {
        const showLoading = this.props.showLoading;
        const navigator = this.props.navigator;
        let auditList = this.props.copyToMe.allList.extension;
        if (this.state.activeTab === "noRead") {
            auditList = this.props.copyToMe.noReadList.extension;
        }
        const noReadCount = this.props.noReadCount || 0;
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
                                            <Menu.Item key="all">全部</Menu.Item>
                                            <Menu.Item key="noRead"><Badge count={noReadCount}><span>未读</span></Badge></Menu.Item>
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
        copyToMe: state.audit.copyToMe,
        noReadCount: state.audit.noReadCount,
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
export default connect(mapStateToProps, mapDispatchToProps)(CopyToMeListPage);