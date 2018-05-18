import { connect } from 'react-redux';
import React, { Component } from 'react'
import {Table, notification, Layout, Form, Modal, Cascader, Upload, InputNumber, Input, Select, Icon, Button, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import { NewGuid } from '../../../utils/appUtils';
import { getDicInfo, uploadFile, postChargeInfo ,getDepartment} from '../../actions/actionCreator';
import WebApiConfig from '../../constants/webapiConfig';



const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};

class PadCharge extends Component {

    state = {
    }
    
    componentDidMount() {
    }

    handleSubmit = (e)=> {
        e.preventDefault();
        let self = this;
        this.props.form.validateFields((err, values) => {
            if (!err) {

            }
        });
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldsValue } = this.props.form;
        return (
            <Form onSubmit={this.handleSubmit}>
                <FormItem {...formItemLayout}/>
                <FormItem {...formItemLayout}/>
                <FormItem {...formItemLayout1} label="发票号">
                    {getFieldDecorator(reciepnumber, {
                        reules: [{
                            required:true, message: 'please entry',
                        }]
                    })(
                        <Input placeholder="请输入发票号" />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="发票金额">
                    {getFieldDecorator(reciepmoney, {
                        reules: [{
                            required:true, message: 'please entry',
                        }]
                    })(
                        <InputNumber placeholder="请输入发票金额" style={{width: '100%'}} />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="备注">
                    {getFieldDecorator(reciepcomment, {
                        reules: [{
                            required:true, message: 'please entry',
                        }]
                    })(
                        <Input placeholder="请输入备注" />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="附件">
                    <div className="clearfix">
                        <Upload
                            action="//jsonplaceholder.typicode.com/posts/"
                            listType="picture-card"
                            fileList={rv.fileList}
                            onPreview={handlePreview}
                            onChange={handleChange}
                            beforeUpload={handleBeforeUpload} 
                            >
                            {rv.fileList.length >= 3 ? null : uploadButton}
                        </Upload>
                        <Modal visible={rv.previewVisible} footer={null} onCancel={handleCancel}>
                            <img alt="example" style={{ width: '100%' }} src={rv.previewImage} />
                        </Modal>
                    </div>
                </FormItem>
                <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                    <Col span={6}><Button type="primary" icon="plus" onClick={self.addCharge.bind(self, i)} ></Button></Col>
                </FormItem>
            </Form>
        );
    }
}


function tableMapStateToProps(state) {
    return {
        chargeList: state.basicData.selchargeList,
        setContractOrgTree: state.basicData.departmentTree
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(PadCharge));