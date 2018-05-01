import React, {Component} from 'react'
import {Row, Col, Tag} from 'antd'
import {TAG_TEXT_COLOR, TAG_BACKGROUND_COLOR, TAG_COLOR} from '../constants/uiColor';
import {getShopName} from '../utils/utils'

const supportMenus = [{label: '上水', value: 'upperWater'},
{label: '下水', value: 'downWater'},
{label: '天然气', value: 'gas'},
{label: '烟管道', value: 'chimney'},
{label: '排污管道', value: 'blowoff'},
{label: '可分割', value: 'split'},
{label: '电梯', value: 'elevator'},
{label: '扶梯', value: 'staircase'},
{label: '外摆区', value: 'outside'},
{label: '架空层', value: 'openFloor'},
{label: '停车位', value: 'parkingSpace'}];

class ShopResultItem extends Component {
    state = {
    }
    handleShopClick = (shopId) => {
        if (this.props.gotoShopDetail) {
            this.props.gotoShopDetail(shopId);
        }
    }
    //获取销售状态标签
    getSaleTag(shopInfo) {
        let saleStatus = [];
        let findResult = this.props.basicData.find(d => d.groupId === 'ZYW_SHOP_SALE_STATUS');
        if (findResult) {
            saleStatus = findResult.dicPars;
        }
        let saleDic = saleStatus.find(s => s.value === shopInfo.saleStatus);
        return saleDic ? <Tag style={{color: TAG_TEXT_COLOR, backgroundColor: TAG_BACKGROUND_COLOR, marginBottom: '5px'}} size='small'>{saleDic.key}</Tag> : null
    }
    render() {
        const shopInfo = this.props.shopInfo || {};
        const buildingInfo = this.props.buildingInfo;
        if (shopInfo.icon === "") {
            shopInfo.icon = null;
        }
        let tags = [];
        for (let p in shopInfo) {
            let item = supportMenus.find(item => item.value === p.toString() && shopInfo[p] === true);
            if (item) {
                tags.push(item);
            }
        }
        if (shopInfo.price && shopInfo.price % 1 > 0) {
            shopInfo.price = (shopInfo.price * 1).toFixed(2);
        }
        console.log("商铺信息:", shopInfo);
        return (
            <div className='itemBorder'>
                <Row>
                    <Col span={3}>
                        <img style={{width: '75px', height: '75px', marginTop: '10px'}} src={shopInfo.icon || '../../images/default-icon.jpg'} />
                    </Col>
                    <Col span={17}>
                        <Row style={{cursor: 'pointer'}}>
                            <Col span={20} onClick={(e) => this.handleShopClick(shopInfo.id)}>{shopInfo.name}<b className="buildingTitle">{getShopName(shopInfo)}</b>（所属楼盘：{buildingInfo.basicInfo.name}）</Col>
                        </Row>
                        <Row >
                            <Col span={7}>{this.getSaleTag(shopInfo)}{shopInfo.areaFullName}</Col>
                        </Row>
                        <Row>
                            <Col span={7}>建筑面积：{shopInfo.buildingArea || "?"}㎡({shopInfo.width}*{shopInfo.depth})</Col>
                        </Row>
                        <Row >
                            <Col>
                                {tags.map(item => <Tag key={item.value} style={{color: TAG_TEXT_COLOR, backgroundColor: TAG_BACKGROUND_COLOR, marginBottom: '5px'}} size='small'>{item.label}</Tag>)}
                            </Col>
                        </Row>
                    </Col>
                    <Col span={4}>
                        <Row style={{textAlign: 'right'}}>
                            <Col>

                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <label style={{color: 'red'}}>{shopInfo.price ? shopInfo.price + '元/月' : "暂未定价"}</label>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <label>{shopInfo.totalPrice ? shopInfo.totalPrice + '元/㎡/月' : "暂未定价"}</label>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </div>
        )
    }

}

export default ShopResultItem;