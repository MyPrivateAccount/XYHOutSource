//提成成本表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col} from 'antd'
import SearchCondition from './searchCondition';

class TCCbTable extends Component{
    appTableColumns = [
        { title: '事业部', dataIndex: 'passDate', key: 'passDate' },
        { title: '片区', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '小组', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '直接成本', dataIndex: 'dealType', key: 'dealType' },
        { title: '分摊入成本', dataIndex: 'wyName', key: 'wyName' },
        { title: '成本', dataIndex: 'wyAddress', key: 'wyAddress' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="查看详情">
                        <Button type='primary' shape='circle' size='small' icon='edit'/>
                    </Tooltip>
                </span>
            )
        }

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
export default TCCbTable