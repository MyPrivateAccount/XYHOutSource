import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { Layout, Menu, Icon, Button, Breadcrumb } from 'antd'
import { connect } from 'react-redux'
import reducers from './reducers'
import { sagaMiddleware } from '../'
import rootSaga from './saga/rootSaga';
import { closeAuditDetail, changeMenu, setLoadingVisible, getAuditList } from './actions/actionCreator';
import ContentPage from './pages/contentPage';
import SearchCondition from './constants/searchCondition';
sagaMiddleware.run(rootSaga);

const { Header, Sider, Content } = Layout;
const menuDefine = [
    // { menuID: "menu_waitAudit", displayName: "待审核", menuIcon: 'info-circle-o', examineStatus: 1 },
    // { menuID: "menu_passed", displayName: "审核通过", menuIcon: 'check-circle-o', examineStatus: 2 },
    // { menuID: "menu_reject", displayName: "审核驳回", menuIcon: 'close-circle-o', examineStatus: 3 }
    { menuID: "menu_myAudit", displayName: "我审批的", menuIcon: 'info-circle-o' },
    { menuID: "menu_mySubmit", displayName: "我发起的", menuIcon: 'check-circle-o' },
    { menuID: "menu_copyToMe", displayName: "抄送我的", menuIcon: 'copy' }
];
const homeStyle = {
    navigator: {
        cursor: 'pointer',
    }
}

class AuditIndex extends Component {

    state = {
        collapsed: false,
        activeMenu: menuDefine[0],
        //searchCondition: SearchCondition.auditListCondition
    }
    componentWillMount() {
        this.props.dispatch(changeMenu(menuDefine[0]));
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(SearchCondition.auditListCondition));
    }
    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }

    handleMenuClick = (e) => {
        for (let i in menuDefine) {
            if (menuDefine[i].menuID === e.key) {
                this.setState({
                    activeMenu: menuDefine[i]
                });
                // this.props.dispatch(changeMenu(menuDefine[i]));
                // this.props.dispatch(setLoadingVisible(true));
                // let searchCondition = SearchCondition.auditListCondition;
                // searchCondition.examineStatus = [menuDefine[i].examineStatus];
                // this.props.dispatch(getAuditList(searchCondition));
                break;
            }
        }
    }
    //面包屑导航处理
    handleNavClick = (e) => {
        console.log("导航处理:", e);
        this.props.dispatch(closeAuditDetail());
    }

    render() {
        let navigator = this.props.navigator;
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
                        defaultSelectedKeys={["menu_myAudit"]}>
                        {menuDefine.map((menu, i) => {
                            return (
                                <Menu.Item key={menu.menuID}>
                                    <Icon type={menu.menuIcon} />
                                    <span>{menu.displayName}</span>
                                </Menu.Item>
                            )
                        })}
                    </Menu>

                </Sider>
                <Layout>
                    <Header>
                        <Breadcrumb separator=">" style={{ fontSize: '1.2rem' }}>
                            <Breadcrumb.Item onClick={this.handleNavClick} key='home' style={homeStyle.navigator}>{this.state.activeMenu.displayName}</Breadcrumb.Item>
                            {
                                navigator.map(nav => <Breadcrumb.Item key={nav.id}>{nav.contentName || '未命名'}</Breadcrumb.Item>)
                            }
                        </Breadcrumb>
                    </Header>
                    <Content className='content'>
                        <ContentPage curMenuID={this.state.activeMenu.menuID} />
                    </Content>
                </Layout>
            </Layout>
        )
    }
}
function mapStateToProps(state, props) {
    return {
        navigator: state.audit.navigator
    }
}
export default withReducer(reducers, 'AuditIndex', { namespaceActions: false })(connect(mapStateToProps)(AuditIndex));