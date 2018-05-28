import { connect } from 'react-redux';
import React, { Component } from 'react'
import {Table, notification, Layout, Form, Modal, Cascader, Upload, InputNumber, Input, Select, Icon, Button, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import { NewGuid } from '../../../utils/appUtils';
import { getDicInfo, uploadFile, postChargeInfo} from '../../actions/actionCreator';
import WebApiConfig from '../../constants/webapiConfig';
import ApiClient from '../../../utils/apiClient';

const Option = Select.Option;
const FormItem = Form.Item;
const { Header, Sider, Content } = Layout;
const CheckboxGroup = Checkbox.Group;
const ButtonGroup = Button.Group;
const formItemLayout = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};
const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};

class ChargeInfo extends Component {

    state = {
        id: NewGuid(),
        costlist: [
            {
                costID: NewGuid(),
                receiptList: [{
                    previewVisible: false,
                    previewImage: '',
                    receiptID: NewGuid(),
                    fileList: []
                }],
            }
        ],
    }

    componentWillMount() {
        let tempthis = this;
        let url = WebApiConfig.server.getChargeid;
        ApiClient.get(url).then(function (f) {
            if (f.data.code==0) {
                tempthis.props.form.setFieldsValue({id: f.data.extension});
                tempthis.state.id = f.data.extension;
            }
        });
        this.props.dispatch(getDicInfo(["CHARGE_COST_TYPE"]));
        //this.props.dispatch(searchConditionType(SearchCondition.topteninfo));
    }

    componentDidMount() {
        if (this.props.isdetail) {
            //this.props.form.setFieldsValue({id:this.state.id});
        } else {
            this.props.form.setFieldsValue({id:this.state.id});
        }
    }

    handleChooseDepartmentChange = (e) => {
        this.state.department = e;
    }

    handleSubmit = (e) => {
        e.preventDefault();
        let self = this;
        this.props.form.validateFields((err, values) => {
            if (!err) {

                let costinfos = [];//1_aa_1/1_aa_2
                let receiptinfos = [];
                for (const ite in values) {
                    let ary = ite.split("_");
                    if (ary.length > 1) {
                        
                        if (ary[1] === "reciepcomment") {
                            receiptinfos[+ary[3]] = Object.assign({}, receiptinfos[+ary[3]], {"comments": values[ite]});
                        } else if (ary[1] === "reciepmoney") {
                            receiptinfos[+ary[3]] = Object.assign({}, receiptinfos[+ary[3]], {"receiptmoney": values[ite]});
                        } else if (ary[1] === "reciepnumber") {
                            receiptinfos[+ary[3]] = Object.assign({}, receiptinfos[+ary[3]], {"reciepnumber": values[ite]});
                            receiptinfos[+ary[3]] = Object.assign({},
                                 receiptinfos[+ary[3]], {"id": self.state.costlist[+ary[0]].receiptList[+ary[2]].receiptID});
                            receiptinfos[+ary[3]] = Object.assign({},
                                 receiptinfos[+ary[3]], {"costid": self.state.costlist[+ary[0]].costID});
                            receiptinfos[+ary[3]] = Object.assign({},
                                receiptinfos[+ary[3]], {"chargeid": self.state.id});
                            receiptinfos[+ary[3]] = Object.assign({},
                                receiptinfos[+ary[3]], {"type": values[ary[0]+"costtype"]});
                        
                        } else if (ary[1] === "costcomment") {
                            costinfos[+ary[0]] = Object.assign({}, costinfos[+ary[0]], {"comments": values[ite]});
                        } else if (ary[1] === "costmoney") {
                            costinfos[+ary[0]] = Object.assign({}, costinfos[+ary[0]], {"cost": values[ite]});
                        } else if (ary[1] === "costtype") {
                            costinfos[+ary[0]] = Object.assign({}, costinfos[+ary[0]], {"type": values[ite]});
                            costinfos[+ary[0]] = Object.assign({}, costinfos[+ary[0]], {"id": self.state.costlist[+ary[0]].costID});
                            costinfos[+ary[0]] = Object.assign({}, costinfos[+ary[0]], {"chargeid": self.state.id});
                        }
                    }
                }
                let tf = {
                    chargeinfo: {
                        id: self.state.id,
                        department: values.department.join("/")
                    },
                    costinfos: costinfos,
                    receiptinfos: receiptinfos
                }
                this.props.dispatch(postChargeInfo(tf));
            }
        });
    }
    
    handleReset = ()=> {
        this.props.form.resetFields();
    }

    addCost = () => {
        let t = {
            costID: NewGuid(),
            receiptList: [{
                previewVisible: false,
                previewImage: '',
                receiptID: NewGuid(),
                fileList: []
            }],
        }
        this.state.costlist.push(t);
        this.setState({costlist: this.state.costlist});
    }

