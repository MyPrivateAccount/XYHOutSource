import { connect } from 'react-redux';
import { setLoadingVisible, getAuditList, openAuditDetail,getAuditHistory, getActiveDetail, getZywActiveDetail  } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Row, Col, Spin, Pagination, Menu } from 'antd';
import AuditRecordItem from './item/auditRecordItem';
import SearchCondition from '../constants/searchCondition';
import Layer, { LayerRouter } from '../../components/Layer';
import { Route } from 'react-router'
import AuditDetail from './auditDetail'

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

        if(this.props.auditedList!== newProps.auditedList){
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
        let auditList = [];
        if (this.state.activeTab === "waitAudit") {
            auditList = this.props.waitAuditList.extension || [];
        } else {
            auditList = this.props.auditedList.extension || [];
        }
        return (
            <Layer>
               
                    <div id='auditList'>
                        <Spin  spinning={showLoading}>
                            <Row>
                                <Col>
                                    <Menu onClick={this.handleMenuChange} selectedKeys={[this.state.activeTab]}
                                        mode="horizontal">
                                        <Menu.Item key="waitAudit">待我审核的</Menu.Item>
                                        <Menu.Item key="audited">我参与的</Menu.Item>
                                    </Menu>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    {
                                        auditList.map((auditItem, i) => <AuditRecordItem key={auditItem.id + i} auditInfo={auditItem} onClick={this.onClick} />)
                                    }</Col>
                                <Col style={{ marginTop: '20px' }}>
                                    <Pagination {...this.state.pagination} onChange={this.handleChangePage} />
                                </Col>
                            </Row>
                        </Spin>
                    </div>
                
                <LayerRouter>
                    <Route path={`${this.props.match.url}/detail`} render={(props) => <AuditDetail setPageTitle={this.props.setPageTitle}  {...props} />} />
                </LayerRouter>
            </Layer>
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