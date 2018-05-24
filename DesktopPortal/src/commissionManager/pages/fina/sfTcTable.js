//实发提成表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col} from 'antd'
import SearchCondition from './searchCondition';

class SFTcTable extends Component{
    appTableColumns = [
        { title: '事业部', dataIndex: 'passDate', key: 'passDate' },
        { title: '片区', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '小组', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '用户', dataIndex: 'dealType', key: 'dealType' },
        { title: '职别', dataIndex: 'wyName', key: 'wyName' },
        { title: '本月实收业绩', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '本月实收业绩提成', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '待扣追佣金额', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '本月实发', dataIndex: 'wyAddress', key: 'wyAddress' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="分月查看">
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
export default SFTcTable