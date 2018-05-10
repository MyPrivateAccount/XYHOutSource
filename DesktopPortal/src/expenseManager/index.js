import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {connect} from 'react-redux';
import reducers from './reducers/index';
import rootSaga from './saga/rootSaga';
import {sagaMiddleware} from '../';
import {Layout, Menu, Icon, Button, Breadcrumb} from 'antd';

const {Header, Sider, Content} = Layout;
sagaMiddleware.run(rootSaga);

const menuDefine = [
    {menuID:'menu_index', displayName: '费用信息', menuIcon:'contacts', },
    {menuID: "menu_limit", displayName: "额度设置", menuIcon: 'appstore-o',},
    {menuID: "menu_ctrl", displayName: "费用管控表", menuIcon: 'contacts'},
    {menuID: "menu_total", displayName: "费用统计表", menuIcon: 'appstore-o', },
];
class ExpenseManagerIndex extends Component {

    state = {
        isCollapse: false,
        activeMenu: menuDefine[0],
    }

    onCollapse = ()=>{
        this.setState({isCollapse: !this.state.isCollapse});
    }

    handleNavigatorClick = () =>{

    }

    handleMenuClick = (menu) =>{

    }
    render(){
        return (
            <Layout className="page">
                <Sider
                    collapsible
                    collapsed={this.state.isCollapse}
                    onCollapse = {this.onCollapse}
                >    <div className="logo" />
                    <Menu
                        theme="dark"
                        defaultSelectedKeys = {['menu_index']}
                        mode="inline"
                        onClick={this.handleMenuClick}
                    >
                    {
                        menuDefine.map((item, i) =>
                            <Menu.Item key={item.menuID}>
                                <Icon type={item.menuIcon} />
                                <span>{item.displayName}</span>
                            </Menu.Item>
                        )
                    }
                    </Menu>
                </Sider>
                <Layout>
                    <Header>
                        <Breadcrumb separator='>' style= {{fontSize:'1.2rem'}}> 
                            <Breadcrumb.Item key= 'first'>{menuDefine[this.props.activeMenu.displayName]}</Breadcrumb.Item>
                            {
                                menuDefine.map((item, i) =>{
                                    return <Breadcrumb.Item key={item.id}>{item.name}</Breadcrumb.Item>
                                })
                            }
                        </Breadcrumb>
                    </Header>
                    <Content>

                    </Content>
                </Layout>
            </Layout>
        );
    }
}

function mapStateToProps(state){
    return{
        activeMenu: state.search.activeMenu,
        navigator: state.search.navigator,
    }
}
export default withReducer(reducers, 'ExpenseManagerIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc,judgePermissions: rootState.app.judgePermissions})})(connect(mapStateToProps)(ExpenseManagerIndex));