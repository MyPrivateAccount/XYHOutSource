// import { connect } from 'react-redux';
// import { getCustomerDeal } from '../../../actions/actionCreator';
import React, {Component} from 'react'
import {Layout, Spin, Collapse, Popover} from 'antd'
import echarts from 'echarts'
import './xkCenter.less'
import moment from 'moment'

const {Header, Sider, Content} = Layout;
const Panel = Collapse.Panel;
const saleStatusStyle = {
    pieChart: {
        width: '40%',
        height: '300px',
    }
}
class SaleControlPanel extends Component {
    state = {
        pieChart: {},
        shopSaleStatus: [],
        colors: [],
    }
    componentWillMount() {
        // let shopSaleStatus = [], colors = [];
        // if (this.props.basicData.shopSaleStatus) {
        //     this.props.basicData.shopSaleStatus.map(status => {
        //         shopSaleStatus.push(status.key);
        //         colors.push(status.ext1);
        //     });
        // }
        // console.log("colors", colors);
        // this.setState({shopSaleStatus: shopSaleStatus, colors: colors});
        // this.props.getShopsSaleStatus({ saleStatusLi,st: [1, 2, 3, 10] })
    }
    componentDidMount() {
        let {shopSaleStatus} = this.state, colors = [];
        if (this.props.basicData.shopSaleStatus && shopSaleStatus.length === 0) {
            this.props.basicData.shopSaleStatus.map(status => {
                shopSaleStatus.push(status.key);
                colors.push(status.ext1);
            });
            console.log("colors", colors);
            this.setState({shopSaleStatus: shopSaleStatus, colors: colors}, () => this.initEcharts());
        }

        // // 基于准备好的dom，初始化echarts实例
        // var pieChart = echarts.init(document.getElementById('saleStatusPieChart'));
        // // 绘制图表
        // let dataSource = this.getPieChartData();
        // pieChart.setOption(this.getChartOption(dataSource));
        // this.setState({pieChart: pieChart});
    }
    initEcharts() {
        // 基于准备好的dom，初始化echarts实例
        var pieChart = echarts.init(document.getElementById('saleStatusPieChart'));
        // 绘制图表
        let dataSource = this.getPieChartData();
        pieChart.setOption(this.getChartOption(dataSource));
        this.setState({pieChart: pieChart});
    }
    componentWillReceiveProps(newProps) {

        let {shopSaleStatus} = this.state, colors = [];
        if (this.props.basicData.shopSaleStatus && shopSaleStatus.length === 0) {
            this.props.basicData.shopSaleStatus.map(status => {
                shopSaleStatus.push(status.key);
                colors.push(status.ext1);
            });
            console.log("colors", colors);
            this.setState({shopSaleStatus: shopSaleStatus, colors: colors}, () => this.initEcharts());
        } else {
            let pieChart = this.state.pieChart;
            let dataSource = this.getPieChartData();
            pieChart.setOption(this.getChartOption(dataSource));
        }
    }
    getPieChartData() {
        let buildingShops = this.props.buildingShops;
        let seriesData = [];
        this.state.shopSaleStatus.map(status => {
            let dicCode = this.props.basicData.shopSaleStatus.find(statusDic => statusDic.key === status).value;
            let result = buildingShops.filter(shop => shop.saleStatus === dicCode);
            seriesData.push({name: status, value: result.length});
        });
        let dataSource = {
            legendData: this.state.shopSaleStatus,
            seriesData: seriesData || []
        }
        return dataSource;
    }

    //设置echarts数据源
    getChartOption(dataSource) {
        let {legendData, seriesData} = dataSource;
        legendData = legendData || [];
        seriesData = seriesData || [];
        let option = {
            color: this.state.colors,//['#7AB1B7', '#78BB9B', '#D75E5C', '#D37E60'],
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
                    let value = seriesData.find(s => s.name === name).value;
                    return `${name} : ${value}户`;
                }
            },
            series: [
                {
                    name: '销售状态',
                    type: 'pie',
                    radius: '55%',
                    center: ['30%', '50%'],
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

    formatPrice = (p, c = 10000) => {
        return ((p * 1) / c).toFixed(0);
    }

    render() {
        const shopSaleStatus = this.props.basicData.shopSaleStatus || [];
        let buildingNo = [];
        let buildingShops = this.props.buildingShops || [];
        buildingShops.forEach((v) => {
            buildingNo.push({buildingNo: v.buildingNo})
        })
        let hash = {};
        buildingNo = buildingNo.reduce((item, next) => {  // 去重楼栋数
            hash[next.buildingNo] ? '' : hash[next.buildingNo] = true && item.push(next);
            return item
        }, [])
        buildingNo.map(v => v.children = [])
        buildingShops.forEach(v => {
            let index = buildingNo.findIndex(item => item.buildingNo === v.buildingNo)
            v.backgroundColor = shopSaleStatus.find(s => s.value === v.saleStatus).ext1;
            buildingNo[index].children.push(v)
        });
        //成交信息处理
        let content, customerDeal = this.props.customerDeal || [];
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
            <div>
                {/* <Spin spinning={this.props.showLoading}> */}
                <div className="relative">
                    <Layout>
                        <Content className='content xkCenterPage'>
                            <div id="saleStatusPieChart" style={saleStatusStyle.pieChart}></div>
                            {/**商铺**/}
                            <Collapse defaultActiveKey={['0']} className='kanbanStyle'>
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
                                                                <p>{this.formatPrice(child.price) + '万'} </p>
                                                                {/* {
                                                                        child.saleStatus !== "10" ? null :
                                                                            <Popover content={content} title="成交信息" trigger="click">
                                                                                <p style={{color: '#fff', cursor: 'pointer', fontWeight: 'bold'}}
                                                                                    onClick={() => this.props.dispatch(getCustomerDeal(child.id))}>查看成交信息</p>
                                                                            </Popover>
                                                                    } */}
                                                            </div>
                                                        )
                                                    }
                                                </div>
                                            </div>
                                        </Panel>
                                    )
                                }
                            </Collapse>
                        </Content>
                    </Layout>
                </div>
                {/* </Spin> */}
            </div>
        )
    }
}

/*function mapStateToProps(state) {
    // console.log('首页', state.index.todayReportListId);
    return {
        showLoading: state.search.showLoading,
        shopSaleStatus: state.basicData.shopSaleStatus,
        buildingShops: state.search.buildingShops,
        customerDeal: state.search.customerDeal
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SaleControlPanel);*/
export default SaleControlPanel;