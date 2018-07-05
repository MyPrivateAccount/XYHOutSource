//客户信息组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Layout, Row, Col, Input, Spin, Select, InputNumber } from 'antd'
import { getDicPars } from '../../../../utils/utils'
import { dicKeys } from '../../../constants/const'

const FormItem = Form.Item;
const Option = Select.Option;

let cjzqList = [];
for(let i = 1;i<=11;i++){
  cjzqList.push({key: `${i-1}~${i}周`,value: `${i-1}~${1}`})
}


class TradeCustomer extends Component {
  state = {
    loading: false,
    rpData: {}
  }

  componentDidMount = () => {
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


  render() {
    const { getFieldDecorator } = this.props.form;
    const {showBbSelector}  = this.props;
    const formItemLayout = {
      labelCol: { span: 6 },
      wrapperCol: { span: 14 },
    };

    const formItemLayout3 = {
      labelCol: { span: 7 },
      wrapperCol: { span: 14 },
    };
    //  let yzChtscTypes = this.props.basicData.yzChtscTypes
    let khKhxzTypes = getDicPars(dicKeys.khxz, this.props.dic);
    let khlyList = getDicPars(dicKeys.khly, this.props.dic);
    return (
      <Layout>
        <Spin spinning={this.state.loading} tip={this.state.tip}>
          <Row className="form-row">
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>名称</span>)}>
                {
                  getFieldDecorator('khMc', {
                    rules: [{ required: true, message: '请填写客户名称' }]
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
                    rules: [{ required: true, message: '请填写客户身份证' }]
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
          </Row>
          <Row className="form-row">
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>地址</span>)}>
                {
                  getFieldDecorator('khDz', {
                    rules: [{ required: false, message: '请填写地址!' }]
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
                    rules: [{ required: true, message: '请填写手机号码!' }]
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
                    rules: [{ required: true, message: '请选择客户来源!' }]
                  })(
                    <Select disabled={showBbSelector} style={{ width: 80 }}>
                      {
                        khlyList.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                      }
                    </Select>
                  )
                }
              </FormItem>
            </Col> 
          </Row>
          <div className="divider"></div>
          <Row className="form-row">
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>客户性质</span>)}>
                {
                  getFieldDecorator('khKhxz', {
                    rules: [{ required: false, message: '请选择客户性质!' }],
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
                    rules: [{ required: false, message: '请填写客源号!' }],
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
                    rules: [{ required: false, message: '请填写置业次数' }]
                  })(
                    <InputNumber style={{ width: 200 }}></InputNumber>
                  )
                }
              </FormItem>
            </Col>
          </Row>
          <Row className="form-row">
            <Col span={24}>
              <FormItem {...formItemLayout} label={(<span>Email</span>)}>
                {
                  getFieldDecorator('khEmail', {
                    rules: [{ required: false }]
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
          </Row>
          <Row className="form-row">
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>代理人</span>)}>
                {
                  getFieldDecorator('khDlr', {
                    rules: [{ required: false, message: '请填写代理人!' }]
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
                    rules: [{ required: false, message: '请填写代理人联系方式!' }]
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
                    rules: [{ required: false, message: '请填写代理人证件号码!' }]
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
          </Row>
           <Row className="form-row">
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
              <FormItem className="auto-width" label={(<span>从登记至签合同时长</span>)}>
                {
                  getFieldDecorator('khCdjzqhtsc', {
                    rules: [{ required: false, message: '请填写成交人!' }],
                    initialValue: this.state.rpData.khCdjzqhtsc,
                  })(
                    <Select style={{ width: 80 }}>
                      {
                        cjzqList.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                      }
                    </Select>
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
        </Spin>
      </Layout>
    )
  }
}
function MapStateToProps(state) {

  return {
    // basicData: state.base,
    // operInfo: state.rp.operInfo,
    // ext: state.rp.ext,
    // syncKhOp: state.rp.syncKhOp,
    // syncKhData: state.rp.syncKhData
  }
}

function MapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
const WrappedRegistrationForm = Form.create()(TradeCustomer);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
