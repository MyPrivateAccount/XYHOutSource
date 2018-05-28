//成交物业组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { getDicParList ,dealWySave} from '../../../actions/actionCreator'
import { notification,Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;

class TradeEstate extends Component {
    state = {
        isDataLoading:false
    }
    componentWillMount = () => {

        this.setState({isDataLoading:true,tip:'信息初始化中...'})
        //获取字典项
        this.props.dispatch(getDicParList(['COMMISSION_WY_CQ', 'COMMISSION_WY_PQ', 'COMMISSION_WY_WYLX', 'COMMISSION_WY_KJLX', 'COMMISSION_WY_ZXZK', 'COMMISSION_WY_ZXND', 'COMMISSION_WY_ZXJJ', 'COMMISSION_WY_WYCX','COMMISSION_PAY_TYPE']));
    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false });
        if(newProps.operInfo.operType === 'WYSAVE_UPDATE'){
            notification.success({
                message: '提示',
                description: '保存成交报告物业信息成功!',
                duration: 3
            });
            newProps.operInfo.operType = ''
        }
    }
    handleSave = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = this.props.rpId;
                console.log(values);
                this.setState({isDataLoading:true,tip:'保存信息中...'})
                this.props.dispatch(dealWySave(values));
            }
        });
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const formItemLayout2 = {
            labelCol: { span: 10 },
            wrapperCol: { span: 10 }
        }
        let wyCqTypes = this.props.basicData.wyCqTypes;
        let wyPqTypes = this.props.basicData.wyPqTypes;
        let wyWylxTypes = this.props.basicData.wyWylxTypes;
        let wyKjlxTypes = this.props.basicData.wyKjlxTypes;
        let wyZxTypes = this.props.basicData.wyZxTypes;
        let wyZxndTypes = this.props.basicData.wyZxndTypes;
        let wyJjTypes = this.props.basicData.wyJjTypes;
        let wyCxTypes = this.props.basicData.wyCxTypes;
        let payTypes = this.props.basicData.payTypes;
        return (
            <Layout>
                <div>
                <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                    <Row style={{ textAlign: 'center', marginLeft: 30 }}>
                        <Col span={3}><span>城区</span></Col>
                        <Col span={3}><span>片区</span></Col>
                        <Col span={3}><span>物业名称</span></Col>
                        <Col span={4}><span>位置/栋/座/单元</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>楼层</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>房号</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>总楼层</span></Col>
                    </Row>
                    <Row>
                        <Col span={4}>
                            <FormItem {...formItemLayout2} label={(<span>物业地址</span>)}>
                                {
                                    getFieldDecorator('wyCq', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyCqTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wqPq', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyPqTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wqMc', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyWz', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyLc', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyFh', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyZlc', {
                                        rules: [{ required: false }],
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>产证物业地址</span>)}>
                                {
                                    getFieldDecorator('wyCzwydz', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 300 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={2} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>房</span>)}>
                                {
                                    getFieldDecorator('wyF', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={2} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>厅</span>)}>
                                {
                                    getFieldDecorator('wyT', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={2} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>卫</span>)}>
                                {
                                    getFieldDecorator('wyW', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>阳台</span>)}>
                                {
                                    getFieldDecorator('wyYt', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>露台</span>)}>
                                {
                                    getFieldDecorator('wyLt', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>观景台</span>)}>
                                {
                                    getFieldDecorator('wyJgf', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>物业类型</span>)}>
                                {
                                    getFieldDecorator('wyWylx', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyWylxTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>空间类型</span>)}>
                                {
                                    getFieldDecorator('wyKjlx', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyKjlxTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>建筑面积</span>)}>
                                {
                                    getFieldDecorator('wyJzmj', {
                                        rules: [{ required: false }],
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>均价</span>)}>
                                {
                                    getFieldDecorator('wyJj', {
                                        rules: [{ required: false }],
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>电梯数</span>)}>
                                {
                                    getFieldDecorator('wyDts', {
                                        rules: [{ required: false }],
                                        initialValue: 1,
                                    })(
                                        <Input style={{ width: 120 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>每层户数</span>)}>
                                {
                                    getFieldDecorator('wyMchs', {
                                        rules: [{ required: false }],
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 120 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>装修年代</span>)}>
                                {
                                    getFieldDecorator('wyZxnd', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyZxndTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>家具</span>)}>
                                {
                                    getFieldDecorator('wyJj', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyJjTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>产权取得时间</span>)}>
                                {
                                    getFieldDecorator('wyCqzqdsj', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 180 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房产按揭号码</span>)}>
                                {
                                    getFieldDecorator('wyFcajhm', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 180 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>是否合租</span>)}>
                                {
                                    getFieldDecorator('wySfhz', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: '',
                                    })(
                                        <Select style={{ width: 180 }}>
                                            <Option key='1' value='1'>是</Option>
                                            <Option key='2' value='2'>否</Option>
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源付款方式</span>)}>
                                {
                                    getFieldDecorator('wyFyfkfs', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: '',
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                payTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源贷款年限</span>)}>
                                {
                                    getFieldDecorator('wyFydknx', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 180 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout2} label={(<span>房源贷款剩余年限</span>)}>
                                {
                                    getFieldDecorator('wyFydksynx', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源贷款金额</span>)}>
                                {
                                    getFieldDecorator('wyFydkje', {
                                        rules: [{ required: false }],
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 120 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源已还金额</span>)}>
                                {
                                    getFieldDecorator('wyFyyhje', {
                                        rules: [{ required: false }],
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 120 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <Button type='primary' onClick={this.handleSave}>保存</Button>
                        </Col>
                    </Row>
                    </Spin>
                </div>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        operInfo:state.rp.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradeEstate);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
