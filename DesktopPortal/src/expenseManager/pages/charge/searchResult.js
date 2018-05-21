import {connect} from 'react-redux';
import {setLoadingVisible,selCharge,clearCharge, /*openComplement, searchStart, saveSearchCondition, 
openAttachMent, openContractRecord, gotoThisContract, openContractRecordNavigator, getAllExportData, endExportAllData*/} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Table} from 'antd';

const columns = [
    {title: 'ID',dataIndex: 'id',key: 'id',},
    {title: '费用类别',dataIndex: 'chargetype',key: 'chargetype'},
    {title: '报销门店',dataIndex: 'organize',key: 'organize',}];

const rowSelection = {
    onChange: (selectedRowKeys, selectedRows) => {
        console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
    },
    getCheckboxProps: record => ({
        disabled: record.name === 'Disabled User', // Column configuration not to be checked
        name: record.name,
    }),
}

class SearchResult extends Component {

    componentWillMount() {
        
    }

    componentWillUnmount() {
        //this.props.dispatch(clearCharge());
    }
    
    handleChangePage = (pagination) => {

    }

    render() {
        let self = this;
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                self.props.dispatch(selCharge(selectedRows));
            }
        };
        
        return (
            <div id="searchResult">
                <Table id= {"table"} rowKey={record => record.id} 
                 columns={columns} 
                 pagination={this.props.searchInfoResult} 
                 onChange={this.handleChangePage} 
                 dataSource={this.props.searchInfoResult.extension} bordered size="middle" 
                 rowSelection={rowSelection} />
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchInfoResult: state.search.searchResult,
        showLoading: state.basicData.showLoading
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchResult);