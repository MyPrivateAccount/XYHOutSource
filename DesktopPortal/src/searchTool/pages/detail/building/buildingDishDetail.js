import {connect} from 'react-redux';
import {getDicParList, getBuildingShops, setLoading, openMsgList, getMsgList, getShopDetail} from '../../../actions/actionCreator';
import React, {Component} from 'react'
import * as actionTypes from '../../../constants/actionType';
import {Layout, Carousel, Menu, Row, Col, Icon, Radio} from 'antd'
import BasicInfo from './basicInfo';
import ProjectInfo from './projectInfo';
import RelShopsInfo from './relShopsInfo';
import SupportInfo from './supportInfo';
// import AttachInfo from '../shops/attachInfo';
import './buildingDish.less';
// import ShopResultItem from '../../shopResultItem';
import ShopResultItem from '../../../../businessComponents/shopSearchItem';
import RulesInfo from './rulesInfo';
import SaleControlPanel from './saleControlPanel';
import CommissionInfo from './commissionInfo';
import ZcManager from './zcManager';
import RulesTemplateInfo from './rulesTemplateInfo';
import Active from './active';

const {Header, Sider, Content} = Layout;

class BuildingDishEdit extends Component {
    state = {
        activeTab: 'basic',
        selectedShopStatus: 'all',
        shopStatusArray: []
    }
    componentWillMount() {
        if (this.props.basicData.saleStatus.length === 0
            && this.props.basicData.saleModel.length === 0) {
            this.props.dispatch(getDicParList(["TRADE_MIXPLANNING", "SALE_MODE", "PROJECT_SALE_STATUS", "SHOP_CATEGORY"]));
        }
        this.props.dispatch(getMsgList({pageIndex: 0, pageSize: 10}));
    }

    componentWillReceiveProps(newProps) {
        this.getShopStatusArray(newProps);
    }

    handleChangTab = (tabId) => {
        const activeBuildingShops = this.props.searchInfo.activeBuildingShops;
        this.setState({activeTab: tabId});
        if (["shopLayout", "marketingCtrl"].includes(tabId) && activeBuildingShops.length === 0) {
            //console.log("加载楼盘的商铺:", this.props.buildInfo.id);
            this.props.dispatch(setLoading(true));
            this.props.dispatch(getBuildingShops({buildingIds: this.props.buildInfo.id, saleStatus: []}));
        }
    }
    //消息查看
    handleMessageView = (msg) => {
        console.log("消息查看:", msg);
        this.props.dispatch(openMsgList());
    }
    //商铺销售状态切换
    handleShopSaleChange = (e) => {
        this.setState({selectedShopStatus: e.key});
        let saleStatus = [];
        if (e.key !== "all") {
            saleStatus = [e.key];
        }
        this.props.dispatch(getBuildingShops({buildingIds: this.props.buildInfo.id, saleStatus: saleStatus}));
    }
    gotoShopDetail = (shopId) => {
        this.props.dispatch(setLoading(true));
        this.props.dispatch(getShopDetail(shopId));
    }

