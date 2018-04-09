import {connect} from 'react-redux';
//import {closeAdjustCustomer, getUserByOrg, adjustCustomer, setLoadingVisible} from '../../actions/actionCreator';
import { getContractDetail, searchStart, saveSearchCondition, setLoadingVisible,closeContractChoose,openAttachMent, openContractRecord} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Row, Col, Modal, Select, TreeSelect, Form, Table,Button} from 'antd';
import SearchBox from '../searchBox';


const Option = Select.Option;
const FormItem = Form.Item;

class ContractChoose extends Component{
    lastSearchInfo = {};
    componentWillMount(){
        //这个地方需要对默认搜索进行处理
        this.lastSearchInfo = this.props.searchInfo;
    }
    state = {
        pagination: {
            pageSize: 5,
            current: 0,
            total: 0
        },
        condition: {
            keyWord: '',
            checkStatu: null,//审核状态
            //organizationName: [],//
            createDateStart: null,//录入时间
            createDateEnd: null,
            //IsExpire:false,
            discard:0,
            follow:0,
            orderRule: 0,
            pageIndex: 0,
            pageSize: 5
        },
        checkList: [],
        curSelectRecord:{},
        curSelectIndex:null
    }
    handleOk = (e) => {
        e.preventDefault();
        this.props.dispatch(closeContractChoose({record: this.state.checkList[0]}));
        this.props.dispatch(setLoadingVisible(true));
        
        //this.props.dispatch(adjustCustomer(requestInfo));
    }
    componentWillReceiveProps(newProps) {
        //console.log("newProps.searchInfo.searchResul", newProps.searchInfo.searchResult);
        let {pageIndex, pageSize, totalCount} = newProps.searchInfo.searchResult;
        if (newProps.searchInfo.searchResult && pageIndex) {
            this.setState({pagination: {current: pageIndex, pageSize: 5, total: totalCount}});
        }
    }
        //searchbox组件render前的回调
    searchBoxWillMount = (searchMethod) => {
        if (searchMethod) {
            //setState由于是异步函数因此当前设置的值未必可以马上生效故而后面的回调函数可以在设置成功后进行的操作
            this.setState({searchHandleMethod: searchMethod}, () => {this.handleSearch();});
        }
    }
    //查询处理
    handleSearch = () => {
        let searchMethod = this.state.searchHandleMethod;
        if (searchMethod) {
            searchMethod();
        }
    }
   
    handleCancel = () => {

        //this.props.dispatch(closeAdjustCustomer());
        this.props.dispatch(closeContractChoose());
    }
    getTableColumns = () =>{
        let columns = [
            {
                title: '合同编号',
                // width: 80,
                dataIndex: 'id',
                key: 'id',

            },
            {
                title: '合同名称',
                // width: 80,
                dataIndex: 'name',
                key: 'name',
                
            },
        ]
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
    handleRowClick = (record) =>{
        return{
            onClick: () =>{
                console.log("curSelectRecord:", record);
                this.setState({curSelectRecord: record});
            }
        }
    }
    render(){
        let dataSource = this.props.searchInfo.searchResult.extension;
        console.log("contractChooseVisible:", this.props.contractChooseVisible);
        const rowSelection = {
            type:"radio",
            onChange: (selectedRowKeys, selectedRows) => {
                console.log("selectedRowKeys:", selectedRows);
                this.setState({checkList: selectedRows});
            }
        };
        return(
            <Modal 
                title="合同选择" style={{ top: 20 }} confirmLoading={this.props.showLoading} className='contractChoose' maskClosable={false} visible={this.props.contractChooseVisible} 
                onCancel={this.handleCancel}
                footer={[
                    <Button key="back" type="default" size="large" onClick={this.handleCancel}>取消</Button>,
                    <Button key="submit" type="primary" size="large" loading={this.state.loading} onClick={this.handleOk}>
                        确定
                    </Button>,
                ]}
            >
                <SearchBox condition={this.state.condition} willMountCallback={this.searchBoxWillMount}/>
                <Table rowKey={record => record.uid} columns={this.getTableColumns()} rowSelection={rowSelection} pagination={this.state.pagination} onChange={this.handleChangePage} dataSource={dataSource}  onRowClick={this.handleRowClick} />  
            </Modal>
        )
    }
}

function mapStateToProps(state){
    return {
        searchInfo: state.search,
        showLoading: state.search.showLoading,
        showContractShow:state.search.showContractShow,
        contractChooseVisible: state.contractData.contractChooseVisible,
        
    };
}

function mapDispatchToProps(dispatch){
    return {
        dispatch,
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ContractChoose);