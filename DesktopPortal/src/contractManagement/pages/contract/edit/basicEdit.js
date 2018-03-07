import { connect } from 'react-redux';
import { getDicParList, saveBuildingBasic, viewBuildingBasic,basicLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react';
import { Icon, Input, InputNumber, Button, Checkbox, Row, Col, Form, DatePicker, Select, Cascader,Radio } from 'antd';
import moment from 'moment';

const FormItem = Form.Item;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const { RangePicker } = DatePicker;

class BasicEdit extends Component {

    handleChangeTime = (value, dateString)=>{
        console.log('curstatrEndTime:', value);
        console.log('format curstatrEndTime:', dateString);
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
        console.log('this.props.basicData.contractCategories:', this.props.basicData.contractCategories);
        return (
          <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px"}}>
          <Icon type="tags-o" className='content-icon'/> <span className='content-title'>基本信息 (必填)</span>
            <Row type="flex" style={{marginTop: "25px"}}>

            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>合同类型</span>}>
                        {getFieldDecorator('contractType', {
                                    initialValue: basicInfo.contractType,
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
                        {getFieldDecorator('contractName', {
                        initialValue: basicInfo.contractName,
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
                        {getFieldDecorator('projectName', {
                        initialValue: basicInfo.projectName,
                        rules:[{required:true, message:'请输入项目名称!'}]
                        })(
                            <Input placeholder="项目名称" />
                        )
                        
                        }
                    </FormItem>
                </Col>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>项目类型</span>}>
                    {getFieldDecorator('projectType', {
                            initialValue: basicInfo.projectType,
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
                    {getFieldDecorator('firstPartyType', {
                                    initialValue: basicInfo.firstPartyType,
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
                    {getFieldDecorator('firstPartyFirmName', {
                            initialValue: basicInfo.firstPartyFirmName,
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
                        {getFieldDecorator('firstMainPeople', {
                                    initialValue: basicInfo.firstMainPeople,
                                    rules:[{required:true, message:'请输入甲方负责人!'}]
                                    })(
                                        <Input placeholder="甲方负责人" />
                                    )
                                    
                            }
                        </FormItem>
                    </Col>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>乙方负责人</span>}>
                    {getFieldDecorator('secondMainPeople', {
                                initialValue: basicInfo.secondMainPeople,
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
                    {getFieldDecorator('projectPeopleName', {
                                    initialValue: basicInfo.projectPeopleName,
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
                                        initialValue: [basicInfo.startTime ? moment(basicInfo.startTime, 'YYYY-MM-DD') : null, 
                                        basicInfo.endTime ? moment(basicInfo.endTime, 'YYYY-MM-DD') : null],
                                        rules:[{required:true, message:'请选择起始时间!'}]
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
                  {getFieldDecorator('contractNumber', {
                                  initialValue: basicInfo.contractNumber,
                                  rules:[{required:true, message:'请输入份数!'}]
                                  })(
                                      <InputNumber min={0}  />
                                  )
                                  
                     }
                 </FormItem>
              </Col>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>返回原件</span>}>
                  {getFieldDecorator('returnOrigin', {
                                  initialValue: basicInfo.returnOrigin == null ? (basicInfo.returnOrigin === 1 ? '是' :'否') : null ,
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
            </Row>
            <Row type="flex" style={{marginTop:"25px"}}>
                <Col span={12}>
                     <FormItem {...formItemLayout} label={<span>佣金方式</span>}>
                        {getFieldDecorator('commissionType', {
                                    initialValue: basicInfo.commissionType,
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
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>续签合同</span>}>
                        {getFieldDecorator('basicInfo.renewContract', {
                                    initialValue: basicInfo.renewContract,
                                    //rules:[{required:true, message:'续签合同'}]
                                    })(
                                        <span>无</span>
                                    )
                                    
                            }
                    </FormItem>
                </Col>

            </Row>


            <Row>
                    <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button type="primary" htmlType="submit" loading={this.props.loadingState} style={{width: "8rem"}} onClick={this.handleSave}>保存</Button>
                        {basicOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
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
    //   loading: state.shop.basicloading
        contractInfo: state.contractData.contractInfo,
        contractBasicInfo: state.contractData.contractInfo.contractBasicInfo,
        operInfo:state.contractData.operInfo,
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