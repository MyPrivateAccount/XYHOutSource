//实发扣减确认表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Input, Layout, Row, Col,Spin} from 'antd'
import SearchCondition from './searchCondition';
import {searchSfkjqrb,yjSkEmpResult} from '../../actions/actionCreator'

class SFKJQRTable extends Component{
    appTableColumns = [
        { title: '员工编号', dataIndex: 'userInfo.userName', key: 'userInfo.userName' },
        { title: '员工姓名', dataIndex: 'userInfo.trueName', key: 'userInfo.trueName' },
        { title: '归属组织', dataIndex: 'userInfo.orgFullname', key: 'userInfo.orgFullname' },
        { title: '本月提成金额', dataIndex: 'byTc', key: 'byTc' },
        { title: '上月追佣金额', dataIndex: 'syZyYe', key: 'syZyYe' },
        { title: '本月追佣金额', dataIndex: 'byZyTc', key: 'byZyTc' },
        { title: '本月应扣除金额', dataIndex: 'byDzyTc', key: 'byDzyTc' },
        { title: '本月实际扣除金额', dataIndex: 'byKjJe', key: 'byKjJe' ,render: (text, recored) =>(
            <Input value={text} onChange={this.handleEdit(recored.key, 'byKjJe')}/>
        )},
        { title: '本月追佣余额', dataIndex: 'byZyYe', key: 'byZyYe' },

    ];
    state = {
        isDataLoading:false,
        pagination: {},
        dataSource:[]
    }
    handleEdit = (key, dataIndex) => {
        return (value) => {
            console.log("handleEdit:" + value)
            const dataSource = [...this.state.dataSource];
            const target = dataSource.find(item => item.key === key);
            if (target) {
                target['byKjJe'] = value.target.value;
                this.setState({ dataSource });
            }
        };
    }
    getCheckEmps=()=>{
        return this.state.dataSource;
    }
    handleSearch = (e) => {
        this.setState({ isDataLoading: true });
        this.props.dispatch(searchSfkjqrb(e))
    }
    handleTableChange = (pagination, filters, sorter) => {
        let cd = this.props.SearchCondition;
        cd.pageIndex = (pagination.current - 1);
        cd.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.handleSearch(this.state.type);
    };
    componentWillReceiveProps(newProps){
        console.log("new Props:" + newProps.dataSource)
        this.setState({ isDataLoading: false });

        let paginationInfo = {
            pageSize: newProps.dataSource.pageSize,
            current: newProps.dataSource.pageIndex,
            total: newProps.dataSource.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });
        this.setState({dataSource:newProps.dataSource})
        if(newProps.operInfo.operType === 'FINA_QUERY_SKQR_EMP'){
            let emps = this.getCheckEmps()
            if(emps.length>0){
                this.props.dispatch(yjSkEmpResult(emps))
            }
            newProps.operInfo.operType = ''
        }
    }
    render(){
        return (
            <Layout>
                <Layout.Content>
                {
                    this.props.showSearch?<Row style={{margin:10}}>
                    <Col span={24}>
                    <SearchCondition handleSearch={this.handleSearch}/>
                    </Col>
                </Row>:null
                }
                
                <Row style={{margin:10}}>
                    <Col span={24}>
                    <Spin spinning={this.state.isDataLoading}>
                    <Table columns={this.appTableColumns} dataSource={this.state.dataSource} pagination={this.state.pagination} onChange={this.handleTableChange}></Table> 
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
        searchCondition:state.fina.SearchCondition,
        operInfo:state.fina.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(SFKJQRTable);