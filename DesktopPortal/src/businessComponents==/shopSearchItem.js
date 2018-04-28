import React, {Component} from 'react'
import {Row, Col, Tag} from 'antd'
import {TAG_TEXT_COLOR, TAG_BACKGROUND_COLOR, TAG_COLOR} from '../constants/uiColor';

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
        return (
            <div className='itemBorder'>
                <Row>
                    <Col span={3}>
                        <img style={{width: '70px', height: '70px'}} src={shopInfo.icon || '../../images/default-icon.jpg'} />
                    </Col>
                    <Col span={17}>
                        <Row style={{marginBottom: '10px', cursor: 'pointer'}}>
                            <Col span={20} onClick={(e) => this.handleShopClick(shopInfo.id)}><b className="buildingTitle">{shopInfo.buildingNo}-{shopInfo.floorNo}-{shopInfo.number}</b>（所属楼盘：{buildingInfo.basicInfo.name}）</Col>
                        </Row>
                        <Row style={{marginBottom: '5px'}}>
                            <Col span={5}>{shopInfo.areaFullName}</Col>
                            <Col >建筑面积：{shopInfo.buildingArea || "?"}㎡({shopInfo.width}*{shopInfo.height})</Col>
                        </Row>
                        <Row >
                            <Col>
                                {tags.map(item => <Tag key={item.value} style={{color: TAG_TEXT_COLOR, backgroundColor: TAG_BACKGROUND_COLOR, marginBottom: '5px'}} size='small'>{item.label}</Tag>)}
                            </Col>
                        </Row>
                    </Col>
                    <Col span={4}>
                        <Row style={{marginBottom: '20px', textAlign: 'right'}}>
                            <Col>

                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <label style={{color: 'red'}}>{shopInfo.price || "?"}元/㎡</label>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </div>
        )
    }

}

export default ShopResultItem;