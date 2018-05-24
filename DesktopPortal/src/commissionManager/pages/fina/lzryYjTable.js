//离职人员业绩确认表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col} from 'antd'
import SearchCondition from './searchCondition';

class LZRYTJTable extends Component{
    appTableColumns = [
        { title: '员工编号', dataIndex: 'passDate', key: 'passDate' },
        { title: '员工姓名', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '归属组织', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '成交报告编号', dataIndex: 'dealType', key: 'dealType' },
        { title: '业绩产生人', dataIndex: 'wyName', key: 'wyName' },
        { title: '成交日期', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '上业绩日期', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '离职日期', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '业绩金额', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '是否包含', dataIndex: 'wyAddress', key: 'wyAddress' },

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
export default LZRYTJTable