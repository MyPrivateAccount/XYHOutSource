import {connect} from 'react-redux';
import {getBuildingDetail, getBuildingShops, getZywBuildingDetail, getZywShopDetail} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Row, Col, Menu, Tag, Pagination, Spin} from 'antd';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory';
import getToolComponent from '../../tools';
import RulesInfo from '../../businessComponents/building/rulesInfo';
import RulesTemplateInfo from '../../businessComponents/building/rulesTemplateInfo';
import BuildingNoInfo from '../../businessComponents/building/buildingNoInfo';
import ZYWOwnerInfo from '../../zywBusinessComponents/shop/ownerInfo';
import IntensionLease from '../../zywBusinessComponents/shop/intensionLease';
import ZYWRulesInfo from '../../zywBusinessComponents/building/rulesInfo';
import ZYWRulesTemplateInfo from '../../zywBusinessComponents/building/rulesTemplateInfo';
import {getShopName, groupShops} from '../../utils/utils';
import XYHCommissionInfo from '../../businessComponents/building/commissionInfo'
import ZYWCommissionInfo from '../../zywBusinessComponents/building/commissionInfo'

const SubMenu = Menu.SubMenu;
class AuditHouseNewInfo extends Component {
    state = {

    }
    componentWillMount() {
        let activeAuditInfo = this.props.activeAuditInfo;
        if (activeAuditInfo.contentType.startsWith("ZYW")) {
            this.props.dispatch(getZywBuildingDetail(activeAuditInfo.contentId));
            this.props.dispatch(getZywShopDetail(activeAuditInfo.contentId));
        } else {
            this.props.dispatch(getBuildingDetail(activeAuditInfo.contentId));
            this.props.dispatch(getBuildingShops(activeAuditInfo.contentId));
        }
    }
    componentWillReceiveProps(newProps) {
        //let activeAuditHistory = newProps.activeAuditHistory;
        //console.log("当前审核信息:", activeAuditHistory);
        // if (activeAuditHistory && activeAuditHistory.updateRecord === undefined) {
        //     this.props.dispatch(setLoadingVisible(true));
        //     this.props.dispatch(getUpdateRecordDetail(activeAuditHistory.submitDefineId));
        // }
    }

    //获取动态
    getDynamicInfo(buildingInfo, updateContent) {
        let activeAuditInfo = this.props.activeAuditInfo;
        if (updateContent) {
            try {
                let jsonObj = JSON.parse(updateContent);
                console.log("updateRecord:", updateContent);
                if (typeof (jsonObj) === "string" || buildingInfo === undefined) {
                    return {};
                }
                if (activeAuditInfo.contentType === "ReportRule") {//报备规则
                    buildingInfo.ruleInfo = jsonObj;
                }
                if (activeAuditInfo.contentType === "BuildingNo") {//楼栋批次
                    buildingInfo.buildingNoInfos = jsonObj || [];
                }
                return jsonObj;
            } catch (e) {}
        }
        return {};
    }

    render() {
        const rootBasicData = (this.props.rootBasicData || {}).dicList || [];
        let curActiveInfo = this.props.curActiveInfo || {};
        let activeAuditInfo = this.props.activeAuditInfo;
        let updateRecord = this.props.curActiveInfo || {};
        let HouseActiveInfo = getToolComponent("houseActiveInfo");
        let BuildingSearchItem = getToolComponent("BuildingSearchAuditInfo");
        let buildingInfo = this.props.buildingOfActiveInfo || {};
        let buildingShops = this.props.buildingOfShops || {};
        const updateContent = this.getDynamicInfo(buildingInfo, curActiveInfo.updateContent);//将楼盘中的信息替换为当时提交的动态信息
        return (
            <div>
                <Spin spinning={this.props.showLoading}>
                    <b>房源动态审核({this.props.subTitle})</b>
                    {
                        activeAuditInfo.contentType !== "Price" ? <BuildingSearchItem contentInfo={buildingInfo} style={{margin: '0.2rem 1rem'}} /> : null
                    }
                    {/**佣金方案**/}
                    {
                        ["ZYWCommissionType", "CommissionType"].includes(activeAuditInfo.contentType) ? <p style={{margin: '1rem'}}>
                            <Row>
                                {activeAuditInfo.contentType == "CommissionType" ? <Col><XYHCommissionInfo buildingInfo={{commissionPlan: updateContent.commissionPlan}} /></Col> : null}
                                {activeAuditInfo.contentType == "ZYWCommissionType" ? <Col><ZYWCommissionInfo buildingInfo={{commissionPlan: updateContent.commissionPlan}} /></Col> : null}
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
                        activeAuditInfo.contentType === "ShopsAdd" ? <p style={{margin: '1rem'}}>
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
                        activeAuditInfo.contentType === "ReportRule" ? <p style={{margin: '1rem'}}>
                            <Row>
                                <Col>
                                    <RulesInfo buildingInfo={buildingInfo} />
                                    <RulesTemplateInfo buildingInfo={buildingInfo} />
                                </Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }
                    {/**热卖户型(热卖没有审核)**/}
                    {
                        activeAuditInfo.contentType === "ShopsHot" ? <p style={{margin: '1rem'}}>
                            <Row>
                                <Col>热卖户型</Col>
                                <Col>
                                    <Menu mode="inline">
                                        {
                                            (groupShops(updateContent.shopList || []).buildings || []).map((x, idx) => {
                                                return (<SubMenu key={x.storied} title={x.storied}>
                                                    {
                                                        x.shops.map(s => {
                                                            return <Menu.Item onClick={() => this.showShopDetail(s)} arrow="horizontal" key={s.id}>{getShopName(s)}</Menu.Item>
                                                        })
                                                    }
                                                </SubMenu >)
                                            })
                                        }
                                    </Menu>
                                </Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }

                    {/**楼栋批次**/}
                    {
                        ["ZYWBuildingNo", "BuildingNo"].includes(activeAuditInfo.contentType) ? <p style={{margin: '1rem'}}>
                            <Row>
                                <Col>
                                    <BuildingNoInfo buildingInfo={buildingInfo} />
                                </Col>
                                <Row>
                                    <Col >备注：{updateRecord.content}</Col>
                                </Row>
                            </Row>
                        </p> : null
                    }
                    {/**优惠政策**/}
                    {
                        ["DiscountPolicy", "ZYWDiscountPolicy"].includes(activeAuditInfo.contentType) ? < p style={{margin: '1rem'}}>
                            <Row>
                                <Col>优惠政策：{updateContent.preferentialPolicies}</Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }
                    {/**楼盘图片**/}
                    {
                        activeAuditInfo.contentType === "BuildingPicture" ? (() => {

                            return (<p style={{margin: '1rem'}}>
                                <Row>
                                    <Col>图片：{updateRecord.updateContent}</Col>
                                </Row>
                                <Row>
                                    <Col >备注：{updateRecord.content}</Col>
                                </Row>
                            </p>)
                        })() : null
                    }
                    {/**商铺图片**/}
                    {
                        activeAuditInfo.contentType === "ShopPicture" ? (() => {

                            return (<p style={{margin: '1rem'}}>
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
                        ["Price"].includes(activeAuditInfo.contentType) ? <p style={{margin: '1rem'}}>
                            <Row>
                                <Col>总价：{updateContent.totalPrice || 0}万元，指导价：{updateContent.guidingPrice || 0}万元</Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }

                    {/***租壹屋相关*****/}
                    {//租壹屋业主信息
                        activeAuditInfo.contentType === "ZYWOwnerInfo" ? <p style={{margin: '1rem'}}>
                            <ZYWOwnerInfo shopInfo={{ownerInfo: updateContent}} />
                        </p> : null
                    }
                    {//租壹屋意向业态
                        activeAuditInfo.contentType === "ZYWIntensionLease" ? <p style={{margin: '1rem'}}>
                            <IntensionLease shopInfo={{intensionLease: updateContent}} basicData={rootBasicData}/>
                        </p> : null
                    }
                    {//租壹屋报备规则
                        activeAuditInfo.contentType === "ZYWReportRule" ? <p style={{margin: '1rem'}}>
                            <Row>
                                <Col>
                                    <ZYWRulesInfo buildingInfo={buildingInfo} />
                                    <ZYWRulesTemplateInfo buildingInfo={buildingInfo} />
                                </Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }
                    {//租壹屋热卖户型
                        activeAuditInfo.contentType === "ZYWShopsHot" ? <p style={{margin: '1rem'}}>
                            <Row>
                                <Col>热卖户型</Col>
                                <Col>
                                    <Menu mode="inline">
                                        {
                                            (groupShops(updateContent.shopList || []).buildings || []).map((x, idx) => {
                                                return (<SubMenu key={x.storied} title={x.storied}>
                                                    {
                                                        x.shops.map(s => {
                                                            return <Menu.Item onClick={() => this.showShopDetail(s)} arrow="horizontal" key={s.id}>{getShopName(s)}</Menu.Item>
                                                        })
                                                    }
                                                </SubMenu >)
                                            })
                                        }
                                    </Menu>
                                </Col>
                            </Row>
                            <Row>
                                <Col >备注：{updateRecord.content}</Col>
                            </Row>
                        </p> : null
                    }
                    {/**租壹屋价格**/}
                    {
                        ["ZYWPrice"].includes(activeAuditInfo.contentType) ? <p style={{margin: '1rem'}}>
                            <Row>
                                <Col>租金(元/㎡/月)：{updateContent.price || 0}</Col>
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
        rootBasicData: state.rootBasicData,
        activeAuditInfo: state.audit.activeAuditInfo,
        activeAuditHistory: state.audit.activeAuditHistory,
        buildingOfActiveInfo: state.audit.buildingOfActiveInfo,
        buildingOfShops: state.audit.buildingOfShops,
        curActiveInfo: state.audit.curActiveInfo
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditHouseNewInfo);