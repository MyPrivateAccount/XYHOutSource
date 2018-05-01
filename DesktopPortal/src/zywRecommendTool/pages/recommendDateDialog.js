import {connect} from 'react-redux';
import {closeRecommendDialog, recommendBuilding} from '../actions/actionCreator';
import React, {Component} from 'react'
import {Modal, Row, Col, DatePicker, InputNumber, Input, notification} from 'antd'
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
            recommendDays: '10',
            title: '',
            desc: ''
        }
    }

    handleDateChange = (date, dateString) => {
        console.log("date", dateString);
        let recommendInfo = {...this.state.recommendInfo};
        recommendInfo.recommendTime = dateString;
        this.setState({recommendInfo});
    }
    handleDayChange = (days) => {
        console.log("e", days);
        let recommendInfo = {...this.state.recommendInfo};
        recommendInfo.recommendDays = days;
        this.setState({recommendInfo});
    }

    handleOk = (e) => {
        const activeBuilding = this.props.activeBuilding;
        let recommendInfo = this.state.recommendInfo;
        recommendInfo.buildingId = activeBuilding.id;
        recommendInfo.isRegion = (activeBuilding.recommendType === "region");
        // console.log("recommendInfo:", recommendInfo, activeBuilding);
        if (activeBuilding.recommendType === "region") {
            if (!recommendInfo.title || recommendInfo.title.length == 0) {
                notification.error({message: '请填写主标题!'});
                return;
            }
        }
        this.props.dispatch(recommendBuilding(recommendInfo));
        if (this.props.refresh) {
            setTimeout(() => this.props.refresh(), 300);
        }
    }

    handleCancel = (e) => {
        this.props.dispatch(closeRecommendDialog());
    }
    handleTitleChange = (e) => {
        let title = e.target.value;
        if (title && title.length > 20) {
            return;
        }
        let recommendInfo = {...this.state.recommendInfo, title: title};
        this.setState({recommendInfo: recommendInfo});
    }
    handleSubTitleChange = (e) => {
        let subTitle = e.target.value;
        if (subTitle && subTitle.length > 20) {
            return;
        }
        let recommendInfo = {...this.state.recommendInfo, desc: subTitle};
        this.setState({recommendInfo: recommendInfo});
    }
    render() {
        let visible = this.props.recommendDialogVisible;
        const activeBuilding = this.props.activeBuilding;
        return (
            <Modal width={500} title="推荐日期设置" maskClosable={false} visible={visible}
                onOk={(e) => this.handleOk()} onCancel={this.handleCancel} >
                {
                    (activeBuilding.recommendType === "region") ? <div>
                        <Row style={{margin: '5px',display:'flex', alignItems:'center'}}>
                            <Col span={5}>广告语：</Col>
                            <Col span={12}><Input placeholder="请输入广告语" value={this.state.recommendInfo.title} onChange={this.handleTitleChange} /></Col>
                        </Row>
                        {/* <Row style={{margin: '5px'}}>
                            <Col span={5}>副标题</Col>
                            <Col span={12}><Input placeholder="请输入副标题" value={this.state.recommendInfo.desc} onChange={this.handleSubTitleChange} /></Col>
                        </Row> */}
                    </div> : null
                }

                <Row style={{margin: '5px',display:'flex', alignItems:'center'}}>
                    <Col span={5}>推荐生效日期：</Col>
                    <Col span={12}><DatePicker defaultValue={moment(this.state.recommendInfo.recommendTime, 'YYYY-MM-DD')} onChange={this.handleDateChange} /></Col>
                </Row>
                <Row style={{margin: '5px',display:'flex', alignItems:'center'}}>
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