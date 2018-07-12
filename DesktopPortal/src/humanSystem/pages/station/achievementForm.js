import {connect} from 'react-redux';
import {getDicParList, postBlackLst, setSalaryInfo, getcreateStation} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Input, Select, Form, Button, Row, Col, Cascader, InputNumber, TreeSelect} from 'antd'
import {NewGuid} from '../../../utils/appUtils';
import {editSalary, addSalary} from '../../serviceAPI/salaryService'
import {getPosition} from '../../serviceAPI/staffService'
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout = {
    labelCol: {span: 6},
    wrapperCol: {span: 17},
};


class AchievementForm extends Component {

    componentWillMount() {
        this.state = {
            id: NewGuid(),
            department: "",
            showLoading: false,
            positionList: []
        };
    }

    componentDidMount() {
        if (this.props.subPageLoadCallback) {
            this.props.subPageLoadCallback(this.props.form, 'achievement')
        }
    }

    handleSubmit = (e) => {
        e.preventDefault();
        let salaryInfo = this.props.location.state;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = salaryInfo.id ? salaryInfo.id : null;
                console.log("表单内容:", values);
                this.setState({showLoading: true});
                if (salaryInfo.id) {
                    editSalary(values).then(res => {
                        this.setState({showLoading: false});
                        if (res.isOk) {
                            this.props.form.resetFields();
                        }
                    });
                } else {
                    addSalary(values).then(res => {
                        this.setState({showLoading: false});
                        if (res.isOk) {
                            this.props.form.resetFields();
                        }
                    });
                }
            }
        });
    }

    handleDepartmentChange = (e) => {
        if (!e) {
            this.props.dispatch(getcreateStation(this.state.department));
        }
    }
    //组织结构变更
    handleDepartmentChange = (e) => {
        getPosition(e).then(res => {
            this.setState({positionList: res.extension || []});
        });

    }

    render() {
        const {getFieldDecorator, getFieldsValue} = this.props.form;
        let achievementInfo = this.props.location.state;
        let disabled = (this.props.readOnly || false);
        return (
            <Form style={{marginTop: '10px'}} onSubmit={this.handleSubmit}>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="基本工资">
                            {getFieldDecorator('baseSalary', {
                                initialValue: achievementInfo.baseSalary,
                                rules: [{
                                    required: true, message: '请输入基本工资',
                                }]
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入基本工资" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="岗位补贴">
                            {getFieldDecorator('subsidy', {
                                initialValue: achievementInfo.subsidy,
                                rules: [{
                                    required: true, message: '请输入岗位补贴',
                                }]
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入岗位补贴" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>

                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="交通补贴">
                            {getFieldDecorator('trafficAllowance', {
                                initialValue: achievementInfo.trafficAllowance,
                                rules: [{
                                    required: true, message: '请输入交通补贴',
                                }]
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入交通补贴" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="通讯补贴">
                            {getFieldDecorator('communicationAllowance', {
                                initialValue: achievementInfo.communicationAllowance,
                                rules: [{
                                    required: true, message: '请输入通讯补贴',
                                }]
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入通讯补贴" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="其他补贴">
                            {getFieldDecorator('otherAllowance', {
                                initialValue: achievementInfo.otherAllowance,
                                rules: [{
                                    required: false, message: '请输入其他补贴',
                                }]
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入其他补贴" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>

                {/* <Row style={{textAlign: 'center', display: (disabled ? 'none' : 'block')}}>
                        <Col span={20}>
                            <Button type="primary" disabled={disabled} htmlType="submit" >提交</Button>
                        </Col>
                    </Row> */}
            </Form>
        );
    }
}

function tableMapStateToProps(state) {
    return {
        setContractOrgTree: state.basicData.searchOrgTree,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(AchievementForm));