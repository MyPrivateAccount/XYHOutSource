import {connect} from 'react-redux';
import {openAdjustCustomer, getCustomerDetail, searchStart, saveSearchCondition, setLoadingVisible} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Table} from 'antd';
import moment from 'moment';
import AdjustCustomer from './dialog/adjustCustomer';


class SearchResult extends Component {
    state = {
        pagination: {
            pageSize: 10,
            current: 0,
            total: 0
        },
        checkList: []//选中客户
    }
    //合同基本信息列
    getCustomerInfoColumns() {
        // const customerSource = this.props.basicData.customerSource;
        let columns = [{
            title: '合同名称',
            // width: 80,
            dataIndex: 'ContractName',
            key: 'ContractName'
        },
        {
            title: '合同类型',
            // width: 80,
            dataIndex: 'ContractType',
            key: 'ContractType'
        },
        {
            title: '申请时间',
            // width: 80,
            dataIndex: 'CreateTime',
            key: 'CreateTime'
        },
        {
            title: '申请部门',
            // width: 80,
            dataIndex: 'CreateDepartment',
            key: 'CreateDepartment'
        },
        {
            title: '项目类型',
            // width: 80,
            dataIndex: 'ProjectType',
            key: 'ProjectType'
        },
        {
            title: '项目名称',
            // width: 80,
            dataIndex: 'ProjectName',
            key: 'ProjectName'
        },
        {
            title: '项目负责人',
            // width: 80,
            dataIndex: 'ProprincipalPepole',
            key: 'ProprincipalPepole' 
        },
        {
            title: '甲方类型',
            // width: 80,
            dataIndex: 'CompanyAType',
            key: 'CompanyAType'
        },
        {
            title: '甲方公司全称',
            // width: 80,
            dataIndex: 'CompanyA',
            key: 'CompanyA'
        },
        {
            title: '甲方负责人',
            // width: 80,
            dataIndex: 'PrincipalpepoleA',
            key: 'PrincipalpepoleA'
        },
        {
            title: '乙方负责人',
            // width: 80,
            dataIndex: 'PrincipalpepoleB',
            key: 'PrincipalpepoleB'
        },
        {
            title: '合同开始时间',
            // width: 80,
            dataIndex: 'StartTime',
            key: 'StartTime'
        },
        {
            title: '合同结束时间',
            // width: 80,
            dataIndex: 'EndTime',
            key: 'EndTime'
        },
        {
            title: '份数',
            // width: 80,
            dataIndex: 'Count',
            key: 'Count'
        },
        {
            title: '返回原件',
            // width: 80,
            dataIndex: 'ReturnOrigin',
            key: 'ReturnOrigin'
        },
        {
            title: '佣金方式',
            // width: 80,
            dataIndex: 'CommisionType',
            key: 'CommisionType'
        },
        {
            title: '续签合同',
            // width: 80,
            dataIndex: 'Follow',
            key: 'Follow'
        },

        {
            title: '是否作废',
            // width: 80,
            dataIndex: 'IsCancel',
            key: 'IsCancel'
        },
        {
            title: '备注',
            // width: 80,
            dataIndex: 'Remark',
            key: 'Remark'
        },
            /*{
                title: '意向价格', width: 90,
                dataIndex: 'price',
                key: 'price',
                render: (text, record) => (
                    record.customerDemandResponse ? <span>¥{record.customerDemandResponse.priceStart}-{record.customerDemandResponse.priceEnd}万元</span> : null
                )
            },
            {
                title: '意向面积', width: 90,
                dataIndex: 'Acreage',
                key: 'Acreage',
                render: (text, record) => (
                    record.customerDemandResponse ? <span>{record.customerDemandResponse.acreageStart}-{record.customerDemandResponse.acreageEnd}㎡</span> : null
                )
            },
            {
                title: '意向区域', width: 90,
                dataIndex: 'customerDemandResponse.areaFullName',
                key: 'customerDemandResponse.areaFullName'
            },
            {
                title: '业态规划',
                dataIndex: 'tradplanning1',
                key: 'tradplanning1'
            }, {
                title: '经营品牌',
                dataIndex: 'brand1',
                key: 'brand1'
            }, {
                title: '经营类型',
                dataIndex: 'businessType1',
                key: 'businessType1'
            }, {
                title: '战略合作',
                dataIndex: 'cooperate1',
                key: 'cooperate1'
            },
            {
                title: '客户来源', width: 90,
                dataIndex: 'source',
                key: 'source',
                render: (text, record) => {
                    let sourceText = record.source;
                    let sourceInfo = customerSource.find(cl => cl.value === sourceText);
                    if (sourceInfo) {
                        sourceText = sourceInfo.key;
                    }
                    return (< span > {sourceText}</span >)
                }
            }, 
            {
                title: '录入时间', width: 90,
                dataIndex: 'createTime',
                key: 'createTime'
            }*/
        ];
        return columns;
    };
    //成交信息列
    getDealInfoColumns() {
        let columns = {
            title: '审核信息',
            children: [{        
                title: '审核人',
                // width: 80,
                dataIndex: 'CheckPeople',
                key: 'CheckPeople'
            },
            {
                title: '审核状态',
                // width: 80,
                dataIndex: 'CheckState',
                key: 'CheckState'
            }, /*{
                title: '提交审核时间',
                dataIndex: 'CheckTime',
                key: 'CheckTime'
            }, {
                title: '审核时间',
                dataIndex: 'CheckResTime',
                key: 'CheckResTime',
                render: this.dateTimeRender
            }*/]
        };
        return columns;
    }
    //其他信息列
    getOtherInfoColumns() {
        let columns = {
            title: '其他',
            children: [
                {
                    title: '附件上传',
                    dataIndex: 'AttachUpload',
                    key: 'AttachUpload',
                    render: (text, record) => (
                        <Button type="primary" size='small' onClick={(e) => this.handleCustomerDetail(record)}>附件上传</Button>
                    )
                },

                {
                    title: '合同详情',
                    dataIndex: 'ContractDetail',
                    key: 'ContractDetail',
                    render: (text, record) => (
                        <Button type="primary" size='small' onClick={(e) => this.handleCustomerDetail(record)}>合同详情</Button>
                    )
                },
                {
                    title: '导出',
                    dataIndex: 'Export',
                    key: 'Export',
                    render: (text, record) => (
                        <Button type="primary" size='small' onClick={(e) => this.handleCustomerDetail(record)}>导出</Button>
                    )
                },
            ]
        };
        // let activeMenu = this.props.searchInfo.activeMenu;
        // let rateProgress = this.props.basicData.rateProgress || [];
        // if (activeMenu === "menu_index") {
        //     let empColumns = [{
        //         title: '商机阶段',
        //         dataIndex: 'rateProgress',
        //         key: 'rateProgress',
        //         render: (text, record) => {
        //             let returnText = (text || '').toString();
        //             let progress = rateProgress.find(cl => cl.value === returnText);
        //             if (progress) {
        //                 returnText = progress.key;
        //             }
        //             return returnText;
        //         }
        //     }, {
        //         title: '跟进情况',
        //         dataIndex: 'followupTime',
        //         key: 'followupTime',
        //         render: (text, record) => {
        //             let days = 0;
        //             if (text) {
        //                 days = moment().diff(moment(text), 'days');
        //             }
        //             return (<span>{days === 0 ? "当天跟进" : days + "天未跟进"}</span>)
        //         }
        //     }, {
        //         title: '跟进总数',
        //         dataIndex: 'followUpNum',
        //         key: 'followUpNum'
        //     }, {
        //         title: '最近跟进房源',
        //         dataIndex: 'followHouse',
        //         key: 'followHouse'
        //     }, {
        //         title: '调客',
        //         dataIndex: 'adjustCustomer',
        //         key: 'adjustCustomer',
        //         render: (text, record) => (
        //             <Button type="primary" size='small' onClick={(e) => this.handleAdjustCustomer(record)}>调客</Button>
        //         )
        //     }];
        //     columns.children = [...empColumns, ...columns.children];
        // } else if (activeMenu === "menu_public_pool") {//公客池
        //     let publicPoolColumn = [{
        //         title: '转入公客时间',
        //         dataIndex: 'transferTime',
        //         key: 'transferTime'
        //     }];
        //     columns.children = [...publicPoolColumn, ...columns.children];
        // }
        // else if (activeMenu === "menu_have_deal") {//已成交
        //     let haveDealPoolColumn = [{
        //         title: '是否仍有购买意向',
        //         dataIndex: 'isSellIntention',
        //         key: 'isSellIntention',
        //         render: (text, record) => {
        //             return (<span>{text ? "有" : "无"}</span>)
        //         }
        //     }];
        //     columns.children = [...haveDealPoolColumn, ...columns.children];
        // }
        // else if (activeMenu === "menu_invalid") {//已失效
        //     let invalidColumn = [{
        //         title: '无效类型',
        //         dataIndex: 'customerLossResponse.lossTypeId',
        //         key: 'customerLossResponse.lossTypeId',
        //         render: (text, record) => {
        //             let newText = text;
        //             let findResult = this.props.basicData.invalidResions.find(r => r.value == text);
        //             newText = findResult ? findResult.key : newText;
        //             return (
        //                 <span>{newText}</span>
        //             )
        //         }
        //     }, {
        //         title: '无效原因',
        //         dataIndex: 'customerLossResponse.lossRemark',
        //         key: 'customerLossResponse.lossRemark'
        //     }, {
        //         title: '无效时间',
        //         dataIndex: 'customerLossResponse.lossTime',
        //         key: 'customerLossResponse.lossTime',
        //         render: this.dateTimeRender
        //     }];
        //     columns.children = [...invalidColumn, ...columns.children];
        // }
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
        console.log("basicData:", this.props.basicData)
        // const customerLevels = this.props.basicData.customerLevels;
        // const requirementLevels = this.props.basicData.requirementLevels;
        let basicIinfoColumns = this.getCustomerInfoColumns();
        let columns = [
            {
                title: '合同基本信息',
                children: basicIinfoColumns,
                // fixed: 'left'
            },
            // {
            //     title: '客户等级',
            //     children: [{
            //         title: '客户等级',
            //         width: 90,
            //         dataIndex: 'customerDemandResponse.importance',
            //         key: 'customerDemandResponse.importance',
            //         render: (text, record) => {
            //             let level = text;
            //             let levelInfo = customerLevels.find(cl => cl.value === level.toString());
            //             if (levelInfo) {
            //                 level = levelInfo.key;
            //             }
            //             return (< span > {level}</span >)
            //         }
            //     }, {
            //         title: '需求等级',
            //         width: 90,
            //         dataIndex: 'customerDemandResponse.demandLevel',
            //         key: 'customerDemandResponse.demandLevel',
            //         render: (text, record) => {
            //             let level = text;
            //             let levelInfo = requirementLevels.find(cl => cl.value === level.toString());
            //             if (levelInfo) {
            //                 level = levelInfo.key;
            //             }
            //             return (< span > {level}</span >)
            //         }
            //     }]
            // },
            // {
            //     title: '客户归属信息',
            //     children: [{
            //         title: '归属部门',
            //         // width: 90,
            //         dataIndex: 'departmentName',
            //         key: 'departmentName'
            //     }, {
            //         title: '业务员',
            //         // width: 90,
            //         dataIndex: 'userName',
            //         key: 'userName'
            //     }]
            // }
        ];
        let activeMenu = this.props.searchInfo.activeMenu;
        if (activeMenu === "menu_index" || activeMenu === "menu_have_deal") {
            columns.push(this.getDealInfoColumns());
        }
        columns.push(this.getOtherInfoColumns());
        console.log('columns:', columns);
        return columns;
    }

    componentWillReceiveProps(newProps) {
        //console.log("newProps.searchInfo.searchResul", newProps.searchInfo.searchResult);
        let {pageIndex, pageSize, totalCount} = newProps.searchInfo.searchResult;
        if (newProps.searchInfo.searchResult && pageIndex) {
            this.setState({pagination: {current: pageIndex, pageSize: pageSize, total: totalCount}});
        }
    }

    handleChangePage = (pagination) => {
        console.log("分页信息:", pagination);
        let condition = {...this.props.searchInfo.searchCondition};
        condition.pageIndex = pagination.current - 1;
        condition.pageSize = pagination.pageSize;
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(saveSearchCondition(condition));
        this.props.dispatch(searchStart(condition));
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
    // getTableScrollX() {
    //     let tableScrollX = 1000;
    //     let activeMenu = this.props.searchInfo.activeMenu;
    //     if (activeMenu === "menu_index") {
    //         tableScrollX = 1000;
    //     }
    //     else if (activeMenu === "menu_public_pool") {
    //         tableScrollX = 1000;
    //     } else {
    //         tableScrollX = 1000;
    //     }
    //     return tableScrollX;
    // }
    render() {
        let dataSource = this.props.searchInfo.searchResult.extension;
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                this.setState({checkList: selectedRows});
            }
        };
        let showSlection = false;
        if (["menu_index"].includes(this.props.searchInfo.activeMenu)) {
            showSlection = true;
        }
        // const tableScrollX = this.getTableScrollX();
        return (
            <div id="searchResult">
                <Table columns={this.getTableColumns()} pagination={this.state.pagination} onChange={this.handleChangePage} dataSource={dataSource} bordered size="middle" rowSelection={showSlection ? rowSelection : null} />
              

            </div>
        )
    }

}

function mapStateToProps(state) {
    return {
        searchInfo: state.search,
        basicData: state.basicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchResult);