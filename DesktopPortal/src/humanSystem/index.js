import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {Layout, Menu, Icon, Button, Breadcrumb, notification, Checkbox} from 'antd';
import {connect} from 'react-redux';
import reducers from './reducers';
import ContentPage from './pages/contentPage';
import {sagaMiddleware} from '../';
import rootSaga from './saga/rootSaga';
import {getOrgList, setSearchLoadingVisible, searchConditionType, setbreadPageItem,closebreadPage, setbreadPageItemIndex} from './actions/actionCreator';
//import OrgSelect from './orgSelect';
sagaMiddleware.run(rootSaga);

const {SubMenu} = Menu;
const {Header, Sider, Content} = Layout;


const menuDefine = [
    {id: 20, menuID: "menu_user_mgr", displayName: "员工信息管理", menuIcon: 'contacts'},
    {id: 21, menuID: "menu_month", displayName: "月结", menuIcon: 'calendar'},
    {id: 22, menuID: "menu_black", displayName: "黑名单管理", menuIcon: 'lock'/*, requirePermission: ['PermissionItemCreate']*/},
    {id: 23, menuID: "menu_station", displayName: "职位和岗位配置", menuIcon: 'solution'},
    {id: 24, menuID: "menu_achievement", displayName: "薪酬管理", menuIcon: 'database'},
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
        marginRight: '10px',

    },
    curOrgStype:{
        marginLeft: '10px',
        overflow: 'hidden', 
        textOverflow: 'ellipsis'
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
        if (e.key === "menu_org_select") {
            return;
        }

        for (let i in menuDefine) {
            if (menuDefine[i].menuID == e.key) {
                if (e.key == "menu_organization" || e.key == "menu_statistics" || e.key == "menu_set") {
                    notification.error({
                        message: "no page",
                        duration: 3
                    });
                    return ;
                }
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
    }

    handleNavClick(i, itm) {
        let navigator = this.props.basicData.navigator;
        if(navigator.length > 0) {
            this.props.dispatch(setbreadPageItemIndex(i));
        }
    }

    componentWillMount () {
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
        let activeOrg = this.props.basicData.activeOrg || {};
        let orgList = orgtree;
        let fullPath = orgtree[0].organizationName;

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
            if (!orgInfo.children||orgInfo.children.length === 0) {
                return orgInfo.id;
            } else {
                return orgInfo.children.map(org => this.getChildOrg(org));
            }
        }
    }

    handleOrgChecked = (f, e) => {
        this.props.dispatch(setSearchLoadingVisible(true));

        this.props.basicData.headVisible = false;
        this.forceUpdate();
        this.props.search.organizate = f.id;
        if (f.children instanceof Array) {
            this.props.search.lstChildren = f.children.map(org => this.getChildrenID(org));
        }
        this.props.search.lstChildren = [];
        
        this.props.dispatch(searchConditionType(this.props.search));
    }

    getChildOrg(orgInfo) {
        if (orgInfo) {
            if (!orgInfo.children||orgInfo.children.length === 0) {
                return (<Menu.Item key={orgInfo.id} >
                    <Checkbox checked={false} onChange={(e) => this.handleOrgChecked(orgInfo, e)} >{orgInfo.organizationName}</Checkbox>
                </Menu.Item>)
            } else {
                return (<SubMenu key={orgInfo.id} title={<span><Checkbox checked={false} onChange={(e) => this.handleOrgChecked(orgInfo, e)}></Checkbox>&nbsp;&nbsp;{"  "+orgInfo.organizationName}</span>}>
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
        orgTree.unshift({id: "0", key: "0", label: "不限", name: "不限", organizationName:"不限", parentId: "0", value: "0"});
        let fullPath = this.getActiveOrgFullPath(orgTree);
        return(
            <Layout className = "page">
                <Sider
                    collapsible
                    collapsed={this.state.collapsed}
                    onCollapse={this.toggle}>
                    <div className="logo" />
                    <Menu 
                         theme="dark" key='menu_org_select' mode="vertical" style={{borderBottom: '1px solid #fff'}}>
                        <SubMenu  key="menu_org_select_sub" title={"当前部门："+fullPath}>
                            {this.props.basicData.headVisible?orgTree.map(org =>this.getChildOrg(org)):null}
                        </SubMenu>
                    </Menu>
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
                            {
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
    return {
        oidc: state.oidc,
        router: state.router,
        search: state.search,
        // search: state.search,
        basicData: state.basicData
    }
}
export default withReducer(reducers, 'HumanIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc})})(connect(mapStateToProps)(HumanIndex));