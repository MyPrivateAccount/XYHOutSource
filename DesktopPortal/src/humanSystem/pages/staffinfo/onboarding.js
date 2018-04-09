import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {Form, Input,InputNumber,DatePicker, Select, Icon,Upload, Button, Row, Col, Checkbox,TreeSelect, Tag, Spin} from 'antd'
import {connect} from 'react-redux';
import reducers from '../../reducers';
import moment from 'moment';
import './staff.less';
const FormItem = Form.Item;

const formItemLayout = {
    labelCol:{ span:6},
    wrapperCol:{ span:8 },
};

class OnBoarding extends Component {

    isCardID(rule, value, callback) {
        var reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
        if(reg.test(value) === false)
        {
            callback('Wrong type')
            return false;
        }
        callback();
        return true;
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit(subdata) {

    }
    
    render() {
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;

        return (
            <Form className="onboardContent" onSubmit={this.handleSubmit}>
                <FormItem {...formItemLayout}/>
                <FormItem {...formItemLayout} label="身份证号" >
                    {getFieldDecorator('idcard', {
                        reules: [{
                            required:true, message: 'please entry IDcard',
                        }, {
                            validator: this.isCardID
                        }]
                    })(
                        <Input placeholder="请输入身份证号码" />
                    )}
                </FormItem>
                <FormItem {...formItemLayout} label="姓名">
                    <Input placeholder="请输入姓名" />
                </FormItem>
                <FormItem {...formItemLayout} label="年龄">
                    <InputNumber style={{width: '100%'}} />
                </FormItem>
                <FormItem {...formItemLayout} label="生日">
                    <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                </FormItem>
                <FormItem {...formItemLayout} label="工号">
                    <Input disabled={true} />
                </FormItem>
                <FormItem {...formItemLayout} label="图片">
                    <div className="dropbox">
                        {getFieldDecorator('dragger', {
                        valuePropName: 'fileList',
                        getValueFromEvent: this.normFile,
                        })(
                        <Upload.Dragger name="files" action="/upload.do">
                            <p className="ant-upload-drag-icon">
                            <Icon type="inbox" />
                            </p>
                            <p className="ant-upload-text">点击或拖拽头像到此区域</p>
                        </Upload.Dragger>
                        )}
                    </div>
                </FormItem>
                <FormItem {...formItemLayout} label="入职时间">
                    <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} defaultValue={moment()} />
                </FormItem>
                <FormItem {...formItemLayout} label="所属部门">
                    <TreeSelect style={{ width: '70%' }} allowClear showSearch
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.organizateTree} />
                </FormItem>
                <FormItem {...formItemLayout} label="职位">
                    <TreeSelect style={{ width: '70%' }} allowClear showSearch
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.positionTree} />
                </FormItem>
                <FormItem {...formItemLayout} label="基本工资">
                    <Input />
                </FormItem>
                <FormItem {...formItemLayout} label="岗位补贴">
                    <Input />
                </FormItem>
                <FormItem {...formItemLayout} label="工装扣款">
                    <Input />
                </FormItem>
                <FormItem {...formItemLayout} label="行政扣款">
                    <Input />
                </FormItem>
                <FormItem {...formItemLayout} label="端口扣款">
                    <Input />
                </FormItem>
                <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                    <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsError())} >提交</Button></Col>
                    <Col span={6}><Button type="primary" htmlType="submit">清空</Button></Col>
                </FormItem>
            </Form>
        );
    }
}

;

function stafftableMapStateToProps(state) {
    return {
        
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

export default connect(null, stafftableMapDispatchToProps)(Form.create()(OnBoarding));