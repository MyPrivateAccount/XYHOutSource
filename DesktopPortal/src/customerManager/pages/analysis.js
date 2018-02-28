import { connect } from 'react-redux';
import { openCustomerDetail } from '../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Row, Col, DatePicker, Table } from 'antd'
import echarts from 'echarts'
import SearchBox from './searchBox'
import './search.less'

const analysisStyle = {
    pieChart: {
        width: '500px',
        height: '350px',
        border: '1px solid #ccc'
    },
    rangeDate: {
        marginBottom: '20px',
        marginLeft: '3px'
    },
    table: {
        marginTop: '10px'
    }
}

class Analysis extends Component {
    state = {
        pieChart: {},
        activePie: {},//当前选中的pie块
    }
    componentDidMount() {
        // 基于准备好的dom，初始化echarts实例
        var pieChart = echarts.init(document.getElementById('tradePlanningPieChart'));
        // 绘制图表
        let dataSource = {
            legendData: ['餐饮', '零售', '生活配套', '休闲娱乐', '培训'],
            seriesData: [
                { value: 335, name: '餐饮' },
                { value: 310, name: '零售' },
                { value: 234, name: '生活配套' },
                { value: 135, name: '休闲娱乐' },
                { value: 1548, name: '培训' }
            ]
        }
        pieChart.setOption(this.getChartOption(dataSource));
        pieChart.on("click", (e) => this.handlePieClick(e));
        this.setState({ pieChart: pieChart });
    }
    //设置echarts数据源
    getChartOption(dataSource) {
        let { legendData, seriesData } = dataSource;
        legendData = legendData || [];
        seriesData = seriesData || [];
        let option = {
            title: {
                text: '业态分析',
                x: 'center'
            },
            tooltip: {
                trigger: 'item',
                formatter: "{a} <br/>{b} : {c} ({d}%)"
            },
            legend: {
                orient: 'vertical',
                left: 'right',
                data: legendData
            },
            series: [
                {
                    name: '业态规划',
                    type: 'pie',
                    radius: '55%',
                    center: ['50%', '60%'],
                    selectedMode: 'single',
                    data: seriesData,
                    itemStyle: {
                        emphasis: {
                            shadowBlur: 10,
                            shadowOffsetX: 0,
                            shadowColor: 'rgba(0, 0, 0, 0.5)'
                        }
                    }, pieSelect: (e) => { console.log("pie选中:", e); }
                }
            ]
        };
        return option;
    }
    handlePieClick = (e) => {
        console.log("pie click", e);
        let activePie = { ...this.state.activePie };
        if (e.data.selected) {
            activePie = { dataIndex: e.dataIndex, percent: e.percent, ...e.data };
            alert(`选中:${activePie.name}`);
        } else {
            activePie = {};
        }
        this.setState({ activePie: activePie });
    }

    render() {
        let tableColumns = [
            {
                title: '业态规划',
                dataIndex: 'tradPlanning',
                key: 'name',
            }, {
                title: '经营品牌',
                dataIndex: 'name1',
                key: 'name1',
            }, {
                title: '战略合作',
                dataIndex: 'name2',
                key: 'name2',
            }, {
                title: '归属部门',
                dataIndex: 'name3',
                key: 'name3',
            }, {
                title: '业务员',
                dataIndex: 'name4',
                key: 'name4',
            }, {
                title: '录入时间',
                dataIndex: 'name5',
                key: 'name5',
            }
        ];
        let dataSource = [];

        return (
            <div>
                <SearchBox />
                <p style={analysisStyle.rangeDate} > 录入时间 < DatePicker size='default' />-<DatePicker size='default' /></p>
                <div id="tradePlanningPieChart" style={analysisStyle.pieChart}></div>
                <Table rowKey={record => record.id} dataSource={dataSource} columns={tableColumns} bordered style={analysisStyle.table} />
            </div >
        )
    }

}

function mapStateToProps(state) {
    return {
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(Analysis);