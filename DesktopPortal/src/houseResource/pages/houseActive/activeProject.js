import { connect } from 'react-redux';
import { getShops, gotoThisBuild, submitDynamic, getExaminesStatus, reportIsUse, getDynamicInfoList ,getDicParList, changeShowGroup} from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Button, Row, Col, Tabs, Input, Spin, notification } from 'antd'
import moment from 'moment'
import HotShop from './hotShop'
import PushShop from './pushShop'
import RulesInfo from '../buildingDish/detail/rulesInfo'
import RulesEdit from '../buildingDish/edit/rulesEdit'
import RulesTemplateInfo from '../buildingDish/detail/rulesTemplateInfo'
import RulesTemplateEdit from '../buildingDish/edit/rulesTemplateEdit'
import CommissionInfo from '../buildingDish/detail/commissionInfo'
import CommissionEdit from '../buildingDish/edit/commissionEdit'
import Youhui from './youhui'
import AttachInfo from '../shops/detail/attachInfo'
import AttachEdit from '../shops/edit/attachEdit'
import BuildingNoInfo from '../buildingDish/detail/buildingNoInfo'
import BuildingNoEdit from '../buildingDish/edit/buildingNoEdit'
import TextPic from './textPic'
import './houseActive.less'

const { Header, Sider, Content } = Layout;
const TabPane = Tabs.TabPane;

class ActiveProject extends Component {
  state = {
    current: '',
    projectId: '',
    mrForm: {},
    commissonForm: {},
  }

  setChildForm = (form) => { // 从子组件获取的form表单
    this.setState({ mrForm: form })
  }
  setCommisson = (form) => { // 佣金方案的this.props.form
    this.setState({ commissonForm: form })
  }

  handleChange = (key) => {
    // 热卖户型只展示该楼盘下的 在售['2'] 商铺 
    // 加推只展示 待售['1'] 的商铺
    this.props.dispatch(changeShowGroup({ type: 1 }))
    const { projectId } = this.props;
    let condition;
    this.setState({ current: key })
    switch (key) {
      case '1':  // 热卖户型
        this.props.dispatch(getShops({
          id: projectId,
          saleStatus: ['2']
        })); break;
      case '2': // 加推
        this.props.dispatch(getShops({
          id: projectId,
          saleStatus: ['1']
        })); break;
      case '3': // 报备规则
        condition = {
          pageSize: 1,
          contentTypes: ['ReportRule'],
          contentIds: [projectId]
        }
        this.props.dispatch(getDynamicInfoList({
          condition: condition,
          id: projectId,
          updateType: 1
        })); break;
      case '4': // 佣金方案
        condition = {
          pageSize: 1,
          contentTypes: ['CommissionType'],
          contentIds: [projectId]
        };
        this.props.dispatch(getDynamicInfoList({
          condition: condition,
          id: projectId,
          updateType: 1
        })); break;
      case '5': // 楼栋批次
        console.log('楼栋批次')
        condition = {
          pageSize: 1,
          contentTypes: ['BuildingNo'],
          contentIds: [projectId]
        };
        this.props.dispatch(getDynamicInfoList({
          condition: condition,
          id: projectId,
          updateType: 1
        })); break;
      case '6': // 优惠政策
        console.log('优惠政策')
        condition = {
          pageSize: 1,
          contentTypes: ['DiscountPolicy'],
          contentIds: [projectId]
        }
        this.props.dispatch(getDynamicInfoList({
          condition: condition,
          id: projectId,
          updateType: 1
        })); break;
      case '7': // 图片
        console.log('图片')
        condition = {
          pageSize: 1,
          contentTypes: ['Image'],
          contentIds: [projectId]
        }
        this.props.dispatch(getDynamicInfoList({
          condition: condition,
          id: projectId,
          updateType: 1
        })); break;
      default:
        condition = {
          pageSize: 1,
          contentTypes: ['Attachment'],
          contentIds: [projectId]
        }
        this.props.dispatch(getDynamicInfoList({
          condition: condition,
          id: projectId,
          updateType: 1
        }))
    }
  }

