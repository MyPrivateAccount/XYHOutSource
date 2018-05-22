//内部分配表格
//外佣表格组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip} from 'antd'

class TradeNTable extends Component{
    appTableColumns = [
        { title: '部门', dataIndex: 'orgName', key: 'orgName' },
        { title: '员工', dataIndex: 'paramName', key: 'paramName' },
        { title: '工号', dataIndex: 'paramVal', key: 'paramVal' },
        { title: '金额', dataIndex: 'paramVal', key: 'paramVal' },
        { title: '比例', dataIndex: 'orgName', key: 'orgName' },
        { title: '单数', dataIndex: 'paramName', key: 'paramName' },
        { title: '身份', dataIndex: 'paramVal', key: 'paramVal' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

                    <Tooltip title='删除'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' />
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
export default TradeNTable