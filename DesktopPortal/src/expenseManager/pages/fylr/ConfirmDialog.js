import React, {Component} from 'react';
import {Select,Upload, Modal, Icon, Table, Form, Checkbox, Input,TreeSelect, Row, Col,Button, notification,Spin} from 'antd'

const FormItem = Form.Item;


const ConfirmForm = Form.create()(
    class extends React.Component {

        reject =()=>{
            const { onReject, form } = this.props;
            var vals = form.getFieldsValue();
            if(!vals["message"]){
                Modal.warning({
                    title: '警告',
                    content: '当选择驳回时，请您输入审核意见',
                  });
                  return;
            }
            if(onReject){
                onReject(vals);
            }
        }

        confirm =()=>{
            const { onSubmit, form } = this.props;
            var vals = form.getFieldsValue();

            if(onSubmit){
                onSubmit(vals);
            }
        }

      render() {
        const { visible, onCancel,  form, title, loading } = this.props;
        const { getFieldDecorator } = form;
        return (
          <Modal
            visible={visible}
            title={title}
            onCancel={onCancel}
            footer={[
                <Button key="back" onClick={onCancel}>取消</Button>,
                <Button key="reject" disabled={loading} onClick={this.reject}>驳回</Button>,
                <Button key="submit" type="primary" disabled={loading} onClick={this.confirm}>
                  确认
                </Button>,
              ]}
          >
            <Form layout="vertical">

              <FormItem label="审核意见">
                {getFieldDecorator('message')(<Input maxLength={200} type="textarea" placeholder="请输入意见"/>)}
              </FormItem>
              
            </Form>
          </Modal>
        );
      }
    }
  );

  export default ConfirmForm;