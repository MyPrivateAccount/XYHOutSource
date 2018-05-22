//成交报告表格
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip} from 'antd'

class DealRpTable extends Component{
    appTableColumns = [
        { title: '审批通过日期', dataIndex: 'passDate', key: 'passDate' },
        { title: '成交编号', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '上业绩日期', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '类型', dataIndex: 'dealType', key: 'dealType' },
        { title: '物业名称', dataIndex: 'wyName', key: 'wyName' },
        { title: '物业地址', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '成交总价', dataIndex: 'totalPrice', key: 'totalPrice' },
        { title: '总佣金', dataIndex: 'totalCms', key: 'totalCms' },
        { title: '佣金比例', dataIndex: 'cmsScale', key: 'cmsScale' },
        { title: '所属部门', dataIndex: 'dep', key: 'dep' },
        { title: '录入人', dataIndex: 'inputer', key: 'inputer' },
        { title: '成交人', dataIndex: 'clinch', key: 'clinch' },
        { title: '进行的申请', dataIndex: 'application', key: 'application' },
        { title: '审批状态', dataIndex: 'checkState', key: 'checkState' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="作废">
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleDelClick(recored)} />
                    </Tooltip>
                    <Tooltip title='收款'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title='付款'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title='调佣'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title='转移'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title='结佣确认'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    render(){
        return (
            <Table columns={this.appTableColumns}></Table>
        )
    }
}
export default DealRpTable