import { connect } from 'react-redux';
import { submitDynamic, priceEdit,getDynamicInfoList ,gotoThisShop} from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Input,Spin,Form, Icon, Row, Col, InputNumber,notification} from 'antd'
import moment from 'moment'
import TextPic from './textPic'

const FormItem = Form.Item;


class Price extends Component {
  state = {
    eidtFlag: false, // 是否点击编辑按钮
    shopId: ''
  }

  handleEdit = (e) => {
    this.props.dispatch(priceEdit())
    // this.setState({ eidtFlag: true })
  }

  handleSave = (e) => {
    e.preventDefault();
    const { descriptionValue, shopId,myTitleValue } = this.props
    console.log(shopId, '商铺id')
    this.props.form.validateFields((err, values) => {
      console.log(values, '吱吱吱')
      if (!err) {
        let obj = {
          updateType: 2,
          contentType: 'Price',
          contentId: shopId,
          updateContent: JSON.stringify(values),
          content: descriptionValue,
          title: myTitleValue
        }
        let isBtn;
        this.props.type !== 'lower' ? isBtn = 'total': isBtn = 'guide'
        if (descriptionValue) {
          this.props.dispatch(submitDynamic({ obj: obj, isBtn: isBtn }));
        } else {
          notification.warning({
            message: '必须输入描述信息',
            duration: 3
          })
        }
      }
    })
  }
  initData = (id) => {
    this.setState({ shopId: id }, () => {
        console.log('请求接口没有？？？？')
        if (this.props.type === 'dynamic') {
            // 如果是动态房源，则需要获取最后一个审核的信息
            let condition = {
                pageSize: 1,
                isCurrent:true,
                contentTypes: ['ReportRule'],
                contentIds: [this.state.shopId]
            }
            this.props.dispatch(getDynamicInfoList({ 
                condition: condition, 
                id: this.state.shopId, 
                updateType: 1 
            }))
        }
    })
}
componentWillMount() {
    console.log(this.state.shopId, '本地id')
    this.initData(this.props.shopId)
}
componentWillReceiveProps(newProps) {
    if (newProps.shopId === this.state.shopId) {
        return
    }
    console.log('变了id', newProps.shopId)
    this.initData(newProps.shopId)
}

// 点击取消按钮
handleCancel = () => {
  console.log('点击取消按钮')
  this.props.dispatch(gotoThisShop({ id: this.props.shopId, type: 'dynamic' }));
}
  


  render() {
    const { getFieldDecorator } = this.props.form;
    const formItemLayout = {
      labelCol: { span: 4},
      wrapperCol: { span: 12 },
    };
    const  {shopInfo, dynamicShopsInfo, eidtFlagPrice, dynamicStatusArr} = this.props
    let basicInfo = (this.props.shopInfo || {}).basicInfo
    let a = ((this.props.statusArr || []).find(v => {
        return v.action === 'Price'
    })) || {}
    // console.log(shopInfo, this.props.dynamicShopsInfo, this.props.dynamicStatusArr, '??价格')
    if (JSON.stringify(a) === '{}') { // 没有进行审核的
        basicInfo = (shopInfo || {}).basicInfo
    } else { // 进行过审核的
        if ([1, 16].includes(dynamicStatusArr.examineStatus)) {
          basicInfo = (dynamicShopsInfo || {}).basicInfo
        } else { // 审核通过以及未提交审核就显示本来的数据
          basicInfo = (shopInfo || {}).basicInfo
        }
    }


    return (
      <div>
        <div style={{backgroundColor: "#ECECEC"}}>
        {
          eidtFlagPrice ? 
          <Form layout="horizontal" style={{padding: '25px 0', }}>
                <Row>
                  <Col span={12}>
                    <FormItem    {...formItemLayout} label="总价(万元/㎡)">
                      {getFieldDecorator('totalPrice', {
                        initialValue: basicInfo.totalPrice,
                        rules: [{ required: true, message: '请输入楼盘总价' },],
                      })(
                        <InputNumber min={1} />
                        )}
                    </FormItem>
                  </Col>
                </Row>
                 <Row style={{marginTop: '15px'}}>
                  <Col span={12}>
                      <FormItem    {...formItemLayout} label="内部最低总价(万元/㎡)">
                      {getFieldDecorator('guidingPrice', {
                        initialValue: basicInfo.guidingPrice,
                        rules: [{ required: true, message: '请输入内部最低总价' },],
                      })(
                        <InputNumber min={1} />
                        )}
                      </FormItem>
                  </Col>
                </Row>
          </Form>
          :
            <Form layout="horizontal" style={{padding: '25px 0'}}>
              <Row type="flex">
                  <Col span={20} style={{textAlign: 'right'}}>
                  {
                    this.props.text === '审核中' ? null: <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                  }
                  </Col>
              </Row>
              <Row className='viewRow'>
                <Col span={12}>总价：<span className='redSpan'>{basicInfo.totalPrice ? `${basicInfo.totalPrice} 万元/㎡` : '无'} </span></Col>
              </Row>
              <Row style={{marginTop: '15px'}}>
                <Col span={12}>内部最低总价：<span className='redSpan'>{basicInfo.guidingPrice ? `${basicInfo.guidingPrice} 万元` : '无'} </span></Col>
              </Row>
            </Form>
        }
        </div>
        <TextPic text={this.props.text}/>
        {
          this.props.text === '审核中' ? null :
          <Row style={{marginTop: '25px'}}>
              <Col span={24} style={{ textAlign: 'center' }}>
                  <Button type="primary" 
                          style={{width: "8rem"}} 
                          loading={this.props.submitLoading}
                          onClick={this.handleSave}>保存</Button>
                  <Button className="login-form-button formBtn" 
                          onClick={this.handleCancel}>取消</Button>
              </Col>
          </Row>
         }
      </div>
    )
  }
}

function mapStateToProps(state) {
  // console.log('首页', state.index.todayReportListId);
  return {
    shopInfo: state.shop.shopsInfo,
    descriptionValue: state.active.descriptionValue,
    operInfo: state.shop.operInfo,
    submitLoading: state.active.submitLoading,
    submitDisabled: state.active.submitDisabled,
    shopId: state.active.shopId,
    eidtFlagPrice: state.shop.eidtFlagPrice,
    dynamicShopsInfo: state.shop.dynamicShopsInfo,
    statusArr: state.active.statusArr,
    dynamicStatusArr: state.active.dynamicStatusArr,
    myTitleValue: state.active.myTitleValue
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
    // getSalestatistics: (...args) => dispatch(getSalestatistics(...args))
  };
}
const WrappedForm = Form.create()(Price);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);