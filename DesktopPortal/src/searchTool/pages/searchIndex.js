import {connect} from 'react-redux';
import {getBuildingShops, getAreaList, getDicParList, searchBuilding, setLoading, expandSearchbox, searchRecommendList, getXYHBuildingDetail} from '../actions/actionCreator';
import React, {Component} from 'react'
import {Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import './searchIndex.less';
import BuildingResultItem from '../../businessComponents/buildingSearchItem';
import SearchResultDetail from './searchResultDetail';

const CheckboxGroup = Checkbox.Group;
const ButtonGroup = Button.Group;

const Option = Select.Option;
const tagsOptionData = [{value: 'hasBus', label: '公交'},
{value: 'hasRail', label: '地铁'},
{value: 'hasOtherTraffic', label: '其他交通'},
{value: 'hasKindergarten', label: '幼儿园'},
{value: 'hasPrimarySchool', label: '小学'}, {value: 'hasMiddleSchool', label: '中学'},
{value: 'hasUniversity', label: '大学'}, {value: 'hasMarket', label: '商场'},
{value: 'hasSupermarket', label: '超市'}, {value: 'hasBank', label: '银行'}];

const styles = {
    conditionRow: {
        width: '80px',
        display: 'inline-block',
        fontWeight: 'bold',
    },
    bSpan: {
        fontWeight: 'bold',
    }
}

class SearchIndex extends Component {
    state = {
        condition: {
            searchType: 'xyh',
            keyWord: '',
            priceRange: '0',
            saleStatus: '0',
            leaseStatus: '0',
            areaRange: '0',
            firstAreaKey: '',
            secondAreaKey: '0',
            thirdAreaKey: '0',
            checkedTags: {},
            priceIsAscSort: 'none',
            pageIndex: 0,
            pageSize: 10
        },
        checkedTag: [],
        firstAreaOption: [],
        secondAreaOption: [],
        thirdAreaOption: [],
        filterTags: [],
    }
    componentWillMount() {
        this.props.dispatch(getDicParList(["SEARCH_PRICE_XYH", "SEARCH_PRICE_ZYW", "SEARCH_AREA", "SHOP_SALE_STATUS"]));
        this.props.dispatch(getAreaList());
    }
    componentDidMount() {
        console.log(this.props.user, '用户信息')

        if (this.props.user) {
            this.setState({
                condition: Object.assign((this.state.condition.firstAreaKey), this.state.condition, {firstAreaKey: this.props.user.City})
            })
        }
        this.handleSearch();
    }
    componentWillReceiveProps(newProps) {
        this.setState({firstAreaOption: newProps.basicData.areaList});
    }
    handleSearchKeyChange = (e) => {//关键词
        let condition = {...this.state.condition, keyWord: e.target.value}
        this.setState({condition: condition});
    }

    handleSearch = (condi) => {
        //console.log("查询条件：", this.state.condition);
        this.setState({...this.state.condition, pageIndex: 0});
        let condition = condi || this.state.condition;
        let formatCondition = {};
        if (condition.searchType === "xyh") {
            formatCondition = {
                keyWord: condition.keyWord, ...condition.checkedTags,
                pageIndex: condi ? condition.pageIndex : 0, pageSize: condition.pageSize
            };
            if (condition.saleStatus !== "0") {
                formatCondition.saleStatus = [condition.saleStatus];
            }
            if (condition.firstAreaKey !== "0") {
                formatCondition.city = condition.firstAreaKey;
            }
            if (condition.secondAreaKey !== "0") {
                formatCondition.district = condition.secondAreaKey;
            }
            if (condition.thirdAreaKey !== "0") {
                formatCondition.area = condition.thirdAreaKey;
            }
            if (condition.priceIsAscSort !== "none") {
                formatCondition.priceIsAscSort = (condition.priceIsAscSort === "asc" ? true : false);
            }
            if (condition.priceRange && condition.priceRange !== "0") {
                if (condition.priceRange.indexOf("-") > 0) {
                    let priceArray = condition.priceRange.split('-');
                    formatCondition.lowPrice = priceArray[0];
                    formatCondition.highPrice = priceArray[1];
                } else {
                    formatCondition.lowPrice = condition.priceRange;
                }
            }
        }
        console.log("条件：", formatCondition);
        this.props.dispatch(searchBuilding(formatCondition));
        this.props.dispatch(searchRecommendList({pageIndex: 0, pageSize: 10}))
        this.props.dispatch(setLoading(true));
    }

    handleAreaChange = (code, level) => {//区域
        console.log("value:" + code + ",level:" + level);
        let condition = {...this.state.condition, pageIndex: 0};
        if (level === "1") {
            let options = this.state.firstAreaOption.find(item => item.value === code);
            condition = {...condition, firstAreaKey: code, secondAreaKey: "0", thirdAreaKey: "0"};
            if (options) {
                this.setState({condition: condition, secondAreaOption: options.children});
            } else {
                this.setState({condition: condition, secondAreaOption: []});
            }
        } else if (level === "2") {
            let options = this.state.secondAreaOption.find(item => item.value === code);
            condition = {...condition, secondAreaKey: code, thirdAreaKey: "0"};
            if (options) {
                this.setState({condition: condition, thirdAreaOption: options.children});
            } else {
                this.setState({condition: condition, thirdAreaOption: []});
            }
        } else if (level === "3") {
            condition = {...condition, thirdAreaKey: code};
            this.setState({condition: condition});
        }
        this.handleSearch(condition);
    }

    handleSearchTypeChange = (value) => {//检索类型：租壹屋、新耀行
        let condition = {...this.state.condition, searchType: value, priceRange: '0', saleStatus: '0', leaseStatus: '0', areaRange: '0', checkedTags: {}};
        this.setState({condition: condition, filterTags: []});
    }
    handleSearchBoxToggle = (e) => {//筛选条件展开、收缩
        let visible = !this.props.searchInfo.expandSearchBox;
        this.props.dispatch(expandSearchbox(visible));
    }
    handlePriceRange = (type, price) => {//价格
        console.log("type:" + type + ',price:' + price.value);
        let condition = {...this.state.condition, priceRange: price.value};
        let tagArray = this.state.filterTags;
        for (let i = tagArray.length - 1; i > -1; i--) {
            if (tagArray[i].type === "priceRange") {
                tagArray.splice(i, 1);
            }
        }
        if (price.value && price.value !== "0") {//价格
            tagArray.push({value: price.value, label: price.key, type: "priceRange"});
        }
        this.setState({condition: condition, filterTags: tagArray});
        this.handleSearch(condition);
    }
    handleSaleStatusChange = (value, text) => {
        let condition = {...this.state.condition, saleStatus: value};
        let tagArray = this.state.filterTags;
        for (let i = tagArray.length - 1; i > -1; i--) {
            if (tagArray[i].type === "saleStatus" || tagArray[i].type === "leaseStatus") {
                tagArray.splice(i, 1);
            }
        }
        if (value !== "0") {
            tagArray.push({value: this.state.condition.saleStatus, label: text, type: 'saleStatus'});
        }
        this.setState({condition: condition, filterTags: tagArray, pageIndex: 0});
        this.handleSearch(condition);
    }
    handleLeaseStatusChange = (value, text) => {
        let condition = {...this.state.condition, leaseStatus: value};
        let tagArray = this.state.filterTags;
        for (let i = tagArray.length - 1; i > -1; i--) {
            if (tagArray[i].type === "saleStatus" || tagArray[i].type === "leaseStatus") {
                tagArray.splice(i, 1);
                break;
            }
        }
        if (value !== "0") {
            tagArray.push({value: this.state.condition.leaseStatus, label: text, type: 'leaseStatus'});
        }
        this.setState({condition: condition, filterTags: tagArray, pageIndex: 0});
        this.handleSearch(condition);
    }
    handleShopAreaChange = (area) => {//租壹屋，铺面面积
        let condition = {...this.state.condition, areaRange: area.value};
        let tagArray = this.state.filterTags;
        for (let i = tagArray.length - 1; i > -1; i--) {
            if (tagArray[i].type === "areaRange") {
                tagArray.splice(i, 1);
                break;
            }
        }
        if (area.value !== "0") {//面积
            tagArray.push({value: area.value, label: "面积" + area.key, type: 'areaRange'});
        }
        this.setState({condition: condition, filterTags: tagArray, pageIndex: 0});
        this.handleSearch(condition);
    }

    handleTagChange = (chkArray) => {//标签
        let checkedTags = {};
        let tagArray = this.state.filterTags.filter(t => t.type !== "tag");
        chkArray.map(tag => {
            checkedTags[tag] = true;
            let originalTag = tagsOptionData.find(t => t.value === tag);
            if (originalTag) {
                tagArray.push({value: tag, label: originalTag.label, type: 'tag'});
            }
        });

        let condition = {...this.state.condition, checkedTags: checkedTags};
        this.setState({condition: condition, filterTags: tagArray, checkedTag: chkArray, pageIndex: 0});
        this.handleSearch(condition); console.log("checked");
    }
    handleTagClose = (tag, i) => {//过滤标签删除
        console.log("移除的tag：", tag, this.state.checkedTag);
        let tagArray = this.state.filterTags;
        let condition = this.state.condition;
        let checkedTag = this.state.checkedTag;
        let removeTag = tagArray.splice(i, 1)[0];
        if (removeTag.type === "tag") {
            //delete checkedTag[removeTag.value];
            for (let i = checkedTag.length - 1; i > -1; i--) {
                if (checkedTag[i] === removeTag.value) {
                    checkedTag.splice(i, 1);
                    break;
                }
            }
        } else {
            condition[removeTag.type] = '0';
        }
        console.log("tagArray", tagArray, checkedTag);
        this.setState({condition: condition, filterTags: tagArray, checkedTag: checkedTag, pageIndex: 0});
        this.handleSearch(condition);
    }

    handlePageChange = (pageIndex, pageSize) => {//查询结果翻页
        let condition = {...this.state.condition, pageIndex: (pageIndex - 1)};
        this.setState({condition: condition});
        this.handleSearch(condition);
    }
    handleOrderChange = (e) => {
        console.log(e.target.value);
        let condition = {...this.state.condition, priceIsAscSort: e.target.value};
        this.setState({condition: condition});
        this.handleSearch(condition);
    }
    //楼盘详细
    handleBuildingClick = (buildingID) => {
        this.props.dispatch(setLoading(true));
        this.props.dispatch(getXYHBuildingDetail(buildingID));
        this.props.dispatch(getBuildingShops({buildingIds: buildingID, saleStatus: []}));
        this.props.dispatch(expandSearchbox(false));
    }

    render() {
        // const selectBefore = (
        //     <Select defaultValue="xyh" style={{ width: 80 }} disabled onChange={this.handleSearchTypeChange}>
        //         <Option value="xyh">新耀行</Option>
        //         <Option value="zyw">租壹屋</Option>
        //     </Select>
        // );
        const searchInfo = this.props.searchInfo || {};
        const searchResult = searchInfo.searchResult;
        const paginationInfo = searchInfo.pagination;
        //const activeBuilding = this.props.searchInfo.activeBuilding;
        const showLoading = searchInfo.showLoading;
        const expandSearchBox = searchInfo.expandSearchBox;
        //const activeBuildingShops = this.props.searchInfo.activeBuildingShops;
        const showResult = searchInfo.showResult;
        const recommendList = searchInfo.recommendList || [];
        console.log(this.state.condition.firstAreaKey, '???', this.state.condition.secondAreaKey, this.state.condition.thirdAreaKey)
        return (
            <div className="searchTool" >
                <div style={{display: showResult.navigator.length === 0 ? "block" : "none"}}>
                    <Row className='searchBox'>
                        <Col span={24}>
                            <img src='../../../favicon.ico' alt="新耀行" />
                            <Input addonBefore="新耀行" prefix={<Icon type="search" />} onChange={this.handleSearchKeyChange} onPressEnter={(e) => this.handleSearch()} style={{width: '400px', marginLeft: '5px'}} placeholder='请输入房源名称' />
                            <Button type="primary" onClick={(e) => this.handleSearch()}>搜索</Button>
                        </Col>
                    </Row>
                    <div className='searchConditon'>
                        <Row>
                            <Col span={12}>
                                <span style={styles.bSpan}>所有楼盘 > </span>
                                <span> {this.state.filterTags.map((tag, i) => <Tag closable onClose={e => this.handleTagClose(tag, i)} key={tag.value}>{tag.label}</Tag>)}</span>
                            </Col>
                            <Col span={4}>
                                <Button onClick={this.handleSearchBoxToggle}>{expandSearchBox ? "收起筛选" : "展开筛选"}<Icon type={expandSearchBox ? "up-square-o" : "down-square-o"} /></Button>
                            </Col>
                        </Row>
                        <div style={{display: expandSearchBox ? "block" : "none"}}>
                            {
                                this.state.condition.searchType === "xyh" ? <Row>
                                    <Col span={24}>
                                        <label style={styles.conditionRow}>销售状态 ：</label> <Button type="primary" size='small' className={this.state.condition.saleStatus === '0' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('0')}>不限</Button>
                                        <Button type="primary" size='small' className={this.state.condition.saleStatus === '2' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('2', '在售')}>在售</Button>
                                        <Button type="primary" size='small' className={this.state.condition.saleStatus === '1' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('1', '待售')}>待售</Button>
                                        <Button type="primary" size='small' className={this.state.condition.saleStatus === '10' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('10', '已售')}>已售</Button>
                                    </Col>
                                </Row> : null
                            }

                            {/* {
                            this.state.condition.searchType === "zyw" ? <Row>
                                <Col span={24}>
                                    <label style={styles.conditionRow}>租赁状态：</label> <Button type="primary" size='small' className={this.state.condition.leaseStatus === '0' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleLeaseStatusChange('0')}>不限</Button>
                                    <Button type="primary" size='small' className={this.state.condition.leaseStatus === '1' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleLeaseStatusChange('1', '未租')}>未租</Button>
                                    <Button type="primary" size='small' className={this.state.condition.leaseStatus === '2' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleLeaseStatusChange('2', '已租')}>已租</Button>
                                </Col>
                            </Row> : null
                        } */}

                            <Row style={{display: this.state.condition.searchType === "xyh" ? "block" : "none"}}>
                                <Col span={24}>
                                    <label style={styles.conditionRow}>价格 ：</label> <Button className="priceRangeBtn" onClick={(e) => this.handlePriceRange('xyh', '0')}>不限</Button>
                                    {
                                        this.props.basicData.searchPriceXYH.map(price =>
                                            <Button className={price.value === this.state.condition.priceRange ? "priceRangeBtn priceBtnActive" : "priceRangeBtn"} key={price.value} onClick={(e) => this.handlePriceRange('xyh', price)}>{price.key}</Button>
                                        )
                                    }
                                </Col>
                            </Row>
                            {/* <Row style={{display: this.state.condition.searchType === "zyw" ? "block" : "none"}}>
                            <Col span={24}>
                                价格： <Button className="priceRangeBtn" onClick={(e) => this.handlePriceRange('zyw', '0')}>不限</Button>
                                {
                                    this.props.basicData.searchPriceZYW.map(price =>
                                        <Button className={price.value === this.state.condition.priceRange ? "priceRangeBtn priceBtnActive" : "priceRangeBtn"} key={price.value} onClick={(e) => this.handlePriceRange('zyw', price)}>{price.key}</Button>
                                    )
                                }
                            </Col>
                        </Row> */}
                            {/* <Row style={{display: this.state.condition.searchType === "zyw" ? "block" : "none"}}>
                            <Col span={24}>
                                面积 <Button className="priceRangeBtn" onClick={(e) => this.handleShopAreaChange('0')}>不限</Button>
                                {
                                    this.props.basicData.serachAreaZYW.map(area => {
                                        return (<Button className={area.value === this.state.condition.areaRange ? "priceRangeBtn priceBtnActive" : "priceRangeBtn"} key={area.value} onClick={(e) => this.handleShopAreaChange(area)}>{area.key}</Button>)
                                    })
                                }
                            </Col>
                        </Row> */}
                            <Row>
                                <Col span={24}>
                                    <label style={styles.conditionRow}>标签 ：</label>
                                    <CheckboxGroup options={tagsOptionData} defaultValue={[]} value={this.state.checkedTag} onChange={this.handleTagChange} style={{display: 'inline'}} />
                                </Col>
                            </Row>
                            <Row>
                                <Col span={24}>
                                    <label style={styles.conditionRow}>区域 ：</label>
                                    <Select value={this.state.condition.firstAreaKey} onChange={(value) => this.handleAreaChange(value, '1')}>
                                        {/* <Option key="0">不限</Option> */}
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
                            <Row>
                                <Col span={24}>
                                    <label style={styles.conditionRow}>价格排序 ：</label>
                                    <ButtonGroup onClick={this.handleOrderChange}>
                                        <Button type={this.state.condition.priceIsAscSort === "none" ? "primary" : ""} value="none">不排序</Button>
                                        <Button type={this.state.condition.priceIsAscSort === "asc" ? "primary" : ""} icon="arrow-up" value="asc">升序</Button>
                                        <Button type={this.state.condition.priceIsAscSort === "desc" ? "primary" : ""} icon="arrow-down" value="desc">降序</Button>
                                    </ButtonGroup>
                                </Col>
                            </Row>
                        </div>
                    </div>
                    <p style={{padding: '15px 10px', borderBottom: '1px solid #e0e0e0', fontSize: '1.4rem', fontWeight: 'bold'}}>目前已为你筛选出<b style={{color: '#f36366'}}> {paginationInfo.totalCount || 0} </b>条房源信息</p>
                    <Spin spinning={showLoading} delay={200} tip="查询中...">
                        {
                            showResult.showBuildingList ? <div className='searchResult'>
                                {/**推荐列表**/}
                                <Row>
                                    <Col span={24}>
                                        {
                                            recommendList.map(recommend =>
                                                <BuildingResultItem key={recommend.id} recommendInfo={recommend} buildInfo={recommend.buildingSearchResponse} gotoBuildingDetail={this.handleBuildingClick} />
                                            )
                                        }
                                    </Col>
                                </Row>
                                <Row>
                                    <Col style={{color: '#333', textAlign: 'center'}}>
                                        {
                                            recommendList.length > 0 ?
                                                <p style={{borderBottom: '1px solid #e0e0e0', paddingBottom: '15px'}}><Icon type="smile-o" style={{marginRight: '5px'}} />以上为推荐楼盘</p>
                                                : null
                                        }
                                    </Col>
                                </Row>
                                {/**搜索结果**/}
                                <Row>
                                    <Col span={24}>
                                        {
                                            (searchResult.length === 1 && searchResult[0].id === "00000000") ? <div style={{marginTop: '10px'}}>{searchResult[0].name}</div> :
                                                searchResult.map((build, i) => <BuildingResultItem key={build.id} buildInfo={build} gotoBuildingDetail={this.handleBuildingClick} />)
                                        }
                                    </Col>
                                </Row>
                                {
                                    (searchResult.length > 0 && searchResult[0].id !== "00000000") ? <Pagination showQuickJumper current={this.state.condition.pageIndex + 1} total={paginationInfo.totalCount} onChange={this.handlePageChange}
                                        style={{display: 'flex', justifyContent: 'flex-end'}} /> : null
                                }
                            </div> : null
                        }
                    </Spin>
                </div>
                {
                    showResult.navigator.length > 0 ? <SearchResultDetail /> : null
                }
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        basicData: state.basicData,
        searchInfo: state.search,
        user: (state.oidc.user || {}).profile || {},
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchIndex);
