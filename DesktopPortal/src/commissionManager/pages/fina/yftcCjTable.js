//应发提成冲减表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col} from 'antd'
import SearchCondition from './searchCondition';

class YFTCCJTable extends Component{
    appTableColumns = [
        { title: '事业部', dataIndex: 'passDate', key: 'passDate' },
        { title: '片区', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '小组', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '直接冲抵成本', dataIndex: 'dealType', key: 'dealType' },
        { title: '分摊冲抵成本', dataIndex: 'wyName', key: 'wyName' },
        { title: '本月总冲抵成本', dataIndex: 'wyAddress', key: 'wyAddress' },
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
export default YFTCCJTable