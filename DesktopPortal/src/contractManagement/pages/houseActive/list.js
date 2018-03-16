import { connect } from 'react-redux';
import { getMyBuildingsListAsync,gotoProjectDetailPage,gotoShopDetailPage } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Input, Icon , Spin } from 'antd'
import './houseActive.less'

const Search = Input.Search;

class HouseActiveList extends Component {
  state = {
    myIndex: 0
  }
  componentDidMount() {
    this.props.getMyBuildingList({type: true});
  }
  componentWillReceiveProps(newProps) {
    
  }
  handleClick = () => {
    console.log(1)
  }
  status = (status) => {
    switch (status) {
      case 1: return '审核中';break;
      case 16: return '审核驳回';break;
    }
  }
  render() {
    let { isLoading, myBuildingList } = this.props

    return (
      <Spin spinning={isLoading}>
      <div className='activeList'>
          {
                  myBuildingList.filter((item) => { 
                    return [8].includes(item.examineStatus)
                  }).map((item, index) => 
                    <div className='listBox' key={index}>
                        <div className='list_part'  style={{cursor: 'pointer'}} onClick={() => this.props.gotoProjectDetailPage(item.id)}>
                          <p className='buldingTitle'>
                            { item.basicInfo.name }
                          </p>
                          <span style={{color: 'red', paddingLeft: '10px'}}>{this.status(item.examineStatus)}</span>
                        </div>
                      {  
                        item.children.length !== 0 ?
                            <div>
                              {
                                item.children.filter((v) => { 
                                  return [8].includes(v.examineStatus)
                                }).map((v, i) => {
                                    return (
                                      <div className='list_part list_shops' key={i}  style={{cursor: 'pointer'}} onClick={() => this.props.gotoShopDetailPage(v.id)}>
                                          <p>
                                            {v.basicInfo.buildingNo}-{v.basicInfo.floorNo}-{v.basicInfo.number} 
                                          </p>
                                          <span style={{color: 'red', paddingLeft: '10px'}}>{this.status(v.examineStatus)}</span>
                                      </div>
                                    )
                                  }) 
                              }
                            </div>
                            : null
                      }
                  </div>
                )
              }
      </div>
      </Spin>
    )
  }
}


function mapStateToProps(state) {
  // console.log('首页' + state.index);
  // console.log( state.shop.myBuildingList, '楼盘列表')
  return {
    myBuildingList: state.shop.myBuildingList,
    isLoading: state.shop.isLoading,
  }
}

function mapDispatchToProps(dispatch) {
  return {
    getMyBuildingList: (...args) => dispatch(getMyBuildingsListAsync(...args)),
    gotoProjectDetailPage: (...args) => dispatch(gotoProjectDetailPage(...args)),
    gotoShopDetailPage: (...args) => dispatch(gotoShopDetailPage(...args)),
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(HouseActiveList);