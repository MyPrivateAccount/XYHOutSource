import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {Layout, Menu, Icon, Button, Breadcrumb} from 'antd';
import {connect} from 'react-redux';
import reducers from './reducers';
import ContentPage from './pages/contentPage';
import {sagaMiddleware} from '../';
import rootSaga from './saga/rootSaga';
import {closeAttachMent, closeContractReord, closeComplement,closeContractDetail, getOrgList, getOrgDetail,getAllOrgList, openOrgSelect,closeOrgSelect, changeContractMenu,setInitActiveOrg} from './actions/actionCreator';
import OrgSelect from './pages/orgSelect/orgSelect';

import AttchMent from './pages/attachMent';

sagaMiddleware.run(rootSaga);

const {Header, Sider, Content} = Layout;
const menuDefine = [
    {menuID: "menu_index", displayName: "合同信息", menuIcon: 'contacts'},
    {menuID: "menu_partA", displayName: "甲方管理", menuIcon: 'appstore-o', requirePermission:['COMPANYA_MANAGE']},
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
    
    componentDidMount(){

    }
    componentWillMount() {
        console.log("当前用户所在部门:", this.props.oidc);
        this.props.dispatch(getAllOrgList("ContractSearchOrg"));
        
        //this.props.dispatch(getAllOrgList("ContractSetOrg"));
        let userInfo = this.props.oidc.user.profile;
        this.props.dispatch(setInitActiveOrg(userInfo.Organization));
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
        //console.log("点击菜单:", e.key);
        if ((e.key === this.state.activeMenu.menuID) &&(this.props.navigator.length === 0) ) return;
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
            {
                this.props.dispatch(closeAttachMent());
            }else if(navigator[navigator.length - 1].type === 'contractDetail')
            {
                this.props.dispatch(closeContractDetail());
            }
            else if(navigator[navigator.length - 1].type === 'complement')
            {
                this.props.dispatch(closeComplement());
            }
        }

    }

    getContentPage() {
        let navigator = this.props.navigator;
        if(navigator.length > 0){
            //console.log('navigator[navigator.length -1].type:', navigator[navigator.length -1].type);
            if(navigator[navigator.length -1].type === 'record'){
                return <ContentPage curMenuID='menu_record'/>;
                
            }else if(navigator[navigator.length -1].type === 'attachMent')
            {
                return <ContentPage curMenuID='menu_attachMent'/>;
            }
            else if(navigator[navigator.length -1].type === 'contractDetail')
            {
                return <ContentPage curMenuID='menu_contractDetail'/>;
            }else if(navigator[navigator.length -1].type === 'complement')
            {
                return <ContentPage curMenuID='menu_complement'/>;
            }
        }
        return <ContentPage curMenuID={this.state.activeMenu.menuID} />
    }
    isShowChooseDepartMent = () =>{
        let navigator = this.props.navigator;
        if(navigator.length == 0){
            return true;
        }
        return false;
    }
    //获取当前选中部门的完整层级路径
    getActiveOrgFullPath() {
        let activeOrg = this.props.activeOrg || {};
        let orgList = (this.props.permissionOrgTree || {}).searchOrgTree || [];
        let fullPath = activeOrg.organizationName;
        if (activeOrg.id !== '0' && activeOrg.parentId) {
            let parentOrg = this.getParentOrg(orgList, activeOrg.parentId);
            //console.log("parentOrgName::", parentOrg);
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
    hasPermission(buttonInfo) {
        let hasPermission = false;
        if (this.props.judgePermissions && buttonInfo.requirePermission) {
            for (let i = 0; i < buttonInfo.requirePermission.length; i++) {
                if (this.props.judgePermissions.includes(buttonInfo.requirePermission[i])) {
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
                        {
                            this.isShowChooseDepartMent() ? 
                            <Menu.Item key='menu_org_select' style={{borderBottom: '1px solid #fff'}}>
                                <span style={homeStyle.curOrgStype} title={fullPath}>当前部门：{fullPath}></span>
                            </Menu.Item>
                            : null
                        }
                        {menuDefine.map((menu, i) =>
                            this.hasPermission(menu) ?
                            <Menu.Item key={menu.menuID} style={{borderBottom: menu.menuID === "menu_invalid" ? '1px solid #fff' : 'none'}}>
                                <Icon type={menu.menuIcon} />
                                <span>{menu.displayName}</span>
                            </Menu.Item> 
                            :null
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
        oidc: state.oidc,
        activeOrg: state.search.activeOrg,
        showOrgSelect: state.search.showOrgSelect,
        orgInfo: state.basicData.orgInfo,
        permissionOrgTree: state.basicData.permissionOrgTree,
     
    }
}
export default withReducer(reducers, 'ContractManagementIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc,judgePermissions: rootState.app.judgePermissions})})(connect(mapStateToProps)(ContractManagementIndex));
