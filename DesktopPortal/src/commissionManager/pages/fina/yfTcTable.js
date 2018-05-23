//应发提成表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col} from 'antd'
import SearchCondition from './searchCondition';

class YFtcTable extends Component{
    appTableColumns = [
        { title: '事业部', dataIndex: 'passDate', key: 'passDate' },
        { title: '片区', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '小组', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '用户', dataIndex: 'dealType', key: 'dealType' },
        { title: '职别', dataIndex: 'wyName', key: 'wyName' },
        { title: '分配业绩', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '待扣坏佣业绩', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '实扣坏佣业绩', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '净业绩', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '人数', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '人均业绩', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '提成比例', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '本月应发提成', dataIndex: 'wyAddress', key: 'wyAddress' },
    ];
    render(){
        return (
            <Layout>
                <Layout.Content>
                <Row style={{margin:10}}>
                    <Col span={24}>
                    <SearchCondition/>
                    </Col>
                </Row>
                <Row style={{margin:10}}>
                    <Col span={24}>
                    <Table columns={this.appTableColumns}></Table> 
                    </Col>
                </Row> 
                </Layout.Content>
            </Layout>
        )
    }
}
export default YFtcTable