import { connect } from 'react-redux';
import { searchStart, saveContractBasic, viewContractBasic,basicLoadingStart, openContractChoose, companyListGet } from '../../../actions/actionCreator';
import React, { Component } from 'react';
import { Icon, Input, InputNumber, Button, Checkbox, Row, Col, Form, DatePicker, Select, Cascader,Radio, notification } from 'antd';
import moment from 'moment';
import { call } from 'redux-saga/effects';
import ContractChoose from '../../dialog/contractChoose';
import CompanyAChoose from '../../dialog/companyAChoose';
import SearchCondition from '../../../constants/searchCondition';
const FormItem = Form.Item;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const { RangePicker } = DatePicker;
const { TextArea } = Input;
class BasicEdit extends Component {
    state = {
        organizateID: '',
        departMentFullName:'',
        departmentFullId:'',
        curSetCompanyA: null,
        curSetContract: null,
        isFollow: false,
    }
    handleCancel = () => {
        this.props.dispatch(viewContractBasic())
    }
    handleChangeTime = (value, dateString)=>{
        //console.log('curstatrEndTime:', value);
       // console.log('format curstatrEndTime:', dateString);
    }
    handleRenewClick = (e)=>{
        console.log("handleRenewClick");
        let curFollowContract = this.props.curFollowContract || {};
        let contractName = curFollowContract.Name ? curFollowContract.Name : '';
        this.props.dispatch(openContractChoose({contractName: contractName}));
    }
    
    componentWillMount(){
        if(this.props.basicInfo.isFollow)
        {
            this.setState({isFollow: this.props.basicInfo.isFollow});
        }
        
    }
    getUserOrgName = () =>{

    }
    handleIsFollow = (e) => {
        console.log('handleIsFollow:', e);
        this.setState({isFollow: e.target.value});
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
                newBasicInfo.createDepartment = this.props.activeOrg.organizationName || "";
                newBasicInfo.organizate = this.props.basicInfo.organizate;
                if(this.state.departMentFullName !='' || this.state.organizateID !='' || this.state.departmentFullId)
                {
                    //newBasicInfo.organizate = this.state.departMentFullName ;
                    newBasicInfo.organizateID = this.state.organizateID ;
                    //newBasicInfo.ext1 = this.state.departmentFullId;
                }
      
                if(this.state.curSetCompanyA)
                {
      
                    newBasicInfo.companyAT = this.state.curSetCompanyA.type;
                    newBasicInfo.companyAId = this.state.curSetCompanyA.id;
                }
                console.log('this.state.curSetContract:', this.state.curSetContract);
                if(this.state.curSetContract)
                {
                    newBasicInfo.follow = this.state.curSetContract.name;
                    newBasicInfo.followId = this.state.curSetContract.id;
                    
                }else if(newBasicInfo.isFollow && this.state.curSetContract === null)
                {
                    notification.warning({
                        message: '请选择续签合同!',
                        duration: 3
                    })
                    return;
                }
                delete newBasicInfo.startAndEndTime;
                delete newBasicInfo.examineStatus;
                let method = (basicOperType === 'add' ? 'POST' : "PUT");
    
                this.props.dispatch(saveContractBasic({ 
                    method: method, 
                    entity: newBasicInfo, 
        
                }));
                this.setState({curSetContract: {}, curSetCompanyA: {}});
            }
        });
    }
    handleChectDuringTime = (rule, value, callback) =>{
        const form = this.props.form;
        const duringTime = form.getFieldValue('startAndEndTime') || [];
        if(duringTime[0] === null || duringTime[1] === null){
            callback("请选择开始和结束时间");
        }
        else if(moment(duringTime[0]).isSameOrAfter(duringTime[1])){

            callback("结束时间不能小于开始时间");
        }
        else
        {
            callback();
        }
    }
    handleChooseDepartmentChange =  (v, selectedOptions) => {
        let organizateID = (v ||v != []) ? v[v.length -1].toString() : 0;
        let departmentFullId = v? v.join('*'): '';

        let text = selectedOptions.map(item => {
            return item.label
        })
     
        this.setState({departMentFullName: text.join('-'), organizateID: organizateID, departmentFullId:departmentFullId})
    }
    handleChooseCompanyA = () =>{
        let condition = {
            
            pageIndex: 0,
            pageSize: 10,
            keyWord: '',
            searchType:'',
            address:'',
            type:'dialog',
        
        };

        //console.log("handleChooseCompanyA:", condition);
        this.props.dispatch(companyListGet(condition));
    }

    handleChooseContract = () =>{
        let condition = {
            
            keyWord: '',
            orderRule: 0,
            pageIndex: 0,
            pageSize: 5,
            type:'dialog',
        
        };

        //console.log("handleChooseCompanyA:", condition);
        this.props.dispatch(searchStart(condition));
    }
    displayRender = (label) => {
        return label[label.length - 1];
    }
    handleChooseCompanyACallback = (info) => {
        console.log('handleChooseCompanyACallback:', info);
        if(info !== null)
        {
            this.setState({curSetCompanyA: info});
            this.props.form.setFieldsValue({
                companyA: info.name,
              });
        }
    }

    handleChooseContractCallback = (info) => {
        console.log('handleChooseContractCallback:', info);
        if(info !== null)
        {
            this.setState({curSetContract: info});
            this.props.form.setFieldsValue({
                follow: info.name,
              });
        }
    }
    render(){

        //console.log("this.props.contractChooseVisible:", this.props.contractChooseVisible);
        //console.log('this.props.curFollowContract:', this.props.curFollowContract);
        let curFollowContract = this.props.curFollowContract || {};
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        //let contractTypes = '1';
        let basicOperType = this.props.basicOperType;
        let basicInfo = this.props.basicInfo;
        const departMentInit = [];
        if(basicInfo.organizateID)
        {
            departMentInit.push(basicInfo.organizateID);
        }
        console.log('departMentInit:', departMentInit);
        const formItemLayout = {
          labelCol: { span: 6 },
          wrapperCol: { span: 14 },
        };
        console.log('this.props.basicData.orgInfo.orgList:', this.props.basicData.orgInfo.orgList);
        return (
          <div>
          <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px"}}>
          <Icon type="tags-o" className='content-icon'/> <span className='content-title'>基本信息 (必填)</span>
            <Row type="flex" style={{marginTop: "25px"}}>

            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>合同类型</span>}>
                        {
                            getFieldDecorator('type', {
                                    initialValue: basicInfo.type,
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
                        {getFieldDecorator('name', {
                        initialValue: basicInfo.name,
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
                    {/* <FormItem {...formItemLayout} label={<span>甲方类型</span>}>
                    {getFieldDecorator('companyAT', {
                                    initialValue: basicInfo.companyAT ? basicInfo.companyAT.toString() : "",
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
                    </FormItem> */}
                    <FormItem {...formItemLayout} label={<span>申请人</span>}>
                        {getFieldDecorator('ext1', {
                        initialValue: basicInfo.projectName,
                        rules:[{required:true, message:'请输入申请人!'}]
                        })(
                            <Input placeholder="申请人" />
                        )
                        
                        }
                    </FormItem>
                </Col>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>甲方公司</span>}>
                    {getFieldDecorator('companyA', {
                            initialValue: basicInfo.companyA,
                            rules:[{required:true, message:'请选择甲方公司全称!'}]
                            })(
                                <Input placeholder="甲方公司"  onClick={this.handleChooseCompanyA}/>
                                
                            )
                            
                    }
                    </FormItem>
                </Col>


            </Row>
            <Row type="flex" style={{marginTop: "25px"}}>
                <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>甲方负责人</span>}>
                        {getFieldDecorator('principalpepoleA', {
                                    initialValue: basicInfo.principalpepoleA,
                                    rules:[{required:true, message:'请输入甲方负责人!'}]
                                    })(
                                        <Input placeholder="甲方负责人" />
                                    )
                                    
                            }
                        </FormItem>
                    </Col>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>乙方负责人</span>}>
                    {getFieldDecorator('principalpepoleB', {
                                initialValue: basicInfo.principalpepoleB,
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
                    {getFieldDecorator('proprincipalPepole', {
                                    initialValue: basicInfo.proprincipalPepole,
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
                        <FormItem {...formItemLayout} label={<span>结算方式</span>}>
                        {getFieldDecorator('settleaccounts', {
                                initialValue: basicInfo.settleaccounts,
                                rules:[{required:true, message:'请选择结算方式!'}]
                                })(
                                    <Select>
                                    {
                                        this.props.basicData.settleAccountsCatogories.map((item) =>
                                            <Option key={item.value}>{item.key}</Option>
                                        )
                                    }
                                    </Select>
                                )
                                
                        }
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>项目地址</span>}>
                            {getFieldDecorator('projectAddress', {
                                        initialValue: basicInfo.projectAddress,
                                        //rules:[{required:true, message:'续签合同'}]
                                        })(
                                            <Input placeholder="项目地址"  />
                                        )
                                        
                                }
                        </FormItem>
                    </Col>


            </Row>

            <Row type="flex" style={{marginTop: "25px"}}>
              <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>份数</span>}>
                  {getFieldDecorator('count', {
                                  initialValue: basicInfo.count,
                                  rules:[{required:true, message:'请输入份数!'}]
                                  })(
                                      <InputNumber min={1}  />
                                  )
                                  
                     }
                 </FormItem>
              </Col>
              <Col span={12}>
                     <FormItem {...formItemLayout} label={<span>佣金方式</span>}>
                        {getFieldDecorator('commisionType', {
                                    initialValue: basicInfo.commisionType,
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
                  {getFieldDecorator('returnOrigin', {
                                  initialValue: basicInfo.returnOrigin !== null ? (basicInfo.returnOrigin === 1 ? 1 :2) : null ,
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
                        <FormItem {...formItemLayout} label={<span>申请部门</span>}>
                        {getFieldDecorator('organizate', {
                                        initialValue: basicInfo.organizateFullId ? basicInfo.organizateFullId.split('*') : [],//departMentInit,//basicInfo.relation ? basicInfo.relation.split('*') : [],//["1", "1385f04d-3ac8-49c6-a310-fe759814a685", "120"],//basicInfo.organizete ? basicInfo.organizete.split('-') : [],
                                        rules:[{required:true, message:'请选择申请部门!'}]
                                        })(
                                            <Cascader options={this.props.setContractOrgTree}  onChange={this.handleChooseDepartmentChange } changeOnSelect  placeholder="归属部门"/>
                                        )
                                        
                        }
                        </FormItem>
                    </Col>
            </Row>
            <Row type="flex" style={{marginTop:"25px"}}>
                {/* {
                    basicOperType === 'edit' ? 
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>是否作废</span>}>
                        {getFieldDecorator('isCancel', {
                                        initialValue: basicInfo.discard == null ? (basicInfo.discard === 1 ? '是' :'否') : null ,
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
                } */}
             <Col span={12}>
                 <FormItem {...formItemLayout} label={<span>是否续签</span>}>
                  {getFieldDecorator('isFollow', {
                                  initialValue: basicInfo.isFollow,//basicInfo.isFollow !== null ? (basicInfo.isFollow  ? 1 :2) : null ,
                                 // rules:[{required:true, message:'请选择是否返还原件!'}]
                                  })(
                                    <RadioGroup onChange={(e) =>this.handleIsFollow(e)}>
                                        <Radio value={true}>是</Radio>
                                        <Radio value={false}>否</Radio>
                                    </RadioGroup>
                                  )
                                  
                  }
                 </FormItem>
              </Col>
              {

                this.state.isFollow ?
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>续签合同</span>}>
                        {getFieldDecorator('follow', {
                                    initialValue:  basicInfo.follow,
                                    //rules:[{required:true, message:'续签合同'}]
                                    })(
                                        //<span>{'无'}</span>
                                        <Input  onClick={this.handleChooseContract}></Input>
                                    )
                                    
                            }
                    </FormItem>
       
                </Col>
                :null
              }
  
            </Row>
            <Row type="flex" style={{marginTop:"25px"}}>
             <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>佣金方案</span>}>
                    {getFieldDecorator('commission', {
                                initialValue: basicInfo.commission,
                                //rules:[{required:true, message:'请输入佣金方案!'}]
                                })(
                                    <Input type="textarea"  autosize={{ minRows: 4, maxRows: 6 }}  placeholder="佣金方案" />
                                )
                                
                        }
                    </FormItem>
                </Col>
 
   

            </Row>
            <Row type="flex" style={{marginTop:"25px"}}>

                <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>备注</span>}>
                            {getFieldDecorator('remark', {
                                        initialValue: basicInfo.remark,
                                        //rules:[{required:true, message:'续签合同'}]
                                        })(
                                            <Input type="textarea"  autosize={{ minRows: 4, maxRows: 6 }} placeholder="备注"  />
                                        )
                                        
                                }
                        </FormItem>
                    </Col>
   

            </Row>
            {/* <Row type="flex" style={{marginTop:"25px"}}>
                <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>补充协议</span>}>
                        {getFieldDecorator('contentInfo', {
                                    initialValue: this.props.complementInfo.contentInfo,
                                    //rules:[{required:true, message:'续签合同'}]
                                    })(
                                        <TextArea placeholder="补充协议" autosize />
                                    )
                                    
                            }
                    </FormItem>
                </Col>
            </Row> */}
            {/*
            <Row>
                    <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button type="primary" htmlType="submit" loading={this.props.loadingState} style={{width: "8rem"}} onClick={this.handleSave}>保存</Button>
                        {basicOperType !== "add" ? <Button className="oprationBtn" onClick={this.handleCancel}>取消</Button> : null}
                    </Col>
            </Row>
            */}
            {
                [1].includes(this.props.basicInfo.examineStatus)  ? null :
                <div>
                    <Row type="flex" justify="space-between">
                        <Col  span={24} style={{ textAlign: 'center' }} className='BtnTop'>

                            <Button type="primary" size='default' className="oprationBtn"
                                style={{ width: "10rem", display: this.props.contractDisplay }}
                                onClick={this.handleSave} loading={this.props.submitLoading}>提交</Button>
                        </Col>
                    </Row>
                </div>
            
            }
          </Form>
          <ContractChoose contractDialogCallback={this.handleChooseContractCallback}/> 
          <CompanyAChoose companyADialogCallback={this.handleChooseCompanyACallback}  />
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
        //setContractOrgTree: state.basicData.permissionOrgTree.setContractOrgTree,
        setContractOrgTree:state.basicData.permissionOrgTree.searchOrgTree,
        isShowCompanyADialog: state.companyAData.isShowCompanyADialog,
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