import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {Layout, Menu, Icon, Button, Breadcrumb} from 'antd';
import {connect} from 'react-redux';
import reducers from './reducers';
import ContentPage from './pages/contentPage';
import {sagaMiddleware} from '../';
import rootSaga from './saga/rootSaga';
import {closeAttachMent, closeContractReord, getOrgList, getOrgDetail, openOrgSelect,closeOrgSelect, changeContractMenu} from './actions/actionCreator';
import OrgSelect from './pages/orgSelect/orgSelect';

import AttchMent from './pages/attachMent';

sagaMiddleware.run(rootSaga);

const {Header, Sider, Content} = Layout;
const menuDefine = [
    {menuID: "menu_index", displayName: "合同信息", menuIcon: 'contacts'},
    //{menuID: "menu_renew", displayName: "合同续签", menuIcon: 'contacts'},

    //{ menuID: "menu_analysis", displayName: "客户业态分析", menuIcon: 'pie-chart' }//租壹屋
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

class ContractManagementIndex extends Component {
    state = {
        collapsed: false,
        activeMenu: menuDefine[0],
    }
    componentWillMount() {
        console.log("当前用户所在部门:", this.props.oidc);
        let userInfo = this.props.oidc.user.profile;
        this.props.dispatch(getOrgList(userInfo.Organization));
        if (userInfo.Organization !== '0') {
            this.props.dispatch(getOrgDetail(userInfo.Organization));
        }
        this.props.dispatch(closeOrgSelect());
    }
    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }

    handleMenuClick = (e) => {
        // console.log("点击菜单:", e.key);
        //if (e.key === this.state.activeMenu.menuID) return;
        if (e.key === "menu_org_select") {//这个是展开组织结构的菜单后面可能会用到
            this.props.dispatch(openOrgSelect());
            return;
        }
        this.props.dispatch(changeContractMenu(e.key));
        for (let i in menuDefine) {
            if (menuDefine[i].menuID == e.key) {
                this.setState({
                    activeMenu: menuDefine[i]
                });
                break;
            }
        }
    }
    //面包屑导航处理
    handleNavClick = (e) => {
        // console.log("导航处理:", e);
        let navigator = this.props.navigator;
        if(navigator.length > 0){
            if(navigator[navigator.length -1].type === 'record'){
                this.props.dispatch(closeContractReord());
            }else if(navigator[navigator.length -1].type === 'attachMent')
                this.props.dispatch(closeAttachMent());
        }

    }

    getContentPage() {
        let navigator = this.props.navigator;
        if(navigator.length > 0){
            if(navigator[navigator.length -1].type === 'record'){
                return <ContentPage curMenuID='menu_record'/>;
            }else if(navigator[navigator.length -1].type === 'attachMent')
            return <ContentPage curMenuID='menu_attachMent'/>;
        }
        return <ContentPage curMenuID={this.state.activeMenu.menuID} />
    }
    //获取当前选中部门的完整层级路径
    getActiveOrgFullPath() {
        let activeOrg = this.props.activeOrg || {};
        let orgList = (this.props.orgInfo || {}).orgList || [];
        let fullPath = activeOrg.organizationName;
        if (activeOrg.id !== '0' && activeOrg.parentId) {
            let parentOrg = this.getParentOrg(orgList, activeOrg.parentId);
            console.log("parentOrgName::", parentOrg);
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

    render() {
        let navigator = this.props.navigator;
        let activeOrg = this.props.activeOrg;
        console.log('我属于的部门:',this.props.activeOrg);
        //console.log('所有部门:', this.props.orgInfo);
        let fullPath = this.getActiveOrgFullPath();
        return (
            <Layout className="page">
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
                        defaultSelectedKeys={["menu_index"]}>
                        <Menu.Item key='menu_org_select' style={{borderBottom: '1px solid #fff'}}>
                            <span style={homeStyle.curOrgStype} title={fullPath}>当前部门：{fullPath}></span>
                        </Menu.Item>
                        {menuDefine.map((menu, i) =>

                            <Menu.Item key={menu.menuID} style={{borderBottom: menu.menuID === "menu_invalid" ? '1px solid #fff' : 'none'}}>
                                <Icon type={menu.menuIcon} />
                                <span>{menu.displayName}</span>
                            </Menu.Item>
                        )}
                    </Menu>

                </Sider>
                {
                    this.props.showOrgSelect ? <OrgSelect /> :
                        <Layout>
                            <Header>
                                <Breadcrumb separator=">" style={{fontSize: '1.2rem'}}>
                                    <Breadcrumb.Item onClick={this.handleNavClick} key='home' style={homeStyle.navigator}>{this.state.activeMenu.displayName}</Breadcrumb.Item>
                                    {
                                        navigator.map((nav,i)=> <Breadcrumb.Item key={i}>{nav.name}</Breadcrumb.Item>)
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
    return {
        navigator: state.search.navigator,
        activeOrg: state.search.activeOrg,
        showOrgSelect: state.search.showOrgSelect,
        orgInfo: state.basicData.orgInfo,
        oidc: state.oidc,
    }
}
export default withReducer(reducers, 'ContractManagementIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc,judgePermissions: rootState.app.judgePermissions})})(connect(mapStateToProps)(ContractManagementIndex));
