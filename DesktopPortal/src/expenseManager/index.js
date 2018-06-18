import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {connect} from 'react-redux';
import reducers from './reducers/index';
import rootSaga from './saga/rootSaga';
import {sagaMiddleware} from '../';
import {Layout, Menu, Icon, Button} from 'antd';
import {changeMenu, closebreadPage, setuserPage, setuserPageIndex} from './actions/actionCreator';
import ContentPage from './pages/contentPage';
import createHistory from 'history/createMemoryHistory'
import {ConnectedRouter, push} from 'react-router-redux'
import {Route, Switch, } from 'react-router'
import AddCharge from './pages/fylr/AddCharge'
import ChargeIndex from './pages/fylr'

const {Header, Sider, Content} = Layout;
const SubMenu = Menu.SubMenu;
sagaMiddleware.run(rootSaga);

const history = createHistory();
let routeCallback =null;

history.listen((location,action)=>{
    if(routeCallback){
        routeCallback(location,action);
    }
})

const menuDefine = [
    {
        menuID:'menu_index', displayName: '费用信息', menuIcon:'contacts', type:'subMenu',
        childMenu:[
            { menuID:'home', displayName: '费用', menuIcon:'contacts', type:'item', path:'/'}
        ],
    },
    {
        menuID: "menu_limit", displayName: "额度设置", menuIcon: 'appstore-o',type:'subMenu',
        childMenu:[
            { menuID:'limitindex', displayName: '费用限制', menuIcon:'contacts', type:'item', path:'/dd'},
        ],
    },
    {menuID: "menu_ctrl", displayName: "费用管控表", menuIcon: 'contacts'},
    {menuID: "menu_total", displayName: "费用统计表", menuIcon: 'appstore-o', },
];

let _activeMenu =menuDefine[0].childMenu[0];

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
    constructor(props){
        super(props);

        this.state = {
            isCollapse: false,
            activeMenu: _activeMenu,
            showBack:false
        }

        this.handleMenuItemClick = this.handleMenuItemClick.bind(this);
    }
    
    componentDidMount=()=>{
        routeCallback=(location, action)=>{
            
            this.setState({showBack: location.pathname!==this.state.activeMenu.path})
        }
    }

    componentWillUnmount=()=>{
        routeCallback=null;
    }
    

    getMenuItem = (menu, key) =>{
        let temp = {};
        if(menu && menu.childMenu){
            temp = menu.childMenu.find(item => item.menuID === key);
            if(temp !== undefined)
            {
                return temp;
            }
            if(menu.subMenu){
                for(let i = 0; i < menu.subMenu.length; i++){
                    let res = this.getMenuItem(menu.subMenu[i], key);
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
        history.entries.length = 0;
        _activeMenu = activeMenu;
        this.setState({activeMenu: _activeMenu},()=>{
            history.replace(activeMenu.path, activeMenu)
        })
      }

    onCollapse = ()=>{
        this.setState({isCollapse: !this.state.isCollapse});
    }

    handleNavigatorClick = (i, menu) =>{
        let navigator = this.props.navigator;
        if(navigator.length > 0 && menu.type === "item") {
            if(navigator[navigator.length -1].menuID !== menu.menuID) {
                this.props.dispatch(setuserPageIndex(i));
            }
        }
    }

    // getContentPage = () =>{
    //     let navigator = this.props.navigator;

    //     if (navigator.length > 0) {
    //             return <ContentPage curMenuID={navigator[navigator.length - 1].menuID} extra={navigator[navigator.length - 1].extra} />;
    //     }
    //     return <ContentPage curMenuID={this.state.activeMenu.menuID} />;
    // }

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

    goBack=()=>{
        history.goBack();
    }

    render(){
        let navigator = [];
        history.entries.forEach(item=>{
            if(item.state){
                navigator.push({menuId: item.state.menuID, disname: item.state.displayName})
            }
        })
        console.log(navigator);
        return (
           
            <Layout className="page">
            
                <Sider
                    collapsible
                    collapsed={this.state.isCollapse}
                    onCollapse = {this.onCollapse}
                >    <div className="logo" />
                    <Menu
                        theme="dark"
                        defaultSelectedKeys = {[this.state.activeMenu.menuID]}
                        defaultOpenKeys={['menu_index']}
                        mode="inline"
                        onClick={this.handleMenuItemClick}
                    >
                    {
                        this.getMenuContent(menuDefine)
                    }
                    </Menu>
                </Sider>
                <Layout>
                    <Header>
                        <div onClick={this.goBack} className="back-btn" style={{display: this.state.showBack?'inline-block':'none'}}>
                            <Icon type="left" /><span className="b-text">返回</span>
                        </div>
                        
                        {/* <Breadcrumb separator='>' style= {{fontSize:'0.8rem'}}> 
                            {
                                navigator.map((item, i) =>{
                                    return <Breadcrumb.Item key={i}  style={homeStyle.navigator} onClick={(e) =>this.handleNavigatorClick(i, item)} >{item.disname}</Breadcrumb.Item>
                                })
                            }
                        </Breadcrumb> */}
                    </Header>
                    <ConnectedRouter history={history}>
                    <Content className='content'>
                        {/* {
                            this.getContentPage()
                        } */}
                        <Route exact path='/' component={ChargeIndex}/>
                        <Route  path='/dd' component={AddCharge}/>
                    </Content>
                    </ConnectedRouter>
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
export default withReducer(reducers, 'ExpenseManagerIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc,judgePermissions: rootState.app.judgePermissions, basicData: rootState.basicData})})(connect(mapStateToProps)(ExpenseManagerIndex));