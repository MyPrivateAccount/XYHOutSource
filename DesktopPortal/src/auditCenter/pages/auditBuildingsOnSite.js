import {connect} from 'react-redux';
import {getUpdateRecordDetail, setLoadingVisible, getBuildingDetail, getBuildingShops} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory';
import getToolComponent from '../../tools';

class AuditBuildingOnSite extends Component {
    state = {

    }
    componentWillMount() {
        let activeAuditInfo = this.props.activeAuditInfo;
        // this.props.dispatch(getBuildingDetail(activeAuditInfo.contentId));
        // this.props.dispatch(getBuildingShops(activeAuditInfo.contentId));
    }
    // componentWillReceiveProps(newProps) {
    //     let activeAuditHistory = newProps.activeAuditHistory;
    //     //console.log("当前审核信息:", activeAuditHistory);
    //     if (activeAuditHistory && activeAuditHistory.updateRecord === undefined) {
    //         this.props.dispatch(setLoadingVisible(true));
    //         this.props.dispatch(getUpdateRecordDetail(activeAuditHistory.submitDefineId));
    //     }
    // }

    //格式化审核内容
    formatUpdateContent(updateContent) {
        let jsonObj = updateContent;
        if (jsonObj && jsonObj.startsWith("{") && jsonObj.endsWith("}")) {
            try {
                jsonObj = JSON.parse(jsonObj);
                console.log("jsonObj:==", jsonObj);
            } catch (e) {}
        }
        return jsonObj || {};
    }
    render() {
        let activeAuditInfo = this.props.activeAuditInfo || {};
        let updateRecord = this.props.activeAuditHistory.updateRecord || {};
        let showLoading = this.props.showLoading;
        return (
            <div>
                <Spin spinning={showLoading}>
                    {/* <b>{activeAuditInfo.taskName}</b> */}
                    {/**驻场指派**/}

                    < p style={{margin: '1rem'}}>
                        <Row>
                            <Col>楼盘：{activeAuditInfo.contentName}</Col>
                        </Row>
                        <Row>
                            <Col>驻场：{activeAuditInfo.ext1}</Col>
                        </Row>
                    </p>
                    <div>
                        {/**审核记录**/}
                        <AuditHistory />
                    </div>
                    {
                        this.props.activeAuditInfo.examineStatus === 1 ? <AuditForm /> : null
                    }
                </Spin>
            </div >
        )
    }
}

function mapStateToProps(state) {
    return {
        showLoading: state.audit.showLoading,
        basicData: state.basicData,
        activeAuditInfo: state.audit.activeAuditInfo,
        activeAuditHistory: state.audit.activeAuditHistory,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditBuildingOnSite);