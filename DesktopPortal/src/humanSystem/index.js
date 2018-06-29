import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {Layout, Menu, Icon, Button, Breadcrumb, notification, Checkbox} from 'antd';
import {connect} from 'react-redux';
import reducers from './reducers';
import ContentPage from './pages/contentPage';
import {sagaMiddleware} from '../';
import rootSaga from './saga/rootSaga';
import {getOrgList, setSearchLoadingVisible, searchConditionType, setbreadPageItem, setVisibleHead, setbreadPageItemIndex} from './actions/actionCreator';
import {getDicParList} from '../actions/actionCreators';
import {globalAction} from 'redux-subspace';
import Layer from '../components/Layer'
import createHistory from 'history/createMemoryHistory'
import {ConnectedRouter} from 'react-router-redux'
import {Route} from 'react-router'
import Staffinfo from './pages/staffinfo/staffinfo'
import Month from './pages/month/month'
import Black from './pages/black/black'
import Station from './pages/station/station'
import Achievement from './pages/achievement/achievement'
import Attendance from './pages/attendance/attendance'
import Organization from './pages/organization/organization'
import Statistics from './pages/statistics/statistics'
import Set from './pages/set/set'
import Rewardpunishment  from './pages/rewardpunishment/rewardpunishment'
sagaMiddleware.run(rootSaga);

const {SubMenu} = Menu;
const {Header, Sider, Content} = Layout;

const menuDefine = [
    {id: 20, menuID: "menu_user_mgr", displayName: "员工信息管理", menuIcon: 'contacts', path: '/staff', par: {noQR: false, noGL: true, noFK: true, noAdd: true, }},
    {id: 21, menuID: "menu_month", displayName: "月结", menuIcon: 'calendar', path: '/month'},
    {id: 22, menuID: "menu_black", displayName: "黑名单管理", menuIcon: 'lock', path: '/black',/*, requirePermission: ['PermissionItemCreate']*/},
    {id: 23, menuID: "menu_station", displayName: "职位和岗位配置", menuIcon: 'solution', path: '/station'},
    {id: 24, menuID: "menu_achievement", displayName: "薪酬管理", menuIcon: 'database', path: '/achievement'},
    {id: 25, menuID: "menu_attendance", displayName: "考勤信息", menuIcon: 'pushpin-o', path: '/attendance'},
    {id: 26, menuID: "menu_organization", displayName: "组织架构管理", menuIcon: 'layout', path: '/organization'},
    // {id: 27, menuID: "menu_statistics", displayName: "统计报表", menuIcon: 'global', path: '/statistics'},
    {id: 27, menuID: "menu_awpu", displayName: "行政奖惩", menuIcon: 'global',path:'/rewardpunishment'},
    {id: 28, menuID: "menu_set", displayName: "设置", menuIcon: 'setting', path: '/set'},
    //{menuID: "menu_app", displayName: "应用管理", menuIcon: 'appstore', requirePermission: ['ApplicationCreate']}
];

// const homeStyle = {
//     navigator: {
//         cursor: 'pointer'
//     },
//     activeOrg: {
//         float: 'right',
//         marginRight: '10px',

//     },
//     curOrgStype: {
//         marginLeft: '10px',
//         overflow: 'hidden',
//         textOverflow: 'ellipsis'
//     }
// }

const history = createHistory();
let routeCallback = null;

history.listen((location, action) => {
    if (routeCallback) {
        routeCallback(location, action);
    }
})

class HumanIndex extends Component {

    state = {
        activeOrg: {},
        collapsed: false,
        activeMenu: menuDefine[0],
        menuList: []
    }
    componentDidMount() {
        const dicArray = ['HUMEN_Nation', 'HUMEN_HOUSE_REGISTER', 'HUMEN_EDUCATION', 'HUMENT_HEALTH', 'HUMEN_POLITICS', 'CONTRACT_CATEGORIES', 'HUMEN_DEGREE','POSITION_TYPE','HUMEN_EMP_STATUS'];
        this.props.dispatch(globalAction(getDicParList([...dicArray])));
        let ml = [];
        menuDefine.map(menu => {
            if (this.hasPermission(menu)) {
                ml.push(menu);
            }
        })
        if (ml.length > 0) {
            this.setState({menuList: ml, activeMenu: ml[0], showBack: false, title: ml[0].displayName}, () => {
                history.replace(this.state.activeMenu.path, {...this.state.activeMenu.par, menu: this.state.activeMenu})
            })
        }
        routeCallback = (location, action) => {
            this.setState({showBack: location.pathname !== this.state.activeMenu.path, title: ((location.state || {}).menu || {}).displayName})
        }
    }

