import { connect } from 'react-redux';
import { editRelshopInfo } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Form, Icon, Button, Checkbox, Row, Col } from 'antd'
import BasicInfo from './basicInfo';


class RelShopsInfo extends Component {
    state = {

    }
    componentWillMount() {

    }

    handleEdit = (e) => {
        this.props.dispatch(editRelshopInfo());
    }
    getTextByCode(dic, code) {
        let text, textArray = [];
        let isArray = Array.isArray(code);
        dic.map(item => {
            if (isArray) {
                if (code.find((c) => c === item.value)) {
                    textArray.push(item.key);
                }
            } else {
                if (item.value === code) {
                    text = item.key;
                }
            }
        });
        return text || textArray.join('、');
    }

    render() {
        let { relShopInfo } = this.props.buildInfo;
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal" >
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>商铺整体概况</span>
                        </Col>
                        <Col span={4}>
                            {
                                [1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                    </Row>
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
                </Form>
            </div>
        )
    }
}

function mapStateToProps(state) {
    //console.log('BuildingDishDetail MapStateToProps:' + JSON.stringify(state));
    return {
        buildInfo: state.building.buildInfo,
        basicData: state.basicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(RelShopsInfo);