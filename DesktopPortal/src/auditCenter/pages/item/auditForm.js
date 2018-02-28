import { connect } from 'react-redux';
import { closeAuditDetail, saveAudit, setLoadingVisible } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Input, Radio, Form, Button, Row, Col } from 'antd'
const FormItem = Form.Item;
const RadioGroup = Radio.Group;
const { TextArea } = Input;

class AuditForm extends Component {
    state = {
        recordId: '',
        auditStatus: true,
        desc: ''
    }
    componentWillReceiveProps(newProps) {
        let activeAuditHistory = newProps.activeAuditHistory;
        if (activeAuditHistory) {
            let curRecord = activeAuditHistory.examineRecordResponses.find(record => record.recordStstus === 2);
            if (curRecord) {
                this.setState({ recordId: curRecord.id });
            }
        }
    }

    hanldeRejectReson = (e) => {
        this.setState({ desc: e.target.value })
    }
    handleAuditdChange = (e) => {
        this.setState({
            auditStatus: e.target.value,
        });
    }
    handleSave = (e) => {
        e.preventDefault();
        let request = { ...this.state };
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                this.props.dispatch(setLoadingVisible(true));
                this.props.dispatch(saveAudit(request));
            }
        });
        console.log("审核结果保存", request);
    }
    handleCancel = (e) => {
        this.props.dispatch(closeAuditDetail());
    }

    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 4 },
            wrapperCol: { span: 24 },
        };
        let auditStatus = this.state.auditStatus;
        return (
            <div style={{ padding: '1rem' }}>
                <Row>
                    <Col>
                        <RadioGroup onChange={this.handleAuditdChange} value={auditStatus}>
                            <Radio value={true}>通过</Radio>
                            <Radio value={false}>驳回</Radio>
                        </RadioGroup>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <FormItem  {...formItemLayout} label="">
                            {getFieldDecorator('desc', {
                                rules: [{ required: !auditStatus, message: '请输入驳回原因!' }],
                            })(
                                <TextArea rows={4} onChange={this.hanldeRejectReson} placeholder={auditStatus ? '请输入意见' : '请输入驳回原因'} />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col style={{ textAlign: 'center', padding: '5px' }}>
                        <Button type="primary" style={{ marginRight: '10px' }} onClick={this.handleSave} loading={this.props.showLoading}>提交</Button>
                        <Button type="primary" onClick={this.handleCancel}>取消</Button>
                    </Col>
                </Row>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        activeAuditInfo: state.audit.activeAuditInfo,
        activeAuditHistory: state.audit.activeAuditHistory,
        showLoading: state.audit.showLoading
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedApp = Form.create()(AuditForm);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedApp);