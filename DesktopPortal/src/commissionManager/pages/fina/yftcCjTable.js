//应发提成冲减表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col,Spin} from 'antd'
import SearchCondition from './searchCondition';
import {searchYftccjb} from '../../actions/actionCreator'

class YFTCCJTable extends Component{
    appTableColumns = [
        { title: '事业部', dataIndex: 'branchLevel1Name', key: 'branchLevel1Name' },
        { title: '片区', dataIndex: 'branchLevel2Name', key: 'branchLevel2Name' },
        { title: '小组', dataIndex: 'branchLevel3Name', key: 'branchLevel3Name' },
        { title: '直接冲抵成本', dataIndex: 'cost', key: 'cost' },
        { title: '分摊冲抵成本', dataIndex: 'ftCost', key: 'ftCost' },
        { title: '本月总冲抵成本', dataIndex: 'totalCost', key: 'totalCost' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="月冲减明细">
                        <Button type='primary' shape='circle' size='small' icon='edit'/>
                    </Tooltip>
                </span>
            )
        }

    ];
    state = {
        isDataLoading:false,
        pagination: {},
        searchCondition:{}
    }
    handleSearch = (e) => {
        this.setState({ isDataLoading: true,searchCondition:e });
        this.props.dispatch(searchYftccjb(e))
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
                    <SearchCondition handleSearch={this.handleSearch} orgPermission={'YJ_CW_YFTCCJB'}/>
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
export default connect(MapStateToProps, MapDispatchToProps)(YFTCCJTable);