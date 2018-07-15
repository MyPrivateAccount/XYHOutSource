//交易合同管理页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Affix, Layout, Modal, Button, Checkbox, Tree, Tabs, Icon, Popconfirm, Spin, Tooltip, notification } from 'antd';
import TradeContract from './tradeContract'
import TradeEstate from './tradeEstate'
import TradeBnsOwner from './tradeBnsOwer'
import TradeCustomer from './tradeCustomer'
import TradePerDis from './tradePerDis'
import TradeAttact from './tradeAttact'
import TradeTransfer from './tradeTransfer'
import TradeAjust from './tradeAjust'
import TradeReportTable from './tradeReportTable'
import Layer, { LayerRouter } from '../../../../components/Layer'
import { Route } from 'react-router'
import moment from 'moment'
import { getDicParList } from '../../../../actions/actionCreators'
import { dicKeys, branchPar } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import { getDicPars } from '../../../../utils/utils';
import {convertFactGet} from '../../../constants/utils'
import reportValidation from '../../../constants/reportValidation'
import validations from '../../../../utils/validations'
import TyPage from '../ty/tyPage'
import SKPage from '../sfk/SKPage'
import FKPage from '../sfk/FKPage'


const { Header, Sider, Content } = Layout;
const TreeNode = Tree.TreeNode;
const TabPane = Tabs.TabPane;
const confirm = Modal.confirm;

class TradeManager extends Component {

    constructor(props) {
        super(props)
        this.state = {
            opType: 'view',//控制状态,新增／编辑
            isEdit: false,
            activeTab: 'jyht',
            isDataLoading: false,

            showBbSelector: false,
            gettingBb: false,
            report: {},
            wyItems: [],
            nyItems: [],
            saving: false,
        }
    }

    componentWillReceiveProps = (newProps) => {

    }
    componentWillMount = () => {

    }
    componentDidMount = () => {
        this.props.getDicParList([
            dicKeys.wyfl,
            dicKeys.cjbglx,
            dicKeys.jylx,
            dicKeys.fkfs,
            dicKeys.xmlx,
            dicKeys.htlx,
            dicKeys.cqlx,
            dicKeys.xxjylx,
            dicKeys.zjjg,
            dicKeys.wylx,
            dicKeys.kjlx,
            dicKeys.jj,
            dicKeys.zxzk,
            dicKeys.cx,
            dicKeys.khxz,
            dicKeys.sfxycqjybgj,
            dicKeys.khly,
            dicKeys.sfdx,
            dicKeys.yjfj
        ]);
        let initState = (this.props.location || {}).state || {};
        this.setState({ opType: initState.op })
        this.initData(initState);

    }

    initData = async (initState) => {
        if (initState.op === 'add' || initState.op === 'edit') {
            //获取组织参数
            await this.getBranchPar();
        }
        if (initState.op === 'add') {
            this.setState({ report: initState.entity || {} })
        } else if (initState.entity) {
            //获取详情
            this.setState({ report: initState.entity || {} })
        }
        await this._getFtItems();
    }

