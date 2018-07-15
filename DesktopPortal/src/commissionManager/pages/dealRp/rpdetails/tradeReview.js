//交易合同管理页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Affix, Button, Tabs, Layout, Spin, notification } from 'antd';
import TradeContract from './tradeContract'
import TradeEstate from './tradeEstate'
import TradeBnsOwner from './tradeBnsOwer'
import TradeCustomer from './tradeCustomer'
import TradePerDis from './tradePerDis'
import TradeAttact from './tradeAttact'
import TradeTransfer from './tradeTransfer'
import { getDicParList } from '../../../../actions/actionCreators'
import { dicKeys } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import { convertReport } from '../../../constants/utils'
import TradeAjust from './tradeAjust'

const TabPane = Tabs.TabPane;
const { Content } = Layout;

class TradeReview extends Component {

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



        this.initData();

    }

    initData = async () => {

        await this._getFtItems();
        this._getReport();
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

    _getReport = async () => {
        let rid = this.props.reportId;
        if (!rid) {
            let ai = this.props.review || {};
            if (!ai.contentId) {
                return;
            }
            rid = ai.contentId;
        }


        this.setState({ isDataLoading: true })
        try {

            let url = `${WebApiConfig.rp.rpGet}${rid}`;
            let r = await ApiClient.get(url);
            r = (r || {}).data;
            if (r.code === '0') {
                if (r.extension) {

                    let report = convertReport(r.extension);
                    this.setState({ report: report })
                } else {
                    notification.error({ message: '成交报告不存在' });
                }
            } else {
                notification.error({ message: '获取成交报告详情失败', description: r.message || '' });
            }
        } catch (e) {
            notification.error({ message: '获取成交报告详情失败' })
        }
        this.setState({ isDataLoading: false })

    }

    render() {
        let canEdit = false;
        let hideYjfp = this.props.hideYjfp;
        if (typeof hideYjfp === 'undefined') {
            hideYjfp = true;
        }
        return (

            <div >
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
                                    hideYjfp={hideYjfp}
                                    hideSfkCk={this.props.hideSfkCk}
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
                                this.props.showYjtz ? <TabPane tab="业绩调整" key="yjtz"><TradeAjust
                                    report={this.state.report}
                                    entity={this.state.report.distributeList}
                                    canEdit={canEdit}
                                    hideYjtzCk={this.props.hideYjtzCk}
                                    view={this._viewDistribute}
                                    opType={this.state.opType} />
                                </TabPane> : null
                            }
                            {/* {
                                    !canEdit ? (<TabPane tab="业绩调整" key="yjtz">
                                        <TradeAjust
                                            report={this.state.report}
                                            entity={this.state.report.distributeList}
                                            canEdit={canEdit}
                                            view = {this._viewDistribute}
                                            opType={this.state.opType} />
                                    </TabPane>) : null
                                } */}
                        </Tabs>
                    </Content>
                </Spin>

            </div>


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
export default connect(MapStateToProps, MapDispatchToProps)(TradeReview);