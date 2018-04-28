import React, {Component} from 'react'
import {Layout, Form, Col, Row, Icon, Tag} from 'antd'
import Slider from 'react-slick';
import {TAG_COLOR} from '../../constants/uiColor';
import {getShopName} from '../../utils/utils'

const {Header, Sider, Content} = Layout;
const towards = [{label: "西", key: 3},
{label: "东", key: 1},
{label: "南", key: 2},
{label: "北", key: 4},
{label: "东南", key: 5},
{label: "西南", key: 6},
{label: "西北", key: 7},
{label: "东北", key: 8}];
class BasicInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }

    getSaleStatus(key) {
        if (key == "1") {
            key = "待售";
        } else if (key == "10") {
            key = "已售";
        }
        else if (key == "2") {
            key = "在售";
        } else if (key == "3") {
            key = "锁定";
        }
        return key;
    }
    getTowardText(key) {
        let toward = towards.find(t => t.key == key);
        if (toward) {
            key = toward.label;
        }
        return key;
    }
    getTextByCode(dic, code) {
        let text = "";
        let isArray = Array.isArray(code);
        dic.map(item => {
            if (isArray) {
                if (code.find((c) => c === item.value)) {
                    text = item.key;
                }
            } else {
                if (item.value === code) {
                    text = item.key;
                }
            }
        });
        return text;
    }
    render() {
        let basicInfo = (this.props.shopInfo || {}).basicInfo || {};
        let buildingBasic = this.props.buildingBasic || {};
        let fileList = this.props.shopInfo.fileList || [];
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
                <Content className='content basicEdit' style={{backgroundColor: "#ECECEC"}}>
                    <Row type="flex" style={{padding: '1rem 0'}}>
                        <Col span={23} >
                            <Icon type="tags-o" className='content-icon' /> <span className='content-title'>基本信息</span>
                        </Col>
                        <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                    </Row>

                    <Row className="buildingPic" style={{display: this.state.expandStatus ? "block" : "none"}}>
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
                                <Col span={24} >
                                    <Tag color={TAG_COLOR}>{this.getSaleStatus(basicInfo.saleStatus)}</Tag>
                                    <span className="buildingNo">{getShopName(basicInfo)}</span> <span className="buildingName">{buildingBasic.name}</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col>{basicInfo.houseArea}㎡({basicInfo.width}*{basicInfo.depth})<span style={{margin: '0 5px', fontSize: '1.1rem'}}>|</span>{basicInfo.floorNo}层</Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12}>总价：<span className='redSpan'>{basicInfo.totalPrice ? basicInfo.totalPrice + '元' : '暂未定价'}</span></Col>
                                <Col span={12}>单价：<span className='redSpan'>{basicInfo.price ? basicInfo.price + '元/㎡' : '暂未定价'}</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col>最低售卖总价:<span className='redSpan'>{basicInfo.guidingPrice ? basicInfo.guidingPrice + '元' : '暂未定价'}</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12}>建筑面积：<span className='boldSpan'>{basicInfo.buildingArea}㎡</span></Col>
                                <Col span={12}>套内面积：<span className='boldSpan'>{basicInfo.houseArea}㎡</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12}>朝向：<span>{this.getTowardText(basicInfo.toward)}</span></Col>
                                <Col span={12}>可外摆区域：<span className='boldSpan'>{basicInfo.outsideArea}㎡</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={24}>地址:{buildingBasic.address}</Col>
                            </Row>
                            <Row className='viewRow' style={{marginTop: '10px'}}>
                                <Col span={24}>
                                    {
                                        basicInfo.isCorner ? <Tag color={TAG_COLOR}>拐角铺</Tag> : null
                                    }
                                    {
                                        basicInfo.hasStreet ? <Tag color={TAG_COLOR}>双边街</Tag> : null
                                    }
                                    {
                                        basicInfo.hasFree ? <Tag color={TAG_COLOR}>赠送({basicInfo.freeArea}㎡)</Tag> : null
                                    }
                                    {
                                        basicInfo.isFaceStreet ? <Tag color={TAG_COLOR}>临街({basicInfo.streetDistance}m)</Tag> : null
                                    }
                                    {
                                        basicInfo.shopCategory ? <Tag color={TAG_COLOR}>{this.getTextByCode(this.props.basicData.shopsTypes || [], basicInfo.shopCategory)}</Tag> : null
                                    }
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Content>
            </Layout>
        )
    }
}

export default BasicInfo;