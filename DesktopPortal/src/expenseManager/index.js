import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {connect} from 'react-redux';
import reducers from './reducers/index';
import rootSaga from './saga/rootSaga';
import {sagaMiddleware} from '../';
import {Layout, Menu, Icon} from 'antd';
import createHistory from 'history/createMemoryHistory'
import {ConnectedRouter} from 'react-router-redux'
import {Route } from 'react-router'
import {AuthorUrl} from '../constants/baseConfig'
import ApiClient from '../utils/apiClient'
import ChargeIndex from './pages/fylr'
import BorrowingIndex from './pages/fylr/BorrowingIndex'
import DetailReport from './pages/fylr/DetailRepor'
import FeeLimitSetting from './pages/fylr/FeeLimitSetting'
import Layer from '../components/Layer'
import {permission, chargeStatus} from './pages/fylr/const'
import './index.less'

const {Header, Sider, Content} = Layout;
sagaMiddleware.run(rootSaga);

const history = createHistory();
let routeCallback =null;

history.listen((location,action)=>{
    if(routeCallback){
        routeCallback(location,action);
    }
})

const homeStyle = {
    navigator: {
        cursor: 'pointer'
    },
    activeOrg: {
        float: 'right',
        marginRight: '10px',

    },
    curOrgStype:{
        marginLeft: '10px',
        overflow: 'hidden', 
        textOverflow: 'ellipsis'
    }
}

class ExpenseManagerIndex extends Component {
    constructor(props){
        super(props);

        this.state = {
            collapsed: false,
            activeMenu: {},
            showBack:false,
            title:'',
            menuList:[]
        }

        this.handleMenuItemClick = this.handleMenuItemClick.bind(this);
    }
    
    componentDidMount=()=>{
        this.getPermission();
        routeCallback=(location, action)=>{
            
            this.setState({showBack: location.pathname!==this.state.activeMenu.path, title:((location.state||{}).menu||{}).displayName})
        }
        
    }

    getPermission = async ()=>{
        let url = `${AuthorUrl}/api/Permission/each`
        let r = await ApiClient.post(url, [permission.qr, permission.fk, permission.gl, permission.mxb, permission.hzb, permission.bxxe, permission.yjk, permission.yjkgl, permission.yjkqr, permission.yjkfk])
        let p = {};
        if( r && r.data && r.data.code==='0'){
            
            (r.data.extension||[]).forEach(pi=>{
                if(pi.permissionItem===permission.qr){
                    p.qr = pi.isHave;
                }else if(pi.permissionItem===permission.fk){
                    p.fk = pi.isHave;
                }else if(pi.permissionItem===permission.gl){
                    p.gl = pi.isHave;
                }else if(pi.permissionItem===permission.mxb){
                    p.mxb = pi.isHave;
                }else if(pi.permissionItem===permission.hzb){
                    p.hzb = pi.isHave;
                }else if(pi.permissionItem===permission.bxxe){
                    p.bxxe = pi.isHave;
                }
                else if(pi.permissionItem === permission.yjk){
                    p.yjk = pi.isHave;
                }else if(pi.permissionItem === permission.yjkgl){
                    p.yjkgl = pi.isHave;
                }else if(pi.permissionItem === permission.yjkqr){
                    p.yjkqr = pi.isHave;
                }else if(pi.permissionItem === permission.yjkfk){
                    p.yjkfk = pi.isHave;
                }
            })
            this.setState({permission: p}) 
        }

        let ml = [];
        ml.push({menuID:'fybx', displayName: '费用报销', menuIcon:'contacts', type:'item', path:'/lr', par:{noQR:true, noFK:true}})
        if(p.qr){
            ml.push({menuID:'bxdqr', displayName: '报销单确认', menuIcon:'contacts', type:'item', path:'/qr', 
            par:{
                noQR:false,
                noGL:true, 
                noFK:true, 
                noAdd:true, 
                status: [chargeStatus.Submit, chargeStatus.Confirm]}})
        }
        if(p.fk){
            ml.push({menuID:'fkqr', displayName: '付款确认', menuIcon:'contacts', type:'item', path:'/fk', par:{noQR:true,noGL:true, noFK:false, noAdd:true, status: [chargeStatus.Confirm]}})
        }
        if(p.mxb){
            ml.push({menuID:'mxb', displayName: '费用明细表', menuIcon:'contacts', type:'item', path:'/mxb', par:{}})
        }
       
        ml.push({menuID:'split'})

        if(p.yjk || p.yjkgl){
            ml.push({menuID:'yjk', displayName: '预借款/还款', menuIcon:'contacts', type:'item', path:'/yjk', par:{noQR:true,noGL: false, noFK:true, noAdd:false, status: []}})
        }
        if(p.yjkqr){
            ml.push({menuID:'yjkqr', displayName: '预借款/还款确认', menuIcon:'contacts', type:'item', path:'/yjkqr', 
            par:{
                noQR:false,
                noGL:true, 
                noFK:true, 
                noAdd:true, 
                status: [chargeStatus.Submit, chargeStatus.Confirm]}})
        }

        if(p.yjkfk){
            ml.push({menuID:'yjkfk', displayName: '预借款财务确认', menuIcon:'contacts', type:'item', path:'/yjkfk', 
            par:{
                noQR:true,
                noGL:true, 
                noFK:false, 
                noAdd:true, 
                status: [ chargeStatus.Confirm]}})
        }
       
       
        if(p.bxxe){
            ml.push({menuID:'split'})
            ml.push({menuID:'bxxe', displayName: '报销限额设置', menuIcon:'contacts', type:'item', path:'/bxxe', par:{}})
        }

        this.setState({menuList: ml,activeMenu: ml[0],showBack:false, title: ml[0].displayName},()=>{
            console.log(this.state.activeMenu)
            history.replace(this.state.activeMenu.path, {...this.state.activeMenu.par, menu: this.state.activeMenu})
        })

      

    }