  handleSave = (current) => {
    console.log(current, '什么？？目前是')
    const { descriptionValue, projectId, buildInfo, shopInfo,myTitleValue } = this.props
    let obj, ruleInfo
    switch (current) {
      case '3':
        // 报备模板+报备规则 this.props.templateData 
        // console.log(this.state.mrForm, '有没有', buildInfo)
        if (Object.keys(this.state.mrForm).length !== 0) {
          this.state.mrForm.validateFields((err, values) => {
            if (!err) {
              // console.log(values, 'hahah')
              values.reportTime = moment(values.reportTime).format('YYYY-MM-DD')
              values.liberatingEnd = new Date(values.liberatingEnd.format('YYYY-MM-DD HH:mm'))
              values.liberatingStart = new Date(values.liberatingStart.format('YYYY-MM-DD HH:mm'))
              // values.reportedTemplate = JSON.stringify(this.props.templateData)
              if (this.props.templateData.length !== 0) {
                values.reportedTemplate = this.props.templateData
              } else {
                values.reportedTemplate = (buildInfo.ruleInfo || {}).reportedTemplate
              }
              // values.isUse = this.props.isUse
              obj = {
                updateType: 1,
                contentType: 'ReportRule',
                contentId: projectId,
                updateContent: JSON.stringify(values),
                content: descriptionValue,
                title: myTitleValue
              }
              // console.log(values, '报备规则obj')
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
        } else { // 只保存模板的时候
          ruleInfo = this.props.buildInfo.ruleInfo || {}
          let value = {
            ...this.props.buildInfo.ruleInfo,
            // reportedTemplate: JSON.stringify(this.props.templateData),
            reportedTemplate: this.props.templateData,
            isUse: this.props.isUse
          }
          obj = {
            updateType: 1,
            contentType: 'ReportRule',
            contentId: projectId,
            updateContent: JSON.stringify(value),
            content: descriptionValue,
            title: myTitleValue
          }
          // console.log(value, '报备规则obj')
          if (descriptionValue) {
            this.props.dispatch(submitDynamic({ obj: obj }));
          } else {
            notification.warning({
              message: '必须输入描述信息',
              duration: 3
            })
          }
        }; break;
      case '4':
        this.state.commissonForm.validateFields((err, values) => {
          if (!err) {
            // console.log(values, '佣金方案')
            obj = {
              updateType: 1,
              contentType: 'CommissionType',
              contentId: projectId,
              updateContent: JSON.stringify(values),
              content: descriptionValue,
              title: myTitleValue
            }
            // console.log(obj, '佣金方案！！！！')
            if (descriptionValue) {
              this.props.dispatch(submitDynamic({ obj: obj }));
            } else {
              notification.warning({
                message: '必须输入描述信息',
                duration: 3
              })
            }
          }
        }); break;
      case '5':
        let mySelectedRows = this.props.dynamicBuildInfo.buildingNoInfos;//this.props.buildingNoInfos
        // console.log(mySelectedRows, '楼栋批次');
        if (mySelectedRows) {
          mySelectedRows.map(r => {
            if (typeof (r.openDate) === "object") {
              r.openDate = moment(r.openDate).format("YYYY-MM-DD")
            }
            if (typeof (r.deliveryDate) === "object") {
              r.deliveryDate = moment(r.deliveryDate).format("YYYY-MM-DD")
            }
          });
        }
        obj = {
          updateType: 1,
          contentType: 'BuildingNo',
          contentId: projectId,
          updateContent: JSON.stringify(mySelectedRows),
          content: descriptionValue,
          title: myTitleValue
        }
        // console.log(obj, '楼栋批次请求体')
        if (descriptionValue) {
          this.props.dispatch(submitDynamic({ obj: obj }));
        } else {
          notification.warning({
            message: '必须输入描述信息',
            duration: 3
          })
        } break;
      // case '6':
      //   let ruleInfo = (buildInfo.ruleInfo || {}).ruleInfo
      //   console.log(ruleInfo, '最新的优惠政策')
      //   break;
      case '7':
        let { completeFileList, deletePicList } = this.props
        console.log(completeFileList, deletePicList, '图片')
        if (completeFileList.length !== 0) {
          obj = {
            updateType: 1,
            contentType: 'Image',
            contentId: projectId,
            updateContent: JSON.stringify(completeFileList),
            content: descriptionValue,
            title: myTitleValue
          }
        }
        if (deletePicList.length !== 0) { // 删除图片
          obj = {
            updateType: 1,
            contentType: 'Image',
            contentId: projectId,
            updateContent: JSON.stringify(deletePicList),
            content: descriptionValue,
            title: myTitleValue
          }
        }
        console.log(obj, 'pp')
        if (descriptionValue) {
          this.props.dispatch(submitDynamic({ obj: obj }));
        } else {
          notification.warning({
            message: '必须输入描述信息',
            duration: 3
          })
        } break;
      default:  // 附件
        ruleInfo = (buildInfo.ruleInfo || {}).ruleInfo
        console.log(ruleInfo, '附件')
        obj = {
          updateType: 1,
          contentType: 'Attachment',
          contentId: projectId,
          updateContent: JSON.stringify(ruleInfo),
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
  }
  componentWillMount () {
    this.props.dispatch(getDicParList(['PHOTO_CATEGORIES']));
  }
  // 点击取消按钮
  handleCancel = () => {
    console.log('点击取消按钮')
    this.props.dispatch(gotoThisBuild({ id: this.props.projectId, type: 'dynamic' }));
  }

  getStatus = (str) => {
    let a = ((this.props.statusArr || []).find(v => {
      return v.action === str
    })) || {}
    let text;
    switch (a.examineStatus) {
      case 1: return text = '审核中';
      // case 2: return text = '审核通过';
      case 3: return text = '审核驳回';
      // case 5: return '未提交审核';
      default: return text = ''
    }
  }
  //  暂停报备
  isReport = () => {
    let isUse = this.props.isUse
    console.log('哈哈,暂停报备？？？', isUse)
    this.props.dispatch(reportIsUse({ isUse: !isUse }));
  }


  render() {
    const submitBtn = (
      <Row style={{ marginTop: '25px' }}>
        <Col span={24} style={{ textAlign: 'center' }}>
          <Button type="primary"
            style={{ width: "8rem" }}
            loading={this.props.submitLoading}
            onClick={() => this.handleSave(this.state.current)}>保存</Button>
          <Button className="login-form-button formBtn"
            onClick={this.handleCancel}>取消</Button>
        </Col>
      </Row>
    )
    const { operInfo, statusArr, isUse } = this.props

    return (
      <Spin spinning={this.props.isLoading}>
        <div className="relative">
          <Layout>
            <Content className='content activeProjectPage'>

              <Tabs defaultActiveKey="1" style={{ marginTop: '20px' }} onChange={this.handleChange}>

                <TabPane
                  tab={<span><i className='iconfont icon-remai myicon' />热卖户型</span>}
                  key="1">
                  <HotShop />
                </TabPane>

                <TabPane
                  tab={<span><i className='iconfont icon-jiatui myicon' />加推
                        <span style={{ color: 'red', paddingLeft: '10px' }}>{this.getStatus('ShopsAdd')}</span>
                  </span>} key="2">
                  <PushShop text={this.getStatus('ShopsAdd')} />
                </TabPane>

                <TabPane
                  tab={<span><i className='iconfont icon-guize myicon' />报备规则
                        <span style={{ color: 'red', paddingLeft: '10px' }}>{this.getStatus('ReportRule')}</span>
                  </span>} key="3">
                  {
                    operInfo.rulesOperType === "view" ? <RulesInfo type='dynamic' /> : <RulesEdit type='dynamic' setChildForm={this.setChildForm} />
                  }
                  {
                    operInfo.rulesTemplateOperType === "view" ? <RulesTemplateInfo type='dynamic' /> : <RulesTemplateEdit type='dynamic' />
                  }
                  {/* <div style={{backgroundColor: "#ECECEC", width:'100%', padding: '15px 0'}}>
                      <Button type='primary' 
                              size='small'
                              style={{marginLeft: '50px'}} 
                              onClick={this.isReport}>
                              {
                                isUse ? '暂停报备' : '启动报备'
                              }
                      </Button>
                    </div> */}

                  <TextPic text={this.getStatus('ReportRule')} />
                  {
                    this.getStatus('ReportRule') === '审核中' ? null : submitBtn
                  }
                </TabPane>

                <TabPane
                  tab={<span><i className='iconfont icon-yongjin myicon' />佣金方案
                      <span style={{ color: 'red', paddingLeft: '10px' }}>{this.getStatus('CommissionType')}</span>
                  </span>} key="4">
                  {operInfo.commissionOperType === "view" ? <CommissionInfo type='dynamic' /> : <CommissionEdit type='dynamic' setCommisson={this.setCommisson} />}
                  <TextPic text={this.getStatus('CommissionType')} />
                  {
                    this.getStatus('CommissionType') === '审核中' ? null : submitBtn
                  }
                </TabPane>

                <TabPane
                  tab={<span><i className='iconfont icon-pici myicon' />楼栋批次（开盘时间）
                        <span style={{ color: 'red', paddingLeft: '10px' }}>{this.getStatus('BuildingNo')}</span>
                  </span>} key="5">
                  {operInfo.batchBuildOperType === "view" ? <BuildingNoInfo type='dynamic' /> : <BuildingNoEdit type='dynamic' />}
                  <TextPic text={this.getStatus('BuildingNo')} />
                  {
                    this.getStatus('BuildingNo') === '审核中' ? null : submitBtn
                  }
                </TabPane>

                <TabPane
                  tab={<span><i className='iconfont icon-youhui myicon' />优惠政策
                        <span style={{ color: 'red', paddingLeft: '10px' }}>{this.getStatus('DiscountPolicy')}</span>
                  </span>} key="6">
                  <Youhui text={this.getStatus('DiscountPolicy')} />
                </TabPane>

                {/* <TabPane tab={<span><i className='iconfont icon-tupian myicon' />图片 */}
                            {/* <span style={{color: 'red', paddingLeft: '10px'}}>{this.getStatus('purchase')}</span> */}
                {/* </span>} key="7"> */}
                  {
                    // operInfo.attachPicOperType === "view" ?
                    //   <AttachInfo parentPage="building" type='dynamic' /> :
                    //   <AttachEdit parentPage="building" type='dynamic' />
                  }
                  {/* <TextPic text={this.getStatus('purchase')} /> */}
                  {/* {submitBtn} */}
                {/* </TabPane> */}
                {/* <TabPane tab={<span><i className='iconfont icon-fujian myicon' />附件</span>} key="8">
                    Tab 2
                  </TabPane> */}
              </Tabs>

            </Content>
          </Layout>
        </div>
      </Spin>
    )
  }
}

function mapStateToProps(state) {
  return {
    isLoading: state.active.isLoading,
    projectId: state.active.projectId,
    shopList: state.active.shopList,
    operInfo: state.building.operInfo,
    buildInfo: state.building.buildInfo,
    shopInfo: state.shop.shopsInfo,
    descriptionValue: state.active.descriptionValue,
    submitLoading: state.active.submitLoading,
    statusArr: state.active.statusArr,
    hotOpertype: state.active.hotOpertype,
    completeFileList: state.shop.completeFileList,
    deletePicList: state.shop.deletePicList,
    templateData: state.building.templateData,
    mySelectedRows: state.building.mySelectedRows,
    isUse: state.active.isUse,
    dynamicBuildInfo: state.building.dynamicBuildInfo,
    myTitleValue: state.active.myTitleValue
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(ActiveProject);