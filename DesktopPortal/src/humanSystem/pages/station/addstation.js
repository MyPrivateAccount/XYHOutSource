import {connect} from 'react-redux';
import {createStation, getOrgList, setStation} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Table, Input, Select, Form, Cascader, Button, Row, Col, TreeSelect, notification} from 'antd'
import Layer from '../../../components/Layer';
import {getDicPars} from '../../../utils/utils';
import {NewGuid} from '../../../utils/appUtils';
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout1 = {
    labelCol: {span: 6},
    wrapperCol: {span: 6},
};


class Station extends Component {
    state = {
        department: ''
    }

    componentDidMount() {
        let dicPositions = getDicPars("POSITION_TYPE", this.props.rootBasicData);
        this.setState({dicPositions: dicPositions});
        console.log("shujuyuan:", this.props.location.state);
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = NewGuid();
                this.props.dispatch(setStation(values));
                console.log('新增职位:', values);
            }
        });
    }

    //部门类型验证
    orgTypeValidate = (rule, value, callback) => {
        console.log("部门类型验证");
        const form = this.props.form;
        let orgObj = form.getFieldValue('parentID');
        if (orgObj && orgObj.type !== 'Filiale') {
            callback('请选择类型为分公司的组织!');
        }
        callback();
    }

    render() {
        const {getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched} = this.props.form;
        let editPositionObj = this.props.location.state;
        let isModify = Object.keys(editPositionObj).length > 0;
        return (
            <Layer>
                <Form onSubmit={this.handleSubmit}>
                    <FormItem {...formItemLayout1} />
                    <FormItem {...formItemLayout1} />
                    <FormItem {...formItemLayout1} label="职位名称">
                        {getFieldDecorator('positionName', {
                            initialValue: editPositionObj.positionName,
                            rules: [{
                                required: true, message: '请输入职位名称',
                            }]
                        })(
                            <Input placeholder="请输入职位名称" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="职位类型">
                        {getFieldDecorator('positionType', {
                            initialValue: editPositionObj.positionType,
                            rules: [{
                                required: true, message: '请选择职位类型',
                            }]
                        })(
                            <Select>
                                {
                                    (this.state.dicPositions || []).map((item, i) => <Option key={i} value={item.value}>{item.key}</Option>)
                                }
                            </Select>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="所属分公司">
                        {getFieldDecorator('parentID', {
                            initialValue: editPositionObj.parentID,
                            rules: [{
                                required: true, message: '请选择所属分公司'
                            }, {
                                validator: this.orgTypeValidate
                            }]
                        })(
                            <TreeSelect
                                allowClear
                                treeData={this.props.setContractOrgTree}
                                placeholder="请选择所属分公司"
                            />
                        )}
                    </FormItem>
                    <FormItem wrapperCol={{span: 12, offset: 6}}>
                        <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >{isModify ? "修改" : "新建"}</Button></Col>
                    </FormItem>
                </Form>
            </Layer>
        )
    }
}

function tableMapStateToProps(state) {
    return {
        setContractOrgTree: state.basicData.searchOrgTree,
        rootBasicData: (state.rootBasicData || {}).dicList,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Station));