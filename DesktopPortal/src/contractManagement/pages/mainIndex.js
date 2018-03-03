import { connect } from 'react-redux';
import { setLoadingVisible, saveSearchCondition, searchCustomer } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Menu, Icon, Row, Col, Spin, Checkbox, Button } from 'antd';
import SearchCondition from './searchCondition';
import SearchResult from './searchResult';

class MainIndex extends Component {
    state = {

    }
    componentWillMount() {
        //this.props.dispatch(setLoadingVisible(true));//后面打开
    }
   
    render() {
        let showLoading = false;//this.props.showLoading;
        return (
            <div id='contractManagement'>
                <Spin spinning={showLoading}>
                    <SearchCondition />
                 
                    <Button>录入</Button>
                    <Button>导出</Button>
                    <Button>附件上传</Button>


                    <SearchResult />
                </Spin>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchResult: state.search.searchResult,
        showLoading: state.search.showLoading,
        searchCondition: state.search.searchCondition,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);