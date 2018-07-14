import React, {Component} from 'react'
import {Table} from 'antd'
import moment from 'moment'

class SkTable extends Component{

    state={
        ids:[]
    }
    
    _formatDate = (date)=>{
        if(!date){
            return '';
        }
        return moment(date).format('YYYY-MM-DD');
    }

    _columns = () => {
        return [
            {
                title: '录入日期',
                dataIndex: 'createTime',
                key: 'createTime',
                align: 'center',
                width: '8rem',
                render : (text,record)=>{
                    return this._formatDate(text); 
                }
            },
            {
                title:'结佣日期',
                dataIndex:'jrrq',
                key:'jrrq',
                align: 'center',
                width: '8rem',
                render : (text,record)=>{
                    return this._formatDate(text); 
                }
            },
            // {
            //     title: '成交编号',
            //     dataIndex: 'cjbgbh',
            //     key: 'cjbgbh',
            //     align: 'center'
            // },
            // {
            //     title: '物业',
            //     dataIndex: 'wymc',
            //     key: 'wymc'
            // },
            {
                title: '收据号码',
                dataIndex: 'sjhm',
                key: 'sjhm'
            },
            {
                title: '公司账户',
                dataIndex: 'gszh',
                key: 'gszh'
            },
            {
                title: '部门',
                dataIndex: 'sectionName',
                key: 'sectionName',
                align: 'center'
            },
            {
                title: '收款人',
                dataIndex: 'skr',
                key: 'skr',
                align: 'center'
            },
            {
                title: '录入人',
                dataIndex: 'username',
                key: 'username',
                align: 'center'
            },
            {
                title: '收据日期',
                dataIndex: 'sjrq',
                width: '8rem',
                key: 'sjrq',
                render : (text,record)=>{
                    return this._formatDate(text); 
                }
            },
            {
                title: '进账日期',
                dataIndex: 'jzrq',
                key: 'jzrq',
                width: '8rem',
                render : (text,record)=>{
                    return this._formatDate(text); 
                }
            },
            {
                title: '用途',
                dataIndex: 'yt',
                key: 'yt'
            },
            {
                title: '收款方式',
                dataIndex: 'skfs',
                key: 'skfs'
            },
            {
                title: '金额',
                dataIndex: 'je',
                key: 'je',
                width: '16rem',
                className: 'column-money'
            },
        ];
    }

    getSelectedIds=()=>{
        return this.state.ids ||[]
    }

    rowSelection = {
        onChange: (selectedRowKeys, selectedRows) => {
          console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
          this.setState({ids: selectedRowKeys})
        },
        getCheckboxProps: record => ({
          disabled: !this.props.canEdit || record.jrrq
        }),
      };

    render(){
        let columns = this._columns();
        let rs = null;
        if(this.props.canSelect){
            rs = this.rowSelection;
        }

        let list= this.props.list ||[];

        return (
            <div>
                <Table rowKey="id" rowSelection={rs} pagination={false} bordered size="middle" columns={columns} dataSource={list} />
            </div>
        )
    }
}


export default SkTable;
