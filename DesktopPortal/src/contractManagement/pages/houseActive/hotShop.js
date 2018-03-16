import { connect } from 'react-redux';
import { getShops, submitDynamic,gotoThisBuild } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Input,Spin, Row, Col,Icon,notification} from 'antd'
import moment from 'moment'
import CollapseList from './collapseList'
import TextPic from './textPic'


class ActiveProject extends Component {
  state = {
    buildingNo: [],
    buildingId: ''
  }

  componentWillMount() {
    // console.log(this.props.projectId, 'idididii')
    this.setState({buildingId: this.props.projectId})
    this.props.dispatch(getShops({
      id: this.props.projectId,
      saleStatus: ['2']
    }))
  }
 
  getBuildingNo = (list) => {
    let buildingNo = [], newShopList= [];
    newShopList = [ ...list ]
    // console.log(newShopList, '???')
    newShopList.forEach((v) => {
      buildingNo.push({ buildingNo: v.buildingNo })
    })
    let hash = {};
    buildingNo = buildingNo.reduce((item, next) => {  // 去重楼栋数
      hash[next.buildingNo] ? '' : hash[next.buildingNo] = true && item.push(next);
      return item
    }, [])
    buildingNo.map(v => v.children = [])
    newShopList.forEach(v => {
      let index = buildingNo.findIndex(item => item.buildingNo === v.buildingNo)
      v.checked = v.isHot
      buildingNo[index].children.push(v)
    })
    this.setState({ buildingNo: buildingNo }, () => {
      // console.log(this.state.buildingNo, '有没有？？？？？？')
    })
  } 

  componentWillReceiveProps(newProps) {
    // console.log(newProps.shopList, '列表')
    if (this.state.buildingId !== newProps.projectId) {
      this.props.dispatch(getShops({
        id: newProps.projectId,
        saleStatus: ['2']
      }))
      this.setState({buildingId: newProps.projectId})
    }
    this.getBuildingNo(newProps.shopList);
  }

  // 点击取消按钮
  handleCancel = () => {
    // console.log('点击取消按钮')
    this.props.dispatch(gotoThisBuild({ id: this.props.projectId, type: 'dynamic' }));
  }

  handleSave = () => {
    const { hotCheckedArr, descriptionValue, projectId, buildInfo, myTitleValue } = this.props
    if (descriptionValue) {
      
    let obj = {
      updateType: 1,
      contentType: 'ShopsHot',
      contentName: buildInfo.basicInfo.name,
      contentId: projectId,
      updateContent: JSON.stringify({shopList: hotCheckedArr, memo: '', fileList: []}),
      title: myTitleValue,
      content: descriptionValue,
      ext2: buildInfo.basicInfo.areaFullName
    }
    console.log(obj, '热销obj')
    // this.props.dispatch(submitDynamic({ obj: obj, type: 'hot' }))
    // 2017-12-25号 更改 房源动态提交接口统一为调用：/api/updaterecord/submit，
    // 所以不用区分热卖和其它板块，即将type: hot 去掉。
    this.props.dispatch(submitDynamic({ obj: obj })) 
    } else {
        notification.warning({
          message: '必须输入描述信息',
          duration: 3
        })
    }
} 
 
  render() {
    const { buildingNo } = this.state
    const { hotBtn } = this.props.submitDisabled || {}
    return ( 
      // <Spin spinning={this.props.isLoading}>
      <div className='hotShopPage' style={{ marginTop: '25px' }}>
      {
          buildingNo.length === 0 
          ? 
          <div style={{textAlign:'center',fontSize: '1rem'}}>
            <Icon type="frown-o" /> 
            <span style={{marginLeft: '15px'}}>暂无商铺数据</span>
          </div> 
          : 
          <div>

          <CollapseList list={buildingNo} type='hot'/>

          <TextPic />
          
          {
            this.hot
          }
          <Row style={{marginTop: '25px'}}>
              <Col span={24} style={{ textAlign: 'center' }}>
                  <Button type="primary" 
                          style={{width: "8rem"}} 
                          // disabled={hotBtn}
                          loading={this.props.submitLoading}
                          onClick={this.handleSave}>保存</Button>
                  <Button className="login-form-button formBtn" 
                          onClick={this.handleCancel}>取消</Button>
              </Col>
          </Row>

          </div> 
      }
      </div>
      // </Spin>
    )
  }
}

function mapStateToProps(state) {
  // console.log('首页', state.index.todayReportListId);
  return {
    isLoading: state.active.isLoading,
    projectId: state.active.projectId,
    shopList: state.active.shopList,
    submitLoading: state.active.submitLoading,
    hotCheckedArr: state.active.hotCheckedArr,
    pushCheckedArr: state.active.pushCheckedArr,
    submitDisabled: state.active.submitDisabled,
    descriptionValue: state.active.descriptionValue,
    buildInfo: state.building.buildInfo,
    myTitleValue: state.active.myTitleValue
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}

export default connect(mapStateToProps, mapDispatchToProps)(ActiveProject);