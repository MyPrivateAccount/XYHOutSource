import { connect } from 'react-redux';
import { openAuditDetail, getAuditHistory, getActiveDetail, getAuditDetail, getZywActiveDetail } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { notification, Row, Col, Icon } from 'antd'
import moment from 'moment';
import AuditHouseSource from '../auditHouseSource';
import AuditCustomer from '../auditCustomer';
import AuditHouseNewInfo from '../auditHouseNewInfo'
import { setLoadingVisible } from '../../../customerManager/actions/actionCreator';
import AuditBuildingOnSite from '../auditBuildingsOnSite';
import AuditDealInfo from '../auditDealInfo';
import ZYWAuditDealInfo from '../zywAuditDealInfo';
import AuditPicture from '../auditPicture';
import AuditDealBack from '../auditDealBack';
import ZYWAuditDealBack from '../ZYWAuditDealBack';
import AuditContract from '../auditContract';
import { getReviewDefine } from '../../../tools'

const itemStyle = {
    itemBorder: {
        height: '80px',
        border: '1px solid #ccc',
        width: '80%',
        margin: '5px 10px',
        padding: '3px',
        borderRadius: '5px'
    },
    img: {
        width: '70px',
        border: '1px solid #ccc',
        borderRadius: '5px',
        verticalAlign: 'middle'
    }
}
//审核类型
export const auditType = {
    building: {
        name: '新增楼盘',
        icon: <i className='iconfont icon-xinzengfangyuan_icon' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseSource sourceType='building' company="xyh" />
    },
    shops: {
        name: '新增商铺',
        icon: <Icon type='shop' style={{ fontSize: '48px', marginTop: '10px' }} />,
        component: <AuditHouseSource sourceType='shop' company="xyh" />
    },
    ShopsHot: {//热卖不需要审核
        name: '热卖户型',
        icon: <i className='iconfont icon-remai' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='热卖户型' />
    },
    ShopsAdd: {
        name: '商铺加推',
        icon: <i className='iconfont icon-jiatui' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='加推' />
    },
    ReportRule: {
        name: '报备规则',
        icon: <i className='iconfont icon-guize' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='报备规则' />
    },
    CommissionType: {
        name: '佣金方案',
        icon: <i className='iconfont icon-yongjin' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='佣金方案' />
    },
    BuildingNo: {
        name: '楼栋批次',
        icon: <i className='iconfont icon-pici' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='楼栋批次' />
    },
    DiscountPolicy: {
        name: '优惠政策',
        icon: <i className='iconfont icon-youhui' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='优惠政策' />
    },
    Image: {
        name: '图片',
        icon: <i className='iconfont icon-tupian' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='图片' />
    },
    Attachment: {
        name: '附件',
        icon: <i className='iconfont icon-fujian' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='附件' />
    },
    Price: {
        name: '价格',
        icon: <i className='iconfont icon-zongjia' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='价格' />
    },
    TransferCustomer: {
        name: '调客审核',
        icon: <Icon type="export" style={{ fontSize: '48px', marginTop: '10px' }} />,
        component: <AuditCustomer />
    },
    BuildingsOnSite: {
        name: '指派驻场',
        icon: <i className='iconfont icon-manager' style={{ fontSize: '48px' }}></i>,
        component: <AuditBuildingOnSite />
    },
    CustomerDeal: {
        name: '成交信息',
        icon: <Icon type="bank" style={{ fontSize: '48px', marginTop: '10px' }} />,
        component: <AuditDealInfo />
    },
    BuildingPicture: {
        name: '楼盘图片',
        icon: <i className='iconfont icon-tupian' style={{ fontSize: '48px' }}></i>,
        component: <AuditPicture picType='building' />
    },
    ShopPicture: {
        name: '商铺图片',
        icon: <i className='iconfont icon-tupian' style={{ fontSize: '48px' }}></i>,
        component: <AuditPicture picType='shop' />
    },
    CustomerDealBack: {
        name: '成交回退',
        icon: <i className='iconfont icon-dijia' style={{ fontSize: '48px' }}></i>,
        component: <AuditDealBack />
    },
    ///////   >>>>>>>>>>>>>>>>  租壹屋 >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>. //////
    ZYWBuilding: {
        name: '租壹屋新增楼盘',
        icon: <i className='iconfont icon-xinzengfangyuan_icon' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseSource sourceType='building' company="zyw" />
    },
    ZYWShops: {
        name: '租壹屋新增商铺',
        icon: <Icon type='shop' style={{ fontSize: '48px', marginTop: '10px' }} />,
        component: <AuditHouseSource sourceType='shop' company="zyw" />
    },
    ZYWCustomerDeal: {
        name: '租壹屋成交信息',
        icon: <Icon type="bank" style={{ fontSize: '48px', marginTop: '10px' }} />,
        component: <ZYWAuditDealInfo />
    },
    ZYWCustomerDealBack: {
        name: '租壹屋成交回退',
        icon: <i className='iconfont icon-dijia' style={{ fontSize: '48px' }}></i>,
        component: <ZYWAuditDealBack />
    },
    ZYWOwnerInfo: {
        name: '租壹屋业主信息',
        icon: <Icon type='user' style={{ fontSize: '48px', marginTop: '10px' }} />,
        component: <AuditHouseNewInfo subTitle="租壹屋业主信息" />
    },
    ZYWBuildingPicture: {
        name: '租壹屋楼盘图片',
        icon: <i className='iconfont icon-tupian' style={{ fontSize: '48px' }}></i>,
        component: <AuditPicture picType='building' />
    },
    ZYWIntensionLease: {
        name: '租壹屋意向租约',
        icon: <Icon type='file-text' style={{ fontSize: '48px', marginTop: '10px' }} />,
        component: <AuditHouseNewInfo subTitle="租壹屋意向租约" />
    },
    ZYWDiscountPolicy: {
        name: '租壹屋优惠政策',
        icon: <i className='iconfont icon-youhui' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='租壹屋优惠政策' />
    },
    ZYWCommissionType: {
        name: '租壹屋佣金方案',
        icon: <i className='iconfont icon-yongjin' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='租壹屋佣金方案' />
    },
    ZYWBuildingNo: {
        name: '租壹屋楼栋批次',
        icon: <i className='iconfont icon-pici' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='租壹屋楼栋批次' />
    },
    ZYWReportRule: {
        name: '租壹屋报备规则',
        icon: <i className='iconfont icon-guize' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='租壹屋报备规则' />
    },
    ZYWPrice: {
        name: '租壹屋价格',
        icon: <i className='iconfont icon-zongjia' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='租壹屋价格' />
    },
    ZYWShopPicture: {
        name: '租壹屋商铺图片',
        icon: <i className='iconfont icon-tupian' style={{ fontSize: '48px' }}></i>,
        component: <AuditPicture picType='shop' />
    },
    ZYWShopsHot: {
        name: '租壹屋热租户型',
        icon: <i className='iconfont icon-remai' style={{ fontSize: '48px' }}></i>,
        component: <AuditHouseNewInfo subTitle='租壹屋热租户型' />
    },
    ZYWBuildingsOnSite: {
        name: '租壹屋指派驻场',
        icon: <i className='iconfont icon-manager' style={{ fontSize: '48px' }}></i>,
        component: <AuditBuildingOnSite />
    },
    ContractCommit: {
        name: "合同",
        icon: <i className='book' style={{ fontSize: '48px' }}></i>,
        component: <AuditContract />
    }
}

class AuditRecordItem extends Component {
    state = {
    }

    handleAuditClick = (auditInfo) => {
        console.log(auditInfo, '???')
        if (!auditType[auditInfo.contentType]) {
            let rd = getReviewDefine(auditInfo.contentType)
            if(!rd){
                notification.error({
                    description: '暂不支持此种类型的审核!',
                    duration: 3
                });
                return;
            }
            
        }
        if (this.props.onClick) {
            this.props.onClick(auditInfo);
        }
    }
    getAuditTooltip(auditInfo) {
        let tooltip = '';
        if (auditType[auditInfo.contentType]) {
            tooltip = auditType[auditInfo.contentType].name;
        }
        return tooltip;
    }
    getAuditIcon(auditInfo) {
        let icon = '';
        let ai = auditType[auditInfo.contentType];
        if (ai) {
            icon = auditType[auditInfo.contentType].icon;
        } else {
            let rd = getReviewDefine(auditInfo.contentType)
            if (rd) {
                return rd.icon;
            }
        }
        return icon;
    }

    render() {
        let auditInfo = { ...this.props.auditInfo };
        if (auditInfo.submitTime) {
            auditInfo.submitTime = moment(auditInfo.submitTime).format("YYYY-MM-DD HH:mm:ss");
        }
        let contentTypeText = this.getAuditTooltip(auditInfo);
        if (auditInfo.contentType === "shops") {
            auditInfo.contentName = auditInfo.ext4;
            if (auditInfo.ext1 && auditInfo.ext2 && auditInfo.ext3) {
                auditInfo.ext2 = auditInfo.ext1 + "-" + (auditInfo.ext2 || '').replace("-", "") + "-" + auditInfo.ext3;
            }
        }

        let rd = getReviewDefine(auditInfo.contentType)
        let descLine = null;
        if (rd) {
            descLine = rd.sencordLine;
            contentTypeText = rd.title;
        }

        return (
            <div style={itemStyle.itemBorder}>
                <Row style={{ cursor: 'pointer' }} onClick={(e) => this.handleAuditClick(auditInfo)}>
                    <Col span={3}>
                        {this.getAuditIcon(auditInfo)}
                    </Col>
                    <Col span={16}>
                        <Row style={{ marginBottom: '10px', cursor: 'pointer' }}>
                            <Col span={20} ><b>{contentTypeText}</b></Col>
                        </Row>

                        {
                            descLine ?
                                <Row style={{ marginBottom: '5px', fontSize: '0.9rem' }}>{descLine(auditInfo)}</Row>
                                :
                                <Row style={{ marginBottom: '5px', fontSize: '0.9rem' }}>
                                    <Col >{auditInfo.contentName || ""}{auditInfo.ext2 ? "(" + auditInfo.ext2 + ")" : null}</Col>
                                    <Col >{auditInfo.desc}</Col>
                                </Row>
                        }


                    </Col>
                    <Col span={5}>
                        <Row style={{ marginBottom: '20px', textAlign: 'right' }}>
                            <Col>
                                {auditInfo.submitTime}
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
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditRecordItem);