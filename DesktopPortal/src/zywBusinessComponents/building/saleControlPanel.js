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
        shopSaleDic: [],
        colors: [],
    }
    componentWillMount() {

    }
    componentDidMount() {
        this.getBasicData(this.props);
    }
    getBasicData(newProps) {
        let {shopSaleStatus} = this.state, colors = [];
        let shopSaleDic = [];
        let findShopSaleStatus = (this.props.basicData || []).find(d => d.groupId === 'ZYW_SHOP_SALE_STATUS');
        if (findShopSaleStatus) {
            shopSaleDic = findShopSaleStatus.dicPars;
        }
        if (shopSaleDic && shopSaleDic.length > 0) {
            shopSaleDic.map(status => {
                shopSaleStatus.push(status.key);
                colors.push(status.ext1);
            });
            console.log("colors", colors);
            this.setState({shopSaleStatus: shopSaleStatus, colors: colors, shopSaleDic: shopSaleDic}, () => this.initEcharts());
        }
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
        let {shopSaleStatus} = this.state;
        if (this.props.basicData && shopSaleStatus.length === 0) {
            this.getBasicData(newProps);
        } else {
            let pieChart = this.state.pieChart;
            let dataSource = this.getPieChartData();
            pieChart.setOption(this.getChartOption(dataSource));
        }
    }
    getPieChartData() {
        let buildingShops = this.props.buildingShops;
        let seriesData = [];
        this.state.shopSaleDic.map(dic => {
            // let dicCode = this.props.basicData.shopSaleStatus.find(statusDic => statusDic.key === status).value;
            let result = buildingShops.filter(shop => shop.saleStatus === dic.value);
            seriesData.push({name: dic.key, value: result.length});
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
        let {shopSaleDic, color} = this.state
        // const shopSaleStatus = this.props.basicData.shopSaleStatus || [];
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
            let dic = shopSaleDic.find(s => s.value === v.saleStatus);
            if (dic) {
                v.backgroundColor = dic.ext1;
            }
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
                                                                <p>{this.formatPrice(child.price) + '元/m²/月'} </p>
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

export default SaleControlPanel;