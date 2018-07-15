import React, { Component } from 'react'
import { Table, Tooltip } from 'antd'
import moment from 'moment'
import { examineStatusMap } from '../../../constants/const';

class SfkTable extends Component {

    state = {
        ids: []
    }

    _formatDate = (date) => {
        if (!date) {
            return '';
        }
        return moment(date).format('YYYY-MM-DD');
    }

    view = (distribute) => {
        if (this.props.view) {
            this.props.view(distribute);
        }
    }

    _columns = () => {
        let fkytList = this.props.wyItems || [];

        let cs = [
            {
                title: '类型',
                dataIndex: 'dsdfType',
                key: 'dsdfType',
                align: 'center',
                width: '4rem',
                render: (text, record) => {
                    return record.dsdfType === 1 ? '收款' : '付款';
                }
            },
            {
                title: '录入日期',
                dataIndex: 'createTime',
                key: 'createTime',
                align: 'center',
                width: '8rem',
                render: (text, record) => {
                    return this._formatDate(text);
                }
            },
            {
                title: '部门',
                dataIndex: 'sectionName',
                key: 'sectionName',
                align: 'center'
            },
            {
                title: '收/付款人',
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
                title: '进账/付款日期',
                dataIndex: 'jzrq',
                key: 'jzrq',
                width: '8rem',
                render: (text, record) => {
                    return this._formatDate(text);
                }
            },
            {
                title: '用途',
                dataIndex: 'yt',
                key: 'yt',
                width: '8rem',
                render: (text, record) => {
                    if (record.dsdfType === 1) {
                        return text;
                    }
                    let item = fkytList.find(x => x.code === text);
                    return (item || {}).name || '';
                }
            },
            {
                title: '金额',
                dataIndex: 'je',
                key: 'je',
                width: '16rem',
                className: 'column-money'
            },
            {
                title: '审核状态',
                dataIndex: 'status',
                key: 'status',
                width: '6rem',
                render: (text,record)=>{
                    return examineStatusMap[record.status]
                }
            },
        ];

        if (!this.props.hideSfkCk) {
            cs.push({
                title: '操作', dataIndex: 'edit', key: 'edit', width: '5rem', render: (text, record) => (
                    <span>

                        <Tooltip title='查看'>
                            <a style={{ marginLeft: 4 }} onClick={(e) => this.view(record)}>查看</a>
                        </Tooltip>
                    </span>
                )
            })
        }
        return cs;
    }

    render() {
        let columns = this._columns();
        let list = this.props.list || [];

        return (
            <div>
                <Table pagination={false} bordered size="middle" columns={columns} dataSource={list} />
            </div>
        )
    }
}


export default SfkTable;
