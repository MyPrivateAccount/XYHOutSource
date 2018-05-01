import React, {Component} from 'react'
import {Modal, Button, Form, Checkbox, Input, InputNumber} from 'antd';
import {connect} from 'react-redux';
import {savePlanning, changeLoading} from '../actions'


const FormItem = Form.Item;

class PlanningDialog extends Component {

    handleOk = (e) => {
        let {entity, operType} = this.props;
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                values['parentId'] = this.props.parent;
                values['level'] = this.props.level;
                console.log('Received values of form: ', values);
                values.level = entity.level;
                values.parentId = entity.parentId;
                this.props.dispatch(changeLoading({status: true}));
                this.props.save({op: operType === "add" ? 1 : 2, entity: values})
            }
        });
    }

    render() {
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {
                xs: {span: 24},
                sm: {span: 6},
            },
            wrapperCol: {
                xs: {span: 24},
                sm: {span: 14},
            },
        };

        let {visible, operType, switchVisible, entity} = this.props;
        let title = operType === "add" ? "新增业态" : "编辑业态";

        return (
            <Modal title={title}
                visible={visible}
                confirmLoading={this.props.planning.submiting}
                onOk={this.handleOk}
                onCancel={() => switchVisible(false)}>
                <Form ref={(e) => this.form = e}>
                    <FormItem hasFeedback {...formItemLayout} label="编码">
                        {getFieldDecorator('id', {
                            rules: [{required: true, message: `必须输入业态编码`}],
                        })(
                            <Input disabled={operType !== "add"} />
                        )}
                    </FormItem>
                    <FormItem hasFeedback {...formItemLayout} label="业态名称">
                        {getFieldDecorator('businessName', {
                            rules: [{required: true, message: "必须输入业态名称名称"}],
                        })(
                            <Input />
                        )}
                    </FormItem>
                    <FormItem hasFeedback {...formItemLayout} label="排序" extra="排序越大越靠后">
                        {getFieldDecorator('order', {
                            rules: [],
                        })(
                            <InputNumber />
                        )}
                    </FormItem>
                    <FormItem hasFeedback {...formItemLayout} label="说明">
                        {getFieldDecorator('desc', {
                            rules: [],
                        })(
                            <Input />
                        )}
                    </FormItem>

                </Form>
            </Modal>
        )
    }
}

const mapStateToProps = (state, props) => {
    return {
        planning: state.planning
    }
}
const mapDispatchToProps = (dispatch) => {
    return {
        save: (...args) => dispatch(savePlanning(...args)),
        dispatch
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Form.create({
    mapPropsToFields(props) {
        if (props.form) {
            props.form.resetFields();
        }
        let mv = {};
        Object.keys(props.entity).map(key => {
            mv[key] = {value: props.entity[key]};
        })
        return mv;
    }
})(PlanningDialog));