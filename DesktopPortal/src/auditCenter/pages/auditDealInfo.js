import {connect} from 'react-redux';
import {getCustomerDealInfo, setLoadingVisible} from '../actions/actionCreator';
import {getDicParList} from '../../actions/actionCreators'
import React, {Component} from 'react';
import {Row, Col, notification, Spin} from 'antd';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory';
import moment from 'moment'


const styles = {
    rowLabel: {
        // fontWeight:'bold'
    }
}

class AuditDealInfo extends Component {
    state = {
        review: null,
        content: null,
        record: null,
        submitDefineId: null
    }

    componentWillReceiveProps(newProps) {
        // const submitDefineId = this.state.submitDefineId;
        const activeAuditInfo = newProps.activeAuditInfo;
        const activeAuditHistory = newProps.activeAuditHistory;
        let submitDefineId = activeAuditInfo.submitDefineId || activeAuditHistory.submitDefineId;
        if (submitDefineId && !this.state.submitDefineId) {
            this.setState({submitDefineId: submitDefineId}, () => {
                this.props.dispatch(getCustomerDealInfo(submitDefineId));
            });
        }
    }


    componentDidMount = () => {

        // console.log("当前审核信息:", activeAuditInfo);
        this.props.dispatch(getDicParList(['XK_SELLER']));
    }
    getDicName = (dicList, group, value) => {
        let dicGroup = dicList.find(x => x.groupId === group);
        if (dicGroup && dicGroup.dictionaryDefines) {
            let dic = dicGroup.dictionaryDefines.find(x => x.value === value);
            if (dic) {
                return dic.key;
            }
        }

        return '';
    }
    render() {
        const activeAuditInfo = this.props.activeAuditInfo;
        let turnover = this.props.dealInfo || {};
        let seller = '';
        let st = (turnover.sellerType || 1) * 1;
        if (st === 1) {
            seller = '自售';
        } else if (st === 2) {
            seller = this.getDicName(this.props.rootBasicData.dicList || [], 'XK_SELLER', turnover.seller);
        } else if (st === 10) {
            seller = turnover.seller;
        }
        return (<div>
            <div><span style={styles.rowLabel}>销售类型</span>：<span className="row-value">
                {seller === '自售' ? '自售' : '第三方销售'}
            </span></div>
            {
                st === 1 ? (
                    <div>
                        <div><span style={styles.rowLabel}>楼盘名</span>：<span className="row-value">{turnover.buildingName || ''}</span></div>
                        <div><span style={styles.rowLabel}>商铺</span>：<span className="row-value">{turnover.shopName || ''}</span></div>
                        <div><span style={styles.rowLabel}>业务员</span>：<span className="row-value">{turnover.userName || ''}  ({turnover.departmentName || ''})</span></div>
                        <div><span style={styles.rowLabel}>业务员电话</span>：<span className="row-value">{turnover.userPhone || ''}</span></div>
                        {/* <div><span style={styles.rowLabel}>组别：</span><span className="row-value">{turnover.departmentName||''}</span></div> */}
                        <div><span style={styles.rowLabel}>客户</span>：<span className="row-value">{turnover.customerName || ''}</span></div>
                        <div><span style={styles.rowLabel}>客户电话</span>：<span className="row-value">{turnover.customerPhone || ''}</span></div>
                        <div><span style={styles.rowLabel}>成交总价</span>：<span className="row-value">{turnover.totalPrice || ''}元</span></div>
                        <div><span style={styles.rowLabel}>佣金</span>：<span className="row-value">{turnover.commission || ''}元</span></div>
                        <div><span style={styles.rowLabel}>成交日期</span>：<span className="row-value">{moment(turnover.createTime).format('YYYY-MM-DD HH:mm') || ''}</span></div>
                    </div>
                ) : null


            }
            {
                (st === 2 || st === 10) ? (
                    <div>
                        <div><span style={styles.rowLabel}>楼盘名</span>：<span className="row-value">{turnover.buildingName || ''}</span></div>
                        <div><span style={styles.rowLabel}>商铺</span>：<span className="row-value">{turnover.shopName || ''}</span></div>
                        <div><span style={styles.rowLabel}>分销商</span>：<span className="row-value">{seller}</span></div>
                        <div><span style={styles.rowLabel}>业务员电话</span>：<span className="row-value">{turnover.userPhone || ''}</span></div>
                        <div><span style={styles.rowLabel}>成交日期</span>：<span className="row-value">{moment(turnover.createTime).format('YYYY-MM-DD HH:mm') || ''}</span></div>
                        {/* <div><span style={styles.rowLabel}>组别：</span><span className="row-value">{turnover.departmentName||''}</span></div> */}
                        {
                            (turnover.proprietor) ? <div><span style={styles.rowLabel}>客户</span>：<span className="row-value">{turnover.proprietor || ''}</span></div> : null
                        }
                        {
                            (turnover.mobile) ? <div><span style={styles.rowLabel}>客户电话</span>：<span className="row-value">{turnover.mobile || ''}</span></div> : null
                        }
                        {
                            (turnover.mobile) ? <div><span style={styles.rowLabel}>身份证</span>：<span className="row-value">{turnover.idcard || ''}</span></div> : null
                        }
                        {
                            (turnover.address) ? <div><span style={styles.rowLabel}>地址</span>：<span className="row-value">{turnover.address || ''}</span></div> : null
                        }

                    </div>
                ) : null
            }
            <div>
                {/**审核记录**/}
                <AuditHistory />
            </div>
            {
                activeAuditInfo.examineStatus === 1 ? <AuditForm /> : null
            }
        </div>)
    }
}
function mapStateToProps(state) {
    console.log("rootBasicDatarootBasicDatarootBasicData:", state.rootBasicData);
    return {
        activeAuditInfo: state.audit.activeAuditInfo,
        activeAuditHistory: state.audit.activeAuditHistory,
        dealInfo: state.audit.dealInfo,
        rootBasicData: state.rootBasicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditDealInfo);