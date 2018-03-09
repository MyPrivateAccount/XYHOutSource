import {connect} from 'react-redux';
//import {closeAdjustCustomer, getUserByOrg, adjustCustomer, setLoadingVisible} from '../../actions/actionCreator';
import { getContractDetail, searchStart, saveSearchCondition, setLoadingVisible,closeContractChoose,openAttachMent, openContractRecord} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Row, Col, Modal, Select, TreeSelect, Form, Table} from 'antd';
import SearchBox from '../searchBox';


const Option = Select.Option;
const FormItem = Form.Item;

class ContractChoose extends Component{
    lastSearchInfo = {};
    componentWillMount(){
        console.log("-----------------");
        //这个地方需要对默认搜索进行处理
        this.lastSearchInfo = this.props.searchInfo;
    }
    state = {
        pagination: {
            pageSize: 10,
            current: 0,
            total: 0
        },
        checkList: [],
        curSelectRecord:{}
    }
    handleOk = (e) => {
        e.preventDefault();
        this.props.dispatch(closeContractChoose());
        this.props.dispatch(setLoadingVisible(true));
        
        //this.props.dispatch(adjustCustomer(requestInfo));
    }
    componentWillReceiveProps(newProps) {
        //console.log("newProps.searchInfo.searchResul", newProps.searchInfo.searchResult);
        let {pageIndex, pageSize, totalCount} = newProps.searchInfo.searchResult;
        if (newProps.searchInfo.searchResult && pageIndex) {
            this.setState({pagination: {current: pageIndex, pageSize: pageSize, total: totalCount}});
        }
    }
    handleCancel = () => {

        //this.props.dispatch(closeAdjustCustomer());
        this.props.dispatch(closeContractChoose());
    }
    getTableColumns = () =>{
        let columns = [
            {
                title: '合同类型',
                // width: 80,
                dataIndex: 'Number',
                key: 'Number'
            },
            {
                title: '合同名称',
                // width: 80,
                dataIndex: 'ContractName',
                key: 'ContractName'
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
            onChange: (selectedRowKeys, selectedRows) => {
                this.setState({checkList: selectedRows});
            }
        };
        return(
            <Modal title="合同选择" confirmLoading={this.props.showLoading} className='adjustCustomer' maskClosable={false} visible={this.props.contractChooseVisible} onOk={this.handleOk} onCancel={this.handleCancel}>
                <Row type= "flex">
                    <Col span={24}>
                        <SearchBox style={{marginRight: '10px', width:'300px'}}/>
                     </Col>
                </Row>
                
                <div id="searchResult">
                    <Table rowKey={record => record.uid} columns={this.getTableColumns()} pagination={this.state.pagination} onChange={this.handleChangePage} dataSource={dataSource} bordered size="small" onRowClick={this.handleRowClick} />
                </div>
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