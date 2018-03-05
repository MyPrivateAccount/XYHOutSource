import { connect } from 'react-redux';
import { getDicParList, saveBuildingBasic, viewBuildingBasic,basicLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Input, InputNumber, Button, Checkbox, Row, Col, Form, DatePicker, Select, Cascader } from 'antd'
import moment from 'moment';

const FormItem = Form.Item;
const Option = Select.Option;
/*guid生成*/
function S4() {
  return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}
function NewGuid() {
  return 'XYH' + (S4() + S4() + S4() + S4() + S4()  + S4() + S4() + S4());
}
const contractType = [
  {id:'0', name:'类型1'},
  {id:'1', name:'类型2'},
  {id:'3', name:'类型3'},
  {id:'4', name:'类型4'},
  {id:'5', name:'类型5'},
];
class BasicEdit extends Component {
    contractNumber = NewGuid();
    componentWillMount(){
      if(this.props.basicData.contractCategories.length === 0){
        this.props.dispatch(getDicParList(['CONTRACT_CATEGORIES']));
      }
    }
    render(){
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        //let contractTypes = '1';
        let basicOperType = 'add';
        const formItemLayout = {
          labelCol: { span: 6 },
          wrapperCol: { span: 14 },
        };
        console.log('this.props.basicData.contractCategories:', this.props.basicData.contractCategories);
        return (
          <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px"}}>
          <Icon type="tags-o" className='content-icon'/> <span className='content-title'>基本信息 (必填)</span>
            <Row type="flex" style={{marginTop: "25px"}}>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>合同编号:</span>} >
                      {getFieldDecorator('number', {initialValue: this.contractNumber})
                        (<span className='ant-form-text'>{this.contractNumber}</span>)
                      }
                    </FormItem>
                </Col>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>合同类型</span>}>
                      {getFieldDecorator('cntractCategory', {
                                //initialValue: contractTypes,
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
            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>项目名称</span>}>
                    
                 </FormItem>
              </Col>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>项目类型</span>}>
                    
                 </FormItem>
              </Col>
            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>甲方公司全称</span>}>
                    
                 </FormItem>
              </Col>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>甲方负责人</span>}>
                    
                 </FormItem>
              </Col>
            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>乙方负责人</span>}>
                    
                 </FormItem>
              </Col>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>项目负责人</span>}>
                    
                 </FormItem>
              </Col>
            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>申请人</span>}>
                    
                 </FormItem>
              </Col>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>申请时间</span>}>
                    
                 </FormItem>
              </Col>
            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>申请部门</span>}>
                    
                 </FormItem>
              </Col>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>佣金方式</span>}>
                    
                 </FormItem>
              </Col>
            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>合同开始时间</span>}>
                    
                 </FormItem>
              </Col>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>合同结束时间</span>}>
                    
                 </FormItem>
              </Col>
            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>份数</span>}>
                    
                 </FormItem>
              </Col>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>返回原件</span>}>
                    
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