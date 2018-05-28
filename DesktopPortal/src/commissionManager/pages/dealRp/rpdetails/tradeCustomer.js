//客户信息组件
import { connect } from 'react-redux';
import { getDicParList, dealKhSave } from '../../../actions/actionCreator'
import React, { Component } from 'react';
import { notification, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;

class TradeCustomer extends Component {
  state = {
    isDataLoading: false,
    rpData: {}
  }
  componentWillMount = () => {
    this.setState({ isDataLoading: true, tip: '信息初始化中...' })
    this.props.dispatch(getDicParList(['COMMISSION_KH_KHXZ', 'COMMISSION_YZ_QHTSC']));
  }
  componentWillReceiveProps(newProps) {
    this.setState({ isDataLoading: false });
    if (newProps.operInfo.operType === 'KHSAVE_UPDATE') {
      notification.success({
        message: '提示',
        description: '保存成交报告客户信息成功!',
        duration: 3
      });
      newProps.operInfo.operType = ''
    }
    else if (newProps.operInfo.operType === 'KHGET_UPDATE') {//信息获取成功
      this.setState({ rpData: newProps.ext});
      newProps.operInfo.operType = ''
    }
  }
  handleSave = (e) => {
    e.preventDefault();
    this.props.form.validateFields((err, values) => {
      if (!err) {
        values.id = this.props.rpId;
        console.log(values);
        this.setState({ isDataLoading: true, tip: '保存信息中...' })
        this.props.dispatch(dealKhSave(values))
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
      labelCol: { span: 14 },
      wrapperCol: { span: 6 },
    };
    const formItemLayout3 = {
      labelCol: { span: 7 },
      wrapperCol: { span: 14 },
    };
    let yzChtscTypes = this.props.basicData.yzChtscTypes
    let khKhxzTypes = this.props.basicData.khKhxzTypes
    return (
      <Layout>
        <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
          <Row>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>名称</span>)}>
                {
                  getFieldDecorator('khMc', {
                    rules: [{ required: false }],
                    initialValue: this.state.rpData.khMc,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>身份证</span>)}>
                {
                  getFieldDecorator('khZjhm', {
                    rules: [{ required: false }],
                    initialValue: this.state.rpData.khZjhm,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>地址</span>)}>
                {
                  getFieldDecorator('khDz', {
                    rules: [{ required: false, message: '请填写分行名称!' }],
                    initialValue: this.state.rpData.khDz,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>手机</span>)}>
                {
                  getFieldDecorator('khSj', {
                    rules: [{ required: false, message: '请填写成交人!' }],
                    initialValue: this.state.rpData.khSj,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>客户来源</span>)}>
                {
                  getFieldDecorator('khKhly', {
                    rules: [{ required: false, message: '请选择成交日期!' }],
                    initialValue: this.state.rpData.khKhly,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>客户性质</span>)}>
                {
                  getFieldDecorator('khKhxz', {
                    rules: [{ required: false, message: '请填写分行名称!' }],
                    initialValue: this.state.rpData.khKhxz,
                  })(
                    <Select style={{ width: 80 }}>
                      {
                        khKhxzTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                      }
                    </Select>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>客源号</span>)}>
                {
                  getFieldDecorator('khKyh', {
                    rules: [{ required: false, message: '请填写成交人!' }],
                    initialValue: this.state.rpData.khKyh,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>置业次数</span>)}>
                {
                  getFieldDecorator('khZycs', {
                    rules: [{ required: false, message: '请选择成交日期!' }],
                    initialValue: this.state.rpData.khZycs,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span={24} pull={4}>
              <FormItem {...formItemLayout} label={(<span>Email</span>)}>
                {
                  getFieldDecorator('khEmail', {
                    rules: [{ required: false }],
                    initialValue: this.state.rpData.khEmail,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>代理人</span>)}>
                {
                  getFieldDecorator('khDlr', {
                    rules: [{ required: false, message: '请填写分行名称!' }],
                    initialValue: this.state.rpData.khDlr,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout3} label={(<span>代理人联系方式</span>)}>
                {
                  getFieldDecorator('khDlrlxfs', {
                    rules: [{ required: false, message: '请填写成交人!' }],
                    initialValue: this.state.rpData.khDlrlxfs,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout3} label={(<span>代理人证件号码</span>)}>
                {
                  getFieldDecorator('khDlrzjhm', {
                    rules: [{ required: false, message: '请选择成交日期!' }],
                    initialValue: this.state.rpData.khDlrzjhm,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>成交原因</span>)}>
                {
                  getFieldDecorator('khCjyy', {
                    rules: [{ required: false, message: '请填写分行名称!' }],
                    initialValue: this.state.rpData.khCjyy,
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout2} label={(<span>从登记至签合同时长</span>)}>
                {
                  getFieldDecorator('khCdjzqhtsc', {
                    rules: [{ required: false, message: '请填写成交人!' }],
                    initialValue: this.state.rpData.khCdjzqhtsc,
                  })(
                    <Select style={{ width: 80 }}>
                      {
                        yzChtscTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                      }
                    </Select>
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
      </Layout>
    )
  }
}
function MapStateToProps(state) {

  return {
    basicData: state.base,
    operInfo: state.rp.operInfo,
    ext:state.rp.ext
  }
}

function MapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
const WrappedRegistrationForm = Form.create()(TradeCustomer);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
