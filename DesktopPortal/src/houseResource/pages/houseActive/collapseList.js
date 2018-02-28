import { connect } from 'react-redux';
import { getShops, postCheckedArr } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Button, Row, Col, Tabs, Radio, Input, Menu,Spin,Collapse,Icon } from 'antd'
import moment from 'moment'

const Panel = Collapse.Panel;
const { Header, Sider, Content } = Layout;
const TabPane = Tabs.TabPane;

class CollapseList extends Component {
  state = {
    buildingNo: [],
    // arr: []
  }
  

  componentWillMount() {
    // console.log(this.props.list, '楼栋list')
    this.setState({ buildingNo: this.props.list })
  }
  componentDidMount() {
     
  }
  componentWillReceiveProps(newProps) {
    // console.log(this.props.list, '楼栋list')
    this.setState({ buildingNo: newProps.list })
  }
  changeChecked = (arr, childIndex, ParentIndex) => {
    
    let buildingNo = this.state.buildingNo.slice()
    buildingNo[ParentIndex].children[childIndex].checked = !buildingNo[ParentIndex].children[childIndex].checked
    // console.log(buildingNo[ParentIndex].children[childIndex].checked,  '比较')
    if (buildingNo[ParentIndex].children[childIndex].checked) {
      arr.push({
        id: buildingNo[ParentIndex].children[childIndex].id,
        buildingNo: buildingNo[ParentIndex].children[childIndex].buildingNo,
        floorNo: buildingNo[ParentIndex].children[childIndex].floorNo,
        number: buildingNo[ParentIndex].children[childIndex].number,
      })
    } else {
      let index = arr.findIndex(v => {
        return v.id === buildingNo[ParentIndex].children[childIndex].id
      })
      arr.splice(index, 1)
    }
    this.setState({ 
      buildingNo: buildingNo, 
    }, () => {
      this.props.dispatch(postCheckedArr({ 
        arr:  arr,
        type: this.props.type
      }))
      // console.log(arr, 'Shuzu ')
    })
  }
  clickItem = (child, childIndex, ParentIndex) => { // 勾选店铺
    let hotArr, pushArr
    if (this.props.type === 'hot') {
      hotArr = this.props.hotCheckedArr;
      this.changeChecked(hotArr, childIndex, ParentIndex)
    } else {
      pushArr = this.props.pushCheckedArr;
      this.changeChecked(pushArr, childIndex, ParentIndex)
    }
    
  
   
  }
  

  render() {
    const { buildingNo } = this.state
    return ( 
      <div className='collapseList'>
        <Collapse>
          {
            buildingNo.map((item, index) => 
              <Panel header={item.buildingNo} key={`${index}`}>
                  <div className='kanbanItem'>
                    <div className='itemContent'>
                    {
                      this.props.text === '审核中' ? // 如果在审核中
                        item.children.map((child, i) => 
                          <div className={child.checked ? 'notCheckedShop' : 'notcheckShop'} key={i} >
                              {
                                child.checked ? <Icon type="check" className='icon' size='small'/> : null
                              }
                              <div>{`${child.buildingNo}-${child.floorNo}-${child.number}`}</div>
                          </div>) 
                          :
                          item.children.map((child, i) => 
                          <div className={child.checked ? 'checkedShop' : 'checkShop'} key={i} 
                               onClick={() => this.clickItem(child, i, index)}>
                              {
                                child.checked ? <Icon type="check" className='icon' size='small'/> : null
                              }
                              <div>{`${child.buildingNo}-${child.floorNo}-${child.number}`}</div>
                          </div>) 

                    }
                    </div>
                  </div>
              </Panel>)
          }
       </Collapse>
      </div>
    )
  }
}

function mapStateToProps(state) {
  // console.log('首页', state.iex.todayReportListId);
  return {
    projectId: state.active.projectId,
    shopId: state.active.shopId,
    pushCheckedArr: state.active.pushCheckedArr,
    shopList: state.active.shopList,
    hotCheckedArr: state.active.hotCheckedArr,
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
    // getSalestatistics: (...args) => dispatch(getSalestatistics(...args))
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(CollapseList);