    componentWillUnmount = () => {
        routeCallback = null;
    }

    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }

    handleMenuClick = (e) => {
        if (e.key === "menu_org_select") {
            return;
        }

        // for (let i in menuDefine) {
        //     if (menuDefine[i].menuID == e.key) {
        //         if (e.key == "menu_statistics" || e.key == "menu_set") {
        //             notification.error({
        //                 message: "no page",
        //                 duration: 3
        //             });
        //             return;
        //         }
        //         this.state.activeMenu = menuDefine[i];
        //         this.props.dispatch(setbreadPageItem(menuDefine[i]));
        //         break;
        //     }
        // }
        let menu = this.state.menuList.find(x => x.menuID === e.key);
        if (!menu) {
            notification.error({
                message: "no page",
                duration: 3
            });
            return;
        }
        history.entries.length = 0;
        this.setState({activeMenu: menu}, () => {
            history.replace(menu.path, {...menu.par, menu: menu})
        })
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
    // getContentPage() {
    //     let navigator = this.props.basicData.navigator;
    //     if (navigator.length > 0) {
    //         return <ContentPage curMenuID={navigator[navigator.length - 1].menuID} />
    //     }
    //     return <ContentPage curMenuID={this.state.activeMenu.menuID} />;
    // }

    handleNavClick(i, itm) {
        let navigator = this.props.basicData.navigator;
        if (navigator.length > 0) {
            this.props.dispatch(setbreadPageItemIndex(i));
        }
    }

    componentWillMount() {
        this.props.dispatch(getOrgList("PublicRoleOper"));
    }

    getParentOrg(orgList, orgId) {
        let org = null;
        if (orgList && orgList.length > 0) {
            for (let i = 0; i < orgList.length; i++) {
                if (orgList[i].id === orgId) {
                    org = orgList[i];
                    break;
                } else {
                    if (orgList[i].children && orgList[i].children.length > 0) {
                        let result = this.getParentOrg(orgList[i].children, orgId);
                        if (result) {
                            org = result;
                            break;
                        }
                    }
                }
            }
        }
        return org;
    }

    //获取当前选中部门的完整层级路径
    getActiveOrgFullPath(orgtree) {
        let activeOrg = this.state.activeOrg || {};
        let orgList = orgtree;
        let fullPath = activeOrg.organizationName || "";

        if (activeOrg.id !== '0' && activeOrg.parentId) {
            let parentOrg = this.getParentOrg(orgList, activeOrg.parentId);
            if (parentOrg) {
                fullPath = parentOrg.organizationName + ">" + fullPath;
            }
            while (parentOrg != null) {
                parentOrg = this.getParentOrg(orgList, parentOrg.parentId);
                if (parentOrg) {
                    fullPath = parentOrg.organizationName + ">" + fullPath;
                }
                else {
                    break;
                }
            }
        }
        return fullPath;
    }

    getChildrenID(orgInfo) {
        if (orgInfo) {
            if (!orgInfo.children || orgInfo.children.length === 0) {
                this.props.search.lstChildren.push(orgInfo.id);
            } else {
                this.props.search.lstChildren.push(orgInfo.id);
                orgInfo.children.forEach(org => this.getChildrenID(org));
            }
        }
    }

    handleOrgChecked = (f, e) => {
        if (e.target.checked) {
            this.state.activeOrg = {id: f.id, parentId: f.parentId, organizationName: f.organizationName};
            //this.props.dispatch(setVisibleHead({headVisible: false, activeOrg: {id:f.id, parentId:f.parentId}}));
            this.props.dispatch(setSearchLoadingVisible(true));

            this.props.search.organizate = f.id;
            this.props.search.lstChildren = [f.id];
            if (f.children instanceof Array) {
                f.children.forEach(org => this.getChildrenID(org));
            }
            this.props.dispatch(searchConditionType(this.props.search));
        }
    }

    getChildOrg(orgInfo) {
        if (orgInfo) {
            if (!orgInfo.children || orgInfo.children.length === 0) {
                return (<Menu.Item key={orgInfo.id} >
                    <Checkbox onChange={(e) => this.handleOrgChecked(orgInfo, e)} >{orgInfo.organizationName}</Checkbox>
                </Menu.Item>)
            } else {
                return (<SubMenu key={orgInfo.id} title={<span><Checkbox onChange={(e) => this.handleOrgChecked(orgInfo, e)}></Checkbox>&nbsp;&nbsp;{"  " + orgInfo.organizationName}</span>}>
                    {
                        orgInfo.children.map(org => this.getChildOrg(org))
                    }
                </SubMenu >)
            }
        }
    }

    render() {
        let navigator = this.props.basicData.navigator;

        let orgTree = this.props.basicData.searchOrgTree.slice();
        orgTree.unshift({id: "ff", key: "ff", label: "不限", name: "不限", organizationName: "不限", parentId: "ff", value: "ff"});
        let fullPath = this.getActiveOrgFullPath(orgTree);
        return (
            <Layout className="page">
                <Sider
                    collapsible
                    collapsed={this.state.collapsed}
                    onCollapse={this.toggle}>
                    <div className="logo" />
                    <Menu
                        theme="dark" key='menu_org_select' mode="vertical" style={{borderBottom: '1px solid #fff'}}>
                        <SubMenu title={"当前部门：" + fullPath}>
                            {orgTree.map(org => this.getChildOrg(org))}
                        </SubMenu>
                    </Menu>
                    <Menu mode="inline"
                        theme="dark"
                        onClick={this.handleMenuClick}
                        inlineCollapsed={this.state.collapsed}
                        selectedKeys={[this.state.activeMenu.menuID]}
                        defaultSelectedKeys={["menu_user_mgr"]}>
                        {menuDefine.map((menu, index) =>
                            this.hasPermission(menu) ? <Menu.Item key={menu.menuID}>
                                <Icon type={menu.menuIcon} />
                                <span>{menu.displayName}</span>
                            </Menu.Item> : null
                        )}
                    </Menu>
                </Sider>
                {
                    <Layout>
                        <Header>
                            {/* <Breadcrumb separator='>' style={{fontSize: '0.8rem'}}>
                                {
                                    navigator.map((item, i) => {
                                        return <Breadcrumb.Item key={i} style={homeStyle.navigator} onClick={(e) => this.handleNavClick(i, item)} >{item.displayName}</Breadcrumb.Item>
                                    })
                                }
                            </Breadcrumb> */}
                            <div onClick={() => history.goBack()} className="back-btn" style={{display: this.state.showBack ? 'inline-block' : 'none'}}>
                                <Icon type="left" /><span className="b-text">返回</span>
                            </div>
                            {
                                !this.state.showBack ? <div>{this.state.title}</div> : null
                            }
                        </Header>
                        <Content className='content'>
                            {/* {
                                this.getContentPage()
                            } */}
                            <ConnectedRouter history={history}>
                                <Layer>
                                    <Route path='/staff' component={Staffinfo} />
                                    <Route path='/month' component={Month} />
                                    <Route path='/black' component={Black} />
                                    <Route path='/station' component={Station} />
                                    <Route path='/achievement' component={Achievement} />
                                    <Route path='/attendance' component={Attendance} />
                                    <Route path='/organization' component={Organization} />
                                    {/* <Route path='/statistics' component={Statistics} /> 由于前端报错，暂时注释，等界面写了再取消注释 
                                    <Route path='/set' component={Set} />*/}
                                    <Route path='/rewardpunishment' component={Rewardpunishment}/>
                                </Layer>
                            </ConnectedRouter>
                        </Content>
                    </Layout>
                }
            </Layout>
        )
    }
}

function mapStateToProps(state, props) {
    return {
        oidc: state.oidc,
        router: state.router,
        search: state.search,
        // search: state.search,
        basicData: state.basicData
    }
}
export default withReducer(reducers, 'HumanIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc, judgePermissions: rootState.app.judgePermissions, rootBasicData: rootState.basicData})})(connect(mapStateToProps)(HumanIndex));