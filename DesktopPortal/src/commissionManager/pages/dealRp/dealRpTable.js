//成交报告表格
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button, Tooltip, Spin } from 'antd'
import { myReportGet,searchReport } from '../../actions/actionCreator'

class DealRpTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
            dataSource: [],
            pagination: {},
            isDataLoading: false,
            type:'myget'
        }
    }
    appTableColumns = [
        { title: '审批通过日期', dataIndex: 'createTime', key: 'createTime' },
        { title: '成交编号', dataIndex: 'cjbgbh', key: 'cjbgbh' },
        { title: '上业绩日期', dataIndex: 'cjrq', key: 'cjrq' },
        { title: '类型', dataIndex: 'jylx', key: 'jylx' },
        { title: '物业名称', dataIndex: 'wyMc', key: 'wyMc' },
        { title: '物业地址', dataIndex: 'wyCzwydz', key: 'wyCzwydz' },
        { title: '成交总价', dataIndex: 'cjzj', key: 'cjzj' },
        { title: '总佣金', dataIndex: 'zyj', key: 'zyj' },
        { title: '佣金比例', dataIndex: 'yjbl', key: 'yjbl' },
        { title: '所属部门', dataIndex: 'sszz', key: 'sszz' },
        { title: '录入人', dataIndex: 'lrr', key: 'lrr' },
        { title: '成交人', dataIndex: 'cjr', key: 'cjr' },
        { title: '进行的申请', dataIndex: 'jxdsq', key: 'jxdsq' },
        { title: '审批状态', dataIndex: 'examineStatus', key: 'examineStatus' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="作废">
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleDelClick(recored)} />
                    </Tooltip>
                    <Tooltip title='收款'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title='付款'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title='调佣'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title='转移'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title='结佣确认'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    handleSearch = (e,type) => {
        console.log(e)
        console.log("查询条件", e);
        this.setState({type:type})
        this.setState({ isDataLoading: true });
        if(type === 'myget'){
            this.props.dispatch(myReportGet(e));
        }
        else{
            this.props.dispatch(searchReport(e));
        }
    }
    handleTableChange = (pagination, filters, sorter) => {
        let cd = this.props.SearchCondition;
        cd.pageIndex = (pagination.current - 1);
        cd.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.handleSearch(this.state.type);
    };
    componentDidMount = () => {
        this.props.onRpTable(this)
        if (JSON.stringify(this.props.SearchCondition) !== "{}") {
            this.handleSearch(this.props.SearchCondition,'myget');
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
                <Table pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.rpSearchResult.extension} onChange={this.handleTableChange}></Table>
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