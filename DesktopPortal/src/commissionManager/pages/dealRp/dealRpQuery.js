//成交报告综合查询主页面
import React, { Component } from 'react';
import { Row, Col, notification, Modal } from 'antd';
import { connect } from 'react-redux';
import DRpSearchCondition from './dRpSearchCondition'
import DealRpTable from './dealRpTable'
import Layer, { LayerRouter } from '../../../components/Layer'
import { Route } from 'react-router'
import { getDicParList } from '../../../actions/actionCreators'
import { dicKeys, permission, reportOperateAction } from '../../constants/const'
import WebApiConfig from '../../constants/webApiConfig'
import ApiClient from '../../../utils/apiClient'
import { convertReport } from '../../constants/utils'
import TradeManager from './rpdetails/tradeManager'
import uuid from 'uuid'
import TyPage from './ty/tyPage'
import SKPage from './sfk/SKPage'
import FKPage from './sfk/FKPage'
import ZyPage from './zy/zyPage'
import JyPage from './jy/jyPage'

const confirm = Modal.confirm;

class DealRpQuery extends Component {
    state = {
        list: [],
        loading: false,
        pagination: { pageSize: 15, pageIndex: 1, total: 0 },
        condition: {},
        permission: {}
    }

    componentDidMount = () => {
        this.props.getDicParList([
            dicKeys.jylx,
        ]);
        this.getPermission();
        this.search();
    }

