import { connect } from 'react-redux';
import React, { Component } from 'react'
import {Table, Layout, Form, Modal, Cascader, Upload, InputNumber, Input, Select, Icon, Button, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import { NewGuid } from '../../../utils/appUtils';
import { getDepartment } from '../../actions/actionCreator'

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
        costlist: []
    }

    componentWillMount() {
        this.props.dispatch(getDepartment());
        //this.props.dispatch(searchConditionType(SearchCondition.topteninfo));
    }

    handleSubmit = ()=> {

    }
    
    handleReset = ()=> {

    }

    addCost = () => {

        let t = {
            previewVisible: false,
            previewImage: '',
            fileList: []
        }
    }

    render() {
        const uploadButton = (
            <div>
              <Icon type='plus' />
              <div className="ant-upload-text">Upload</div>
            </div>
          );

          let self = this;
          const { getFieldDecorator, getFieldsError, getFieldsValue } = this.props.form;
        return (
            <Form className="onchargeContent" onSubmit={this.handleSubmit}>
                <FormItem {...formItemLayout}/>
                <FormItem {...formItemLayout} label="编号" >
                    {getFieldDecorator('id', {
                        reules: [{
                            required:true, message: 'please entry IDcard',
                        }, {
                            validator: this.isCardID
                        }]
                    })(
                        <Input enable="false" />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="报销门店">
                    {getFieldDecorator('name', {
                        reules: [{
                            required:true, message: 'please entry Name',
                        }]
                    })(
                        <Cascader options={this.props.setContractOrgTree}  onChange={this.handleChooseDepartmentChange } changeOnSelect  placeholder="归属部门"/>
                    )}
                </FormItem>
                {
                    this.state.costlist.map(
                        function(v, i) {
                            let costtype = 'costtype_' + i;
                            let costmoney = 'costmoney_' + i;
                            let costcomment = 'costcomment_' + i;
                            let chargenumber = 'chargenumber_' + i;
                            let chargemoney = 'chargemoney_' + i;
                            let chargecomment = 'chargecomment_' + i;

                            let handleCancel = () => {
                                self.state.costlist[i].previewVisible = false;
                                self.setState(Object.assign({}, self.state));
                            }

                            let handlePreview = (file) => {
                                self.state.costlist[i].previewImage = file.url || file.thumbUrl;
                                self.state.costlist[i].previewVisible = true;
                                self.setState(Object.assign({}, self.state));
                            }

                            let handleBeforeUpload = () => { return true;}
                        
                            let handleChange = ({ fileList }) => {
                                self.state.costlist[i].fileList = fileList;
                                self.setState(Object.assign({}, self.state));
                            }

                            return (
                                <div>
                                    <FormItem {...formItemLayout1} label="费用类型">
                                        {getFieldDecorator(costtype, {
                                            reules: [{
                                                required:true, message: 'please entry Age',
                                            }]
                                        })(
                                            <Select placeholder="选择费用类型">
                                                {
                                                    this.props.chargeCostTypeList.map(
                                                        function (params) {
                                                            return <Option value="1">{params.key}</Option>;
                                                        }
                                                    )
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
                                            <InputNumber style={{width: '100%'}} />
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
                                    <FormItem {...formItemLayout1} label="发票号">
                                        {getFieldDecorator(chargenumber, {
                                            reules: [{
                                                required:true, message: 'please entry',
                                            }]
                                        })(
                                            <Input placeholder="请输入发票号" />
                                        )}
                                    </FormItem>
                                    <FormItem {...formItemLayout1} label="发票金额">
                                        {getFieldDecorator(chargemoney, {
                                            reules: [{
                                                required:true, message: 'please entry',
                                            }]
                                        })(
                                            <Input placeholder="请输入发票金额" />
                                        )}
                                    </FormItem>
                                    <FormItem {...formItemLayout1} label="备注">
                                        {getFieldDecorator(chargecomment, {
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
                                                fileList={v.fileList}
                                                onPreview={handlePreview}
                                                onChange={handleChange}
                                                beforeUpload={handleBeforeUpload} 
                                                >
                                                {v.fileList.length >= 3 ? null : uploadButton}
                                            </Upload>
                                            <Modal visible={v.previewVisible} footer={null} onCancel={handleCancel}>
                                                <img alt="example" style={{ width: '100%' }} src={v.previewImage} />
                                            </Modal>
                                        </div>
                                    </FormItem>
                                </div>
                            )}
                    )
                }
                <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                    <Col span={6}><Button type="primary" icon="plus" onClick={this.addCost} ></Button></Col>
                </FormItem>
                <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                    <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button></Col>
                    <Col span={6}><Button type="primary" onClick={this.handleReset}>清空</Button></Col>
                </FormItem>
            </Form>
        );
    }
}

function chargetableMapStateToProps(state) {
    return {
        department: state.eachDepartment,
        chargeCostTypeList: state.chargeCostTypeList
    }
}

function chargetableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(chargetableMapStateToProps, chargetableMapDispatchToProps)(Form.create()(ChargeInfo));