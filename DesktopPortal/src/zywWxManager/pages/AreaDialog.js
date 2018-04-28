import React, { Component } from 'react'
import { Modal, Button, Form, Checkbox, Input,InputNumber } from 'antd';
import {connect} from 'react-redux';
import {cancelArea, saveAreaAsync} from '../actions'


const FormItem = Form.Item;

class AreaDialog extends Component {

    handleOk = (e) => {
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                values['parentId'] = this.props.parent;
                values['level'] = this.props.level;
                console.log('Received values of form: ', values);
                this.props.save({op: this.props.area.isAdd?1:2 , entity: values, level: this.props.level, parent: this.props.parent})
            }
        });
    }
    handleCancel = (e) => {
        this.props.cancel();
    }

    render() {
        const { getFieldDecorator } = this.props.form;

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
          
          let {title, show, readonly, isAdd} = this.props.area;
          let level = this.props.level;
          let levelName = '城市'
          if( level === 2)
        {
            levelName="区域"
        }else if(level===3){
            levelName="片区"
        }
        title = `${title}${levelName}`

        return (
            <Modal title={title}
                visible={show}
                confirmLoading={this.props.area.operating}
                onOk={this.handleOk}
                onCancel={this.handleCancel}>
                <Form ref={(e) => this.form = e}>
                    <FormItem hasFeedback {...formItemLayout} label={`${levelName}名称`}>
                        {getFieldDecorator('name', {
                            rules: [{ required: true, message: `必须输入${levelName}名称` } ],
                        })(
                            <Input disabled={readonly} />
                            )}
                    </FormItem>
                    <FormItem hasFeedback {...formItemLayout} label="编码">
                        {getFieldDecorator('code', {
                            rules: [{ required: true, message: `必须输入${levelName}编码` }],
                        })(
                            <Input disabled={readonly || !isAdd} />
                            )}
                    </FormItem>                    
                    <FormItem hasFeedback {...formItemLayout} label="拼音缩写">
                        {getFieldDecorator('abbreviation', {
                            rules: [{ required: true, message: '必须输入拼音缩写' }],
                        })(
                            <Input disabled={readonly}/>
                            )}
                    </FormItem>   
                    <FormItem hasFeedback {...formItemLayout} label="排序" extra="排序越大越靠后">
                        {getFieldDecorator('order', {
                            rules: [],
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
                           
                </Form>
            </Modal>
        )
    }
}

const mapStateToProps = (state, props)=>{
    return{
        area: state.area.edit,
        parent: state.area.edit.parent,
        level: state.area.edit.level
    }
}
const mapDispatchToProps = (dispatch)=>{
    return {
        cancel: (...args)=> dispatch(cancelArea(...args)),
        save: (...args)=> dispatch(saveAreaAsync(...args))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Form.create({
    mapPropsToFields(props){
        if(props.form){
            props.form.resetFields();
        }
        let mv= {};
        Object.keys(props.area.entity).map(key=>{
            mv[key] = {value: props.area.entity[key]};
        })
        
        return mv;
    }
})(AreaDialog) );