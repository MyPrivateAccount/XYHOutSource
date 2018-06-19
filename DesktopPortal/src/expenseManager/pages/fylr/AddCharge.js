import React, {Component} from 'react';
import {Select, Table, Form, Checkbox, Input,TreeSelect, Row, Col,Button, notification,Spin} from 'antd'
import {connect} from 'react-redux';
import FixedTable from '../../../components/FixedTable'
import {AuthorUrl, basicDataServiceUrl} from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import {getDicPars, getOrganizationTree} from '../../../utils/utils'
import {getDicParList} from '../../../actions/actionCreators'
import validations from '../../../utils/validations'
import moment from 'moment'
import uuid from 'uuid';

const FormItem = Form.Item;
const Option = Select.Option;

const feeValidationRules = {
    type: [[validations.isRequired,'必须选择费用类型']],
    amount: [[validations.isCurrency, '金额格式错误'],[validations.isGreaterThan, '金额必须大于0',0]]
}

const billValidationRules = {
    receiptNumber: [[validations.isRequired,'必须输入发票号码']],
    receiptMoney: [[validations.isCurrency, '金额格式错误'],[validations.isGreaterThan, '金额必须大于0',0]]

}

class AddCharge extends Component{
    state={
        nodes:[],
        feeList:[],
        billList:[],
        userList:[],
        fetching:false,
        entity:{},
        op:'',
        saveing:false
    }

    componentDidMount=()=>{
        let initState= (this.props.location||{}).state;
        if(initState.entity){
            this.setState({entity: initState.entity, op: initState.op})
        }
        console.log(this.props.match);
        this.props.getDicParList(['CHARGE_COST_TYPE']);
        this.getNodes();
    }

    changeRowValue = (record, key, e)=>{
        let v= e||'';
        if(v.target){
            v = v.target.value;
        }
        
        record[key] = v;
        if(!record.errors){
            record.errors={...record.errors};
        }
        if(feeValidationRules[key]){
            let e = validations.validateOne(v, feeValidationRules[key])
            record.errors[key] = e;
        }

        let fl = this.state.feeList;
        let idx = fl.indexOf(record);

        fl[idx] = {...record}
        this.setState({feeList:[...fl]})
        this.calc();
    }

    getNodes=async ()=>{
        let url = `${AuthorUrl}/api/Permission/FY_BXMD`;
        let r = await ApiClient.get(url, true);
        if(r && r.data && r.data.code==='0'){
            var nodes = getOrganizationTree(r.data.extension);
            this.setState({nodes: nodes});
        }else{
            notification.error(`获取报销门店失败:${((r||{}).data||{}).message||''}`);
        }
    }

    fetchUser = async (value)=>{
        let url = `${basicDataServiceUrl}/api/humaninfo/simpleSearch`;
        var r = await ApiClient.get(url, true,{permissionId:'FY_BXMD', keyword:value,pageSize:0, pageIndex:0});
        if(r && r.data && r.data.code==='0'){
            this.setState({userList: r.data.extension})
        }
    }
    selectedUser = (value) => {
        this.setState({
          data: [],
          fetching: false,
        });
        this.props.form.setFieldsValue({reimburseUser:[value[value.length-1]]})
      }
    addItem=()=>{
        let list = [...this.state.feeList];
        let item = {id:uuid.v1(),amount:0, memo:'', type:'',errors:{}};
        item.errors = validations.validate(item, feeValidationRules);

        list.push(item);
        this.setState({feeList: list})
    }

    delItem=(record)=>{
        let fl = this.state.feeList;
        let idx = fl.indexOf(record);
        if(idx>=0){
            fl.splice(idx,1)
            this.setState({feeList: [...fl]})
        }
    }

