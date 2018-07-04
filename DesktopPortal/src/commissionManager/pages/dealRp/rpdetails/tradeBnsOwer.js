//业主信息组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Layout, Row, Col, Input,Spin,Select } from 'antd'
import { getDicPars } from '../../../../utils/utils'
import { dicKeys, permission } from '../../../constants/const'

const FormItem = Form.Item;

let cjzqList = [];
for(let i = 1;i<=11;i++){
  cjzqList.push({key: `${i-1}~${i}周`,value: `${i-1}~${1}`})
}

class TradeBnsOwner extends Component {
  state = {
    loading: false,
    rpData: []
  }
  componentWillMount = () => {

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
    const { showBbSelector } = this.props;
    const formItemLayout = {
      labelCol: { span: 6 },
      wrapperCol: { span: 14 },
    };
    
    const formItemLayout3 = {
      labelCol: { span: 7 },
      wrapperCol: { span: 14 },
    };
    //  let yzChtscTypes = this.props.basicData.yzChtscTypes
    let khlyList = getDicPars(dicKeys.khly, this.props.dic);
    return (
      <Layout>
        <Spin spinning={this.state.loading} tip={this.state.tip}>
          <Row className="form-row">
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>名称</span>)}>
                {
                  getFieldDecorator('yzMc', {
                    rules: [{ required: true, message: '请填写业主名称' }]
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>身份证</span>)}>
                {
                  getFieldDecorator('yzZjhm', {
                    rules: [{ required: false }],
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
                  getFieldDecorator('yzDz', {
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
                  getFieldDecorator('yzSj', {
                    rules: [{ required: false, message: '请填写客户手机!' }]
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout} label={(<span>客户来源</span>)}>
                {
                  getFieldDecorator('yzYzly', {
                    rules: [{ required: false, message: '请填写客户来源!' }]
                  })(
                    <Select disabled={showBbSelector} >
                        {
                            khlyList.map(u => (<Select.Option key={u.key} value={u.key}>{u.key}</Select.Option>))
                        }
                    </Select>
                  )
                }
              </FormItem>
            </Col> 
          </Row>
          <div className="divider"></div>
          <Row className="form-row">
            <Col span={24}>
              <FormItem {...formItemLayout} label={(<span>Email</span>)}>
                {
                  getFieldDecorator('yzEmail', {
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
                  getFieldDecorator('yzDlr', {
                    rules: [{ required: false, message: '请填写代理人' }]
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem {...formItemLayout3} label={(<span>代理人联系方式</span>)}>
                {
                  getFieldDecorator('yzDlrlxfs', {
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
                  getFieldDecorator('yzDlrzjhm', {
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
                  getFieldDecorator('yzCjyy', {
                    rules: [{ required: false, message: '请填写成交原因!' }]
                  })(
                    <Input style={{ width: 200 }}></Input>
                  )
                }
              </FormItem>
            </Col>
            <Col span={8}>
              <FormItem className="auto-width" label={(<span>从登记至签合同时长</span>)}>
                {
                  getFieldDecorator('yzCdjzqhtsc', {
                    rules: [{ required: false, message: '请填写从登记至签合同时长!' }]
                  })(
                    <Select style={{ width: 80 }}>
                      {
                        cjzqList.map(tp => <Select.Option key={tp.key} value={tp.value}>{tp.key}</Select.Option>)
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
    // syncYzOp:state.rp.syncYzOp,
    // syncYzData:state.rp.syncYzData
  }
}

function MapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
const WrappedRegistrationForm = Form.create()(TradeBnsOwner);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
