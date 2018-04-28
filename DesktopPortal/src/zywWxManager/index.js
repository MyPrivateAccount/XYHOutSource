import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer'
import {Layout, Menu, Icon, Button} from 'antd'
import {connect} from 'react-redux'
import {Route} from 'react-router'
import {push} from 'react-router-redux'
import {APPNAME} from './constants'
import reducers from './reducers'
import './index.less'
import FootPrint from './pages/footPrint'
import DayReport from './pages/dayReport'
import HotShop from './pages/hotShop'
import initSagas from './sagas'
import CitySelect from './pages/citySelect'
import {globalAction} from 'redux-subspace';
import {getAllArea} from '../actions/actionCreators'
import {changeCurCity} from './actions/index'

initSagas();


const {Header, Sider, Content} = Layout;
const Pages = {
    'footPrint': {
        title: '客户足迹',
        component: <FootPrint />
    },
    'dayReport': {
        title: '日报数据',
        component: <DayReport />
    },
    'hotShop': {
        title: '热门铺源',
        component: <HotShop />
    }
}

class WXManagerIndex extends Component {
    state = {
        collapsed: false,
        pageTitle: '',
        PageContent: null,
        showCitySelect: false,
        curMenuKey: 'footPrint'
    }
    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }
    gotoPath = (value) => {
        let key = (value || {}).key;
        if (!key) {
            key = this.state.curMenuKey;
        }
        if (key === "city_select") {
            this.setState({showCitySelect: true});
        } else {
            this.setState({showCitySelect: false}, () => {
                let page = Pages[key];
                if (page) {
                    this.setState({pageTitle: page.title, PageContent: page.component, curMenuKey: key});
                }
            });
        }
    }
    componentWillMount() {
        if (this.props.areaList || this.props.areaList.length === 0) {
            this.props.dispatch(globalAction(getAllArea()));
        }
        this.gotoPath({key: 'footPrint'});
    }

    componentWillReceiveProps(newProps) {
        let {curCity, areaList} = this.props;
        let userInfo = ((this.props.oidc || {}).user || {}).profile;
        userInfo.City='500000';
        if (userInfo && userInfo.City !== '' && !curCity) {
            if (areaList || areaList.length > 0) {
                let city = areaList.find(c => c.value === userInfo.City);
                if (city) {
                    this.props.dispatch(changeCurCity(city));
                }
            }
        }
    }

    render() {
        const {curCity, areaList} = this.props;
        let {pageTitle, PageContent} = this.state;
        return (
            <Layout className="page">
                <Sider
                    collapsible
                    collapsed={this.state.collapsed}
                    onCollapse={this.toggle}
                >
                    <div className="logo" />
                    <Menu mode="inline"
                        theme="dark"
                        inlineCollapsed={this.state.collapsed}
                        onClick={this.gotoPath}
                        selectedKeys={[this.state.curMenuKey]}
                        defaultSelectedKeys={["footPrint"]}>
                        <Menu.Item key="city_select">
                            <Icon type="menu-unfold" />
                            <span>当前城市>{curCity ? curCity.label : ''}</span>
                        </Menu.Item>
                        <Menu.Item key="footPrint">
                            <Icon type="setting" />
                            <span>客户足迹</span>
                        </Menu.Item>
                        <Menu.Item key="dayReport">
                            <Icon type="line-chart" />
                            <span>日报数据</span>
                        </Menu.Item>
                        <Menu.Item key="hotShop">
                            <Icon type="star-o" />
                            <span>热门铺源</span>
                        </Menu.Item>
                    </Menu>

                </Sider>
                {this.state.showCitySelect ? <CitySelect hideCitySelect={this.gotoPath} /> : null}
                <Layout style={{display: this.state.showCitySelect ? 'none' : 'flex'}}>
                    <Header>
                        {pageTitle}
                    </Header>
                    <Content className="content">
                        {PageContent ? PageContent : null}
                    </Content>
                </Layout>
            </Layout>

        )
    }
}


const mapStateToProps = (state, props) => ({
    curCity: state.footPrint.curCity,
    oidc: state.oidc,
    areaList: state.rootBasicData.areaList
})
const mapActionToProps = (dispatch) => ({
    gotoPath: (...args) => dispatch(push(...args))
})

export default withReducer(reducers, APPNAME, {mapExtraState: (state, rootState) => ({oidc: rootState.oidc, rootBasicData: rootState.basicData})})(connect(mapStateToProps, mapActionToProps)(WXManagerIndex));