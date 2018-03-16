import { connect } from 'react-redux';
import { getSalestatistics,getDicParList } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Row, Col, Tabs, Radio, Input, Menu, Select,Spin,Modal,Pagination } from 'antd'
import moment from 'moment'
import echarts from 'echarts'
import List from './list'
import Kanban from './kanban'
import './xkCenter.less'

const { Header, Sider, Content } = Layout;
const TabPane = Tabs.TabPane;
const SubMenu = Menu.SubMenu;
const MenuItemGroup = Menu.ItemGroup;
const Option = Select.Option;
const saleStatusStyle = {
  pieChart: {
      width: '45%',
      height: '300px',
  },
  mt_n: {
      marginTop: '20px',
  }
}
class XkCenter extends Component {
  state = {
    current: 'xkd',
    buildingId: '',
    colors: [],
  }
  componentDidMount() {
    // console.log(this.props.seriesData, 'ccc')
    // if (this.props.seriesData.length !== 0) {
    //     this.loadData(this.props.shopInfoList)
    // }
    // if (Object.keys(this.props.nowInfo).length === 0) {
    //   console.log(1,  this.props.buildingInfo)
      this.setState({
        buildingId: this.props.buildingInfo.id
      }, () => {
        this.props.getSalestatistics({ buildingId: this.state.buildingId })
      })
    // } 
    // else {
    //   console.log(2)
    //   this.setState({
    //   }, () => {
    //     this.props.getSalestatistics({ buildingId: this.state.buildingId })
    //   })
    // }
      
  }
  componentWillReceiveProps(newProps) {
    // console.log(newProps.seriesData, 'aaa')
    // if (Object.keys(newProps.nowInfo).length === 0) {
        // console.log(1,  newProps.buildingInfo)
        if (this.state.buildingId !== newProps.buildingInfo.id) {
            // console.log('???不一样不一样')
            this.props.getSalestatistics({ buildingId: newProps.buildingInfo.id })
            this.setState({buildingId: newProps.buildingInfo.id})
        }
    //   } 
    //   else {
    //     console.log(33,  newProps.nowInfo)
    //     if (this.state.buildingId !== newProps.nowInfo.id) {
    //         this.props.getSalestatistics({ buildingId: newProps.nowInfo.id })
    //         this.setState({buildingId: newProps.nowInfo.id})
    //     }
    //   }

        if (this.props.seriesData.length !== 0) {
            // console.log('cccadasfe', newProps.shopInfoList)
            this.loadData(newProps.shopInfoList)
        }
  }

  loadData = (list) => {
    let shopSaleStatus = this.props.basicData.shopSaleStatus ||  []
    let data = this.props.seriesData || []

    data.forEach((v, i) => {
        switch (v.name) {
            case '10': v.name = '已售', v.color = shopSaleStatus[1].ext1;break;
            case '3': v.name = '锁定', v.color = shopSaleStatus[3].ext1;break;
            case '2': v.name = '在售', v.color = shopSaleStatus[2].ext1;break;
            case '1': v.name = '待售', v.color = shopSaleStatus[0].ext1;break;
            // default : v.name = '其它', v.color = '#76B9ED';
        }
    })
    let legendData = []
    data.map(v => {
        legendData.push({ name: v.name, value: v.value })
    })
    let colorData = data.map(v => {
        return v.color
    })

    this.setState({colors:colorData})
    // 基于准备好的dom，初始化echarts实例
    var pieChart = echarts.init(document.getElementById('saleStatusPieChart'));
    // 绘制图表
    let dataSource = {
        legendData: legendData, //['已售', '锁定', '在售', '待售'], // 10, 3, 2, 1
        seriesData: data
    }
    pieChart.setOption(this.getChartOption(dataSource));
  }

  //设置echarts数据源
  getChartOption(dataSource) {
      let { legendData, seriesData } = dataSource;
      legendData = legendData || [];
      seriesData = seriesData || [];
      let option = {
          color: this.state.colors,//['#D37E60', '#D75E5C', '#78BB9B', '#7AB1B7'],
          tooltip: {
              trigger: 'item',
              formatter: "{a} <br/>{b} : {c} ({d}%)"
          },
          legend: {
              orient: 'vertical',
              left: 'right',
              top: 70,
              data: legendData,
              formatter: function (name) {
                let value = legendData.find(s => s.name === name).value;
                return `${name} : ${value}户`;
              }
          },
          series: [
              {
                  name: '销售状态',
                  type: 'pie',
                  radius: '55%',
                  center: ['45%', '50%'],
                  selectedMode: 'single',
                  data: seriesData,
                  itemStyle: {
                      emphasis: {
                          shadowBlur: 10,
                          shadowOffsetX: 0,
                          shadowColor: 'rgba(0, 0, 0, 0.5)'
                      }
                  }
              }
          ]
      };
      return option;
  }

  callback = (key)  => {
  }

  render() {
    return ( 
      <Spin spinning={this.props.isLoading}>
      <div className="relative">
          <Layout>
              <Content className='content xkCenterPage'>
              {
                  this.props.seriesData.length === 0 ? null : <div id="saleStatusPieChart" style={saleStatusStyle.pieChart}></div>
              }
                  <Tabs defaultActiveKey="1" onChange={this.callback} type="card" style={this.props.seriesData.length === 0 ? saleStatusStyle.mt_n : null}>
                    <TabPane tab="销控单" key="1">
                        <List/>
                    </TabPane>
                    <TabPane tab="销控看板" key="2">
                        <Kanban/>
                    </TabPane>
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
    basicData: state.basicData,
    isLoading: state.center.isLoading,
    shopInfoList: state.center.shopInfoList,
    kanBanList: state.center.kanBanList,
    seriesData: state.center.seriesData,
    nowInfo: state.index.nowInfo, // 切换后的楼盘信息
    buildingInfo: state.index.buildingInfo,
  }
}

function mapDispatchToProps(dispatch) {
  return {
    getSalestatistics: (...args) => dispatch(getSalestatistics(...args)),
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(XkCenter);