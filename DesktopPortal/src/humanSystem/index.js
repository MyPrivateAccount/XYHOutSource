import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {Layout, Menu, Icon, Button, Breadcrumb} from 'antd';
import {connect} from 'react-redux';
import reducers from './reducers';
import ContentPage from './pages/contentPage';
import {sagaMiddleware} from '../';
import rootSaga from './saga/rootSaga';
import {getOrgList, getOrgDetail, openOrgSelect, setbreadPageItem,closebreadPage, setbreadPageItemIndex} from './actions/actionCreator';
import OrgSelect from './pages/orgSelect/orgSelect';
sagaMiddleware.run(rootSaga);
const {Header, Sider, Content} = Layout;


const menuDefine = [
    {id: 20, menuID: "menu_user_mgr", displayName: "员工信息管理", menuIcon: 'contacts'},
    {id: 21, menuID: "menu_month", displayName: "月结", menuIcon: 'calendar'},
    {id: 22, menuID: "menu_black", displayName: "黑名单管理", menuIcon: 'lock'/*, requirePermission: ['PermissionItemCreate']*/},
    {id: 23, menuID: "menu_station", displayName: "职位和岗位配置", menuIcon: 'solution'},
    {id: 24, menuID: "menu_achievement", displayName: "职位薪酬管理", menuIcon: 'database'},
    {id: 25, menuID: "menu_attendance", displayName: "考勤信息", menuIcon: 'pushpin-o'},
    {id: 26, menuID: "menu_organization", displayName: "组织架构管理", menuIcon: 'layout'},
    {id: 27, menuID: "menu_statistics", displayName: "统计报表", menuIcon: 'global'},
    {id: 28, menuID: "menu_set", displayName: "设置", menuIcon: 'setting'},
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

 class HumanIndex extends Component {
    
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
                    // this.setState({
                    //     activeMenu: menuDefine[i]
                    // });
                    this.state.activeMenu = menuDefine[i];
                    this.props.dispatch(setbreadPageItem(menuDefine[i]));
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
                return <ContentPage curMenuID={navigator[navigator.length-1].menuID} />
            }
            return <ContentPage curMenuID={this.state.activeMenu.menuID} />;
        //     if (navigator.length > 0) {
        //         if (navigator[navigator.length - 1].id === 0) {
        //             return <ContentPage curMenuID='Onboarding' />;
        //         }
        //     }
        //    return <ContentPage curMenuID={this.state.activeMenu.menuID} />;
        }

        handleNavClick(i, itm) {
            let navigator = this.props.basicData.navigator;
            if(navigator.length > 0) {
                this.props.dispatch(setbreadPageItemIndex(i));
                // if(navigator[navigator.length -1].id === 0) {
                //     this.props.dispatch(closebreadPage(0));
                // }
            }
        }

        componentWillMount () {
            this.props.dispatch(getOrgList("PublicRoleOper"));
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
                            <Breadcrumb separator='>' style= {{fontSize:'0.8rem'}}> 
                                {
                                    navigator.map((item, i) =>{
                                        return <Breadcrumb.Item key={i}  style={homeStyle.navigator} onClick={(e) =>this.handleNavClick(i, item)} >{item.displayName}</Breadcrumb.Item>
                                    })
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
export default withReducer(reducers, 'HumanIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc})})(connect(mapStateToProps)(HumanIndex));