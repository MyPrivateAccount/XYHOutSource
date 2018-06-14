//交易合同管理页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { dealRpGet, dealGhGet, dealFpGet, dealWyGet, dealKhGet, dealYzGet, getShopDetail, getBuildingDetail } from '../../../actions/actionCreator'
import { Modal, Layout, Table, Button, Checkbox, Tree, Tabs, Icon, Popconfirm, Spin, Tooltip } from 'antd';
import TradeContract from './tradeContract'
import TradeEstate from './tradeEstate'
import TradeBnsOwner from './tradeBnsOwer'
import TradeCustomer from './tradeCustomer'
import TradePerDis from './tradePerDis'
import TradeAttact from './tradeAttact'
import TradeTransfer from './tradeTransfer'
import TradeAjust from './tradeAjust'
import TradeReportTable from './tradeReportTable'
import moment from 'moment'

const { Header, Sider, Content } = Layout;
const TreeNode = Tree.TreeNode;
const TabPane = Tabs.TabPane;

class TradeManager extends Component {

    state = {
        rpId: '',//统一的reportId
        opType: 'add',//控制状态,新增／编辑
        isEdit: false,
        activeTab: 'jyht',
        isGetShopDetail: false,
        isGetBuildingDetail: false,
        isDataLoading: false,
        ccbbInfo: {},//成交报备信息
        shopInfo: {},//商铺详情
        buildingInfo: {},//楼盘详情
        rpds: {},//自动填充报告基础信息
        wyds: {},//自动填充的物业信息
        yzds: {},//自动填充的业主信息
        khds: {},//自动填充的客户信息
        fpds: {},//自动填充的业绩分配信息
    }
    componentWillReceiveProps = (newProps) => {

        this.setState({ isDataLoading: false })

        if (newProps.vs) {
            if (this.state.isEdit) {
                this.loadTabData(this.state.activeTab)
            }
        }
        if (newProps.operInfo.operType === 'DEALRP_BUILDING_GET_SUCCESS') {
            console.log("autoSetCJBBInfo")
            //自动设置成交报备信息
            this.autoSetCJBBInfo(newProps.shopInfo,newProps.buildingInfo)
            newProps.operInfo.operType = ''
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
    //显示成交报备列表
    onShowCjbbtb = (e) => {
        this.cjbbtb.show()
    }
    onTradReportTableSelf = (e) => {
        this.cjbbtb = e
    }
    //双击选择成交报备
    onHandleChooseCjbb = (e) => {
        console.log(e)
        this.setState({ ccbbInfo: e })
        this.setState({ isDataLoading: true })
        this.props.dispatch(getShopDetail(e.shopId));
        this.props.dispatch(getBuildingDetail(e.projectId));
    }
    //自动设置成交报备信息
    autoSetCJBBInfo = (s,b) => {
        let cjbbInfo = this.state.ccbbInfo
        let shopInfo = s
        let buildingInfo = b

        let rpds = [...this.state.rpds]
        rpds.fyzId = cjbbInfo.departmentName//分行（组）名称
        rpds.cjrId = cjbbInfo.userName//成交人名称
        rpds.cjrq = cjbbInfo.createTime//成交日期
        rpds.cjzj = cjbbInfo.totalPrice//成交总价
        rpds.ycjyj = cjbbInfo.commission//成交佣金
        this.setState({ rpds },()=>{
            if(this.rpds!=null){
                this.rpds.loadData()
            }
        })

        let wyds = [...this.state.wyds]
        wyds.wyCq = buildingInfo.basicInfo.city//物业城区
        wyds.wyPq = buildingInfo.basicInfo.district//物业片区
        wyds.wyMc = buildingInfo.basicInfo.name//物业名称
        /*wyds.wyWz=//物业位置
        wyds.wyLc=//物业楼层
        wyds.wyFh=//物业房号
        wyds.wyZlc=//物业总楼层
        wyds.wyCzwydz=//产证物业地址
        wyds.wyF=//房
        wyds.wyT=//厅
        wyds.wyW=//卫
        wyds.wyYt=//阳台
        wyds.wyLt=//露台
        wyds.wyJgf=//观景台
        wyds.wyWylx=//物业类型
        wyds.wyKjlx=//空间类型
        wyds.wyJzmj=//建筑面积
        wyds.wyWyJj=//均价
        wyds.wyDts=//电梯数
        wyds.wyMchs=//每层户数
        wyds.wyZxnd=//装修年代
        wyds.wyJj=//家具
        wyds.wyCqzqdsj=//产权取得时间
        wyds.wyFcajhm=//房产按揭号码
        wyds.wySfhz=//是否合租
        wyds.wyFyfkfs=//房源付款方式
        wyds.wyFydknx=//房源贷款年限
        wyds.wyFydksynx=//房源贷款剩余年限*/
        this.setState({ wyds },()=>{
            if(this.wyds!=null){
                this.wyds.loadData()
            }
        })

        let yzds = [...this.state.yzds]
        yzds.yzMc = cjbbInfo.customerName//业主名称
        yzds.yzSj = cjbbInfo.customerPhone//业主电话
        this.setState({ yzds },()=>{
            if(this.yzds!=null){
                this.yzds.loadData()
            }
        })

        let khds = [...this.state.khds]
        khds.khMc = cjbbInfo.customerName
        khds.khSj = cjbbInfo.customerPhone
        this.setState({ khds },()=>{
            if(this.khds!=null){
                this.khds.loadData()
            }
        })

        let fpds = [...this.state.fpds]
        fpds.yjYzys = cjbbInfo.commission//业主应收
        let m1 = moment(cjbbInfo.createTime).add(180, 'days')
        fpds.yjYzyjdqr = m1.format('YYYY-MM-DD')//业主佣金到期日
        fpds.yjKhyjdqr = m1.format('YYYY-MM-DD')//客户佣金到期日

        this.setState({ fpds },()=>{
            if(this.fpds!=null){
                this.fpds.loadData()
            }
        })
    }
    onSelf=(e,type)=>{
        if(type==='rpds'){
            this.rpds = e
        }
        else if(type==='wyds'){
            this.wyds = e
        }
        else if(type==='yzds'){
            this.yzds = e
        }
        else if(type==='khds'){
            this.khds = e
        }
        else if(type==='fpds'){
            this.fpds = e
        }
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
                    <Spin spinning={this.state.isDataLoading}>
                        <Content style={{ overflowY: 'auto', height: '100%' }}>
                            <Tabs defaultActiveKey="jyht" onChange={this.loadTabData}>
                                <TabPane tab="交易合同" key="jyht">
                                    <TradeContract onSelf={this.onSelf} rpId={this.state.rpId} opType={this.state.opType} onShowCjbbtb={this.onShowCjbbtb} ds={this.state.rpds} />
                                </TabPane>
                                <TabPane tab="成交物业" key="cjwy">
                                    <TradeEstate onSelf={this.onSelf} rpId={this.state.rpId} opType={this.state.opType} ds={this.state.wyds} />
                                </TabPane>
                                <TabPane tab="业主信息" key="yzxx">
                                    <TradeBnsOwner onSelf={this.onSelf} rpId={this.state.rpId} opType={this.state.opType} ds={this.state.yzds} />
                                </TabPane>
                                <TabPane tab="客户信息" key="khxx">
                                    <TradeCustomer onSelf={this.onSelf} rpId={this.state.rpId} opType={this.state.opType} ds={this.state.khds} />
                                </TabPane>
                                <TabPane tab="业绩分配" key="yjfp">
                                    <TradePerDis onSelf={this.onSelf} rpId={this.state.rpId} opType={this.state.opType} ds={this.state.fpds} />
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
                    </Spin>
                    <TradeReportTable onSelf={this.onTradReportTableSelf} onHandleChooseCjbb={this.onHandleChooseCjbb} />
                </Layout>
            </div>
        )
    }
}
function MapStateToProps(state) {
    return {
        operInfo: state.rp.operInfo,
        shopInfo: state.rp.shopInfo,
        buildingInfo: state.rp.buildingInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(TradeManager);