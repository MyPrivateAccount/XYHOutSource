//成交报告综合查询主页面
import React, { Component } from 'react';
import { Row, Col, notification } from 'antd';
import { connect } from 'react-redux';
import DRpSearchCondition from './dRpSearchCondition'
import DealRpTable from './dealRpTable'
import Layer, { LayerRouter } from '../../../components/Layer'
import { Route } from 'react-router'
import { getDicParList } from '../../../actions/actionCreators'
import { dicKeys, permission , reportOperateAction} from '../../constants/const'
import WebApiConfig from '../../constants/webApiConfig'
import ApiClient from '../../../utils/apiClient'
import { convertReport } from '../../constants/utils'
import TradeManager from './rpdetails/tradeManager'
import TyPage from './ty/tyPage'
import DRpDlg from './dRpDlg'
import ZYComponet from './zYComponet'
import TYComponet from './tYComponet'

class DealRpQuery extends Component {
    state = {
        list: [],
        loading: false,
        pagination: { pageSize: 1, pageIndex: 1, total: 0 },
        condition: {},
        permission:{}
    }

    componentDidMount = () => {
        this.props.getDicParList([
            dicKeys.jylx
        ]);
        this.getPermission();
        this.search();
    }

    getPermission = async ()=>{
        let url = `${WebApiConfig.privilege.Check}`
        let r = await ApiClient.post(url, [
            permission.op_zf, 
            permission.op_sk, 
            permission.op_fk, 
            permission.op_ty, 
            permission.op_zy, 
            permission.op_jy])
        if( r && r.data && r.data.code==='0'){
            let p = {};
            (r.data.extension||[]).forEach(pi=>{
                if(pi.permissionItem===permission.op_zf){
                    p.zf = pi.isHave;
                }else if(pi.permissionItem===permission.op_sk){
                    p.sk = pi.isHave;
                }else if(pi.permissionItem===permission.op_fk){
                    p.fk = pi.isHave;
                }else if(pi.permissionItem===permission.op_ty){
                    p.ty = pi.isHave;
                }else if(pi.permissionItem===permission.op_zy){
                    p.zy = pi.isHave;
                }else if(pi.permissionItem===permission.op_jy){
                    p.jy = pi.isHave;
                }
            })
            this.setState({permission: p})
        }

    }


    search = (condition) => {
        this.setState({ condition: condition || {}, pagination: { ...this.state.pagination, pageIndex: 1 } }, () => {
            this._search();
        })
    }

    _search = async () => {
        let {condition} = this.state;

        condition.pageSize = this.state.pagination.pageSize;
        condition.pageIndex = this.state.pagination.pageIndex;
        this.setState({ loading: true })
        try {
            let url = `${WebApiConfig.rp.searchRp}`;
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

    viewReport = async (report, action) => {
        if (!report || !report.id)
            return;

        this.setState({ loading: true })
        try {
            let url = `${WebApiConfig.rp.rpGet}${report.id}`;
            let r = await ApiClient.get(url);
            r = (r || {}).data;
            if (r.code === '0') {
                if (r.extension) {

                    let report = convertReport(r.extension);
                    let op = 'view'
                    this.props.history.push(`${this.props.match.url}/reportInfo`, { entity: report, op: op, pagePar: this.state.pagePar })
                } else {
                    notification.error({ message: '成交报告不存在' });
                }
            } else {
                notification.error({ message: '获取成交报告详情失败', description: r.message || '' });
            }
        } catch (e) {
            notification.error({ message: '获取成交报告详情失败' })
        }
        this.setState({ loading: false })
    }

    reportChanged = (report, action) => {
        // if(action==='DEL'){
        //     let rl = this.state.list;
        //     let idx = rl.findIndex(r=>r.id === report.id);
        //     if(idx>=0){
        //         rl.splice(idx,1)
        //         this.setState({list: [...rl]})
        //     }
        // }else{
        //     let rl = this.state.list;
        //     let idx = rl.findIndex(r=>r.id === report.id);
        //     if(idx>=0){
        //         rl[idx] = {...rl[idx],...report}
        //         this.setState({list: [...rl]})
        //     }
        // }
    }

    operate = (action,report)=>{
        if(action.action === reportOperateAction.ty.action ){
            //调佣
            this._opTy(report);
        }
    }

    _opTy =async (report)=>{
        if (!report || !report.id)
            return;

        this.setState({ loading: true })
        try {
            let url = `${WebApiConfig.rp.rpDis}${report.id}`;
            let r = await ApiClient.get(url, true, {});
            r = (r || {}).data;
            if (r.code === '0') {
                if (r.extension) {
                    this.props.history.push(`${this.props.match.url}/ty`, { entity: r.extension, op: 'add', pagePar: this.state.pagePar })
                } else {
                    notification.error({ message: '业绩分配表不存在' });
                }
            } else {
                notification.error({ message: '获取业绩分配表详情失败', description: r.message || '' });
            }
        } catch (e) {
            notification.error({ message: '获取业绩分配表详情失败，'+(e.message||'') })
        }
        this.setState({ loading: false })
    }

    render() {
        return (
            <Layer className="content-page">
                <Row>
                    <Col span={24}>
                        <DRpSearchCondition 
                            export = {this.export}
                            search = {this.search}/>
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <DealRpTable
                            loading={this.state.loading}
                            viewReport={this.viewReport}
                            dic={this.props.dic}
                            dataSource={this.state.list}
                            pagination={this.state.pagination}
                            pageChanged={this.pageChanged}
                            reportChanged={this.reportChanged}
                            permission={this.state.permission}
                            operate = {this.operate}
                            type={'query'} />
                    </Col>
                </Row>
                {/* <DRpDlg onDlgSelf={this.onDlgSelf} />
                <ZYComponet onZYSelf={this.onZYSelf} />
                <TYComponet onTYSelf={this.onTYSelf} /> */}
                <LayerRouter>
                    <Route path={`${this.props.match.url}/reportInfo`} render={(props) => <TradeManager {...props} reportChanged={this.reportChanged} />} />
                    <Route path={`${this.props.match.url}/ty`} render={(props) => <TyPage {...props} />} />
                </LayerRouter>
            </Layer>
        )
    }
}

function MapStateToProps(state) {

    return {
        // operInfo: state.rp.operInfo,
        // rpOpenParam: state.rp.rpOpenParam,
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
export default connect(MapStateToProps, MapDispatchToProps)(DealRpQuery);
