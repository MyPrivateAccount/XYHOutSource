import {connect} from 'react-redux';
import React, {Component} from 'react'
import {Form, Icon, Button, Layout, Row, Col} from 'antd'
import moment from 'moment'

const {Header, Sider, Content} = Layout;
class RulesInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {
    }


    render() {
        let ruleInfo = (this.props.buildingInfo || {}).ruleInfo || {};
        // ruleInfo.validityDay = ruleInfo.validityDay ? ruleInfo.validityDay  : '无';
        // ruleInfo.beltProtectDay = ruleInfo.beltProtectDay ? ruleInfo.beltProtectDay  : '无';
        // ruleInfo.maxCustomer = ruleInfo.maxCustomer ? ruleInfo.maxCustomer : '无';
        return (
            <Layout>
                <Content className='content' >
                    <div style={{marginTop: '15px', backgroundColor: "#ECECEC"}}>
                        <Form layout="horizontal" >
                            <Row type="flex" style={{padding: '1rem 0'}}>
                                <Col span={23}>
                                    <Icon type="tags-o" className='content-icon' /><span className='content-title'>报备规则</span>
                                </Col>
                                <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                            </Row>
                            <div style={{display: this.state.expandStatus ? "block" : "none"}}>
                                <Row className='viewRow'>
                                    <Col span={12}>报备有效期：{ruleInfo.validityDay ? <span>{ruleInfo.effectiveType == '2' ? ruleInfo.validityDay + " 小时" : ruleInfo.validityDay + " 天"}</span> : ' 无 '} </Col>
                                    <Col span={12}>带看保护期：{ruleInfo.beltProtectType == '0' ? "永久" : (ruleInfo.beltProtectDay ? ` ${ruleInfo.beltProtectDay} 天 ` : ' 无 ')} </Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12}>向开发商报备信息上限：{ruleInfo.maxCustomer ? ` ${ruleInfo.maxCustomer}  条 ` : ' 无 '}</Col>
                                    <Col span={12}>带看时间：{moment(ruleInfo.liberatingStart).format('HH:mm')} —— {moment(ruleInfo.liberatingEnd).format('HH:mm')}</Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12}>报备开始日期：{ruleInfo.reportTime ? moment(ruleInfo.reportTime).format('YYYY-MM-DD') : '无'}</Col>
                                    <Col span={12}>报备提前时间：{ruleInfo.advanceTime ? ` ${ruleInfo.advanceTime}  分钟 ` : ' 无 '}</Col>
                                </Row>
                                <Row className='viewRow' style={{color: 'red'}}>
                                    <Col span={24}>
                                        {ruleInfo.isCompletenessPhone ? '开发商要求报备时显示客户手机完整号码！' : null}
                                    </Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={24}>备注：{ruleInfo.mark || '无'}</Col>
                                </Row>
                            </div>
                        </Form>
                    </div>
                </Content>
            </Layout>
        )
    }
}

export default RulesInfo;