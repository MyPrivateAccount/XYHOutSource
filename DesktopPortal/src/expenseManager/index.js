import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {connect} from 'react-redux';
import reducers from './reducers/index';
import rootSaga from './saga/rootSaga';
import {sagaMiddleware} from '../';
import {Layout, Menu, Icon, Button, Breadcrumb} from 'antd';
import {changeMenu} from './actions/actionCreator';
import ContentPage from './pages/contentPage';
const {Header, Sider, Content} = Layout;
const SubMenu = Menu.SubMenu;
sagaMiddleware.run(rootSaga);


const menuDefine = [
    {
        menuID:'menu_index', displayName: '费用信息', menuIcon:'contacts', type:'subMenu',
        childMenu:[
            { menuID:'basicinfo', displayName: '基本信息', menuIcon:'contacts', type:'item',},
            { menuID:'optin2', displayName: 'option2', menuIcon:'contacts', type:'item',},
        ],
        subMenu:[{
            menuID:'sub1', displayName: 'sub1', menuIcon:'contacts', parent:'menu_index',type:'subMenu',
            childMenu:[
                { menuID:'optin3', displayName: 'option3', menuIcon:'contacts', type:'item',},
                { menuID:'optin4', displayName: 'option4', menuIcon:'contacts', type:'item',},
            ],
         }],
    },
    {
        menuID: "menu_limit", displayName: "额度设置", menuIcon: 'appstore-o',type:'subMenu',
        childMenu:[
            { menuID:'optin5', displayName: 'option5', menuIcon:'contacts', type:'item',},
            { menuID:'optin6', displayName: 'option6', menuIcon:'contacts', type:'item',},
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
        activeMenu: menuDefine[0],
        current: 'menu_index',
        keyPath: [],
        openKeys: [],
        
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
                    this.getMenuItem(menu.subMenu[i],key);
                }
            }
        }
    }
    handleMenuItemClick = (e) => {
        console.log('Clicked: ', e);
        this.setState({ current: e.key, keyPath: e.keyPath });
        
        if (e.key === this.state.activeMenu.menuID ) return;
        let curMainMenu = {};
        for (let i in menuDefine) {
            if (menuDefine[i].menuID == e.keyPath[e.keyPath.length -1]) {
                curMainMenu = menuDefine[i];
                break;
            }
        }
        let activeMenu = this.getMenuItem(curMainMenu, e.key);
  
        console.log('activeMenu:', activeMenu)
 
        this.props.dispatch(changeMenu(activeMenu));

       
        this.setState({ activeMenu:activeMenu});
               
       
      }
      onOpenChange = (openKeys) => {
          console.log('openKeys:', openKeys);
          console.log("state:", this.state);
        const state = this.state;
        const latestOpenKey = openKeys.find(key => !(state.openKeys.indexOf(key) > -1));
        const latestCloseKey = state.openKeys.find(key => !(openKeys.indexOf(key) > -1));
        console.log(latestOpenKey, latestCloseKey);
        let nextOpenKeys = [];
        if (latestOpenKey) {
          nextOpenKeys = this.getAncestorKeys(latestOpenKey).concat(latestOpenKey);
        }
        if (latestCloseKey) {
          nextOpenKeys = this.getAncestorKeys(latestCloseKey);
        }
        this.setState({ openKeys: nextOpenKeys });
      }
      getSubMenuParentID = (menuID)=>{
        for(let i = 0; i < menuDefine.length; i ++){

           let temp = menuDefine[i].subMenu;
        
            while(temp){
                
                let menu = temp.find(item =>item.menuID === menuID)
                if(menu !== undefined)
                {
                    return menu.parent;
                }
                temp = temp.subMenu;
            }
        }
        return null;
      }
      getAncestorKeys = (key) => {
          console.log('getAncestorKeys:', key);
        let parent = [];
        let temp = this.getSubMenuParentID(key);
        if(temp)
        {
            parent.push(temp);
        }
        
        return parent || [];
      }
    onCollapse = ()=>{
        this.setState({isCollapse: !this.state.isCollapse});
    }

    handleNavigatorClick = (e) =>{
        let navigator = this.props.navigator;

    }


    getContentPage = () =>{
        let navigator = this.props.navigator;
        if (navigator.length > 0) {
            if (navigator[navigator.length - 1].id === 0) {
                return <ContentPage curMenuID='Onboarding' />;
            }
        }
       return <ContentPage curMenuID={this.state.activeMenu.menuID} />;
    }

    getSubMenu = (menu) =>{
        if(menu)
        {
            //console.log('menu:', menu.displayName);
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
    getBreadcrumb = () =>{
        let keyPath = this.state.keyPath;
        let menuList = [];
        if(keyPath.length > 0){

            let i = keyPath.length -2;
            let src = menuDefine;
            let temp = menuDefine.find(item => item.menuID === keyPath[keyPath.length -1]);
            menuList.push(temp);
      
            while(i >= 0){
      
                let item;
                if(temp.childMenu)
                {
                    console.log('temp.child：',temp.childMenu );
                    item = (temp.childMenu || []).find(item => item.menuID === keyPath[i]);
                    console.log('temp.item：',item );
                    if(item !== undefined)
                    {
                     
                        menuList.push(item);
                       
                        break;
                    }
                }
                if(temp.subMenu && item === undefined)
                {
                    item = (temp.subMenu || []).find(item => item.menuID === keyPath[i]);
                    console.log('item:', item);
                    if(item !== undefined){
                        menuList.push(item);
                        temp = item;
                    }
                }
                i --;
                
              
            }
      
        }
        return menuList;
    }
    render(){
        let breadcrumbList = this.getBreadcrumb();
        console.log('breadcrumbList:', breadcrumbList);
        return (
            <Layout className="page">
                <Sider
                    collapsible
                    collapsed={this.state.isCollapse}
                    onCollapse = {this.onCollapse}
                >    <div className="logo" />
                    <Menu
                        theme="dark"
                        openKeys={this.state.openKeys}
                        defaultSelectedKeys = {['menu_index']}
                        selectedKeys={[this.state.current]}
                        mode="inline"
                        //onClick={this.handleMenuClick}
                        onClick={this.handleMenuItemClick}
                        onOpenChange={this.onOpenChange}
                    >
                    {
                        this.getMenuContent(menuDefine)
                        // menuDefine.map((item, i) =>
                        //     <Menu.Item key={item.menuID}>
                        //         <Icon type={item.menuIcon} />
                        //         <span>{item.displayName}</span>
                        //     </Menu.Item>
                        // )
                    }
                    </Menu>
                </Sider>
                <Layout>
                    <Header>
                 
                        <Breadcrumb separator='>' style= {{fontSize:'1.2rem'}}> 

                            {
                                breadcrumbList.map((item, i) =>{
                                    return <Breadcrumb.Item key={item.menuID}  style={homeStyle.navigator} onClick={item.type !== 'subMenu' ? this.handleNavigatorClick : null} >{item.displayName}</Breadcrumb.Item>
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
        activeMenu: state.search.activeMenu,
        navigator: state.search.navigator,
    }
}
export default withReducer(reducers, 'ExpenseManagerIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc,judgePermissions: rootState.app.judgePermissions})})(connect(mapStateToProps)(ExpenseManagerIndex));