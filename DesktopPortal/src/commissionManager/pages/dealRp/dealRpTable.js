//成交报告表格
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button, Tooltip, Spin, Popconfirm } from 'antd'
import { myReportGet, searchReport,openRpDetail } from '../../actions/actionCreator'
import moment from 'moment'

class DealRpTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
            dataSource: [],
            pagination: {},
            isDataLoading: false,
            type: 'myget'
        }
    }
    appTableColumns = [
        { title: '审批通过日期', dataIndex: 'examinedTime', key: 'examinedTime' },
        {
            title: '成交编号', dataIndex: 'cjbgbh', key: 'cjbgbh',
            render:(text,recored)=>{
                return <a onClick={() => {this.props.dispatch(openRpDetail(recored))}}>{text}</a>
            }
        },
        {
            title: '上业绩日期', dataIndex: 'cjrq', key: 'cjrq'
        },
        { title: '类型', dataIndex: 'jylx', key: 'jylx' },
        { title: '物业名称', dataIndex: 'wyMc', key: 'wyMc' },
        { title: '物业地址', dataIndex: 'wyCzwydz', key: 'wyCzwydz' },
        { title: '成交总价', dataIndex: 'cjzj', key: 'cjzj' },
        { title: '总佣金', dataIndex: 'zyj', key: 'zyj' },
        { title: '佣金比例', dataIndex: 'yjbl', key: 'yjbl' },
        { title: '所属部门', dataIndex: 'sszz', key: 'sszz' },
        { title: '录入人', dataIndex: 'lrrUserName', key: 'lrrUserName' },
        { title: '成交人', dataIndex: 'cjrUserName', key: 'cjrUserName' },
        { title: '进行的申请', dataIndex: 'jxdsq', key: 'jxdsq' },
        { title: '审批状态', dataIndex: 'examineStatus', key: 'examineStatus' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Popconfirm title="请确认是否要作废成交报告?" onConfirm={this.zfconfirm} onCancel={this.zfcancel} okText="确认" cancelText="取消">
                        <Button type='primary' shape='circle' size='small' icon='team' />
                    </Popconfirm>
                    <Tooltip title='收款'>
                        <Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleSKClick(recored)} />
                    </Tooltip>
                    <Tooltip title='付款'>
                    <Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleFKClick(recored)} />
                    </Tooltip>
                    <Tooltip title='调佣'>
                    <Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleTYClick(recored)} />
                    </Tooltip>
                    <Tooltip title='转移'>
                        <Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleZYClick(recored)} />
                    </Tooltip>
                    <Popconfirm title="请确认xxx报告的结佣资料是否已经收齐?" onConfirm={this.jyconfirm} onCancel={this.jycancel} okText="确认" cancelText="取消">
                        <Button type='primary' shape='circle' size='small' icon='team' />
                    </Popconfirm>
                </span>
            )
        }
    ];
    jyconfirm = (e) => {

    }
    jycancel = (e) => {

    }
    zfconfirm = (e)=>{

    }
    zfcancel = (e)=>{

    }
    handleSKClick=(e)=>{
        if(this.props.onOpenDlg!==null){
            e.type = 'sk'
            this.props.onOpenDlg(e)
        }
    }
    handleFKClick=(e)=>{
        if(this.props.onOpenDlg!==null){
            e.type = 'fk'
            this.props.onOpenDlg(e)
        }
    }
    handleTYClick=(e)=>{
        if(this.props.onOpenTy!==null){
            this.props.onOpenTy(e)
        }
    }
    handleZYClick=(e)=>{
        if(this.props.onOpenZy!==null){
            this.props.onOpenZy(e)
        }
    }
    handleSearch = (e, type) => {
        console.log(e)
        console.log("查询条件", e);
        this.setState({ type: type })
        this.setState({ isDataLoading: true });
        if (type === 'myget') {
            this.props.dispatch(myReportGet(e));
        }
        else {
            this.props.dispatch(searchReport(e));
        }
    }
    handleMySearch = () => {
        this.setState({ isDataLoading: true });
        this.props.dispatch(myReportGet(this.props.SearchCondition));
    }
    handleTableChange = (pagination, filters, sorter) => {
        console.log(pagination, filters, sorter)
        let cd = this.props.SearchCondition;
        cd.pageIndex = (pagination.current - 1);
        cd.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.handleSearch(cd, this.state.type);
    };
    componentDidMount = () => {
        this.props.onRpTable(this)
        if (JSON.stringify(this.props.SearchCondition) !== "{}") {
            this.handleSearch(this.props.SearchCondition, 'myget');
        }
    }
    componentWillReceiveProps = (newProps) => {
        console.log("new Props:" + newProps.rpSearchResult)
        this.setState({ isDataLoading: false });

        let paginationInfo = {
            pageSize: newProps.rpSearchResult.pageSize,
            current: newProps.rpSearchResult.pageIndex,
            total: newProps.rpSearchResult.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });

    }
    render() {
        return (
            <Spin spinning={this.state.isDataLoading}>
                <Table pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.rpSearchResult.extension} onChange={this.handleTableChange} bordered size="middle"></Table>
            </Spin>
        )
    }
}
function MapStateToProps(state) {

    return {
        rpSearchResult: state.rp.rpSearchResult
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(DealRpTable);