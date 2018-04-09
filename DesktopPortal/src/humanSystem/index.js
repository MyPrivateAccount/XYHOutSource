import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {Layout, Menu, Icon, Button, Breadcrumb} from 'antd';
import {connect} from 'react-redux';
import reducers from './reducers';
import ContentPage from './pages/contentPage';
import {sagaMiddleware} from '../';
import rootSaga from './saga/rootSaga';
import {getOrgList, getOrgDetail, openOrgSelect, changeCustomerMenu,closebreadPage} from './actions/actionCreator';
import OrgSelect from './pages/orgSelect/orgSelect';
sagaMiddleware.run(rootSaga);
const {Header, Sider, Content} = Layout;


const menuDefine = [
    {menuID: "menu_user_mgr", displayName: "员工信息管理", menuIcon: 'contacts'},
    {menuID: "menu_month", displayName: "月结", menuIcon: 'calendar'},
    {menuID: "menu_attendance", displayName: "考勤信息", menuIcon: 'pushpin-o'},
    {menuID: "menu_statistics", displayName: "统计报表", menuIcon: 'global'},
    {menuID: "menu_black", displayName: "黑名单管理", menuIcon: 'lock'/*, requirePermission: ['PermissionItemCreate']*/},
    {menuID: "menu_station", displayName: "职位和岗位配置", menuIcon: 'solution'},
    {menuID: "menu_achievement", displayName: "职位薪酬管理", menuIcon: 'database'},
    {menuID: "menu_organization", displayName: "组织架构管理", menuIcon: 'layout'},
    {menuID: "menu_set", displayName: "设置", menuIcon: 'setting'},
    //{menuID: "menu_app", displayName: "应用管理", menuIcon: 'appstore', requirePermission: ['ApplicationCreate']}
];

const homeStyle = {
    navigator: {
        cursor: 'pointer'
    },
    activeOrg: {
        float: 'right',
        marginRight: '10px'
    }
}

 class PersonnelSystemIndex extends Component {
    
        state = {
            collapsed: false,
            activeMenu: menuDefine[0],
        }
        toggle = () => {
            this.setState({
                collapsed: !this.state.collapsed,
            });
        }
        handleMenuClick = (e) => {
            console.log('click ', e);
            for (let i in menuDefine) {
                if (menuDefine[i].menuID == e.key) {
                    this.setState({
                        activeMenu: menuDefine[i]
                    });
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
        //当前页面区域替换
        getContentPage() {
            let navigator = this.props.basicData.navigator;
            if (navigator.length > 0) {
                if (navigator[navigator.length - 1].id === 0) {
                    return <ContentPage curMenuID='Onboarding' />;
                }
            }
           return <ContentPage curMenuID={this.state.activeMenu.menuID} />;
        }

        handleNavClick() {
            let navigator = this.props.basicData.navigator;
            if(navigator.length > 0) {
                if(navigator[navigator.length -1].id === 0) {
                    this.props.dispatch(closebreadPage(0));
                }
            }
        }

    render() {
        let navigator = this.props.basicData.navigator;
        //let showHeaderMenuIDs = menuDefine.map((menu)=> {return menu.menuID});//[menuDefine[0].menuID,menuDefine[1].menuID, menuDefine[2].menuID,menuDefine[0].menuID];
        //let isShowHeader = (showHeaderMenuIDs.find((menuID) => menuID == this.state.activeMenu.menuID) != undefined);
        //let className = isShowHeader ? 'content' : '';
        return(
            <Layout className = "page">
                <Sider
                    collapsible
                    collapsed={this.state.collapsed}
                    onCollapse={this.toggle}>
                    <div className="logo" />
                    <Menu mode="inline"
                        theme="dark"
                        onClick={this.handleMenuClick}
                        inlineCollapsed={this.state.collapsed}
                        selectedKeys={[this.state.activeMenu.menuID]}
                        defaultSelectedKeys={["menu_user_mgr"]}>
                          {menuDefine.map((menu, index)=>
                            this.hasPermission(menu) ? <Menu.Item key={menu.menuID}>
                                <Icon type={menu.menuIcon}/>
                                <span>{menu.displayName}</span>
                            </Menu.Item> : null
                        )}
                    </Menu>
                </Sider>
                {
                    this.props.showOrgSelect ? <OrgSelect /> :
                        <Layout>
                            <Header>
                                <Breadcrumb separator=">" style={{fontSize: '1.2rem'}}>
                                    <Breadcrumb.Item onClick={this.handleNavClick.bind(this)} key='home' style={homeStyle.navigator}>{this.state.activeMenu.displayName}</Breadcrumb.Item>
                                    {
                                        navigator.map(nav => <Breadcrumb.Item key={nav.id}>{nav.name}</Breadcrumb.Item>)
                                    }
                                </Breadcrumb>
                            </Header>
                            <Content className='content'>
                                {/* {
                                    navigator.length > 0 ? <CustomerDetail /> : <ContentPage curMenuID={this.state.activeMenu.menuID} />
                                } */
                                    this.getContentPage()
                                }
                            </Content>
                        </Layout>
                }
            </Layout>
        )
    }
}

function mapStateToProps(state, props) {
    //console.log("权限管理mapStateToProps:" + JSON.stringify(props));
    return {
        oidc: state.oidc,
        router: state.router,
        search: state.search,
        basicData: state.basicData
    }
}
export default withReducer(reducers, 'PersonnelSystemIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc})})(connect(mapStateToProps)(PersonnelSystemIndex));