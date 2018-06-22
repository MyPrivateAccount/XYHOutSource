//选择成交报备列表页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Spin, Modal } from 'antd'
import { getTradeReg} from '../../../actions/actionCreator'

class TradeReportTable extends Component {
    appTableColumns = [
        { title: '业务员', dataIndex: 'userName', key: 'userName' },
        { title: '组织', dataIndex: 'departmentName', key: 'departmentName' },
        { title: '成交日期', dataIndex: 'createTime', key: 'createTime' },
        { title: '成交楼盘', dataIndex: 'buildingName', key: 'buildingName' },
        { title: '商铺编号', dataIndex: 'shopId', key: 'shopId' },
        { title: '成交客户', dataIndex: 'customerName', key: 'customerName' },
        { title: '成交佣金', dataIndex: 'commission', key: 'commission' },
        { title: '成交总价', dataIndex: 'totalPrice', key: 'totalPrice' }

    ];
    state = {
        isDataLoading: false,
        pagination: {},
        visible: false,
    }
    handleSearch = (e) => {
        this.setState({ isDataLoading: true });
        this.props.dispatch(getTradeReg())
    }
    handleTableChange = (pagination, filters, sorter) => {
        let cd = { pageIndex: 0, pageSize: 10 };
        cd.pageIndex = (pagination.current - 1);
        cd.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.handleSearch();
    };
    handleCancel = (e) => {
        this.setState({ visible: false })
    }
    handleOk = (e) => {

    }
    show = (e) => {
        this.setState({ visible: true })
        this.handleSearch()
    }
    componentDidMount() {
        this.props.onSelf(this)
    }
    componentWillReceiveProps(newProps) {
        console.log("new Props:" + newProps.dataSource)
        this.setState({ isDataLoading: false });

        let paginationInfo = {
            pageSize: newProps.rpCJBBResult.pageSize,
            current: newProps.rpCJBBResult.pageIndex,
            total: newProps.rpCJBBResult.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });
    }
    handleDoubleClick=(e)=>{
        this.props.onHandleChooseCjbb(e)
        this.setState({ visible: false })
    }
    render() {
        return (
            <Modal width={900} title={'成交报备列表'} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Spin spinning={this.state.isDataLoading}>
                    <Table onRowDoubleClick={this.handleDoubleClick} columns={this.appTableColumns} dataSource={this.props.rpCJBBResult.extension} pagination={this.state.pagination} onChange={this.handleTableChange} bordered={true}></Table>
                </Spin>
            </Modal>
        )
    }
}
function MapStateToProps(state) {

    return {
        operInfo:state.rp.operInfo,
        rpCJBBResult: state.rp.rpCJBBResult,
        searchCondition: state.rp.searchCondition
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(TradeReportTable);