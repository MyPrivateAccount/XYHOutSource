import { connect } from 'react-redux';
import { getDicParList, saveContractBasic, viewContractBasic,basicLoadingStart, openContractChoose,contractComplementSave } from '../../../actions/actionCreator';
import React, { Component } from 'react';
import { Icon, Input, Modal, Button, Checkbox, Row, Col, Form, DatePicker, Select,Radio } from 'antd';
import moment from 'moment';
import { NewGuid } from '../../../../utils/appUtils';


const FormItem = Form.Item;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const { RangePicker } = DatePicker;
const { TextArea } = Input;
const confirm = Modal.confirm;
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
            keys: keys.filter(key => key.id !== k.id),
        });
    }

     add = () => {
        let uuid = NewGuid();
        const { form } = this.props;
        // can use data-binding to get
 
        const keys = form.getFieldValue('keys');
        let info = {};
        info.id = uuid;
        info.contentID = keys.length;
        info.contentInfo = "";
        const nextKeys = keys.concat(info);
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
               // console.log("handleSave:", values);
                let newComplementInfo = [];
                newComplementInfo = values.keys.map((item, i) =>{
                    let complementInfo = {};
                    console.log('item', item);
                    complementInfo.id = item.id;
                    complementInfo.contentID = i;
                    complementInfo.contentInfo = values[item.id];
                    return complementInfo;
                })
                //console.log("newComplementInfo:", newComplementInfo);
                this.props.dispatch(contractComplementSave({entity: newComplementInfo, method: method, id:contractId}));

            }
        });
    }
    handleDelete = (k)=>{
        let that = this;
        let isExist = false;
        if(this.props.complementOperType !== 'add')
        {
            for(let i =0; i <this.props.complementInfo; i++ )
            {
                let item = this.props.complementInfo[i];
                if(k.id === item.id)
                {
                    isExist = true;
                    break;
                }
            }
        }
        if(this.props.complementOperType === 'add' || !isExist){
            that.remove(k);
            return;
        }

        confirm({
            title: '确定真的删除吗？',
            content: '删除不可恢复!',
            okText: '确定',
            cancelText: '取消',
            onOk() {
                that.remove(k);
            },
            onCancel() {
              
            },
          });
    }
    render(){
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched,getFieldValue } = this.props.form;
        const formItemLayout = {
          labelCol: { span: 6 },
          wrapperCol: { span: 14 },
        };
        getFieldDecorator('keys', { initialValue: this.props.complementInfo || [] });
        console.log("this.props.complementInfo:", this.props.complementInfo);
      
        const keys = getFieldValue('keys');
        console.log("keys:", keys);
        const formItems = keys.map((k, index) => {
          return (
            <Row type="flex" style={{marginTop:"25px"}} key={k.id || k}>
                <Col span={24}>
                    <FormItem
                    {...formItemLayout}
                    label={`补充内容` + (index + 1)}
                    required={false}
                    key={k.id || k}
                    >
                    {getFieldDecorator(`${k.id || k}`, {
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
                        onClick={() => this.handleDelete(k)}
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
                            <Button type="dashed" onClick={this.add} style={{ width: '30%', marginLeft: 30 }}>
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
                //[8, 1].includes(this.props.basicInfo.examineStatus)  ? null :
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
        complementInfo: state.contractData.contractInfo.complementInfos.complementInfo,
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