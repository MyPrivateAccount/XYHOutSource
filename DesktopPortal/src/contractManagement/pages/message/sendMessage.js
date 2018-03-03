import { connect } from 'react-redux';
import { sendBuildingMsg, setMsgLoading,getChangeBuildingList } from '../../actions/actionCreator';
import React, { Component } from 'react';
import { Button, Row, Col, Input, Form, Select, notification, Modal, Spin } from 'antd';
import { NewGuid } from '../../../utils/appUtils';

const FormItem = Form.Item;
const { TextArea } = Input;
const Option = Select.Option;

class SendMessage extends Component {
    state = {
        showPreview: false
    }
    componentWillMount() {
        this.props.dispatch(getChangeBuildingList({city: this.props.user.City}))
    }
    componentWillReceiveProps(newProps) {
    }
    //消息预览
    handlePreview = (e) => {
        e.preventDefault();
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                // console.log("values", values);
                this.showPreviewDialog(values);
            }
        });
    }
    //消息发送
    handleSend = (e) => {
        let myBuildingList = this.props.changeList || [];
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = NewGuid();
                values.ext1 = myBuildingList.find(building => building.id === values.buildingId).basicInfo.name;
                // console.log("消息内容:", values);
                this.props.dispatch(setMsgLoading());
                this.props.dispatch(sendBuildingMsg(values));
            }
        })
    }
    //显示预览对话框
    showPreviewDialog(msg) {
        Modal.info({
            title: msg.title,
            content: (
                <div>
                    <p>{msg.content}</p>
                </div>
            ),
            onOk() { },
        });
    }
    handleReset = (e) => {
        this.props.form.resetFields();
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 4 },
            wrapperCol: { span: 20 },
        };
        let myBuildingList = this.props.changeList || [];
        let id;
        console.log(myBuildingList, this.props.buildingInfo,'/??')
        if (myBuildingList.length !== 0) {
            id = (this.props.buildingInfo || {}).id
        }
        return (
            <div style={{ padding: '25px 5px' }}>
                <Spin spinning={this.props.showLoading}>
                    <Row>
                        <Col span={12}>
                            <FormItem  {...formItemLayout} label="标题">
                                {getFieldDecorator('title', {
                                    rules: [{ required: true, message: '请输入标题!' }, { min: 3, message: "标题不少于3个字符!" }, { max: 20, message: "最多只能输入20个字符!" }],
                                })(
                                    <Input style={{ width: '300px' }} placeholder="请输入标题" />
                                    )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            <FormItem  {...formItemLayout} label="相关楼盘">
                                {getFieldDecorator('buildingId', {
                                    initialValue: id,
                                    rules: [{ required: true, message: '请选择相关楼盘!' }],
                                })(
                                    <Select style={{ width: '300px' }} >
                                        {
                                            myBuildingList.map(build => <Option key={build.id} value={build.id}>{build.basicInfo.name}</Option>)
                                        }
                                    </Select>
                                    )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            <FormItem  {...formItemLayout} label="内容描述">
                                {getFieldDecorator('content', {
                                    rules: [{ required: true, message: '请输入驳回原因!' }, { max: 2000, message: "最多只能输入20个字符!" }],
                                })(
                                    <TextArea rows={6} style={{ width: '450px' }} onChange={this.hanldeRejectReson} placeholder={'请输入消息内容!'} />
                                    )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ paddingLeft: '8%' }}>
                        <Col span={12}>
                            <Button onClick={this.handleReset} type='primary' style={{ margin: '5px' }}>重置</Button>
                            <Button onClick={this.handlePreview} type='primary' style={{ margin: '5px' }}>预览</Button>
                            <Button onClick={this.handleSend} type='primary' style={{ margin: '5px' }}>发布</Button>
                        </Col>
                    </Row>
                </Spin>
            </div>
        )
    }
}


function apptableMapStateToProps(state) {
    // console.log("nowInfo:", state.index.nowInfo);
    // console.log("searchList:", state.shop.changeList);
    return {
        showLoading: state.msg.showLoading,
        changeList: state.shop.changeList,
        // nowInfo: state.index.nowInfo,
        buildingInfo: state.index.buildingInfo,
        user: (state.oidc.user || {}).profile || {},
    }
}

function apptableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedApp = Form.create()(SendMessage);
export default connect(apptableMapStateToProps, apptableMapDispatchToProps)(WrappedApp);