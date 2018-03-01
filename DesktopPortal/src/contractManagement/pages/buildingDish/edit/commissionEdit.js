import { connect } from 'react-redux';
import { saveCommissionInfo, commissionView,commissionLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Form, Input, Row, Col, Icon } from 'antd'

const FormItem = Form.Item;

class CommissionEdit extends Component {
    state = {
        loadingState: false
    }
    componentWillMount() {
        if (this.props.setCommisson) {
         this.props.setCommisson(this.props.form)
        }
    }
    componentWillReceiveProps(newProps) {
        this.setState({ loadingState: false });
    }
    handleSave = (e) => {
        let { commissionOperType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                this.props.dispatch(commissionLoadingStart())
                // this.setState({ loadingState: true });
                let buildInfo = this.props.buildInfo.id;
                let newcommission = values;
                newcommission.id = this.props.buildInfo.id;
                console.log(buildInfo, '&&')
                this.props.dispatch(saveCommissionInfo({
                    commissionOperType: commissionOperType, 
                    entity: newcommission, 
                    id: buildInfo,
                    ownCity: this.props.user.City 
                }));
            }
        });
    }
    handleCancel = (e) => {
        this.props.dispatch(commissionView());
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 0 },
            wrapperCol: { span: 24 },
        };
        let { commissionOperType } = this.props.operInfo;
        let commissionPlan = this.props.buildInfo.commissionPlan;
        return (
            <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px",backgroundColor: "#ECECEC"}}>
                <Icon type="tags-o" className='content-icon'/> <span className='content-title'>佣金方案</span>
                <Row type="flex" style={{marginTop: "25px"}}>
                    <Col span={1}></Col>
                    <Col span={22}>
                        <FormItem {...formItemLayout}>
                            {getFieldDecorator('commissionPlan', {
                                initialValue: commissionPlan
                            })(
                                <Input type="textarea" style={{ height: '100px' }} placeholder="请输入佣金方案..." />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={1}></Col>
                </Row>
                {
                    this.props.type === 'dynamic' ? null : 
                    <Row>
                        <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                            <Button 
                            type="primary" 
                            htmlType="submit" 
                            disabled={this.props.buildInfo.isDisabled} 
                            loading={this.props.loadingState} 
                            style={{width: "8rem"}} 
                            onClick={this.handleSave}>保存</Button>
                            {commissionOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
                        </Col>
                    </Row>
                }
            </Form>
        )
    }
}

function mapStateToProps(state) {
    return {
        buildInfo: state.building.buildInfo,
        operInfo: state.building.operInfo,
        loadingState: state.building.commissionloading,
        user: (state.oidc.user || {}).profile || {},
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedForm = Form.create()(CommissionEdit);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);