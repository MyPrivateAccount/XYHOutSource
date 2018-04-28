import React, {Component} from 'react';
import moment from 'moment';
import {Layout, Form, Col, Row, Icon} from 'antd';


const {Header, Sider, Content} = Layout;

class LeaseInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }

    render() {
        let leaseInfo = (this.props.shopInfo || {}).leaseInfo || {};
        let startDate = leaseInfo.startDate ? moment(leaseInfo.startDate).format('YYYY-MM-DD') : '';
        let endDate = leaseInfo.endDate ? moment(leaseInfo.endDate).format('YYYY-MM-DD') : '';
        return (
            <div className="relative">
                <Layout>
                    <Content className='content'>
                        <Form layout="horizontal" >
                            <Row type="flex" style={{padding: '1rem 0'}}>
                                <Col span={23} >
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>租约信息</span>
                                </Col>
                                <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                            </Row>
                            <div style={{display: this.state.expandStatus ? "block" : "none"}}>
                                <Row className='viewRow'>
                                    <Col span={12} >
                                        当前经营：<span className='boldSpan'>{leaseInfo.currentOperation}</span>
                                    </Col>
                                    <Col span={12}>起止时间：<span className='boldSpan'>{startDate} ~ {endDate}</span></Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12}>租金：<span className='redSpan'>{leaseInfo.rental} 元/月</span></Col>
                                    <Col span={12}>押金：<span className='redSpan'>{leaseInfo.deposit} 元</span></Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12}>返祖比列：<span className='boldSpan'>{leaseInfo.backRate} %</span></Col>
                                    <Col span={12}>返祖时间:<span className='boldSpan'>{leaseInfo.backMonth} %</span></Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12}>递增比率：<span className='boldSpan'>{leaseInfo.upscale} %</span></Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={24}>
                                        {
                                            leaseInfo.backRate !== null ? <span>备注：<span className='boldSpan'>{leaseInfo.backRate}</span></span> : null
                                        }
                                    </Col>
                                </Row>
                            </div>
                        </Form>
                    </Content>
                </Layout>
            </div>
        )
    }
}

export default LeaseInfo;