import React, {Component} from 'react'
import {Form, Icon, Row, Col, Layout} from 'antd'
import moment from 'moment'
import {getDicPars} from '../../utils/utils'

const {Header, Sider, Content} = Layout;
class IntensionLease extends Component {
    state = {
        expandStatus: true
    }

    getDicText(key, value) {
        let basicData = this.props.basicData || [];
        let findDic = getDicPars(key, basicData);
        if (findDic) {
            // return result.key;
            return (findDic.find(dic => dic.value == value) || {}).key;
        }
        return '';
    }

    render() {
        let detail = (this.props.shopInfo || {}).intensionLease || {};
        return (
            <div className="relative">
                <Layout>
                    <Content className='content' >
                        <Form layout="horizontal" >
                            <Row type="flex" style={{padding: '1rem 0'}}>
                                <Col span={23} >
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>意向租约</span>
                                </Col>
                                <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                            </Row>
                            <div style={{display: this.state.expandStatus ? "block" : "none"}}>
                                <Row className='viewRow'>
                                    <Col span={12} >租金(元/月)：{detail.rental}</Col>
                                    <Col span={12}>押金方式：{this.getDicText("SHOP_DEPOSITTYPE", detail.depositType)}</Col>
                                </Row>
                                {
                                    ((detail.depositType * 1) === 10 || (detail.depositType * 1) === 20) ?
                                        <Row className='viewRow'>
                                            <Col span={12} >押金：{detail.deposit}</Col>
                                        </Row> : null
                                }
                                <Row className='viewRow'>
                                    <Col span={12} >转让费(元)：{detail.assignmentFee}</Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12} >支付方式：{this.getDicText("SHOP_LEASE_PAYMENTTIME", detail.paymentTime)}</Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12} >递增方式：从第几年开始每隔多少年递增多少比例</Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12} >递增起始年：{detail.upscaleStartYear}</Col>
                                    <Col span={12} >递增间隔：{detail.upscaleInterval}</Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12} >递增比率(%)：{detail.upscale}</Col>
                                    <Col span={12} >备注：{detail.memo}</Col>
                                </Row>
                            </div>
                        </Form>
                    </Content>
                </Layout>
            </div>
        )
    }
}

export default IntensionLease;