    getBranchPar = async () => {
        if (!this.props.user.Filiale) {
            notification.warn({ message: '用户没有归属分公司' })
            return;
        }
        let url = `${WebApiConfig.baseset.orgget}${this.props.user.Filiale}/${branchPar.showBb}`
        let r = await ApiClient.get(url);
        r = (r || {}).data || {};
        if (r.code === '0') {
            if (r.extension && r.extension.parValue === '0') {
                this.setState({ showBbSelector: false })
            } else {
                this.setState({ showBbSelector: true })
            }
        }

       
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

    //验证
    validateVales = () => {
        let r = { code: '0', entity: null }
        let report = this._getSubFormValues();
        report = { ...this.state.report, ...report }
        report.reportWy = report.reportWy || {};
        report.reportYz = report.reportYz || {};
        report.reportKh = report.reportKh || {};
        report.reportYjfp = report.reportYjfp || {};
        r.entity = report;

        console.log(report);

        let errors = validations.validate(report, reportValidation.jyht);
        let wyErrors = validations.validate(report.reportWy, reportValidation.cjwy)
        errors = { ...errors, ...wyErrors }
        let yzErrors = validations.validate(report.reportYz, reportValidation.yzxx);
        errors = { ...errors, ...yzErrors }
        let khError = validations.validate(report.reportKh, reportValidation.khxx);
        errors = { ...errors, ...khError }
        let fpErrors = validations.validate(report.reportYjfp, reportValidation.yjfp, report);
        errors = { ...errors, ...fpErrors }

        if (validations.checkErrors(errors)) {
            r.code = "500"
            r.errors = errors;
            return r;
        }

        let ol = report.reportYjfp.reportOutsides || [];
        for (let i = 0; i < ol.length; i++) {
            let item = ol[i]
            let e = validations.validate(item, reportValidation.wyItem);
            if (validations.checkErrors(e)) {
                r.code = "500"
                r.errors = e;
                return r;
            }
        }


        let il = report.reportYjfp.reportInsides || [];
        for (let i = 0; i < il.length; i++) {
            let item = il[i]
            let e = validations.validate(item, reportValidation.nrItem);
            if (validations.checkErrors(e)) {
                r.code = "500"
                r.errors = e;
                return r;
            }
        }

        return r;

    }

    save = async (msg) => {
        let r = this.validateVales();
        if (r.code !== '0') {
            let msg = [];
            for (let k in r.errors) {
                msg.push(<li>{r.errors[k]}</li>)
            }
            notification.error({ message: '验证失败，无法保存', description: <ol>{msg}</ol> })
            return;
        }

        let values = r.entity;

        this.setState({ report: values, saving: true })
        let url = `${WebApiConfig.rp.rpAdd}`
        r = await ApiClient.post(url, values, null, 'PUT');
        r = (r || {}).data || {};
        if (r.code === '0') {
            let newReport = r.extension;
            let report = this.state.report;
            report.cjbgbh = newReport.cjbgbh;
            report.gsqz = newReport.gsqz;
            report.seq = newReport.seq;

            console.log(report);
            this.setState({ report: { ...report } })
            if (msg) {
                notification.success({ message: '保存成功' })
            }
            if (this.props.reportChanged) {
                this.props.reportChanged(report)
            }
            return true;
        } else {
            notification.error({ message: '保存失败', description: r.message || '' })
        }

        this.setState({ saving: false })
    }

    submit = async () => {
        confirm({
            title: `您确定要将此成交报告提交审核吗?`,
            content: '提交后不可再进行修改',
            onOk: async () => {
                let isOk = await this.save(false);
                if (isOk) {
                    //提交
                    this.setState({ saving: true })
                    try {
                        let url = `${WebApiConfig.rp.rpSubmit}${this.state.report.id}`;
                        let r = await ApiClient.post(url, null)
                        r = (r || {}).data || {};
                        if (r.code === '0') {
                            notification.success({ message: '成交报告已提交审核' })
                            setTimeout(() => {
                                let r = this.state.report;
                                r.examineStatus = 1;
                                this.setState({ report: { ...r } })
                                if (this.props.reportChanged) {
                                    this.props.reportChanged({ id: r.id, examineStatus: r.examineStatus })
                                }
                            }, 0);
                        } else {
                            notification.error({ message: '提交成交报告失败', description: r.message || '' })
                        }
                    } catch (e) {
                        notification.error({ message: '提交成交报告失败', description: e.message })
                    }
                    this.setState({ saving: false })
                }

            },
            onCancel() {

            },
        });
    }

    inputChanged = (key, value) => {
        let newReport = this._getSubFormValues();

        if (key === 'cjrq') {
            console.log(value)
        }
        if (key === 'cjzj') {
            //更新均价
            newReport.cjzj = value;
            let wy = newReport.reportWy || this.state.report.reportWy;
            if (wy) {
                if (wy.wyJzmj) {
                    wy.wyWyJj = Math.round((newReport.cjzj / wy.wyJzmj) * 100) / 100;
                    newReport.reportWy = { ...wy };
                }
                this.setState({ report: { ...this.state.report, ...newReport } })
            }



        }
        if (key === 'yj') {
            newReport.ycjyj = value;
            let yjfp = newReport.reportYjfp || this.state.report.reportYjfp;
            if (yjfp) {
                yjfp.yjYzys = value - (yjfp.yjKhys || 0)
                yjfp.yjZcjyj = value;
                let zyj = value;

                //更新外佣
                let ol = yjfp.reportOutsides || [];
                let wyItems = this.state.wyItems;
                let wyJe = 0;
                ol.forEach(item => {
                    let x = wyItems.find(x => x.code === item.moneyType);
                    if (x && x.isfixed) {
                        item.money = Math.round((zyj * (x.percent || 0)) * 100) / 100;
                    }
                    wyJe = wyJe + item.money;
                    wyJe = Math.round(wyJe * 100) / 100;
                })
                yjfp.yjJyj = zyj - wyJe;

                //更新内佣
                let il = yjfp.reportInsides || [];
                let nyItems = this.state.nyItems;
                let nyJe = 0;
                let lastItem = null;
                il.forEach(item => {
                    // let x = nyItems.find(x=>x.code === item.type);

                    item.money = Math.round((yjfp.yjJyj * (item.percent || 0))) / 100;

                    nyJe = nyJe + item.money;
                    nyJe = Math.round(nyJe * 100) / 100;
                    lastItem = item;
                })
                let diff = Math.abs(nyJe - yjfp.yjJyj)
                if (diff <= 0.2 && diff > 0) {
                    if (lastItem) {
                        lastItem.money = lastItem.money + (yjfp.yjJyj - nyJe);
                    }
                }


                newReport.reportYjfp = { ...yjfp }
                this.setState({ report: { ...this.state.report, ...newReport } })
            }
        }


    }

    showBbDialog = (b) => {
        this.setState({ showBbDialog: b })
    }
    bbChanged = async (item) => {
        console.log(item);
        this.setState({ gettingBb: true })
        let newReport = this._getSubFormValues();

        newReport.fyzId = item.departmentId;
        newReport.fyzName = item.departmentName;
        newReport.cjrq = moment(item.dealTime);
        newReport.cjzj = item.totalPrice * 1;
        newReport.ycjyj = item.commission * 1;
        newReport.cjrId = item.userId;
        newReport.cjrName = item.userName;
        newReport.cjbbId = item.id;
        newReport.lpId = item.projectId;
        newReport.spId = item.spId;
        newReport.cjbbId = item.id;

        //获取楼盘信息
        let wy = newReport.reportWy || this.state.report.reportWy || {};
        wy = { ...wy }
        let url = `${WebApiConfig.project.get}${item.projectId}`
        let r = await ApiClient.get(url);
        r = (r || {}).data || {};
        if (r.code === '0' && r.extension) {
            let bi = r.extension.basicInfo || {};
            wy.wyCq = bi.district;
            wy.wyPq = bi.area;
            wy.wyMc = bi.name;
            wy.wyCzwydz = bi.address;
        } else {
            notification.error({ message: '获取楼盘信息失败' })
        }
        url = `${WebApiConfig.project.getShop}${item.shopId}`
        r = await ApiClient.get(url);
        r = (r || {}).data || {};
        if (r.code === '0' && r.extension) {
            let bi = r.extension.basicInfo || {};
            wy.wyWz = bi.buildingNo;
            wy.wyLc = bi.floorNo;
            wy.wyFh = bi.number;
            wy.wyZlc = bi.floors;
            wy.wyJzmj = bi.buildingArea;
            wy.wySymj = bi.houseArea;
            if (wy.wyJzmj) {
                wy.wyWyJj = Math.round((newReport.cjzj / wy.wyJzmj) * 100) / 100;
            }

            let cxList = getDicPars(dicKeys.cx, this.props.dic);
            let item = cxList.find(x => x.value === bi.toward);
            if (item) {
                wy.wyCx = item.key;
            }
        } else {
            notification.error({ message: '获取商铺信息失败' })
        }
        newReport.reportWy = wy;

        let yz = newReport.reportYz || this.state.report.reportYz || {};
        yz = { ...yz }
        yz.yzMc = wy.wyMc;
        newReport.reportYz = yz;

        let kh = newReport.reportKh || this.state.report.reportKh || {};
        kh = { ...kh }
        kh.khMc = item.customerName;
        newReport.reportKh = kh;
        if (item.customerInfo) {
            let csList = getDicPars(dicKeys.khly, this.props.dic);
            let cs = csList.find(x => x.value === item.customerInfo.source);
            if (cs) {
                kh.khKhly = cs.key;
            }
        }

        let yj = newReport.reportYjfp || this.state.report.reportYjfp || {};
        yj.yjYzyjdqr = newReport.cjrq.days(180);
        yj.yjKhyjdqr = newReport.cjrq.days(180);
        yj.yjYzys = newReport.ycjyj;
        yj.yjKhys = 0;
        yj.yjZcjyj = yj.yjYzys + yj.yjKhys;
        yj.yjJyj = yj.yjZcjyj;
        newReport.reportYjfp = { ...yj };

        console.log(newReport);

        this.setState({ report: { ...this.state.report, ...newReport } })

        this.showBbDialog(false);
        this.setState({ gettingBb: false })
    }

    _getSubFormValues = () => {
        let values = this.jyhtForm.props.form.getFieldsValue();
        if (this.wyForm) {
            values.reportWy = this.wyForm.props.form.getFieldsValue();
        }
        if (this.yzForm) {
            values.reportYz = this.yzForm.props.form.getFieldsValue();
        }
        if (this.khForm) {
            values.reportKh = this.khForm.props.form.getFieldsValue();
        }
        if (this.yjForm) {
            values.reportYjfp = this.yjForm.getValues();
        }
        if (this.reportFiles) {
            values.reportFiles = this.reportFiles.getValues();
        }
        if (this.ajForm) {
            values.reportGh = this.ajForm.props.form.getFieldsValue();
        }

        return values;
    }
    
    
    _viewDistribute =async ({id, reportId})=>{
        if (!id)
            return;

        this.setState({ isDataLoading: true })
        try {
            let url = `${WebApiConfig.rp.rpDis}${reportId}`;
            let r = await ApiClient.get(url, true, {distributeId: id});
            r = (r || {}).data;
            if (r.code === '0') {
                if (r.extension) {
                    this.props.history.push(`${this.props.match.url}/ty`, { entity: r.extension, op: 'view', pagePar: this.state.pagePar })
                } else {
                    notification.error({ message: '业绩分配表不存在' });
                }
            } else {
                notification.error({ message: '获取业绩分配表详情失败', description: r.message || '' });
            }
        } catch (e) {
            notification.error({ message: '获取业绩分配表详情失败，' + (e.message || '') })
        }
        this.setState({ isDataLoading: false })
    }

    _viewFactGet  = async ({id, reportId})=>{
        if (!id)
        return;

    this.setState({ isDataLoading: true })
    try {
        let url = `${WebApiConfig.rp.factget}${id}`;
        let r = await ApiClient.get(url, true);
        r = (r || {}).data;
        if (r.code === '0') {
            if (r.extension) {
                let fg = convertFactGet(r.extension)
                let pn = fg.dsdfType===1?'sk':'fk'
                this.props.history.push(`${this.props.match.url}/${pn}`, { entity: fg, op: 'view', pagePar: this.state.pagePar })
            } else {
                notification.error({ message: '获取收付款明细失败' });
            }
        } else {
            notification.error({ message: '获取收付款明细失败', description: r.message || '' });
        }
    } catch (e) {
        notification.error({ message: '获取收付款明细失败' + (e.message || '') })
    }
    this.setState({ isDataLoading: false })
    }
    

    render() {
        let canEdit = this.state.opType == 'edit' || this.state.opType == 'add'
        if (this.state.report.examineStatus !== 0 && this.state.report.examineStatus !== 16) {
            canEdit = false;
        }
        return (
            <Layer>
                <div className="full" style={{ overflow: 'auto' }} ref={(node) => { this.container = node; }}>
                    {/* <div>
                        <Tooltip title="返回">
                            <Button type='primary' shape='circle' icon='arrow-left' style={{ 'margin': 10, float: 'left' }} onClick={this.props.handleback} />
                        </Tooltip>
                    </div> */}
                    <Spin spinning={this.state.isDataLoading || this.state.saving}>
                        <Content style={{ overflowY: 'auto', height: '100%' }} >
                            <Affix offsetTop target={() => this.container} style={{ paddingTop: '1rem', paddingBottom: '1rem' }}>
                                {canEdit ? <div>
                                    <Button disabled={this.state.saving} type="primary" onClick={this.save}>保存</Button>
                                    <Button disabled={this.state.saving} type="primary" onClick={this.submit} style={{ marginLeft: '0.5rem' }}>提交审核</Button>
                                </div> : null}
                            </Affix >
                            <Tabs defaultActiveKey="jyht" onChange={this.loadTabData}>
                                <TabPane tab="交易合同" key="jyht">
                                    <TradeContract
                                        canEdit={canEdit}
                                        inputChanged={this.inputChanged}
                                        entity={this.state.report}
                                        wrappedComponentRef={(inst) => this.jyhtForm = inst}
                                        showDialog={this.showBbDialog}
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}
                                        rpId={this.state.rpId}
                                        opType={this.state.opType}
                                        onShowCjbbtb={this.onShowCjbbtb} />
                                </TabPane>
                                <TabPane tab="成交物业" key="cjwy">
                                    <TradeEstate
                                        canEdit={canEdit}
                                        entity={this.state.report.reportWy}
                                        wrappedComponentRef={(inst) => this.wyForm = inst}
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}
                                        rpId={this.state.rpId}
                                        opType={this.state.opType} />
                                </TabPane>
                                <TabPane tab="业主信息" key="yzxx">
                                    <TradeBnsOwner
                                        canEdit={canEdit}
                                        entity={this.state.report.reportYz}
                                        wrappedComponentRef={(inst) => this.yzForm = inst}
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}
                                        rpId={this.state.rpId}
                                        opType={this.state.opType} />
                                </TabPane>
                                <TabPane tab="客户信息" key="khxx">
                                    <TradeCustomer
                                        canEdit={canEdit}
                                        entity={this.state.report.reportKh}
                                        wrappedComponentRef={(inst) => this.khForm = inst}
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}
                                        rpId={this.state.rpId}
                                        opType={this.state.opType} />
                                </TabPane>
                                <TabPane tab="业绩分配" key="yjfp">
                                    <TradePerDis
                                        canEdit={canEdit}
                                        report={this.state.report}
                                        entity={this.state.report.reportYjfp}
                                        wrappedComponentRef={(inst) => this.yjForm = inst}
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}
                                        viewFactGet={this._viewFactGet}
                                        wyItems={this.state.wyItems}
                                        nyItems={this.state.nyItems}
                                        rpId={this.state.rpId}
                                        opType={this.state.opType}
                                    />
                                </TabPane>
                                <TabPane tab="附件" key="fj">
                                    <TradeAttact
                                        canEdit={canEdit}
                                        report={this.state.report}
                                        entity={this.state.report.reportFiles}
                                        showBbSelector={this.state.showBbSelector}
                                        wrappedComponentRef={(inst) => this.reportFiles = inst}
                                        dic={this.props.dic}
                                        rpId={this.state.rpId}
                                        opType={this.state.opType} />
                                </TabPane>
                                <TabPane tab="按揭过户" key="ajgh">
                                    <TradeTransfer
                                        canEdit={canEdit}
                                        entity={this.state.report.reportGh}
                                        wrappedComponentRef={(inst) => this.ajForm = inst}
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}
                                        rpId={this.state.rpId}
                                        opType={this.state.opType} />
                                </TabPane>
                                {
                                    !canEdit ? (<TabPane tab="业绩调整" key="yjtz">
                                        <TradeAjust
                                            report={this.state.report}
                                            entity={this.state.report.distributeList}
                                            canEdit={canEdit}
                                            view = {this._viewDistribute}
                                            opType={this.state.opType} />
                                    </TabPane>) : null
                                }
                            </Tabs>
                        </Content>
                    </Spin>
                    <TradeReportTable loading={this.state.gettingBb} visible={this.state.showBbDialog} onClose={this.showBbDialog} selectedCallback={this.bbChanged} />
                </div>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/ty`} render={(props) => <TyPage hidePre={true} {...props} />} /> 
                    <Route path={`${this.props.match.url}/sk`} render={(props) => <SKPage {...props} />} />
                    <Route path={`${this.props.match.url}/fk`} render={(props) => <FKPage {...props} />} />
                </LayerRouter>
            </Layer>
        )
    }
}
function MapStateToProps(state) {
    return {
        // operInfo: state.rp.operInfo,
        // shopInfo: state.rp.shopInfo,
        // buildingInfo: state.rp.buildingInfo,
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
export default connect(MapStateToProps, MapDispatchToProps)(TradeManager);