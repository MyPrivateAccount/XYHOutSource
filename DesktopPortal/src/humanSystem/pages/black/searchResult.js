import {connect} from 'react-redux';
import {setSearchLoadingVisible, selBlackList, getBlackList} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Table} from 'antd';

const columns = [
    {title: '身份证',dataIndex: 'idCard',key: 'idCard',},
    {title: '姓名',dataIndex: 'name',key: 'name'},];


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
        this.props.dispatch(setSearchLoadingVisible(true));
        this.props.dispatch(getBlackList(this.props.searchInfoResult));
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
                self.props.dispatch(selBlackList(selectedRows));
            }
        };
        
        return (
            <div>
                {<p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{this.props.searchInfoResult.blackList.extension.length}</b>条费用信息</p>}
                <div id="searchResult">
                    <Table id= {"table"} rowKey={record => record.key} 
                    columns={columns} 
                    pagination={this.props.searchInfoResult} 
                    onChange={this.handleChangePage} 
                    dataSource={this.props.searchInfoResult.blackList.extension} bordered size="middle" 
                    rowSelection={rowSelection} />
                </div>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchInfoResult: state.search,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchResult);