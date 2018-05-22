//外佣表格组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip} from 'antd'

class TradeWyTable extends Component{
    appTableColumns = [
        { title: '款项类型', dataIndex: 'orgName', key: 'orgName' },
        { title: '收付对象', dataIndex: 'paramName', key: 'paramName' },
        { title: '备注', dataIndex: 'paramVal', key: 'paramVal' },
        { title: '金额', dataIndex: 'paramVal', key: 'paramVal' },
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
export default TradeWyTable