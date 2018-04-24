import {connect} from 'react-redux';
//import {closeAdjustCustomer, getUserByOrg, adjustCustomer, setLoadingVisible} from '../../actions/actionCreator';
import {  searchStart, saveSearchCondition, setLoadingVisible,closeContractChoose,openAttachMent, openContractRecord} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Row, Col, Modal, Select, TreeSelect, Form, Table,Button, Layout, Spin, Input} from 'antd';

import moment from 'moment';
import SearchCondition from '../../constants/searchCondition';
const Option = Select.Option;
const FormItem = Form.Item;

class ContractChoose extends Component{

    componentWillMount(){
        if(this.props.isShowCompanyADialog)
        {
            this.handleSearch();
        }
    }
    state = {
        pagination: {
            pageSize: 5,
            current: 0,
            total: 0
        },
        condition: {
            keyWord: '',
            orderRule: 0,
            pageIndex: 0,
            pageSize: 5,
            type:'dialog',
        },
        checkList: [],
        curSelectRecord:{},
        curSelectIndex:null
    }
    getCurChoose = (isConfirm) =>{
        return  isConfirm ? this.state.curSelectRecord : null;
    }
    handleOk = (e) => {
        e.preventDefault();
        this.props.dispatch(closeContractChoose({}));
     
        if(this.props.companyADialogCallback){
    
            this.props.companyADialogCallback(this.getCurChoose(true));
        }
        //this.props.dispatch(setLoadingVisible(true));
        
        //this.props.dispatch(adjustCustomer(requestInfo));
    }
    componentWillReceiveProps(newProps, prevProps) {

        this.setState({ dataLoading: false });
        let paginationInfo = {
            pageSize: newProps.allContractData.pageSize,
            current: newProps.allContractData.pageIndex,
            total: newProps.allContractData.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });

    }
    handleSearch = (e) => {
        SearchCondition.contractSearchCondition = this.state.condition;
        SearchCondition.contractSearchCondition.pageIndex = 0;
        SearchCondition.contractSearchCondition.pageSize = 5;
        console.log("查询条件", SearchCondition);
        this.setState({ dataLoading: true });
        this.props.dispatch(searchStart(SearchCondition.contractSearchCondition));
    }

    handleTableChange = (pagination, filters, sorter) => {
        SearchCondition.contractSearchCondition.pageIndex = (pagination.current - 1);
        SearchCondition.contractSearchCondition.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.props.dispatch(searchStart(SearchCondition.contractSearchCondition));
    };
    

    handleCancel = () => {
        this.setState({curSelectRecord: {}});

        let condition = { ...this.state.condition };
        condition.keyWord = '';
        this.setState({ condition: condition });
 
        this.props.dispatch(closeContractChoose());
        if(this.props.handleChooseContract){
    
            this.props.handleChooseContract(this.getCurChoose(false));
        }
    }
    getTableColumns = () =>{
        let columns = [
            {
                title: '合同编号',
                // width: 80,
                dataIndex: 'num',
                key: 'num',

            },
            {
                title: '合同名称',
                // width: 80,
                dataIndex: 'name',
                key: 'name',
                
            },
            {
                title: '合同开始时间',
                // width: 80,
                dataIndex: 'startTime',
                key: 'startTime',
                render:  (text, record) => {
        
                    let newText = text;
                    if (text) {
                        newText = moment(text).format('YYYY-MM-DD');
                    }
                    
                    return (<span>{newText}</span>);
                }
            },
            {
                title: '合同结束时间',
                // width: 80,
                dataIndex: 'endTime',
                key: 'endTime',
                render:  (text, record) => {
        
                    let newText = text;
                    if (text) {
                        newText = moment(text).format('YYYY-MM-DD');
                    }
                    
                    return (<span>{newText}</span>);
                }
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
    handleNameChange = (e) => {
        //console.log("输入内容：", e.target.value);
        let condition = { ...this.state.condition };
        condition.keyWord = e.target.value;
        this.setState({ condition: condition });
    }
    render(){
        let dataSource = this.props.searchInfo.searchResult.extension ;
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
            title="合同选择" style={{ top: 20 }} confirmLoading={this.props.showLoading} maskClosable={false} visible={this.props.contractChooseVisible} 
            onCancel={this.handleCancel}
            footer={[
                <Button key="back" type="default" size="large" onClick={this.handleCancel}>取消</Button>,
                <Button key="submit" type="primary" size="large" loading={this.state.loading} onClick={this.handleOk}>
                    确定
                </Button>,
            ]}>
            <Layout>
                {/* <Content >
                        <Row>
                           <Col style={{ margin: '7px 5px'}}>  */}
                                <div className="searchBox">
                                    <Row >
                                        <Col span={12}>
                                            <Input value={this.state.condition.keyWord} style={{ width: '100%', verticalAlign: 'left',  }} placeholder={"请输入合同名称或者编号"} onPressEnter={this.handleSearch} onChange={this.handleNameChange}  />
                                            <Button style={{position:'absolute', marginleft:'2px'}} type="primary" icon="search" onClick={this.handleSearch}>查询</Button>
                                         </Col>
                                    
                                    </Row>
                                </div>
                            {/* </Col>
                        </Row>
                </Content> */}
                
                <Spin spinning={this.state.dataLoading}>
                    {<Table rowKey={record => record.id} pagination={this.state.pagination} rowSelection={rowSelection}  columns={this.getTableColumns()} dataSource={this.props.allContractData.extension} onChange={this.handleTableChange} onRowClick={this.handleRowClick}/>}
                </Spin>
            </Layout>
        </Modal>

            // <Modal 
            //     title="合同选择" style={{ top: 20 }} confirmLoading={this.props.showLoading} className='contractChoose' maskClosable={false} visible={this.props.contractChooseVisible} 
            //     onCancel={this.handleCancel}
            //     footer={[
            //         <Button key="back" type="default" size="large" onClick={this.handleCancel}>取消</Button>,
            //         <Button key="submit" type="primary" size="large" loading={this.state.loading} onClick={this.handleOk}>
            //             确定
            //         </Button>,
            //     ]}
            // >
            //     <SearchBox condition={this.state.condition} willMountCallback={this.searchBoxWillMount}/>
            //     <Table rowKey={record => record.uid} columns={this.getTableColumns()} rowSelection={rowSelection} pagination={this.state.pagination} onChange={this.handleChangePage} dataSource={dataSource}  onRowClick={this.handleRowClick} />  
            // </Modal>
        )
    }
}

function mapStateToProps(state){
    return {
        searchInfo: state.search,
        showLoading: state.search.showLoading,
        contractChooseVisible: state.contractData.contractChooseVisible,
        allContractData: state.contractData.allContractData,
    };
}

function mapDispatchToProps(dispatch){
    return {
        dispatch,
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ContractChoose);