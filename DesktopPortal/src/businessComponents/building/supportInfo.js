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
                    <div style={{marginTop: '15px', backgroundColor: "#ECECEC"}}>
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
                                        <Col><b>公交：</b>{supportInfo.busDesc || "暂无"}</Col>
                                    </Row>
                                    <Row>
                                        <Col><b>轨道交通：</b>{supportInfo.railDesc || "暂无"}</Col>
                                    </Row>
                                    <Row>
                                        <Col><b>其他交通方式：</b>{supportInfo.otherTrafficDesc || "暂无"}</Col>
                                    </Row>
                                </Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={4}>项目配套</Col>
                                <Col span={20}>
                                    <Row>
                                        <Col><b>幼儿园：</b>{supportInfo.kindergartenDesc || "暂无"}</Col>
                                    </Row>
                                    <Row>
                                        <Col><b>小学：</b>{supportInfo.primarySchoolDesc || "暂无"}</Col>
                                    </Row>
                                    <Row>
                                        <Col><b>中学：</b>{supportInfo.middleSchoolDesc || "暂无"}</Col>
                                    </Row>
                                    <Row>
                                        <Col><b>大学：</b>{supportInfo.universityDesc || "暂无"}</Col>
                                    </Row>
                                    <Row>
                                        <Col><b>商场：</b>{supportInfo.marketDesc || "暂无"}</Col>
                                    </Row>
                                    <Row>
                                        <Col><b>超市：</b>{supportInfo.supermarketDesc || "暂无"}</Col>
                                    </Row>
                                    <Row>
                                        <Col><b>医院：</b>{supportInfo.hospitalDesc || "暂无"}</Col>
                                    </Row>
                                    <Row>
                                        <Col><b>银行：</b>{supportInfo.bankDesc || "暂无"}</Col>
                                    </Row>
                                </Col>
                            </Row>
                        </div>
                    </div>
                </Content>
            </Layout>
        )
    }
}
export default SupportInfo;