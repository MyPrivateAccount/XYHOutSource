import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Tooltip, Spin, Popconfirm } from 'antd'
import { getDicPars } from '../../../utils/utils'
import { dicKeys, examineStatusMap } from '../../constants/const'
import moment from 'moment'


class YjfpTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
            dataSource: [],
            pagination: {},
            isDataLoading: false,
            type: this.props.type,
            loading: false
        }



    }

    _formatDate = (date) => {
        if (!date) {
            return '';
        }
        return moment(date).format('YYYY-MM-DD');
    }


    _getColumns = () => {
        let cjbglxList = getDicPars(dicKeys.cjbglx, this.props.dic)
        let nyItems = this.props.nyItems;
        let appTableColumns = [

            {
                title: '成交编号', dataIndex: 'report.cjbgbh', key: 'report.cjbgbh', width: '8rem',
                render: (text, record) => {
                    return <a onClick={() => this.viewReport(record.report)}>{text}</a>
                }
            },
            {
                title: '成交日期', dataIndex: 'report.cjrq', key: 'report.cjrq', width: '8rem', render: (text, record) => {
                    return this._formatDate(text)
                }
            },
            { title: '物业名称', dataIndex: 'report.reportWy.wyMc', key: 'report.reportWy.wyMc' },
            {
                title: '上业绩日期', dataIndex: 'report.createTime', key: 'report.createTime', width: '8rem', render: (text, record) => {
                    return this._formatDate(text)
                }
            },
            {
                title: '合同签约日期', dataIndex: 'report.htqyrq', key: 'report.htqyrq', width: '8rem', render: (text, record) => {
                    return this._formatDate(text)
                }
            },
            {
                title: '类型', dataIndex: 'report.cjbglx', key: 'report.cjbglx', width: '6rem', render: (text, record) => {
                    let item = cjbglxList.find(x => x.value === record.report.jylx);
                    if (item) {
                        return item.key;
                    }
                    return text;
                }
            },
            { title: '工号', dataIndex: 'workNumber', key: 'workNumber', width: '8rem' },
            { title: '姓名', dataIndex: 'username', key: 'username', width: '8rem' },
            { title: '部门', dataIndex: 'sectionName', key: 'sectionName' },
            {
                title: '身份', dataIndex: 'type', key: 'type', width: '5rem',
                render: (text, record) => {
                    let item = nyItems.find(x => x.code === text);
                    if (item) {
                        return item.name;
                    }
                    return text;
                }
            },
            {
                title: '金额',
                dataIndex: 'money',
                key: 'money',
                width: '6rem',
                className: 'column-money'
            },
            {
                title: '单数',
                dataIndex: 'oddNum',
                key: 'oddNum',
                width: '3rem'
            },
            {
                title: '审核状态',
                dataIndex: 'report.examineStatus',
                key: 'report.examineStatus',
                width: '5rem',
                render:(text,record)=>{
                    return examineStatusMap[record.report.examineStatus]||''
                }
            }
        ];
        return appTableColumns;
    }



    viewReport = (record) => {
        if (this.props.viewReport) {
            this.props.viewReport(record);
        }
    }

    _showTotal = (total, range) => `当前 ${range[0]}-${range[1]} | 共 ${total} 条数据`

    render() {
        const columns = this._getColumns();
        const p = { ...this.props.pagination, showTotal: this._showTotal }
        return (
            <Spin spinning={this.props.loading || this.state.loading}>
                <Table pagination={p} columns={columns} dataSource={this.props.dataSource} onChange={this.props.pageChanged} bordered size="middle"></Table>
            </Spin>
        )
    }
}
function MapStateToProps(state) {

    return {
        user: state.oidc.user.profile
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(YjfpTable);