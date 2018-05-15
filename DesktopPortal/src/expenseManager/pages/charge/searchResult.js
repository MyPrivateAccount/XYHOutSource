import {connect} from 'react-redux';
import {setLoadingVisible, /*openComplement, searchStart, saveSearchCondition, 
openAttachMent, openContractRecord, gotoThisContract, openContractRecordNavigator, getAllExportData, endExportAllData*/} from '../actions/actionCreator';
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

    componentWillReceiveProps(newProps) {
    }
    
    handleChangePage = (pagination) => {
    }

    render() {
        let dataSource = this.props.searchInfo.searchResult.extension ;
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                this.setState({checkList: selectedRows});
            }
        };
        let showSlection = false;
        if (["menu_index"].includes(this.props.searchInfo.activeMenu)) {
            showSlection = true;
        }
        return (
            <div id="searchResult">
                <Table id= {"table"} rowKey={record => record.key} 
                 columns={columns} 
                 pagination={this.props.searchInfo.searchResult} 
                 onChange={this.handleChangePage} 
                 dataSource={this.props.searchInfo.searchResult.extension} bordered size="middle" 
                 rowSelection={rowSelection} />
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchInfo: state.search,
        showLoading: state.basicData.showLoading
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchResult);