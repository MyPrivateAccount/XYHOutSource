//应发提成表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Layout, Row, Col,Spin} from 'antd'
import SearchCondition from './searchCondition';
import {searchYftcb} from '../../actions/actionCreator'

class YFtcTable extends Component{
    appTableColumns = [
        { title: '事业部', dataIndex: 'branchLevel1Name', key: 'branchLevel1Name' },
        { title: '片区', dataIndex: 'branchLevel2Name', key: 'branchLevel2Name' },
        { title: '小组', dataIndex: 'branchLevel3Name', key: 'branchLevel3Name' },
        { title: '用户', dataIndex: 'userInfo.trueName', key: 'userInfo.trueName' },
        { title: '职别', dataIndex: 'userInfo.positionName', key: 'userInfo.positionName' },
        { title: '分配业绩', dataIndex: 'fpYj', key: 'fpYj' },
        { title: '待扣坏佣业绩', dataIndex: 'dkHyYj', key: 'dkHyYj' },
        { title: '实扣坏佣业绩', dataIndex: 'skHyYj', key: 'skHyYj' },
        { title: '净业绩', dataIndex: 'yjJe', key: 'yjJe' },
        { title: '人数', dataIndex: 'rs', key: 'rs' },
        { title: '人均业绩', dataIndex: 'rjYj', key: 'rjYj' },
        { title: '提成比例', dataIndex: 'rate', key: 'rate' },
        { title: '本月应发提成', dataIndex: 'byyfJe', key: 'byyfJe' },
    ];
    state = {
        isDataLoading:false,
        pagination: {},
        searchCondition:{}
    }
    componentDidMount(){
        console.log("YFTC Table load")
    }
    handleSearch = (e) => {
        this.setState({ isDataLoading: true,searchCondition:e });
        this.props.dispatch(searchYftcb(e))
    }
    handleTableChange = (pagination, filters, sorter) => {
        let cd = {...this.state.searchCondition};
        cd.pageIndex = (pagination.current - 1);
        cd.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.handleSearch(cd);
    };
    componentWillReceiveProps(newProps){
        console.log("new Props:" + newProps.dataSource)
        this.setState({ isDataLoading: false });
        if(newProps.dataSource){
            let paginationInfo = {
                pageSize: newProps.dataSource.pageSize,
                current: newProps.dataSource.pageIndex,
                total: newProps.dataSource.totalCount
            };
            console.log("分页信息：", paginationInfo);
            this.setState({ pagination: paginationInfo });
        }

    }
    render(){
        return (
            <Layout>
                <Layout.Content>
                <Row style={{margin:10}}>
                    <Col span={24}>
                    <SearchCondition handleSearch={this.handleSearch} orgPermission={'YJ_CW_YFTCB'}/>
                    </Col>
                </Row>
                <Row style={{margin:10}}>
                    <Col span={24}>
                    <Spin spinning={this.state.isDataLoading}>
                    <Table columns={this.appTableColumns} dataSource={this.props.dataSource.extension} pagination={this.state.pagination} onChange={this.handleTableChange}></Table> 
                    </Spin>
                    </Col>
                </Row> 
                </Layout.Content>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        dataSource:state.fina.dataSource,
        searchCondition:state.fina.SearchCondition
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(YFtcTable);