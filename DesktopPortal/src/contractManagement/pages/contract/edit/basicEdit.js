import { connect } from 'react-redux';
import { getDicParList, saveContractBasic, viewBuildingBasic,basicLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react';
import { Icon, Input, InputNumber, Button, Checkbox, Row, Col, Form, DatePicker, Select, Cascader,Radio } from 'antd';
import moment from 'moment';
import { call } from 'redux-saga/effects';

const FormItem = Form.Item;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const { RangePicker } = DatePicker;
const { TextArea } = Input;
class BasicEdit extends Component {

    handleCancel = () => this.setState({ previewVisible: false })
    handleChangeTime = (value, dateString)=>{
        console.log('curstatrEndTime:', value);
        console.log('format curstatrEndTime:', dateString);
    }
    handleSave = (e) => {
        e.preventDefault();
        let { basicOperType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                // this.setState({ loadingState: true });
                
                this.props.dispatch(basicLoadingStart())
                let StartTime = moment(values.startAndEndTime[0]).format("YYYY-MM-DD");
                let EndTime = moment(values.startAndEndTime[1]).format("YYYY-MM-DD");
                let newBasicInfo = Object.assign({},values, {StartTime:StartTime, EndTime:EndTime});
                delete newBasicInfo.startAndEndTime;
    
                newBasicInfo.id = this.props.contractInfo.id;
                if(basicOperType === 'add')
                {
                    newBasicInfo.CreateTime = moment().format("YYYY-MM-DD");
                    newBasicInfo.CreateDepartment = this.props.activeOrg.organizationName;
                }
 
                if (basicOperType != 'add') {
                    newBasicInfo = Object.assign({}, this.props.contractBasicInfo, values);
                }
                // newBasicInfo.city = values.location[0];
                // newBasicInfo.district = values.location[1];
                // newBasicInfo.area = values.location[2];
                // newBasicInfo.areaFullName = this.state.areaFullName
                //console.log('newBasicInfo:', newBasicInfo);
                let method = (basicOperType === 'add' ? 'POST' : "PUT");
                this.props.dispatch(saveContractBasic({ 
                    method: method, 
                    entity: newBasicInfo, 
                    //ownCity: this.props.user.City 
                }));
            }
        });
    }
    handleChectDuringTime = (rule, value, callback) =>{
        const form = this.props.form;
        const duringTime = form.getFieldValue('startAndEndTime') || [];
        if(duringTime[0] === null || duringTime[1] === null){
            callback("请选择开始和结束时间");
        }
        else if(moment(duringTime[0]).Date >= moment(duringTime[1]).Date){

            callback("结束时间不能小于开始时间");
        }
        else
        {
            callback();
        }
    }
    render(){
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        //let contractTypes = '1';
        let basicOperType = this.props.basicOperType;
        let basicInfo = this.props.contractBasicInfo;
  
        const formItemLayout = {
          labelCol: { span: 6 },
          wrapperCol: { span: 14 },
        };
        //console.log('this.props.basicData.contractCategories:', this.props.basicData.contractCategories);
        return (
          <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px"}}>
          <Icon type="tags-o" className='content-icon'/> <span className='content-title'>基本信息 (必填)</span>
            <Row type="flex" style={{marginTop: "25px"}}>

            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>合同类型</span>}>
                        {getFieldDecorator('ContractType', {
                                    initialValue: basicInfo.ContractType,
                                    rules: [{ required: true, message: '请选择合同类型!' }],
                                })(
                                    <Select>
                                        {
                                            this.props.basicData.contractCategories.map((item) =>
                                                <Option key={item.value}>{item.key}</Option>
                                            )
                                        }
                                    </Select>
                        )}
                        </FormItem>
                    </Col>
                <Col span={12}>
                    
                    <FormItem {...formItemLayout} label={<span>合同名称</span>}>
                        {getFieldDecorator('ContractName', {
                        initialValue: basicInfo.Name,
                        rules:[{required:true, message:'请输入合同名称!'}]
                        })(
                            <Input placeholder="合同名称" />
                        )
                        
                        }
                    </FormItem>
                </Col>

            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
                <Col span={12}>
                    
                    <FormItem {...formItemLayout} label={<span>项目名称</span>}>
                        {getFieldDecorator('ProjectName', {
                        initialValue: basicInfo.ProjectName,
                        rules:[{required:true, message:'请输入项目名称!'}]
                        })(
                            <Input placeholder="项目名称" />
                        )
                        
                        }
                    </FormItem>
                </Col>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>项目类型</span>}>
                    {getFieldDecorator('ProjectType', {
                            initialValue: basicInfo.ProjectType,
                            rules:[{required:true, message:'请选择项目类型!'}]
                            })(
                                <Select>
                                {
                                    this.props.basicData.contractProjectCatogories.map((item) =>
                                        <Option key={item.value}>{item.key}</Option>
                                    )
                                }
                                </Select>
                            )
                            
                    }
                    </FormItem>
                </Col>


            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>甲方类型</span>}>
                    {getFieldDecorator('CompanyAType', {
                                    initialValue: basicInfo.CompanyAType,
                                    rules: [{ required: true, message: '请选择甲方类型!' }],
                                })(
                                    <Select>
                                        {
                                            this.props.basicData.firstPartyCatogories.map((item) =>
                                                <Option key={item.value}>{item.key}</Option>
                                            )
                                        }
                                    </Select>
                        )}
                    </FormItem>
                </Col>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>甲方公司全称</span>}>
                    {getFieldDecorator('CompanyA', {
                            initialValue: basicInfo.CompanyA,
                            rules:[{required:true, message:'请输入甲方公司全称!'}]
                            })(
                                <Input placeholder="甲方公司全称" />
                            )
                            
                    }
                    </FormItem>
                </Col>


            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
                <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>甲方负责人</span>}>
                        {getFieldDecorator('PrincipalpepoleA', {
                                    initialValue: basicInfo.PrincipalpepoleA,
                                    rules:[{required:true, message:'请输入甲方负责人!'}]
                                    })(
                                        <Input placeholder="甲方负责人" />
                                    )
                                    
                            }
                        </FormItem>
                    </Col>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>乙方负责人</span>}>
                    {getFieldDecorator('PrincipalpepoleB', {
                                initialValue: basicInfo.PrincipalpepoleB,
                                rules:[{required:true, message:'请输入乙方负责人!'}]
                                })(
                                    <Input placeholder="乙方负责人" />
                                )
                                
                        }
                    </FormItem>
                </Col>

            </Row>

            <Row type="flex" style={{marginTop: "25px"}}>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>项目负责人</span>}>
                    {getFieldDecorator('ProprincipalPepole', {
                                    initialValue: basicInfo.ProprincipalPepole,
                                    rules:[{required:true, message:'请输入项目负责人!'}]
                                    })(
                                        <Input placeholder="项目负责人" />
                                    )
                                    
                            }
                    </FormItem>
                </Col>
              <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>合同起始时间</span>}>
                        {getFieldDecorator('startAndEndTime', {
                                        initialValue: [basicInfo.StartTime ? moment(basicInfo.StartTime, 'YYYY-MM-DD') : null, 
                                        basicInfo.EndTime ? moment(basicInfo.EndTime, 'YYYY-MM-DD') : null],
                                        rules:[{required:true, message:'请选择起始时间!'},
                                               {validator: this.handleChectDuringTime}],
                                        })(
                                            <RangePicker
                                              format="YYYY-MM-DD"
                                              placeholder={['开始时间', '结束时间']}
                                              onChange={this.handleChangeTime}
                                          />
                                        )
                                        
                                }
                    </FormItem>

              </Col>

            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>份数</span>}>
                  {getFieldDecorator('Count', {
                                  initialValue: basicInfo.Count,
                                  rules:[{required:true, message:'请输入份数!'}]
                                  })(
                                      <InputNumber min={1}  />
                                  )
                                  
                     }
                 </FormItem>
              </Col>
              <Col span={12}>
                     <FormItem {...formItemLayout} label={<span>佣金方式</span>}>
                        {getFieldDecorator('CommisionType', {
                                    initialValue: basicInfo.CommisionType,
                                    rules:[{required:true, message:'请选择佣金方式'}]
                                    })(
                                        <Select>
                                        {
                                            this.props.basicData.commissionCatogories.map((item) =>
                                                <Option key={item.value}>{item.key}</Option>
                                            )
                                        }
                                        </Select>
                                    )
                                    
                            }
                    </FormItem>
                </Col>

            </Row>
            <Row type="flex" style={{marginTop:"25px"}}>
            <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>返回原件</span>}>
                  {getFieldDecorator('ReturnOrigin', {
                                  initialValue: basicInfo.ReturnOrigin == null ? (basicInfo.returnOrigin === 1 ? '是' :'否') : null ,
                                  rules:[{required:true, message:'请选择是否返还原件!'}]
                                  })(
                                    <RadioGroup >
                                        <Radio value={1}>是</Radio>
                                        <Radio value={2}>否</Radio>
                                    </RadioGroup>
                                  )
                                  
                  }
                 </FormItem>
              </Col>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>续签合同</span>}>
                        {getFieldDecorator('Follow', {
                                    initialValue: basicInfo.Follow,
                                    //rules:[{required:true, message:'续签合同'}]
                                    })(
                                        <span title="点击选择">无</span>
                                    )
                                    
                            }
                    </FormItem>
                </Col>

            </Row>
            <Row type="flex" style={{marginTop:"25px"}}>
                {
                    basicOperType === 'edit' ? 
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>是否作废</span>}>
                        {getFieldDecorator('IsCancel', {
                                        initialValue: basicInfo.IsCancel == null ? (basicInfo.IsCancel === 1 ? '是' :'否') : null ,
                                        rules:[{required:true, message:'请选择是否作废!'}]
                                        })(
                                            <RadioGroup >
                                                <Radio value={1}>是</Radio>
                                                <Radio value={2}>否</Radio>
                                            </RadioGroup>
                                        )
                                        
                        }
                        </FormItem>
                    </Col>
                    : null
                }
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>备注</span>}>
                        {getFieldDecorator('Remark', {
                                    initialValue: basicInfo.Remark,
                                    //rules:[{required:true, message:'续签合同'}]
                                    })(
                                        <TextArea placeholder="备注" autosize />
                                    )
                                    
                            }
                    </FormItem>
                </Col>

            </Row>


            <Row>
                    <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button type="primary" htmlType="submit" loading={this.props.loadingState} style={{width: "8rem"}} onClick={this.handleSave}>保存</Button>
                        {basicOperType !== "add" ? <Button className="oprationBtn" onClick={this.handleCancel}>取消</Button> : null}
                    </Col>
                </Row>
          </Form>
        )
    }
}

function mapStateToProps(state) {
    return {
    //   dataSource: state.shop.buildingNoInfos,
    //   basicInfo: state.shop.shopsInfo.basicInfo,
    //   buildingList: state.shop.buildingList,
    //   operInfo: state.shop.operInfo,
    //   shopsInfo: state.shop.shopsInfo,
        basicData: state.basicData,
        loadingState: state.contractData.basicloading,
        contractInfo: state.contractData.contractInfo,
        contractBasicInfo: state.contractData.contractInfo.contractBasicInfo,
        operInfo:state.contractData.operInfo,
        activeOrg: state.search.activeOrg,
    }
  }
  
  const mapDispatchToProps = (dispatch) => {
    return {
      dispatch,
    //   save: (...args) => dispatch(saveShopBasicAsync(...args)),
    //   getBuildingsList: () => dispatch(getBuildingsListAsync()),
    //   viewShopBasic: () => dispatch(viewShopBasic())
    }
  }
  
  export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(BasicEdit));