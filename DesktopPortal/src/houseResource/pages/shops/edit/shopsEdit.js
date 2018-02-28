import { connect } from 'react-redux';
import { submitShopsInfo, getDicParList } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Row, Col, Icon, Radio, BackTop, Modal, notification } from 'antd'
import AttachEdit from './attachEdit';
import AttachInfo from '../detail/attachInfo';
import BasicEdit from './basicEdit';
import BasicInfo from '../detail/basicInfo';
import LeaseEdit from './leaseEdit';
import LeaseInfo from '../detail/leaseInfo';
import SupportEdit from './supportEdit';
import SupportInfo from '../detail/supportInfo';
import ShopSummaryEdit from './shopSummaryEdit';
import ShopSummaryInfo from '../detail/shopSummaryInfo';
import { shopsInfoGroup } from '../../../constants/commonDefine';
import { isEmptyObject } from '../../buildingDish/edit/utils'
const { Header, Sider, Content } = Layout;

class Shops extends Component {
    state = {
    }
    componentWillMount () {
        this.props.dispatch(getDicParList(["TRADE_MIXPLANNING", "SALE_MODE",
         "SHOP_CATEGORY", "SHOP_TOWARD", 'SHOP_DEPOSITTYPE',
        "SHOP_STATUS", "SHOP_LEASE_PAYMENTTIME", "SHOP_SALE_STATUS",'PHOTO_CATEGORIES']));
    }
    handleOk = (e) => {
        const shopsInfo = this.props.shopsInfo
        const bi = shopsInfo.basicInfo || {}; // 基本信息
        const fi = shopsInfo.supportInfo||{}; // 配套信息
        console.log(shopsInfo, 123)
        const hasBasicInfo = !isEmptyObject(bi);
        const hasFi =  !isEmptyObject(fi);
        if(!hasBasicInfo  || !hasFi){
            notification.warning({
                message: '请先完善商铺信息，再提交',
                duration: 3
            })
            return;
        }
        this.props.submitShopsInfo({ entity: { id: shopsInfo.id, buildingId: shopsInfo.buildingId } })
    }
    
    handleAnchorChange = (e) => {
        window.location.href = "#" + e.target.value;
    }

    render() {
        let operInfo = this.props.operInfo;
        let shopsInfo = this.props.shopsInfo;
        const submitLoading = this.props.submitLoading;
        return (
            <div className="relative">
                <Layout>
                    <Content className='content' style={{ marginTop: '25px'}}>
                        <div>
                            <Row type="flex" justify="space-between" >
                                <Col span={12} style={{ textAlign: 'left' }}>
                                {/* {   this.props.shopsInfo.examineStatus === 8 ? null :
                                     <Button type="primary" size='large' style={{ width: "10rem" }} 
                                     onClick={this.handleOk} loading={submitLoading}>提交</Button>
                                } */}
                                </Col>
                                <Col span={12} style={{ textAlign: 'right' }}>
                                    <Radio.Group defaultValue='basicInfo' onChange={this.handleAnchorChange} size='large'>
                                        {
                                            shopsInfoGroup.map((info) => <Radio.Button value={info.id} key={info.id}>{info.name}<Icon type="check" /></Radio.Button>)
                                        }
                                    </Radio.Group></Col>
                            </Row>
                            {/* <Row>
                                {
                                    this.props.shopsInfo.examineStatus !== 16 ? null :
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
                            <Row id="leaseInfo">
                                {/**
                                 * 租约信息
                                 */}
                                <Col span={24}>{operInfo.leaseOperType === 'view' ? <LeaseInfo /> : <LeaseEdit />}</Col>
                            </Row>
                            <Row id="supportInfo">
                                {/**
                                 * 配套设置
                                 */}
                                <Col span={24}>{operInfo.supportOperType === 'view' ? <SupportInfo /> : <SupportEdit />}</Col>
                            </Row>
                            <Row id="summaryInfo">
                                {/**
                                 * 简介
                                 */}
                                <Col span={24}>{operInfo.projectOperType === 'view' ? <ShopSummaryInfo /> : <ShopSummaryEdit />}</Col>
                            </Row>
                            <Row id="attachInfo">
                                {/**
                                 * 附加信息
                                 */}
                                <Col span={24}>{operInfo.attachPicOperType === 'view' ? <AttachInfo parentPage='shops' type='shops'/> : <AttachEdit parentPage='shops' type='shops'/>}</Col>
                            </Row>
                            <Row type="flex" justify="space-between">
                            <Col span={24} style={{ display: 'flex', justifyContent: 'flex-end',margin: '0  0 25px 0' }}>
                                {
                                   [1, 8].includes(this.props.shopsInfo.examineStatus)  ? null :
                                   <Button type="primary" size='large' style={{ width: "10rem" }} 
                                   onClick={this.handleOk} loading={submitLoading}>提交</Button>
                                }
                            </Col>
                        </Row>
                        </div>

                    </Content>
                </Layout>

            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        shopsInfo: state.shop.shopsInfo,
        operInfo: state.shop.operInfo,
        submitLoading: state.shop.submitLoading
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch,
        submitShopsInfo: (...args) => dispatch(submitShopsInfo(...args))
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(Shops);