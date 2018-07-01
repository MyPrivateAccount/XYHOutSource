//外部框架组件，负责菜单显示，动态加载对应菜单项，每个菜单项都是一个单独的组件
//UI 组件用的是ant design组件，数据中间件saga,状态管理redux
import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { Layout, Menu, Icon, Button } from 'antd'
import createHistory from 'history/createMemoryHistory'
import { ConnectedRouter } from 'react-router-redux'
import { Route } from 'react-router'
import { connect } from 'react-redux'
import reducers from './reducers'
import { sagaMiddleware } from '../'
import rootSaga from './saga/rootSaga';
import Layer from '../components/Layer'
import { permission } from './constants/const'
import WebApiConfig from './constants/webApiConfig'
import ApiClient from '../utils/apiClient'
import {getDicParList} from '../actions/actionCreators'

import {
    LoadableAcmentPage, //业绩分摊项设置
    LoadablePeopleSetPage, //人数分摊组织设置
    LoadableInComeScaleSetPage, //分成比例设置
    LoadableOrgParamSetPage, //组织参数设置

    LoadableDealRpPage, //我录入的成交报告
    LoadableDealRpQueryPage, //我录入的成交报告

    LoadableMonthPage, //月结
    LoadablePPFTPage, //人员分摊
    LoadableYFTCPage, //应发提成
    LoadableSFTCPage, //实发提成
    LoadableTCCBPage,//提成成本
    LoadableYFTCCJPage, //应发提成冲减
    LoadableLZRYYJPage, //离职人员业绩
    LoadableSFKJQRJPage, //实发扣减确认

    LoadableFYXQBPage, //分佣详情表
    LoadableYJTZHZPage, //业绩调整明细汇总表
    LoadableTYXQPage, //调整佣

} from './pages/contentPage'


const { Header, Sider, Content } = Layout;

const history = createHistory();
let routeCallback = null;

history.listen((location, action) => {
    if (routeCallback) {
        routeCallback(location, action);
    }
})

const { SubMenu } = Menu;

