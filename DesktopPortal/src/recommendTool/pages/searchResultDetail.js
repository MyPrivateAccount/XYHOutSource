import {connect} from 'react-redux';
import {closeResultDetail, changeNav, getCustomerDeal, getShopDetail} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Popconfirm, Card, Breadcrumb} from 'antd';
import BuildingDishEdit from '../../businessComponents/building/buildingDetail'
import ShopsDetail from '../../businessComponents/shop/shopsDetail'
// import MessageList from './message/messageList';
// import MessageDetail from './message/messageDetail';

class SearchResultDetail extends Component {
    state = {
    }
    componentWillMount() {
    }
    handleDetailClose = (e) => {
        console.log("关闭");
        this.props.dispatch(closeResultDetail());
    }
    handleNavClick = (navigator) => {
        console.log("导航信息:", navigator);
        this.props.dispatch(changeNav(navigator));
    }
    getShopDetail = (shopId) => {
        this.props.dispatch(getShopDetail(shopId));
    }
    render() {
        const showResult = this.props.showResult;
        const navigator = showResult.navigator || [];
        const buildingInfo = this.props.activeBuilding || {};
        const basicData = this.props.basicData || {};
        const activeBuildingShops = this.props.activeBuildingShops || [];
        const shopInfo = this.props.activeShop || {};
        console.log("查询结果详细:", showResult, shopInfo);
        return (
            // <div className="searchResultDetail" style={{height: '100%'}}>
            <Card title={<Breadcrumb separator=">">
                <Breadcrumb.Item key='default' onClick={this.handleDetailClose}><b style={{fontSize: '0.9rem', cursor: 'pointer'}}>房源推荐首页</b></Breadcrumb.Item>
                {
                    navigator.map((n, i) => <Breadcrumb.Item key={"Breadcrumb" + i} onClick={(e) => this.handleNavClick(n)}><b style={{fontSize: '0.9rem', cursor: 'pointer'}}>{n.name}</b></Breadcrumb.Item>)
                }
            </Breadcrumb>} style={{margin: '2px'}} noHovering>
                {/**楼盘列表详情**/}
                {
                    showResult.showBuildingDetal ? <div style={{height: '100%', overflowY: 'auto', padding: '24px 32px'}}><BuildingDishEdit buildingInfo={buildingInfo} buildingShops={activeBuildingShops} basicData={basicData} getShopDetail={this.getShopDetail} /></div> : null
                }
                {/**商铺列表详情**/}
                {
                    showResult.showShopDetail ? <div style={{height: '100%', overflowY: 'auto', padding: '24px 32px'}}><ShopsDetail shopInfo={shopInfo} /></div> : null
                }
                {/**消息列表**/}
                {/* {
                        showResult.showMsgList ? <div><MessageList /></div> : null
                    } */}
                {/**消息详细**/}
                {/* {
                        showResult.showMsgDetail ? <div><MessageDetail /></div> : null
                    } */}
            </Card>
            // </div>
        )
    }

}

function mapStateToProps(state) {
    return {
        showResult: state.search.showResult,
        activeBuilding: state.search.activeBuilding,
        basicData: state.basicData,
        activeBuildingShops: state.search.activeBuildingShops,
        activeShop: state.search.activeShop
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchResultDetail);