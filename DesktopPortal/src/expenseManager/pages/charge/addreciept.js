import { connect } from 'react-redux';
import React, { Component } from 'react'
import {Table, notification, Layout, Form, Modal, Upload, InputNumber, Input, Select, Icon, Button, Col, Spin} from 'antd'
import { NewGuid } from '../../../utils/appUtils';
import { postReciept, uploadFile, postChargeInfo ,getDepartment, getRecieptByID, clearCharge} from '../../actions/actionCreator';
import WebApiConfig from '../../constants/webapiConfig';
const FormItem = Form.Item;
const Option = Select.Option;

const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};

class Addreciept extends Component {

    state = {
        previewImage:'',
        previewVisible: false,
        fileList: []
    }
    
    componentWillMount() {
        this.props.dispatch(getRecieptByID(this.props.chargeList[0].id));//获取信息
    }

    componentWillUnmount() {
        this.props.dispatch(clearCharge());
    }
    componentDidMount() {
    }

    handleSubmit = (e)=> {
        e.preventDefault();
        let self = this;
        let chargeid = this.props.chargeList[0].id;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                let receiptinfos = [];
                for (const ite in values) {
                    let ary = ite.split("_");
                    if (ary.length > 1) {
                        if (ary[1] === "reciepcomment") {
                            receiptinfos[+ary[0]] = Object.assign({}, receiptinfos[+ary[0]], {"comments": values[ite]});
                        } else if (ary[1] === "reciepmoney") {
                            receiptinfos[+ary[0]] = Object.assign({}, receiptinfos[+ary[0]], {"receiptmoney": values[ite]});
                        } else if (ary[1] === "reciepnumber") {
                            receiptinfos[+ary[0]] = Object.assign({}, receiptinfos[+ary[0]], {"reciepnumber": values[ite]});
                            receiptinfos[+ary[0]] = Object.assign({},
                                receiptinfos[+ary[0]], {"id": self.props.recieptInfoList[+ary[0]].id});
                            receiptinfos[+ary[0]] = Object.assign({},
                                receiptinfos[+ary[0]], {"chargeid": chargeid});
                        } else if (ary[1] === "recieptype") {
                            receiptinfos[+ary[0]] = Object.assign({}, receiptinfos[+ary[0]], {"type": values[ite]});
                        }
                    }
                }
                this.props.dispatch(postReciept(receiptinfos));
            }
        });
    }

    handlePreview = (file) => {
        this.state.setState({previewImage:　file.url || file.thumbUrl, previewVisible: true});
    }

    handleChange = ({ fileList }) => {
        this.setState({fileList: fileList});
    }

    handleBeforeUpload = (pf) => {
        this.UploadFile(pf, (ufile) => {
            this.props.dispatch(uploadFile(ufile));//这里已经上传图片了，所以不需要记录

        });
        return true;
    }

    handleCancel = () => {
        this.setState({previewVisible: false});
    }

    addCharge = () => {
        this.props.recieptInfoList.push({receiptID: NewGuid(),fileList: []});
        this.setState(this.state);
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    UploadFile = (file, callback) => {
        let id = this.state.id;
        let uploadUrl = `${WebApiConfig.attach.uploadUrl}${id}`;
        let fileGuid = NewGuid();
        let fd = new FormData();
        fd.append("fileGuid", fileGuid)
        fd.append("name", file.name)
        fd.append("file", file);
    
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

    render() {
        const { getFieldDecorator, getFieldsError, getFieldsValue } = this.props.form;
        const uploadButton = (
            <div>
              <Icon type='plus' />
              <div className="ant-upload-text">Upload</div>
            </div>
          );

        let self = this;
        return (
            <Form onSubmit={this.handleSubmit}>
                {
                    this.props.recieptInfoList.map(
                        function(rv, i) {
                            let reciepnumber = i+"_reciepnumber";
                            let reciepmoney = i+"_reciepmoney";
                            let reciepcomment = i+"_reciepcomment";
                            let recieptype = i+"_recieptype";

                            let handleCancel = () => {
                                self.props.recieptInfoList[i].previewVisible = false;
                                self.setState(Object.assign({}, self.state));
                            }

                            let handlePreview = (file) => {
                                self.props.recieptInfoList[i].previewImage = file.url || file.thumbUrl;
                                self.props.recieptInfoList[i].previewVisible = true;
                                self.setState(Object.assign({}, self.state));
                            }

                            let handleChange = ({ fileList }) => {
                                self.props.recieptInfoList[i].fileList = fileList;
                                self.setState(Object.assign({}, self.state));
                            }

                            let handleBeforeUpload = (pf) => {
                                self.UploadFile(pf, (ufile) => {
                                    ufile.receiptID = rv.receiptID;
                                    self.props.dispatch(uploadFile(ufile));//这里已经上传图片了，所以不需要记录
                                });
                                return true;
                            }

                            return (
                                <div key={i}>
                                    <FormItem {...formItemLayout1}/>
                                    <FormItem {...formItemLayout1}/>
                                    <FormItem {...formItemLayout1} label="发票类型">
                                        {getFieldDecorator(recieptype, {
                                            reules: [{
                                                required:true, message: 'please entry Age',
                                            }]
                                        })(
                                            <Select placeholder="选择费用类型">
                                                {
                                                    (self.props.chargeCostTypeList && self.props.chargeCostTypeList.length > 0) ?
                                                        self.props.chargeCostTypeList.map(
                                                            function (params) {
                                                                return <Option key={params.value} value={params.value+""}>{params.key}</Option>;
                                                            }
                                                        ):null
                                                }
                                            </Select>
                                        )}
                                    </FormItem>
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
                                </div>
                            );
                        }
                    )
                }
                <FormItem {...formItemLayout1}/>
                <FormItem {...formItemLayout1}/>
                <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                    <Col span={6}><Button type="primary" icon="plus" onClick={this.addCharge} ></Button></Col>
                </FormItem>
                <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                    <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} ></Button></Col>
                </FormItem>
            </Form>
        );
    }
}


function tableMapStateToProps(state) {
    return {
        chargeList: state.basicData.selchargeList,
        setContractOrgTree: state.basicData.departmentTree,
        chargeCostTypeList: state.basicData.chargeCostTypeList,
        recieptInfoList: state.search.recieptInfoList
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Addreciept));