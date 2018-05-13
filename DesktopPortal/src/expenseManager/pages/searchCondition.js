import {connect} from 'react-redux';
import { setLoadingVisible} from '../actions/actionCreator';
import React, {Component} from 'react'
import {Input, InputNumber, Select, Icon, Button, Row, Col, Checkbox, Tag, Spin, Radio, DatePicker} from 'antd'
import './search.less';
import moment from 'moment';
import SearchBox from './searchBox';

// const CheckboxGroup = Checkbox.Group;
// const ButtonGroup = Button.Group;
const Option = Select.Option;
const checkStateDefine = [{type: '1', key: '审核中'},{type: '8', key: '已审核'}];
let t = null;
class SearchCondition extends Component {
    state = {
        expandSearchCondition: true,
        filterTags: [],
        condition: {
            keyWord: '',
            checkStatu: null,//审核状态
            //organizationName: [],//
            createDateStart: null,//录入时间
            createDateEnd: null,
            //IsExpire:false,
            discard:0,
            follow:0,
            orderRule: 0,
            pageIndex: 0,
            pageSize: 10
        },
        // firstAreaOption: [],
        // secondAreaOption: [],
        // thirdAreaOption: [],
        selectedMenuKey: 'menu_index',
        searchHandleMethod: null,//searchbox的查询方法,
        checkedList:null,
        isNowCheck: false,
        isHasCheck: false,
    }
    componentWillMount() {
      
    }
    componentDidMount() {
        console.log(this.props.user, '用户信息')

    }
    componentWillReceiveProps(newProps) {
        if (newProps.activeMenu) {
            let condition = {...this.state.condition};
            if (newProps.activeMenu !== this.state.selectedMenuKey) {
                condition = {
                    //更换菜单的时候查询条件初始化
                }
                this.setState({filterTags: []}, () => {this.handleSearch();});
            }
            this.setState({selectedMenuKey: newProps.activeMenu, condition: condition});
        }

    }

    handleSearchBoxToggle = (e) => {//筛选条件展开、收缩
        let visible = !this.state.expandSearchCondition;
        this.setState({expandSearchCondition: visible});
    }

    //searchbox组件render前的回调
    searchBoxWillMount = (searchMethod) => {
        // if (searchMethod) {
        //     //setState由于是异步函数因此当前设置的值未必可以马上生效故而后面的回调函数可以在设置成功后进行的操作
        //     this.setState({searchHandleMethod: searchMethod}, () => {this.handleSearch();});
        // }
    }
    //查询处理
    handleSearch = () => {
        // let searchMethod = this.state.searchHandleMethod;
        // if (searchMethod) {
        //     searchMethod();
        // }
    }
   


    handleRecordTimeChange = (e) => {//更近信息更改
     
        // let followStart = '', followEnd = '';
        // if (e.key.includes("-")) {
        //     followStart = e.key.split('-')[0];
        //     followEnd = e.key.split('-')[1];
        // }
        // let condition = {...this.state.condition};
        // condition.followUpStart = followStart;
        // condition.followUpEnd = followEnd;
        // this.setState({condition: condition}, () => {this.handleSearch()});
    }

    handleCreateTime = (e, field) => {
        //console.log("录入时间:", e, field);
        let condition = {...this.state.condition};
        condition[field] = (e === "" ? null : e);
        this.setState({condition: condition}, () => {this.handleSearch()});
    }
    //排序更改
    handleOrderChange = (e) => {
        console.log("排序:", e.target.value);
        let condition = {...this.state.condition};
        condition["orderRule"] = e.target.value;
        this.setState({condition: condition}, () => {this.handleSearch()});
    }

    handleExpire = (e) =>{
        let condition = {...this.state.condition};
        condition["overTime"] = e.target.checked ? 1: 0;
        this.setState({condition:condition}, () =>{this.handleSearch()});
    }


    handleFollow = (e) =>{
        let condition = {...this.state.condition};
        condition["follow"] = e.target.checked ? 1: 0;
        this.setState({condition:condition}, () =>{this.handleSearch()});
    }
    handleInvalid = (e) =>{
        let condition = {...this.state.condition};
        condition["discard"] = e.target.checked ? 1: 0;
        this.setState({condition:condition}, () =>{this.handleSearch()});
    }

