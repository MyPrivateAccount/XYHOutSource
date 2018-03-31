import { connect } from 'react-redux';
import { setLoadingVisible, saveSearchCondition, searchCustomer } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Menu, Icon, Row, Col, Spin, Checkbox, Button } from 'antd';
import SearchCondition from './searchCondition';
import SearchResult from './searchResult';

class SearchIndex extends Component {
    state = {

    }
    componentWillMount() {
        this.props.dispatch(setLoadingVisible(true));
    }
   
    render() {
        return (
            <div id='customerManager'>
                <Spin spinning={this.props.showLoading}>
                    <SearchCondition />
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
export default connect(mapStateToProps, mapDispatchToProps)(SearchIndex);