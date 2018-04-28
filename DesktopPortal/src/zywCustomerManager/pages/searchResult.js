import {connect} from 'react-redux';
import {
    openAdjustCustomer, getCustomerDetail, searchCustomer, showCustomerlossModal, customerlossActive,
    saveSearchCondition, setLoadingVisible, getCustomerByGroup, getRepeatGroup
} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Pagination, Table, Collapse} from 'antd';
import moment from 'moment';
import AdjustCustomer from './dialog/adjustCustomer';
import CustomerLoss from './dialog/customerloss';
import {formatPhoneNo} from '../../utils/utils'
import {TAG_TEXT_COLOR} from '../../constants/uiColor';
import {getDicPars} from '../../utils/utils';


const Panel = Collapse.Panel;
class SearchResult extends Component {
    state = {
        pagination: {
            pageSize: 1000,
            current: 0,
            total: 0
        },
        repeatPagination: {
            pageSize: 1000,
            current: 0,
            total: 0
        },
        curPanelKey: '',
        checkList: [],//选中客户
        selectedRowKeys: [],
        checkAll: false,//全选
        isOnlyRepeat: false,
    }
    //客户基本信息列
    getCustomerInfoColumns() {
        let columns = [{
            title: '姓名',
            // width: 80,
            dataIndex: 'customerName',
            key: 'customerName'
        },
        {
            title: '电话',
            // width: 80,
            dataIndex: 'mainPhone',
            key: 'mainPhone',
            render: (text, record) => {
                if (!(text || '').includes('*')) {
                    text = formatPhoneNo(text);
                }
                return text;
            }
        },
        ];
        return columns;
    };
    //成交信息列
    getDealInfoColumns() {
        let columns = {
            title: '客户成交信息',
            children: [{
                title: '成交房源',
                dataIndex: 'customerDealResponse.buildingName',
                key: 'customerDealResponse.buildingName',
                render: (text, record) => (<div>{(record.customerDealResponse || {}).buildingName}  {(record.customerDealResponse || {}).shopName}</div>)
            }, {
                title: '成交金额(元/月)',
                dataIndex: 'customerDealResponse.totalPrice',
                key: 'customerDealResponse.totalPrice'
            }, {
                title: '成交佣金',
                dataIndex: 'customerDealResponse.commission',
                key: 'customerDealResponse.commission'
            }, {
                title: '成交时间',
                dataIndex: 'customerDealResponse.createTime',
                key: 'customerDealResponse.createTime',
                render: this.dateTimeRender
            }]
        };
        return columns;
    }
    //其他信息列
    getOtherInfoColumns() {
        let columns = {
            title: '其他',
            children: [
                {
                    title: '客户详情',
                    dataIndex: 'customerDetail',
                    key: 'customerDetail',
                    render: (text, record) => (
                        <Button type="primary" size='small' onClick={(e) => this.handleCustomerDetail(record)}>客户详情</Button>
                    )
                }
            ]
        };
        let activeMenu = this.props.searchInfo.activeMenu;
        let rateProgress = getDicPars("RATE_PROGRESS", this.props.rootBasicData.dicList);
        if (activeMenu === "menu_index") {
            let empColumns = [{
                title: '商机阶段',
                dataIndex: 'rateProgress',
                key: 'rateProgress',
                render: (text, record) => {
                    let returnText = (text || '').toString();
                    let progress = rateProgress.find(cl => cl.value === returnText);
                    if (progress) {
                        returnText = progress.key;
                    }
                    return returnText;
                }
            }, {
                title: '跟进情况',
                dataIndex: 'followupTime',
                key: 'followupTime',
                render: (text, record) => {
                    let days = 0;
                    if (text) {
                        days = moment().diff(moment(text), 'days');
                    }
                    return (<span>{days === 0 ? "当天跟进" : days + "天未跟进"}</span>)
                }
            }, {
                title: '跟进总数',
                dataIndex: 'followUpNum',
                key: 'followUpNum'
            },
            {
                title: '调客',
                dataIndex: 'adjustCustomer',
                key: 'adjustCustomer',
                render: (text, record) => (
                    <Button type="primary" disabled={record.customerStatus === 6} size='small' onClick={(e) => this.handleAdjustCustomer(record)}>调客</Button>
                )
            }, {
                title: '拉无效',
                dataIndex: 'customerloss',
                key: 'customerloss',
                render: (text, record) => (
                    <Button type="primary" size='small' onClick={(e) => this.handleCustomerloss(record)}>拉无效</Button>
                )
            }];
            columns.children = [...empColumns, ...columns.children];
        } else if (activeMenu === "menu_public_pool") {//公客池
            let publicPoolColumn = [{
                title: '转入公客时间',
                dataIndex: 'transferTime',
                key: 'transferTime'
            }];
            columns.children = [...publicPoolColumn, ...columns.children];
        }
        else if (activeMenu === "menu_have_deal") {//已成交
            let haveDealPoolColumn = [{
                title: '是否仍有租赁意向',
                dataIndex: 'isSellIntention',
                key: 'isSellIntention',
                render: (text, record) => {
                    return (<span>{text ? "有" : "无"}</span>)
                }
            }];
            columns.children = [...haveDealPoolColumn, ...columns.children];
        }
        else if (activeMenu === "menu_invalid") {//已失效
            let invalidColumn = [{
                title: '无效类型',
                dataIndex: 'customerLossResponse.lossTypeId',
                key: 'customerLossResponse.lossTypeId',
                render: (text, record) => {
                    let newText = text;
                    let findResult = getDicPars("ZYW_INVALID_REASON", this.props.rootBasicData.dicList).find(r => r.value == text);
                    newText = findResult ? findResult.key : newText;
                    return (
                        <span>{newText}</span>
                    )
                }
            }, {
                title: '无效原因',
                dataIndex: 'customerLossResponse.lossRemark',
                key: 'customerLossResponse.lossRemark'
            }, {
                title: '无效时间',
                dataIndex: 'customerLossResponse.lossTime',
                key: 'customerLossResponse.lossTime',
                render: this.dateTimeRender
            }, {
                title: '激活',
                dataIndex: 'customerlossActive',
                key: 'customerlossActive',
                render: (text, record) => (
                    <Button type="primary" size='small' onClick={(e) => this.handleCustomerlossActive(record)}>激活</Button>
                )
            }];
            columns.children = [...invalidColumn, ...columns.children];
        }
        return columns;
    }
    dateTimeRender = (text, record) => {
        let newText = text;
        if (text) {
            newText = moment(text).format('YYYY-MM-DD HH:mm:ss');
        }
        return (<span>{newText}</span>);
    }

    getTableColumns() {
        let basicIinfoColumns = this.getCustomerInfoColumns();
        let columns = [
            {
                title: '客户基本信息',
                children: basicIinfoColumns,
                // fixed: 'left'
            },
            {
                title: '客户归属信息',
                children: [{
                    title: '归属部门',
                    // width: 90,
                    dataIndex: 'departmentName',
                    key: 'departmentName'
                }, {
                    title: '业务员',
                    // width: 90,
                    dataIndex: 'userName',
                    key: 'userName'
                }]
            }];
        let activeMenu = this.props.searchInfo.activeMenu;
        if (activeMenu === "menu_have_deal") {
            columns.push(this.getDealInfoColumns());
        }
        columns.push(this.getOtherInfoColumns());
        return columns;
    }

    componentWillReceiveProps(newProps) {
        let {pageIndex, pageSize, totalCount} = newProps.searchInfo.searchResult;
        if (newProps.searchInfo.searchResult && pageIndex) {
            this.setState({pagination: {current: pageIndex, pageSize: pageSize, total: totalCount}});
        }
        //重客分页
        let repeatGroups = newProps.searchInfo.repeatGroups;
        if (newProps.searchInfo.searchResult && repeatGroups) {
            this.setState({repeatPagination: {current: repeatGroups.pageIndex, pageSize: repeatGroups.pageSize, total: repeatGroups.totalCount}});
        }
        let condition = {...newProps.searchInfo.searchCondition};
        let isOnlyRepeat = condition.isOnlyRepeat;
        if (isOnlyRepeat != this.state.isOnlyRepeat) {
            this.setState({isOnlyRepeat: isOnlyRepeat, checkList: [], selectedRowKeys: [], checkAll: false});
        }
    }

    handleChangePage = (pagination) => {
        // console.log("分页信息:", pagination);
        let condition = {...this.props.searchInfo.searchCondition};
        condition.pageIndex = pagination.current - 1;
        condition.pageSize = pagination.pageSize;
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(saveSearchCondition(condition));
        this.props.dispatch(searchCustomer(condition));
        this.setState({
            checkAll: false,
            checkList: [],
            selectedRowKeys: []
        })
    }
    handleRepeatChangePage = (page, pageSize) => {//处理重客翻页
        let pagination = {current: page, pageSize: pageSize};
        let condition = {...this.props.searchInfo.searchCondition};
        condition.pageIndex = pagination.current - 1;
        condition.pageSize = pagination.pageSize;
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getRepeatGroup(condition));
        this.setState({
            checkAll: false,
            checkList: [],
            selectedRowKeys: []
        })
    }
    //查看客户详情
    handleCustomerDetail = (record) => {
        this.props.dispatch(getCustomerDetail(record));
    }
    //调客
    handleAdjustCustomer = (record) => {
        //console.log("客户详情:", record);
        this.props.dispatch(openAdjustCustomer([record]));
    }
    //批量调客
    handleMultiAdjust = () => {
        this.props.dispatch(openAdjustCustomer(this.state.checkList));
    }
    // 拉无效
    handleCustomerloss = (record) => {
        // console.log("客户详情:", record);
        record.type = 'list'
        this.props.dispatch(showCustomerlossModal(record));
    }
    // 激活客户
    handleCustomerlossActive = (record) => {
        this.props.dispatch(customerlossActive({
            customerId: record.id,
            isDeleteOldData: false,
            searchInfo: this.props.searchInfo,
            type: 'list'
        }));
    }
    //分组切换
    handleGroupChange = (e) => {
        // console.log("分组切换:", e);
        if (e && e.length > 0) {
            let latestKey = e[e.length - 1];
            if (this.state.curPanelKey != latestKey) {
                this.setState({curPanelKey: latestKey, checkList: [], selectedRowKeys: [], checkAll: false}, () => {
                    this.props.dispatch(setLoadingVisible(true));
                    this.props.dispatch(getCustomerByGroup(latestKey));
                });
            }
        } else {
            this.setState({curPanelKey: '', checkList: [], selectedRowKeys: [], checkAll: false});
        }
    }
    onSelectChange = (record, selected, selectedRows) => {
        let condition = {...this.props.searchInfo.searchCondition};
        let {checkList, selectedRowKeys} = this.state;
        let isOnlyRepeat = condition.isOnlyRepeat;
        console.log("选择变更:", record, selected, selectedRows);
        // if (!this.state.checkAll) {
        let dataSource = this.props.searchInfo.searchResult.extension;
        if (isOnlyRepeat) {
            dataSource = this.props.searchInfo.repeatCustomers || [];
        }
        if (record.customerStatus === 6) {
            return;
        }
        let rowIndex = dataSource.findIndex(x => x.id === record.id);
        if (selected) {
            checkList.push(record);
            selectedRowKeys.push(rowIndex);
        } else {
            let delIndex = checkList.findIndex(item => item.id === record.id);
            if (delIndex !== -1) {
                checkList.splice(delIndex, 1);
            }
            let keyIndex = selectedRowKeys.findIndex(item => item === rowIndex);
            if (keyIndex != -1) {
                selectedRowKeys.splice(keyIndex, 1);
            }
        }
        this.setState({checkList: checkList, selectedRowKeys: selectedRowKeys});
    }
    onSelectAll = (selected, selectedRows, changeRows) => {
        let condition = {...this.props.searchInfo.searchCondition};
        let isOnlyRepeat = condition.isOnlyRepeat;
        console.log("全选状态:", selected, selectedRows, changeRows);
        this.setState({checkAll: !this.state.checkAll}, () => {
            if (this.state.checkAll) {
                let dataSource = this.props.searchInfo.searchResult.extension;
                if (isOnlyRepeat) {
                    dataSource = this.props.searchInfo.repeatCustomers || [];
                }
                let newArr = selectedRows.filter(v => v.customerStatus !== 6)
                let indexArr = []
                newArr.map(v => {
                    let index = dataSource.findIndex(x => x.id === v.id)
                    indexArr.push(index)
                });
                this.setState({checkList: newArr, selectedRowKeys: indexArr});
            } else {
                this.setState({checkList: [], selectedRowKeys: []});
            }
        });
    }
    render() {
        let dataSource = this.props.searchInfo.searchResult.extension;
        let selectedRowKeys = this.state.selectedRowKeys
        const rowSelection = {
            selectedRowKeys,
            // onChange: this.onSelectChange,
            hideDefaultSelections: true,
            getCheckboxProps: (record) => ({defaultChecked: false}),
            onSelectAll: this.onSelectAll,
            onSelect: this.onSelectChange
        };
        let showSlection = false;
        if (["menu_index"].includes(this.props.searchInfo.activeMenu)) {
            showSlection = true;
        }
        let showPagination = false;
        if (this.props.searchInfo.searchResult.totalCount > 1000) {
            showPagination = true;
        } else {
            showPagination = (this.state.pagination.pageSize < 1000);
        }
        // const tableScrollX = this.getTableScrollX();
        let condition = {...this.props.searchInfo.searchCondition};
        let isOnlyRepeat = condition.isOnlyRepeat;
        const repeatGroups = this.props.searchInfo.repeatGroups || {};
        const curGroupCustomers = this.props.searchInfo.repeatCustomers || [];
        return (
            <div id="searchResult">
                {showSlection ? <Button type="primary" style={{marginBottom: '5px'}} disabled={this.state.checkList.length === 0} onClick={this.handleMultiAdjust}>批量调客</Button> : null}
                {
                    isOnlyRepeat ? <div>
                        <Collapse activeKey={this.state.curPanelKey} onChange={this.handleGroupChange}>
                            {
                                (repeatGroups.extension || []).map((group, i) => <Panel header={<div>{formatPhoneNo(group.phone)}<b style={{color: TAG_TEXT_COLOR}}>({group.count})</b></div>} key={group.phone}>
                                    {this.state.curPanelKey === group.phone ? <div>
                                        <Table columns={this.getTableColumns()} pagination={null} dataSource={curGroupCustomers} bordered size="middle" rowSelection={showSlection ? rowSelection : null} />
                                    </div> : null}
                                </Panel>)
                            }
                        </Collapse>
                        <p style={{textAlign: 'right', marginTop: '10px'}}>
                            <Pagination {...this.state.repeatPagination} onChange={this.handleRepeatChangePage} />
                        </p>
                    </div> : <Table columns={this.getTableColumns()} pagination={showPagination ? this.state.pagination : false} onChange={this.handleChangePage} dataSource={dataSource} bordered size="middle" rowSelection={showSlection ? rowSelection : null} />
                }
                <AdjustCustomer />
                <CustomerLoss searchInfo={this.props.searchInfo} />
            </div>
        )
    }

}

function mapStateToProps(state) {
    // console.log("wulalalal:::", state);
    return {
        searchInfo: state.search,
        rootBasicData: state.rootBasicData
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchResult);