import {connect} from 'react-redux';
import { openComplement, searchStart, saveSearchCondition, setLoadingVisible,openAttachMent, openContractRecord, gotoThisContract, openContractRecordNavigator} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Table} from 'antd';
import moment from 'moment';
import XLSX from 'xlsx';

const buttonDef = [
    { buttonID:"record", buttonName:"录入", icon:'', type:'primary', size:'small', requirePermission:['RECORD_FUC']},
    { buttonID:"uploadFile", buttonName:"附件上传", icon:'', type:'primary', size:'small',requirePermission:['UPLOAD_FILE']},
    { buttonID:"export", buttonName:"导出", icon:'', type:'primary', size:'small', requirePermission:['EXPORT_CONTRACT']},
   
];
class SearchResult extends Component {
    state = {
        pagination: {
            pageSize: 10,
            current: 0,
            total: 0
        },
        checkList: []//
    }
    
    componentWillReceiveProps(newProps) {
       /// console.log("newProps.searchInfo.searchResul", newProps.searchInfo.searchResult);
        let {pageIndex, pageSize, validityContractCount} = newProps.searchInfo.searchResult;
        console.log("validityContractCount:", validityContractCount);
        if (newProps.searchInfo.searchResult && pageIndex) {
            this.setState({pagination: {current: pageIndex, pageSize: pageSize, total: validityContractCount}});
        }
    }
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
     
            obj = Object.assign({}, {v:item.title, position: String.fromCharCode(colIndex) + row, key: item.key}) ;    
      
            newHeader.push(obj);
       
            if(item.children){

                this.getHeader(item.children, colIndex -1, row + 1, newHeader);
                colIndex = colIndex + item.children.length -1;
            }
            
        }
 
        return newHeader;
    }
    getContractInfoExportColumns() {
 
        let columns = [
            {
                title: '申请日期',
                // width: 80,
                dataIndex: 'createTime',
                key: 'createTime',
                render: this.dateTimeRender,
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
                dataIndex: 'projectName',
                key: 'projectName'
            },
            {
                title: '项目类型',
                // width: 80,
                dataIndex: 'projectType',
                key: 'projectType',
                render: this.getNameByType('projectType'),
            }, 
    
            {
                title: '合同名称',
                // width: 80,
                dataIndex: 'name',
                key: 'name'
            },
            {
                title: '合同类型',
                // width: 80,
                dataIndex: 'type',
                key: 'type',
                render: this.getNameByType('type'),
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
                title: '签订起始时间',
                dataIndex:'startTime',
                key:'startTime',
                render: this.dateTimeRender,
            },
            {
                title: '签订止时间',
                dataIndex:'endTime',
                key:'endTime',
                render: this.dateTimeRender,
            },
                
            {
                title: '包销方/开发商',
                dataIndex:'companyAT',
                key:'companyAT',
                render: this.getNameByType('companyAT'),
            },
            {
                title: '是否提供关系证明',
                dataIndex:'isSubmmitRelation',
                key:'isSubmmitRelation',
                render: this.isTrueOrFalse,
            },
            {
                title: '是否提供铺号(销控明细)',
                dataIndex:'isSubmmitShop',
                key:'isSubmmitShop',
                render: this.isTrueOrFalse,
            },
            {
                title: '佣金方案',
                dataIndex:'commisionType',
                key:'commisionType',
                render: this.getNameByType('commisionType'),
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

    getSingleItem = ( pos, key, value,isNum, num) =>{
        let obj = {};
        if(isNum){
            value = num;
        }
        else if(value === null){
            value = '';
        }else if(value === true){
            value = '是';
        }else if(value === false){
            value = '否';
        }else{
            if(key === 'type'){
                value = this.findKeyByValue(this.props.basicData.contractCategories, value);
            }else if(key === 'projectType'){
                value = this.findKeyByValue(this.props.basicData.contractProjectCatogories, value);
            }else if(key === 'companyAT'){
                value = this.findKeyByValue(this.props.basicData.firstPartyCatogories, value);
            }else if(key === 'commisionType'){
                value = this.findKeyByValue(this.props.basicData.commissionCatogories, value);
            }else if(key === 'startTime' || key === 'endTime'){
                let newText = value;
                if (value) {
                    newText = moment(value).format('YYYY-MM-DD');
                }
                
                value = newText;
            }
        }
        obj = Object.assign({}, {v:value,position: pos.substr(0, 1) + (num+1)}) ;  
        return obj;
    }
    getSingleExportData = (header, record, num = 1) =>{
        let isNum = false;
        let data = [];
        data = header.map((item, i) =>{
            item.key === 'num' ? isNum = true: isNum =false;
            return this.getSingleItem(item.position, item.key, record[item.key], isNum, num);
        })
        return data;
    }
    //导出
    onClickExPort = (record) =>{
        let header = [{v:"序号", position:'A1', key: 'num'}];
        let columns = this.getContractInfoExportColumns();
        let headers = header.concat(this.getHeader(columns));
        let newHeader = headers.reduce((prev, next) => Object.assign({}, prev, {[next.position]: {v: next.v}}), {});
        let data = this.getSingleExportData(headers, record);
        let newData = data.reduce((prev, next) => Object.assign({}, prev, {[next.position]: {v: next.v}}), {});
        console.log(headers,newHeader);
        console.log('newData:', newData);
        let output = Object.assign({}, newHeader, newData);
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
    onClickUploadFile = (record)=>{
        //console.log('====record:' , record);
        this.props.dispatch(openAttachMent({'record':record}));
    }

    onClickContractDetail = (record) =>{
        //console.log('record:', record);
        this.props.dispatch(gotoThisContract({'record':record}));
    }
    dateTimeRender = (text, record) => {
        
        let newText = text;
        if (text) {
            newText = moment(text).format('YYYY-MM-DD');
        }
        
        return (<span>{newText}</span>);
    }
    findKeyByValue = (dic, value) =>{
  
        let key = '';
        dic.forEach(item => {
            if(item.value === value){
                key = item.key;
            }
        });
        return key;
    }
    
    getNameByType = (curType) =>{

       return  (text, record) =>{
            if(curType === 'type'){
                return this.findKeyByValue(this.props.basicData.contractCategories, text);
            }else if(curType === 'projectType'){
                return this.findKeyByValue(this.props.basicData.contractProjectCatogories, text);
            }else if(curType === 'companyAT'){
                return this.findKeyByValue(this.props.basicData.firstPartyCatogories, text);
            }else if(curType === 'commisionType'){
                return this.findKeyByValue(this.props.basicData.commissionCatogories, text);
            }else{
                return '';
            }
        }
    }
    //合同基本信息列
    getContractInfoColumns() {
 
        let columns = [
            {
                title: '申请日期',
                // width: 80,
                dataIndex: 'createTime',
                key: 'createTime',
                render: this.dateTimeRender,
                sorter: (a, b)=>{
                    return (moment(a.createTime).isSameOrBefore(moment(b.createTime)) ? -1: 1);
                },
                //sortOrder:'ascend',
            },
            {
                title: '申请部门',
                // width: 80,
                dataIndex: 'organizate',
                key: 'organizate'
            },
            // {
            //     title: '申请人',
            //     dataIndex:'createUser',
            //     key: 'createUser',
            // },
            {
                title: '合同编号',
                dataIndex: 'num',
                key: 'num',
            },
            {
                title: '项目名称',
                // width: 80,
                dataIndex: 'projectName',
                key: 'projectName'
            },
            {
                title: '项目类型',
                // width: 80,
                dataIndex: 'projectType',
                key: 'projectType',
                render: this.getNameByType('projectType'),
            }, 
    
            {
                title: '合同名称',
                // width: 80,
                dataIndex: 'name',
                key: 'name'
            },
            {
                title: '合同类型',
                // width: 80,
                dataIndex: 'type',
                key: 'type',
                render: this.getNameByType('type'),
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
            // {
            //     title: '签约时间',
            //     children: [
            //         {
            //             title: '签订起始时间',
            //             dataIndex:'startTime',
            //             key:'startTime',
            //             render: this.dateTimeRender,
            //         },
            //         {
            //             title: '签订止时间',
            //             dataIndex:'endTime',
            //             key:'endTime',
            //             render: this.dateTimeRender,
            //         },
                    
            //     ],
            // },
            
            // {
            //     title: '包销方/开发商',
            //     children: [
            //         {
            //             title: '包销方/开发商',
            //             dataIndex:'companyAT',
            //             key:'companyAT',
            //             render: this.getNameByType('companyAT'),
            //         },
            //         {
            //             title: '是否提供关系证明',
            //             dataIndex:'isSubmmitRelation',
            //             key:'isSubmmitRelation',
            //             render: this.isTrueOrFalse,
            //         },
                    
            //     ],
            // },
            // {
            //     title: '是否提供铺号(销控明细)',
            //     dataIndex:'isSubmmitShop',
            //     key:'isSubmmitShop',
            //     render: this.isTrueOrFalse,
            // },
            // {
            //     title: '佣金方案',
            //     dataIndex:'commisionType',
            //     key:'commisionType',
            //     render: this.getNameByType('commisionType'),
            // },
            // {
            //     title: '结算方式',
            //     dataIndex:'settleaccounts',
            //     key:'settleaccounts',
            // },
            {
                title: '新签/续签',
                dataIndex:'isFollow',
                key:'isFollow',
                render:(text, record) =>{
                    return <span>{text ? '续签' : '新签'}</span>
                }
            },
            // {
            //     title: '备注',
            //     dataIndex:'remark',
            //     key:'remark',
            // },
        ];
        return columns;
    };
    isTrueOrFalse = (text, record) =>{
        return <span>{text ? '是' : '否'}</span>
    }
    getExamineValueByType = (type) =>{
        switch(type){
            case 0: return "未提交";
            case 1: return "审核中";
            case 8: return "审核通过";
            case 16: return "驳回";
        default:
            return type;
        }

    }
    getExamineInfoColumns() {
        let columns = {
            title: '审核信息',
            children: [
            //     {        
            //     title: '审核人',
            //     // width: 80,
            //     dataIndex: 'CheckPeople',
            //     key: 'CheckPeople'
            // },
            {
                title: '审核状态',
                // width: 80,
                dataIndex: 'examineStatus',
                key: 'examineStatus',
                render: (status) =>{
                    let newText = status;
                    newText = this.getExamineValueByType(status);
             
                    return <span>{newText}</span>
                }
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

    getPermissonCol = (button,i) =>{
        return {
            
                title: button.buttonName,
                dataIndex: button.buttonID,
                key: button.buttonID,
                render: (text, record) => (
                    
                    <Button key = {i} id= {button.buttonID}  icon={button.icon} size={button.size} type={button.type} onClick={button.buttonID === 'export' ? (e) => this.onClickExPort(record) : (e) => this.onClickUploadFile(record)}>{button.buttonName}</Button>
                )
            
        };
    }
    //其他信息列
    getOtherInfoColumns() {
        let columns ={
            title: '其他',
            children: [
                {
                    title: '合同详情',
                    dataIndex: 'contractDetail',
                    key: 'contractDetail',
                    render: (text, record) => (
                        <Button type="primary" size='small' onClick={(e) => this.onClickContractDetail(record)}>合同详情</Button>
                    )
                },
                {
                    title: '补充协议',
                    dataIndex: 'complement',
                    key: 'complement',
                    render: (text, record) => (
                        <Button type="primary" size='small' onClick={(e) => this.onClickComplement(record)}>补充协议</Button>
                    )
                },
            ]
        };
        buttonDef.map((button, i) =>{
            return this.hasPermission(button,i)&& button.buttonID != 'record' ? columns.children.push(this.getPermissonCol(button)) : null
        });
        //console.log('getOtherInfoColumns:', columns);
        return columns;
    }


    getTableColumns() {
        //console.log("basicData:", this.props.basicData)
        // const customerLevels = this.props.basicData.customerLevels;
        // const requirementLevels = this.props.basicData.requirementLevels;
        let columns = this.getContractInfoColumns();

        let activeMenu = this.props.searchInfo.activeMenu;
        if (activeMenu === "menu_index" || activeMenu === "menu_have_deal") {
            columns.push(this.getExamineInfoColumns());
        }
        columns.push(this.getOtherInfoColumns());
        //console.log('columns:', columns);
        return columns;
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
    // handleContractDetail = (record) => {
    //     this.props.dispatch(getContractDetail(record));
    // }
    handleMultiExport = () =>{
        let header = [{v:"序号", position:'A1', key: 'num'}];
        let columns = this.getContractInfoExportColumns();
        let headers = header.concat(this.getHeader(columns));
        let newHeader = headers.reduce((prev, next) => Object.assign({}, prev, {[next.position]: {v: next.v}}), {});
        let dataList = {};
        let newData = {};
        for(let i = 0; i < this.state.checkList.length ; i++)
        {
            let data = this.getSingleExportData(headers, this.state.checkList[i], i + 1);
            newData = data.reduce((prev, next) => Object.assign({}, prev, {[next.position]: {v: next.v}}), {});
            Object.assign(dataList, {...newData});
        }
        console.log('datalist:', dataList);
        let output = Object.assign({}, newHeader, dataList);
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
     onClickRecord = (e) =>{
         console.log('录入');
         this.props.dispatch(openContractRecord({record:null}));
         this.props.dispatch(openContractRecordNavigator({id:0}));
     }
     onClickComplement = (record) =>{
        this.props.dispatch(openComplement({record: record}));
     }
    handleClickFucButton = (buttonID) =>{
        switch(buttonID){
            case 'record':
                return this.onClickRecord;
            case 'export':
                return this.handleMultiExport;
            case 'uploadFile':
                return this.onClickUploadFile;
         
            default:
                return null;
        }
    }

    getMainButton = (button, i) =>{
        if(this.hasPermission(button)&& (button.buttonID !='uploadFile' )){
            if(button.buttonID ==='export' && this.state.checkList.length === 0){
                return null;
            }
            return <Button key = {i} id= {button.buttonID} style={{marginBottom: '10px', marginRight: '10px', border:0}}  onClick = {this.handleClickFucButton(button.buttonID)} icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button> ;
        }else{
            return null;
        }
    }
    render() {
        let dataSource = this.props.searchInfo.searchResult.extension ;//(this.props.searchInfo.searchResult.extension || []).sort((a, b) => {return (moment(a.createTime).isSameOrBefore(moment(b.createTime)) ? 1: -1) }) ;
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                console.log('checkList:', selectedRows);
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
                {    
                    buttonDef.map(
                        (button,i) => this.getMainButton(button,i))
                }
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