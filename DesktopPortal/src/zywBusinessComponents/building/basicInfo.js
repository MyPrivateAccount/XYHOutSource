import {connect} from 'react-redux';
// import {getBuildingShops, setLoading, openRecommendDialog, cancelRecommend} from '../../../actions/actionCreator';
import React, {Component} from 'react'
import {Layout, Icon, Row, Col, Form} from 'antd'
import Slider from 'react-slick'
import '../../../node_modules/slick-carousel/slick/slick.css'
import '../../../node_modules/slick-carousel/slick/slick-theme.css'

const {Header, Sider, Content} = Layout;

class BasicInfo extends Component {
    state = {
        activeImg: "../../../images/noPicture.png",
        expandStatus: true
    }
    componentWillMount() {

    }

    // handleRecommend = (building) => {
    //     console.log("推荐楼盘：", building);
    //     this.props.dispatch(openRecommendDialog(building));
    // }
    // confirmCancelRecommend = (recommendInfo) => {
    //     console.log("取消推荐：", recommendInfo);
    //     this.props.dispatch(cancelRecommend([recommendInfo.id]));
    // }

    // handleViewShops = (e) => {//查看商铺
    //     console.log("查看商铺", e);
    //     this.props.dispatch(setLoading(true));
    //     this.props.dispatch(getBuildingShops(e));
    // }


    render() {
        const buildingInfo = this.props.buildingInfo || {};
        const basicInfo = buildingInfo.basicInfo || {};
        const fileList = buildingInfo.fileList || [];
        if (fileList && fileList.length === 0) {
            fileList.push({
                "fileGuid": "000-0000-0000-000",
                "icon": "../../../../../images/default-img.jpg",
                "original": "string",
                "medium": "string",
                "small": "../../../../../images/default-img.jpg"
            });
        }
        const sliderSettings = {
            dots: true,
            infinite: true,
            speed: 500,
            slidesToShow: 1,
            slidesToScroll: 1,
            customPaging: function (i) {
                return <a><img className='navImg' src={fileList[i].icon} /></a>
            }
        };

        return (
            <div className="relative">
                <Layout>
                    <Content className='content basicEdit' style={{backgroundColor: "#ECECEC"}}>
                        <Form layout="horizontal" >
                            <Row type="flex" style={{padding: '1rem 0'}}>
                                <Col span={23}>
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>基本信息</span>
                                </Col>
                                <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                            </Row>
                            <Row className="buildingPic" style={{display: this.state.expandStatus ? "block" : "none"}}>
                                {/** * 图片*/}
                                <Col span={12}>
                                    <Slider {...sliderSettings}>
                                        {
                                            fileList.map((f, i) => <div key={i}><img src={f.small} /></div>)
                                        }
                                    </Slider>
                                </Col>
                                {/*** 详细 */}
                                <Col span={12}>
                                    <Row className='viewRow'>
                                        <Col span={24} ><b style={{fontSize: '1.3rem'}}>{basicInfo.name} | </b>{basicInfo.areaFullName}</Col>
                                    </Row>
                                    <Row className='viewRow'>
                                        <Col span={12}>{basicInfo.builtupArea}㎡ | 共{basicInfo.houseHolds}户</Col>
                                        <Col span={12} style={{color: 'red', fontSize: '1.2rem'}}>{basicInfo.minPrice || basicInfo.maxPrice ? (basicInfo.minPrice + '-' + basicInfo.maxPrice + '元/㎡/月') : '暂未定价'}</Col>
                                    </Row>
                                    {/* <Row className='viewRow'>
                                        <Col span={12}>开盘时间：{basicInfo.openDate ? basicInfo.openDate.replace("T", " ") : ""}</Col>
                                        <Col span={12}>交房时间：{basicInfo.deliveryDate ? basicInfo.deliveryDate.replace("T", " ") : ""}</Col>
                                    </Row> */}
                                    <Row className='viewRow'>
                                        {/* <Col span={12}>租金：{priceStr ? priceStr : ""}</Col> */}
                                        <Col span={12}>商业总体量：{basicInfo.commercialAggregate ? basicInfo.commercialAggregate : ""}</Col>
                                    </Row>
                                    <Row className='viewRow'>
                                        <Col span={12}>产权年限：{basicInfo.propertyTerm}年</Col>
                                        <Col span={12}>土地到期时间：{basicInfo.landExpireDate ? basicInfo.landExpireDate.replace("T", " ") : ""}</Col>
                                    </Row>
                                    <Row className='viewRow'>
                                        <Col span={12}>占地面积：{basicInfo.floorSurface}㎡</Col>
                                        <Col span={12}>建筑面积：{basicInfo.builtupArea}㎡</Col>
                                    </Row>
                                    <Row className='viewRow'>
                                        <Col span={12}>容积率：{basicInfo.plotRatio}</Col>
                                        <Col span={12}>绿化率：{basicInfo.greeningRate}%</Col>
                                    </Row>
                                    <Row className='viewRow'>
                                        <Col span={12}>地下停车位：{basicInfo.basementParkingSpace}个</Col>
                                        <Col span={12}>地面停车位：{basicInfo.parkingSpace}个</Col>
                                    </Row>
                                    <Row className='viewRow'>
                                        <Col span={12}>楼栋总数：{basicInfo.buildingNum}栋</Col>
                                        <Col span={12}>商铺总数：{basicInfo.shops}个</Col>
                                    </Row>
                                    <Row className='viewRow'>
                                        <Col span={12}>物业公司：{basicInfo.pmc}</Col>
                                        <Col span={12}>物业费：{basicInfo.pmf}元/㎡</Col>
                                    </Row>
                                    <Row className='viewRow'>
                                        <Col span={12}>开发商：{basicInfo.developer}</Col>
                                    </Row>
                                    <Row className='viewRow'>
                                        <Col span={24}>地址:{basicInfo.address}</Col>
                                    </Row>
                                    {/* <Row className='viewRow'>
                                        <Col span={24}>
                                            {hasRecommendPermission === true && !isRecommend ? <Button type="primary" onClick={() => this.handleRecommend(buildingInfo)} color="blue-inverse">推荐楼盘</Button> : null}
                                            {hasRecommendPermission === true && isRecommend ? <Popconfirm title="确认要取消推荐吗?" onConfirm={() => this.confirmCancelRecommend(recommendInfo)}>
                                                <Button type="primary" color="blue-inverse">取消推荐</Button>
                                            </Popconfirm> : null}
                                        </Col>
                                    </Row> */}
                                </Col>
                            </Row>
                        </Form>
                    </Content>
                </Layout>
            </div>
        )
    }
}
export default BasicInfo;