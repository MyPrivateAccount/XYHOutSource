import { connect } from 'react-redux';
import { getShopsSaleStatus, getCustomerDeal } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Spin,Icon,Collapse,Popover  } from 'antd'
import moment from 'moment'
import echarts from 'echarts'

const Panel = Collapse.Panel;

class Kanban extends Component {
  state = {
    buildingId: '',
  }
  componentWillReceiveProps(newProps) {
    if (Object.keys(newProps.nowInfo).length === 0) {
      if (this.state.buildingId !== newProps.buildingInfo.id) {
        this.props.getShopsSaleStatus({ saleStatus: [], buildingId: [newProps.buildingInfo.id], who: 'kanban'})
        this.setState({buildingId: newProps.buildingInfo.id})
      }
    } else {
      if (this.state.buildingId !== newProps.nowInfo.id) {
        this.props.getShopsSaleStatus({ saleStatus: [], buildingId: [newProps.nowInfo.id], who: 'kanban'})
        this.setState({buildingId: newProps.nowInfo.id})
      }
    }
  }
  componentWillMount() {
    if (Object.keys(this.props.nowInfo).length === 0) {
      this.setState({
        buildingId: this.props.buildingInfo.id
      }, () => {
        this.props.getShopsSaleStatus({ saleStatus: [], buildingId: [this.state.buildingId], who: 'kanban'})
      })
    } else {
      this.setState({
        buildingId: this.props.nowInfo.id
      }, () => {
        this.props.getShopsSaleStatus({ saleStatus: [], buildingId: [this.state.buildingId], who: 'kanban'})
      })
    }
  }
  callback = (key) => {
    console.log(key);
  }
  formatPrice = (p, c = 10000) => {
    return ((p * 1) / c).toFixed(0);
  }

  render() {
    let { nowInfo, buildingInfo, kanBanList, customerDeal } = this.props
    let buildingNo = []
    kanBanList.forEach((v) => {
      buildingNo.push({ buildingNo: v.buildingNo })
      v.backgroundColor = ''
    })
    let hash = {};
    buildingNo = buildingNo.reduce((item, next) => {  // 去重楼栋数
      hash[next.buildingNo] ? '' : hash[next.buildingNo] = true && item.push(next);
      return item
    }, [])
    buildingNo.map(v => v.children = [])
    let shopSaleStatus = this.props.basicData.shopSaleStatus ||  []
    kanBanList.forEach(v => {
      let index = buildingNo.findIndex(item => item.buildingNo === v.buildingNo)
      switch(v.saleStatus) {
        case '1': v.backgroundColor = shopSaleStatus[0].ext1;break;
        case '2': v.backgroundColor = shopSaleStatus[2].ext1;break;
        case '3': v.backgroundColor = shopSaleStatus[3].ext1;break;
        default: v.backgroundColor = shopSaleStatus[1].ext1;
      }
      buildingNo[index].children.push(v)
    })
    let content;
    if (customerDeal.length === 0) {
      content = (
        <div>暂无成交信息</div>
      )
    } else {
      content = (
        <div className='customerDealContent'>
          {
              customerDeal[0].sellerType === 1 ? 
              <p>
              <p>楼盘名： <span>{customerDeal[0].buildingName}</span></p>
              <p>商铺： <span>{customerDeal[0].shopName}</span></p>
              <p>销售类型： <span>自售</span></p>
              <p>组别： <span>{customerDeal[0].departmentName}</span></p>
              <p>业务员： <span>{customerDeal[0].userName}</span></p>
              <p>业务员电话：<span>{customerDeal[0].userPhone}</span></p>
              <p>客户： <span>{customerDeal[0].customerName}</span></p>
              <p>客户电话： <span>{customerDeal[0].customerPhone}</span></p>
              <p>佣金： <span>{customerDeal[0].commission}</span></p>
              <p>成交总价： <span>{customerDeal[0].totalPrice}</span></p>
              <p>成交日期： <span>{moment(customerDeal[0].createTime).format('YYYY-MM-DD')}</span></p>
              </p>
              : 
              <p>
              <p>楼盘名： <span>{customerDeal[0].buildingName}</span></p>
              <p>商铺： <span>{customerDeal[0].shopName}</span></p>
              <p>销售类型： <span>第三方</span></p>
              <p>分销商： <span>{customerDeal[0].seller}</span></p>
              <p>客户： <span>{customerDeal[0].proprietor}</span></p>
              <p>客户电话： <span>{customerDeal[0].mobile}</span></p>
              <p>身份证： <span>{customerDeal[0].idcard}</span></p>
              <p>居住地： <span>{customerDeal[0].address}</span></p>
              <p>成交日期： <span>{moment(customerDeal[0].createTime).format('YYYY-MM-DD')}</span></p>
              </p>
          }
        </div>
      )
    }
    // console.log(buildingNo, '看板buildingNo')
    return ( 
      <div className='kanbanStyle'>
       <Collapse defaultActiveKey={['0']} onChange={this.callback}>
        {
          buildingNo.map((item, index) => 
            <Panel header={item.buildingNo} key={`${index}`}>
                <div className='kanbanItem'>
                  <div className='itemContent'>
                  {
                  item.children.map((child, i) => 
                        <div className='itemDiv' style={{backgroundColor: child.backgroundColor}} key={i}>
                          <p>{`${child.buildingNo}-${child.floorNo}-${child.number}`}</p>
                          <p>{child.buildingArea} m²</p>
                          <p>{this.formatPrice(child.price)+'万'} </p>
                          {
                            child.saleStatus !== "10" ? null : 
                          <Popover content={content} title="成交信息" trigger="click">
                            <p style={{color: '#fff', cursor: 'pointer', fontWeight: 'bold'}}
                            onClick={() => this.props.getCustomerDeal(child.id)}>查看成交信息</p>
                          </Popover>
                          }
                        </div>
                  )
                  }
                  </div>
                </div>
          </Panel>
          )
        }
       </Collapse>
      </div>
    )
  }
}

function mapStateToProps(state) {
  // console.log('首页', state.index.todayReportListId);
  return {
    basicData: state.basicData,
    kanBanList: state.center.kanBanList, // 看板list
    nowInfo: state.index.nowInfo, // 切换后的楼盘信息
    buildingInfo: state.index.buildingInfo, // 初始楼盘信息
    customerDeal: state.center.customerDeal, // 成交信息
  }
}

function mapDispatchToProps(dispatch) {
  return {
    getShopsSaleStatus: (...args) => dispatch(getShopsSaleStatus(...args)),
    getCustomerDeal: (...args) => dispatch(getCustomerDeal(...args)),
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(Kanban);