import { connect } from 'react-redux';
import { getUpdateRecordDetail, setLoadingVisible, getBuildingDetail, getBuildingShops } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Row, Col, Checkbox, Tag, Pagination, Spin } from 'antd';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory';
import getToolComponent from '../../tools';

class AuditHouseNewInfo extends Component {
    state = {

    }
    componentWillMount() {
        let activeAuditInfo = this.props.activeAuditInfo;
        this.props.dispatch(getBuildingDetail(activeAuditInfo.contentId));
        this.props.dispatch(getBuildingShops(activeAuditInfo.contentId));
    }
    componentWillReceiveProps(newProps) {
        //let activeAuditHistory = newProps.activeAuditHistory;
        //console.log("当前审核信息:", activeAuditHistory);
        // if (activeAuditHistory && activeAuditHistory.updateRecord === undefined) {
        //     this.props.dispatch(setLoadingVisible(true));
        //     this.props.dispatch(getUpdateRecordDetail(activeAuditHistory.submitDefineId));
        // }
    }
    //获取报备规则
    getRegularInfo() {

    }
    //格式化审核内容
    formatUpdateContent(updateContent) {
        let jsonObj = updateContent;
        if (jsonObj && jsonObj.startsWith("{") && jsonObj.endsWith("}")) {
            try {
                jsonObj = JSON.parse(jsonObj);
                console.log("jsonObj:==", jsonObj);
            } catch (e) { }
        }
        return jsonObj || {};
    }
    //获取动态优惠政策：
    getPolicy(updateContent) {
        let jsonObj = this.formatUpdateContent(updateContent);
        if (typeof (jsonObj) !== "string") {
            return jsonObj.preferentialPolicies;
        }
        return updateContent;
    }
    //获取佣金方案
    getCommissionType(updateContent) {
        let jsonObj = this.formatUpdateContent(updateContent);
        if (typeof (jsonObj) !== "string") {
            console.log("获取佣金方案:", jsonObj);
            return jsonObj.commissionPlan;
        }
        return updateContent;
    }
    //获取价格
    getPrice(updateContent) {
        let jsonObj = this.formatUpdateContent(updateContent);
        if (typeof (jsonObj) !== "string") {
            console.log("获取佣金方案:", jsonObj);
            return jsonObj || { totalPrice: 0, guidingPrice: 0 };
        }
        return { totalPrice: 0, guidingPrice: 0 };
    }
    //获取动态
    getDynamicInfo(buildingInfo, updateContent) {
        let activeAuditInfo = this.props.activeAuditInfo;
        let jsonObj = this.formatUpdateContent(updateContent);
        console.log("updateRecord:", updateContent);
        if (typeof (jsonObj) === "string" || buildingInfo === undefined) {
            return;
        }
        if (activeAuditInfo.contentType === "ReportRule") {//报备规则
            buildingInfo.ruleInfo = jsonObj;
        }
        if (activeAuditInfo.contentType === "BuildingNo") {//楼栋批次
            buildingInfo.buildingNoInfos = jsonObj || [];
        }
    }

    render() {
        let activeAuditInfo = this.props.activeAuditInfo;
        let updateRecord = this.props.activeAuditHistory.updateRecord || {};
        let HouseActiveInfo = getToolComponent("houseActiveInfo");
        let BuildingSearchItem = getToolComponent("BuildingSearchAuditInfo");
        let buildingInfo = this.props.buildingOfActiveInfo;
        let buildingShops = this.props.buildingOfShops;
        let showLoading = this.props.showLoading;
        this.getDynamicInfo(buildingInfo, updateRecord.updateContent);//将楼盘中的信息替换为当时提交的动态信息
        const price = this.getPrice(updateRecord.updateContent);
        return (
            <div>
                <Spin spinning={showLoading}>
                    <b>房源动态审核({this.props.subTitle})</b>
                    {
                        activeAuditInfo.contentType !== "Price" ? <BuildingSearchItem contentInfo={buildingInfo} style={{ margin: '0.2rem 1rem' }} /> : null
                    }
                    {/**佣金方案**/}
                    {
                        activeAuditInfo.contentType === "CommissionType" ? <p style={{ margin: '1rem' }}>
                            <Row>
                                <Col>佣金方案：{this.getCommissionType(updateRecord.updateContent)}</Col>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                            <Row>
                                <Col>
                                    {/* {activeAuditInfo.fileList.map(file =>)} */}
                                </Col>
                            </Row>
                        </p> : null
                    }
                    {/**加推**/}
                    {
                        activeAuditInfo.contentType === "ShopsAdd" ? <p style={{ margin: '1rem' }}>
                            <Row>
                                <Col>加推</Col>
                                <Col>
                                    <HouseActiveInfo buildInfo={buildingInfo} buildingShops={buildingShops} contentType={activeAuditInfo.contentType} />
                                </Col>
                            </Row>
                            <Row>
                                <Col>备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }
                    {/**报备规则**/}
                    {
                        activeAuditInfo.contentType === "ReportRule" ? <p style={{ margin: '1rem' }}>
                            <Row>
                                <Col>
                                    <HouseActiveInfo buildInfo={buildingInfo} contentType={activeAuditInfo.contentType} />
                                </Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }
                    {/**热卖户型(热卖没有审批)**/}
                    {/* {
                        activeAuditInfo.contentType === "ShopsHot" ? <p style={{ margin: '1rem' }}>
                            <Row>
                                <Col>热卖户型</Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    } */}

                    {/**楼栋批次**/}
                    {
                        activeAuditInfo.contentType === "BuildingNo" ? <p style={{ margin: '1rem' }}>
                            <Row>
                                <Col>
                                    <HouseActiveInfo buildInfo={buildingInfo} contentType={activeAuditInfo.contentType} />
                                </Col>
                                <Row>
                                    <Col >备注：{updateRecord.content}</Col>
                                </Row>
                            </Row>
                        </p> : null
                    }
                    {/**优惠政策**/}
                    {
                        activeAuditInfo.contentType === "DiscountPolicy" ? < p style={{ margin: '1rem' }}>
                            <Row>
                                <Col>优惠政策：{this.getPolicy(updateRecord.updateContent)}</Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }
                    {/**图片**/}
                    {
                        activeAuditInfo.contentType === "Image" ? (() => {

                            return (<p style={{ margin: '1rem' }}>
                                <Row>
                                    <Col>图片：{updateRecord.updateContent}</Col>
                                </Row>
                                <Row>
                                    <Col >备注：{updateRecord.content}</Col>
                                </Row>
                            </p>)
                        })() : null
                    }
                    {/**价格**/}
                    {
                        activeAuditInfo.contentType === "Price" ? <p style={{ margin: '1rem' }}>
                            <Row>
                                <Col>总价：{price.totalPrice}万元，指导价：{price.guidingPrice || 0}万元</Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }
                    <div>
                        {/**审核记录**/}
                        <AuditHistory />
                    </div>
                    {
                        this.props.activeAuditInfo.examineStatus === 1 ? <AuditForm /> : null
                    }
                </Spin>
            </div >
        )
    }
}

function mapStateToProps(state) {
    return {
        showLoading: state.audit.showLoading,
        basicData: state.basicData,
        activeAuditInfo: state.audit.activeAuditInfo,
        activeAuditHistory: state.audit.activeAuditHistory,
        buildingOfActiveInfo: state.audit.buildingOfActiveInfo,
        buildingOfShops: state.audit.buildingOfShops,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditHouseNewInfo);