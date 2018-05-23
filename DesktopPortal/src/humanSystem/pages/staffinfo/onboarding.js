import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer';
import {Form,Modal, Cascader,Input,InputNumber,DatePicker,notification, Select, Icon,Upload, Button, Row, Col, Checkbox,TreeSelect, Tag, Spin} from 'antd'
import {connect} from 'react-redux';
import reducers from '../../reducers';
import moment from 'moment';
import WebApiConfig from '../../constants/webapiConfig';
import './staff.less';
import { getworkNumbar, postHumanInfo, getallOrgTree} from '../../actions/actionCreator';
import { NewGuid } from '../../../utils/appUtils';
import ApiClient from '../../../utils/apiClient';

const Option = Select.Option;
const FormItem = Form.Item;

const formItemLayout = {
    labelCol:{ span:6},
    wrapperCol:{ span:8 },
};

const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};

class OnBoarding extends Component {

    state = {
        loading: false,
        fileList: [],
        previewVisible: false,
        previewImage: '',
        fileinfo:{},
        userinfo:{}
    }

    componentWillMount() {
        this.state.userinfo.id = NewGuid();
        //this.props.dispatch(getallOrgTree('PublicRoleOper'));
    }

    componentDidMount() {
        this.getWorkNumber();
    }
    
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

    handleCancel = () => this.setState({ previewVisible: false })

    handlePreview = (file) => {
        this.setState({
        previewImage: file.url || file.thumbUrl,
        previewVisible: true,
        });
    }
    handleChange = ({ fileList }) => this.setState({ fileList })

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    getWorkNumber() {
        let tempthis = this;
        let url = WebApiConfig.server.GetWorkNumber;
        ApiClient.get(url).then(function (f) {
            if (f.data.code==0) {
                tempthis.props.form.setFieldsValue({id:f.data.extension});
            }
        });
    }

    UploadFile(file, callback) {
        let id = this.state.userinfo.id;
        let uploadUrl = `${WebApiConfig.attach.uploadUrl}${id}`;
        let fileGuid = NewGuid();
        let fd = new FormData();
        fd.append("fileGuid", fileGuid)
        fd.append("name", file.name)
        fd.append("file", file);
    
        let _this = this;
        var xhr = new XMLHttpRequest();
        xhr.open('POST', uploadUrl, true);
        xhr.send(fd);
        xhr.onload = function (e) {
          if (this.status === 200) {
            let r = JSON.parse(this.response);
             console.log("返回结果：", this.response);
            if (r.code === "0") {
              let uf = {
                fileGuid: fileGuid,
                from: 'pc-upload',
                WXPath: r.extension,
                sourceId: id,
                appId: 'contractManagement',
                localUrl: file.url,
                name: file.name
              }
              // console.log("pic值", uf);
              if (callback) {
                callback(uf);
              }
            } else {
              notification.error({
                message: '上传失败：',
                duration: 3
              });
            }
          } else {
            notification.error({
              message: '图片上传失败!',
              duration: 3
            });
          }
        }
        xhr.onerror = function (e) {
          notification.error({
            message: '图片上传失败!',
            duration: 3
          });
        }
        xhr.onabort = function () {
          notification.error({
            message: '图片上传异常终止!',
            duration: 3
          });
        }
      }
    
