import { connect } from 'react-redux';
//import { editShopBasic } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Popconfirm, Form, Col, Row, Icon, Tag } from 'antd'
import Slider from 'react-slick'
import '../../../../../node_modules/slick-carousel/slick/slick.css'
import '../../../../../node_modules/slick-carousel/slick/slick-theme.css'

const { Header, Sider, Content } = Layout;
const FormItem = Form.Item;

class BasicInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }

    getSaleStatus(key) {
        if (key === "1") {
            key = "待售";
        } else if (key === "10") {
            key = "已售";
        }
        else if (key === "2") {
            key = "在售";
        }
        return key;
    }
    render() {
        let basicInfo = this.props.shopInfo.basicInfo;
        let buildingBasic = this.props.buildingBasic;
        let fileList = this.props.shopInfo.fileList;
        if (fileList && fileList.length == 0) {
            fileList.push({
                "fileGuid": "000-0000-0000-000",
                "icon": "../../../../../images/noPicture.png",
                "original": "string",
                "medium": "string",
                "small": "../../../../../images/noPictureBig.png"
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
            <Layout>
                <Content className='content basicEdit' style={{ backgroundColor: "#ECECEC" }}>
                    <Form layout="horizontal" >
                        <Row type="flex" style={{ padding: '1rem 0' }}>
                            <Col span={23} >
                                <Icon type="tags-o" className='content-icon' /> <span className='content-title'>基本信息</span>
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({ expandStatus: !this.state.expandStatus })} /></Col>
                        </Row>

                        <Row className="buildingPic" style={{ display: this.state.expandStatus ? "block" : "none" }}>
                            <Col span={12}>
                                {/**
                             * 商铺图片展示
                             */}
                                <Slider {...sliderSettings}>
                                    {
                                        fileList.map((f, i) => <div key={i}><img src={f.small} /></div>)
                                    }
                                </Slider>
                            </Col>
                            <Col span={12}>
                                {/**
                             * 基础详细
                             */}
                                <Row className='viewRow'>
                                    <Col span={12} >
                                        <Tag color="#108ee9">{this.getSaleStatus(basicInfo.saleStatus)}</Tag>
                                        <span className="buildingNo">{basicInfo.buildingNo}-{basicInfo.floorNo}-{basicInfo.number}</span> <span className="buildingName">{buildingBasic.name}</span></Col>
                                    <Col span={12}></Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12}>总价：<span className='redSpan'>{basicInfo.totalPrice}万元</span></Col>
                                    <Col span={12}>单价：<span className='redSpan'>{basicInfo.price}万元/㎡</span></Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12}>建筑面积：<span className='boldSpan'>{basicInfo.buildingArea}㎡</span></Col>
                                    <Col span={12}>套内面积：<span className='boldSpan'>{basicInfo.houseArea}㎡</span></Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={12}>朝向：<span>{basicInfo.toward}</span></Col>
                                    <Col span={12}>可外摆区域：<span className='boldSpan'>{basicInfo.outsideArea}㎡</span></Col>
                                </Row>
                                <Row className='viewRow'>
                                    <Col span={24}>地址:{buildingBasic.address}</Col>
                                </Row>
                                <Row className='viewRow' style={{ marginTop: '10px' }}>
                                    <Col span={24}>
                                        {
                                            basicInfo.isCorner ? <span className='someSpan'>拐角铺</span> : null
                                        }
                                        {
                                            basicInfo.hasStreet ? <span className='someSpan'>双边街</span> : null
                                        }
                                        {
                                            basicInfo.hasFree ? <span className='someSpan'>赠送({basicInfo.freeArea}㎡)</span> : null
                                        }
                                        {
                                            basicInfo.isFaceStreet ? <span className='someSpan'>临街({basicInfo.streetDistance}m)</span> : null
                                        }

                                    </Col>
                                </Row>
                            </Col>
                        </Row>



                    </Form>
                </Content>
            </Layout>
        )
    }
}

function mapStateToProps(state) {
    return {
        shopInfo: state.search.activeShop,
        buildingBasic: state.search.activeBuilding,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BasicInfo);