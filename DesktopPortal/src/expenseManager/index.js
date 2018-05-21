import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {connect} from 'react-redux';
import reducers from './reducers/index';
import rootSaga from './saga/rootSaga';
import {sagaMiddleware} from '../';
import {Layout, Menu, Icon, Button, Breadcrumb} from 'antd';
import {changeMenu, closebreadPage, setuserPage, setuserPageIndex} from './actions/actionCreator';
import ContentPage from './pages/contentPage';
const {Header, Sider, Content} = Layout;
const SubMenu = Menu.SubMenu;
sagaMiddleware.run(rootSaga);


const menuDefine = [
    {
        menuID:'menu_index', displayName: '费用信息', menuIcon:'contacts', type:'subMenu',
        childMenu:[
            { menuID:'home', displayName: '费用', menuIcon:'contacts', type:'item',}
        ],
    },
    {
        menuID: "menu_limit", displayName: "额度设置", menuIcon: 'appstore-o',type:'subMenu',
        childMenu:[
            { menuID:'limitindex', displayName: '费用限制', menuIcon:'contacts', type:'item',},
        ],
        subMenu:[{
            menuID:'sub2', displayName: 'sub2', menuIcon:'contacts', parent:'menu_limit',type:'subMenu',
            childMenu:[
                { menuID:'optin7', displayName: 'option7', menuIcon:'contacts', type:'item',},
                { menuID:'optin8', displayName: 'option8', menuIcon:'contacts', type:'item',},
            ],
         }],
    },
    {menuID: "menu_ctrl", displayName: "费用管控表", menuIcon: 'contacts'},
    {menuID: "menu_total", displayName: "费用统计表", menuIcon: 'appstore-o', },
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

class ExpenseManagerIndex extends Component {

    state = {
        isCollapse: false,
        activeMenu: menuDefine[0].childMenu[0],
    }

    getMenuItem = (menu, key, navigator) =>{
        let temp = {};
        if(menu && menu.childMenu){
            temp = menu.childMenu.find(item => item.menuID === key);
            if(temp !== undefined)
            {
                navigator.push({menuID: menu.menuID, disname: menu.displayName});
                navigator.push({menuID: temp.menuID, disname: temp.displayName});
                return temp;
            }
            if(menu.subMenu){
                navigator.push({menuID: menu.menuID, disname: menu.displayName});
                for(let i = 0; i < menu.subMenu.length; i++){
                    let res = this.getMenuItem(menu.subMenu[i], key, navigator);
                    if(res !== undefined) {
                        return res;
                    }
                }
            }
        }
    }
    handleMenuItemClick = (e) => {
        if (e.key === this.state.activeMenu.menuID ) return;

        let activeMenu = null;
        let navigator = [];
        for (let item of menuDefine) {
            activeMenu = this.getMenuItem(item, e.key, navigator);
            if (activeMenu) {
                break;
            }
        }

        this.state.activeMenu = activeMenu;//只是存储
        this.props.dispatch(setuserPage(navigator));
      }

    onCollapse = ()=>{
        this.setState({isCollapse: !this.state.isCollapse});
    }

    handleNavigatorClick = (i, menu) =>{
        let navigator = this.props.navigator;
        if(navigator.length > 0) {
            if(navigator[navigator.length -1].menuID !== menu.menuID) {
                this.props.dispatch(setuserPageIndex(i));
            }
        }
    }

    getContentPage = () =>{
        let navigator = this.props.navigator;

        if (navigator.length > 0) {
                return <ContentPage curMenuID={navigator[navigator.length - 1].menuID} />;
        }
        return <ContentPage curMenuID={this.state.activeMenu.menuID} />;
    }

    getSubMenu = (menu) =>{
        if(menu)
        {
            return (
                <SubMenu key={menu.menuID} title={<span><Icon type={menu.menuIcon} /><span>{menu.displayName}</span></span>}>
                    {
                        menu.childMenu ? menu.childMenu.map( (item, j) =>
                             <Menu.Item key={item.menuID}>{item.displayName}</Menu.Item>
                        ) : null
                        
                    }
                    {
                        menu.subMenu ? menu.subMenu.map(item => this.getSubMenu(item)) : null
                    }
                </SubMenu>
            )
        }

    }
    getMenuContent = (menuList) =>{
        if(menuList)
        {
           return  menuList.map((menu, i) => this.getSubMenu(menu));
        }
    }

    render(){
        let navigator = this.props.navigator;
        return (
            <Layout className="page">
                <Sider
                    collapsible
                    collapsed={this.state.isCollapse}
                    onCollapse = {this.onCollapse}
                >    <div className="logo" />
                    <Menu
                        theme="dark"
                        defaultSelectedKeys = {['home']}
                        defaultOpenKeys={['menu_index']}
                        mode="inline"
                        onClick={this.handleMenuItemClick.bind(this)}
                    >
                    {
                        this.getMenuContent(menuDefine)
                    }
                    </Menu>
                </Sider>
                <Layout>
                    <Header>
                 
                        <Breadcrumb separator='>' style= {{fontSize:'0.8rem'}}> 
                            {
                                navigator.map((item, i) =>{
                                    return <Breadcrumb.Item key={i}  style={homeStyle.navigator} onClick={(e) =>this.handleNavigatorClick(i, item)} >{item.disname}</Breadcrumb.Item>
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
            </Layout>
        );
    }
}

function mapStateToProps(state){
    return{
        navigator: state.basicData.navigator,
    }
}
export default withReducer(reducers, 'ExpenseManagerIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc,judgePermissions: rootState.app.judgePermissions})})(connect(mapStateToProps)(ExpenseManagerIndex));