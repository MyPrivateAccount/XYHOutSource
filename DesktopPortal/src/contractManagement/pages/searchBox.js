import {connect} from 'react-redux';
import {changeKeyWord, searchCustomer, setLoadingVisible, saveSearchCondition} from '../actions/actionCreator';
import React, {Component} from 'react'
import {Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import './search.less'


class SearchBox extends Component{
    state = {
        searchType: '1'
    };
    componentWillMount(){
        //这个地方需要对默认搜索进行处理
    }

    componentWillReceiveProps(newProps){
        //更新某些属性
    }

    handleKeyChangeWord = (e) =>{
        //处理关键字改变-需要
    }
    handleSearch = () =>{
        //处理搜索逻辑
    }
    handleSearchTypeChange = (e) =>{
        this.setState({searchType : e});
    }
    render(){
        let keyword = '';//this.props.searchInfo.searchKeyWord;
        return (
            <div>
                <div className="searchBox">
                    <p>
                        <Input style ={{width: '450px'}} placeholder={this.state.searchType === '1' ? '请输入合同编号或者名称': ''} 
                         value = {keyword} onChange = {this.handleKeyChangeWord}/> 
                        <Button type='primary' className='searchButton' onClick={this.handleSearch}>查询</Button>
                    </p>
                </div>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchInfo: state.search
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchBox);