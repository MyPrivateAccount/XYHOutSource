import {connect} from 'react-redux';
import {} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Input, Steps} from 'antd';
import moment from 'moment';


const Step = Steps.Step;

class AuditHistory extends Component {
    state = {

    }
    componentWillReceiveProps(newProps) {
    }
    getRecordStstusText(status) {
        let text = status;
        switch (text) {
            case 1:
                text = "提交";
                break;
            case 2:
                text = "待审核";
                break;
            case 3:
                text = "审核通过";
                break;
            case 4:
                text = "驳回";
                break;
            case 5:
                text = "完成";
                break;
            default:
                break;
        }
        return text;
    }

    render() {
        let activeAuditHistory = this.props.activeAuditHistory;
        if (this.props.contentInfo) {
            activeAuditHistory = this.props.contentInfo;
        }
        const examineRecordList = activeAuditHistory.examineRecordResponses || [];
        //examineRecordList.reverse();
        return (
            <div style={{padding: '1rem'}}>
                <p className='content-title'>审核记录</p>
                <Steps direction="vertical" >
                    {
                        examineRecordList.map(record => {
                            // let desc = <div><p>审核人：{record.examineUserName}</p><p>状态：{this.getRecordStstusText(record.recordStstus)}</p><p>内容：{record.examineContents}</p></div>;
                            let desc = <div><p>{moment(record.recordTime).format("YYYY-MM-DD HH:mm:ss")}</p><p>{record.examineContents}</p></div>;
                            let stepStatus = "finish"; //(record.recordStstus !== 2 ? "finish" : "process");
                            if (record.recordStstus === 2) {
                                stepStatus = "process";
                            } else if (record.recordStstus === 4) {
                                stepStatus = "error";
                            }
                            let title = (record.examineUserName || '') + this.getRecordStstusText(record.recordStstus);
                            return (<Step key={record.id} status={stepStatus} title={title} description={desc} />)
                        })
                    }
                </Steps>
            </div >
        )
    }
}

function mapStateToProps(state) {
    return {
        activeAuditHistory: state.audit.activeAuditHistory
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditHistory);