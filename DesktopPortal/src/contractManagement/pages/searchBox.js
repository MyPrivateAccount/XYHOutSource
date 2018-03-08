import {connect} from 'react-redux';
import {changeKeyWord, searchStart, setLoadingVisible, saveSearchCondition} from '../actions/actionCreator';
import React, {Component} from 'react'
import {Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import './search.less'


class SearchBox extends Component{
    state = {
        searchType: '1'
    };
    componentWillMount(){
        //这个地方需要对默认搜索进行处理
        if (this.props.willMountCallback) {
            this.props.willMountCallback(this.handleSearch);
        }
    }

    componentWillReceiveProps(newProps){
        //更新某些属性
    }
    getSearchType = ()=>{

    }
    handleKeyChangeWord = (e) =>{
        //处理关键字改变-需要
        //this.props.dispatch(changeKeyWord(e.target.value));
    }
    handleSearch = () =>{
        let activeMenu = (this.props.searchInfo || {}).activeMenu;
        let condition = {...this.props.condition};
        condition.keyWord = this.props.searchInfo.searchKeyWord;
        if (this.props.searchInfo.activeOrg.id !== "0") {
            condition.departmentId = this.props.searchInfo.activeOrg.id;
        }
        condition.searchSourceType = getSearchType(activeMenu);
        condition.searchType = this.state.searchType;
        // console.log("格式化前的搜索条件:", condition);
        let standardCondition = formatSearchCondition(condition);
        console.log("最终搜索条件:", standardCondition);
        this.props.dispatch(saveSearchCondition(standardCondition));//每次搜索时保存便于在此基础上更新
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(searchStart(standardCondition));
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
export function getSearchType(activeMenu) {
    let searchSourceType = "0";
    switch (activeMenu) {
        case 'menu_index':
            searchSourceType = "1";
            break;
        default:
            searchSourceType = "1";
            break;
    }
    return searchSourceType;
}
//搜索条件格式化
export function formatSearchCondition(condition) {
    let standardCondition = {...condition};
    if (standardCondition) {
        standardCondition.importance = standardCondition.customerLevel;
        standardCondition.demandLevel = standardCondition.requirementLevel;
        standardCondition.source = standardCondition.customerSource;
        standardCondition.provinceId = standardCondition.firstAreaKey === '0' ? null : standardCondition.firstAreaKey;
        standardCondition.cityId = standardCondition.secondAreaKey === '0' ? null : standardCondition.secondAreaKey;
        standardCondition.areaId = standardCondition.thirdAreaKey === '0' ? null : standardCondition.thirdAreaKey;
        standardCondition.businessStage = standardCondition.businessChance;
        delete standardCondition.customerLevel;
        delete standardCondition.requirementLevel;
        delete standardCondition.customerSource;
        delete standardCondition.firstAreaKey;
        delete standardCondition.secondAreaKey;
        delete standardCondition.thirdAreaKey;
        delete standardCondition.businessChance;
    }
    return standardCondition;
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