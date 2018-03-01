import { connect } from 'react-redux';
import { saveRelShops, viewRelshopInfo,relShopLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Select, Form, Button, Checkbox, Icon, Row, Col, Input, InputNumber } from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;
const CheckboxGroup = Checkbox.Group;

class RelShopsEdit extends Component {
    state = {
        loadingState: false
    }
    componentWillMount() {

    }
    componentWillReceiveProps(newProps) {
        this.setState({ loadingState: false });
    }
    handleSave = (e) => {
        let { relShopOperType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                // this.setState({ loadingState: true });
                this.props.dispatch(relShopLoadingStart())
                let newRelshopsInfo = values;
                newRelshopsInfo.id = this.props.buildInfo.id;
                if (relShopOperType != 'add') {

                }
                let method = (relShopOperType == 'add' ? 'POST' : "PUT");
                console.log(`method:${method},empInfo:${JSON.stringify(newRelshopsInfo)}`);
                this.props.dispatch(saveRelShops({ 
                    method: method, 
                    entity: newRelshopsInfo,
                    ownCity: this.props.user.City  
                }));
            }
        });

    }
    handleCancel = (e) => {
        this.props.dispatch(viewRelshopInfo());
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const tradePlanningOptions = [];
        this.props.basicData.tradePlannings.map((item) => {
            tradePlanningOptions.push({ label: item.key, value: item.value });
        });
        let relShopInfo = this.props.buildInfo.relShopInfo;
        let { relShopOperType } = this.props.operInfo;
        return (
            <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px"}}>
                <Icon type="tags-o" className='content-icon'/> <span className='content-title'>商铺整体情况</span>
                <Row type="flex" style={{marginTop: "25px"}}>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>销售状态</span>} >
                            {getFieldDecorator('saleStatus', {
                                initialValue: relShopInfo.saleStatus,
                                rules: [{ required: true, message: '请选择销售状态!' }],
                            })(
                                <Select>
                                    {
                                        this.props.basicData.shopSaleStatus.map((sItem) =>
                                            <Option key={sItem.value}>{sItem.key}</Option>
                                        )
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>商铺类别</span>} >
                            {getFieldDecorator('shopCategory', {
                                initialValue: relShopInfo.shopCategory,
                                rules: [{ required: true, message: '请选择商铺类别!' }],
                            })(
                                <Select>
                                    {
                                        this.props.basicData.shopsTypes.map((item) =>
                                            <Option key={item.value}>{item.key}</Option>
                                        )
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>销售模式</span>} >
                            {getFieldDecorator('saleMode', {
                                initialValue: relShopInfo.saleMode,
                                rules: [{ required: true, message: '请选择销售模式!' }],
                            })(
                                <Select>
                                    {
                                        this.props.basicData.saleModel.map((model) =>
                                            <Option key={model.value}>{model.key}</Option>
                                        )
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>周边三公里居住人数</span>} >
                            {getFieldDecorator('populations', {
                                initialValue: relShopInfo.populations
                                //rules: [{ required: true, message: '请输入楼盘名称!' }],
                            })(
                                <InputNumber />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={24} pull={3}>
                        <FormItem {...formItemLayout} label={<span>业态规划</span>} >
                            {getFieldDecorator('tradeMixPlanning', {
                                initialValue: relShopInfo.tradeMixPlanning,
                                // rules: [{ required: true, message: '请选择业态规划!' }],
                            })(
                                <CheckboxGroup options={tradePlanningOptions} onChange={this.onPlanningChange} />
                                )}
                        </FormItem>
                    </Col>
                </Row>

                <Row>
                    <Col span={24} pull={3}>
                        <FormItem {...formItemLayout} label={<span>优惠政策</span>} >
                            {getFieldDecorator('preferentialPolicies', {
                                initialValue: relShopInfo.preferentialPolicies
                                //rules: [{ required: true, message: '请输入楼盘名称!' }],
                            })(
                                <Input type="textarea" placeholder="请输入优惠政策" />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button 
                        type="primary" 
                        htmlType="submit" 
                        disabled={this.props.buildInfo.isDisabled} 
                        loading={this.props.loadingState} 
                        style={{width: "8rem"}} 
                        onClick={this.handleSave}>保存</Button>
                        {relShopOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
                    </Col>
                </Row>
            </Form >
        )
    }
}

function mapStateToProps(state) {
    //console.log('relshops MapStateToProps:' + JSON.stringify(state));
    return {
        basicData: state.basicData,
        buildInfo: state.building.buildInfo,
        operInfo: state.building.operInfo,
        loadingState: state.building.relShoploading,
        user: (state.oidc.user || {}).profile || {},
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

const WrappedForm = Form.create()(RelShopsEdit);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);