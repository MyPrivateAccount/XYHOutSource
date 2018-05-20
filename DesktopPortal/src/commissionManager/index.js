//外部框架组件，负责菜单显示，动态加载对应菜单项，每个菜单项都是一个单独的组件
//UI 组件用的是ant design组件，数据中间件saga,状态管理redux
import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer'
import {Layout, Menu, Icon} from 'antd'
import {connect} from 'react-redux'
import reducers from './reducers'
import {sagaMiddleware} from '../'
import rootSaga from './saga/rootSaga';
import ContentPage from './pages/contentPage'


const {Header, Sider, Content} = Layout;
const { SubMenu } = Menu;

const menuDefine = [
    {
        menuID:"menu_tranrp",
        displayName:"成交报告",
        menuIcon:'contacts',
        type:'subMenu',
        menuItems:[
            {menuID:"menu_myrp",displayName:"我录入的成交报告",menuIcon:'contacts',type:'item'},
            {menuID:"menu_query",displayName:"成交报告综合查询",menuIcon:'contacts',type:'item'}
        ]
    },
    {
        menuID:"menu_fina",
        displayName:"财务",
        menuIcon:'appstore-o',
        type:'subMenu',
        menuItems:[
            {menuID:"menu_sumbymonth",displayName:"月结",menuIcon:'contacts',type:'item'},
            {menuID:"menu_ps",displayName:"人员分摊表",menuIcon:'contacts',type:'item'},
            {menuID:"menu_yftcb",displayName:"应发提成表",menuIcon:'contacts',type:'item'},
            {menuID:"menu_sftcb",displayName:"实发提成表",menuIcon:'contacts',type:'item'},
            {menuID:"menu_tccbftb",displayName:"提成成本分摊表",menuIcon:'contacts',type:'item'},
        ]
    },
    {
        menuID:"menu_rpt",
        displayName:"报表",
        menuIcon:'appstore-o',
        type:'subMenu',
        menuItems:[
            {menuID:"menu_fyxcb",displayName:"分佣详情表",menuIcon:'contacts',type:'item'},
            {menuID:"menu_yjtzmxb",displayName:"业绩调整明细汇总表",menuIcon:'contacts',type:'item'},
            {menuID:"menu_tymxb",displayName:"调佣明细表",menuIcon:'contacts',type:'item'}
        ]
    },
    {
        menuID:"menu_bset",
        displayName:"基本设置",
        menuIcon:'appstore-o',
        type:'subMenu',
        menuItems:[
            {menuID:"menu_yjftxsz",displayName:"业绩分摊项设置",menuIcon:'contacts',type:'item'},
            {menuID:"menu_rsftzzsz",displayName:"人数分摊组织设置",menuIcon:'contacts',type:'item'},
            {menuID:"menu_tcblsz",displayName:"提成比例设置",menuIcon:'contacts',type:'item'},
            {menuID:"menu_zzcssz",displayName:"组织参数设置",menuIcon:'contacts',type:'item'}
        ]
    }
];

sagaMiddleware.run(rootSaga);

class CommissionManagerIndex extends Component {

    state = {
        collapsed: false,
        activeMenu: menuDefine[0].menuItems[0],
    }
    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }
    handleMenuClick = (e) => {
        console.log('click ', e);
        let find = false;
        for (let i in menuDefine) {
            for(let j in menuDefine[i].menuItems){
                if (menuDefine[i].menuItems[j].menuID == e.key) {
                    this.setState({
                        activeMenu: menuDefine[i].menuItems[j]
                    });
                    find = true;
                    break;
                }
            }
            if(find){
                break;
            }
        }
    }
    //是否有权限
    hasPermission(menuInfo) {
        let hasPermission = false;
        if (this.props.judgePermissions && menuInfo.requirePermission) {
            for (let i = 0; i < menuInfo.requirePermission.length; i++) {
                if (this.props.judgePermissions.includes(menuInfo.requirePermission[i])) {
                    hasPermission = true;
                    break;
                }
            }
        } else {
            hasPermission = true;
        }
        return hasPermission;
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
                        onClick={this.handleMenuClick}
                        inlineCollapsed={this.state.collapsed}
                        defaultSelectedKeys={["menu_myrp"]}
                        defaultOpenKeys={['menu_tranrp']}>
                        {menuDefine.map((submenu, i) =>
                            <SubMenu key={submenu.menuID} title={<span><Icon type={submenu.menuIcon} /><span>{submenu.displayName}</span></span>}>
                            {
                            submenu.menuItems.map((menu,j)=>
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
                    {
                        isShowHeader ? < Header > {this.state.activeMenu.displayName}</Header> : null
                    }
                    <Content className={className}>
                         <ContentPage curMenuID={this.state.activeMenu.menuID} />
                    </Content>
                </Layout>
            </Layout>

        )
    }
}
function mapStateToProps(state, props) {
    
    return {
        judgePermissions: state.judgePermissions
    }
}
function mapDispatchToProps(dispatch) {
    return {
        dispatch
    }
}
export default withReducer(reducers, 'CommissionManagerIndex', { mapExtraState: (state, rootState) => ({ oidc: rootState.oidc }) })(connect(mapStateToProps, mapDispatchToProps)(CommissionManagerIndex));