    changeBillRowValue = (record, key, e)=>{
        let v= e||'';
        if(v.target){
            v = v.target.value;
        }
        record[key] = v;
        if(!record.errors){
            record.errors={...record.errors};
        }
        if(billValidationRules[key]){
            let e = validations.validateOne(v, billValidationRules[key])
            record.errors[key] = e;
        }

        let fl = this.state.billList;
        let idx = fl.indexOf(record);

        fl[idx] = {...record}
        this.setState({billList:[...fl]})
    }

    addBill=()=>{
        let list = [...this.state.billList];
        var item = {id:uuid.v1(),receiptMoney:0, memo:'', receiptNumber:'', attachments:[], errors:[]};
        validations.validate(item, billValidationRules);
        list.push(item);
        this.setState({billList: list})
    }

    delBill=(record)=>{
        let fl = this.state.billList;
        let idx = fl.indexOf(record);
        if(idx>=0){
            fl.splice(idx,1)
            this.setState({billList: [...fl]})
        }
    }

    calc = ()=>{
        let chargeAmount = 0;
        let fl = this.state.feeList;
        fl.forEach(item=>{
            if(!(item.errors && item.errors.amount)){
                chargeAmount = chargeAmount + item.amount*1;
            }
        });
        let bl = this.state.billList;
        let billAmount = 0;
        bl.forEach(item=>{
            if(!(item.errors && item.errors.receiptMoney)){
                billAmount = billAmount + item.receiptMoney*1;
            }
        })
        this.props.form.setFieldsValue({chargeAmount: chargeAmount, billAmount: billAmount})
    }

    submit=async ()=>{
        //检查
        this.calc();
        this.props.form.validateFieldsAndScroll();
        var errors = this.props.form.getFieldsError();
        
        if(validations.checkErrors(errors)){
            notification.error({message:'验证失败', description:'表单验证失败，请检查'});
            return;
        }

        
        let values = this.props.form.getFieldsValue();
        let isBackup = values.isBackup;
        if((values.chargeAmount*1)===0){
            notification.error({message:'验证失败', description:'费用总额为0'});
            return;
        }
        if(this.state.op==='add' && isBackup && this.state.billList && this.state.billList.length>0){
            notification.error({message:'验证失败', description:'您选择了后补发票，请不要录入发票信息'})
            return;
        }

        //验证费用项列表
        let fl = this.state.feeList;
        for(let i = 0;i<fl.length;i++){
            let fitem = fl[i];
            if(fitem.errors && (fitem.errors.type || fitem.errors.amount)){
                notification.error({message:'验证失败', description:'费用列表验证未通过，请检查'});
                return;
            }
        }

        //发票列表检查
        let bl = this.state.billList;
        for(let i = 0; i< bl.length; i++){
            let bitem = bl[i];
            if(bitem.errors && (bitem.errors.receiptNumber || bitem.errors.receiptNumber)){
                notification.error({message:'验证失败', description:'发票列表验证未通过，请检查'});
                return;
            }
        }

        if(this.state.op==='add' && !isBackup && (values.billAmount*1)<(values.chargeAmount*1)){
            notification.error({message:'验证失败', description:'发票总金额小于报销总额'});
            return;
        }

        var entity = {...this.state.entity, ...values};
        entity.feeList = [...fl];
        entity.billList = [...bl];
        entity.reimburseUser = entity.reimburseUser[0].key;

        console.log(entity);
        this.setState({saveing:true})
        try{
        let url = `${basicDataServiceUrl}/api/chargeinfo`;
        let r = await ApiClient.post(url, entity);
        if(r && r.data && r.data.code==='0'){
            console.log(r.data.extension);
            let ritem = r.data.extension;
            this.props.form.setFieldsValue({chargeNo: ritem.chargeNo});
            this.setState({entity: {...entity,...{branchId: ritem.branchId,seq:ritem.seq,chargeNo: ritem.chargeNo,createTime: ritem.createTime }}})
            notification.success({message:"保存成功！"});
        }else{
            notification.error({message:'保存失败', description:`保存失败：${((r||{}).data||{}).message||''}`})
        }
        }catch(e){
            
        }
        this.setState({saveing:false})
    }