const menuDefine = [
    {
        menuID: "menu_tranrp",
        displayName: "成交报告",
        menuIcon: 'contacts',
        type: 'subMenu',
        menuItems: [
            { menuID: "menu_myrp",path:'/myreport',  permissionKeys: [permission.myReport], displayName: "我录入的成交报告", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_query",path:'/reportquery',  permissionKeys: [permission.reportQuery], displayName: "成交报告综合查询", menuIcon: 'contacts', type: 'item' }
        ]
    },
    {
        menuID: "menu_fina",
        displayName: "财务",
        menuIcon: 'appstore-o',
        type: 'subMenu',
        menuItems: [
            { menuID: "menu_sumbymonth", path:'/yj', permissionKeys: [permission.monthlyClosing], displayName: "月结", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_ps",path:'/ryft', permissionKeys: [permission.ryftTable], displayName: "人员分摊表", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_yftcb", path:'/yftc', permissionKeys: [permission.yftcTable], displayName: "应发提成表", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_sftcb", path:'/sftc', permissionKeys: [permission.sftcTable], displayName: "实发提成表", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_tccbftb", path:'/tccb', permissionKeys: [permission.tccbTable], displayName: "提成成本表", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_yftccjb", path:'/yftccj', permissionKeys: [permission.yfcbcjTable], displayName: "应发提成冲减表", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_lzryyjqrb", path:'/lzryyj', permissionKeys: [permission.lzryyjqrTable], displayName: "离职人员业绩确认表", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_sfkjqrb", path:'/sfkj', permissionKeys: [permission.sfkjqrTable], displayName: "实发扣减确认表", menuIcon: 'contacts', type: 'item' },
        ]
    },
    {
        menuID: "menu_rpt",
        displayName: "报表",
        menuIcon: 'appstore-o',
        type: 'subMenu',
        menuItems: [
            { menuID: "menu_fyxcb", path:'/fyxq', permissionKeys: [permission.fyxqTable], displayName: "分佣详情表", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_yjtzmxb", path:'/yjtzmxhz', permissionKeys: [permission.yjtzmxhzTable], displayName: "业绩调整明细汇总表", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_tymxb", path:'/tyxq', permissionKeys: [permission.tymxTable], displayName: "调佣明细表", menuIcon: 'contacts', type: 'item' }
        ]
    },
    {
        menuID: "menu_bset",
        displayName: "基本设置",
        menuIcon: 'appstore-o',
        type: 'subMenu',
        menuItems: [
            { menuID: "menu_yjftxsz", path: '/sz_yjft', permissionKeys: [permission.allocationQuery], displayName: "业绩分摊项设置", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_rsftzzsz", path: '/sz_rsft', permissionKeys: [permission.shareQuery], displayName: "人数分摊组织设置", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_tcblsz", path: '/sz_fcbl', permissionKeys: [permission.ruleQuery], displayName: "提成比例设置", menuIcon: 'contacts', type: 'item' },
            { menuID: "menu_zzcssz", path: '/sz_zzcs', permissionKeys: [permission.parSetQuery], displayName: "组织参数设置", menuIcon: 'contacts', type: 'item' }
        ]
    }
];

sagaMiddleware.run(rootSaga);

class CommissionManagerIndex extends Component {
    constructor(props) {
        super(props);
        this.state = {
            collapsed: false,
            showBack: false,
            openKeys:[],
            title: '',
            menuList: [],
            permission: {},
            activeMenu: menuDefine[0].menuItems[0],
        }

        this.handleMenuClick = this.handleMenuClick.bind(this);
    }

    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }

    componentDidMount = () => {
        this.getPermission();
        routeCallback = (location, action) => {

            this.setState({ showBack: location.pathname !== this.state.activeMenu.path, title: ((location.state || {}).menu || {}).displayName })
        }

    }


    getPermission = async () => {
        let url = WebApiConfig.privilege.Check;
        let ps = [];
        let p = {};
        let map = [];
        for (let k in permission) {
            if (permission.hasOwnProperty(k)) {
                ps.push(permission[k]);
                p[k] = false;
                map.push({ key: k, value: permission[k] })
            }
        }
        let r = await ApiClient.post(url, ps)
        if (r && r.data && r.data.code === '0') {

            (r.data.extension || []).forEach(pi => {
                let m = map.find(x => x.value === pi.permissionItem)
                if (m) {
                    p[m.key] = pi.isHave
                }
            })
            this.setState({ permission: p })
        }



        let ml = [];
        let activeMenu = null;
        let parentMenu = null;
        menuDefine.forEach(mi => {
            let nm = this.filterMenu(p, map, mi);
            if ((nm.hasPermission && nm.type !== 'subMenu') || nm.menuItems.length > 0) {
                ml.push(nm);
                if(!activeMenu){
                    if(nm.type==="subMenu"){
                        activeMenu = nm.menuItems[0]
                    }else{
                        activeMenu = nm;
                    }
                    parentMenu = nm;
                }
            }
        })

        this.setState({ menuList: ml, activeMenu: activeMenu, showBack: false, title: activeMenu.displayName, openKeys:[parentMenu.menuID] }, () => {
            history.replace(this.state.activeMenu.path, { ...this.state.activeMenu.par, menu: this.state.activeMenu })
        })
    }

    filterMenu = (p, pm, menu) => {
        let newMenu = { ...menu }
        newMenu.menuItems = [];
        newMenu.hasPermission = this.checkMenuPermission(menu, pm, p);
        if (menu.menuItems && menu.menuItems.length > 0) {
            menu.menuItems.forEach(mi => {
                let nm = this.filterMenu(p, pm, mi);
                if (nm.menuItems.find(x => x.hasPermission)) {
                    nm.hasPermission = true;
                }

                nm.menuItems.forEach(mi => {
                    if (mi.hasPermission) {
                        nm.menuCount++;
                    }
                    nm.menuCount = nm.menuCount + (mi.menuCount || 0)
                })

                if (nm.hasPermission || nm.menuCount > 0) {
                    newMenu.menuItems.push(nm);
                }
            })

        }
        return newMenu;
    }

    checkMenuPermission = (mi, pm, p) => {
        let hasP = true;
        if (mi.permissionKeys && mi.permissionKeys.length > 0) {
            for (let i = 0; i < mi.permissionKeys.length; i++) {
                let k = mi.permissionKeys[i]
                let pkey = pm.find(x => x.value === k);
                if (pkey) {
                    if (!p[pkey.key]) {
                        hasP = false;
                        break;
                    }
                } else {
                    hasP = false;
                    break;
                }
            }
        }
        return hasP;
    }

    componentWillMount = () => {
        console.log("commission 当前用户所在部门:", this.props.oidc);
        let userInfo = this.props.oidc.user.profile;
        console.log("commission 当前用户信息:" + userInfo)
        routeCallback = null;
    }
    handleMenuClick = (e) => {
        console.log('click ', e);
        let find = false;
        let am = null;
        for (let i in menuDefine) {
            for (let j in menuDefine[i].menuItems) {
                if (menuDefine[i].menuItems[j].menuID == e.key) {
                    am = menuDefine[i].menuItems[j];
                    break;
                }
            }
            if (find) {
                break;
            }
        }
        if (am) {
            history.entries.length = 0;
            this.setState({ activeMenu: am }, () => {
                history.replace(am.path, { ...am.par, menu: am })
            })
        }
    }
    onMenuOpenChange =(keys)=>{
        this.setState({openKeys: keys})
    }

    goBack = () => {
        history.goBack();
    }

    render() {
        let isShowHeader = true;
        let className = isShowHeader ? 'content' : '';
        return (
            <Layout className="page">
                <Sider
                    collapsible
                    collapsed={this.state.collapsed}
                    onCollapse={this.toggle}>
                    <Menu mode="inline"
                        theme="dark"
                        className="left-menu"
                        onClick={this.handleMenuClick}
                        inlineCollapsed={this.state.collapsed}
                        defaultSelectedKeys = {[this.state.activeMenu.menuID]}
                        selectedKeys={[this.state.activeMenu.menuID]}
                        openKeys={this.state.openKeys}
                        onOpenChange={this.onMenuOpenChange}>
                        {this.state.menuList.map((submenu, i) =>
                            <SubMenu key={submenu.menuID} title={<span><Icon type={submenu.menuIcon} /><span>{submenu.displayName}</span></span>}>
                                {
                                    submenu.menuItems.map((menu, j) =>
                                        <Menu.Item key={menu.menuID}>
                                            <Icon type={menu.menuIcon} />
                                            <span>{menu.displayName}</span>
                                        </Menu.Item>
                                    )}
                            </SubMenu>
                        )}
                    </Menu>

                </Sider>
                <Layout>
                    < Header >
                        <Button type="primary" onClick={this.goBack} style={{ display: this.state.showBack ? 'inline-block' : 'none' }}>
                            <Icon type="left" />返回
                        </Button>

                        {
                            !this.state.showBack ? <div>{this.state.title}</div> : null
                        }
                    </ Header>

                    <Content className={className}>
                        <ConnectedRouter history={history}>
                            <Layer>
                                <Route path='/sz_yjft' render={(props) => <LoadableAcmentPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/sz_rsft' render={(props) => <LoadablePeopleSetPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/sz_fcbl' render={(props) => <LoadableInComeScaleSetPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/sz_zzcs' render={(props) => <LoadableOrgParamSetPage user={this.props.user}judgePermissions={this.props.judgePermissions} {...props} />} />

                                <Route path='/yj' render={(props) => <LoadableMonthPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/ryft' render={(props) => <LoadablePPFTPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/yftc' render={(props) => <LoadableYFTCPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/sftc' render={(props) => <LoadableSFTCPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/tccb' render={(props) => <LoadableTCCBPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/yftccj' render={(props) => <LoadableYFTCCJPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/lzryyj' render={(props) => <LoadableLZRYYJPage user={this.props.user} showSearch={true} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/sfkj' render={(props) => <LoadableSFKJQRJPage user={this.props.user} showSearch={true} judgePermissions={this.props.judgePermissions} {...props} />} />
                                
                                <Route path='/fyxq' render={(props) => <LoadableFYXQBPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/yjtzmxhz' render={(props) => <LoadableYJTZHZPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/tyxq' render={(props) => <LoadableTYXQPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />

                                <Route path='/myreport' render={(props) =>  <LoadableDealRpPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />
                                <Route path='/reportquery' render={(props) => <LoadableDealRpQueryPage user={this.props.user} judgePermissions={this.props.judgePermissions} {...props} />} />

                            </Layer>
                        </ConnectedRouter>
                    </Content>
                </Layout>
            </Layout>

        )
    }
}
function mapStateToProps(state, props) {

    return {
        judgePermissions: state.judgePermissions,
        oidc: state.oidc,
        user: state.oidc.user,
        dic: state.basicData.dicList,
    }
}
function mapDispatchToProps(dispatch) {
    return {
        dispatch,
        getDicParList: (...args) => dispatch(getDicParList(...args))
    }
}
export default withReducer(reducers, 'CommissionManagerIndex', { mapExtraState: (state, rootState) => ({ oidc: rootState.oidc, judgePermissions: rootState.app.judgePermissions, basicData: rootState.basicData }) })(connect(mapStateToProps, mapDispatchToProps)(CommissionManagerIndex));