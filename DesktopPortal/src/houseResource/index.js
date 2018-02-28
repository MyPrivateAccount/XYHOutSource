import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { Layout, Menu, Icon, Button, Badge } from 'antd'
import { connect } from 'react-redux'
import reducers from './reducers'
import ContentPage from './pages/contentPage'
import { sagaMiddleware } from '../'
import rootSaga from './saga/rootSaga';
import './index.less';
import HouseList from './pages/houseList'
import BuildingChangeList from './pages/houseList/buildingChangeList'
import List from './pages/houseActive/houseActive'
import { changeMyAdd, getMyBuildingsListAsync, getMyBuildingList, gotoThisShopStart,
     changeShowGroup, getChangeBuildingList,getBuilding, getAddBuilding, getThisProjectIndex} from './actions/actionCreator';
sagaMiddleware.run(rootSaga);

const { Header, Sider, Content } = Layout;
const iconStyle = {
    marginRight: '8px',
    verticalAlign: 'middle'
}


class HouseResourceIndex extends Component {

    state = {
        displayName: '首页',
        myCurMenuID: 'menu_index',
        collapsed: false,
        display: 'none',
        changeListDisplay: 'none',
        activeListDisplay: 'none',
        isChangeShow: true,
        show: false,
        shopShow: false
    }
    componentWillReceiveProps(newProps) {
        let myAdd = newProps.myAdd;
        if (myAdd === 'menu_building_dish') {
            this.setState({ myCurMenuID: myAdd, displayName: '楼盘录入', display: 'none' })
        } else if (myAdd === 'menu_shops') {
            this.setState({ myCurMenuID: myAdd, displayName: '商铺录入', display: 'none' })
        } else if (myAdd === 'activeProject') {
            this.setState({ myCurMenuID: myAdd, displayName: '楼盘动态详情', activeListDisplay: 'none' })
        } else if (myAdd === 'activeShop') {
            this.setState({ myCurMenuID: myAdd, displayName: '商铺动态详情', activeListDisplay: 'none' })
        }
    }
    componentWillMount() {
        this.props.dispatch(getChangeBuildingList({city: this.props.user.City}))
    }
    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }
    handleClickKeys = (e) => {
        let { isShow, isActiveShow } = this.props.showGroup
        const {buildingInfo, changeList} = this.props
        switch (e.key) {
            case 'menu_building':
                if (this.state.isChangeShow) {
                    this.props.changeMyAdd();
                    this.props.dispatch(changeShowGroup({ type: 0 }))
                    let currentProjectId = (buildingInfo || {}).id || ''
                    let index = changeList.findIndex(v => {
                        return v.id === currentProjectId
                    })
                    this.props.dispatch(getThisProjectIndex({index: index}))
                    this.setState({
                        changeListDisplay: 'block',
                        display: 'none',
                        activeListDisplay: 'none',
                        isChangeShow: !this.state.isChangeShow,
                    });
                } else {
                    this.props.dispatch(changeShowGroup({ type: 0 }))
                    this.setState({
                        changeListDisplay: 'none',
                        display: 'none',
                        activeListDisplay: 'none',
                        isChangeShow: !this.state.isChangeShow,
                    })
                };
                break;
            case 'menu_index':
                this.props.changeMyAdd();
                this.props.dispatch(changeShowGroup({ type: 0 }))
                this.setState({
                    display: 'none',
                    changeListDisplay: 'none',
                    activeListDisplay: 'none',
                    isChangeShow: true,
                    myCurMenuID: 'menu_index',
                    displayName: '首页',
                });
                break;
            case 'menu_xkCenter':
                this.props.changeMyAdd();
                this.props.dispatch(changeShowGroup({ type: 0 }))
                this.setState({
                    display: 'none',
                    changeListDisplay: 'none',
                    activeListDisplay: 'none',
                    isChangeShow: true,
                    myCurMenuID: 'menu_xkCenter',
                    displayName: '销控中心',
                });
                break;
            case 'menu_manager':
                this.props.changeMyAdd();
                this.props.dispatch(changeShowGroup({ type: 0 }))
                this.setState({
                    display: 'none',
                    changeListDisplay: 'none',
                    activeListDisplay: 'none',
                    isChangeShow: true,
                    myCurMenuID: 'menu_manager',
                    displayName: '指派驻场',
                });
                break;
            case 'menu_active':
                if (isActiveShow) {
                    this.props.changeMyAdd();
                    this.props.dispatch(changeShowGroup({ type: 1 }))
                    if (this.props.dynamicData.length === 0) {
                        this.props.dispatch(getBuilding({city: this.props.user.City}));
                    }
                    this.setState({
                        activeListDisplay: 'block',
                        display: 'none',
                        changeListDisplay: 'none',
                        isChangeShow: true,
                    });
                    
                } else {
                    this.props.dispatch(changeShowGroup({ type: 1 }))
                    this.setState({
                        activeListDisplay: 'none',
                        changeListDisplay: 'none',
                        display: 'none',
                        isChangeShow: true,
                    })
                };
                break;
            case 'menu_message':
                this.props.changeMyAdd();
                this.props.dispatch(changeShowGroup({ type: 0 }))
                this.setState({
                    display: 'none',
                    changeListDisplay: 'none',
                    activeListDisplay: 'none',
                    isChangeShow: true,
                    myCurMenuID: 'menu_message',
                    displayName: '发布消息',
                });
                break;
            default: // 新增房源
                if (isShow) {
                    this.props.changeMyAdd();
                    this.props.dispatch(changeShowGroup({ type: 2 }))
                    if (this.props.myBuildingList.length === 0) {
                       console.log(this.props.user)
                       this.props.dispatch(getAddBuilding({city: this.props.user.City}));
                    }
                    this.setState({
                        display: 'block',
                        changeListDisplay: 'none',
                        activeListDisplay: 'none',
                        isChangeShow: true,
                    });
                } else {
                    this.props.dispatch(changeShowGroup({ type: 2 }))
                    this.setState({
                        display: 'none',
                        changeListDisplay: 'none',
                        activeListDisplay: 'none',
                        isChangeShow: true,
                    })
                }
        }
    }

    handleLeave = () => {
        this.setState({ changeListDisplay: 'none', isChangeShow: !this.state.isChangeShow, })
    }
    leaveActive = () => {
        this.setState({ activeListDisplay: 'none' })
    }
    render() {
        const { opreation, nowInfo, buildingInfo} = this.props;
        const changeList = this.props.changeList || [];
        let newList = [], listName;
        if (changeList.length !== 0) {
            listName = (buildingInfo.basicInfo || {}).name || ''
        }
        return (
            <Layout className="page menuPage">
                <Sider collapsible collapsed={this.state.collapsed} onCollapse={this.toggle} className='myMenusider'>
                    <Menu mode="inline" theme="dark" onClick={this.handleClickKeys}
                        inlineCollapsed={this.state.collapsed} defaultSelectedKeys={["menu_index"]}>
                        {
                            changeList.length !== 0 ?
                                <Menu.Item key='menu_building' style={{ textAlign: 'center' }}>
                                    <span>{listName}</span>
                                    <Icon type="right" style={{ marginLeft: '15px' }} />
                                </Menu.Item> : null
                        }
                        <Menu.Item key='menu_index' >
                            <Icon type='home' />
                            <span>首页</span>
                        </Menu.Item>
                        <Menu.Item key='menu_xkCenter'  style={{borderBottom: '1px solid white'}}>
                            <i className='iconfont icon-zhongxin' style={iconStyle} />
                            <span>销控中心</span>
                        </Menu.Item>
                        <Menu.Item key='menu_houseList'>
                            <i className='iconfont icon-xinzengfangyuan_icon' style={iconStyle} />
                            <span>新增房源</span>
                        </Menu.Item>
                        <Menu.Item key='menu_active' >
                            <i className='iconfont icon-dongtai' style={iconStyle} />
                            <span>房源动态</span>
                        </Menu.Item>
                        {
                            this.props.judgePermissions.includes('APPOINT_SCENE') ? <Menu.Item key='menu_manager' >
                                <i className='iconfont icon-manager' style={iconStyle} />
                                <span>驻场管理</span>
                            </Menu.Item> : null
                        }
                        <Menu.Item key='menu_message' >
                            <Icon type='message' />
                            <span>发布消息</span>
                        </Menu.Item>
                    </Menu>
                </Sider>
                <Layout>
                    <Header>{this.state.displayName}</Header>
                    <Content className='content'>
                        <ContentPage curMenuID={this.state.myCurMenuID} />
                    </Content>
                </Layout>
                <div className={this.state.collapsed ? 'collapsed' : 'houseList'} style={{ display: this.state.display }}>
                    <HouseList />
                </div>
                <div className={this.state.collapsed ? 'collapsed' : 'houseList'} style={{ display: this.state.changeListDisplay }} onMouseLeave={this.handleLeave}>
                    <BuildingChangeList buildingList={changeList} />
                </div>
                <div className={this.state.collapsed ? 'collapsed' : 'houseList'} style={{ display: this.state.activeListDisplay }} onMouseLeave={this.leaveActive}>
                    <List />
                </div>
            </Layout>


        )
    }
}
function mapStateToProps(state) {
    return {
        myAdd: state.shop.myAdd,
        nowInfo: state.index.nowInfo,
        showGroup: state.shop.showGroup,
        changeList: state.shop.changeList,
        judgePermissions: state.judgePermissions || [],
        dynamicData: state.active.dynamicData,
        myBuildingList: state.shop.myBuildingList,
        buildingInfo: state.index.buildingInfo,
        user: (state.oidc.user || {}).profile || {},
    }
}
function mapDispatchToProps(dispatch) {
    return {
        dispatch,
        changeMyAdd: () => dispatch(changeMyAdd()),
        getMyBuildingList: () => dispatch(getMyBuildingList()),
        gotoThisShopStart: () => dispatch(gotoThisShopStart())
    }
}

export default withReducer(reducers, 'HouseResourceIndex', { mapExtraState: (state, rootState) => ({ oidc: rootState.oidc, judgePermissions: rootState.app.judgePermissions }) })(connect(mapStateToProps, mapDispatchToProps)(HouseResourceIndex));