    componentWillUnmount=()=>{
        routeCallback=null;
    }
    
    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }
 
    handleMenuItemClick = (e) => {
        let menu = this.state.menuList.find(x=>x.menuID === e.key);
        if(!menu){
            return;
        }
        history.entries.length = 0;
        this.setState({activeMenu: menu},()=>{
            history.replace(menu.path, {...menu.par, menu: menu})
        })

      }


    goBack=()=>{
        history.goBack();
    }

    render(){

        return (
           
            <Layout className="page">
            
                <Sider
                    collapsible
                    collapsed={this.state.collapsed}
                    onCollapse = {this.toggle}
                >    <div className="logo" />
                    <Menu
                        theme="dark"
                        defaultSelectedKeys = {[this.state.activeMenu.menuID]}
                        selectedKeys={[this.state.activeMenu.menuID]}
                        mode="inline"
                        onClick={this.handleMenuItemClick}
                    >
                    {
                        this.state.menuList.map(mi=>{
                            if(mi.menuID==='split'){
                                return <Menu.Divider />
                            }
                            return  <Menu.Item key={mi.menuID} ><span><Icon type={mi.menuIcon} /><span>{mi.displayName}</span></span></Menu.Item>;
                        })
                     
                    }
                    </Menu>
                </Sider>
                <Layout>
                    <Header>
                        <div onClick={this.goBack} className="back-btn" style={{display: this.state.showBack?'inline-block':'none'}}>
                            <Icon type="left" /><span className="b-text">返回</span>
                        </div> 
                        {
                            !this.state.showBack? <div>{this.state.title}</div>:null
                        }
                    </Header>
                    
                    <Content className='content'>
                        <ConnectedRouter history={history}>
                            <Layer>
                                <Route  path='/lr' component={ChargeIndex}/>
                                <Route  path='/qr' component={ChargeIndex}/>
                                <Route  path='/fk' component={ChargeIndex}/>
                                <Route  path='/mxb' component={DetailReport}/>
                                <Route  path='/bxxe' component={FeeLimitSetting}/>
                                <Route  path='/yjk' component={BorrowingIndex}/>
                                <Route  path='/yjkqr' component={BorrowingIndex}/>
                                <Route  path='/yjkfk' component={BorrowingIndex}/>
                            </Layer>
                        </ConnectedRouter>
                    </Content>
                   
                </Layout>
                
            </Layout>
          
        );
    }
}



function mapStateToProps(state){
    return{
        navigator: state.basicData.navigator,
    }
}
export default withReducer(reducers, 'ExpenseManagerIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc,judgePermissions: rootState.app.judgePermissions, basicData: rootState.basicData})})(connect(mapStateToProps)(ExpenseManagerIndex));