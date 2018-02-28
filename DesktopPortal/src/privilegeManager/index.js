import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer'
import {Layout, Menu, Icon, Button} from 'antd'
import {connect} from 'react-redux'
import reducers from './reducers'
import ContentPage from './pages/contentPage'
import {sagaMiddleware} from '../'
import rootSaga from './saga/rootSaga';
import {hasBasename} from 'history/PathUtils';

sagaMiddleware.run(rootSaga);

const {Header, Sider, Content} = Layout;

const menuDefine = [
    {menuID: "menu_org", displayName: "组织管理", menuIcon: 'contacts'},
    {menuID: "menu_emp", displayName: "员工查询", menuIcon: 'user'},
    {menuID: "menu_role", displayName: "角色管理", menuIcon: 'usergroup-add'},
    {menuID: "menu_access", displayName: "权限定义", menuIcon: 'lock', requirePermission: ['PermissionItemCreate']},
    {menuID: "menu_app", displayName: "应用管理", menuIcon: 'appstore', requirePermission: ['ApplicationCreate']}
];

class PrivilegeManagerIndex extends Component {

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
    render() {
        let showHeaderMenuIDs = [menuDefine[0].menuID, menuDefine[2].menuID];
        let isShowHeader = (showHeaderMenuIDs.find((menuID) => menuID == this.state.activeMenu.menuID) != undefined);
        let className = isShowHeader ? 'content' : '';
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
                        defaultSelectedKeys={["menu_org"]}>
                        {menuDefine.map((menu, i) =>
                            this.hasPermission(menu) ? <Menu.Item key={menu.menuID}>
                                <Icon type={menu.menuIcon} />
                                <span>{menu.displayName}</span>
                            </Menu.Item> : null
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
    //console.log("权限管理mapStateToProps:" + JSON.stringify(props));
    return {
        judgePermissions: state.judgePermissions
    }
}
export default withReducer(reducers, 'PrivilegeManagerIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc, judgePermissions: rootState.app.judgePermissions, rootBasicData: rootState.basicData})})(connect(mapStateToProps)(PrivilegeManagerIndex));