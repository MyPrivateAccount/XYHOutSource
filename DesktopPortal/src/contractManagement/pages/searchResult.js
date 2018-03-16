import {connect} from 'react-redux';
import { getContractDetail, searchStart, saveSearchCondition, setLoadingVisible,openAttachMent, openContractRecord} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Table} from 'antd';
import moment from 'moment';
import XLSX from 'xlsx';


class SearchResult extends Component {
    state = {
        pagination: {
            pageSize: 10,
            current: 0,
            total: 0
        },
        checkList: []//
    }

           //是否有权限
    hasPermission(buttonInfo) {
            let hasPermission = false;
            if (this.props.judgePermissions && buttonInfo.requirePermission) {
                for (let i = 0; i < buttonInfo.requirePermission.length; i++) {
                    if (this.props.judgePermissions.includes(buttonInfo.requirePermission[i])) {
                        hasPermission = true;
                        break;
                    }
                }
            } else {
                hasPermission = true;
            }
            return hasPermission;
    }
    getCurTitlePosition = (item,  colIndex, row= 1) =>{
        let obj = {};

        obj = Object.assign({}, {v:item.title, position: String.fromCharCode(colIndex) + row}) ;
        
        if(item.children){
            let nowRow = row + 1;
            for(let j =0 ; j < item.children.length; j ++){
                let itemCh = item.children[j];
               this.getCurTitlePosition(itemCh, colIndex + j, nowRow );
            }
            // item.children.map((itemCh, i) =>{   
            //     return this.getCurTitlePosition(itemCh, colIndex + i, nowRow );
             
            // })
           
        }
        console.log("obj:", obj);
        return obj;
    }
    getHeadersFromColums = () =>{
        let columns = this.getContractInfoColumns();
        let header = [{v:"序号", position:'A1'}];
        var colIndex = 65;

        let sheader = columns.map((item, i) => {
            colIndex = colIndex + 1;
            return this.getCurTitlePosition(item, colIndex);
        });
        
        return header.concat(sheader);
    }


    formatContractData = () =>{
        let dataSource = this.props.searchInfo.searchResult.extension;
        let baseInfo = dataSource.baseInfo;
        //
    }
    getHeader = (columns, colIndex = 65, row= 1, header=[]) =>{
       
        //let header = [{v:"序号", position:'A1'}];
   
        var newHeader = header;
        for(let i =0; i < columns.length; i ++){
            colIndex = colIndex + 1;
            let item = columns[i];
            var obj = {};
     
            obj = Object.assign({}, {v:item.title, position: String.fromCharCode(colIndex) + row}) ;    
      
            newHeader.push(obj);
       
            if(item.children){

                this.getHeader(item.children, colIndex -1, row + 1, newHeader);
                colIndex = colIndex + item.children.length -1;
            }
            
        }
 
        return newHeader;
    }
    //导出
    onClickExPort = (e) =>{
        //new TableExport(document.getElementsByTagName("table"));
        //let headers = this.getHeadersFromColums();
        let header = [{v:"序号", position:'A1'}];
        let columns = this.getContractInfoColumns();
        let headers = header.concat(this.getHeader(columns));
        let newHeader = headers.reduce((prev, next) => Object.assign({}, prev, {[next.position]: {v: next.v}}), {});
        console.log(headers,newHeader);
        let output = Object.assign({}, newHeader, {});
        // 获取所有单元格的位置
        let outputPos = Object.keys(output);
        // 计算出范围
        let ref = outputPos[0] + ':' + outputPos[outputPos.length - 1];
        // 构建 workbook 对象
        let wb = {
            SheetNames: ['mySheet'],
            Sheets: {
                'mySheet': Object.assign({}, output, { '!ref': ref })
            }
        };
        XLSX.writeFile(wb, 'output.xlsx');
    }
    //文件上传
    onClickUploadFile = (e)=>{
        this.props.dispatch(openAttachMent({id:1}));
    }

    onClickContractDetail = (e) =>{

    }
    //合同基本信息列
    getContractInfoColumns() {
        // const customerSource = this.props.basicData.customerSource;
        /*
        let columns = [
        {
            title: '申请日期',
            // width: 80,
            dataIndex: 'createTime',
            key: 'createTime'
        },
        {
            title: '部门',
            // width: 80,
            dataIndex: 'createDepartment',
            key: 'createDepartment'
        },
        {
            title: '申请人',
            dataIndex:'createUser',
            key: 'createUser',
        },
        {
            title: '合同编号',
            dataIndex: 'id',
            key: 'id',
        },
        {
            title: '项目名称',
            // width: 80,
            dataIndex: 'ProjectName',
            key: 'ProjectName'
        },
        {
            title: '项目类型',
            // width: 80,
            dataIndex: 'ProjectType',
            key: 'ProjectType'
        }, 

        {
            title: '合同名称',
            // width: 80,
            dataIndex: 'contractName',
            key: 'contractName'
        },
        {
            title: '合同类型',
            // width: 80,
            dataIndex: 'ContractType',
            key: 'ContractType'
        },
        {
            title: '开发商(甲方全称)',
            // width: 80,
            dataIndex: 'companyA',
            key: 'companyA'
        },
        {
            title: '甲方项目对接人',
            // width: 80,
            dataIndex: 'principalpepoleA',
            key: 'principalpepoleA'
        },




        {
            title: '项目类型',
            // width: 80,
            dataIndex: 'ProjectType',
            key: 'ProjectType'
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

        ];*/
        let columns = [
            {
                title: '申请日期',
                // width: 80,
                dataIndex: 'createTime',
                key: 'createTime'
            },
            {
                title: '部门',
                // width: 80,
                dataIndex: 'createDepartment',
                key: 'createDepartment'
            },
            {
                title: '申请人',
                dataIndex:'createUser',
                key: 'createUser',
            },
            {
                title: '合同编号',
                dataIndex: 'id',
                key: 'id',
            },
            {
                title: '项目名称',
                // width: 80,
                dataIndex: 'ProjectName',
                key: 'ProjectName'
            },
            {
                title: '项目类型',
                // width: 80,
                dataIndex: 'ProjectType',
                key: 'ProjectType'
            }, 
    
            {
                title: '合同名称',
                // width: 80,
                dataIndex: 'contractName',
                key: 'contractName'
            },
            {
                title: '合同类型',
                // width: 80,
                dataIndex: 'contractType',
                key: 'contractType'
            },
            {
                title: '开发商(甲方全称)',
                // width: 80,
                dataIndex: 'companyA',
                key: 'companyA'
            },
            {
                title: '甲方项目对接人',
                // width: 80,
                dataIndex: 'principalpepoleA',
                key: 'principalpepoleA'
            },
            {
                title: '签约时间',
                children: [
                    {
                        title: '签订起始时间',
                        dataIndex:'startTime',
                        key:'startTime',
                    },
                    {
                        title: '签订止时间',
                        dataIndex:'endTime',
                        key:'endTime',
                    },
                    
                ],
            },
            
            {
                title: '包销方/开发商',
                children: [
                    {
                        title: '包销方/开发商',
                        dataIndex:'companyAT',
                        key:'companyAT',
                    },
                    {
                        title: '是否提供关系证明',
                        dataIndex:'isSubmmitRelation',
                        key:'isSubmmitRelation',
                    },
                    
                ],
            },
            {
                title: '是否提供铺号(销控明细)',
                dataIndex:'isSubmmitShop',
                key:'isSubmmitShop',
            },
            {
                title: '佣金方案',
                dataIndex:'commission',
                key:'commission',
            },
            {
                title: '结算方式',
                dataIndex:'settleaccounts',
                key:'settleaccounts',
            },
            {
                title: '新签/续签',
                dataIndex:'follow',
                key:'follow',
            },
            {
                title: '备注',
                dataIndex:'remark',
                key:'remark',
            },
        ];
        return columns;
    };

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
                        <Button type="primary" size='small' onClick={(e) => this.onClickUploadFile(record)}>附件上传</Button>
                    )
                },

                {
                    title: '合同详情',
                    dataIndex: 'ContractDetail',
                    key: 'ContractDetail',
                    render: (text, record) => (
                        <Button type="primary" size='small' onClick={(e) => this.onClickContractDetail(record)}>合同详情</Button>
                    )
                },
                {
                    title: '导出',
                    dataIndex: 'Export',
                    key: 'Export',
                    render: (text, record) => (
                        <Button type="primary" size='small' onClick={(e) => this.onClickExPort(record)}>导出</Button>
                    )
                },
            ]
        };

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
        //console.log("basicData:", this.props.basicData)
        // const customerLevels = this.props.basicData.customerLevels;
        // const requirementLevels = this.props.basicData.requirementLevels;
        let columns = this.getContractInfoColumns();

        let activeMenu = this.props.searchInfo.activeMenu;
        if (activeMenu === "menu_index" || activeMenu === "menu_have_deal") {
            //columns.push(this.getDealInfoColumns());
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
    //查看合同详情
    handleContractDetail = (record) => {
        this.props.dispatch(getContractDetail(record));
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
                <Button onClick={this.onClickExPort}>{'测试'}</Button>
                <Table id= {"table"} rowKey={record => record.uid} columns={this.getTableColumns()} pagination={this.state.pagination} onChange={this.handleChangePage} dataSource={dataSource} bordered size="middle" rowSelection={showSlection ? rowSelection : null} />
              

            </div>
        )
    }

}

function mapStateToProps(state) {
    return {
        searchInfo: state.search,
        basicData: state.basicData,
        judgePermissions: state.judgePermissions
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchResult);