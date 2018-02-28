import { connect } from 'react-redux';
//import { editRelshopInfo } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Form, Icon, Button, Checkbox, Row, Col } from 'antd'
import BasicInfo from './basicInfo';

const { Header, Sider, Content } = Layout;
class RelShopsInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }


    getTextByCode(dic, code) {
        let text;
        let isArray = Array.isArray(code);
        dic.map(item => {
            if (isArray) {
                if (code.find((c) => c === item.value)) {
                    text = item.key;
                }
            } else {
                if (item.value === code) {
                    text = item.key;
                }
            }
        });
        return text;
    }

    render() {
        let relShopInfo = this.props.buildInfo.shopInfo;
        return (
            <Layout>
                <Content className='content' >
                    <Form layout="horizontal" >
                        <Row type="flex" style={{ padding: '1rem 0' }}>
                            <Col span={23}>
                                <Icon type="tags-o" className='content-icon' /> <span className='content-title'>商铺整体概况</span>
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({ expandStatus: !this.state.expandStatus })} /></Col>
                        </Row>
                        <div style={{ display: this.state.expandStatus ? "block" : "none" }}>
                            <Row className='viewRow'>
                                <Col span={12}>销售状态：{this.getTextByCode(this.props.basicData.saleStatus, relShopInfo.saleStatus)}</Col>
                                <Col span={12}>商铺类别：{this.getTextByCode(this.props.basicData.shopsTypes, relShopInfo.shopCategory)}</Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12}>销售模式：{this.getTextByCode(this.props.basicData.saleModel, relShopInfo.saleMode)}</Col>
                                <Col span={12}>周边三公里居住人数：{relShopInfo.populations}</Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={24}>业态规划：{this.getTextByCode(this.props.basicData.tradePlannings, relShopInfo.tradeMixPlanning)}</Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={24}>优惠政策：{relShopInfo.preferentialPolicies}</Col>
                            </Row>
                        </div>
                    </Form>
                </Content>
            </Layout>
        )
    }
}

function mapStateToProps(state) {
    //console.log('BuildingDishDetail MapStateToProps:' + JSON.stringify(state));
    return {
        buildInfo: state.search.activeBuilding,
        basicData: state.basicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(RelShopsInfo);