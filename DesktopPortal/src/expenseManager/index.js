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
    }

    onCollapse = ()=>{
        this.setState({isCollapse: !this.state.isCollapse});
    }
    render(){
        return (
            <Layout>
                <Sider
                    collapsible
                    collapsed={this.state.isCollapse}
                    onCollapse = {this.onCollapse}
                >
                    <Menu
                        theme=''
                        defaultSelectedKeys = {['menu_index']}

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
            </Layout>
        );
    }
}

function mapStateToProps(state){
    
}
export default withReducer(reducers, 'ExpenseManagerIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc,judgePermissions: rootState.app.judgePermissions})})(connect(mapStateToProps)(ExpenseManagerIndex));