import {connect} from 'react-redux';
import {getAreaList, setLoadingVisible, getTreeListAsync} from '../actions/actionCreator';
import {getDicParList} from '../../actions/actionCreators';
import {globalAction} from 'redux-subspace';
import React, {Component} from 'react';
import {Input, InputNumber, Select, Icon, Button, Row, Col, Checkbox, Tag, Spin, Radio, DatePicker, TreeSelect} from 'antd';
import './search.less';
import moment from 'moment';
import SearchBox from './searchBox';
import {getDicPars} from '../../utils/utils';

// const CheckboxGroup = Checkbox.Group;
// const ButtonGroup = Button.Group;
const Option = Select.Option;
const businessChanceDefine = [{value: '0', key: '已报备'}, {value: '1', key: '已带看'}];
let t = null;
class SearchCondition extends Component {
    state = {
        expandSearchCondition: true,
        filterTags: [],
        condition: {
            keyWord: '',
            tradePlannings: [],//业态规划 租壹屋
            businessTypes: [],//经营类型 租壹屋
            isCooperate: false,//是否战略合作 租壹屋
            customerLevel: null,//客户等级
            requirementLevel: null,//需求等级
            businessChance: [],//商机阶段
            customerSource: [],//客户来源
            invalidTypes: [],//无效类型
            followUpStart: null,//跟进
            followUpEnd: null,
            createDateStart: null,//录入时间
            createDateEnd: null,
            acreageStart: null,//面积
            acreageEnd: null,
            priceStart: null,//价格
            priceEnd: null,
            firstAreaKey: '0',//意向区域
            secondAreaKey: '0',
            thirdAreaKey: '0',
            orderRule: false,
            isOnlyRepeat: false,
            businessPlanningIds: [], // 意向业态
            pageIndex: 0,
            pageSize: 10
        },
        firstAreaOption: [],
        secondAreaOption: [],
        thirdAreaOption: [],
        selectedMenuKey: 'menu_index',
        searchHandleMethod: null,//searchbox的查询方法
        notPagination: false,
        dicSetting: {//字典配置
            customerSource: [],
            businessTypes: [],
            customerLevels: [],
            requirementLevels: [],
            invalidResions: []
        },
        treeSelectvalue: null,
        planningTreeSelect: []//业态层次选择
    }
    componentWillMount() {
        this.props.dispatch(globalAction(getDicParList(["ZYW_CUSTOMER_SOURCE", "CUSTOMER_LEVEL", "REQUIREMENT_LEVEL", "CUSTOMER_SOURCE", "ZYW_INVALID_REASON", "RATE_PROGRESS", "REQUIREMENT_TYPE", "FOLLOWUP_TYPE", "LEASE_TYPE"])));
        this.props.dispatch(globalAction(getAreaList()));
        this.props.dispatch(getTreeListAsync())

    }

    componentWillReceiveProps(newProps) {
        if (this.state.planningTreeSelect.length === 0 && newProps.treeData.length > 0) {
            this.setState({planningTreeSelect: [{list: newProps.treeData, selectedKey: '0'}]});
        }
        if (newProps.activeMenu) {
            let condition = {...this.state.condition};
            if (newProps.activeMenu !== this.state.selectedMenuKey) {
                condition = {
                    keyWord: '', tradePlannings: [], businessTypes: [],
                    isCooperate: false, customerLevel: null, requirementLevel: null,
                    businessChance: [], customerSource: [], invalidTypes: [],
                    followUpStart: null, followUpEnd: null, createDateStart: null,
                    createDateEnd: null, acreageStart: null, acreageEnd: null,
                    priceStart: null, priceEnd: null, firstAreaKey: '0',//this.props.user.City,
                    secondAreaKey: '0', thirdAreaKey: '0', businessPlanningIds: [],
                    pageIndex: 0,
                    pageSize: 10
                }
                this.setState({filterTags: []}, () => {this.handleSearch();});
            }
            this.setState({selectedMenuKey: newProps.activeMenu, condition: condition});
        }
        let rootBasicData = newProps.rootBasicData || [];
        let areaList = rootBasicData.areaList || [];
        if (this.props.user && this.props.user.City !== "") {
            areaList = areaList.filter(area => area.value === this.props.user.City);
        }
        this.setState({firstAreaOption: areaList});
        //提取字典
        let dicSetting = this.state.dicSetting;
        if (this.state.dicSetting.customerSource.length === 0) {
            dicSetting.customerSource = getDicPars("ZYW_CUSTOMER_SOURCE", rootBasicData.dicList);
            dicSetting.customerLevels = getDicPars("CUSTOMER_LEVEL", rootBasicData.dicList);
            dicSetting.requirementLevels = getDicPars("REQUIREMENT_LEVEL", rootBasicData.dicList);
            dicSetting.invalidResions = getDicPars("ZYW_INVALID_REASON", rootBasicData.dicList);
            if (dicSetting.customerSource && dicSetting.customerSource.length > 0) {
                this.setState({dicSetting: dicSetting});
            }
        }
    }