    render(){
        const { getFieldDecorator } = this.props.form;
        const lenValidator = [{ max: 120, message: '参数值长度不得大于120个字符' }]
        let groupList=  getDicPars('CHARGE_COST_TYPE', this.props.dic);

        const columns = [{
            title: '费用项目',
            dataIndex: 'type',
            key: 'type',
            width:'15rem',
            render:(text,record)=>(
                <FormItem hasFeedback validateStatus={record.errors['type']?'error':''}>
                    <Select defaultValue={record.type} 
                        style={{width:'100%'}}
                        onChange={(e)=>{this.changeRowValue(record, 'type',e)}}>
                        {
                            groupList.map((item)=>{
                                return <Option key={item.value} value={item.value}>{item.key}</Option>
                            })
                        }
                    </Select>
                </FormItem>
            )

            
          }, {
            title: '摘要',
            dataIndex: 'memo',
            key: 'memo',
            render:(text,record)=>(
                <FormItem>
                <Input value={record.memo} onChange={(e)=>this.changeRowValue(record, 'memo',e)}/>
                </FormItem>
            )
          }, {
            title: '金额',
            dataIndex: 'amount',
            key: 'amount',
            width:'10rem',
            render:(text,record)=>(
                <FormItem hasFeedback validateStatus={record.errors['amount']?'error':''}>
                    <Input value={record.amount} onChange={(e)=>this.changeRowValue(record, 'amount',e)}
                        style={{textAlign:'right'}}/>
                </FormItem>
            )
          }, {
            title: '操作',
            key: 'action',
            width:'5rem',
            render: (text, record) => (
              <span>
                <a href="javascript:;" onClick={()=>{this.delItem(record)}}>删除</a>
              </span>
            ),
          }];

        const billConlumns = [
            {
                title: '发票号',
                key:'receiptNumber',
                width: '20rem',
                render:(text,record)=>(
                    <FormItem hasFeedback validateStatus={record.errors['receiptNumber']?'error':''}>
                        <Input value={record.receiptNumber} onChange={(e)=>this.changeBillRowValue(record, 'receiptNumber',e)}
                            />
                    </FormItem>
                )
            },
            {
                title: '摘要',
                key:'memo',
                render:(text,record)=>(
                    <FormItem>
                        <Input value={record.memo} onChange={(e)=>this.changeBillRowValue(record, 'memo',e)}/>
                    </FormItem>
                )
            },
            {
                title: '金额',
                key:'receiptMoney',
                width:'10rem',
                render:(text,record)=>(
                    <FormItem hasFeedback validateStatus={record.errors['receiptMoney']?'error':''}>
                        <Input value={record.receiptMoney} onChange={(e)=>this.changeBillRowValue(record, 'receiptMoney',e)}
                            style={{textAlign:'right'}}/>
                    </FormItem>
                )
            },{
                title: '附件',
                key:''
            }, {
                title: '操作',
                key: 'action',
                width:'5rem',
                render: (text, record) => (
                  <span>
                    <a href="javascript:;" onClick={()=>{this.delBill(record)}}>删除</a>
                  </span>
                ),
              }
        ]

        var values = this.props.form.getFieldsValue(["isBackup","reimburseUser"]);
        let ru = values["reimburseUser"];
        let canAddFeeItem = ru && ru.length>=1 && this.state.op==='add';
        let {fetchingUser, userList} = this.state;

        
        let canAddBillItem = (this.state.op==='add' && !(values["isBackup"]||false)) ||
                this.state.op === 'backup';
        
        return <div className="content-page">
            <div>
                <div className="page-title">费用报销单</div>
                <div className="page-subtitle">{moment(this.state.entity.createTime).format('YYYY年MM月DD日')}</div>
            </div>
            <div>
            <Form ref={(e) => this.form = e}>
                <Row className="form-row">
                    <Col span={6}>
                        <FormItem label="单据号">
                                {getFieldDecorator('chargeNo', {})(
                                    <Input readOnly disabled />
                                    )}
                        </FormItem>
                    </Col>
                    <Col span={10}>
                        <FormItem hasFeedback label="报销门店">
                            {getFieldDecorator('reimburseDepartment', {
                                rules: [{ required: true, message: '必须选择报销门店' }],
                            })(
                                <TreeSelect
                                    style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.state.nodes}
                                    placeholder="请选择报销门店"
                                />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={8}>
                        <FormItem label="报销人">
                                {getFieldDecorator('reimburseUser', {rules:[{required:true, message:'必须选择报销人'}]})(
                                    <Select
                                        mode="multiple"
                                        maxTagCount={1}
                                        labelInValue
                                        placeholder="输入姓名、员工编号或手机号码"
                                        notFoundContent={fetchingUser ? <Spin size="small" /> : null}
                                        filterOption={false}
                                        onSearch={this.fetchUser}
                                        onChange={this.selectedUser}
                                        style={{ width: '100%' }}
                                    >
                                        {userList.map(d => <Option key={d.id}>{`${d.name}(${d.organizationFullName})`}</Option>)}
                                    </Select>
                                    )}
                        </FormItem>
                    </Col>

                </Row>
                <Row className="form-row">
                    <Col span={6}>
                        <FormItem label="后补发票">
                                {getFieldDecorator('isBackup', {})(
                                    <Checkbox  />
                                    )}
                        </FormItem>
                    </Col>
                   
                    <Col span={18}>
                        <FormItem  label="说明">
                                {getFieldDecorator('memo', {rules:[...lenValidator]})(
                                    <Input  />
                                    )}
                        </FormItem>
                    </Col>
                </Row>

                
                <Row className="form-row">
                    <Col span={6}>
                        <FormItem label="报销总额">
                                {getFieldDecorator('chargeAmount', {})(
                                    <Input style={{textAlign:'right'}} readOnly disabled />
                                    )}
                        </FormItem>
                    </Col>
                    <Col span={18}>
                        <FormItem  label="收款单位">
                                {getFieldDecorator('payee', {rules:[...lenValidator]})(
                                    <Input  />
                                    )}
                        </FormItem>
                    </Col>
                </Row>    
                
            </Form>
            </div>
            <div style={{marginTop:'1rem'}}>
                <Button disabled={!canAddFeeItem} onClick={this.addItem}>添加费用项</Button>
            </div>
            <div style={{marginTop:'0.5rem'}}>
            <Form>
                <Table columns={columns} dataSource={this.state.feeList}
                    bordered={true} pagination={false}/>
            </Form>
            </div>
            <Row className="form-row" style={{marginTop:'1rem'}}>
                <Col span={6}>                       
                    <Button disabled={!canAddBillItem} onClick={this.addBill}>添加发票</Button>
                </Col>
                <Col span={12}></Col>
                <Col span={6}>
                    <FormItem label="发票总金额">
                            {getFieldDecorator('billAmount', {})(
                                <Input style={{textAlign:'right'}} readOnly disabled />
                                )}
                    </FormItem>
                </Col>
            </Row>
            <div style={{marginTop:'0.5rem'}}>
            <Form>
                <Table columns={billConlumns} dataSource={this.state.billList}
                    bordered={true} pagination={false}/>
            </Form>               
            </div>
            <div style={{marginTop:'1rem', textAlign:'center'}}>
                  <Button size="large" loading={this.state.saveing} type="primary" onClick={this.submit}>提交</Button>
            </div>
        </div>
    }
}

const mapStateToProps = (state, props) => ({
    dic: state.basicData.dicList
})
const mapActionToProps = (dispatch) => ({
    getDicParList: (...args) => dispatch(getDicParList(...args))
})

export default connect(mapStateToProps, mapActionToProps)(Form.create()(AddCharge))