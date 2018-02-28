import { connect } from 'react-redux';
import { closeRecommendDialog, recommendBuilding } from '../actions/actionCreator';
import React, { Component } from 'react'
import { Modal, Row, Col, DatePicker, InputNumber } from 'antd'
import moment from 'moment';

function getNowFormatDate() {
    var date = new Date();
    var seperator1 = "-";
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = year + seperator1 + month + seperator1 + strDate;
    return currentdate;
}

class RecommendDateDialog extends Component {
    state = {
        recommendInfo: {
            recommendTime: getNowFormatDate(),
            recommendDays: '10'
        }
    }

    handleDateChange = (date, dateString) => {
        console.log("date", dateString);
        let recommendInfo = { ...this.state.recommendInfo };
        recommendInfo.recommendTime = dateString;
        this.setState({ recommendInfo });
    }
    handleDayChange = (days) => {
        console.log("e", days);
        let recommendInfo = { ...this.state.recommendInfo };
        recommendInfo.recommendDays = days;
        this.setState({ recommendInfo });
    }

    handleOk = (e) => {
        const activeBuilding = this.props.activeBuilding;
        let recommendInfo = this.state.recommendInfo;
        recommendInfo.buildingId = activeBuilding.id;
        recommendInfo.isRegion = (activeBuilding.recommendType === "region");
        console.log("recommendInfo:", recommendInfo, activeBuilding);
        this.props.dispatch(recommendBuilding(recommendInfo));
        if (this.props.refresh) {
            setTimeout(()=>this.props.refresh(),300);
        }
    }

    handleCancel = (e) => {
        this.props.dispatch(closeRecommendDialog());
    }
    render() {
        let visible = this.props.recommendDialogVisible;
        return (
            <Modal width={500} title="推荐日期设置" maskClosable={false} visible={visible}
                onOk={(e) => this.handleOk()} onCancel={this.handleCancel} >
                <Row>
                    <Col span={5}>推荐生效日期：</Col>
                    <Col span={12}><DatePicker defaultValue={moment(this.state.recommendInfo.recommendTime, 'YYYY-MM-DD')} onChange={this.handleDateChange} /></Col>
                </Row>
                <Row>
                    <Col span={12}></Col>
                </Row>
                <Row>
                    <Col span={5}>推荐持续天数：</Col>
                    <Col span={12}><InputNumber min={1} max={30} defaultValue={10} onChange={this.handleDayChange} /></Col>
                </Row>
            </Modal>
        )
    }
}

function mapStateToProps(state) {
    return {
        activeBuilding: state.search.activeBuilding,
        recommendDialogVisible: state.search.recommendDialogVisible
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(RecommendDateDialog);