    handleSearchBoxToggle = (e) => {//筛选条件展开、收缩
        let visible = !this.state.expandSearchCondition;
        this.setState({expandSearchCondition: visible});
    }
    handleAreaChange = (code, level) => {//区域
        // console.log("value:" + code + ",level:" + level);
        let condition = {...this.state.condition, pageIndex: 0};
        if (level === "1") {
            let options = this.state.firstAreaOption.find(item => item.value === code);
            // console.log(this.state.firstAreaOption, 123)
            condition = {...condition, firstAreaKey: code, secondAreaKey: "0", thirdAreaKey: "0"};
            if (options) {
                this.setState({condition: condition, secondAreaOption: options.children}, () => {this.handleSearch();});
            } else {
                this.setState({condition: condition, secondAreaOption: []}, () => {this.handleSearch();});
            }
        } else if (level === "2") {
            let options = this.state.secondAreaOption.find(item => item.value === code);
            condition = {...condition, secondAreaKey: code, thirdAreaKey: "0"};
            if (options) {
                this.setState({condition: condition, thirdAreaOption: options.children}, () => {this.handleSearch();});
            } else {
                this.setState({condition: condition, thirdAreaOption: []}, () => {this.handleSearch();});
            }
        } else if (level === "3") {
            condition = {...condition, thirdAreaKey: code};
            this.setState({condition: condition}, () => {this.handleSearch();});
        }

    }
    //searchbox组件加载完成的回调
    searchBoxWillMount = (searchMethod) => {
        if (searchMethod) {
            this.setState({searchHandleMethod: searchMethod}, () => {this.handleSearch();});
        }
    }
    //查询处理
    handleSearch = () => {
        let searchMethod = this.state.searchHandleMethod;
        if (searchMethod) {
            searchMethod();
        }
    }
    handleCheckChange = (e, type) => {
        // console.log("checked:", e, type);
        let {businessTypes, customerSource, invalidResions} = this.state.dicSetting;
        let condition = {...this.state.condition};
        let filterTags = [...this.state.filterTags];
        for (let i = filterTags.length - 1; i > -1; i--) {//移除当前选中类型的旧数据
            if (filterTags[i].type === type) {
                filterTags.splice(i, 1);
            }
        }
        condition[type] = e;
        e.map(value => {
            if (type === "businessTypes") {
                let result = businessTypes.find(tp => tp.value === value);
                if (result) {
                    filterTags.push({value: result.value, label: result.key, type: type});
                }
            }
            else if (type === "businessChance") {
                let result = businessChanceDefine.find(tp => tp.value === value);
                if (result) {
                    filterTags.push({value: result.value, label: result.key, type: type});
                }
            }
            else if (type === "customerSource") {
                let result = customerSource.find(tp => tp.value === value);
                if (result) {
                    filterTags.push({value: result.value, label: result.key, type: type});
                }
            }
            else if (type === "invalidTypes") {
                let result = invalidResions.find(tp => tp.value === value);
                if (result) {
                    filterTags.push({value: result.value, label: result.key, type: type});
                }
            }
        });
        this.setState({filterTags: filterTags, condition: condition}, () => {this.handleSearch();});
    }
    //战略合作方式更改处理
    handleCooperateChange = (e) => {
        // console.log("战略合作方式更改", e.target.value);
        this.setState({condition: {...this.state.condition, isCooperate: e.target.value}});
    }
    handleCustomerLevelChange = (level, type) => {
        // console.log("客户等级:", level);
        let condition = {...this.state.condition};
        if (type === "customerLevel") {//客户等级
            condition.customerLevel = (level.value === "" ? null : level.value);
            if (level.value !== "1") {
                condition.requirementLevel = '';
            }
            this.setState({condition: condition}, () => {this.handleSearch();});
        } else {//需求等级 requirementLevel
            condition.requirementLevel = level.value;
            this.setState({condition: condition}, () => {this.handleSearch();});
        }
    }
    handleTagClose = (tag, i) => {//过滤标签删除
        let filterTags = [...this.state.filterTags];
        let tradePlannings = [...this.state.condition.tradePlannings];
        let businessTypes = [...this.state.condition.businessTypes];
        let customerSource = [...this.state.condition.customerSource];
        let businessChance = [...this.state.condition.businessChance];
        let invalidTypes = [...this.state.condition.invalidTypes];
        filterTags.splice(i, 1);
        if (tag.type === "tradePlannings") {
            for (let i = tradePlannings.length - 1; i > -1; i--) {
                if (tradePlannings[i] === tag.value) {
                    tradePlannings.splice(i, 1);
                }
            }
        } else if (tag.type === "businessTypes") {
            for (let i = businessTypes.length - 1; i > -1; i--) {
                if (businessTypes[i] === tag.value) {
                    businessTypes.splice(i, 1);
                }
            }
        } else if (tag.type === "customerSource") {
            for (let i = customerSource.length - 1; i > -1; i--) {
                if (customerSource[i] === tag.value) {
                    customerSource.splice(i, 1);
                }
            }
        } else if (tag.type === "businessChance") {
            for (let i = businessChance.length - 1; i > -1; i--) {
                if (businessChance[i] === tag.value) {
                    businessChance.splice(i, 1);
                }
            }
        } else if (tag.type === "invalidTypes") {
            for (let i = invalidTypes.length - 1; i > -1; i--) {
                if (invalidTypes[i] === tag.value) {
                    invalidTypes.splice(i, 1);
                }
            }
        }
        this.setState({
            filterTags: filterTags,
            condition: {
                ...this.state.condition,
                tradePlannings: tradePlannings,
                businessTypes: businessTypes,
                customerSource: customerSource,
                businessChance: businessChance,
                invalidTypes: invalidTypes
            }
        }, () => {this.handleSearch()});
    }

    handleFollowChange = (e) => {//更近信息更改
        //console.log("更近信息更改:", e);
        let followStart = '', followEnd = '';
        if (e.key.includes("-")) {
            followStart = e.key.split('-')[0];
            followEnd = e.key.split('-')[1];
        }
        let condition = {...this.state.condition};
        condition.followUpStart = followStart;
        condition.followUpEnd = followEnd;
        this.setState({condition: condition}, () => {this.handleSearch()});
    }
    handleNumberChange = (e, field) => {
        //console.log("数字范围更改:", e, field);
        let condition = {...this.state.condition};
        condition[field] = e;
        this.setState({condition: condition}, () => {
            if (t) {
                clearTimeout(t);
            }
            t = setTimeout(() => this.handleSearch(), 1000);
        });
    }
    handleCreateTime = (e, field) => {
        //console.log("录入时间:", e, field);
        let condition = {...this.state.condition};
        condition[field] = (e === "" ? null : e);
        this.setState({condition: condition}, () => {this.handleSearch()});
    }
    //排序更改
    handleOrderChange = (e) => {
        // console.log("排序:", e.target.value);
        let condition = {...this.state.condition};
        condition["orderRule"] = e.target.value;
        this.setState({condition: condition}, () => {this.handleSearch()});
    }
    //禁用日期
    disabledDate(current) {
        // Can not select days before today and today
        return current && current.valueOf() > Date.now();
    }
    //处理查看重客状态变更
    handleViewRepeatChange = (e) => {
        // console.log("查看重客状态变更:", e.target.checked);
        let condition = {...this.state.condition};
        condition.pageIndex = 0;
        condition.isOnlyRepeat = e.target.checked;
        this.setState({condition: condition}, () => {this.handleSearch()});
    }
    //处理是否分页的变更
    handlePaginationChange = (e) => {
        let {condition} = this.state;
        condition.pageIndex = 0;
        condition.pageSize = (e.target.checked) ? 1000 : 10;
        this.setState({notPagination: e.target.checked, condition: condition}, () => {this.handleSearch()});
    }

    // 业态意向
    // onTreeSelectValueChange = (value) => {
    //     let condition = {...this.state.condition};
    //     condition["businessPlanningIds"] = value ? [value] : [];
    //     this.setState({condition: condition}, () => {this.handleSearch()});
    //     this.setState({treeSelectvalue: value});
    // }
    //
    planningTreeSelect = (v, levelIndex) => {
        let condition = {...this.state.condition};
        let treeSelect = this.state.planningTreeSelect || [];
        let treeArray = ((treeSelect[levelIndex].list.find(s => s.id === v) || {}).children || []).map(item => item.id);
        condition["businessPlanningIds"] = v ? [v, ...treeArray] : [];
        if (v === '0') {
            treeSelect.map((select, i) => {
                if (i >= levelIndex) {
                    select.selectedKey = '0';
                }
            });
            if (levelIndex === 0) {
                condition["businessPlanningIds"] = [];
            } else {
                treeArray = ((treeSelect[levelIndex - 1].list.find(s => s.id === treeSelect[levelIndex - 1].selectedKey) || {}).children || []).map(item => item.id);
                condition["businessPlanningIds"] = [treeSelect[levelIndex - 1].selectedKey, ...treeArray];
            }
            // condition["businessPlanningIds"] = levelIndex === 0 ? [] : [treeSelect[levelIndex - 1].selectedKey, ...treeArray];
        } else {
            this._getChildTree(this.props.treeData, v, levelIndex);
            treeSelect[levelIndex].selectedKey = v;
            this.setState({planningTreeSelect: treeSelect});

        }
        this.setState({condition: condition}, () => {this.handleSearch()});
    }
    //获取子级业态
    _getChildTree(treeData, parentID, levelIndex) {
        for (let i = 0; i < treeData.length; i++) {
            if (treeData[i].id === parentID && treeData[i].children) {
                let treeSelect = this.state.planningTreeSelect || [];
                if (treeSelect.length > (levelIndex + 1)) {
                    treeSelect[levelIndex + 1] = {list: treeData[i].children, selectedKey: '0'};
                } else {
                    treeSelect.push({list: treeData[i].children, selectedKey: '0'});
                }
                this.setState({planningTreeSelect: treeSelect});
            } else {
                this._getChildTree(treeData[i].children || [], parentID, levelIndex);
            }
        }
    }

