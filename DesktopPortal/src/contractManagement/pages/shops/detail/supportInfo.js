import { connect } from 'react-redux';
import { editShopSupport } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Form, Col, Row, Icon, Tag } from 'antd'
import {TAG_COLOR} from '../../../../constants/uiColor'
const { Header, Sider, Content } = Layout;

class SupportInfo extends Component {
    state = {

    }
    componentWillMount() {

    }
    handleEdit = (e) => {
        this.props.editShopSupport();
    }


    render() {
        let supportInfo = this.props.supportInfo
        return (
            <div className="relative">
                <Layout>
                    <Content className='' style={{ padding: '25px 0', marginTop: '25px', backgroundColor: "#ECECEC" }}>

                        <Form layout="horizontal" >
                            <Row type="flex" style={{ padding: '1rem 0' }}>
                                <Col span={20} >
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>配套设施</span>
                                </Col>
                                <Col span={4}>
                                    {
                                        [1, 8].includes(this.props.shopInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                    }
                                </Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={24} >
                                    {supportInfo.upperWater ? <Tag color={TAG_COLOR}>上水</Tag> : null}
                                    {supportInfo.downWater ? <Tag color={TAG_COLOR}>下水</Tag> : null}
                                    {supportInfo.gas ? <Tag color={TAG_COLOR}>天然气</Tag> : null}
                                    {supportInfo.chimney ? <Tag color={TAG_COLOR}>烟管道 </Tag> : null}
                                    {supportInfo.blowoff ? <Tag color={TAG_COLOR}>排污管道 </Tag> : null}
                                    {supportInfo.split ? <Tag color={TAG_COLOR}>可分割 </Tag> : null}
                                    {supportInfo.elevator ? <Tag color={TAG_COLOR}>电梯 </Tag> : null}
                                    {supportInfo.staircase ? <Tag color={TAG_COLOR}>扶梯 </Tag> : null}
                                    {supportInfo.outside ? <Tag color={TAG_COLOR}>外摆区 </Tag> : null}
                                    {supportInfo.openFloor ? <Tag color={TAG_COLOR}>架空层 </Tag> : null}
                                    {supportInfo.parkingSpace ? <Tag color={TAG_COLOR}>停车位 </Tag> : null}
                                </Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={24} >
                                    {supportInfo.voltage ? <Tag color={TAG_COLOR}>{supportInfo.voltage}V</Tag> : null}
                                    {supportInfo.capacitance ? <Tag color={TAG_COLOR}>{supportInfo.capacitance}F</Tag> : null}
                                </Col>
                            </Row>
                        </Form>
                    </Content>
                </Layout>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        shopInfo: state.shop.shopsInfo,
        supportInfo: state.shop.shopsInfo.supportInfo,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        editShopSupport: () => dispatch(editShopSupport())
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SupportInfo);