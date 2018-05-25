import {connect} from 'react-redux';
import {setLoadingVisible, selBlackList} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Table} from 'antd';

const columns = [
    {title: '组织(分公司)',dataIndex: 'org',key: 'org',},
    {title: '职位',dataIndex: 'station',key: 'station'},
    {title: '基本工资',dataIndex: 'baseSalary',key: 'baseSalary'},
    {title: '岗位补贴',dataIndex: 'subsidy',key: 'subsidy'},
    {title: '工装扣款',dataIndex: 'clothesBack',key: 'clothesBack'},
    {title: '行政扣款',dataIndex: 'administrativeBack',key: 'administrativeBack'},
    {title: '端口扣款',dataIndex: 'portBack',key: 'portBack'},
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
                {<p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{this.props.searchInfoResult.selAchievementList.length}</b>条费用信息</p>}
                <div id="searchResult">
                    <Table id= {"table"} rowKey={record => record.idcard} 
                    columns={columns} 
                    pagination={this.props.searchInfoResult} 
                    onChange={this.handleChangePage} 
                    dataSource={this.props.searchInfoResult.selAchievementList} bordered size="middle" 
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