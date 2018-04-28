import React, {Component} from 'react'
import {Form, Icon, Row, Col, Layout} from 'antd'

const {Header, Sider, Content} = Layout;
class ShopSummaryInfo extends Component {
    state = {
        expandStatus: true
    }
    render() {
        let summary = (this.props.shopInfo || {}).summary;
        return (
            <Layout>
                <Content className='content' >
                    <Form layout="horizontal" >
                        <div style={{backgroundColor: "#ECECEC"}}>
                            <Row type="flex" style={{padding: '1rem 0'}}>
                                <Col span={23}>
                                    <Icon type="tags-o" className='content-icon' /><span className='content-title'>商铺简介</span>
                                </Col>
                                <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                            </Row>
                            <Row className='viewRow' style={{display: this.state.expandStatus ? "block" : "none"}}>
                                <Col span={24}>{summary || '暂无'}</Col>
                            </Row>
                        </div>
                    </Form>
                </Content>
            </Layout>
        )
    }
}

export default ShopSummaryInfo;