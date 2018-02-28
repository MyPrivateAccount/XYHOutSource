import React, { Component } from 'react'
import { Modal, Button, Form, Checkbox, Input,InputNumber } from 'antd';
import {connect} from 'react-redux';
import {cancelEditValue, saveDicValueAsync} from '../actions'


const FormItem = Form.Item;

class ParDialog extends Component {

    handleOk = (e) => {
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                values['groupId'] = this.props.currentGroup.id;
                console.log('Received values of form: ', values);
                this.props.save({op: this.props.editDic.isAdd?1:2 , entity: values})
            }
        });
    }
    handleCancel = (e) => {
        this.props.cancel();
    }

    render() {
        const { getFieldDecorator } = this.props.form;
        const lenValidator = [{ max: 40, message: '名称长度不得大于40个字符' }]

        const formItemLayout = {
            labelCol: {
              xs: { span: 24 },
              sm: { span: 6 },
            },
            wrapperCol: {
              xs: { span: 24 },
              sm: { span: 14 },
            },
          };
          
          let {title, show, readonly, isAdd} = this.props.editDic;
          let currentGroup = this.props.currentGroup || {};
          let hasExt1 = currentGroup.hasExt1;
          let hasExt2 = currentGroup.hasExt2;
          let renderValueInput = ()=>{
              if(currentGroup.valueType === 1){
                    return <InputNumber disabled={readonly || !isAdd}/>
              }else{
                return <Input disabled={readonly || !isAdd}/>
              }
          }
        return (
            <Modal title={title}
                visible={show}
                confirmLoading={this.props.editDic.operating}
                onOk={this.handleOk}
                onCancel={this.handleCancel}>
                <Form ref={(e) => this.form = e}>
                    <FormItem hasFeedback {...formItemLayout} label="名称">
                        {getFieldDecorator('key', {
                            rules: [{ required: true, message: '必须输入字典名称' },...lenValidator ],
                        })(
                            <Input disabled={readonly} />
                            )}
                    </FormItem>
                    <FormItem hasFeedback {...formItemLayout} label="值">
                        {getFieldDecorator('value', {
                            rules: [{ required: true, message: '必须输入值' }],
                        })(
                            renderValueInput()
                            )}
                    </FormItem>                    
                    <FormItem hasFeedback {...formItemLayout} label="排序">
                        {getFieldDecorator('order', {
                            rules: [{ required: true, message: '必须输入排序值' }],
                        })(
                            <InputNumber disabled={readonly}/>
                            )}
                    </FormItem>   
                    <FormItem hasFeedback {...formItemLayout} label="说明">
                        {getFieldDecorator('desc', {
                            rules: [],
                        })(
                            <Input disabled={readonly}/>
                            )}
                    </FormItem> 
                    {
                        hasExt1?(
                        <FormItem hasFeedback {...formItemLayout} label={currentGroup.ext1Desc}>
                            {getFieldDecorator('ext1', {
                                rules: [],
                            })(
                                <Input disabled={readonly}/>
                                )}
                        </FormItem> 
                        ):null
                    }
                       {
                        hasExt2?(
                        <FormItem hasFeedback {...formItemLayout} label={currentGroup.ext2Desc}>
                            {getFieldDecorator('ext2', {
                                rules: [],
                            })(
                                <Input disabled={readonly}/>
                                )}
                        </FormItem> 
                        ):null
                    }               
                </Form>
            </Modal>
        )
    }
}

const mapStateToProps = (state, props)=>{
    return{
        editDic: state.dic.editDic,
        currentGroup: state.dic.currentGroup
    }
}
const mapDispatchToProps = (dispatch)=>{
    return {
        cancel: (...args)=> dispatch(cancelEditValue(...args)),
        save: (...args)=> dispatch(saveDicValueAsync(...args))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Form.create({
    mapPropsToFields(props){
        if(props.form){
            props.form.resetFields();
        }
        let mv= {};
        Object.keys(props.editDic.entity).map(key=>{
            mv[key] = {value: props.editDic.entity[key]};
        })
        
        return mv;
    }
})(ParDialog) );