    addCharge = (i) => {
        this.state.costlist[i].receiptList.push({receiptID: NewGuid(),fileList: []});
        this.setState({costlist: this.state.costlist});
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
        const uploadButton = this.props.isdetail?null:(
            <div>
              <Icon type='plus' />
              <div className="ant-upload-text">Upload</div>
            </div>
          );

          let nindex = 0;
          let self = this;
          const { getFieldDecorator, getFieldsError, getFieldsValue } = this.props.form;
        return (
            <Form className="onchargeContent" onSubmit={this.handleSubmit}>
                <FormItem {...formItemLayout}/>
                <FormItem {...formItemLayout} label="编号" >
                    {getFieldDecorator('id', {
                        reules: [{
                            required:true, message: 'please entry IDcard',
                        }]
                    })(
                        <Input disabled={true} />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="报销门店">
                    {getFieldDecorator('department', {
                        reules: [{
                            required:true, message: 'please entry department',
                        }]
                    })(
                        <Cascader options={this.props.setContractOrgTree}  onChange={this.handleChooseDepartmentChange } changeOnSelect  placeholder="归属部门"/>
                    )}
                </FormItem>
                {
                    self.state.costlist.map(
                        function(v, i) {
                            let costtype = i+'_costtype';
                            let costmoney = i+'_costmoney';
                            let costcomment = i+'_costcomment';

                            let reciepnumberroot = i+'_reciepnumber';
                            let reciepmoneyroot = i+'_reciepmoney';
                            let reciepcommentroot = i+'_reciepcomment';

                            return (
                                <div key={i}>
                                    <FormItem {...formItemLayout}/>
                                    <FormItem {...formItemLayout}/>
                                    <FormItem {...formItemLayout}/>
                                    <FormItem {...formItemLayout1} label="费用类型">
                                        {getFieldDecorator(costtype, {
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
                                    <FormItem {...formItemLayout1} label="金额">
                                        {getFieldDecorator(costmoney, {
                                            reules: [{
                                                required:true, message: 'please entry',
                                            }]
                                        })(
                                            <InputNumber placeholder="请输入金额" style={{width: '100%'}} />
                                        )}
                                    </FormItem>
                                    <FormItem {...formItemLayout1} label="摘要">
                                        {getFieldDecorator(costcomment, {
                                            reules: [{
                                                required:true, message: 'please entry',
                                            }]
                                        })(
                                            <Input placeholder="请输入摘要" />
                                        )}
                                    </FormItem>
                                    {
                                        v.receiptList.map(
                                            function(rv, ri) {
                                                let reciepnumber = reciepnumberroot+'_'+ri+'_'+nindex;
                                                let reciepmoney = reciepmoneyroot+'_'+ri+'_'+nindex;
                                                let reciepcomment = reciepcommentroot+'_'+ri+'_'+nindex;

                                                nindex++;

                                                let handleCancel = () => {
                                                    self.state.costlist[i].receiptList[ri].previewVisible = false;
                                                    self.setState(Object.assign({}, self.state));
                                                }
                    
                                                let handlePreview = (file) => {
                                                    self.state.costlist[i].receiptList[ri].previewImage = file.url || file.thumbUrl;
                                                    self.state.costlist[i].receiptList[ri].previewVisible = true;
                                                    self.setState(Object.assign({}, self.state));
                                                }
                    
                                                let handleChange = ({ fileList }) => {
                                                    self.state.costlist[i].receiptList[ri].fileList = fileList;
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
                                                    <div key={ri}>
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
                                                    </div>
                                                );
                                            }
                                        )
                                    }
                                    <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                                        <Col span={6}><Button type="primary" icon="plus" onClick={self.addCharge.bind(self, i)} ></Button></Col>
                                    </FormItem>
                                </div>
                            )}
                    )
                }
                <FormItem {...formItemLayout}/>
                <FormItem {...formItemLayout}/>
                {
                    this.props.isdetail?null:(
                        <div>
                            <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                                <Col span={6}><Button type="primary" icon="plus" onClick={this.addCost.bind(this)} ></Button></Col>
                            </FormItem>
                            <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                                <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button></Col>
                                <Col span={6}><Button type="primary" onClick={this.handleReset}>清空</Button></Col>
                            </FormItem>
                        </div>
                    )
                }
                
            </Form>
        );
    }
}

function chargetableMapStateToProps(state) {
    return {
        selchargeList: state.basicData.selchargeList,
        department: state.basicData.eachDepartment,
        chargeCostTypeList: state.basicData.chargeCostTypeList,
        setContractOrgTree: state.basicData.departmentTree
    }
}

function chargetableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(chargetableMapStateToProps, chargetableMapDispatchToProps)(Form.create()(ChargeInfo));