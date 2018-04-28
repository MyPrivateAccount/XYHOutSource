import {connect} from 'react-redux';
import {getBuildingDetail, getZywBuildingDetail, getShopDetail, getZywShopDetail, setLoadingVisible} from '../actions/actionCreator';
import {getDicParList} from '../../actions/actionCreators'
import React, {Component} from 'react';
import {Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory';
import XYHBuildingDishInfo from '../../businessComponents/building/xyhBuildingDetail';
import ZYWBuildingDishInfo from '../../zywBusinessComponents/building/zywBuildingDetail';
import XYHShopsDetail from '../../businessComponents/shop/shopsDetail';
import ZYWShopsDetail from '../../zywBusinessComponents/shop/shopsDetail';
import {globalAction} from 'redux-subspace';

class AuditHouseSource extends Component {
    state = {

    }
    componentWillMount() {
        let sourceType = this.props.sourceType || 'building';
        let company = this.props.company || "xyh"
        let activeAuditInfo = this.props.activeAuditInfo;
        this.props.dispatch(setLoadingVisible(true));
        if (company == "xyh") {
            if (sourceType == 'building') {
                this.props.dispatch(getBuildingDetail(activeAuditInfo.contentId));
            } else {
                this.props.dispatch(getShopDetail(activeAuditInfo.contentId));
            }
        } else {
            if (sourceType == 'building') {
                this.props.dispatch(getZywBuildingDetail(activeAuditInfo.contentId));
            } else {
                this.props.dispatch(globalAction(getDicParList(["SHOP_DEPOSITTYPE","SHOP_LEASE_PAYMENTTIME"])))
                this.props.dispatch(getZywShopDetail(activeAuditInfo.contentId));
            }
        }


    }

    getSourceContent() {
        let sourceType = this.props.sourceType || 'building';
        let company = this.props.company || "xyh"
        const buildingInfo = this.props.buildingOfActiveInfo || {};
        const shopOfActiveInfo = this.props.shopOfActiveInfo || {};
        const rootBasicData = (this.props.rootBasicData || {}).dicList || [];
        if (company == "xyh") {
            if (sourceType == 'building') {
                return (<XYHBuildingDishInfo buildingInfo={buildingInfo} basicData={rootBasicData} />)
            } else {
                return (<XYHShopsDetail shopInfo={shopOfActiveInfo} basicData={rootBasicData} />)
            }
        } else {
            if (sourceType == 'building') {
                return (<ZYWBuildingDishInfo buildingInfo={buildingInfo} basicData={rootBasicData} />)
            } else {
                return (<ZYWShopsDetail shopInfo={shopOfActiveInfo} basicData={rootBasicData} />)
            }
        }
    }

    render() {
        return (
            <div>
                <b>房源审核</b>
                {this.getSourceContent()}
                <AuditHistory />
                {
                    this.props.activeAuditInfo.examineStatus === 1 ? <AuditForm /> : null
                }
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        rootBasicData: state.rootBasicData,
        activeAuditInfo: state.audit.activeAuditInfo,
        buildingOfActiveInfo: state.audit.buildingOfActiveInfo,
        shopOfActiveInfo: state.audit.shopOfActiveInfo,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditHouseSource);