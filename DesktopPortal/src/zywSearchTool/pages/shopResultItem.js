import {connect} from 'react-redux';
import {getShopDetail, setLoading, expandSearchbox} from '../actions/actionCreator';
import React, {Component} from 'react'
import {Button, Row, Col, Tag} from 'antd'
import {getShopName} from '../../utils/utils'

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
    handleShopClick = (id) => {
        this.props.dispatch(setLoading(true));
        this.props.dispatch(getShopDetail(id));
    }
    render() {
        const shopInfo = this.props.shopInfo;
        const activeBuilding = this.props.activeBuilding;
        if (shopInfo.icon == "") {
            shopInfo.icon = null;
        }
        let tags = [];
        for (let p in shopInfo) {
            let item = supportMenus.find(item => item.value === p.toString() && shopInfo[p] === true);
            if (item) {
                tags.push(item);
            }
        }
        //console.log("activeBuilding:", activeBuilding);
        return (
            <div className='itemBorder'>
                <Row>
                    <Col span={3}>
                        <img style={{width: '70px'}} src={shopInfo.icon || '../../../images/default-icon.jpg'} />
                    </Col>
                    <Col span={17}>
                        <Row style={{marginBottom: '10px', cursor: 'pointer'}}>
                            <Col span={20} onClick={(e) => this.handleShopClick(shopInfo.id)}><b className="buildingTitle">{getShopName(shopInfo)}</b>（所属楼盘：{activeBuilding.basicInfo.name}）</Col>
                        </Row>
                        <Row style={{marginBottom: '5px'}}>
                            <Col span={4}>{shopInfo.areaFullName}</Col>
                            <Col >建筑面积：{shopInfo.buildingArea || "?"}㎡({shopInfo.width}*{shopInfo.height})</Col>
                        </Row>
                        <Row >
                            <Col>
                                {tags.map(item => <Tag key={item.value} color="rgb(16, 142, 233)">{item.label}</Tag>)}
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

function mapStateToProps(state) {
    return {
        activeBuilding: state.search.activeBuilding
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(ShopResultItem);