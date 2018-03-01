import { connect } from 'react-redux';
import { saveProject, viewProjectInfo,projectLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Form, Input, Row, Col, Icon } from 'antd'

const FormItem = Form.Item;

class ProjectEdit extends Component {
    state = {
        loadingState: false
    }
    componentWillMount() {

    }
    componentWillReceiveProps(newProps) {
        this.setState({ loadingState: false });
    }
    handleSave = (e) => {
        let { projectOperType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                // this.setState({ loadingState: true });
                this.props.dispatch(projectLoadingStart())
                let newProjectInfo = values;
                newProjectInfo.id = this.props.buildInfo.id;
                let method = 'PUT';//(projectOperType == 'add' ? 'POST' : "PUT");
                console.log(`method:${method},newProjectInfo:${JSON.stringify(newProjectInfo)}`);
                this.props.dispatch(saveProject({ 
                    method: method, 
                    entity: newProjectInfo, 
                    ownCity: this.props.user.City 
                }));
            }
        });
    }
    handleCancel = (e) => {
        this.props.dispatch(viewProjectInfo());
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 0 },
            wrapperCol: { span: 24 },
        };
        let { projectOperType } = this.props.operInfo;
        let projectInfo = this.props.buildInfo.projectInfo;
        return (
            <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px"}}>
                <Icon type="tags-o" className='content-icon'/> <span className='content-title'>项目简介 (必填)</span>
                <Row type="flex" style={{marginTop: "25px"}}>
                    <Col span={1}></Col>
                    <Col span={22}>
                        <FormItem {...formItemLayout}>
                            {getFieldDecorator('summary', {
                                initialValue: projectInfo.summary,
                                rules: [{ required: true, message: '请输入项目简介' }],
                            })(
                                <Input type="textarea" style={{ height: '100px' }} placeholder="请输入楼盘项目简介!" />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={1}></Col>
                </Row>
                <Row>
                    <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button 
                        type="primary" 
                        htmlType="submit" 
                        disabled={this.props.buildInfo.isDisabled} 
                        loading={this.props.loadingState} 
                        style={{width: "8rem"}} 
                        onClick={this.handleSave}>保存</Button>
                        {projectOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
                    </Col>
                </Row>
            </Form>
        )
    }
}

function mapStateToProps(state) {
    //console.log('BuildingDishDetail MapStateToProps:' + JSON.stringify(state));
    return {
        buildInfo: state.building.buildInfo,
        operInfo: state.building.operInfo,
        loadingState: state.building.projectloading,
        user: (state.oidc.user || {}).profile || {},
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedForm = Form.create()(ProjectEdit);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);