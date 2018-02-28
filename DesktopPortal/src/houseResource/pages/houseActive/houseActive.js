import { connect } from 'react-redux';
import { getMyBuildingsListAsync,gotoProjectDetailPage,gotoShopDetailPage, getShopList,getBuilding} from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Input, Icon , Spin } from 'antd'
import './houseActive.less'

const Search = Input.Search;

class HouseActiveList extends Component {
  state = {
    myIndex: 0
  }
  componentDidMount() {
    // this.props.dispatch(getBuilding());
  }
  componentWillReceiveProps(newProps) {
    
  }

  getShopList = (v, i, e) => {
    e.stopPropagation()
    this.props.dispatch(getShopList({
      value: v,
      index: i,
    }))
    console.log(v, '点击图标')
  }

  

  


  render() {
    const { dynamicLoading, dynamicData } = this.props
    return ( 
      <Spin spinning={dynamicLoading}>
      <div className="activeList">
          {
                (dynamicData || []).map((item, index) => 
                    <div className='listBox' key={index}>
                        <div className='list_part'  style={{cursor: 'pointer'}} onClick={() => this.props.gotoProjectDetailPage(item.build.id)}>
                          <p className='buldingTitle'>
                            { item.build.basicInfo.name }
                          </p>
                          <Icon onClick={(e) => this.getShopList(item, index, e)} style={{fontWeight:'bold'}}
                                type={item.isChecked ? 'up' : 'down'} 
                                style={{padding: '5px 3rem'}} />
                        </div>
                      {  
                        item.shops ? 
                        item.shops.length !== 0 ?
                            <div>
                              {
                                item.shops.map((v, i) => {
                                    return (
                                      <div className='list_part list_shops' key={i}  style={{cursor: 'pointer'}} onClick={() => this.props.gotoShopDetailPage(v.id)}>
                                          <div>
                                            {v.buildingNo}-{v.floorNo}-{v.number} 
                                          </div>
                                      </div>
                                    )
                                  }) 
                              }
                            </div>  : <div style={{padding: '10px 0', textAlign:'center', borderTop: '1px solid #dcdcdc'}}>暂无数据</div> 
                        : 
                        null
                        // this.props.loadingData ?
                        //  <div  style={{padding: '10px 0 ', textAlign: 'center'}}>
                        //    <Button shape="circle" loading={this.props.loadingData} size='small'/>数据加载中 
                        //  </div>
                        // : null
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
  return {
    dynamicLoading: state.active.dynamicLoading,
    dynamicData: state.active.dynamicData,
    loadingData: state.active.loadingData
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch,
    gotoProjectDetailPage: (...args) => dispatch(gotoProjectDetailPage(...args)),
    gotoShopDetailPage: (...args) => dispatch(gotoShopDetailPage(...args)),
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(HouseActiveList);