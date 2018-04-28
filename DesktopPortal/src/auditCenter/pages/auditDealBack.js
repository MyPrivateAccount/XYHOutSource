import {connect} from 'react-redux';
import {getCustomerDealInfo, setLoadingVisible} from '../actions/actionCreator';
import {getDicParList} from '../../actions/actionCreators'
import React, {Component} from 'react';
import {Row, Col, Modal, Upload} from 'antd';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory';
import moment from 'moment'

const styles = {

}
class AuditDealBack extends Component {
    state = {
        review: null,
        content: null,
        record: null,
        turnover: null
    }
    componentWillReceiveProps(newProps) {
        // const activeAuditHistory = newProps.activeAuditHistory || {};
        // if (this.state.turnover == null) {
        //     if (activeAuditHistory.submitDefineId) {
        //         this.props.dispatch(setLoadingVisible(true));
        //         this.props.dispatch(getCustomerDealInfo(activeAuditHistory.submitDefineId));
        //     }
        //     if (Object.keys(newProps.dealInfo).length > 0) {
        //         this.setState({turnover: newProps.dealInfo});
        //     }
        // }
    }
    componentDidMount() {
        this.props.dispatch(getDicParList(['XK_SELLER']));
        const submitDefineId = (this.props.activeAuditInfo || {}).submitDefineId;
        if (submitDefineId) {
            this.props.dispatch(setLoadingVisible(true));
            this.props.dispatch(getCustomerDealInfo(submitDefineId));
        }
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
    componentDidMount = () => {
        this.props.dispatch(getDicParList(['XK_SELLER']));
    }

    render() {
        const activeAuditInfo = this.props.activeAuditInfo;
        let {review, turnover} = this.state;
        review = review || {};
        turnover = turnover || {};
        turnover = this.props.turnover ? this.props.turnover : turnover
        let seller = '';
        let st = (turnover.sellerType || 2) * 1;
        if (st === 1) {
            seller = '自售';
        } else if (st === 2) {
            seller = this.getDicName(this.props.rootBasicData.dicList, 'XK_SELLER', turnover.seller);
        } else if (st === 10) {
            seller = turnover.seller;
        }
        return (<div>
            <div style={[styles.flex]}><span style={[styles.textWidth]}>销售类型：</span><span className="row-value">
                {seller}
            </span></div>
            {
                st === 1 ? (
                    <div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>楼盘名：</span><span className="row-value">{turnover.buildingName || ''}</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>商铺：</span><span className="row-value">{turnover.shopName || ''}</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>业务员：</span><span className="row-value">{turnover.userName || ''}  {turnover.departmentName || ''}</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>业务员电话：</span><span className="row-value">{turnover.userPhone || ''}</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>客户：</span><span className="row-value">{turnover.customerName || ''}</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>客户电话：</span><span className="row-value">{turnover.customerPhone || ''}</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>佣金：</span><span className="row-value">{turnover.commission || ''}元</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>成交总价：</span><span className="row-value">{turnover.totalPrice || ''}元</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>成交日期：</span><span className="row-value">{moment(turnover.createTime).format('YYYY-MM-DD HH:mm')}</span></div>
                    </div>
                ) : null


            }
            {
                (st === 2 || st === 10) ? (
                    <div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>楼盘名：</span><span className="row-value">{turnover.buildingName || ''}</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>商铺：</span><span className="row-value">{turnover.shopName || ''}</span></div>
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>分销商：</span><span className="row-value">{seller}</span></div>
                        {
                            (turnover.proprietor) ? <div style={[styles.flex]}><span style={[styles.textWidth]}>客户：</span><span className="row-value">{turnover.proprietor || ''}</span></div> : null
                        }
                        {
                            (turnover.mobile) ? <div style={[styles.flex]}><span style={[styles.textWidth]}>客户电话：</span><span className="row-value">{turnover.mobile || ''}</span></div> : null
                        }
                        {
                            (turnover.mobile) ? <div style={[styles.flex]}><span style={[styles.textWidth]}>身份证：</span><span className="row-value">{turnover.idcard || ''}</span></div> : null
                        }
                        {
                            (turnover.address) ? <div style={[styles.flex]}><span style={[styles.textWidth]}>地址：</span><span className="row-value">{turnover.address || ''}</span></div> : null
                        }
                        <div style={[styles.flex]}><span style={[styles.textWidth]}>成交日期：</span><span className="row-value">{moment(turnover.createTime).format('YYYY-MM-DD HH:mm')}</span></div>
                    </div>
                ) : null
            }
            <Row>
                <Col>
                    <div style={{border: '1px solid red'}}>
                        {/**审核记录**/}
                        <AuditHistory />
                    </div>
                </Col>
            </Row>
            {
                activeAuditInfo.examineStatus === 1 ? <AuditForm /> : null
            }
        </div>
        )
    }
}
function mapStateToProps(state) {
    return {
        activeAuditInfo: state.audit.activeAuditInfo,
        activeAuditHistory: state.audit.activeAuditHistory,
        dealInfo: state.audit.dealInfo,
        rootBasicData: state.rootBasicData
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditDealBack);