import {connect} from 'react-redux';
import {openRecommendDialog, cancelRecommend} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Tag, Badge, Popconfirm} from 'antd';
import {TAG_TEXT_COLOR, TAG_BACKGROUND_COLOR, TAG_COLOR} from '../../constants/uiColor';

const recommendStyle = {
    button: {
        color: TAG_COLOR,
        border: '1px solid ' + TAG_COLOR,
        backgroundColor: '#ffffff'
    }
}

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
    handleRecommend = (building, type) => {
        console.log("推荐楼盘：", building, type);
        building.recommendType = type;
        this.props.dispatch(openRecommendDialog(building));
    }
    confirmCancelRecommend = (recommendInfo, type) => {
        console.log("取消推荐：", recommendInfo, type);
        if (type === "region") {
            this.props.dispatch(cancelRecommend([recommendInfo.regionRecommendID]));
        } else {
            this.props.dispatch(cancelRecommend([recommendInfo.filialeRecommendID]));
        }
        if (this.props.refresh) {
            setTimeout(() => this.props.refresh(), 300);
        }
    }
    //查看楼盘详细
    viewBuildingDetail = () => {
        const buildInfo = this.props.buildInfoProps ? this.props.buildInfoProps : this.props.buildInfo;
        if (this.props.viewBuildingDetail) {
            this.props.viewBuildingDetail(buildInfo.id);
        }
    }
    render() {
        //const recommendInfo = this.props.recommendInfo;
        const buildInfo = this.props.buildInfoProps ? this.props.buildInfoProps : this.props.buildInfo;
        let tags = [];
        for (let b in buildInfo) {
            let item = supportMenus.find(item => item.value === b.toString() && buildInfo[b] === true);
            if (item) {
                tags.push(item);
            }
        }
        const myRecommendList = this.props.searchInfo.myRecommendList;
        const hasRegionRecommendPermission = this.props.judgePermissions.includes("RECOMMEND_REGION");
        const hasFilialeRecommendPermission = this.props.judgePermissions.includes("RECOMMEND_FILIALE");
        let hasRegionRecommend = false, hasFilialeRecommend = false;
        let regionRecommendInfo = {}, filialeRecommenInfo = {};
        let result = myRecommendList.find(r => r.buildingId === buildInfo.id);

        if (result) {//两条记录表示两种推荐都有
            regionRecommendInfo = result;
            filialeRecommenInfo = result;
            hasRegionRecommend = result.isRegion;
            hasFilialeRecommend = result.isFiliale;
            // regionRecommendInfo = result.find(r => r.isRegion === true);
            // if (regionRecommendInfo) {
            //     hasRegionRecommend = true;
            // }
            // filialeRecommenInfo = result.find(r => r.isRegion === false);
            // if (filialeRecommenInfo) {
            //     hasFilialeRecommend = true;
            // }
        }

        return (
            <div className='RecommendItemBorder'>
                <Row>
                    <Col span={19}>
                        <div className='itemLeft'>
                            <div className='imgDiv'>
                                <img style={{width: '11rem', height: '9rem'}} src={'../../../images/default-icon.jpg'} />
                            </div>
                            {/* <img style={{width: '70px', height: '70px'}} src={buildInfo.icon || '../../../images/default-icon.jpg'} /> */}
                            {/* </Col>
                        <Col span={17}> */}
                            <div className='leftText'>
                                <p style={{display: 'flex', flexDirection: 'row', alignItems: 'center', }}>
                                    <b onClick={this.viewBuildingDetail} className="buildingTitle">{buildInfo.name || '未命名'}</b>
                                </p>
                                <p >{buildInfo.areaFullName}</p>
                                <p>
                                    {
                                        tags.map(item => <Tag key={item.value} style={{color: TAG_TEXT_COLOR, backgroundColor: TAG_BACKGROUND_COLOR}}>{item.label}</Tag>)
                                    }
                                </p>
                            </div>
                        </div>
                    </Col>
                    <Col span={5}>
                        {
                            //外部引用时不显示推荐
                            this.props.buildInfoProps === undefined ?
                                <Row style={{marginBottom: '20px', textAlign: 'right'}}>
                                    <Col>
                                        {hasRegionRecommendPermission === true && !hasRegionRecommend ? <Tag onClick={() => this.handleRecommend(buildInfo, "region")} style={recommendStyle.button}>大区推荐</Tag> : null}
                                        {hasRegionRecommend === true && hasRegionRecommend ? <Popconfirm title="确认要取消推荐吗?" onConfirm={() => this.confirmCancelRecommend(regionRecommendInfo, 'region')}>
                                            <Tag color={TAG_COLOR}>取消大区推荐</Tag>
                                        </Popconfirm> : null}

                                        {hasFilialeRecommendPermission === true && !hasFilialeRecommend ? <Tag onClick={() => this.handleRecommend(buildInfo, "filiale")} style={recommendStyle.button}>公司推荐</Tag> : null}
                                        {hasFilialeRecommend === true && hasFilialeRecommend ? <Popconfirm title="确认要取消推荐吗?" onConfirm={() => this.confirmCancelRecommend(filialeRecommenInfo, 'filiale')}>
                                            <Tag color={TAG_COLOR}>取消公司推荐</Tag>
                                        </Popconfirm> : null}
                                    </Col>
                                </Row> : null
                        }
                        <Row>
                            <Col>
                                <label style={{color: 'red', fontSize: '1.3rem', fontWeight: 'bold'}}>
                                    {buildInfo.minPrice || "?"} — {buildInfo.maxPrice || "?"}</label>
                                <span style={{fontWeight: 'bold', marginLeft: '5px'}}>元/㎡</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </div >
        )
    }

}

function mapStateToProps(state) {
    return {
        judgePermissions: state.judgePermissions || [],
        searchInfo: state.search,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BuildingResultItem);