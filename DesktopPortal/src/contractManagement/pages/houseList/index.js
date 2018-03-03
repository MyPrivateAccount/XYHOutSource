import { connect } from 'react-redux';
import { getBuildingsListAsync, getShopsListAsync, searchMyBuildingList,
        getMyBuildingsListAsync,  gotoBuildPage, gotoShopPage,
        gotoThisBuild, gotoThisShop, deleteShop, gotoAddShop,lookMore,changeShowGroup,
        getAddBuilding, deleteBuilding, geAddtShopList} from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Row, Col, Tabs, Radio, Input, Icon, Tooltip ,
  Popconfirm, Spin } from 'antd'

const { Header, Sider, Content } = Layout;
const Search = Input.Search;

class HouseList extends Component {
  state = {
  }
  componentWillMount() {
    // this.props.dispatch(getAddBuilding());
  }
  gotoAddBuild = () => {
    this.props.gotoBuildPage();
    this.props.dispatch(changeShowGroup({ type: 2 }))
  }
  lookMore = (v, index) => {
    this.props.lookMore({pageSize: 100000, buildingId: [v.id], index: index});
  }
  search = (value) => {
    this.props.searchMyBuildingList({value: value, type: 2}) // 2 表示现在房源得搜索
  }
  pressEnterSearch = (e) => {
    let value = e.target.value
    this.props.searchMyBuildingList({value: value, type: 2})
  }
  status = (status) => {
    switch (status) {
      case 0: return '未提交';break;
      case 1: return '审核中';break;
      case 16: return '审核驳回';break;
      default: return '审核通过';
    }
  }

  getShopList = (v, i, e) => {
    e.stopPropagation()
    this.props.dispatch(geAddtShopList({
      value: v,
      index: i,
    }))
    // console.log(v, '点击图标')
  }


  render() {
      let { isLoading, operInfo, myBuildingList } = this.props
      return (
        <Spin spinning={isLoading}>
          <div className='mycontent'>
           
            <div className='searchBox'>
              <Search placeholder="搜索" className='searchInput' size='large'
               onSearch={value => this.search(value)} 
               onPressEnter={this.pressEnterSearch}/>
              <Tooltip title="新增楼盘">
                <Icon type="plus-circle-o" style={{color: '#555555',fontSize: '1.5rem', cursor: 'pointer',fontWeight:'bold'}} onClick={this.gotoAddBuild}/>
              </Tooltip>
            </div>
                { // 楼盘显示所有状态的楼盘（只要提交的楼盘就可以新增商铺）
                  (myBuildingList || []).map((item, index) => 
                    <div className='listBox' key={index}>
                      <div className='list_part'>
                        <p className='buldingTitle' style={{cursor: 'pointer'}} onClick={() => this.props.gotoThisBuild(item.build)}>
                          {
                           item.build.basicInfo.name !== null ? item.build.basicInfo.name : '未命名'
                          }

                          <span className={this.status(item.build.examineStatus) === '审核通过' ? 'submit':  'noSubmit'}>
                                {
                                  this.status(item.build.examineStatus)
                                }
                          </span>
                        </p>
                       
                        <p style={{display: 'flex', alignItems:'center', justifyContent: 'space-between'}}>
                        <Icon onClick={(e) => this.getShopList(item, index, e)} style={{fontWeight:'bold'}}
                                type={item.isChecked ? 'up' : 'down'} 
                                style={{padding: '.2rem 1rem', cursor: 'pointer'}} />
                        { 
                          
                          [1, 8].includes(item.build.examineStatus) ? // 楼盘状态审核中和审核通过。可以新增商铺，不能删除
                          <p className='addShopsIcon'  style={{cursor: 'pointer',padding: '.2rem 1rem'}} onClick={() => this.props.gotoAddShop(item.build)}>
                            <Tooltip title="新增商铺">
                              <Icon type="plus"/>
                            </Tooltip>
                          </p> 
                          : // 楼盘状态驳回和未提交。可以删除楼盘，不能新增商铺
                          <Popconfirm title="你确定要删除此楼盘吗？"
                                      onConfirm={() => this.props.dispatch(deleteBuilding({body: item.build, buidingIndex: index}))} 
                                      okText="是" cancelText="否">
                                        <p className='deleteShop' style={{cursor: 'pointer',padding: '.2rem 1rem'}}>
                                          <Icon type="delete" />
                                        </p>
                          </Popconfirm>
                        }
                        </p>
                      </div>
                      {  
                        item.shops ? 
                          item.shops.length !== 0 ?
                            <div>
                              {  // 商铺显示所有状态的 0 未提交 1 审核中 8 审核通过  16 审核驳回
                                item.shops.map((v, i) => {
                                    return (
                                      <div className='list_part list_shops' key={i}>
                                          <p style={{cursor: 'pointer'}} onClick={() => this.props.gotoThisShop({shopsInfo: v, build: item.build})}>
                                            {v.basicInfo.buildingNo}-{v.basicInfo.floorNo}-{v.basicInfo.number} 
                                            <span className={this.status(v.examineStatus) === '审核通过' ? 'submit':  'noSubmit'}>
                                            {this.status(v.examineStatus)}
                                            </span>
                                          </p>
                                          { // 只有状态为未提交才能删除商铺
                                           [0, 16].includes(v.examineStatus) ? null : 
                                          <Popconfirm title="你确定要删除此商铺吗？"
                                                      onConfirm={() => this.props.deleteShop({body: v, shopIndex: i, buildingIndex: index})} 
                                                      okText="是" cancelText="否">
                                                        <p className='deleteShop'>
                                                          <Icon type="delete" />
                                                        </p>
                                          </Popconfirm>
                                          }
                                      </div>
                                    )
                                  })
                            }
                        
                          </div>
                            : <div style={{padding: '10px 0', textAlign:'center', borderTop: '1px solid #dcdcdc'}}>暂无数据</div>  : null
                          }
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
    myBuildingList: state.shop.myBuildingList,
    opreation: state.shop.opreation,
    isLoading: state.shop.isLoading,
    operInfo: state.shop.operInfo,
    
  }
}

function apptableMapDispatchToProps(dispatch) {
  return {
    dispatch,
    getMyBuildingList: (...args) => dispatch(getMyBuildingsListAsync(...args)),
    gotoBuildPage: () => dispatch(gotoBuildPage()),
    gotoAddShop: (...args) => dispatch(gotoAddShop(...args)),
    gotoThisShop: (...args) => dispatch(gotoThisShop(...args)),
    gotoThisBuild: (...args) => dispatch(gotoThisBuild(...args)),
    deleteShop: (...args) => dispatch(deleteShop(...args)),
    lookMore: (...args) => dispatch(lookMore(...args)),
    searchMyBuildingList: (...args) => dispatch(searchMyBuildingList(...args)),
  };
}
export default connect(apptableMapStateToProps, apptableMapDispatchToProps)(HouseList);