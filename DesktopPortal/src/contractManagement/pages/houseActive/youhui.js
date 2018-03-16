import { connect } from 'react-redux';
import { submitDynamic, youhuiEdit ,getDynamicInfoList,gotoThisBuild,youhuiView} from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Input,Spin,Form, Icon, Row, Col,notification } from 'antd'
import moment from 'moment'
import TextPic from './textPic'

const FormItem = Form.Item;


class Youhui extends Component {
  state = {
    // eidtFlag: false, // 是否点击编辑按钮
    projectId: ''
  }
  
  handleEdit = (e) => {
    this.props.dispatch(youhuiEdit())
    // this.setState({ eidtFlag: true })
  }
  initData = (id) => {
    this.setState({ projectId: id }, () => {
        // console.log('请求接口没有？？？？')
        if (this.props.type === 'dynamic') {
            // 如果是动态房源，则需要获取最后一个审核的信息
            let condition = {
                pageSize: 1,
                isCurrent:true,
                contentTypes: ['DiscountPolicy'],
                contentIds: [this.state.projectId]
            }
            this.props.dispatch(getDynamicInfoList({ 
                condition: condition, 
                id: this.state.projectId, 
                updateType: 1 
            }))
        }
    })
}
componentWillMount() {
    // console.log(this.state.projectId, '本地id')
    this.initData(this.props.projectId)
}
componentWillReceiveProps(newProps) {
    if (newProps.projectId === this.state.projectId) {
        return
    }
    console.log('变了id', newProps.projectId)
    this.initData(newProps.projectId)
}
  handleSave = (e) => {
    e.preventDefault();
    const { descriptionValue, projectId, myTitleValue } = this.props
    this.props.form.validateFields((err, values) => {
      if (!err) {
        let obj = {
          updateType: 1,
          contentType: 'DiscountPolicy',
          contentId: projectId,
          updateContent: JSON.stringify(values),
          content: descriptionValue,
          title: myTitleValue
        }
        if (descriptionValue) {
          this.props.dispatch(submitDynamic({ obj: obj }));
        } else {
          notification.warning({
            message: '必须输入描述信息',
            duration: 3
          })
        }
      }
    })
  }
// 点击取消按钮
handleCancel = () => {
  this.props.dispatch(gotoThisBuild({ id: this.props.projectId, type: 'dynamic' }));
}

  


  render() {
    const { getFieldDecorator } = this.props.form;
    const formItemLayout = {
      labelCol: { span: 2 },
      wrapperCol: { span: 14 },
    };
    const { eidtFlagYhui, dynamicBuildInfo, buildInfo ,dynamicStatusArr} = this.props
    let relShopInfo;
    let a = ((this.props.statusArr || []).find(v => {
      return v.action === 'DiscountPolicy'
    })) || {}
    // console.log(buildInfo , dynamicBuildInfo,this.props.dynamicStatusArr, '??优惠政策')
    if (JSON.stringify(a) === '{}') { // 没有进行审核的
      relShopInfo = buildInfo.relShopInfo;
    } else { // 进行过审核的
        if ([1, 16].includes(dynamicStatusArr.examineStatus)) {  //  0, 1, 8, 16
          relShopInfo = dynamicBuildInfo.relShopInfo;
          // this.props.dispatch(youhuiView())
        } else { // 审核通过以及未提交审核就显示本来的数据
          relShopInfo = buildInfo.relShopInfo;
        }
    }
    // console.log(this.props.eidtFlagYhui, '状态？？')

    return (
      <div>
        <div style={{backgroundColor: "#ECECEC"}}>
        {
          eidtFlagYhui ? 
          <Form layout="horizontal" style={{padding: '25px 0'}} >
                <FormItem {...formItemLayout} label='优惠政策' >
                  {getFieldDecorator('preferentialPolicies', {
                      initialValue: relShopInfo.preferentialPolicies
                  })(
                      <Input type="textarea" placeholder="请输入优惠政策"  style={{ height: '100px' }}/>
                  )}
                </FormItem>
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
                  <Col span={24}>优惠政策：{relShopInfo.preferentialPolicies ? relShopInfo.preferentialPolicies : '暂无优惠政策' }</Col>
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
     buildInfo: state.building.buildInfo,
     descriptionValue: state.active.descriptionValue,
     operInfo: state.building.operInfo,
     submitLoading: state.active.submitLoading,
     eidtFlagYhui: state.building.eidtFlagYhui,
     statusArr: state.active.statusArr,
     dynamicBuildInfo: state.building.dynamicBuildInfo,
     projectId: state.active.projectId,
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
const WrappedForm = Form.create()(Youhui);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);