    handleBeforeUpload = (uploadFile) => {
        let reader = new FileReader();
        let the = this;
        reader.readAsDataURL(uploadFile);
    
        reader.onloadend = function () {
        the.UploadFile(uploadFile, (ufile) => {
            let filelist = [{
                uid: -1,
                name: uploadFile.name,
                status: 'done',
                url: reader.result,
              }]
            the.setState({ fileinfo: ufile, fileList: filelist});
          });
        }
        return true;
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                this.props.dispatch(postHumanInfo({humaninfo:values, fileinfo:this.state.fileinfo}));
            }
        });
    }

    handleReset = () => {
        this.props.form.resetFields();
    }

    handleChooseDepartmentChange =  (v, selectedOptions) => {
        let organizateID = (v ||v != []) ? v[v.length -1].toString() : 0;
        let departmentFullId = v? v.join('*'): '';

        let text = selectedOptions.map(item => {
            return item.label
        })
     
        this.setState({departMentFullName: text.join('-'), organizateID: organizateID, departmentFullId:departmentFullId})
    }

    render() {
        const { previewVisible, previewImage, fileList } = this.state;
        const { getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched } = this.props.form;

        const uploadButton = (
            <div>
              <Icon type='plus' />
              <div className="ant-upload-text">Upload</div>
            </div>
          );

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
                <FormItem {...formItemLayout1} label="姓名">
                    {getFieldDecorator('name', {
                        reules: [{
                            required:true, message: 'please entry Name',
                        }]
                    })(
                        <Input placeholder="请输入姓名" />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="性别">
                    {getFieldDecorator('sex', {
                        reules: [{
                            required:true, message: 'please entry Age',
                        }]
                    })(
                        <Select
                            placeholder="选择性别">
                            <Option value="1">男</Option>
                            <Option value="2">女</Option>
                        </Select>
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="年龄">
                    {getFieldDecorator('age', {
                        reules: [{
                            required:true, message: 'please entry Age',
                        }]
                    })(
                        <InputNumber style={{width: '100%'}} />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="生日">
                    {getFieldDecorator('birthday', {
                        reules: [{
                            required:true, message: 'please entry Birthday',
                        }]
                    })(
                        <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="工号">
                    {getFieldDecorator('id', {
                        reules: [{
                            required:true,
                            message: 'please entry Worknumber',
                            initialValue: this.state.userinfo.worknumber
                        }]
                    })(
                        <Input disabled={true} />
                    )}
                </FormItem>
                <FormItem {...formItemLayout} label="图片">
                    <div className="clearfix">
                        <Upload
                            action="//jsonplaceholder.typicode.com/posts/"
                            listType="picture-card"
                            fileList={fileList}
                            onPreview={this.handlePreview}
                            onChange={this.handleChange}
                            beforeUpload={this.handleBeforeUpload} 
                            >
                            {fileList.length >= 1 ? null : uploadButton}
                        </Upload>
                        <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                            <img alt="example" style={{ width: '100%' }} src={previewImage} />
                        </Modal>
                    </div>
                </FormItem>
                <FormItem {...formItemLayout1} label="入职时间">
                    {getFieldDecorator('entrytime', {
                            reules: [{
                                required:true,
                                message: 'please entry Entrytime',
                                initialValue: moment()
                            }]
                        })(
                            <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                        )}
                </FormItem>
                <FormItem {...formItemLayout} label="所属部门">
                    {getFieldDecorator('orgdepartment', {
                                reules: [{
                                    required:true,
                                    message: 'please entry Orgdepartment',
                                }]
                            })(
                                <Cascader options={this.props.setDepartmentOrgTree}  onChange={this.handleChooseDepartmentChange } changeOnSelect  placeholder="归属部门"/>
                            )}
                </FormItem>
                <FormItem {...formItemLayout} label="职位">
                    {getFieldDecorator('position', {
                                reules: [{
                                    required:true,
                                    message: 'please entry Position',
                                }]
                            })(
                                <TreeSelect style={{ width: '70%' }} allowClear showSearch
                                        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                        treeData={this.props.positionTree} />
                            )}
                </FormItem>
                <FormItem {...formItemLayout1} label="基本工资">
                    {getFieldDecorator('basicsalary')(
                                    <InputNumber style={{width: '100%'}} />
                                )}
                </FormItem>
                <FormItem {...formItemLayout1} label="岗位补贴">
                    {getFieldDecorator('subsidy')(
                                        <InputNumber style={{width: '100%'}} />
                                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="工装扣款">
                    {getFieldDecorator('clothesback')(
                                    <InputNumber style={{width: '100%'}} />
                                )}
                </FormItem>
                <FormItem {...formItemLayout1} label="行政扣款">
                    {getFieldDecorator('administrativeback')(
                                    <InputNumber style={{width: '100%'}} />
                                )}
                </FormItem>
                <FormItem {...formItemLayout1} label="端口扣款">
                    {getFieldDecorator('portback')(
                                    <InputNumber style={{width: '100%'}} />
                                )}
                </FormItem>
                <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                    <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button></Col>
                    <Col span={6}><Button type="primary" onClick={this.handleReset}>清空</Button></Col>
                </FormItem>
            </Form>
        );
    }
}

;

function stafftableMapStateToProps(state) {
    return {
        setDepartmentOrgTree: state.searchOrgTree
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

export default connect(null, stafftableMapDispatchToProps)(Form.create()(OnBoarding));