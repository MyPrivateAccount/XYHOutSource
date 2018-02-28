import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { Spin, Menu, Icon, Button } from 'antd'
import { connect } from 'react-redux'
import reducers from './reducers'
import ContentPage from './pages/contentPage'
import { sagaMiddleware } from '../'
import rootSaga from './saga/rootSaga';
import BuildingDishInfo from './pages/buildingDish/detail/buildingDashInfo'
// import Shops from './pages/shops/edit/shopsEdit'
import ShopsDetail from './pages/shops/detail/ShopsDetail'
import { gotoThisBuild, gotoThisShop } from './actions/actionCreator';
sagaMiddleware.run(rootSaga);

class HouseAuditViewIndex extends Component {

    state = {
    }
    componentWillMount() {
        //console.log("首次加载 buildID:", this.props.buildingID);
        const contentInfo = this.props.contentInfo;
        if (contentInfo.contentType === "building") {
            this.props.dispatch(gotoThisBuild({ id: contentInfo.contentID }));
        } else if (contentInfo.contentType === "shops") {
            this.props.dispatch(gotoThisShop({ shopsInfo: { id: contentInfo.contentID } }));
        }
    }

    componentWillReceiveProps(newProps) {

    }



    render() {
        const contentInfo = this.props.contentInfo;
        return (
            <div>
                {contentInfo.contentType === "building" ? <BuildingDishInfo /> : <ShopsDetail />}
            </div>
        )
    }
}
function mapStateToProps(state) {
    //console.log(state, '首页加载我的楼盘')
    return {
        buildInfo: state.building.buildInfo,
    }
}
function mapDispatchToProps(dispatch) {
    return {
        dispatch
    }
}

export default withReducer(reducers, 'HouseResourceIndex', { mapExtraState: (state, rootState) => ({ oidc: rootState.oidc }) })(connect(mapStateToProps, mapDispatchToProps)(HouseAuditViewIndex));