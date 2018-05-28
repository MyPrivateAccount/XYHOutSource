//外佣表格组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button, Tooltip } from 'antd'

class TradeWyTable extends Component {
    state={
        dataSource:[],
        count:0
    }
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
    componentWillReceiveProps(newProps) {
        if (newProps.add) {
            this.handleAdd()
        }
    }
    handleAdd = () => {
        const { count, dataSource } = this.state;
        const newData = {
            key: count,
            kxlx: `平台费`,
            sfdx: '11',
            bz: `11111`,
            je:'1'
        };
        this.setState({
            dataSource: [...dataSource, newData],
            count: count + 1,
        });
    }
    render() {
        const { dataSource } = this.state;
        return (
            <Table columns={this.appTableColumns} dataSource={dataSource}></Table>
        )
    }
}
export default TradeWyTable