    render() {

        let {customerSource, customerLevels, requirementLevels, invalidResions} = this.state.dicSetting;

        let expandSearchCondition = this.state.expandSearchCondition;
        // const areaList = this.props.rootBasicData.areaList;
        let createDateStart = this.state.condition.createDateStart === null ? null : moment(this.state.condition.createDateStart);
        let createDateEnd = this.state.condition.createDateEnd === null ? null : moment(this.state.condition.createDateEnd);
        let followUpStart = this.state.condition.followUpStart === null ? null : moment(this.state.condition.followUpStart);
        let followUpEnd = this.state.condition.followUpEnd === null ? null : moment(this.state.condition.followUpEnd);
        const activeMenu = this.props.activeMenu;
        const dataSourceTotal = this.props.searchResult.totalCount;
        const repeatCount = this.props.searchResult.repeatCount;//dataSourceTotal - (this.props.searchResult.validityCustomerCount || 0);
        return (
            <div>
                <SearchBox condition={this.state.condition} willMountCallback={this.searchBoxWillMount} />
                <div className='searchCondition'>
                    <Row>
                        <Col span={12}>
                            <span>所有客户></span>
                            <span> {this.state.filterTags.map((tag, i) => <Tag closable onClose={e => this.handleTagClose(tag, i)} key={tag.label + i}>{tag.label}</Tag>)}</span>
                        </Col>
                        <Col span={4}>
                            <Button onClick={this.handleSearchBoxToggle}>{expandSearchCondition ? "收起筛选" : "展开筛选"}<Icon type={expandSearchCondition ? "up-square-o" : "down-square-o"} /></Button>
                        </Col>
                    </Row>
                    <div style={{display: expandSearchCondition ? "block" : "none"}}>
                        <Row className="normalInfo">
                            <Col span={24}>
                                <label>客户等级：</label>
                                <Button key="-1" size='small' type="primary" className={this.state.condition.customerLevel === null ? 'levelBtnActive' : 'levelBtnDefault'} onClick={(e) => this.handleCustomerLevelChange({value: ""}, 'customerLevel')}>不限</Button>
                                {
                                    customerLevels.map(level =>
                                        <Button key={level.key} size='small' type="primary" className={this.state.condition.customerLevel === level.value ? 'levelBtnActive' : 'levelBtnDefault'}
                                            onClick={(e) => this.handleCustomerLevelChange(level, 'customerLevel')}>
                                            {level.key}</Button>)
                                }
                                {
                                    this.state.condition.customerLevel === "1" ? <label>需求等级：</label>
                                        : null
                                }
                                {
                                    this.state.condition.customerLevel === "1" ? requirementLevels.map((level, i) =>
                                        <Button key={level.key + i} size='small' type="primary" className={this.state.condition.requirementLevel === level.value ? 'levelBtnActive' : 'levelBtnDefault'} onClick={(e) => this.handleCustomerLevelChange(level, 'requirementLevel')}>
                                            {level.key}</Button>) : null
                                }
                            </Col>
                        </Row>
                        {
                            activeMenu !== "menu_have_deal" && activeMenu !== "menu_invalid" ? <Row className="normalInfo">
                                <Col span={24}>
                                    <label>商机阶段：</label>
                                    <Checkbox.Group onChange={(e) => this.handleCheckChange(e, 'businessChance')} value={this.state.condition.businessChance}>
                                        {
                                            businessChanceDefine.map(b =>
                                                <Checkbox key={b.key} value={b.value}>{b.key}</Checkbox>
                                            )
                                        }
                                    </Checkbox.Group>
                                </Col>
                            </Row> : null
                        }
                        {
                            activeMenu === "menu_invalid" ? <Row className="normalInfo">
                                <Col span={24}>
                                    <label>无效类型：</label>
                                    <Checkbox.Group onChange={(e) => this.handleCheckChange(e, 'invalidTypes')} value={this.state.condition.invalidTypes}>
                                        {
                                            invalidResions.map(b =>
                                                <Checkbox key={b.key} value={b.value}>{b.key}</Checkbox>
                                            )
                                        }
                                    </Checkbox.Group>
                                </Col>
                            </Row> : null
                        }
                        <Row className="normalInfo">
                            <Col span={24}>
                                <label>意向业态：</label>
                                {/* <TreeSelect
                                    allowClear
                                    style={{width: 200}}
                                    value={this.state.treeSelectvalue}
                                    dropdownStyle={{maxHeight: 400, overflow: 'auto'}}
                                    treeData={this.props.treeData || []}
                                    placeholder="请选择意向业态"
                                    onChange={this.onTreeSelectValueChange}
                                /> */}
                                {
                                    (this.state.planningTreeSelect || []).map((select, i) => <Select key={"select" + i} value={select.selectedKey} onChange={(v) => this.planningTreeSelect(v, i)}>
                                        <Option key="0">不限</Option>
                                        {(select.list || []).map(item => <Option key={item.id}>{item.label}</Option>)}
                                    </Select>)
                                }
                            </Col>
                        </Row>
                        <Row className="normalInfo">
                            <Col span={24}>
                                <label>客户来源：</label>
                                {
                                    <Checkbox.Group onChange={(e) => this.handleCheckChange(e, 'customerSource')} value={this.state.condition.customerSource}>
                                        {
                                            customerSource.map(tp => <Checkbox key={tp.key} value={tp.value}>{tp.key}</Checkbox>)
                                        }
                                    </Checkbox.Group>
                                }
                            </Col>
                        </Row>
                        <Row className="normalInfo">
                            <Col span={24}>
                                <label>意向区域： </label>
                                <Select value={this.state.condition.firstAreaKey} onChange={(value) => this.handleAreaChange(value, '1')}>
                                    <Option key="0">不限</Option>
                                    {
                                        this.state.firstAreaOption.map(city => <Option key={city.value}>{city.label}</Option>)
                                    }
                                </Select>
                                <Select value={this.state.condition.secondAreaKey} onChange={(value) => this.handleAreaChange(value, '2')}>
                                    <Option key="0">不限</Option>
                                    {
                                        this.state.secondAreaOption.map((item) => <Option key={item.value}>{item.label}</Option>)
                                    }
                                </Select>
                                <Select value={this.state.condition.thirdAreaKey} onChange={(value) => this.handleAreaChange(value, '3')}>
                                    <Option key="0">不限</Option>
                                    {
                                        this.state.thirdAreaOption.map((item) => <Option key={item.value}>{item.label}</Option>)
                                    }
                                </Select>

                            </Col>
                        </Row>
                        <Row className="normalInfo">
                            <Col>
                                <label>意向价格(元/㎡/月)：</label><InputNumber min={0} max={100} formatter={value => `¥${value}`} value={this.state.condition.priceStart} onChange={(e) => this.handleNumberChange(e, 'priceStart')} />-<InputNumber min={0} max={100} formatter={value => `¥${value}`} value={this.state.condition.priceEnd} onChange={(e) => this.handleNumberChange(e, 'priceEnd')} />
                                <label>意向面积：</label><InputNumber min={0} max={100} formatter={value => `${value}㎡`} value={this.state.condition.acreageStart} onChange={(e) => this.handleNumberChange(e, 'acreageStart')} />-<InputNumber min={0} max={100} formatter={value => `${value}㎡`} value={this.state.condition.acreageEnd} onChange={(e) => this.handleNumberChange(e, 'acreageEnd')} />
                                {activeMenu !== "menu_invalid" ?
                                    <label><span style={{marginRight: '10px'}}>跟进日期：</span>
                                        <DatePicker disabledDate={this.disabledDate} value={followUpStart} onChange={(e, dateString) => this.handleCreateTime(dateString, 'followUpStart')} />- <DatePicker disabledDate={this.disabledDate} value={followUpEnd} onChange={(e, dateString) => this.handleCreateTime(dateString, 'followUpEnd')} />
                                    </label> : null}
                            </Col>
                        </Row>
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
                    </div>
                </div>
                <div style={{marginTop: '15px'}}>
                    <p> 目前已为你筛选出<span style={{fontWeight: '700', color: "red", fontSize: '1.3rem'}}>{dataSourceTotal}</span>条客户信息
                   {activeMenu === "menu_index" ? <p style={{marginBottom: '10px', display: 'inline'}}><span>，共<span style={{fontWeight: '700', color: "red", fontSize: '1.3rem'}}>{repeatCount}</span>组重客信息</span>
                            <Checkbox onChange={this.handleViewRepeatChange} style={{marginLeft: '10px'}}><b>查看重客</b></Checkbox></p> : null}
                        <Checkbox checked={this.state.notPagination} onChange={this.handlePaginationChange} style={{marginLeft: '10px'}}><b>不分页</b></Checkbox>
                    </p>
                </div>

            </div>
        )
    }
}

function mapStateToProps(state) {
    // console.log("查询条件页面:", state);
    return {
        searchResult: state.search.searchResult,
        activeMenu: state.search.activeMenu,
        user: (state.oidc.user || {}).profile || {},
        rootBasicData: state.rootBasicData,
        treeData: state.orgData.treeData
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchCondition);