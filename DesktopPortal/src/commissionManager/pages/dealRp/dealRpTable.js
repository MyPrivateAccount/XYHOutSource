//成交报告表格
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button, Tooltip, Spin, Popconfirm, notification } from 'antd'
import { myReportGet, searchReport, openRpDetail, dealRpDelete } from '../../actions/actionCreator'
import { getDicPars } from '../../../utils/utils'
import { dicKeys, examineStatusMap } from '../../constants/const'
import { report } from 'import-inspector';
import WebApiConfig from '../../constants/webApiConfig'
import ApiClient from '../../../utils/apiClient';

class DealRpTable extends Component {
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


    _getColumns = () => {
        let jylxList = getDicPars(dicKeys.jylx, this.props.dic)
        let appTableColumns = [
            { title: '审批通过日期', dataIndex: 'successTime', key: 'successTime' },
            {
                title: '成交编号', dataIndex: 'cjbgbh', key: 'cjbgbh',
                render: (text, record) => {
                    return <a onClick={() => this.viewReport(record)}>{text}</a>
                }
            },
            {
                title: '上业绩日期', dataIndex: 'createTime', key: 'createTime'
            },
            {
                title: '类型', dataIndex: 'jylx', key: 'jylx', render: (text, record) => {
                    let item = jylxList.find(x => x.value === record.jylx);
                    if (item) {
                        return item.key;
                    }
                    return text;
                }
            },
            { title: '物业名称', dataIndex: 'reportWy.wyMc', key: 'reportWy.wyMc' },
            { title: '物业地址', dataIndex: 'reportWy.wyCzwydz', key: 'reportWy.wyCzwydz' },
            { title: '成交总价', dataIndex: 'cjzj', key: 'cjzj' },
            { title: '总佣金', dataIndex: 'ycjyj', key: 'ycjyj' },
            { title: '佣金比例', dataIndex: 'yjBl', key: 'yjBl' },
            { title: '所属部门', dataIndex: 'applySectionName', key: 'applySectionName' },
            { title: '录入人', dataIndex: 'uTrueName', key: 'uTrueName' },
            { title: '成交人', dataIndex: 'cjrTrueName', key: 'cjrTrueName' },
            { title: '进行的申请', dataIndex: 'jxdsq', key: 'jxdsq' },
            {
                title: '审批状态', dataIndex: 'examineStatus', key: 'examineStatus', render: (text, record) => {
                    return examineStatusMap[record.examineStatus];
                }
            },
            {
                title: '操作', dataIndex: 'edit', key: 'edit', render: (text, record) => (
                    this.state.type === 'myget' ? (<span>
                        {
                            (record.examineStatus===0||record.examineStatus===16)?
                        <Popconfirm title="是否删除该项?" onConfirm={(e) => this.handleDelClick(record)} onCancel={this.zfcancel} okText="确认" cancelText="取消">
                            <Button type='primary' shape='circle' size='small' icon='delete' />
                        </Popconfirm>:null
                        }
                        {
                            (record.examineStatus===0||record.examineStatus===16)?
                            <Tooltip title='修改'>
                                <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.editReport(record)} />
                            </Tooltip>:null
                        }
                        
                    </span>) : (
                            <span>
                                <Popconfirm title="请确认是否要作废成交报告?" onConfirm={(e) => this.zfconfirm(record)} onCancel={this.zfcancel} okText="确认" cancelText="取消">
                                    <Button type='primary' shape='circle' size='small' icon='delete' />
                                </Popconfirm>
                                <Tooltip title='收款'>
                                    <Button type='primary' shape='circle' size='small' icon='wallet' onClick={(e) => this.handleSKClick(record)} />
                                </Tooltip>
                                <Tooltip title='付款'>
                                    <Button type='primary' shape='circle' size='small' icon='pay-circle-o' onClick={(e) => this.handleFKClick(record)} />
                                </Tooltip>
                                <Tooltip title='调佣'>
                                    <Button type='primary' shape='circle' size='small' icon='setting' onClick={(e) => this.handleTYClick(record)} />
                                </Tooltip>
                                <Tooltip title='转移'>
                                    <Button type='primary' shape='circle' size='small' icon='swap' onClick={(e) => this.handleZYClick(record)} />
                                </Tooltip>
                                <Popconfirm title="请确认xxx报告的结佣资料是否已经收齐?" onConfirm={this.jyconfirm} onCancel={this.jycancel} okText="确认" cancelText="取消">
                                    <Button type='primary' shape='circle' size='small' icon='check' />
                                </Popconfirm>
                            </span>)

                )
            }
        ];
        return appTableColumns;
    }

    jyconfirm = (e) => {
    }
    jycancel = (e) => {
    }
    zfconfirm = (e) => {
        this.handleDelClick(e)
    }
    zfcancel = (e) => {

    }
    handleSKClick = (e) => {
        if (this.props.onOpenDlg !== null) {
            e.type = 'sk'
            this.props.onOpenDlg(e)
        }
    }
    handleFKClick = (e) => {
        if (this.props.onOpenDlg !== null) {
            e.type = 'fk'
            this.props.onOpenDlg(e)
        }
    }
    handleTYClick = (e) => {
        if (this.props.onOpenTy !== null) {
            this.props.onOpenTy(e)
        }
    }
    handleZYClick = (e) => {
        if (this.props.onOpenZy !== null) {
            this.props.onOpenZy(e)
        }
    }

    
    viewReport = (record) => {
        if (this.props.viewReport) {
            this.props.viewReport(record);
        }
    }

    editReport = (record)=>{
        if (this.props.viewReport) {
            this.props.viewReport(record, 'edit');
        }
    }

    handleDelClick = async (report) => {
        if (!report || !report.id) {
            return;
        }
        let url = `${WebApiConfig.rp.rpDel}${report.id}`
        this.setState({ loading: true })
        try {
            let r = await ApiClient.post(url, null, null, 'DELETE');
            r = (r || {}).data;
            if (r.code === '0') {
                notification.success({ message: '成交报告已经作废!' })
                if(this.props.reportChanged){
                    this.props.reportChanged(report, 'DEL')
                }
            } else {
                notification.error({ message: '作废成交报告失败!', description: r.message||'' })
                
            }
        } catch (e) {
            notification.error({ message: '作为成交报告失败!', description: e.message })
        }
        this.setState({loading:false})
    }

    render() {
        const columns = this._getColumns();
        return (
            <Spin spinning={this.props.loading || this.state.loading}>
                <Table pagination={this.props.pagination} columns={columns} dataSource={this.props.dataSource} onChange={this.props.pageChanged} bordered size="middle"></Table>
            </Spin>
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
export default connect(MapStateToProps, MapDispatchToProps)(DealRpTable);