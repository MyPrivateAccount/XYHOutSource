import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { Spin, Menu, Icon, Button } from 'antd'
import { connect } from 'react-redux'
import reducers from './reducers'
import { sagaMiddleware } from '../'
import rootSaga from './saga/rootSaga';
import RulesInfo from './pages/buildingDish/detail/rulesInfo';
import RelShopsInfo from './pages/buildingDish/detail/rulesTemplateInfo';
import BuildingNoInfo from './pages/buildingDish/detail/buildingNoInfo';
import AttachInfo from './pages/shops/detail/attachInfo';
import CollapseList from './pages/houseActive/collapseList';
import './houseActive.less';
sagaMiddleware.run(rootSaga);
//房源动态审核查看
class HouseActiveAuditView extends Component {

    state = {
    }
    componentWillMount() {
    }

    componentWillReceiveProps(newProps) {
    }
    //获取加推/热卖组件的数据
    getBuildingNO(shopList) {
        let buildingNo = [], newShopList = shopList.slice()
        newShopList.forEach((v) => {
            buildingNo.push({ buildingNo: v.buildingNo })
        })
        let hash = {};
        buildingNo = buildingNo.reduce((item, next) => {  // 去重楼栋数
            hash[next.buildingNo] ? '' : hash[next.buildingNo] = true && item.push(next);
            return item
        }, [])
        buildingNo.map(v => v.children = [])
        newShopList.forEach(v => {
            let index = buildingNo.findIndex(item => item.buildingNo === v.buildingNo)
            buildingNo[index].children.push(v)
        })
        return buildingNo;
    }

    render() {
        const buildInfo = this.props.buildInfo;
        const buildingShops = this.props.buildingShops;//楼盘的商铺列表
        const contentType = this.props.contentType;
        console.log("buildInfo=====:", buildInfo);
        return (
            <div>
                {
                    contentType === "ReportRule" ? <div>
                        <RulesInfo buildInfoActive={buildInfo} type='auditCenter' />
                        <RelShopsInfo buildInfoActive={buildInfo} type='auditCenter' /></div> : null
                }
                {//楼栋批次
                    contentType === "BuildingNo" ? <BuildingNoInfo buildInfoActive={buildInfo} type='auditCenter' /> : null
                }
                {//加推
                    contentType === "ShopsAdd" ? <div className='hotShopPage'><CollapseList list={this.getBuildingNO(buildingShops)} type='push' /></div> : null
                }
            </div>
        )
    }
}
function mapStateToProps(state) {
    //console.log(state, '首页加载我的楼盘')
    return {

    }
}
function mapDispatchToProps(dispatch) {
    return {
        dispatch
    }
}

export default withReducer(reducers, 'HouseResourceIndex', { mapExtraState: (state, rootState) => ({ oidc: rootState.oidc }) })(connect(mapStateToProps, mapDispatchToProps)(HouseActiveAuditView));