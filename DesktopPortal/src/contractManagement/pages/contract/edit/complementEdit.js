import { connect } from 'react-redux';
import { getDicParList, saveContractBasic, viewContractBasic,basicLoadingStart, openContractChoose,contractComplementSave } from '../../../actions/actionCreator';
import React, { Component } from 'react';
import { Icon, Input, InputNumber, Button, Checkbox, Row, Col, Form, DatePicker, Select, Cascader,Radio } from 'antd';
import moment from 'moment';
import { NewGuid } from '../../../../utils/appUtils';


const FormItem = Form.Item;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const { RangePicker } = DatePicker;
const { TextArea } = Input;

let uuid = 0;
class ComplementEdit extends Component {
    remove = (k) => {
        const { form } = this.props;
        // can use data-binding to get
        const keys = form.getFieldValue('keys');
        // We need at least one passenger
        if (keys.length === 1) {
           // return;
        }

        // can use data-binding to set
        form.setFieldsValue({
            keys: keys.filter(key => key !== k),
        });
    }

     add = () => {
        let uuid = NewGuid();
        const { form } = this.props;
        // can use data-binding to get
        const keys = form.getFieldValue('keys');
        const nextKeys = keys.concat(uuid);
        // can use data-binding to set
        // important! notify form to detect changes
        form.setFieldsValue({
            keys: nextKeys,
        });
     }
    
    handleSave = (e) => {
        e.preventDefault();
        let { basicOperType } = this.props.operInfo;
        let contractId = this.props.basicInfo.id;
        let method = this.props.complementOperType === "add" ? 'post' : 'put';
        this.props.form.validateFields((err, values) => {
            if (!err) {
                //console.log("handleSave:", values);
                let newComplementInfo = [];
                newComplementInfo = values.keys.map((item, i) =>{
                    let complementInfo = {};
                    console.log('item', item);
                    complementInfo.contentID = item;
                    complementInfo.contentInfo = values[item];
                    return complementInfo;
                })
                console.log("newComplementInfo:", newComplementInfo);
                //this.props.dispatch(contractComplementSave({entity: newComplementInfo, method: method, id:contractId}));
                //console.log("handleSave:", values);
                // this.setState({ loadingState: true });
                
            //     this.props.dispatch(basicLoadingStart())
            //     let StartTime = moment(values.startAndEndTime[0]).format("YYYY-MM-DD");
            //     let EndTime = moment(values.startAndEndTime[1]).format("YYYY-MM-DD");
            //     let newBasicInfo = Object.assign({},values, {startTime:StartTime, endTime:EndTime});
                
    
            //     newBasicInfo.id = this.props.contractInfo.baseInfo.id;

            //    // newBasicInfo.relation = this.state.departmentFullId;
            //     if(basicOperType === 'add')
            //     {
            //         newBasicInfo.createTime = moment().format("YYYY-MM-DD");
            //         //newBasicInfo.createDepartment = this.props.activeOrg.organizationName;
            //     }
 
            //     if (basicOperType != 'add') {
            //         newBasicInfo = Object.assign({}, this.props.basicInfo, values);
            //     }
            //     newBasicInfo.isSubmmitShop = 1;
            //     newBasicInfo.isSubmmitRelation = 1;
            //     newBasicInfo.createDepartment = this.props.activeOrg.organizationName = "";
            //     newBasicInfo.organizete = this.state.departMentFullName;
            //     newBasicInfo.createDepartmentID = this.state.createDepartmentID;
                
            //     delete newBasicInfo.startAndEndTime;
            //     let method = (basicOperType === 'add' ? 'POST' : "PUT");
    
            //     this.props.dispatch(saveContractBasic({ 
            //         method: method, 
            //         entity: newBasicInfo, 
        
            //     }));
            }
        });
    }

    render(){
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched,getFieldValue } = this.props.form;
        const formItemLayout = {
          labelCol: { span: 6 },
          wrapperCol: { span: 14 },
        };
        getFieldDecorator('keys', { initialValue: this.props.complementInfo || [] });
        const keys = getFieldValue('keys');
        const formItems = keys.map((k, index) => {
          return (
            <Row type="flex" style={{marginTop:"25px"}} key={k.contentId || k}>
                <Col span={12}>
                    <FormItem
                    {...formItemLayout}
                    label={`补充内容` + (index + 1)}
                    required={false}
                    key={k.contentId || k}
                    >
                    {getFieldDecorator(`${k.contentId || k}`, {
                        initialValue: k.contentInfo,
                        validateTrigger: ['onChange', 'onBlur'],
                        rules: [{
                        required: true,
                        whitespace: true,
                        message: "请输入补充内容或者删除",
                        }],
                    })(
                        <Input type="textarea" style={{ height: '100px' }} placeholder="请输入补充内容!" />
                    )}
                    <Icon
                        className="dynamic-delete-button"
                        type="minus-circle-o"
                        //disabled={keys.length === 1}
                        onClick={() => this.remove(k)}
                    />
                    </FormItem>
                </Col>
            </Row>
          );
        });
        return (
          <div>
          <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px"}}>
          <Icon type="tags-o" className='content-icon'/> <span className='content-title'>补充协议</span>
            
            <Row type="flex" style={{marginTop:"25px"}}>
                    <Col span={24}>
                        <FormItem {...formItemLayout}>
                            <Button type="dashed" onClick={this.add} style={{ width: '20%', marginLeft: 30 }}>
                                <Icon type="plus" /> 添加补充内容
                            </Button>
                        </FormItem>
                    </Col>
            </Row>
            {formItems}

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

                            <Button type="primary" size='default' className="oprationBtn"
                                style={{ width: "10rem", display: this.props.contractDisplay }}
                                disabled = {keys.length === 0}
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
        complementOperType: state.contractData.operInfo.complementOperType,
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