import { connect } from 'react-redux';
import {getNoReadCount, setLoadingVisible, getAuditList, openAuditDetail, getAuditHistory, getActiveDetail, getZywActiveDetail } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Row, Col, Spin, Pagination, Menu, Badge } from 'antd';
import AuditRecordItem from './item/auditRecordItem';
import SearchCondition from '../constants/searchCondition';
import Layer, { LayerRouter } from '../../components/Layer';
import { Route } from 'react-router'
import AuditDetail from './auditDetail'

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
        if (this.props.auditedList !== newProps.auditedList) {
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



    onClick = (auditInfo) => {
        this.props.dispatch(getAuditHistory({
            id: auditInfo.id, callback: (b, entity) => {
                if (b) {
                    this.props.history.push(`${this.props.match.url}/detail`, { entity: entity })
                }
            }
        }));
        if (auditInfo.submitDefineId && !(auditInfo.contentType || "").includes("Deal")) {
            if (auditInfo.contentType.startsWith("ZYW")) {
                this.props.dispatch(getZywActiveDetail(auditInfo.submitDefineId));
            } else {
                this.props.dispatch(getActiveDetail(auditInfo.submitDefineId));
            }
        }
        this.props.dispatch(openAuditDetail(auditInfo))
    }

    render() {
        const showLoading = this.props.showLoading;
        let auditList = this.props.copyToMe.allList.extension;
        if (this.state.activeTab === "noRead") {
            auditList = this.props.copyToMe.noReadList.extension;
        }
        const noReadCount = this.props.noReadCount || 0;
        return (
            <Layer>
                <div id='auditList'>
                    <Spin spinning={showLoading}>
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
                                    auditList.map(auditItem => <AuditRecordItem key={auditItem.id} auditInfo={auditItem} onClick={this.onClick} />)
                                }</Col>
                            <Col style={{ marginTop: '20px' }}>
                                <Pagination {...this.state.pagination} onChange={this.handleChangePage} />
                            </Col>
                        </Row>
                    </Spin>
                </div>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/detail`} render={(props) => <AuditDetail setPageTitle={this.props.setPageTitle} {...props} />} />
                </LayerRouter>
            </Layer>

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