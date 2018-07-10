//按揭过户组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import { getDicPars } from '../../../../utils/utils'
import { dicKeys } from '../../../constants/const'
import { Checkbox, DatePicker, Form, Layout, Radio, Row, Col, Input, Select, InputNumber } from 'antd'
import RadioGroup from 'antd/lib/radio/group';

const FormItem = Form.Item;
const Option = Select.Option;

let dknxList = [];
for (let i = 1; i <= 50; i++) {
    dknxList.push({ key: i + '年', value: i })
}

class TradeTransfer extends Component {

    state = {
        districtList: []
    }
    componentWillMount = () => {

    }
    componentDidMount = () => {
        this.getDistrictList();
        this.initEntity(this.props);
    }
    componentWillReceiveProps = (nextProps) => {
        if (this.props.entity !== nextProps.entity && nextProps.entity) {

            this.initEntity(nextProps)
        }
    }

    initEntity = (nextProps) => {
        var entity = nextProps.entity;
        if (!entity) {
            return;
        }

        let mv = {};
        Object.keys(entity).map(key => {
            mv[key] = entity[key];
        })
        this.props.form.setFieldsValue(mv);
    }

    getDistrictList = async () => {
        let city = this.props.user.City;
        if (!city) {
            return;
        }

        let url = `${WebApiConfig.area.get}${city}`
        let r = await ApiClient.get(url);
        r = (r || {}).data || {};
        if (r.code === '0' && r.extension) {
            this.setState({ districtList: r.extension })

        }
    }

    render() {
        const { districtList } = this.state;


        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const { getFieldDecorator } = this.props.form;

        let fkfsList = getDicPars(dicKeys.fkfs, this.props.dic);
        const canEdit = this.props.canEdit;

        return (
            <Layout>
                <div>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>所属区域</span>)}>
                                {
                                    getFieldDecorator('ghSsqy', {
                                        rules: [{ required: false }]
                                    })(
                                        <Select disabled={!canEdit} style={{ width: 80 }}>
                                            {
                                                districtList.map(tp => <Option key={tp.code} value={tp.code}>{tp.name}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>付款方式</span>)}>
                                {
                                    getFieldDecorator('ghFkfs', {
                                        rules: [{ required: false }]
                                    })(
                                        <Select disabled={!canEdit}  style={{ width: 200 }}>
                                            {
                                                fkfsList.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>签约担保公司</span>)}>
                                {
                                    getFieldDecorator('ghQydbgs')(
                                        <RadioGroup disabled={!canEdit}  style={{ width: 200 }}>
                                            <Radio key='1' value={1}>是</Radio>
                                            <Radio key='0' value={0}>否</Radio>
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>担保公司</span>)}>
                                {
                                    getFieldDecorator('ghDbgs')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款银行</span>)}>
                                {
                                    getFieldDecorator('ghDkyh')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>保费金额</span>)}>
                                {
                                    getFieldDecorator('ghDbfje')(
                                        <InputNumber disabled={!canEdit}  precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款支行</span>)}>
                                {
                                    getFieldDecorator('ghDkzh')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款成数</span>)}>
                                {
                                    getFieldDecorator('ghDkcs')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款金额</span>)}>
                                {
                                    getFieldDecorator('ghDkje')(
                                        <InputNumber disabled={!canEdit}  precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款年限</span>)}>
                                {
                                    getFieldDecorator('ghDknx')(
                                        <Select disabled={!canEdit}  style={{ width: 80 }}>
                                            {
                                                dknxList.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>评估公司</span>)}>
                                {
                                    getFieldDecorator('ghPggs')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>资金监管金额</span>)}>
                                {
                                    getFieldDecorator('ghZjjgje')(
                                        <InputNumber disabled={!canEdit}  precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>业主欠款</span>)}>
                                {
                                    getFieldDecorator('ghYzqk')(
                                        <InputNumber disabled={!canEdit}  precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>赎楼罚息</span>)}>
                                {
                                    getFieldDecorator('ghSlfx')(
                                        <InputNumber disabled={!canEdit}  precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>赎楼利息</span>)}>
                                {
                                    getFieldDecorator('ghSllx')(
                                        <InputNumber disabled={!canEdit}  precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>担保金额</span>)}>
                                {
                                    getFieldDecorator('ghDbje')(
                                        <InputNumber disabled={!canEdit}  precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>抵押回执号</span>)}>
                                {
                                    getFieldDecorator('ghDyhzh')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户回执号</span>)}>
                                {
                                    getFieldDecorator('ghGhhzh')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8} style={{display:'flex'}}>
                            <FormItem >
                                {
                                    getFieldDecorator('ghZzyh')(
                                        <Checkbox disabled={!canEdit}  >自找银行</Checkbox>
                                    )
                                }
                            </FormItem>
                            <FormItem>
                                {
                                    getFieldDecorator('ghZzdb')(
                                        <Checkbox  disabled={!canEdit} >自找担保</Checkbox>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>放款日期</span>)}>
                                {
                                    getFieldDecorator('ghFkrq')(
                                        <DatePicker disabled={!canEdit}  style={{ width: 200 }} onChange={this.ghFkrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>解保日期</span>)}>
                                {
                                    getFieldDecorator('ghJbrq')(
                                        <DatePicker disabled={!canEdit}  style={{ width: 200 }} onChange={this.ghJbrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>

                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户日期</span>)}>
                                {
                                    getFieldDecorator('ghGhrq')(
                                        <DatePicker disabled={!canEdit}  style={{ width: 200 }} onChange={this.ghGhrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户产证地址</span>)}>
                                {
                                    getFieldDecorator('ghGhczdz')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户价格</span>)}>
                                {
                                    getFieldDecorator('ghGhjg')(
                                        <InputNumber disabled={!canEdit}  precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户业主电话</span>)}>
                                {
                                    getFieldDecorator('ghGhyzdh')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户客户电话</span>)}>
                                {
                                    getFieldDecorator('ghGhkhdh')(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                                {
                                    getFieldDecorator('ghBz')(
                                        <Input.TextArea disabled={!canEdit}  rows={4} style={{ width: 510 }}></Input.TextArea>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    {/* <Row>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <Button type='primary' onClick={this.handleSave}>保存</Button>
                        </Col>
                    </Row> */}

                </div>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        user: state.oidc.user.profile || {}
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradeTransfer);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);