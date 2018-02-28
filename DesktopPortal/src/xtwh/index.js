import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer'
import {Layout, Menu, Icon, Button} from 'antd'
import {connect} from 'react-redux'
import {Route} from 'react-router'
import {push} from 'react-router-redux'
import {APPNAME} from './constants'
import reducers from './reducers'
import './index.less'
import Dic from './pages/dic'
import Area from './pages/area'
import initSagas from './sagas'
import TradePlannnings from './pages/tradePlannings';
initSagas();


const {Header, Sider, Content} = Layout;
const Pages = {
    'dic': {
        title: '数据字典维护',
        component: Dic
    },
    'area': {
        title: '房源区域维护',
        component: Area
    },
    'tradePlannings': {
        title: '租壹屋业态规划维护',
        component: TradePlannnings
    }
}

class XTWHIndex extends Component {
    state = {
        collapsed: false,
        pageTitle: '',
        PageContent: null
    }
    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }
    gotoPath = ({item, key, keyPath}) => {
        let page = Pages[key];
        this.setState({pageTitle: page.title, PageContent: page.component})
    }
    componentWillMount() {
        this.gotoPath({key: 'dic'});
    }
    render() {
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
                        defaultSelectedKeys={["dic"]}>
                        <Menu.Item key="dic">
                            <Icon type="setting" />
                            <span>数据字典维护</span>
                        </Menu.Item>
                        <Menu.Item key="area">
                            <Icon type="environment" />
                            <span>房源区域维护</span>
                        </Menu.Item>
                        <Menu.Item key="tradePlannings">
                            <Icon type="environment" />
                            <span>租壹屋业态规划维护</span>
                        </Menu.Item>
                    </Menu>

                </Sider>
                <Layout>
                    <Header>
                        {pageTitle}
                    </Header>
                    <Content className="content">

                        {PageContent ? <PageContent /> : null}
                    </Content>
                </Layout>
            </Layout>

        )
    }
}


const mapStateToProps = (state, props) => ({
    areaList: state.area.areaList
})
const mapActionToProps = (dispatch) => ({
    gotoPath: (...args) => dispatch(push(...args))
})

export default withReducer(reducers, APPNAME)(connect(mapStateToProps, mapActionToProps)(XTWHIndex));