    getShopStatusArray = (props) => {
        let shopStatusArray = [...this.state.shopStatusArray];
        const activeBuildingShops = props.searchInfo.activeBuildingShops;
        if (shopStatusArray.length > 0) return;
        if (activeBuildingShops && activeBuildingShops.length > 0) {
            activeBuildingShops.map(shop => {
                let shopStatusResult = shopStatusArray.find(s => s.saleStatus == shop.saleStatus);
                if (shopStatusResult) {
                    shopStatusResult.count = shopStatusResult.count + 1;
                } else {
                    shopStatusArray.push({saleStatus: shop.saleStatus, count: 1});
                }
            });
        }
        this.setState({shopStatusArray: shopStatusArray});
    }
    getShopStatusStatistic(status) {
        console.log("getShopStatusStatistic:", status);
        let count = 0;
        let shopStatusArray = [...this.state.shopStatusArray];
        if (status) {
            let result = shopStatusArray.find(s => s.saleStatus == status);
            if (result) {
                count = result.count;
            }
        } else {
            shopStatusArray.map(shopS => {
                count += shopS.count;
            });
        }
        return "(" + count + ")";
    }
    render() {
        const activeBuildingShops = this.props.searchInfo.activeBuildingShops;
        const msgList = this.props.msgList.extension || [];
        const shopSaleStatus = this.props.basicData.shopSaleStatus || [];

        return (
            <div className="relative" style={{border: '1px solid #ccc', marginTop: '10px'}}>
                <Layout>
                    <Content className='content buildingDish'>
                        {/**基本信息**/}
                        <Row id="basicInfo">
                            <Col span={24}><BasicInfo /> </Col>
                        </Row>
                        {/**走马灯消息**/}
                        {/* <Row>
                            <Col>
                                <Carousel autoplay={true} dots={false} vertical={true}>
                                    {
                                        msgList.map(msg => <div key={msg.id} onClick={(e) => this.handleMessageView(msg)} style={{cursor: 'pointer'}}><Icon type="notification" />{msg.title}</div>)
                                    }
                                </Carousel>
                            </Col>
                        </Row> */}
                        <Row style={{marginTop: '25px'}}>
                            <Col className="content">
                                <Radio.Group value={this.state.activeTab} size='large' onChange={(e) => this.handleChangTab(e.target.value)}>
                                    <Radio.Button value="basic">基本信息</Radio.Button>
                                    <Radio.Button value="marketingCtrl">销控看板</Radio.Button>
                                    <Radio.Button value="reportRegular">报备规则</Radio.Button>
                                    <Radio.Button value="buildingNewInfo">楼盘动态</Radio.Button>
                                    <Radio.Button value="shopLayout">商铺列表</Radio.Button>
                                </Radio.Group>
                            </Col>
                        </Row>
                        {
                            this.state.activeTab === "basic" ? <div>
                                <Row id="commission">
                                    {/*** 佣金方案*/}
                                    <Col span={24}> <CommissionInfo /></Col>
                                </Row>
                                <Row id="zcManager">
                                    {/*** 驻场信息*/}
                                    <Col span={24}> <ZcManager /></Col>
                                </Row>
                                <Row id="supportInfo">
                                    {/*** 配套信息 */}
                                    <Col span={24}><SupportInfo /> </Col>
                                </Row>

                                <Row id="relShopsInfo">
                                    {/*** 商铺整体概况*/}
                                    <Col span={24}> <RelShopsInfo /> </Col>
                                </Row>

                                <Row id="projectInfo">
                                    {/*** 项目简介*/}
                                    <Col span={24}> <ProjectInfo /></Col>
                                </Row>

                            </div> : null
                        }
                        {
                            this.state.activeTab === "reportRegular" ?
                                <div>
                                    <RulesInfo />
                                    <RulesTemplateInfo />
                                </div> : null
                        }

                        {
                            this.state.activeTab === "marketingCtrl" ? <SaleControlPanel /> : null
                        }

                        {
                            this.state.activeTab === "buildingNewInfo" ?
                                <div>
                                    <Active id={this.props.buildInfo.id} />
                                </div> : null
                        }
                        {
                            this.state.activeTab === "shopLayout" ? <div className="searchResult" style={{padding: "20px 50px"}}>
                                <Menu onClick={this.handleShopSaleChange}
                                    selectedKeys={[this.state.selectedShopStatus]}
                                    mode="horizontal"
                                >
                                    <Menu.Item key="all">全部{this.getShopStatusStatistic()}</Menu.Item>
                                    {shopSaleStatus.map(s => <Menu.Item key={s.value}>{s.key}{this.getShopStatusStatistic(s.value)}</Menu.Item>)}
                                </Menu>
                                {activeBuildingShops.map((item, i) => <ShopResultItem key={i} shopInfo={item} buildingInfo={this.props.buildInfo} gotoShopDetail={this.gotoShopDetail} />)}</div> : null
                        }
                    </Content>
                </Layout>
            </div>
        )
    }
}

function mapStateToProps(state) {
    // console.log('消息列表:' + JSON.stringify(state.search.msgList));
    return {
        buildInfo: state.search.activeBuilding,
        searchInfo: state.search,
        basicData: state.basicData,
        msgList: state.search.msgList
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BuildingDishEdit);