    handleCheckChange = (e, type) =>{
        //this.setState({checkedList:,});
        console.log('e.target.checked:', e.target.checked);
        
        if(type === '1'){
            console.log('1type:', type);
            this.setState({isNowCheck:e.target.checked, isHasCheck:false});
        }else if(type === '8'){
            console.log('8type:', type);
            this.setState({isNowCheck:false, isHasCheck:e.target.checked});
        }
        let condition = {...this.state.condition};
        condition.checkStatu = e.target.checked ? type : null;
        this.setState({condition:condition}, () =>{this.handleSearch()});
    }
    render() {
        let expandSearchCondition = this.state.expandSearchCondition;

        let createDateStart = this.state.condition.createDateStart === null ? null : moment(this.state.condition.createDateStart);
        let createDateEnd = this.state.condition.createDateEnd === null ? null : moment(this.state.condition.createDateEnd);
        const activeMenu = this.props.activeMenu;
        let dataSourceTotal = 0;
        if(this.props.searchResult.validityContractCount){
            dataSourceTotal = this.props.searchResult.validityContractCount;
        }
       

        return (
            <div>
                <SearchBox condition={this.state.condition} willMountCallback={this.searchBoxWillMount} />
                <div className='searchCondition'>
                    <Row>
                        <Col span={12}>
                            <span>所有合同></span>
                            <span> {this.state.filterTags.map((tag, i) => <Tag closable onClose={e => this.handleTagClose(tag, i)} key={tag.label + i}>{tag.label}</Tag>)}</span>
                        </Col>
                        <Col span={4}>
                            <Button onClick={this.handleSearchBoxToggle}>{expandSearchCondition ? "收起筛选" : "展开筛选"}<Icon type={expandSearchCondition ? "up-square-o" : "down-square-o"} /></Button>
                        </Col>
                    </Row>
                    <div style={{display: expandSearchCondition ? "block" : "none"}}>   
                        <Row className="normalInfo">
                            {/* <Col span={4}>
                                <label>已作废：</label>
                                    <Checkbox onChange={this.handleInvalid}></Checkbox>
                            </Col> */}
                            <Col span={4}>
                                <label>已过期：</label>
                                    <Checkbox onChange={this.handleExpire}></Checkbox>
                            </Col>
                            <Col span={4}>
                                <label>已续签：</label>
                                    <Checkbox onChange={this.handleFollow}></Checkbox>
                            </Col>
                        </Row>
                        {
                        <Row className="normalInfo">
            
                                {/* <label>审核状态：</label> */}
                                <Col span={4}>
                                <label>审核中：</label>
                                    <Checkbox onChange={(e) =>this.handleCheckChange(e, "1")} checked={this.state.isNowCheck}></Checkbox>
                                </Col>
                                <Col span={4}>
                                    <label>审核通过：</label>
                                    <Checkbox onChange={(e) =>this.handleCheckChange(e, "8")} checked={this.state.isHasCheck}></Checkbox>
                                </Col>
                        
                        </Row>
                        }
                        {      

                            <Row className="normalInfo">
                                <Col>
                                    {activeMenu !== "menu_invalid" ?
                                        <label><span style={{marginRight: '10px'}}>录入日期：</span>
                                            <DatePicker disabledDate={this.disabledDate} value={createDateStart} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateStart')} />- <DatePicker disabledDate={this.disabledDate} value={createDateEnd} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateEnd')} />
                                        </label> : null}
                                </Col>
                            </Row>
                        /*这些条件可能后面会需要  
                        <Row className="normalInfo">
                            <Col>
                                {(activeMenu === "menu_index" || activeMenu === "menu_invalid") ? <label><span style={{marginRight: '10px'}}>录入时间：</span><DatePicker disabledDate={this.disabledDate} value={createDateStart} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateStart')} />- <DatePicker disabledDate={this.disabledDate} value={createDateEnd} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateEnd')} /></label> : null}
                                {activeMenu !== "menu_invalid" ?
                                    <label><span style={{marginRight: '10px'}}>未跟天数进排序：</span>
                                        <Radio.Group onChange={this.handleOrderChange} defaultValue={false} value={this.state.condition.orderRule}>
                                            <Radio.Button value={true}><Icon type="arrow-up" />由少至多</Radio.Button>
                                            <Radio.Button value={false}><Icon type="arrow-down" />由多至少</Radio.Button>
                                        </Radio.Group>
                                    </label> : null}
                            </Col>
                        </Row>
                        */}
                    </div>
                </div>
                {activeMenu === "menu_index" ? <p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{dataSourceTotal}</b>条合同信息</p> : null}
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchResult: state.search.searchResult,
        basicData: state.basicData,
        activeMenu: state.search.activeMenu,
        user: (state.oidc.user || {}).profile || {},

    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchCondition);