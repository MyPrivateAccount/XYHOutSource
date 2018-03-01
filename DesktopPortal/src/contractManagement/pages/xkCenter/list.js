import { connect } from 'react-redux';
import {  showTimeDialog, getShopsSaleStatus, showEditModal, getCustomerDeal,saveTimeAsync } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Checkbox, Row, Col, Tabs, Radio, Input, Menu, Select,Spin,Modal,Pagination ,Tag,Popover,Popconfirm} from 'antd'
import moment from 'moment'
import echarts from 'echarts'
import TimeDialog from './timeDialog'
import XyhDealDialog from '../dialog/xyhDealInfo'
import {TAG_COLOR} from '../../../constants/uiColor'

const TabPane = Tabs.TabPane;
const SubMenu = Menu.SubMenu;
const MenuItemGroup = Menu.ItemGroup;
const Option = Select.Option;
const ptKeys = [
  { label: '上水', name: 'upperWater', id: 'i1' },
  { label: '下水', name: 'downWater', id: 'i2' },
  { label: '天然气', name: 'gas', id: 'i3' },
  { label: '烟管道', name: 'chimney', id: 'i4' },
  { label: '排污管道', name: 'blowoff', id: 'i5' },
  { label: '可拆分', name: 'split', id: 'i6' },
  { label: '电梯', name: 'elevator', id: 'i7' },
  { label: '扶梯', name: 'staircase', id: 'i8' },
  { label: '外摆区', name: 'outside', id: 'i9' },
  { label: '架空层', name: 'openFloor', id: 'i10' },
  { label: '停车场', name: 'parkingSpace', id: 'i11' }
];

class List extends Component {
  state = {
    current: '0',
    pageIndex: 0,
    buildingId: '',
    indeterminate: true, // cheackBox 多选
    checkAll: false, // 是否全选
    checkedList: [], //勾选的数据集合id
    selectedStores: new Array(this.props.shopInfoList.length), // 勾选的数据集合布尔值，控制全选单选
  }
  componentWillReceiveProps(newProps) {
    if (Object.keys(newProps.nowInfo).length === 0) {
      if (this.state.buildingId !== newProps.buildingInfo.id) {
        this.setState({pageIndex: 0}, () => {
          this.props.getShopsSaleStatus({ 
          saleStatus: [], 
          buildingId: [newProps.buildingInfo.id],
          pageIndex: this.state.pageIndex,
          })
          this.setState({buildingId: newProps.buildingInfo.id})
        })
      }
    } else {
      if (this.state.buildingId !== newProps.nowInfo.id) {
        this.setState({pageIndex: 0}, () => {
          this.props.getShopsSaleStatus({ 
            saleStatus: [], 
            buildingId: [newProps.nowInfo.id],
            pageIndex: this.state.pageIndex,
          })
          this.setState({buildingId: newProps.nowInfo.id})
        })
      }
    }
  }

  componentWillMount() {
    if (Object.keys(this.props.nowInfo).length === 0) {
      this.setState({
        buildingId: this.props.buildingInfo.id
      }, () => {
        this.props.getShopsSaleStatus({ 
          saleStatus: [],
          buildingId: [this.state.buildingId],
          pageIndex: this.state.pageIndex,
        })
      })
    } else {
      this.setState({
        buildingId: this.props.nowInfo.id
      }, () => {
        this.props.getShopsSaleStatus({ 
          saleStatus: [], 
          buildingId: [this.state.buildingId],
          pageIndex: this.state.pageIndex,
        })
      })
    }
  }

  handleClick = (e) => { // 点击menu切换数据
    this.setState({
      current: e.key,
    }, () => {
      switch(this.state.current) {
        case '0': this.props.getShopsSaleStatus({
          saleStatus: [], 
          type:'current', 
          buildingId: [this.state.buildingId],
          pageIndex: this.state.pageIndex,
        });break;
        default: this.props.getShopsSaleStatus({
          saleStatus: [`${this.state.current}`], 
          type:'current', 
          buildingId: [this.state.buildingId],
          pageIndex: this.state.pageIndex,
        });
      }
    });
  }
  handlePageChange = (pageIndex, pageSize) => {  // 翻页
    this.setState({ pageIndex: (pageIndex - 1) }, () => {
        if (this.state.current === '0') {
            this.props.getShopsSaleStatus({
                 buildingId: [this.state.buildingId],
                 saleStatus: [],
                 pageIndex: this.state.pageIndex,
             }) 
         } else {
            this.props.getShopsSaleStatus({
                buildingId: [this.state.buildingId],
                saleStatus: [`${this.state.current}`],
                pageIndex: this.state.pageIndex,
            })
         }
    });
  }

  onCheckAllChange = (e) => { // 点击全选
    let selectedStores = new Array(this.props.shopInfoList.length)
    this.setState({ selectedStores: selectedStores }, () => {
      this.setState({ selectedStores: selectedStores.fill(e.target.checked) })
    })
    let shopInfoList = this.props.shopInfoList.map(v => {
      return v.id
    })
    this.setState({
      checkedList: e.target.checked ? shopInfoList : [],
      indeterminate: false,
      checkAll: e.target.checked
    });
  }

  onChange = (checked, id, index) => { // 点击单个选择框
    let selectedStores = this.state.selectedStores
    selectedStores[index] = !selectedStores[index]
    this.setState({ selectedStores: selectedStores })
    let checkedList = this.state.checkedList
    if (checked) {
      checkedList.push(id)
    } else {
      if (checkedList.indexOf(id) !== -1) {
        let index = checkedList.indexOf(id)
        checkedList.splice(index, 1)
      }
    }
    this.setState({
      checkedList, // Id集合
      indeterminate: !!checkedList.length && (checkedList.length < this.props.shopInfoList.length),
      checkAll: checkedList.length === this.props.shopInfoList.length,
    })
  }
  
  render() {
    const { nowInfo, buildingInfo, shopInfoList, listLoading, customerDeal } = this.props
    let basicInfo, name, icon;
    // if (Object.keys(nowInfo).length === 0) {
        basicInfo = buildingInfo.basicInfo || {}
        name = basicInfo.name || ''
        icon = basicInfo.icon
    // } else {
    //     basicInfo = nowInfo.basicInfo || {}
    //     name = basicInfo.name || ''
    //     icon = basicInfo.icon
    // }
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
    return ( 
     
      <div className='xkList'>
                  <Menu onClick={this.handleClick} selectedKeys={[this.state.current]} mode="horizontal" style={{marginBottom: '20px'}}>
                    <Menu.Item key="0">全部</Menu.Item>
                    <Menu.Item key="1">待售</Menu.Item>
                    <Menu.Item key="2">在售</Menu.Item>
                    <Menu.Item key="3">锁定</Menu.Item>
                    <Menu.Item key="10">已售</Menu.Item>
                  </Menu>
                <Spin spinning={listLoading}>
                  {
                    shopInfoList.map((item, index) => {
                      let tags = [];
                      ptKeys.forEach(v => {
                          if (item[v.name]) {
                              if (tags.length < 4) {
                                  tags.push(v);
                              }
                          }
                      });
                      return (
                        <Row  className='itemStyle' key={index}>
                          <Col span={this.state.current === "1" ? 1 : 0} style={{textAlign: 'center'}}>
                          {
                            this.state.current === "1" ? 
                            <Checkbox onChange={(e) => this.onChange(e.target.checked, item.id, index)} 
                                      checked={this.state.selectedStores[index]}/> 
                            : null
                          }
                          </Col>
                          <Col span={this.state.current === "1" ? 23 : 24}>
                              <Row className='borderDiv'>
                                  <Col span={20}>
                                  <div className='listLeft'>
                                      <div className='imgDiv'>
                                          <img src={ icon || '../../../images/default-icon.jpg' } />
                                      </div>
                                      <div className='listText'>
                                          <p>{`${item.buildingNo}-${item.floorNo}-${item.number}`} <span style={{marginLeft: '10px'}}>{name}</span></p>
                                          <p>{item.buildingArea} ㎡ ( {item.width} * {item.depth} * {item.height} )</p>
                                          <p>
                                            {
                                              tags.map(tag => {
                                                return <Tag color={TAG_COLOR} key={tag.name}>{tag.label}</Tag>
                                              })
                                            }
                                          </p>
                                      </div>
                                  </div>
                                  </Col>
                                  <Col span={4} className='priceDiv'>
                                      {/* <p className='redText'>{item.price ? (item.price/10000).toFixed(2) : '暂未定价'} 万元/m²</p> */}
                                      <p className='redText'>{item.price ? item.price : '暂未定价'} 元/m²</p>
                                      {
                                        //  1 待售  2 在售 3 锁定 10 已售
                                        (item.saleStatus === "2" || item.saleStatus === "3") ? 
                                        <div className='btnGroup'>
                                        {
                                          item.saleStatus === "2" ? 
                                          <Button type='primary' size='small' style={{marginRight: '10px'}} 
                                            onClick={() => this.props.showTimeDialog({
                                              id: item.id, 
                                              saleStatus: item.saleStatus, 
                                              current: this.state.current,
                                              buildingId: item.buildingId,
                                            })}>小定</Button>
                                          :
                                          <Popconfirm title="您确定要解锁吗？" okText="是" cancelText="否"
                                              onConfirm={() => this.props.save({
                                                            id: [item.id], 
                                                            saleStatus: item.saleStatus, 
                                                            current: this.state.current,
                                                            buildingId: item.buildingId
                                                          })} >
                                              <Button size='small' style={{marginRight: '10px'}} >解锁</Button>
                                          </Popconfirm>
                                        }
                                          <Button type='primary' size='small' style={{marginRight: '10px'}}
                                          onClick={() => this.props.showEditModal(item)}>确认成交</Button>
                                          {
                                            item.saleStatus === "2" ? 
                                            <Popconfirm title="您确定要暂停销售吗？" okText="是" cancelText="否"
                                            onConfirm={() => this.props.save({
                                                stop: true,
                                                id: [item.id], 
                                                saleStatus: item.saleStatus, 
                                                current: this.state.current,
                                                buildingId: item.buildingId,
                                              })} >
                                                <Button type='primary' size='small'>暂停销售</Button>
                                            </Popconfirm>
                                            : null
                                          }
                                          
                                        </div>
                                      : 
                                      <div className='btnGroup'>
                                      {
                                        item.saleStatus === "1" ? 
                                        <Popconfirm title="您确定要开始售卖吗？(为了及时通知业务员，建议在房源动态处加推商铺。)" okText="继续售卖" cancelText="取消"
                                                    onConfirm={() => this.props.save({
                                                        id: [item.id], 
                                                        saleStatus: item.saleStatus, 
                                                        current: this.state.current,
                                                        buildingId: item.buildingId,
                                                      })} >
                                            <Button type='primary' size='small'>开始售卖</Button>
                                        </Popconfirm>
                                        :
                                        <Popover content={content} title="成交信息" trigger="click">
                                            <Button type='primary' size='small' 
                                            onClick={() => this.props.getCustomerDeal(item.id)}>查看成交信息</Button>
                                        </Popover>
                                      }
                                      </div>
                                    }
                                  </Col>
                              </Row>
                          </Col>
                        </Row>
                  )})
                  }

                  {
                    this.state.current === '1'  && shopInfoList.length !== 0 ? 
                    <Row className='allDiv'>
                      <Col span={1} style={{textAlign: 'center'}}>
                        <Checkbox 
                         indeterminate={this.state.indeterminate}
                         onChange={this.onCheckAllChange}
                         checked={this.state.checkAll}>
                          全选
                        </Checkbox> 
                      </Col>
                      <Col span={23} style={{textAlign: 'right'}}>
                            <Popconfirm title="您确定要开始售卖吗？" okText="是" cancelText="否"
                                        onConfirm={() => this.props.save({
                                          id: this.state.checkedList, 
                                          saleStatus: '1', 
                                          current: '1',
                                          buildingId: this.state.buildingId
                                        })} >
                                <Button type='primary' disabled={this.state.checkedList.length === 0}>开始售卖</Button>
                            </Popconfirm>
                      </Col>
                    </Row>
                    : null
                  }
                  {
                    shopInfoList.length === 0 ? null :
                    <Pagination showQuickJumper style={{textAlign: 'right', margin: '15px 0'}}
                      current={this.state.pageIndex + 1}  
                      total={shopInfoList.totalCount} 
                      onChange={this.handlePageChange}/>
                  }

                </Spin>

              <TimeDialog />
              <XyhDealDialog />
      </div>
     
    )
  }
}

function mapStateToProps(state) {
  // console.log('字典数据', state.center.shopInfoList, state.index.buildingInfo);
  return {
    shopInfoList: state.center.shopInfoList,
    nowInfo: state.index.nowInfo, // 切换后的楼盘信息
    buildingInfo: state.index.buildingInfo,
    listLoading: state.center.listLoading,
    customerDeal: state.center.customerDeal,
    editTimeInfo: state.center.editTimeInfo
  }
}

function mapDispatchToProps(dispatch) {
  return {
    showTimeDialog: (...args) => dispatch(showTimeDialog(...args)),
    getShopsSaleStatus: (...args) => dispatch(getShopsSaleStatus(...args)),
    showEditModal: (...args) => dispatch(showEditModal(...args)),
    getCustomerDeal: (...args) => dispatch(getCustomerDeal(...args)),
    save: (...args) => dispatch(saveTimeAsync(...args)), //更新商铺状态
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(List);