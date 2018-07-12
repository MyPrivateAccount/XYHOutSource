import {connect} from 'react-redux';
import {postChangeHuman, getcreateStation, getSalaryItem} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {TreeSelect, Input, Form, Select, Button, Row, Col} from 'antd'
import SocialSecurity from '../../../businessComponents/humanSystem/socialSecurity'
import Salary from '../../../businessComponents/humanSystem/salary'
import Layer from '../../../components/Layer'
import {getPosition, adjustHuman} from '../../serviceAPI/staffService'
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout = {
    labelCol: {span: 6},
    wrapperCol: {span: 17},
};


class Change extends Component {
    state = {
        department: '',
        oldPositionList: [],
        newPositionList: [],
        showLoading: false
    }

    componentDidMount() {
        // let len = this.props.selHumanList.length;
        // if (len > 0) {
        //     let lstvalue = [];
        //     this.props.setDepartmentOrgTree.findIndex(e => this.findCascaderLst(this.props.selHumanList[len - 1].departmentId, e, lstvalue));
        //     this.props.form.setFieldsValue({name: this.props.selHumanList[len - 1].name});
        //     this.props.form.setFieldsValue({idCard: this.props.selHumanList[len - 1].idCard});
        //     this.props.form.setFieldsValue({orgDepartmentId: lstvalue});
        // }
        let humanInfo = this.props.location.state;
        if (humanInfo.departmentId) {
            getPosition(humanInfo.departmentId).then(res => {
                this.setState({oldPositionList: res.extension || []});
            });
        }
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        let humanInfo = this.props.location.state;
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            let {salaryForm, socialSecurityForm} = this.state;
            salaryForm.validateFields();
            socialSecurityForm.validateFields();
            let salaryErrs = salaryForm.getFieldsError();
            let socialSecurityErrs = socialSecurityForm.getFieldsError();

            let hasErr = false;
            for (let item in salaryErrs) {
                if (salaryErrs[item]) {
                    hasErr = true;
                }
            }
            for (let item in socialSecurityErrs) {
                if (socialSecurityErrs[item]) {
                    hasErr = true;
                }
            }
            console.log("表单:", values, hasErr);
            if (!err && !hasErr) {
                // this.props.dispatch(postChangeHuman(values));
                this.setState({showLoading: true});
                values.humanId = humanInfo.id;
                adjustHuman(values).then(res => {
                    this.setState({showLoading: false});
                })
            }
        });
    }

    handleNewDepartmentChange = (e) => {
        if (e) {
            getPosition(e).then(res => {
                this.setState({newPositionList: res.extension || []});
            });
        }
    }

    handleSelectChange = (e) => {
        this.props.dispatch(getSalaryItem(e));
    }

    findCascaderLst(id, tree, lst) {
        if (tree) {
            if (tree.id === id) {
                lst.unshift(tree.id);
                return true;
            } else if (tree.children && tree.children.length > 0) {
                if (tree.children.findIndex(org => this.findCascaderLst(id, org, lst)) !== -1) {
                    lst.unshift(tree.id);
                    return true;
                }
            }
        }
        return false;
    }

    //子页面回调
    subPageLoadCallback = (formObj, pageName) => {
        console.log("表单对象:", formObj, pageName);
        if (pageName == "socialSecurity") {
            this.setState({socialSecurityForm: formObj});
        } else if (pageName == "salary") {
            this.setState({salaryForm: formObj});
        }
    }

    render() {
        let humanInfo = this.props.location.state;
        const {getFieldDecorator, getFieldsValue} = this.props.form;
        return (
            <Layer showLoading={this.state.showLoading}>
                <div className="page-title" style={{marginBottom: '10px'}}>异动调薪</div>
                <Form onSubmit={this.handleSubmit}>
                    <Row style={{marginTop: '10px'}}>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="员工编号">
                                {getFieldDecorator('userID', {
                                    initialValue: humanInfo.userID,
                                    rules: [{
                                        required: true, message: '请输入员工编号',
                                    }]
                                })(
                                    <Input disabled={true} placeholder="请输入员工编号" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="姓名">
                                {getFieldDecorator('name', {
                                    initialValue: humanInfo.name,
                                    rules: [{
                                        required: true, message: '请输入姓名',
                                    }]
                                })(
                                    <Input disabled={true} placeholder="请输入姓名" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}></Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="原部门">
                                {getFieldDecorator('departmentId', {
                                    initialValue: humanInfo.departmentId,
                                    rules: [{
                                        required: true,
                                        message: '',
                                    }]
                                })(
                                    // <Cascader disabled={true} options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect placeholder="归属部门" />
                                    <TreeSelect treeData={this.props.setDepartmentOrgTree} placeholder="归属部门" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="原职位">
                                {getFieldDecorator('position', {
                                    initialValue: humanInfo.position,
                                    rules: [{
                                        required: true, message: '请选择职位',
                                    }],
                                })(
                                    <Select disabled={true} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            (this.state.oldPositionList && this.state.oldPositionList.length > 0) ?
                                                this.state.oldPositionList.map(
                                                    function (params) {
                                                        return <Option key={params.id} value={params.id}>{params.positionName}</Option>;
                                                    }
                                                ) : null
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}></Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="新部门">
                                {getFieldDecorator('newDepartmentId', {
                                    rules: [{
                                        required: true,
                                        message: '请选择新部门',
                                    }]
                                })(
                                    <TreeSelect treeData={this.props.setDepartmentOrgTree} placeholder="归属部门" onChange={this.handleNewDepartmentChange} />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="新职位">
                                {getFieldDecorator('newStation', {
                                    rules: [{
                                        required: true, message: '请选择新职位',
                                    }]
                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            (this.state.newPositionList && this.state.newPositionList.length > 0) ?
                                                this.state.newPositionList.map(
                                                    function (params) {
                                                        return <Option key={params.id} value={params.id}>{params.positionName}</Option>;
                                                    }
                                                ) : null
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}></Col>
                    </Row>

                    <SocialSecurity subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)} />
                    <Salary subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)} />

                    <Row>
                        <Col span={20} style={{textAlign: 'center'}}>
                            <Button type="primary" disabled={this.hasErrors(getFieldsValue())} onClick={this.handleSubmit}>提交</Button>
                        </Col>
                    </Row>
                </Form>
            </Layer>

        );
    }
}

function tableMapStateToProps(state) {
    return {
        stationList: state.search.stationList,
        orgstationList: state.search.orgstationList,
        selSalaryItem: state.basicData.selSalaryItem,
        changeResonList: state.basicData.changeResonList,
        changeTypeList: state.basicData.changeTypeList,
        selHumanList: state.basicData.selHumanList,
        setDepartmentOrgTree: state.basicData.searchOrgTree,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Change));