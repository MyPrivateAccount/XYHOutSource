//交易合同管理页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { dealRpGet, dealGhGet, dealFpGet, dealWyGet, dealKhGet, dealYzGet, getShopDetail, getBuildingDetail 
         ,syncRp,syncWy,syncYz,syncKh,syncFp} from '../../../actions/actionCreator'
import { Affix, Layout, Table, Button, Checkbox, Tree, Tabs, Icon, Popconfirm, Spin, Tooltip, notification } from 'antd';
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
import {Route } from 'react-router'
import moment from 'moment'
import {getDicParList} from '../../../../actions/actionCreators'
import {dicKeys, branchPar} from '../../../constants/const'
import WebApiConfig from  '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import { getDicPars } from '../../../../utils/utils';



const { Header, Sider, Content } = Layout;
const TreeNode = Tree.TreeNode;
const TabPane = Tabs.TabPane;

class TradeManager extends Component {

    constructor(props){
        super(props)
        this.state = {
            showBbDialog:false,
            rpId: '',//统一的reportId
            opType: 'view',//控制状态,新增／编辑
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
    
            showBbSelector: false,
            report: {}
        }
    }
    
    componentWillReceiveProps = (newProps) => {

        // this.setState({ isDataLoading: false })
        // if(this.state.rpId!==newProps.rpId){
        //     this.setState({rpId:newProps.rpId,isEdit:newProps.isEdit},()=>{
        //         if (newProps.vs) {
        //             if (newProps.isEdit) {
        //                 this.loadTabData(this.state.activeTab)
        //             }
        //         }
        //     })
        // }
        // if (newProps.operInfo.operType === 'DEALRP_BUILDING_GET_SUCCESS') {
        //     console.log("autoSetCJBBInfo")
        //     //自动设置成交报备信息
        //     this.autoSetCJBBInfo(newProps.shopInfo,newProps.buildingInfo)
        //     newProps.operInfo.operType = ''
        // }
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
            dicKeys.sfdx
        ]);
        let initState= (this.props.location||{}).state ||{};
        this.setState({opType: initState.op})
        this.initData(initState);
       

        //新增加录入成交报告
        // if (this.props.rpId == null || this.props.rpId === '') {
        //     let uuid = this.uuid()
        //     this.setState({ rpId: uuid })
        // }
        // else {
        //     this.setState({ rpId: this.props.rpId, isEdit: true })
        // }
    }

    initData = async (initState)=>{
        if( initState.op === 'add' || initState.op === 'edit'){
            //获取组织参数
            await this.getBranchPar();
        }
        if(initState.op==='add'){
            this.setState({report: initState.entity||{}})
        }else if(initState.entity){
            //获取详情
        }
        
    }

    getBranchPar = async ()=>{
        if(!this.props.user.Filiale){
            notification.warn({message:'用户没有归属分公司'})
            return;
        }
        let url  = `${WebApiConfig.baseset.orgget}${this.props.user.Filiale}/${branchPar.showBb}`
        let r=  await ApiClient.get(url);
        r= (r||{}).data||{};
        if(r.code==='0'){
            if(r.extension && r.extension.parValue==='0'){
                this.setState({showBbSelector: false})
            }else{
                this.setState({showBbSelector: true})
            }
        }
    }

    // loadTabData = (e) => {
    //     console.log(e)
    //     if (e === 'jyht' && this.state.isEdit) {
    //         this.props.dispatch(dealRpGet(this.state.rpId));
    //     }
    //     else if (e === 'cjwy' && this.state.isEdit) {
    //         this.props.dispatch(dealWyGet(this.state.rpId));
    //     }
    //     else if (e === 'yzxx' && this.state.isEdit) {
    //         this.props.dispatch(dealYzGet(this.state.rpId));
    //     }
    //     else if (e === 'khxx' && this.state.isEdit) {
    //         this.props.dispatch(dealKhGet(this.state.rpId));
    //     }
    //     else if (e === 'yjfp' && this.state.isEdit) {
    //         this.props.dispatch(dealFpGet(this.state.rpId));
    //     }
    //     else if (e === 'ajgh' && this.state.isEdit) {
    //         this.props.dispatch(dealGhGet(this.state.rpId));
    //     }
    //     else if (e === 'fj' && this.state.isEdit) {

    //     }
    //     else if (e === 'yjtz' && this.state.isEdit) {

    //     }
    //     this.setState({ activeTab: e });
    // }
 
    inputChanged = (key, value)=>{
        let newReport = this._getSubFormValues();

        if(key === 'cjrq'){
            console.log(value)
        }
        if(key ==='cjzj'){
            //更新均价
            newReport.cjzj = value;
            let wy = newReport.reportWy || this.state.report.reportWy;
            if(wy){
                if(wy.wyJzmj){
                    wy.wyWyJj =  Math.round(( newReport.cjzj / wy.wyJzmj)*100)/100;
                    newReport.reportWy = {...wy};
                }
                this.setState({report: {...this.state.report,...newReport}})
            }
            

           
        }
        if(key === 'yj'){
            newReport.ycjyj = value;
            let yjfp = newReport.reportYjfp || this.state.report.reportYjfp;
            if(yjfp){
                yjfp.yjYzys = value - (yjfp.yjKhys||0)
                newReport.reportYjfp = {...yjfp}
                this.setState({report: {...this.state.report,...newReport}})
            }
        }

       
    }

    showBbDialog =(b)=>{
        this.setState({showBbDialog: b})
    }
    bbChanged=async (item)=>{
        console.log(item);
        let newReport = this._getSubFormValues();

        newReport.fyzId = item.departmentId;
        newReport.fyzName = item.departmentName;
        newReport.cjrq = moment(item.dealTime);
        newReport.cjzj = item.totalPrice*1;
        newReport.ycjyj = item.commission*1;
        newReport.cjrId = item.userId;
        newReport.cjrName = item.userName;
        newReport.cjbbId = item.id;

        //获取楼盘信息
        let wy = newReport.reportWy || this.state.report.reportWy ||{};
        wy = {...wy}
        let url = `${WebApiConfig.project.get}${item.projectId}`
        let r = await ApiClient.get(url);
        r = (r||{}).data||{};
        if(r.code==='0' && r.extension){
            let bi = r.extension.basicInfo||{};
            wy.wyCq=bi.district;
            wy.wyPq = bi.area;
            wy.wyMc = bi.name;
            wy.wyCzwydz = bi.address;
        }else{
            notification.error({message:'获取楼盘信息失败'})
        }
        url = `${WebApiConfig.project.getShop}${item.shopId}`
        r = await ApiClient.get(url);
        r = (r||{}).data||{};
        if(r.code==='0' && r.extension){
            let bi = r.extension.basicInfo||{};
            wy.wyWz = bi.buildingNo;
            wy.wyLc = bi.floorNo;
            wy.wyFh = bi.number;
            wy.wyZlc = bi.floors;
            wy.wyJzmj = bi.buildingArea;
            wy.wySymj = bi.houseArea;
            if(wy.wyJzmj){
                wy.wyWyJj =  Math.round(( newReport.cjzj / wy.wyJzmj)*100)/100;
            }
            
            let cxList = getDicPars(dicKeys.cx, this.props.dic);
            let item = cxList.find(x=>x.value === bi.toward);
            if(item){
                wy.wyCx = item.key;
            }
        }else{
            notification.error({message:'获取商铺信息失败'})
        }
        newReport.reportWy =  wy;

        let yz = newReport.reportYz || this.state.report.reportYz || {};
        yz = {...yz}
        yz.yzMc = wy.wyMc;
        newReport.reportYz = yz;

        let kh = newReport.reportKh || this.state.report.reportKh || {};
        kh = {...kh}
        kh.khMc = item.customerName;
        newReport.reportKh = kh;
        if(item.customerInfo){
            let csList = getDicPars(dicKeys.khly, this.props.dic);
            let cs = csList.find(x=>x.value === item.customerInfo.source);
            if(cs){
                kh.khKhly = cs.key;
            }
        }
        
        let yj = newReport.reportYjfp || this.state.report.reportYjfp || {};
            yj.yjYzyjdqr  = newReport.cjrq.days(180);
            yj.yjKhyjdqr= newReport.cjrq.days(180);
            yj.yjYzys = newReport.ycjyj;
            yj.yjKhys = 0;
            yj.yjZcjyj = yj.yjYzys + yj.yjKhys;
            newReport.reportYjfp = {...yj};
    
        console.log(newReport);

        this.setState({report: {...this.state.report,...newReport}})

        this.showBbDialog(false);
    }

    _getSubFormValues = ()=>{
        let values = this.jyhtForm.props.form.getFieldsValue();
        if(this.wyForm){
            values.reportWy = this.wyForm.props.form.getFieldsValue();
        }
        if(this.yzForm){
            values.reportYz = this.yzForm.props.form.getFieldsValue();
        }
        if(this.khForm){
            values.reportKh = this.khForm.props.form.getFieldsValue();
        }
        if(this.yjForm){
            values.reportYjfp = this.yjForm.props.form.getFieldsValue();
        }

        return values;
    }

    render() {
        return (
            <Layer>
                <div className="full" style={{overflow:'auto'}} ref={(node) => { this.container = node; }}>
                    {/* <div>
                        <Tooltip title="返回">
                            <Button type='primary' shape='circle' icon='arrow-left' style={{ 'margin': 10, float: 'left' }} onClick={this.props.handleback} />
                        </Tooltip>
                    </div> */}
                    <Spin spinning={this.state.isDataLoading}>
                        <Content style={{ overflowY: 'auto', height: '100%' }} >
                        <Affix offsetTop  target={() => this.container} style={{paddingTop:'1rem', paddingBottom:'1rem'}}>
                            <Button type="primary">保存</Button>
                            <Button type="primary" style={{marginLeft:'0.5rem'}}>提交审核</Button>
                            </Affix >
                            <Tabs defaultActiveKey="jyht" onChange={this.loadTabData}>
                                <TabPane tab="交易合同" key="jyht">
                                    <TradeContract 
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
                                        entity={this.state.report.reportWy} 
                                        wrappedComponentRef={(inst) => this.wyForm = inst}  
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}  
                                        rpId={this.state.rpId} 
                                        opType={this.state.opType}  />
                                </TabPane>
                                <TabPane tab="业主信息" key="yzxx">
                                    <TradeBnsOwner  
                                        entity={this.state.report.reportYz} 
                                        wrappedComponentRef={(inst) => this.yzForm = inst}  
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}  
                                        rpId={this.state.rpId} 
                                        opType={this.state.opType} />
                                </TabPane>
                                <TabPane tab="客户信息" key="khxx">
                                    <TradeCustomer  
                                        entity={this.state.report.reportKh} 
                                        wrappedComponentRef={(inst) => this.khForm = inst}  
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}  
                                        rpId={this.state.rpId} 
                                        opType={this.state.opType}  />
                                </TabPane>
                                <TabPane tab="业绩分配" key="yjfp">
                                    <TradePerDis  
                                        report={this.state.report}
                                        entity={this.state.report.reportYjfp} 
                                        wrappedComponentRef={(inst) => this.yjForm = inst}  
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}  
                                        rpId={this.state.rpId} 
                                        opType={this.state.opType}  
                                        />
                                </TabPane>
                                <TabPane tab="附件" key="fj">
                                    <TradeAttact rpId={this.state.rpId} opType={this.state.opType} />
                                </TabPane>
                                <TabPane tab="按揭过户" key="ajgh">
                                    <TradeTransfer 
                                        entity={this.state.report.reportGh} 
                                        wrappedComponentRef={(inst) => this.yjForm = inst}  
                                        showBbSelector={this.state.showBbSelector}
                                        dic={this.props.dic}
                                        rpId={this.state.rpId} 
                                        opType={this.state.opType} />
                                </TabPane>
                                {
                                    this.state.isEdit?(<TabPane tab="业绩调整" key="yjtz">
                                    <TradeAjust rpId={this.state.rpId} opType={this.state.opType} />
                                </TabPane>):null
                                }
                            </Tabs>
                        </Content>
                    </Spin>
                    <TradeReportTable visible={this.state.showBbDialog} onClose={this.showBbDialog} selectedCallback={this.bbChanged}/>
                </div>
            </Layer>
        )
    }
}
function MapStateToProps(state) {
    return {
        operInfo: state.rp.operInfo,
        shopInfo: state.rp.shopInfo,
        buildingInfo: state.rp.buildingInfo,
        dic: state.basicData.dicList,
        user: state.oidc.user.profile||{}
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch,
        getDicParList: (...args) => dispatch(getDicParList(...args))
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(TradeManager);