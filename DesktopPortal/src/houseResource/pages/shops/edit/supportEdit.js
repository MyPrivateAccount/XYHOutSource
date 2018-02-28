// author (chensijing)
// 用于填写商铺的配套设施信息的表单


import { connect } from 'react-redux';
import { saveShopSupportAsync, viewShopSupport,shopSupportLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import './editCommon.less'
import {
  Layout, Button, Checkbox,
  Icon, Row, Col, Form,
  Radio, InputNumber
} from 'antd'

const { Content } = Layout;
const FormItem = Form.Item;
const RadioGroup = Radio.Group;
const CheckboxGroup = Checkbox.Group;

class SupportEdit extends Component {
  state = {
    options: [
      { label: '上水', value: 'upperWater' },
      { label: '下水', value: 'downWater' },
      { label: '天然气', value: 'gas' },
      { label: '烟管道', value: 'chimney' },
      { label: '排污管道', value: 'blowoff' },
      { label: '可分割', value: 'split' },
      { label: '电梯', value: 'elevator' },
      { label: '扶梯', value: 'staircase' },
      { label: '外摆区', value: 'outside' },
      { label: '架空层', value: 'openFloor' },
      { label: '停车位', value: 'parkingSpace' }
    ],
    selectArr: {}
  }

  componentWillMount() {

  }
  onChange = (checkedValues) => {
    console.log(checkedValues)
    let newVaules = {};
    this.state.options.map((v) => {
      if (checkedValues.indexOf(v.value) !== -1) {
        newVaules[v.value] = true;
      } else {
        newVaules[v.value] = false;
      }
    })
    this.setState({
      selectArr: newVaules
    })
  }
  handleCancel = (e) => {
    this.props.viewShopSupport();
  }
  handleSave = (e) => {
    e.preventDefault();
    let { supportOperType } = this.props.operInfo;
    let shopsInfo = this.props.shopsInfo;
    this.props.form.validateFields((err, values) => {
      if (!err) {
        // this.setState({ loading: true });
        this.props.dispatch(shopSupportLoadingStart())
        let info = Object.assign({}, values, { id: shopsInfo.id, ...this.state.selectArr })
        if (supportOperType !== 'add') {
          info.buildingId = this.props.shopsInfo.buildingId
        }
        this.props.save({ supportOperType: supportOperType, entity: info })
      }
    })
  }


  render() {
    const { getFieldDecorator } = this.props.form;
    const formItemLayout = {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 6 },
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 14 },
      },
    };
    const supportInfo = this.props.shopsInfo.supportInfo;
    let supportOperType = this.props.operInfo.supportOperType;
    supportInfo.basic = []
    Object.keys(supportInfo).forEach((v) => {
      if (supportInfo[v]) {
        supportInfo.basic.push(v)
      }
    })
    return (

      <div className="relative">
        <Layout>
          <Content className='supporEdit' style={{ padding: '25px 0', margin: '20px 0', backgroundColor: "#ECECEC" }}>
            <Icon type="tags-o" className='content-icon' /> <span className='content-title'>配套设施</span>
            <Row type="flex" justify="start" style={{ paddingTop: "25px" }}>
              <Col span={24}>
                <Form ref={(e) => this.form = e} onSubmit={this.handleSubmit}>
                  <FormItem    {...formItemLayout} label="基础设施" >
                    {getFieldDecorator('basic', {
                      initialValue: supportInfo.basic,
                      // rules: [{ required: true, message: '请选择基础设施' },],
                    })(
                      <CheckboxGroup className="divBorder" options={this.state.options} onChange={this.onChange} />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="电压" >
                    {getFieldDecorator('voltage', {
                      initialValue: supportInfo.voltage,
                      rules: [{ required: true, message: '请选择电压' },],
                    })(
                      <RadioGroup>
                        <Radio value={220}>220V</Radio>
                        <Radio value={380}>380V</Radio>
                      </RadioGroup>
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="电容量F(法拉)" >
                    {getFieldDecorator('capacitance', {
                      initialValue: supportInfo.capacitance,
                      // rules: [{ required: true, message: '请输入电容量' },],
                    })(
                      <InputNumber min={0} />
                      )}
                  </FormItem>
                </Form>
              </Col>
            </Row>
            <Row type="flex" justify="center" className='BtnTop'>
              <Button type="primary" size='default' style={{ width: "8rem" }}
                onClick={this.handleSave} 
                loading={this.props.loading} 
                disabled={this.props.shopsInfo.isDisabled}>保存</Button>
              {supportOperType !== 'add' ? <Button size='default' className='oprationBtn' onClick={this.handleCancel}>取消</Button> : null}
            </Row>
          </Content>
        </Layout>
      </div>
    )
  }
}

function mapStateToProps(state) {
  return {
    operInfo: state.shop.operInfo,
    shopsInfo: state.shop.shopsInfo,
    loading: state.shop.supportloading
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch,
    save: (...args) => dispatch(saveShopSupportAsync(...args)),
    viewShopSupport: () => dispatch(viewShopSupport())
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(SupportEdit));