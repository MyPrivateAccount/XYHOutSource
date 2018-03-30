import {connect} from 'react-redux';
import {changeKeyWord, searchCustomer, setLoadingVisible, saveSearchCondition} from '../actions/actionCreator';
import React, {Component} from 'react'
import {Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import './search.less'
import {closeRecommendDialog} from '../../searchTool/actions/actionCreator';

const CheckboxGroup = Checkbox.Group;
const ButtonGroup = Button.Group;
const Option = Select.Option;


class SearchBox extends Component {
    state = {
        searchType: '1'
    }
    componentWillMount() {
        if (this.props.willMountCallback) {
            this.props.willMountCallback(this.handleSearch);
        }
    }
    componentWillReceiveProps(newProps) {
    }
    //搜索处理
    handleSearch = () => {
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
        this.props.dispatch(saveSearchCondition(standardCondition));
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(searchCustomer(standardCondition));
    }
    selectBefore = () => {
        const activeMenu = this.props.searchInfo.activeMenu;
        return (
            activeMenu !== "menu_public_pool" ? <Select defaultValue="1" style={{width: 80}} onChange={this.handleSearchTypeChange}>
                <Option value="0" style={{color: '#000'}}>客户</Option>
                <Option value="1" style={{color: '#000'}}>业务员</Option>
            </Select> : null)
    }
    handleKeyWordChange = (e) => {//关键字
        //console.log("keyword:",e.target.value);
        this.props.dispatch(changeKeyWord(e.target.value));
    }
    //处理检索类型更改
    handleSearchTypeChange = (e) => {
        console.log("searchtype change:", e);
        this.setState({searchType: e});
    }
    render() {
        let keyword = this.props.searchInfo.searchKeyWord;
        return (
            <div>
                <div className="searchBox">
                    <p>
                        <Input style={{width: '450px'}} addonBefore={this.selectBefore()} placeholder={this.state.searchType === '1' ? '请输入业务员姓名或电话号码' : '请输入客户姓名或电话号码'} value={keyword} onChange={this.handleKeyWordChange} />
                        <Button type="primary" className='searchButton' onClick={this.handleSearch}>搜索</Button>
                    </p>
                </div>
            </div>
        )
    }
}


function mapStateToProps(state) {
    return {
        basicData: state.basicData,
        searchInfo: state.search
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchBox);

export function getSearchType(activeMenu) {
    let searchSourceType = "0";
    switch (activeMenu) {
        case 'menu_index':
            searchSourceType = "1";
            break;
        case 'menu_have_deal':
            searchSourceType = "2";
            break;
        case 'menu_invalid':
            searchSourceType = "3";
            break;
        case 'menu_public_pool':
            searchSourceType = "4";
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