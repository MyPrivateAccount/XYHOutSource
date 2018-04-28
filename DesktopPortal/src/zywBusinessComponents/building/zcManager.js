// import {connect} from 'react-redux';
import React, {Component} from 'react'
import {Layout, Form, Icon, Row, Col} from 'antd'

const {Header, Sider, Content} = Layout;
class ZcmanagerInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }

    render() {
        let buildInfo = this.props.buildingInfo || {};
        let isNoResidentUser = false;
        if ((buildInfo.residentUser1Info || {}).id == null && (buildInfo.residentUser2Info || {}).id == null
            && (buildInfo.residentUser3Info || {}).id == null && (buildInfo.residentUser4Info || {}).id == null) {
            isNoResidentUser = true;
        }
        return (
            <Layout>
                <Content className='content' >
                    <div style={{marginTop: '15px', backgroundColor: "#ECECEC"}}>
                        <Row type="flex" style={{padding: '1rem 0'}}>
                            <Col span={23}>
                                <Icon type="tags-o" className='content-icon' /> <span className='content-title'>驻场信息</span>
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                        </Row>
                        <div style={{display: this.state.expandStatus ? "block" : "none"}}>
                            {isNoResidentUser ? <Row className='viewRow'>
                                {
                                    buildInfo.createUserInfo ? <Col span={12}> {`${buildInfo.createUserInfo.userName} ： ${buildInfo.createUserInfo.phoneNumber}`}</Col> : null
                                }
                            </Row> : <div>
                                    <Row className='viewRow'>
                                        {buildInfo.residentUserManager && buildInfo.residentUserManager.id ? <Col> {buildInfo.residentUserManager.userName}(驻场经理) ： {buildInfo.residentUserManager.phoneNumber}</Col> : null}
                                    </Row>
                                    <Row className='viewRow'>
                                        {
                                            buildInfo.residentUser1 ? <Col span={12}> {`${buildInfo.residentUser1Info.userName} ： ${buildInfo.residentUser1Info.phoneNumber}`}</Col> : null
                                        }
                                        {
                                            buildInfo.residentUser2 ? <Col span={12}> {`${buildInfo.residentUser2Info.userName} ： ${buildInfo.residentUser2Info.phoneNumber}`}</Col> : null
                                        }
                                    </Row>
                                    <Row className='viewRow'>
                                        {
                                            buildInfo.residentUser3 ? <Col span={12}> {`${buildInfo.residentUser3Info.userName} ： ${buildInfo.residentUser3Info.phoneNumber}`}</Col> : null
                                        }
                                        {
                                            buildInfo.residentUser4 ? <Col span={12}> {`${buildInfo.residentUser4Info.userName} ： ${buildInfo.residentUser4Info.phoneNumber}`}</Col> : null
                                        }
                                    </Row>
                                </div>}
                        </div>
                    </div>
                </Content>
            </Layout>
        )
    }
}

export default ZcmanagerInfo;