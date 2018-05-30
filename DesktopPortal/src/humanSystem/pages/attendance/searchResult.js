import {connect} from 'react-redux';
import {setLoadingVisible, selBlackList} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Table} from 'antd';

//{key: "1", time: "tt", name: "tt", idcard: "tta", signed: "today"}
const columns = [
    {title: '日期',dataIndex: 'time',key: 'time',},
    {title: '姓名',dataIndex: 'name',key: 'name'},
    {title: '身份证号',dataIndex: 'idcard',key: 'idcard'},
    {title: '签到记录',dataIndex: 'signed',key: 'signed'},
];


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
                self.props.dispatch(selBlackList(selectedRows));
            }
        };
        
        return (
            <div>
                {<p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{this.props.searchInfoResult.attendanceList.extension.length}</b>条考勤信息</p>}
                <div id="searchResult">
                    <Table id= {"table"} rowKey={record => record.idcard} 
                    columns={columns} 
                    pagination={this.props.searchInfoResult} 
                    onChange={this.handleChangePage} 
                    dataSource={this.props.searchInfoResult.attendanceList.extension} bordered size="middle" 
                    rowSelection={rowSelection} />
                </div>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchInfoResult: state.search,
        showLoading: state.basicData.showLoading
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchResult);