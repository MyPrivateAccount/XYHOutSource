// import {connect} from 'react-redux';
//import { editSupportInfo } from '../../../actions/actionCreator';
import React, {Component} from 'react'
import {Layout, Form, Row, Col, Icon} from 'antd'
// import BasicInfo from './basicInfo';

const {Header, Sider, Content} = Layout;

class SupportInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }

    render() {
        let supportInfo = (this.props.buildingInfo || {}).facilitiesInfo || {};
        return (
            <Layout>
                <Content className='content' >
                    <Form layout="horizontal" >
                        <Row type="flex" style={{padding: '1rem 0', backgroundColor: "#ECECEC"}}>
                            <Col span={23}>
                                <Icon type="tags-o" className='content-icon' /> <span className='content-title'>配套信息</span>
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                        </Row>
                        <div style={{display: this.state.expandStatus ? "block" : "none"}}>
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
                        </div>
                    </Form>
                </Content>
            </Layout>
        )
    }
}
export default SupportInfo;