//实发扣减确认表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col} from 'antd'
import SearchCondition from './searchCondition';

class SFKJQRTable extends Component{
    appTableColumns = [
        { title: '员工编号', dataIndex: 'passDate', key: 'passDate' },
        { title: '员工姓名', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '归属组织', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '本月提成金额', dataIndex: 'dealType', key: 'dealType' },
        { title: '上月追佣金额', dataIndex: 'wyName', key: 'wyName' },
        { title: '本月追佣金额', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '本月应扣除金额', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '本月实际扣除金额', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '本月追佣余额', dataIndex: 'wyAddress', key: 'wyAddress' },

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
export default SFKJQRTable