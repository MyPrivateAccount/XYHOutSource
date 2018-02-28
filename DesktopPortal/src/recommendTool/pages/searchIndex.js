import {connect} from 'react-redux';
import {getMyRecommendBuilding, searchBuilding, setLoading, getBuildingDetail, getBuildingShops, getDicParList} from '../actions/actionCreator';
import React, {Component} from 'react'
import {Input, Icon, Button, Row, Col, Pagination, Spin} from 'antd'
import './searchIndex.less';
import BuildingResultItem from './buildingResultItem'
import RecommendDateDialog from './recommendDateDialog'
import SearchResultDetail from './searchResultDetail';

class SearchIndex extends Component {
    state = {
        condition: {
            keyWord: '',
            examineStatus: [8]//只有通过审核的才能推荐
        },
        showResult: [],
        pagination: {current: 1, pageSize: 10, total: 0},
    }
    componentDidMount() {
        this.handleSearch();
        this.props.dispatch(getDicParList(["TRADE_MIXPLANNING", "SALE_MODE", "PROJECT_SALE_STATUS", "SHOP_CATEGORY", "SHOP_SALE_STATUS"]));
    }
    componentWillReceiveProps(newProps) {
        let searchResult = newProps.searchInfo.searchResult || [];
        if (searchResult.length !== this.state.showResult.length ||
            (searchResult.length > 0 && this.state.showResult.length > 0 && searchResult[0].id !== this.state.showResult[0].id)) {
            let paginationInfo = {
                pageSize: newProps.searchInfo.pagination.pageSize,
                current: newProps.searchInfo.pagination.pageIndex,
                total: newProps.searchInfo.pagination.totalCount
            };
            this.setState({pagination: paginationInfo, showResult: searchResult});
        }
    }
    handleSearchKeyChange = (e) => {//关键词
        let condition = {...this.state.condition, keyWord: e.target.value}
        this.setState({condition: condition});
    }

    handleSearch = () => {
        let condition = this.state.condition;
        console.log("条件：", condition);
        this.props.dispatch(setLoading(true));
        this.props.dispatch(getMyRecommendBuilding({keyWord: condition.keyWord, pageIndex: 0, pageSize: 1000}));
        this.props.dispatch(searchBuilding(condition));
    }

    handlePageChange = (pageIndex, pageSize) => {//查询结果翻页
        let pagination = {...this.state.pagination, current: pageIndex};
        this.setState({pagination: pagination});
    }

    getCurPageList() {
        let showList = [];
        let showResult = this.state.showResult;
        let {current, pageSize} = this.state.pagination;
        console.log("取数据,", current, pageSize);
        showList = showResult.slice((current - 1) * pageSize, current * pageSize);
        return showList;
    }
    //查看楼盘详细
    viewBuildingDetail = (buildingID) => {
        // alert("楼盘id:" + buildingID);
        this.props.dispatch(getBuildingDetail(buildingID));
        this.props.dispatch(getBuildingShops({buildingIds: buildingID, saleStatus: []}));
    }

    render() {
        const searchResult = this.props.searchInfo.searchResult;
        const paginationInfo = this.props.searchInfo.pagination;
        const showLoading = this.props.searchInfo.showLoading;
        const showResult = this.props.searchInfo.showResult;
        const showList = this.getCurPageList();
        const myRecommendList = this.props.searchInfo.myRecommendList || [];
        const showDetailPage = (showResult.showBuildingDetal || showResult.showShopDetail);
        return (
            <div style={{width: '80%', margin: '0 auto', height: '100%'}} className='recommendTool'>
                {!showDetailPage ? <div>
                    <Row className='searchBox'>
                        <Col span={24}>
                            <Input addonBefore="新耀行" prefix={<Icon type="search" />} onChange={this.handleSearchKeyChange} onPressEnter={(e) => this.handleSearch()} style={{width: '400px'}} placeholder='请输入房源名称' />
                            <Button type="primary" onClick={(e) => this.handleSearch()}>查询</Button>
                        </Col>
                    </Row>
                    <p style={{padding: '0 0 10px 0', borderBottom: '1px solid #e0e0e0', fontSize: '1.4rem', fontWeight: 'bold'}}>
                        目前已为你筛选出<b style={{color: '#f36366'}}> {(paginationInfo.totalCount || 0) + (paginationInfo.myRecommendTotalCount || 0)} </b>条房源信息,其中 <b style={{color: '#f36366'}}>{paginationInfo.myRecommendTotalCount || 0}</b> 条为推荐房源</p>
                </div> : null}
                <Spin spinning={showLoading} delay={200} >
                    {
                        showResult.showBuildingList ? <div className='searchResult'>
                            {/**推荐楼盘**/}
                            {
                                (myRecommendList && myRecommendList.length > 0) ? <div>
                                    <Row>
                                        <Col span={24}>
                                            {
                                                myRecommendList.map((build, i) => <BuildingResultItem key={build.id} buildInfo={build.buildingSearchResponse} refresh={this.handleSearch} viewBuildingDetail={this.viewBuildingDetail}/>)
                                            }
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Col style={{color: '#333', textAlign: 'center'}}>
                                            <p style={{borderBottom: '1px solid #e0e0e0', paddingBottom: '15px'}}><Icon type="smile-o" style={{marginRight: '5px'}} />以上为推荐楼盘</p>
                                        </Col>
                                    </Row>
                                </div> : null
                            }
                            {/**搜索结果**/}
                            <Row>
                                <Col span={24}>
                                    {
                                        (searchResult.length === 1 && searchResult[0].id === "00000000") ? <div style={{marginTop: '10px'}}>{searchResult[0].name}</div> :
                                            showList.map((build, i) => <BuildingResultItem key={build.id} buildInfo={build} refresh={this.handleSearch} viewBuildingDetail={this.viewBuildingDetail} />)
                                    }
                                </Col>
                            </Row>
                            {
                                (searchResult.length > 0 && searchResult[0].id !== "00000000") ?
                                    <Pagination {...this.state.pagination} onChange={this.handlePageChange} style={{display: 'flex', justifyContent: 'flex-end'}} /> : null
                            }
                        </div> : null
                    }
                </Spin>
                <RecommendDateDialog refresh={this.handleSearch} />
                {showDetailPage ? <SearchResultDetail /> : null}
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
export default connect(mapStateToProps, mapDispatchToProps)(SearchIndex);