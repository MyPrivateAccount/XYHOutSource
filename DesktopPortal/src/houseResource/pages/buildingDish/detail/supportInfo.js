import { connect } from 'react-redux';
import { editSupportInfo } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Table, Button, Checkbox, Form, Row, Col, Icon } from 'antd'
import BasicInfo from './basicInfo';

class SupportInfo extends Component {
    state = {

    }
    componentWillMount() {

    }

    handleEdit = (e) => {
        this.props.dispatch(editSupportInfo());
    }

    render() {
        let { supportInfo } = this.props.buildInfo;
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal" >
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>配套信息</span>
                        </Col>
                        <Col span={4}>
                            {
                                [1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={4}>交通情况</Col>
                        <Col span={20}>
                            <Row>
                                <Col>公交：{supportInfo.busDesc || "暂无"}</Col>
                            </Row>
                            <Row>
                                <Col span={4}>轨道交通：{supportInfo.railDesc || "暂无"}</Col>
                            </Row>
                            <Row>
                                <Col span={4}>其他交通方式：{supportInfo.otherTrafficDesc || "暂无"}</Col>
                            </Row>
                        </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={4}>项目配套</Col>
                        <Col span={20}>
                            <Row>
                                <Col span={4}>幼儿园：{supportInfo.kindergartenDesc || "暂无"}</Col>
                            </Row>
                            <Row>
                                <Col span={4}>小学：{supportInfo.primarySchoolDesc || "暂无"}</Col>
                            </Row>
                            <Row>
                                <Col span={4}>中学：{supportInfo.middleSchoolDesc || "暂无"}</Col>
                            </Row>
                            <Row>
                                <Col span={4}>大学：{supportInfo.universityDesc || "暂无"}</Col>
                            </Row>
                            <Row>
                                <Col span={4}>商场：{supportInfo.marketDesc || "暂无"}</Col>
                            </Row>
                            <Row>
                                <Col span={4}>超市：{supportInfo.supermarketDesc || "暂无"}</Col>
                            </Row>
                            <Row>
                                <Col span={4}>医院：{supportInfo.hospitalDesc || "暂无"}</Col>
                            </Row>
                            <Row>
                                <Col span={4}>银行：{supportInfo.bankDesc || "暂无"}</Col>
                            </Row>
                        </Col>
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
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SupportInfo);