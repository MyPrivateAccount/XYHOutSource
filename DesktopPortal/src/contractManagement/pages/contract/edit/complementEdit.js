import { connect } from 'react-redux';
import { getDicParList, saveContractBasic, viewContractBasic,basicLoadingStart, openContractChoose } from '../../../actions/actionCreator';
import React, { Component } from 'react';
import { Icon, Input, InputNumber, Button, Checkbox, Row, Col, Form, DatePicker, Select, Cascader,Radio } from 'antd';
import moment from 'moment';



const FormItem = Form.Item;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const { RangePicker } = DatePicker;
const { TextArea } = Input;
class ComplementEdit extends Component {

    handleSave = (e) => {
        e.preventDefault();
        let { basicOperType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                // this.setState({ loadingState: true });
                
                this.props.dispatch(basicLoadingStart())
                let StartTime = moment(values.startAndEndTime[0]).format("YYYY-MM-DD");
                let EndTime = moment(values.startAndEndTime[1]).format("YYYY-MM-DD");
                let newBasicInfo = Object.assign({},values, {startTime:StartTime, endTime:EndTime});
                
    
                newBasicInfo.id = this.props.contractInfo.baseInfo.id;

               // newBasicInfo.relation = this.state.departmentFullId;
                if(basicOperType === 'add')
                {
                    newBasicInfo.createTime = moment().format("YYYY-MM-DD");
                    //newBasicInfo.createDepartment = this.props.activeOrg.organizationName;
                }
 
                if (basicOperType != 'add') {
                    newBasicInfo = Object.assign({}, this.props.basicInfo, values);
                }
                newBasicInfo.isSubmmitShop = 1;
                newBasicInfo.isSubmmitRelation = 1;
                newBasicInfo.createDepartment = this.props.activeOrg.organizationName = "";
                newBasicInfo.organizete = this.state.departMentFullName;
                newBasicInfo.createDepartmentID = this.state.createDepartmentID;
                
                delete newBasicInfo.startAndEndTime;
                let method = (basicOperType === 'add' ? 'POST' : "PUT");
    
                this.props.dispatch(saveContractBasic({ 
                    method: method, 
                    entity: newBasicInfo, 
        
                }));
            }
        });
    }

    render(){
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        const formItemLayout = {
          labelCol: { span: 6 },
          wrapperCol: { span: 14 },
        };
       
        return (
          <div>
          <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px"}}>
          <Icon type="tags-o" className='content-icon'/> <span className='content-title'>补充协议</span>
     

            <Row type="flex" style={{marginTop:"25px"}}>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>补充内容</span>}>
                        {getFieldDecorator('contentInfo', {
                                    initialValue: this.props.complementInfo.contentInfo,
                                    //rules:[{required:true, message:'续签合同'}]
                                    })(
                                        <TextArea placeholder="补充内容" autosize />
                                    )
                                    
                            }
                    </FormItem>
                </Col>
            </Row>
            {/*
            <Row>
                    <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button type="primary" htmlType="submit" loading={this.props.loadingState} style={{width: "8rem"}} onClick={this.handleSave}>保存</Button>
                        {basicOperType !== "add" ? <Button className="oprationBtn" onClick={this.handleCancel}>取消</Button> : null}
                    </Col>
            </Row>
            */}
            {
                [8, 1].includes(this.props.basicInfo.examineStatus)  ? null :
                <div>
                    <Row type="flex" justify="space-between">
                        <Col  span={24} style={{ textAlign: 'center' }} className='BtnTop'>

                            <Button type="primary" size='large' className="oprationBtn"
                                style={{ width: "10rem", display: this.props.contractDisplay }}
                                onClick={this.handleSave} loading={this.props.submitLoading}>提交</Button>
                        </Col>
                    </Row>
                </div>
            
            }
          </Form>
        
         </div>
        
                        

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
        basicInfo: state.contractData.contractInfo.baseInfo,
        operInfo:state.contractData.operInfo,
        activeOrg: state.search.activeOrg,
        contractChooseVisible: state.contractData.contractChooseVisible,
        complementInfo: state.contractData.contractInfo.complementInfo,
        curFollowContract: state.contractData.curFollowContract,
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
  
  export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(ComplementEdit));