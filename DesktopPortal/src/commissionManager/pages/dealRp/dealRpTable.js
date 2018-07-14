//成交报告表格
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Dropdown,Icon, Table, Button, Tooltip, Spin, Popconfirm, notification,Menu } from 'antd'
import { getDicPars } from '../../../utils/utils'
import { dicKeys, examineStatusMap ,reportOperateAction} from '../../constants/const'
import { report } from 'import-inspector';
import WebApiConfig from '../../constants/webApiConfig'
import ApiClient from '../../../utils/apiClient'
import moment from 'moment'

const MenuItem = Menu.Item;

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

    _formatDate = (date)=>{
        if(!date){
            return '';
        }
        return moment(date).format('YYYY-MM-DD');
    }

    _getActionMenu = (actions, record)=>{
        let btns = [];
        if(actions.length>3){
            //menu
            for(let i = 0;i<2;i++){
                let a = actions[i]
                btns.push(<a   style={{marginLeft:4}} onClick={()=>this.operate(a, record)} key={a.action}>{a.text}</a>)
            }
            let menus = []
            for(let i = 2;i<actions.length;i++){
                let a = actions[i]
                menus.push(<MenuItem onClick={()=>this.operate(a, record)} key={a.key}>{a.text}</MenuItem>)
            }
            btns.push(<Dropdown overlay={<Menu onClick ={(e)=>this._onMenuClick(e, record)}>{menus}</Menu>}><a style={{marginLeft:4}}  className="ant-dropdown-link" >
            更多 <Icon type="down" />
            </a>
            </Dropdown>
            )
        }else{
            for(let i = 0;i<actions.length;i++){
                let a = actions[i]
                btns.push(<a size="small" style={{marginLeft:4}} onClick={()=>this.operate(a, record)} key={a.action}>{a.text}</a>)
            }
        }
        return btns;
    }

    _onMenuClick = ({key},record)=>{
        this.operate(reportOperateAction[key], record);
    }

    _getColumns = () => {
        let cjbglxList = getDicPars(dicKeys.cjbglx, this.props.dic)
        let appTableColumns = [
            { title: '审批通过日期', dataIndex: 'successTime', key: 'successTime', render:(text,record)=>{
                return this._formatDate(text)
            } },
            {
                title: '成交编号', dataIndex: 'cjbgbh', key: 'cjbgbh',
                render: (text, record) => {
                    return <a onClick={() => this.viewReport(record)}>{text}</a>
                }
            },
            {
                title: '上业绩日期', dataIndex: 'createTime', key: 'createTime', render:(text,record)=>{
                    return this._formatDate(text)
                }
            },
            {
                title: '类型', dataIndex: 'cjbglx', key: 'cjbglx', render: (text, record) => {
                    let item = cjbglxList.find(x => x.value === record.jylx);
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
                title: '操作', dataIndex: 'edit', key: 'edit', width:'10rem', render: (text, record) => {
                    if (this.state.type === 'myget') {
                        return <span>
                            {
                                (record.examineStatus === 0 || record.examineStatus === 16) ?
                                    <Popconfirm title="是否删除该项?" onConfirm={(e) => this.handleDelClick(record)} onCancel={this.zfcancel} okText="确认" cancelText="取消">
                                        <Button type='primary' shape='circle' size='small' icon='delete' />
                                    </Popconfirm> : null
                            }
                            {
                                (record.examineStatus === 0 || record.examineStatus === 16) ?
                                    <Tooltip title='修改'>
                                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.editReport(record)} />
                                    </Tooltip> : null
                            }

                        </span>;
                    } else {
                        let actions = [];
                        let es = record.examineStatus || 0;
                        let p = this.props.permission||{};
                        let isMy = record.uid === this.props.user.sub
                        if(es === 8){
                            //审核通过
                            if(p.sk || isMy){
                                actions.push(reportOperateAction.sk)
                            }
                            if(p.fk || isMy){
                                actions.push(reportOperateAction.fk)
                            }
                            if(p.ty || isMy){
                                actions.push(reportOperateAction.ty)
                            }
                            if(p.zy || isMy){
                                actions.push(reportOperateAction.zy)
                            }
                            if(p.jy){
                                actions.push(reportOperateAction.jy)
                            }
                        }else if(es !==1){
                            if(p.zf || isMy){
                                actions.push(reportOperateAction.zf)
                            }
                        }

                        let element = this._getActionMenu(actions, record);
                       
                        return element.length>0? <div>{element}</div>: null
                    }
                }



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

    operate = ( action, report) => {
        if (this.props.operate) {
            this.props.operate(action, report)
        }
    }


    viewReport = (record) => {
        if (this.props.viewReport) {
            this.props.viewReport(record);
        }
    }

    editReport = (record) => {
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
                if (this.props.reportChanged) {
                    this.props.reportChanged(report, 'DEL')
                }
            } else {
                notification.error({ message: '作废成交报告失败!', description: r.message || '' })

            }
        } catch (e) {
            notification.error({ message: '作为成交报告失败!', description: e.message })
        }
        this.setState({ loading: false })
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
        user: state.oidc.user.profile
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(DealRpTable);