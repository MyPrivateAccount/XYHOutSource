import React, {Component} from 'react'
import {Modal, Button, Form, Checkbox, Input, Select} from 'antd';
import {connect} from 'react-redux';
import {cancelEditGroup, saveDicGroupAsync} from '../actions'


const FormItem = Form.Item;
const Option = Select.Option;

class DicGroupDialog extends Component {
    state = {
        visible: false,
        confirmDirty: false
    }
    handleOk = (e) => {
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                //  console.log('Received values of form: ', values);
                values.valueType = values.vType;//处理兼容问题
                this.props.save({op: this.props.editGroup.isAdd ? 1 : 2, entity: values})
            }
        });
    }
    handleCancel = (e) => {
        this.props.cancel();
    }
    componentWillReceiveProps = (nextProps) => {
        if (nextProps.editGroup.show !== this.props.editGroup.show) {
            if (nextProps.editGroup.show) {
                //  this.props.form.resetFields();

            }
        }
    }
    hasExtChanged = (num, v) => {
        if (v) {
            window.setTimeout(() => {
                this.props.form.validateFields([`ext${num}Desc`], {force: true})
            })

        }
    }
    checkExt1 = (rule, value, callback) => {
        const form = this.props.form;
        const has = form.getFieldValue('hasExt1');
        const name = form.getFieldValue("ext1Desc")
        if (has && !name) {
            callback('必须输入扩展字段1名称');
        } else {
            callback();
        }

    }
    checkExt2 = (rule, value, callback) => {
        const form = this.props.form;
        const has = form.getFieldValue('hasExt2');
        const name = form.getFieldValue("ext2Desc")
        if (has && !name) {
            callback('必须输入扩展字段2名称');
        } else {
            callback();
        }

    }
    render() {
        const {getFieldDecorator} = this.props.form;
        const lenValidator = [{max: 40, message: '名称长度不得大于40个字符'}]

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
        const tailFormItemLayout = {
            wrapperCol: {
                xs: {
                    span: 24,
                    offset: 0,
                },
                sm: {
                    span: 14,
                    offset: 6,
                },
            },
        };
        let {title, show, readonly, operating, isAdd} = this.props.editGroup;
        return (
            <Modal title={title}
                visible={show}
                confirmLoading={operating}
                onOk={this.handleOk}
                onCancel={this.handleCancel}>
                <Form ref={(e) => this.form = e}>
                    <FormItem hasFeedback {...formItemLayout} label="字典组编码">
                        {getFieldDecorator('id', {
                            rules: [{required: true, message: '必须输入编码'},],
                        })(
                            <Input disabled={readonly || !isAdd} />
                            )}
                    </FormItem>
                    <FormItem hasFeedback {...formItemLayout} label="字典组名称">
                        {getFieldDecorator('name', {
                            rules: [{required: true, message: '必须输入名称'}, ...lenValidator],
                        })(
                            <Input disabled={readonly} />
                            )}
                    </FormItem>

                    <FormItem hasFeedback {...formItemLayout} label="值类型">
                        {getFieldDecorator('vType', {
                            // initialValue: '1',
                            //rules: [{ required: true, message: '必须选择值类型' }],
                        })(
                            <Select style={{width: '120px'}} disabled={readonly}>
                                <Option key='1' value="1">数字</Option>
                                <Option key='3' value="3">字符串</Option>
                            </Select>
                            )}
                    </FormItem>
                    <FormItem hasFeedback {...formItemLayout} label="说明">
                        {getFieldDecorator('desc', {
                            rules: [],
                        })(
                            <Input disabled={readonly} />
                            )}
                    </FormItem>
                    {/* <FormItem {...tailFormItemLayout} >
                        {getFieldDecorator('hasExt1', {
                            valuePropName: 'checked',
                        })(
                            <Checkbox disabled={readonly} onChange={(v)=>{this.hasExtChanged(1,v)}} >使用扩展字段1</Checkbox>
                        )}
                    </FormItem> */}
                    <FormItem hasFeedback {...formItemLayout} label="扩展字段1名称">
                        {getFieldDecorator('ext1Desc', {
                            /* rules: [{ validator: this.checkExt1 }, ...lenValidator], */
                            rules: [],
                        })(
                            <Input disabled={readonly} />
                            )}
                    </FormItem>
                    {/* <FormItem {...tailFormItemLayout} >
                        {getFieldDecorator('hasExt2', {
                            valuePropName: 'checked',
                        })(
                            <Checkbox disabled={readonly} onChange={(v)=>{this.hasExtChanged(2,v)}} >使用扩展字段2</Checkbox>
                        )}
                    </FormItem> */}
                    <FormItem hasFeedback {...formItemLayout} label="扩展字段2名称">
                        {getFieldDecorator('ext2Desc', {
                            /* rules: [{ validator: this.checkExt2 }, ...lenValidator], */
                            rules: [],
                        })(
                            <Input disabled={readonly} />
                            )}
                    </FormItem>

                </Form>
            </Modal>
        )
    }
}

const mapStateToProps = (state, props) => {
    if (state.dic.editGroup.entity && state.dic.editGroup.entity.valueType) {
        state.dic.editGroup.entity.vType = state.dic.editGroup.entity.valueType;
    }
    return {
        editGroup: state.dic.editGroup
    }
}
const mapDispatchToProps = (dispatch) => {
    return {
        cancel: (...args) => dispatch(cancelEditGroup(...args)),
        save: (...args) => dispatch(saveDicGroupAsync(...args)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Form.create({
    mapPropsToFields(props) {
        if (props.form) {
            props.form.resetFields();
        }
        let mv = {};
        Object.keys(props.editGroup.entity).map(key => {
            mv[key] = {value: props.editGroup.entity[key]};
        })
        return mv;
    }
})(DicGroupDialog));