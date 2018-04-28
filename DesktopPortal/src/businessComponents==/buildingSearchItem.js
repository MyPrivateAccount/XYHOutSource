import React, {Component} from 'react';
import {Button, Row, Col, Tag, Badge, Popconfirm, Icon} from 'antd';
import {TAG_TEXT_COLOR, TAG_BACKGROUND_COLOR,TAG_COLOR} from '../constants/uiColor';
import './buildingSearchItem.less'

const supportMenus = [{label: '公交', value: 'hasBus'},
{label: '地铁', value: 'hasRail'},
{label: '其他交通', value: 'hasOtherTraffic'},
{label: '幼儿园', value: 'hasKindergarten'},
{label: '小学', value: 'hasPrimarySchool'},
{label: '中学', value: 'hasMiddleSchool'},
{label: '大学', value: 'hasUniversity'},
{label: '商场', value: 'hasMarket'},
{label: '超市', value: 'hasSupermarket'},
{label: '银行', value: 'hasBank'}];

class BuildingResultItem extends Component {
    state = {
    }
    handleBuildingClick = (buildingId) => {
        if (this.props.gotoBuildingDetail) {
            this.props.gotoBuildingDetail(buildingId);
        }
    }
    handleRecommend = (building, type) => {
        console.log("推荐楼盘：", building, type);
        building.recommendType = type;
        if (this.props.openRecommendDialog) {
            this.props.openRecommendDialog(building);
        }
    }
    confirmCancelRecommend = (recommendInfo, type) => {
        console.log("取消推荐：", recommendInfo, type);
        if (this.props.cancelRecommend) {
            this.props.cancelRecommend([recommendInfo.id]);
        }
    }
    render() {
        const recommendInfo = this.props.recommendInfo;
        let hasRegionRecommend = false, hasFilialeRecommend = false;
        if (recommendInfo) {
            hasRegionRecommend = recommendInfo.isRegion;
            hasFilialeRecommend = !recommendInfo.isRegion;
        }

        const buildInfo = this.props.buildInfoProps ? this.props.buildInfoProps : this.props.buildInfo;
        let tags = [];
        for (let b in buildInfo) {
            let item = supportMenus.find(item => item.value === b.toString() && buildInfo[b] === true);
            if (item) {
                tags.push(item);
            }
        }
        return (
            <Row className='itemBorder'>
                    <Col span={19} >
                    <div className='itemLeft'>
                        <div className='imgDiv'>
                            <img  style={{width: '11rem', height:'9rem'}} src={ '../../../images/default-icon.jpg'} />
                        </div>
                        <div className='leftText'>
                            <p style={{display:'flex',flexDirection:'row', alignItems:'center', }}>
                                <b className="buildingTitle" onClick={(e) => this.handleBuildingClick(buildInfo.id)}>{buildInfo.name || '未命名'}</b>
                                {recommendInfo ? 
                                <p style={{ marginLeft: '15px'}}>
                                    <p>
                                        
                                        {hasRegionRecommend ? <div style={{color: TAG_COLOR, fontWeight:'bold'}}><Icon type="like" style={{fontSize: '16px', marginRight:'5px'}} />大区推荐</div> : null}
                                        {hasFilialeRecommend ? <div style={{color: TAG_COLOR, fontWeight:'bold'}}><Icon type="like" style={{fontSize: '16px', marginRight:'5px'}} />公司推荐</div> : null}
                                    </p>
                                </p> : null
                                }
                            </p>
                            <p >{buildInfo.areaFullName}
                           
                            </p>
                            <p>
                            {
                                tags.map(item => <Tag key={item.value} style={{color: TAG_TEXT_COLOR, backgroundColor: TAG_BACKGROUND_COLOR}}>{item.label}</Tag>)
                            }
                            </p>
                        </div>
                    </div>
                    </Col>
                    <Col span={5}>
                        <div className='itemRight'>
                            <p>
                                <label style={{color: 'red',fontSize:'1.3rem',fontWeight:'bold'}}>{buildInfo.minPrice || "?"} — {buildInfo.maxPrice || "?"}</label>
                                <span style={{fontWeight:'bold',marginLeft: '5px'}}>元/㎡</span>
                            </p>
                        </div>
                    </Col>
            </Row >
        )
    }

}

export default BuildingResultItem;