    getPermission = async () => {
        let url = `${WebApiConfig.privilege.Check}`
        let r = await ApiClient.post(url, [
            permission.op_zf,
            permission.op_sk,
            permission.op_fk,
            permission.op_ty,
            permission.op_zy,
            permission.op_jy])
        if (r && r.data && r.data.code === '0') {
            let p = {};
            (r.data.extension || []).forEach(pi => {
                if (pi.permissionItem === permission.op_zf) {
                    p.zf = pi.isHave;
                } else if (pi.permissionItem === permission.op_sk) {
                    p.sk = pi.isHave;
                } else if (pi.permissionItem === permission.op_fk) {
                    p.fk = pi.isHave;
                } else if (pi.permissionItem === permission.op_ty) {
                    p.ty = pi.isHave;
                } else if (pi.permissionItem === permission.op_zy) {
                    p.zy = pi.isHave;
                } else if (pi.permissionItem === permission.op_jy) {
                    p.jy = pi.isHave;
                }
            })
            this.setState({ permission: p })
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

    _getFactGet = async (report) => {
        if (!report || !report.id)
            return;


        try {
            let url = `${WebApiConfig.rp.rpFaceget}${report.id}`;
            let r = await ApiClient.get(url, true, { type: 1, status:"8" });
            r = (r || {}).data;
            if (r.code === '0') {
                if (r.extension) {
                    return r.extension;
                } else {

                }
            } else {
                notification.error({ message: '获取成交报告收款信息失败', description: r.message || '' });
            }
        } catch (e) {
            notification.error({ message: '获取成交报告收款信息失败' })
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

    reportChanged = (report, action) => {
        if (action === 'DEL') {
            let rl = this.state.list;
            let idx = rl.findIndex(r => r.id === report.id);
            if (idx >= 0) {
                rl.splice(idx, 1)
                this.setState({ list: [...rl] })
            }
        } else {
            let rl = this.state.list;
            let idx = rl.findIndex(r => r.id === report.id);
            if (idx >= 0) {
                rl[idx] = { ...rl[idx], ...report }
                this.setState({ list: [...rl] })
            }
        }
    }

    operate = (action, report) => {
        let a = action.action;
        if (a === reportOperateAction.ty.action) {
            //调佣
            this._opTy(report);
        } else if (a === reportOperateAction.sk.action) {
            //收款
            this._opSk(report);
        } else if (a === reportOperateAction.fk.action) {
            //付款
            this._opFk(report);
        } else if (a === reportOperateAction.zf.action) {
            //作废
            this._opZf(report);
        } else if (a === reportOperateAction.zy.action) {
            //转移
            this._opZy(report);
        }else if (a === reportOperateAction.jy.action) {
            //结佣
            this._opJy(report);
        }
    }

    _opTy = async (report) => {
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
            notification.error({ message: '获取业绩分配表详情失败，' + (e.message || '') })
        }
        this.setState({ loading: false })
    }

    _opSk = async (report) => {
        if (!report || !report.id)
            return;


        this.setState({ loading: true })
        try {
            let rp = await this._getReport(report);
            let entity = {
                id: uuid.v1(),
                sectionId: rp.applySectionId,
                sectionName: rp.applySectionName,
                skrId: rp.uid,
                skr: rp.uTrueName,
                dsdfType: 1,
                status: 0,
                yt: '业主佣金',
                gszh: null,
                skfs: '转账',
                cjbgbh: rp.cjbgbh,
                wymc: (rp.reportWy || {}).wyMc,
                fgs: rp.gsmc,
                report: rp
            }

            this.props.history.push(`${this.props.match.url}/sk`, { entity: entity, op: 'add', pagePar: this.state.pagePar })
        } catch (e) {
            notification.error({ message: '获取业绩分配表详情失败，' + (e.message || '') })
        }
        this.setState({ loading: false })
    }

    _opFk = async (report) => {
        if (!report || !report.id)
            return;


        this.setState({ loading: true })
        try {
            let rp = await this._getReport(report);
            let entity = {
                id: uuid.v1(),
                sectionId: rp.applySectionId,
                sectionName: rp.applySectionName,
                skrId: rp.uid,
                skr: rp.uTrueName,
                dsdfType: 2,
                status: 0,
                yt: null,
                gszh: null,
                skfs: '转账',
                cjbgbh: rp.cjbgbh,
                wymc: (rp.reportWy || {}).wyMc,
                fgs: rp.gsmc,
                report: rp
            }

            this.props.history.push(`${this.props.match.url}/fk`, { entity: entity, op: 'add', pagePar: this.state.pagePar })
        } catch (e) {
            notification.error({ message: '获取业绩分配表详情失败，' + (e.message || '') })
        }
        this.setState({ loading: false })
    }

    _opZf = async (report) => {
        if (!report || !report.id) {
            return;
        }

        confirm({
            title: `您确定要作废此成交报告吗?`,
            content: '作废后不可恢复',
            onOk: async () => {
                let url = `${WebApiConfig.rp.rpDel}${report.id}`
                this.setState({ loading: true })
                try {
                    let r = await ApiClient.post(url, null, null, 'DELETE');
                    r = (r || {}).data;
                    if (r.code === '0') {
                        notification.success({ message: '成交报告已经作废!' })
                        this.reportChanged(report, 'DEL')
                    } else {
                        notification.error({ message: '作废成交报告失败!', description: r.message || '' })

                    }
                } catch (e) {
                    notification.error({ message: '作为成交报告失败!', description: e.message })
                }
                this.setState({ loading: false })
            },
            onCancel() {

            },
        });


    }

    _opZy = (report) => {
        this.props.history.push(`${this.props.match.url}/zy`, { entity: report, op: 'add', pagePar: this.state.pagePar })
    }

    _opJy = async (report) => {
        if (!report || !report.id)
            return;

        this.setState({ loading: true })
        try {
            let rp = await this._getReport(report);
            let list = await this._getFactGet(report);

            this.props.history.push(`${this.props.match.url}/jy`, { entity: rp, list: list || [], op: 'add', pagePar: this.state.pagePar })

        } catch (e) {
            notification.error({ message: '获取成交报告收款信息失败' })
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
                        <DealRpTable
                            loading={this.state.loading}
                            viewReport={this.viewReport}
                            dic={this.props.dic}
                            dataSource={this.state.list}
                            pagination={this.state.pagination}
                            pageChanged={this.pageChanged}
                            reportChanged={this.reportChanged}
                            permission={this.state.permission}
                            operate={this.operate}
                            type={'query'} />
                    </Col>
                </Row>



                <LayerRouter>
                    <Route path={`${this.props.match.url}/reportInfo`} render={(props) => <TradeManager {...props} reportChanged={this.reportChanged} />} />
                    <Route path={`${this.props.match.url}/ty`} render={(props) => <TyPage {...props} />} />
                    <Route path={`${this.props.match.url}/sk`} render={(props) => <SKPage {...props} />} />
                    <Route path={`${this.props.match.url}/fk`} render={(props) => <FKPage {...props} />} />
                    <Route path={`${this.props.match.url}/zy`} render={(props) => <ZyPage {...props} />} />
                    <Route path={`${this.props.match.url}/jy`} render={(props) => <JyPage {...props} />} />
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
export default connect(MapStateToProps, MapDispatchToProps)(DealRpQuery);
