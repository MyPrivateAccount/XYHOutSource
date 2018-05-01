import React, {Component} from 'react'
import {Row, Col, Modal, Select, notification, Form, Input} from 'antd';
import {basicDataBaseApiUrl} from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'

const Option = Select.Option;
const FormItem = Form.Item;
const {TextArea} = Input;
class AddFollow extends Component {
    state = {
        customerInfo: {},
        customerLevels: [],
        requirementLevels: [],
        followUpTypes: [],
        curLevel: null
    }
    componentWillMount() {
        // let userInfo = this.props.oidc.user.profile;
    }

    componentWillReceiveProps(newProps) {
        const basicData = (newProps.dicInfo || {});
        const customerInfo = newProps.customerInfo || {};
        const customerLevels = basicData.customerLevels || [];
        const requirementLevels = basicData.requirementLevels || [];
        const followUpTypes = basicData.followUpTypes || [];
        this.setState({
            customerInfo: customerInfo,
            customerLevels: customerLevels,
            requirementLevels: requirementLevels,
            followUpTypes: followUpTypes
        });
    }
    handleLevelChange = (value) => {
        this.setState({curLevel: value});
    }

    handleOk = (e) => {
        e.preventDefault();
        let customerInfo = this.state.customerInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                let url = basicDataBaseApiUrl + 'customerfollowup/manager/addfollowup';
                let entity = {...values, customerId: customerInfo.id}
                entity.importance = entity.importance * 1;
                if (entity.demandLevel) {
                    entity.demandLevel = entity.demandLevel * 1;
                }
                ApiClient.post(url, entity).then(res => {
                    if (res.data.code === "0") {
                        notification.success({description: "保存成功."});
                        if (this.props.reloadDetail) {
                            this.props.reloadDetail(customerInfo);
                        }
                        this.handleCancel();
                    } else {
                        notification.error({description: "写跟进失败!"})
                    }
                }).catch((e) => {
                    notification.error({description: "写跟进失败!"})
                });
            }
        });
    }
    handleCancel = () => {
        if (this.props.closeDialog) {
            this.props.closeDialog();
        }
        this.props.form.resetFields();
    }

    render() {
        const visible = this.props.visible;
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 4},
            wrapperCol: {span: 20},
        };
        const {followUpTypes, customerLevels, requirementLevels} = this.state;
        return (
            <Modal title="写跟进" maskClosable={false} visible={visible} onOk={this.handleOk} onCancel={this.handleCancel}>
                <Row>
                    <Col span={24} pull={1}>
                        <FormItem {...formItemLayout} label={(<span>跟进方式</span>)}>
                            {getFieldDecorator('followMode', {
                                rules: [{required: true, message: '请选择跟进方式!'}],
                            })(
                                <Select style={{width: 120}} placeholder="请选择客户等级">
                                    {
                                        followUpTypes.map(ftype => <Option key={ftype.value} value={ftype.value}>{ftype.key}</Option>)
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={24} pull={1}>
                        <FormItem {...formItemLayout} label={(<span>客户等级</span>)}>
                            {getFieldDecorator('importance', {
                                rules: [{required: true, message: '请选择客户等级!'}],
                            })(
                                <Select style={{width: 120}} placeholder="请选择客户等级" onChange={this.handleLevelChange}>
                                    {
                                        customerLevels.map(cLevel => <Option key={cLevel.value} value={cLevel.value}>{cLevel.key}</Option>)
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                {
                    this.state.curLevel === '1' ? <Row>
                        <Col span={24} pull={1}>
                            <FormItem {...formItemLayout} label={(<span>需求等级</span>)}>
                                {getFieldDecorator('demandLevel', {
                                    rules: [{required: true, message: '请选择需求等级!'}],
                                })(
                                    <Select style={{width: 120}} placeholder="请选择需求等级">
                                        {
                                            requirementLevels.map(rLevel => <Option key={rLevel.key} value={rLevel.value}>{rLevel.key}</Option>)
                                        }
                                    </Select>
                                    )}
                            </FormItem>
                        </Col>
                    </Row> : null
                }
                <Row>
                    <Col span={24} pull={1}>
                        <FormItem {...formItemLayout} label={(<span>跟进描述</span>)}>
                            {getFieldDecorator('followUpContents', {
                                rules: [{required: true, message: '请填写跟进描述!'}],
                            })(
                                <TextArea placeholder='写跟进描述' />
                                )}
                        </FormItem>
                    </Col>
                </Row>
            </Modal>
        )
    }
}

const WrappedRegistrationForm = Form.create()(AddFollow);
export default WrappedRegistrationForm;