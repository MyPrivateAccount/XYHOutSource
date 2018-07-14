//成交报告综合查询主页面
import React, { Component } from 'react';
import { Row, Col, notification, Modal } from 'antd';
import { connect } from 'react-redux';
import DRpSearchCondition from './dRpSearchCondition'
import YjfpTable from './yjfpTable'
import Layer, { LayerRouter } from '../../../components/Layer'
import { Route } from 'react-router'
import { getDicParList } from '../../../actions/actionCreators'
import { dicKeys} from '../../constants/const'
import WebApiConfig from '../../constants/webApiConfig'
import ApiClient from '../../../utils/apiClient'
import { convertReport } from '../../constants/utils'
import TradeManager from './rpdetails/tradeManager'


class YjfpQuery extends Component {
    state = {
        list: [],
        loading: false,
        pagination: { pageSize: 15, pageIndex: 1, total: 0 },
        condition: {},
        permission: {},
        wyItems:[],
        nyItems:[]
    }

    componentDidMount = () => {
        this.props.getDicParList([
            dicKeys.jylx,
        ]);
        this._getFtItems();
    }

    _getFtItems = async () => {

        let url = `${WebApiConfig.baseset.acmentlistget}${this.props.user.Filiale}`
        let r = await ApiClient.get(url);
        r = (r || {}).data || {};
        if (r.code === '0') {
            let list = r.extension || [];
            let wyItems = [], nyItems = [];
            list.forEach(item => {
                if (item.type === 1) {
                    wyItems.push(item)
                } else if (item.type === 2) {
                    nyItems.push(item)
                }
            })
            this.setState({ wyItems: wyItems, nyItems: nyItems })
        } else {
            notification.error({ message: '获取分摊项列表失败' })
        }
    }


    search = (condition) => {
        this.setState({ condition: condition || {}, pagination: { ...this.state.pagination, pageIndex: 1 } }, () => {
            this._search();
        })
    }

    _search = async () => {
        let { condition } = this.state;

        condition.pageSize = this.state.pagination.pageSize;
        condition.pageIndex = this.state.pagination.pageIndex;
        this.setState({ loading: true })
        try {
            let url = `${WebApiConfig.rp.searchYjfp}`;
            let r = await ApiClient.post(url, condition);
            r = (r || {}).data;
            if (r.code === '0') {
                this.setState({ list: r.extension, pagination: { ...this.state.pagination, total: r.totalCount } });
            } else {
                notification.error({ message: '查询成交报告列表失败' });
            }
        } catch (e) {
            notification.error({ message: '查询成交报告列表失败' })
        }
        this.setState({ loading: false })
    }

    pageChanged = (pagination, filters, sorter) => {
        this.setState({
            pagination: { ...this.state.pagination, ...{ pageIndex: pagination.current } }
        }, () => {
            this._search();
        })
    }

    _getReport = async (report) => {
        if (!report || !report.id)
            return;


        try {
            let url = `${WebApiConfig.rp.rpGet}${report.id}`;
            let r = await ApiClient.get(url);
            r = (r || {}).data;
            if (r.code === '0') {
                if (r.extension) {

                    let report = convertReport(r.extension);
                    return report;
                } else {
                    notification.error({ message: '成交报告不存在' });
                }
            } else {
                notification.error({ message: '获取成交报告详情失败', description: r.message || '' });
            }
        } catch (e) {
            notification.error({ message: '获取成交报告详情失败' })
        }
        return null;

    }


    viewReport = async (report, action) => {
        if (!report || !report.id)
            return;

        this.setState({ loading: true })
        try {
            let rp = await this._getReport(report);
            let op = 'view'
            this.props.history.push(`${this.props.match.url}/reportInfo`, { entity: rp, op: op, pagePar: this.state.pagePar })

        } catch (e) {
            notification.error({ message: '获取成交报告详情失败' })
        }
        this.setState({ loading: false })
    }

   

    render() {
        return (
            <Layer className="content-page">
                <Row>
                    <Col span={24}>
                        <DRpSearchCondition
                            export={this.export}
                            search={this.search} />
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <YjfpTable
                            loading={this.state.loading}
                            viewReport={this.viewReport}
                            dic={this.props.dic}
                            dataSource={this.state.list}
                            pagination={this.state.pagination}
                            pageChanged={this.pageChanged}
                            reportChanged={this.reportChanged}
                            nyItems = {this.state.nyItems}
                            type={'query'} />
                    </Col>
                </Row>



                <LayerRouter>
                    <Route path={`${this.props.match.url}/reportInfo`} render={(props) => <TradeManager {...props} reportChanged={this.reportChanged} />} />
                </LayerRouter>
            </Layer>
        )
    }
}

function MapStateToProps(state) {

    return {
        dic: state.basicData.dicList,
        user: state.oidc.user.profile || {}
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch,
        getDicParList: (...args) => dispatch(getDicParList(...args))
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(YjfpQuery);
