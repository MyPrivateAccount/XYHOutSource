import {connect} from 'react-redux';
import {setLoadingVisible, selCharge, clearCharge, adduserPage} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Table} from 'antd';


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
    constructor(pro) {
        super(pro);
        this.columns = [
            {title: 'ID',dataIndex: 'id',key: 'id',},
            {title: '创建时间',dataIndex: 'createtime',key: 'createtime'},
            {title: '创建用户',dataIndex: 'createname',key: 'createname'},
            {title: '报销门店',dataIndex: 'organize',key: 'organize',},
            {title: "是否付款", dataIndex: "ispayed", key: "ispayed"},
            {title: "操作", dataIndex: "operation", key: "operation",
            render: (text, record) => {
                return (
                    <span> <a onClick={() => this.show(record.key)}>显示详细</a> </span>
                );
              }
            },
        ];
    }

    show = () => {
        this.props.dispatch(adduserPage({menuID: 'chargedetailinfo', disname: '费用信息', type:'item'}));
    }

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
                 columns={this.columns} 
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