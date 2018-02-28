import { connect } from 'react-redux';
//import { editShopSupport } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Popconfirm, Tag, Form, Col, Row, Icon } from 'antd'

const { Header, Sider, Content } = Layout;
const FormItem = Form.Item;

class SupportInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }

    render() {
        let supportInfo = this.props.supportInfo
        return (
            <div className="relative">
                <Layout>
                    <Content className='content' >
                        <Form layout="horizontal" >
                            <Row type="flex" style={{ padding: '1rem 0' }}>
                                <Col span={23} >
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>配套设施</span>
                                </Col>
                                <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({ expandStatus: !this.state.expandStatus })} /></Col>
                            </Row>
                            <div style={{ display: this.state.expandStatus ? "block" : "none" }}>
                                <Row className='viewRow'>
                                    <Col span={24} >
                                        {supportInfo.upperWater ? <Tag color="rgb(16, 142, 233)">上水</Tag> : null}
                                        {supportInfo.downWater ? <Tag color="rgb(16, 142, 233)">下水</Tag> : null}
                                        {supportInfo.gas ? <Tag color="rgb(16, 142, 233)">天然气</Tag> : null}
                                        {supportInfo.chimney ? <Tag color="rgb(16, 142, 233)">烟管道 </Tag> : null}
                                        {supportInfo.blowoff ? <Tag color="rgb(16, 142, 233)">排污管道 </Tag> : null}
                                        {supportInfo.split ? <Tag color="rgb(16, 142, 233)">可分割 </Tag> : null}
                                        {supportInfo.elevator ? <Tag color="rgb(16, 142, 233)">电梯 </Tag> : null}
                                        {supportInfo.staircase ? <Tag color="rgb(16, 142, 233)">扶梯 </Tag> : null}
                                        {supportInfo.outside ? <Tag color="rgb(16, 142, 233)">外摆区 </Tag> : null}
                                        {supportInfo.openFloor ? <Tag color="rgb(16, 142, 233)">架空层 </Tag> : null}
                                        {supportInfo.parkingSpace ? <Tag color="rgb(16, 142, 233)">停车位 </Tag> : null}
                                    </Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={24} >
                                        {supportInfo.voltage ? <Tag color="rgb(16, 142, 233)">{supportInfo.voltage}V</Tag> : null}
                                        {supportInfo.capacitance ? <Tag color="rgb(16, 142, 233)">{supportInfo.capacitance}F</Tag> : null}
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

function mapStateToProps(state) {
    return {
        supportInfo: state.search.activeShop.facilitiesInfo,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SupportInfo);