import { connect } from 'react-redux';
import { getExaminesStatus,gotoThisShop, submitDynamic,getDynamicInfoList } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Button, Row, Col, Tabs,notification,Spin} from 'antd'
import moment from 'moment'
import Price from './price'
import AttachInfo from '../shops/detail/attachInfo'
import AttachEdit from '../shops/edit/attachEdit'
import TextPic from './textPic'
import './houseActive.less'

const { Header, Sider, Content } = Layout;
const TabPane = Tabs.TabPane;

class ActiveShop extends Component {
  state = {
    current: '',
    shopId: '',
  }
  

  componentWillMount() {
    console.log(this.props.shopId, '楼盘id')
    this.setState({shopId: this.props.shopId}, () => {
      this.props.dispatch(getExaminesStatus({
        id: this.state.shopId
      }));
    })
  }
  componentWillReceiveProps(newProps) {
    console.log(newProps.shopId, '楼盘id新的')
    if (this.props.shopId === newProps.shopId) {
      return
    }
    
    this.setState({shopId:newProps.shopId}, () => {
      this.props.dispatch(getExaminesStatus({
        id: this.state.shopId
      }));
    })
  }
  handleChange = (key) => {
    const { shopId } = this.state;
    let condition;
    this.setState({current: key})
    switch (key) {
      case '1':  // 总价
      condition = {
          pageSize: 1,
          contentTypes: ['Price'],
          contentIds: [shopId]
      }
      this.props.dispatch(getDynamicInfoList({ 
          condition: condition, 
          id: shopId, 
          updateType: 2
      }));break;
      default:
      condition = {
          pageSize: 1,
          contentTypes: ['Image'],
          contentIds: [shopId]
      }
      this.props.dispatch(getDynamicInfoList({ 
          condition: condition, 
          id: shopId, 
          updateType: 2 
      }));break;
    }
  }
  getStatus = (str) => {
    let a = ((this.props.statusArr || []).find(v => {
      return v.action === str
    })) || {}
    let text;
    switch(a.examineStatus){
      case 1: return text = '审核中';
      case 2: return text = '审核通过';
      case 3: return text = '审核驳回';
      default: return text = ''
    }
  }
  handleSave = () => {
    let {completeFileList, deletePicList,shopId, descriptionValue, myTitleValue} = this.props
    let obj;
    console.log(completeFileList, deletePicList, '图片')
    if (completeFileList.length !== 0) {
      obj = {
        updateType: 2,
        contentType: 'Image',
        contentId: shopId,
        updateContent: JSON.stringify(completeFileList),
        content: descriptionValue,
        title: myTitleValue
      }
    }
    if (deletePicList.length !== 0) { // 删除图片
      obj = {
        updateType: 2,
        contentType: 'Image',
        contentId: shopId,
        updateContent: JSON.stringify(deletePicList),
        content: descriptionValue,
        title: myTitleValue
      }
    }
    console.log(obj, '最新的图片商铺')
    if (descriptionValue) {
      this.props.dispatch(submitDynamic({ obj: obj }));
    } else {
      notification.warning({
        message: '必须输入描述信息',
        duration: 3
      })
    }
  }                 
// 点击取消按钮
handleCancel = () => {
  console.log('点击取消按钮')
  this.props.dispatch(gotoThisShop({ id: this.props.shopId, type: 'dynamic' }));
}   
  


  render() {
    const { operInfo } = this.props
    return ( 
      <Spin spinning={this.props.isLoading}>
      <div className="relative">
          <Layout>
              <Content className='content activeShopPage'>
              <Tabs defaultActiveKey="1" style={{marginTop: '20px'}}>

                  <TabPane 
                  tab={<span><i className='iconfont icon-zongjia myicon' />总价
                  <span style={{color: 'red', paddingLeft: '10px'}}>{this.getStatus('Price')}</span>
                  </span>} key="1">
                    <Price text={this.getStatus('Price')}/>
                  </TabPane>

                  {/* <TabPane tab={<span><i className='iconfont icon-tupian myicon' />图片 */}
                  {/* <span style={{color: 'red', paddingLeft: '10px'}}>{this.getStatus('purchase')}</span> */}
                  {/* </span>} key="3"> */}
                    {
                      // operInfo.attachPicOperType === "view" ? 
                      // <AttachInfo type='dynamic' parentPage='shops'/> : 
                      // <AttachEdit type='dynamic' parentPage='shops'/>
                    }
                    {/* <TextPic /> */}
                    {
                      // this.getStatus('purchase') === '审核中' ? null : 
                      // <Row style={{marginTop: '25px'}}>
                      //     <Col span={24} style={{ textAlign: 'center' }}>
                      //         <Button type="primary" 
                      //                 style={{width: "8rem"}} 
                      //                 onClick={this.handleSave}>保存</Button>
                      //         <Button className="login-form-button formBtn" 
                      //                 onClick={this.handleCancel}>取消</Button>
                      //     </Col>
                      // </Row>
                    }
                  {/* </TabPane> */}

                  {/* <TabPane tab={<span><i className='iconfont icon-fujian myicon' />附件</span>} key="4">
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
  // console.log('首页', state.index.todayReportListId);
  return {
    isLoading: state.active.isLoading,
    shopId: state.active.shopId,
    operInfo: state.shop.operInfo,
    shopInfo: state.shop.shopsInfo,
    descriptionValue: state.active.descriptionValue,
    statusArr: state.active.statusArr,
    completeFileList: state.shop.completeFileList,
    deletePicList: state.shop.deletePicList,
    myTitleValue: state.active.myTitleValue
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(ActiveShop);