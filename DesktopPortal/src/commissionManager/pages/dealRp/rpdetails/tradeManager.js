//交易合同管理页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { dealRpGet,dealGhGet,dealFpGet,dealWyGet,dealKhGet,dealYzGet } from '../../../actions/actionCreator'
import { Modal, Layout, Table, Button, Checkbox, Tree, Tabs, Icon, Popconfirm, Spin, Tooltip } from 'antd';
import TradeContract from './tradeContract'
import TradeEstate from './tradeEstate'
import TradeBnsOwner from './tradeBnsOwer'
import TradeCustomer from './tradeCustomer'
import TradePerDis from './tradePerDis'
import TradeAttact from './tradeAttact'
import TradeTransfer from './tradeTransfer'
import TradeAjust from './tradeAjust'

const { Header, Sider, Content } = Layout;
const TreeNode = Tree.TreeNode;
const TabPane = Tabs.TabPane;

class TradeManager extends Component {

    state = {
        rpId: '',//统一的reportId
        opType: 'add',//控制状态,新增／编辑
        isEdit: false,
        activeTab: 'jyht',
    }
    componentWillReceiveProps = (newProps) => {
        if (newProps.vs) {
            if (this.state.isEdit) {
                this.loadTabData(this.state.activeTab)
            }
        }
    }
    componentWillMount = () => {
    }
    componentDidMount = () => {
        //新增加录入成交报告
        if (this.props.rpId == null || this.props.rpId === '') {
            let uuid = this.uuid()
            this.setState({ rpId: uuid })
        }
        else {
            this.setState({ rpId: this.props.rpId, isEdit: true })
        }
    }
    loadTabData = (e) => {
        console.log(e)
        if (e === 'jyht' && this.state.isEdit) {
            this.props.dispatch(dealRpGet(this.props.rpId));
        }
        else if (e === 'cjwy' && this.state.isEdit) {
            this.props.dispatch(dealWyGet(this.props.rpId));
        }
        else if (e === 'yzxx' && this.state.isEdit) {
            this.props.dispatch(dealYzGet(this.props.rpId));
        }
        else if (e === 'khxx' && this.state.isEdit) {
            this.props.dispatch(dealKhGet(this.props.rpId));
        }
        else if (e === 'yjfp' && this.state.isEdit) {
            this.props.dispatch(dealFpGet(this.props.rpId));
        }
        else if (e === 'ajgh' && this.state.isEdit) {
            this.props.dispatch(dealGhGet(this.props.rpId));
        }
        else if (e === 'fj' && this.state.isEdit) {
            
        }
        else if (e === 'yjtz' && this.state.isEdit) {
            
        }
        this.setState({ activeTab: e });
    }
    uuid = () => {
        var s = [];
        var hexDigits = "0123456789abcdef";
        for (var i = 0; i < 36; i++) {
            s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
        }
        s[14] = "4";  // bits 12-15 of the time_hi_and_version field to 0010
        s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01
        s[8] = s[13] = s[18] = s[23] = "";

        var uuid = s.join("")
        console.log(uuid);
        return uuid;
    }
    render() {
        return (
            <div style={{ display: this.props.vs ? 'block' : 'none' }}>
                <Layout>
                    <div>
                        <Tooltip title="返回">
                            <Button type='primary' shape='circle' icon='arrow-left' style={{ 'margin': 10, float: 'left' }} onClick={this.props.handleback} />
                        </Tooltip>
                    </div>
                    <Content style={{ overflowY: 'auto', height: '100%' }}>
                        <Tabs defaultActiveKey="jyht" onChange={this.loadTabData}>
                            <TabPane tab="交易合同" key="jyht">
                                <TradeContract rpId={this.state.rpId} opType={this.state.opType} />
                            </TabPane>
                            <TabPane tab="成交物业" key="cjwy">
                                <TradeEstate rpId={this.state.rpId} opType={this.state.opType} />
                            </TabPane>
                            <TabPane tab="业主信息" key="yzxx">
                                <TradeBnsOwner rpId={this.state.rpId} opType={this.state.opType} />
                            </TabPane>
                            <TabPane tab="客户信息" key="khxx">
                                <TradeCustomer rpId={this.state.rpId} opType={this.state.opType} />
                            </TabPane>
                            <TabPane tab="业绩分配" key="yjfp">
                                <TradePerDis rpId={this.state.rpId} opType={this.state.opType} />
                            </TabPane>
                            <TabPane tab="附件" key="fj">
                                <TradeAttact rpId={this.state.rpId} opType={this.state.opType} />
                            </TabPane>
                            <TabPane tab="按揭过户" key="ajgh">
                                <TradeTransfer rpId={this.state.rpId} opType={this.state.opType} />
                            </TabPane>
                            <TabPane tab="业绩调整" key="yjtz">
                                <TradeAjust rpId={this.state.rpId} opType={this.state.opType} />
                            </TabPane>
                        </Tabs>
                    </Content>
                </Layout>
            </div>
        )
    }
}
function MapStateToProps(state) {
    return {
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(TradeManager);