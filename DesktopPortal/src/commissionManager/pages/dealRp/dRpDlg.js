//弹出的收付款框模版
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Row, Col, Form, Input, Tooltip, Button, Modal, Layout, Tabs, Spin } from 'antd'
import FKCp from './fKComponet'
import SKCp from './sKComponet'
import SJCp from './sJComponet'
import Avatar from './rpdetails/tradeUpload'
import { factGet, factGetGet, factGetPay } from '../../actions/actionCreator'

const TabPane = Tabs.TabPane;
const FormItem = Form.Item;
class DRpDlg extends Component {
    state = {
        vs: false,
        isDataLoading: false,
        type: 'sk',
        sfkinfo: {},//收付款信息
        isSjcpInit: false
    }
    handleOk = (e) => {
        let data = {}
        let sjdata = {}
        data = this.skcp.getData()

        console.log("skcp data :" + data)
        if (data === null) {
            return
        }
        if (this.state.isSjcpInit) {
            sjdata = this.sjcp.getData()
            if (sjdata !== null) {
                data.sjhm = sjdata.sjhm
                data.qtsj = sjdata.qtsj
                data.sjbz = sjdata.sjbz
            }
        }
        else {
            data.sjhm = this.state.sfkinfo.sjhm
            data.qtsj = this.state.sfkinfo.qtsj
            data.sjbz = this.state.sfkinfo.sjbz
        }
        this.setState({ isDataLoading: true })
        this.props.dispatch(factGetGet(data))
    };
    handleCancel = (e) => {
        this.setState({ vs: false })
    };
    componentDidMount() {
        this.props.onDlgSelf(this)
    }
    show = (e) => {
        console.log(e)
        this.props.form.setFieldsValue({ 'zyj': e.zyj })
        this.props.form.setFieldsValue({ 'cjbgbh': e.cjbgbh })
        this.props.form.setFieldsValue({ 'wymc': e.wyMc })
        this.setState({ vs: true })
        this.setState({ type: e.type })
        this.props.dispatch(factGet('1'))
    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false })
        if (newProps.operInfo.operType === 'DEALRP_FACTGET_SUCCESS') {
            this.setState({ sfkinfo: newProps.ext })
        }
        if (newProps.operInfo.operType === 'DEALRP_FACTGET_GET_SAVE_SUCCESS' ||
            newProps.operInfo.operType === 'DEALRP_FACTGET_PAY_SAVE_SUCCESS') {
            this.handleCancel()
        }
    }
    onSKCp = (e) => {
        this.skcp = e
    }
    onSJCp = (e) => {
        this.sjcp = e
        this.setState({ isSjcpInit: true })
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        return (
            <Modal title={this.state.type === 'sk' ? '收款' : '付款'} width={800} maskClosable={false} visible={this.state.vs}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Spin spinning={this.state.isDataLoading}>
                    <Layout>
                        <Row style={{ margin: 5 }}>
                            <Col span={12} style={{ marginLeft: 10 }}>
                                <FormItem {...formItemLayout} label={(<span>总佣金</span>)}>
                                    {
                                        getFieldDecorator('zyj', {
                                            rules: [{ required: false, message: '请填写公司名称!' }],
                                        })(
                                            <Input style={{ width: 200 }}></Input>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <Row style={{ margin: 5 }}>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>成交报告编号</span>)}>
                                    {
                                        getFieldDecorator('cjbgbh', {
                                            rules: [{ required: false, message: '请填写公司名称!' }],
                                        })(
                                            <Input style={{ width: 200 }} disabled={true}></Input>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>物业名称</span>)}>
                                    {
                                        getFieldDecorator('wymc', {
                                            rules: [{ required: false, message: '请填写公司名称!' }],
                                        })(
                                            <Input style={{ width: 200 }} disabled={true}></Input>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <Row style={{ margin: 5 }}>
                            <Col span={24}>
                                <Tabs defaultActiveKey="dsdfk">
                                    <TabPane tab="代收代付款" key="dsdfk">
                                        {
                                            this.state.type === 'sk' ? <SKCp onSKCp={this.onSKCp} sfkinfo={this.state.sfkinfo} /> : <FKCp onFKCp={this.onSKCp} sfkinfo={this.state.sfkinfo} />
                                        }
                                    </TabPane>
                                    <TabPane tab="收据" key="sj">
                                        <SJCp onSJCp={this.onSJCp} sfkinfo={this.state.sfkinfo} />
                                    </TabPane>
                                    <TabPane tab="附件" key="fj">

                                    </TabPane>
                                </Tabs>
                            </Col>
                        </Row>
                    </Layout>
                </Spin>
            </Modal>
        )
    }
}
function MapStateToProps(state) {

    return {
        ext: state.rp.ext,
        operInfo: state.rp.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(DRpDlg);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);