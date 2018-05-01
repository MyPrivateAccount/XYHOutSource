import React, {Component} from 'react'
import {Layout, Form, Col, Row, Icon} from 'antd';
const {Header, Sider, Content} = Layout;

class OwnerInfo extends Component {
    state = {
        expandStatus: true
    }
    render() {
        let detail = (this.props.shopInfo || {}).ownerInfo || {};
        return (
            <div className="relative">
                <Layout>
                    <Content className='content' >
                        <Form layout="horizontal" >
                            <Row type="flex" style={{padding: '1rem 0'}}>
                                <Col span={23} >
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>业主信息</span>
                                </Col>
                                <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                            </Row>
                            <div style={{display: this.state.expandStatus ? "block" : "none"}}>
                                <Row className='viewRow'>
                                    <Col >
                                        <div style={{paddingRight: 10}}>姓名：{detail.name}</div>
                                    </Col>
                                    <Col >
                                        <div style={{paddingRight: 10}} >电话：{detail.tel}</div>
                                    </Col>
                                </Row>
                                <br />
                                {
                                    detail.cardNo ?
                                        <Row className='viewRow'>
                                            <Col >
                                                <div style={{paddingRight: 10}}>身份证：{detail.cardNo}</div>
                                            </Col>
                                        </Row> : null
                                }

                            </div>
                        </Form>
                    </Content>
                </Layout>
            </div>
        )
    }
}
export default OwnerInfo;