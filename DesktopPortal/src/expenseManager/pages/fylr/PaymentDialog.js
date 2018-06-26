import React, {Component} from 'react';
import {DatePicker, Modal, Icon, Table, Form, Checkbox, Input,TreeSelect, Row, Col,Button, notification,Spin} from 'antd'
import moment from 'moment'

const FormItem = Form.Item;


const PaymentForm = Form.create()(
    class extends React.Component {

        componentDidMount=()=>{
            this.props.form.setFieldsValue({paymentDate:moment(new Date())})
        }

        componentWillReceiveProps=(nextProps)=>{
            if(nextProps.initData!== this.props.initData && nextProps.initData){
                this.props.form.setFieldsValue({payee: nextProps.initData.payee, paymentAmount: nextProps.initData.chargeAmount - nextProps.initData.paymentAmount})
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
                <Button key="submit" type="primary" disabled={loading} onClick={this.confirm}>
                  确认
                </Button>,
              ]}
          >
            <Form layout="vertical">
              <FormItem label="付款日期">
                {getFieldDecorator('paymentDate')(<DatePicker />)}
              </FormItem>
              <FormItem label="付款金额">
                {getFieldDecorator('paymentAmount')(<Input  />)}
              </FormItem>
              <FormItem label="收款单位">
                {getFieldDecorator('payee')(<Input />)}
              </FormItem>
              <FormItem label="备注">
                {getFieldDecorator('memo')(<Input maxLength={200} type="textarea" placeholder="请输入备注"/>)}
              </FormItem>
              
            </Form>
          </Modal>
        );
      }
    }
  );

  export default PaymentForm;