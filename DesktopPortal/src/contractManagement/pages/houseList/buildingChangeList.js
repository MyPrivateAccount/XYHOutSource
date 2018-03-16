import { connect } from 'react-redux';
import { changeNowListInfo, searchMyBuildingList, getThisBuilding, 
  getMyCustomerInfo, statusCount, upDateUserTypeValue } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Row, Col, Tabs, Radio, Input, Icon, Tooltip ,
  Popconfirm, Spin } from 'antd'
import buildingChangeList from './buildingChangeList.less'

const { Header, Sider, Content } = Layout;
const Search = Input.Search;

class BuildingChangeList extends Component {
  state = {
  }
  componentWillMount() {
  }
  componentWillReceiveProps(newProps) {
    
  }
  search = (value) => {
    this.props.searchMyBuildingList({value: value, type: 1}) // 1 表示切换楼盘搜索
  }
  pressEnterSearch = (e) => {
    let value = e.target.value
    this.props.searchMyBuildingList({value: value, type: 1})
  }
  handleClick = (v, index) => {
    this.props.dispatch(upDateUserTypeValue({entity: v, index: index})) // 记录当前用户的历史字段 ===saga
  }
  render() {
    const { myIndex } = this.state
    let { isLoading, operInfo, changeList } = this.props
    let nowList = [];

    // changeList.length === 0 ? 
    nowList = this.props.buildingList.slice()
    // nowList = changeList.slice()
    
    return (
      <Spin spinning={isLoading}>
          <div className='buildingChangePage'>
              <Search  className='search' size='large' 
                       onSearch={value => this.search(value)} 
                       onPressEnter={this.pressEnterSearch}/>
              {
                nowList.filter((v) => {
                  return v.examineStatus === 8
                }).map((v, i) => 
                  <div className='list' key={i} onClick={() => this.handleClick(v, i)}>
                      <div className='listDiv'>
                          <p>{v.basicInfo.name}</p>
                          { 
                            i === this.props.nowInfoIndex ? 
                            <p><Icon type="check" className='icon'/></p> 
                            : null 
                          }
                      </div>
                  </div>
                )
              }
          </div>
      </Spin>
    )
  }
}


function apptableMapStateToProps(state) {
  return {
    opreation: state.shop.opreation,
    isLoading: state.shop.isLoading,
    operInfo: state.shop.operInfo,
    searchList: state.index.searchList,
    changeList: state.shop.changeList,
    nowInfoIndex: state.index.nowInfoIndex,
  }
}

function apptableMapDispatchToProps(dispatch) {
  return {
    dispatch,
    changeNowListInfo: (...args) => dispatch(changeNowListInfo(...args)),
    searchMyBuildingList: (...args) => dispatch(searchMyBuildingList(...args)),
    getMyCustomerInfo: (...args) => dispatch(getMyCustomerInfo(...args)),
    getThisBuilding: (...args) => dispatch(getThisBuilding(...args)),
    statusCount: (...args) => dispatch(statusCount(...args)),
  };
}
export default connect(apptableMapStateToProps, apptableMapDispatchToProps)(BuildingChangeList);