import {connect} from 'react-redux';
//import {closeAdjustCustomer, getUserByOrg, adjustCustomer, setLoadingVisible} from '../../actions/actionCreator';
import { getContractDetail, searchStart, saveSearchCondition, setLoadingVisible,closeCompanyADialog, companyListGet} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Row, Col, Modal, Select, TreeSelect, Form, Table,Button, Spin,Layout, Input, Checkbox} from 'antd';
import SearchBox from '../searchBox';
import SearchCondition from '../../constants/searchCondition';
import '../search.less'
const Option = Select.Option;
const FormItem = Form.Item;
const { Header, Content } = Layout;
class CompanyAChoose extends Component{

    componentWillMount(){
        if(this.props.isShowCompanyADialog)
        {
            this.handleSearch();
        }
     
    }
    state = {
        checkList: [],
        condition: { isSearch: true, keyWord: '', searchType: '', address: '', type:'dialog', pageSize:5 },//条件
        dataLoading: false,
        curSelectRecord: {},
        pagination: {},
    }
    handleCheckChange = (e, Info) => {
        //console.log("checkbox change：" + JSON.stringify(e.target.checked));
        //console.log('Info:', Info);
        let compayAId = Info.id;
        let checked = e.target.checked;
      
        let checkList = this.state.checkList.slice();
        var hasValue = false;
        for (var i in checkList) {
            if (checkList[i].id == compayAId) {
                checkList[i].status = checked;
                hasValue = true;
                //break;
            }
            else
            {
                checkList[i].status = false;
            }
        }
        if (!hasValue) {
            checkList.push({ id: compayAId, status: checked });
        }
        checkList.push({ id: compayAId, status: checked });
        this.setState({ checkList: [{ id: compayAId, status: checked }] });
    }
    getCurChoose = (isConfirm) =>{
        return  isConfirm ? this.state.curSelectRecord : null;
    }
    handleOk = (e) => {
        e.preventDefault();
        this.props.dispatch(closeCompanyADialog({}));
     
        if(this.props.companyADialogCallback){
    
            this.props.companyADialogCallback(this.getCurChoose(true));
        }
        //this.props.dispatch(setLoadingVisible(true));
        
        //this.props.dispatch(adjustCustomer(requestInfo));
    }
    componentWillReceiveProps(newProps, prevProps) {
        this.setState({ dataLoading: false });
        let paginationInfo = {
            pageSize: newProps.allCompanyAData.pageSize,
            current: newProps.allCompanyAData.pageIndex,
            total: newProps.allCompanyAData.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });

    }
    handleSearch = (e) => {
        SearchCondition.companyASearchCondition = this.state.condition;
        SearchCondition.companyASearchCondition.pageIndex = 0;
        SearchCondition.companyASearchCondition.pageSize = 5;
        console.log("查询条件", SearchCondition);
        this.setState({ dataLoading: true });
        this.props.dispatch(companyListGet(SearchCondition.companyASearchCondition));
    }

    handleTableChange = (pagination, filters, sorter) => {
        SearchCondition.companyASearchCondition.pageIndex = (pagination.current - 1);
        SearchCondition.companyASearchCondition.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.props.dispatch(companyListGet(SearchCondition.companyASearchCondition));
    };
    

    handleCancel = () => {
        this.setState({curSelectRecord: {}});

        let condition = { ...this.state.condition };
        condition.keyWord = '';
        this.setState({ condition: condition });
 
        this.props.dispatch(closeCompanyADialog());
        if(this.props.companyADialogCallback){
    
            //this.props.companyADialogCallback(this.getCurChoose(false));
        }
    }
    getTableColumns = () =>{
        let columns = [
            // {
            //     title: '选择', dataIndex: 'id', key: 'id', render: (text, record) => (
            //         <span>
            //             <Checkbox defaultChecked={false} checked={record.status} onChange={(e) => this.handleCheckChange(e, record)} />
            //         </span>
            //     )
            // },
            {
                title: '甲方名称',
                // width: 80,
                dataIndex: 'name',
                key: 'name',

            },
            {
                title: '类型',
                // width: 80,
                dataIndex: 'type',
                key: 'type',
                render:  (text, record) =>{ 
                    let key = '';
                    this.props.basicData.firstPartyCatogories.forEach(item => {
                        if(item.value === text){
                            key = item.key;
                        }
                    });
                    return key; 
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

       // console.log("allCompanyAData:", this.props.allCompanyAData);
        const rowSelection = {
            type:"radio",
            onChange: (selectedRowKeys, selectedRows) => {
                console.log("selectedRowKeys:", selectedRows);
                //this.setState({checkList: selectedRows});
                this.setState({curSelectRecord: selectedRows[0]});
            }
        };
        return(
          
             <Modal 
                title="甲方选择" style={{ top: 20 }} confirmLoading={this.props.showLoading} maskClosable={false} visible={this.props.isShowCompanyADialog} 
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
                                                <Input value={this.state.condition.keyWord} style={{ width: '100%', verticalAlign: 'left',  }} placeholder={"请输入甲方名称"} onPressEnter={this.handleSearch} onChange={this.handleNameChange}  />
                                                <Button style={{position:'absolute', marginleft:'2px'}} type="primary" icon="search" onClick={this.handleSearch}>查询</Button>
                                             </Col>
                                        
                                        </Row>
                                    </div>
                                {/* </Col>
                            </Row>
                    </Content> */}
                    
                    <Spin spinning={this.state.dataLoading}>
                        {<Table rowKey={record => record.id} pagination={this.state.pagination} rowSelection={rowSelection}  columns={this.getTableColumns()} dataSource={this.props.allCompanyAData.extension} onChange={this.handleTableChange} onRowClick={this.handleRowClick}/>}
                    </Spin>
                </Layout>
            </Modal>
        )
    }
}

function mapStateToProps(state){
    return {
        allCompanyAData: state.companyAData.allCompanyAData,
        isShowCompanyADialog: state.companyAData.isShowCompanyADialog,
        basicData:state.basicData,
    };
}

function mapDispatchToProps(dispatch){
return {
    dispatch,
};
}

export default connect(mapStateToProps, mapDispatchToProps)(CompanyAChoose);