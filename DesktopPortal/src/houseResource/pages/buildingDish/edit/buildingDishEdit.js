import { connect } from 'react-redux';
import { getDicParList, submitBuildInfo } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import * as actionTypes from '../../../constants/actionType';
import { Layout, Table, Button, Checkbox, Radio, Row, Col, Icon, Anchor, BackTop, Modal, notification } from 'antd'
import AttachEdit from '../../shops/edit/attachEdit';
import BasicEdit from './basicEdit';
import BasicInfo from '../detail/basicInfo';
import ProjectEdit from './projectEdit';
import ProjectInfo from '../detail/projectInfo';
import RelShopsEdit from './relShopsEdit';
import RelShopsInfo from '../detail/relShopsInfo';
import SupportEdit from './supportEdit';
import SupportInfo from '../detail/supportInfo';
import BuildingNoEdit from './buildingNoEdit';
import BuildingNoInfo from '../detail/buildingNoInfo';
import RulesEdit from './rulesEdit';
import RulesInfo from '../detail/rulesInfo';
import RulesTemplateEdit from './rulesTemplateEdit';
import RulesTemplateInfo from '../detail/rulesTemplateInfo';
import CommissionEdit from './commissionEdit';
import CommissionInfo from '../detail/commissionInfo';
import AttachInfo from '../../shops/detail/attachInfo'
import { buildInfoGroup } from '../../../constants/commonDefine';
import './buildingDish.less';
import { isEmptyObject } from './utils'


const { Header, Sider, Content } = Layout;
const { Link } = Anchor;

class BuildingDishEdit extends Component {
    state = {
    }
    getGroup = (fl)  => {
        
    }
    handleOk = (e) => {
        // console.log(this.props.buildInfo.attachInfo.fileList, this.props.buildInfo, '6',this.props.basicData.photoCategories)
        let a = [], fileList = (this.props.buildInfo.attachInfo || {}).fileList || []
        let list= {}
        if(fileList && fileList.length>0){
            fileList.forEach(v => {
                v.group = v.group ? v.group : '5'
                if (!list.hasOwnProperty(v.group)) {
                    list[v.group] = [v]
                } else {
                    list[v.group].push(v)
                }
            })
        }
        this.props.basicData.photoCategories.forEach(v=>{
            if(v.ext1 === '1') {
                console.log(1111)
                if((list[v.value*1]||[]).length === 0){
                    a.push(v.key)
                }
            }
        })
        if(a.length>0){
            notification.warning({
                message: `请上传${a.join(',')}分类下的图片`,
                duration: 3
            })
            return
        }
        const buildInfo = this.props.buildInfo||{};
        const bi = buildInfo.basicInfo||{}; // 基本信息
        const fi = buildInfo.supportInfo ||{}; // 配套信息
        const si = buildInfo.relShopInfo||{}; // 商铺整体情况
        // const mi = buildInfo.commissionPlan;
        // const ri = buildInfo.ruleInfo || {};
        const li = buildInfo.projectInfo || {}; // 项目简介

        const hasBasicInfo = !isEmptyObject(bi);
        const hasFi =  !isEmptyObject(fi);
        const hasSi = !isEmptyObject(si);
        const hasSum = !isEmptyObject(li);
        console.log(hasBasicInfo, hasFi, hasSi,hasSum)
        // const hasMi = mi !== '';
        // const hasRi = !isEmptyObject(ri, reportRule);
        // const hasSum = ii !== '';
        if(!hasBasicInfo  || !hasSi || !hasFi || !hasSum){
            notification.warning({
                message: '请先完善楼盘信息，再提交',
                duration: 3
            })
            return;
        }
        this.props.dispatch(submitBuildInfo({ entity: { id: buildInfo.id } }))
    }
    componentWillMount() {
        if (this.props.basicData.saleStatus.length === 0
            && this.props.basicData.saleModel.length === 0) {
            this.props.dispatch(getDicParList(["TRADE_MIXPLANNING", "SALE_MODE", "PROJECT_SALE_STATUS", "SHOP_CATEGORY", "SHOP_SALE_STATUS",'PHOTO_CATEGORIES']));
        }
    }
    handleAnchorChange = (e) => {
        window.location.href = "#" + e.target.value;
    }


    render() {
        let operInfo = this.props.operInfo;
        const submitLoading = this.props.submitLoading;

        return (
            <div className="relative">
                <Layout>
                    <Content className='content buildingDish' style={{ marginTop: '25px' }}>
                        <Row type="flex" justify="space-between">
                            <Col span={4} style={{ textAlign: 'left' }}>
                                {/* {this.props.buildInfo.examineStatus === 8 ? null :
                                    <Button type="primary" size='large'
                                        style={{ width: "10rem", display: this.props.buildDisplay }}
                                        onClick={this.handleOk} loading={submitLoading}>提交</Button>
                                } */}
                            </Col>
                            <Col span={20} style={{ textAlign: 'right' }}>
                                <Radio.Group defaultValue='basicInfo' onChange={this.handleAnchorChange} size='large'>
                                    {
                                        buildInfoGroup.map((info) => <Radio.Button value={info.id} key={info.id}>{info.name}<Icon type="check" /></Radio.Button>)
                                    }
                                </Radio.Group>
                            </Col>
                        </Row>

                        {/* <Row>
                            {
                                this.props.buildInfo.examineStatus !== 16 ? null :
                                <p style={{color: 'red', fontWeight: 'bold'}}>
                                    驳回原因：XXXXXXX
                                </p>
                            }
                        </Row> */}

                        <Row id="basicInfo">
                            {/**
                                 * 基本信息
                                 */}
                            <Col span={24}>{operInfo.basicOperType === 'view' ? <BasicInfo /> : <BasicEdit />}</Col>
                        </Row>
                        <Row id="batchBuildingInfo">
                            {/**
                                 * 楼栋批次信息
                                 */}
                            <Col span={24}>{operInfo.batchBuildOperType === "view" ? <BuildingNoInfo /> : <BuildingNoEdit />}</Col>
                        </Row>
                        <Row id="supportInfo">
                            {/**
                                 * 配套信息
                                 */}
                            <Col span={24}>{operInfo.supportOperType === "view" ? <SupportInfo /> : <SupportEdit />}</Col>
                        </Row>
                        <Row id="relShopsInfo">
                            {/**
                                 * 商铺整体概况
                                 */}
                            <Col span={24}>{operInfo.relShopOperType === "view" ? <RelShopsInfo /> : <RelShopsEdit />}</Col>
                        </Row>
                        <Row id="projectInfo">
                            {/**
                                 * 项目简介
                                 */}
                            <Col span={24}>{operInfo.projectOperType === "view" ? <ProjectInfo /> : <ProjectEdit />}</Col>
                        </Row>
                        <Row id="rulesInfo">
                            {/**
                                 * 报备规则
                                 */}
                            <Col span={24}>
                                {
                                    operInfo.rulesOperType === "view" ? <RulesInfo /> : <RulesEdit />
                                }
                                {
                                    operInfo.rulesTemplateOperType === "view" ? <RulesTemplateInfo /> : <RulesTemplateEdit />
                                }
                            </Col>
                        </Row>
                        <Row id="commissionInfo">
                            {/**
                                 * 佣金方案
                                 */}
                            <Col span={24}>{operInfo.commissionOperType === "view" ? <CommissionInfo /> : <CommissionEdit />}</Col>
                        </Row>
                        
                        <Row id="attachInfo">
                            {/**
                                 * 附加信息
                                 */}
                            <Col span={24}>{operInfo.attachPicOperType === "view" ? <AttachInfo parentPage="building" type='building' /> : <AttachEdit parentPage="building" type='building' />}</Col>
                        </Row>
                        <div>
                            <BackTop visibilityHeight={400} />
                        </div>
                        <Row type="flex" justify="space-between">
                            <Col span={24} style={{ display: 'flex', justifyContent: 'flex-end',margin: '5px  0 25px 0' }}>
                                {[8, 1].includes(this.props.buildInfo.examineStatus)  ? null :
                                    <Button type="primary" size='large'
                                        style={{ width: "10rem", display: this.props.buildDisplay }}
                                        onClick={this.handleOk} loading={submitLoading}>提交</Button>
                                }
                            </Col>
                        </Row>
                    </Content>
                </Layout>
            </div>
        )
    }
}

function mapStateToProps(state) {
    // console.log('BuildingDishDetail MapStateToProps:', state.oidc.user); 
    return {
        buildInfo: state.building.buildInfo,
        basicData: state.basicData,
        operInfo: state.building.operInfo,
        submitLoading: state.building.submitLoading,
        buildDisplay: state.building.buildDisplay,
        user: (state.oidc.user || {}).profile || {}, // 有个City字段
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BuildingDishEdit);