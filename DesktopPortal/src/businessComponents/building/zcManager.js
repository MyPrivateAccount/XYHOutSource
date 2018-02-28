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
        let buildInfo = this.props.buildInfo || {};
        return (
            <Layout>
                <Content className='content' >
                    <Form layout="horizontal" >
                        <Row type="flex" style={{padding: '1rem 0'}}>
                            <Col span={23}>
                                <Icon type="tags-o" className='content-icon' /> <span className='content-title'>驻场信息</span>
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                        </Row>
                        <div style={{display: this.state.expandStatus ? "block" : "none"}}>
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
                        </div>
                    </Form>
                </Content>
            </Layout>
        )
    }
}

/*function mapStateToProps(state) {
    return {
        buildInfo: state.search.activeBuilding,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(ZcmanagerInfo);*/
export default ZcmanagerInfo;