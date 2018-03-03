import { connect } from 'react-redux';
import { saveShopLeaseAsync, viewShopLease,shopLeaseLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import './editCommon.less'
import moment from 'moment'
import { Layout, Table, Button, Checkbox, DatePicker,InputNumber,
         Popconfirm, Tooltip, Icon, Row, Col, Form, Input,
         Radio, Select, } from 'antd'

const { Header, Sider, Content } = Layout;
const FormItem = Form.Item;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const RadioButton = Radio.Button;
const {RangePicker} = DatePicker;

class LeaseEdit extends Component {
    state = {
      disabled: true,
      loading: false,
      isHas: false,
      depositType: '',
      hasLeaseback: false
    }
    componentWillMount() {
      const leaseInfo = this.props.leaseInfo;
      if (Object.keys(leaseInfo).length !== 0) {
        this.setState({
          isHas: true, 
          depositType: leaseInfo.depositType,
          hasLeaseback: leaseInfo.hasLeaseback,
        })
      } else {
        this.setState({isHas: false})
      }

    }
    changIshas = (e)  => {
      console.log(e.target.checked, '点击')
      this.setState({ isHas: e.target.checked })
    }
    changeDeposit = (v) => {
      console.log(v, '选择押金方式')
      this.setState({depositType: v})
      
    }
    onChange = (e) => {
      this.setState({hasLeaseback: e.target.value})
      if (!e.target.value) {
        this.setState({
          disabled: true
        });
      } else {
        this.setState({
          disabled: false
        });
      }
    }
    onChangeDate = (e) => {
      console.log(e, '时间选择')
    }
    handleCancel = (e) => {
      this.props.viewShopLease();
    }
    handleSave = (e) => {
      e.preventDefault();
      let { leaseOperType } = this.props.operInfo;
      let shopsInfo = this.props.shopsInfo;
      this.props.form.validateFields((err, values) => {
        if (!err) {
          // this.setState({ loading: true });
          this.props.dispatch(shopLeaseLoadingStart())
          let info = Object.assign({}, values, {id: shopsInfo.id})
          if (leaseOperType !== 'add') {
            info.buildingId = this.props.shopsInfo.buildingId
          }
          this.props.save({leaseOperType: leaseOperType, entity: info})
        }
      });
    }
    checkBackMonth = (rule, value, callback) => {
      const form = this.props.form;
      const has = form.getFieldValue('hasLeaseback');
      const name = form.getFieldValue('backMonth');
      if (has && !name) {
        callback('请输入返祖时间')
      } else {
        callback()
      }
    }
    checkBackRate = (rule, value, callback) => {
      const form = this.props.form;
      const has = form.getFieldValue('hasLeaseback');
      const rate = form.getFieldValue('backRate');
      if (has && !rate ){
        callback('请输入返祖比例')
      } else {
        callback()
      }
    }

    render() {
      const dateFormat = 'YYYY-MM-DD';
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
      const leaseInfo = this.props.leaseInfo;
      let leaseOperType = this.props.operInfo.leaseOperType;
      console.log(this.props.basicData, '字典数据')
      const {shopLease, shopDepositype} = this.props.basicData

      return (
        <div className="relative">
          <Layout>
            <Content className='leaseEdit'  style= {{ padding: '25px 0', margin: '20px 0', backgroundColor: "#ECECEC" }}>
              <Icon type="tags-o" className='content-icon'/> <span className='content-title'>租约信息</span>
              <Row style={{padding: '25px 5rem'}}>
                <Checkbox onChange={this.changIshas} checked={this.state.isHas}>包含租约</Checkbox>
              </Row>
              { // disabled={this.props.shopsInfo.isDisabled}
                this.state.isHas ? 
             
              <div>
                  <Row  type="flex" justify="space-between" style={{paddingTop: "25px"}}>
                        <Col span={12}>
                          <Form ref={(e) => this.form = e} onSubmit={this.handleSubmit}>
                            <FormItem    {...formItemLayout}  label="当前经营" >
                              {getFieldDecorator('currentOperation', {
                                initialValue: leaseInfo.currentOperation,
                                rules: [{ required: true, message: '请输入当前经营'},],
                              })(
                                <Input className='someInput'/>
                              )}
                            </FormItem>
                            <FormItem    {...formItemLayout}  label="租金(元/月)">
                              {getFieldDecorator('rental', {
                                initialValue: leaseInfo.rental,
                                rules: [{ required: true, message: '请输入租约租金'},],
                              })(
                                <InputNumber  min={0}  className='someInput'/>
                              )}
                            </FormItem>
                            <FormItem    {...formItemLayout}  label="支付方式">
                              {getFieldDecorator('paymentTime', {
                                initialValue: leaseInfo.paymentTime,
                                rules: [{ required: true, message: '请选择支付方式'},],
                              })(
                                <Select size="large" className='someInput'>
                                {
                                  shopLease.map((item) =>
                                    <Option key={item.value}>{item.key}</Option>)
                                }
                              </Select>
                              )}
                            </FormItem>
                            <FormItem    {...formItemLayout}  label="返祖">
                              {getFieldDecorator('hasLeaseback', {
                                initialValue: leaseInfo.hasLeaseback,
                                // rules: [{ required: true, message: '请勾选是否返祖'},],
                              })(
                                <RadioGroup onChange={this.onChange} >
                                  <Radio value={true}>是</Radio>
                                  <Radio value={false}>否</Radio>
                                </RadioGroup>
                              )}
                            </FormItem>
                            {
                              this.state.hasLeaseback ? 
                            <div>
                            <FormItem    {...formItemLayout}  label="返祖时间(月)" className='redContent'>
                              {getFieldDecorator('backMonth', {
                                initialValue: leaseInfo.backMonth,
                                // rules: [{ validator: this.checkBackMonth },],
                                rules: [{ required: true, message: '请输入返祖时间'},],
                              })(
                                <InputNumber  min={0}  className='someInput' />
                              )}
                            </FormItem>
                            <FormItem    {...formItemLayout}  label="返祖比例(%)" className='redContent'>
                                {getFieldDecorator('backRate', {
                                  initialValue: leaseInfo.backRate,
                                  // rules: [{ validator: this.checkBackRate },],
                                  rules: [{ required: true, message: '请输入返祖比例'},],
                                })(
                                  <InputNumber  min={0}  className='someInput' />
                                )}
                            </FormItem>
                            </div> : null
                            }
                          </Form>
                        </Col>
                        <Col span={12}>
                          <FormItem    {...formItemLayout}  label="起止日期">
                            {getFieldDecorator('dateRange', {
                              initialValue: leaseInfo.dateRange,
                                rules: [{ required: true, message: '请选择起止日期'},],
                            })(
                              <RangePicker format={dateFormat} onChange={this.onChangeDate} className='someInput'/>
                            )}
                          </FormItem>
                          <FormItem    {...formItemLayout}  label="押金方式">
                              {getFieldDecorator('depositType', {
                                initialValue: leaseInfo.depositType,
                                rules: [{ required: true, message: '请选择押金方式'},],
                              })(
                                <Select size="large" className='someInput' onChange={this.changeDeposit}>
                                {
                                  shopDepositype.map((item) =>
                                    <Option key={item.value}>{item.key}</Option>)
                                }
                              </Select>
                              )}
                            </FormItem>
                            {
                              ['1', '2'].includes(this.state.depositType) ? null : 
                              <FormItem    {...formItemLayout}  label={`押金(${this.state.depositType === '20'? '元': '月'})`}>
                                  {getFieldDecorator('deposit', {
                                    initialValue: leaseInfo.deposit,
                                    rules: [{ required: true, message: '请输入租约押金'},],
                                  })(
                                    <InputNumber  min={1}  className='someInput'/>
                                  )}
                                </FormItem>
                             }
                            <FormItem    {...formItemLayout}  label="递增起始年">
                              {getFieldDecorator('upscaleStartYear', {
                                initialValue: leaseInfo.upscaleStartYear,
                                // rules: [{ required: true, message: '请输入递增比率'},],
                              })(
                                <Input   className='someInput' placeholder='从第几年开始递增'/> 
                              )}
                            </FormItem>
                            <FormItem    {...formItemLayout}  label="递增间隔">
                              {getFieldDecorator('upscaleInterval', {
                                initialValue: leaseInfo.upscaleInterval,
                                // rules: [{ required: true, message: '请输入递增比率'},],
                              })(
                                <Input  className='someInput' placeholder='每个多少年递增一次'/> 
                              )}
                            </FormItem>
                            <FormItem    {...formItemLayout}  label="递增比例(%)">
                              {getFieldDecorator('upscale', {
                                initialValue: leaseInfo.upscale,
                                // rules: [{ required: true, message: '请输入递增比率'},],
                              })(
                                <Input className='someInput' placeholder='每次递增多少比例'/> 
                              )}
                            </FormItem>
                           
                        </Col>
                  </Row>

                <Row type="flex" justify="space-between">
                  <Col span={12}>
                      
                  </Col>
                </Row>

                <Row type="flex" justify="space-between" className='beizhu'>
                  <Col span={24}>
                  <FormItem {...formItemLayout}  label="备注">
                        {getFieldDecorator('memo', {
                          initialValue: leaseInfo.memo,
                        })(
                          <Input type="textarea" rows={4}/>
                        )}
                    </FormItem>
                  </Col>
                </Row>
             </div> 
             : null
              }
             <Row type="flex" justify="center" className='BtnTop'>
                <Button type="primary" size='default' style={{width: "8rem"}} 
                       onClick={this.handleSave} loading={this.props.loading} disabled={this.props.shopsInfo.isDisabled}>保存</Button>
                { leaseOperType !== 'add' ? <Button size='default' className='oprationBtn' onClick={this.handleCancel}>取消</Button> : null }
            </Row>
            </Content>
          </Layout>
        </div>
      )
    }
}

function mapStateToProps(state) {
  return {
    leaseInfo: state.shop.shopsInfo.leaseInfo,
    operInfo: state.shop.operInfo,
    shopsInfo: state.shop.shopsInfo,
    basicData: state.basicData,
    loading: state.shop.leaseLoading
  }
}

const mapDispatchToProps = (dispatch) => {
  return {
    dispatch,
    save: (...args) => dispatch(saveShopLeaseAsync(...args)),
    viewShopLease: () => dispatch(viewShopLease())
  }
}


export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(LeaseEdit));