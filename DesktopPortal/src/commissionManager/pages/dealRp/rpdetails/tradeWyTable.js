//外佣表格组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button, Tooltip, Input, Select, Form, InputNumber } from 'antd'
import { getDicPars } from '../../../../utils/utils'
import validations from '../../../../utils/validations'
import { dicKeys } from '../../../constants/const'

const FormItem = Form.Item;

class TradeWyTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
           
        }
       
    }

    _getTableColums = ()=>{
        let sfdxList = getDicPars(dicKeys.sfdx, this.props.dic);
        let kxItems = this.props.items ||[];
        let canEdit = this.props.canEdit || false;
        const isTy = this.props.type === 'ty'

        let appTableColumns = [
            {
                title: '款项类型', dataIndex: 'moneyType', key: 'moneyType',
                render: (text, record) => (
                    <FormItem hasFeedback validateStatus={record.errors['moneyType'] ? 'error' : ''}>
                        <Select value={record.moneyType} disabled={!canEdit || (!record.isNew && isTy) }
                            onChange={v=> this._onRowChanged(record, 'moneyType', v)}>
                            {
                                kxItems.map(tp => <Select.Option key={tp.name} value={tp.code}>{tp.name}</Select.Option>)
                            }
                        </Select>
                    </FormItem>
                )
            },
            {
                title: '收付对象', dataIndex: 'object', key: 'object',
                render: (text, record) => (
                    <FormItem hasFeedback validateStatus={record.errors['object'] ? 'error' : ''}>
                        <Select value={record.object} disabled={!canEdit || (!record.isNew && isTy)}
                            onChange={v=> this._onRowChanged(record, 'object', v)}>
                            {
                                sfdxList.map(tp => <Select.Option key={tp.key} value={tp.value}>{tp.key}</Select.Option>)
                            }
                        </Select>
                    </FormItem>
                )
            },
            {
                title: '备注', dataIndex: 'remark', key: 'remark',
                render: (text, record) => (
                    <FormItem hasFeedback validateStatus={record.errors['remark'] ? 'error' : ''}>
                        <Input value={record.remark}  disabled={!canEdit} onChange={v=> this._onRowChanged(record, 'remark', v)}/>
                    </FormItem>
                )
            },
            {
                title: '金额', dataIndex: 'money', key: 'money',
                width: '16rem',
                render: (text, record) => {
                    let item = kxItems.find(x=>x.code===record.moneyType) ||{};
                    return <FormItem hasFeedback validateStatus={record.errors['money'] ? 'error' : ''}>
                        <InputNumber min={0} value={record.money} style={{width:'100%', textAlign: 'right'}} disabled={!canEdit || item.isfixed}  onChange={v=> this._onRowChanged(record, 'money', v)}/>
                    </FormItem>
                }
            },
            {
                title: '操作', dataIndex: 'edit', key: 'edit', render: (text, record) => {
                    return canEdit?
                    <span>
    
                        <Tooltip title='删除'>
                            &nbsp;<Button type='primary' onClick={()=>this._onDelRow(record)} size='small' >删除</Button>
                        </Tooltip>
                    </span>:null;
                }
            }
        ];

        return appTableColumns;
    }
    
    _onRowChanged = (row, key,value)=>{
        if(this.props.onRowChanged){
            if(value && value.target){
                value = value.target.value;
            }
            this.props.onRowChanged(row, key, value)
        }
    }

    _onDelRow = (row)=>{
        if(this.props.onDelRow){
            this.props.onDelRow(row);
        }
    }

    render() {
        const { dataSource } = this.props;
        const columns = this._getTableColums();
        return (
            <Form>
                <Table bordered size="small" columns={columns} dataSource={dataSource} pagination={false}></Table>
            </Form>
        )
    }
}
function MapStateToProps(state) {

    return {
       
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(TradeWyTable);