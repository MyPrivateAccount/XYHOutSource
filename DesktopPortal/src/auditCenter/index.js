import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { Layout, Menu, Icon, Button, Breadcrumb } from 'antd'
import { connect } from 'react-redux'
import reducers from './reducers'
import { sagaMiddleware } from '../'
import createHistory from 'history/createMemoryHistory'
import { ConnectedRouter } from 'react-router-redux'
import { Route } from 'react-router'
import rootSaga from './saga/rootSaga';
import { closeAuditDetail, changeMenu, setLoadingVisible, getAuditList } from './actions/actionCreator';
import ContentPage from './pages/contentPage';
import SearchCondition from './constants/searchCondition';
import { getDicParList } from '../actions/actionCreators'
import Layer, { LayerRouter } from '../components/Layer'
import {
    LoadableMyAuditPage,
    LoadableMySubmitPage,
    LoadableCopyToMePage
} from './pages/contentPage'

sagaMiddleware.run(rootSaga);

const { Header, Sider, Content } = Layout;
const menuDefine = [
    // { menuID: "menu_waitAudit", displayName: "待审核", menuIcon: 'info-circle-o', examineStatus: 1 },
    // { menuID: "menu_passed", displayName: "审核通过", menuIcon: 'check-circle-o', examineStatus: 2 },
    // { menuID: "menu_reject", displayName: "审核驳回", menuIcon: 'close-circle-o', examineStatus: 3 }
    { menuID: "menu_myAudit", displayName: "我审核的", menuIcon: 'info-circle-o', path: '/audit' },
    { menuID: "menu_mySubmit", displayName: "我发起的", menuIcon: 'check-circle-o', path: '/submit' },
    { menuID: "menu_copyToMe", displayName: "抄送我的", menuIcon: 'copy', path: '/copytome' }
];
const homeStyle = {
    navigator: {
        cursor: 'pointer',
    }
}

const history = createHistory();
let routeCallback = null;

history.listen((location, action) => {
    if (routeCallback) {
        routeCallback(location, action);
    }
})


class AuditIndex extends Component {

    state = {
        collapsed: false,
        activeMenu: menuDefine[0],
        showBack: false,
        title: ''
        //searchCondition: SearchCondition.auditListCondition
    }
    componentWillMount() {
        this.props.dispatch(getDicParList(['PHOTO_CATEGORIES', 'SHOP_LEASE_PAYMENTTIME', 'SHOP_DEPOSITTYPE']));
        this.props.dispatch(changeMenu(menuDefine[0]));
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getAuditList(SearchCondition.auditListCondition));
    }

    componentDidMount = () => {
        routeCallback = (location, action) => {
            this.setState({ showBack: location.pathname !== this.state.activeMenu.path, title: ((location.state || {}).menu || {}).displayName })
        }
        if (this.state.activeMenu) {
            history.replace(this.state.activeMenu.path, { ...this.state.activeMenu.par, menu: this.state.activeMenu })
        }
    }

    setPageTitle=(title)=>{
        this.setState({title: title})
    }


    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }

    handleMenuItemClick = (e) => {
        let menu = menuDefine.find(x => x.menuID === e.key);
        if (!menu) {
            return;
        }
        history.entries.length = 0;
        this.setState({ activeMenu: menu }, () => {
            history.replace(menu.path, { ...menu.par, menu: menu })
        })

        // for (let i in menuDefine) {
        //     if (menuDefine[i].menuID === e.key) {
        //         this.setState({
        //             activeMenu: menuDefine[i]
        //         });
        //         // this.props.dispatch(changeMenu(menuDefine[i]));
        //         // this.props.dispatch(setLoadingVisible(true));
        //         // let searchCondition = SearchCondition.auditListCondition;
        //         // searchCondition.examineStatus = [menuDefine[i].examineStatus];
        //         // this.props.dispatch(getAuditList(searchCondition));
        //         break;
        //     }
        // }
    }
    //面包屑导航处理
    // handleNavClick = (e) => {
    //     console.log("导航处理:", e);
    //     this.props.dispatch(closeAuditDetail());
    // }

    goBack = () => {
        history.goBack();
    }

    render() {
        //let navigator = this.props.navigator;
        let isShowHeader = true;
        let className = isShowHeader ? 'content' : '';
        return (
            <Layout className="page">
                <Sider
                    collapsible
                    collapsed={this.state.collapsed}
                    onCollapse={this.toggle}>

                    <div className="logo" />
                    <Menu
                        theme="dark"
                        className="left-menu"
                        defaultSelectedKeys={[this.state.activeMenu.menuID]}
                        selectedKeys={[this.state.activeMenu.menuID]}
                        mode="inline"
                        onClick={this.handleMenuItemClick}
                    >
                        {
                            menuDefine.map(mi => {
                                if (mi.menuID === 'split') {
                                    return <Menu.Divider />
                                }
                                return <Menu.Item key={mi.menuID} ><span><Icon type={mi.menuIcon} /><span>{mi.displayName}</span></span></Menu.Item>;
                            })

                        }
                    </Menu>
                    {/*                     
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
                    </Menu> */}

                </Sider>
                <Layout>
                    < Header >
                        <Button type="primary" onClick={this.goBack} style={{ display: this.state.showBack ? 'inline-block' : 'none' }}>
                            <Icon type="left" />返回
                        </Button>

                        {
                             <div>{this.state.title}</div> 
                        }
                    </ Header>
                    <Content className={className}>
                        <ConnectedRouter history={history}>
                            <Layer>
                                <Route path='/copytome' component={LoadableCopyToMePage}/>
                                <Route path='/audit' component={LoadableMyAuditPage}/>
                                <Route path='/submit' component={LoadableMySubmitPage}/>
                            </Layer>
                        </ConnectedRouter>
                    </Content>
                    {/* <Header>
                        <Breadcrumb separator=">" style={{fontSize: '1.2rem'}}>
                            <Breadcrumb.Item onClick={this.handleNavClick} key='home' style={homeStyle.navigator}>{this.state.activeMenu.displayName}</Breadcrumb.Item>
                            {
                                navigator.map(nav => <Breadcrumb.Item key={nav.id}>{nav.contentName || nav.taskName || '未命名'}</Breadcrumb.Item>)
                            }
                        </Breadcrumb>
                    </Header> */}
                    {/* <Content className='content'>
                        <ContentPage curMenuID={this.state.activeMenu.menuID} />
                    </Content> */}
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
export default withReducer(reducers, 'AuditIndex', { namespaceActions: false, mapExtraState: (state, rootState) => ({ oidc: rootState.oidc, basicData: rootState.basicData }) })(connect(mapStateToProps)(AuditIndex));