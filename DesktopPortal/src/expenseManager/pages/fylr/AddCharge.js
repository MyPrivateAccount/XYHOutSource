import React, {Component} from 'react';
import {Select,Upload, Modal, Icon, Table, Form, Checkbox, Input,TreeSelect, Row, Col,Button, notification,Spin} from 'antd'
import {connect} from 'react-redux';
import FixedTable from '../../../components/FixedTable'
import {AuthorUrl, basicDataServiceUrl, UploadUrl} from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import {getDicPars, getOrganizationTree} from '../../../utils/utils'
import {getDicParList} from '../../../actions/actionCreators'
import validations from '../../../utils/validations'
import Layer from '../../../components/Layer'
import {chargeStatus} from './const'
import moment from 'moment'
import uuid from 'uuid';

const FormItem = Form.Item;
const Option = Select.Option;

const formFields = ['chargeNo', 'reimburseDepartment', 'reimburseUser', 'isBackup',  'memo', 'chargeAmount', 'payee', 'billAmount']

const feeValidationRules = {
    type: [[validations.isRequired,'必须选择费用类型']],
    amount: [[validations.isCurrency, '金额格式错误'],[validations.isGreaterThan, '金额必须大于0',0]]
}

const billValidationRules = {
    receiptNumber: [[validations.isRequired,'必须输入发票号码']],
    receiptMoney: [[validations.isCurrency, '金额格式错误'],[validations.isGreaterThan, '金额必须大于0',0]]

}

const styles={
    statusText:{
        position: 'absolute',
        right: '1rem',
        top: '1rem',
        fontSize:'1.2rem'
    }
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
        saveing:false,
        previewVisible:false,
        previewImage:'',
        loading:false,
        canEditBase:false,
        canEditFee:false,
        canEditBill:false
    }

    componentDidMount=()=>{
        let initState= (this.props.location||{}).state;
        if(initState.op==='add'){
            this.setState({entity: initState.entity, op: initState.op, canEditBase:true, canEditBill:true, canEditFee:true})
        }else if(initState.entity){
            let canEditBase=false, canEditBill=false, canEditFee=false;
            if( initState.op ==='edit'){
                canEditBase = canEditBill = canEditFee = true;
            }
            this.setState({canEditBase,canEditBill, canEditFee})
            this.setState({op: initState.op})
            this.getChargeInfo(initState.entity.id);
        }
        console.log(this.props.match);
        this.props.getDicParList(['CHARGE_COST_TYPE']);
        this.getNodes();
    }


    getChargeInfo = async (id)=>{
        if(!id){
            notification.error({message:'报销单id为空'})
            return;
        }
        
        this.setState({loading:true})
        let url = `${basicDataServiceUrl}/api/chargeinfo/${id}`;
        try{
            let r = await ApiClient.get(url);
            if(r  && r.data && r.data.code==='0'){
                if(!r.data.extension){
                    notification.warn({message:'警告',description:'报销单不存在'});
                }else{
                    //转换
                    let entity = r.data.extension;
                    if(entity.billList){
                        entity.billList.forEach(item=>{

                            let attachments = [];
                            if(item.fileScopes){
                                item.fileScopes.forEach(fs=>{
                                    if(fs.fileItem && fs.fileList && fs.fileList.length>0){
                                        let f = fs.fileList[0];
                                        f.url = fs.fileItem.original;
                                        f.uid = f.fileGuid;
                                        attachments.push(f);
                                    }
                                })
                            }
                            item.attachments = attachments;
                            item.errors={};
                        })
                    }
                    if(entity.feeList){
                        entity.feeList.forEach(item=>item.errors={})
                    }
                    await this.fetchUser(entity.reimburseUser);
                    entity.reimburseUser = [{key:entity.reimburseUser}]

                    this.setState({billList: entity.billList, feeList: entity.feeList, entity: entity},()=>{
                        var initValues = {};
                        formFields.forEach(k=>{
                            initValues[k] = entity[k]
                        })

                        this.props.form.setFieldsValue(initValues);
                    });
                }
            }
        }catch(e){
            notification.error({message:'异常',description: e.message})
        }
        this.setState({loading:false})
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
        
        entity.reimburseUser = entity.reimburseUser[0].key;
        
        entity.billList=[];
        bl.forEach(item=>{
            var bi = {...item};
            delete bi.attachments;
            delete bi.errors;
            bi.fileScopes = [];
            if(item.attachments){
                item.attachments.forEach(f=>{
                    bi.fileScopes.push({
                        reciptId: item.id,
                        fileGuid: f.fileGuid,
                        fileList:[
                            {...f}
                        ]
                    });
                });
            }
            entity.billList.push(bi);
        });


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

    uploadCallback=(record, file)=>{
        let bl = this.state.billList;
        let idx=  bl.findIndex(x=>x.id === record.id);
      
        if(!record.attachments){
            record.attachments = [];
        }
        let idx2 = record.attachments.findIndex(x=>x.uid === file.uid);
        if(idx2>=0){
            record.attachments[idx2] = file;
        }else{
            record.attachments.push(file);
        }
        
        bl[idx] = {...record, ...{attachments: [...record.attachments]}}
        this.setState({billList: [...bl]});
    }
    upload=(record, callback, file, fileList)=>{
        let id = this.state.entity.id;
        let uploadUrl = `${UploadUrl}/file/upload/${id}`;
        let fileGuid = uuid.v1();
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
                appId: 'ExpenseManagerIndex',
                localUrl: file.url,
                url: `${UploadUrl}${r.url}`,
                driver: r.deviceName,
                uri: r.path,
                type:'ORIGINAL',
                name: file.name,
                uid: file.uid,
                fileExt:r.ext
              }
              if (callback) {
                callback(record, uf);
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
    handleCancel = () => this.setState({ previewVisible: false })

    handlePreview = (file) => {
        this.setState({
          previewImage: file.url || file.thumbUrl,
          previewVisible: true,
        });
      }

    handleUploadChange=(record,{fileList})=>{
        if(!this.state.canEditBill){
            return;
        }
        let bl = this.state.billList;
        let idx=  bl.indexOf(record);
        
        record.attachments=fileList;
        bl[idx] = {...record, ...{attachments: [...record.attachments]}}
        this.setState({billList: [...bl]});
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
                        disabled={!this.state.canEditFee}
                        style={{width:'100%'}}
                        onChange={(e)=>{this.changeRowValue(record, 'type',e)}}>
                        {
                            groupList.map((item)=>{
                                return <Option key={item.value*1} value={item.value*1}>{item.key}</Option>
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
                <Input value={record.memo} disabled={!this.state.canEditFee} onChange={(e)=>this.changeRowValue(record, 'memo',e)}/>
                </FormItem>
            )
          }, {
            title: '金额',
            dataIndex: 'amount',
            key: 'amount',
            width:'10rem',
            render:(text,record)=>(
                <FormItem hasFeedback validateStatus={record.errors['amount']?'error':''}>
                    <Input disabled={!this.state.canEditFee} value={record.amount} onChange={(e)=>this.changeRowValue(record, 'amount',e)}
                        style={{textAlign:'right'}}/>
                </FormItem>
            )
          }, {
            title: '操作',
            key: 'action',
            width:'5rem',
            render: (text, record) => (
              <span>
                {this.state.canEditFee? <a href="javascript:;" onClick={()=>{this.delItem(record)}}>删除</a>:null}
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
                        <Input disabled={!this.state.canEditBill} value={record.receiptNumber} onChange={(e)=>this.changeBillRowValue(record, 'receiptNumber',e)}
                            />
                    </FormItem>
                )
            },
            {
                title: '摘要',
                key:'memo',
                render:(text,record)=>(
                    <FormItem>
                        <Input  disabled={!this.state.canEditBill} value={record.memo} onChange={(e)=>this.changeBillRowValue(record, 'memo',e)}/>
                    </FormItem>
                )
            },
            {
                title: '金额',
                key:'receiptMoney',
                width:'10rem',
                render:(text,record)=>(
                    <FormItem hasFeedback validateStatus={record.errors['receiptMoney']?'error':''}>
                        <Input disabled={!this.state.canEditBill} value={record.receiptMoney} onChange={(e)=>this.changeBillRowValue(record, 'receiptMoney',e)}
                            style={{textAlign:'right'}}/>
                    </FormItem>
                )
            },{
                title: '附件',
                key:'',
                render:(text,record)=>{
                    let fl = record.attachments||[]
                    return <Upload
                        listType="picture-card"
                        fileList={fl}
                        disabled={!this.state.canEditBill}
                        onPreview={this.handlePreview}
                        onChange={(...args)=>this.handleUploadChange(record, ...args)}
                        beforeUpload={(...args)=>this.upload(record,this.uploadCallback,...args)}
                    >{
                        this.state.canEditBill?<div>
                        <Icon type="plus" />
                        <div className="ant-upload-text">上传</div>
                      </div>:null
                    }
                      
                    </Upload>
                }
            }, {
                title: '操作',
                key: 'action',
                width:'5rem',
                render: (text, record) => (
                  <span>
                      {this.state.canEditBill?<a href="javascript:;"  onClick={()=>{this.delBill(record)}}>删除</a>:null}
                    
                  </span>
                ),
              }
        ]

        var values = this.props.form.getFieldsValue(["isBackup","reimburseUser"]);
        let ru = values["reimburseUser"];
        let canAddFeeItem = ru && ru.length>=1 ;
        let {fetchingUser, userList} = this.state;

        
        let canAddBillItem =  !(values["isBackup"]||false) ||
                this.state.op === 'backup';
        
        return <Layer showLoading={this.state.loading} className="content-page">
            <div>
                <div className="page-title">费用报销单<div style={styles.statusText}>{chargeStatus[this.state.entity.status]||''}</div></div>
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
                                    disabled={!this.state.canEditBase}
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
                                        disabled={!this.state.canEditBase}
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
                                    <Checkbox disabled={!this.state.canEditBase} />
                                    )}
                        </FormItem>
                    </Col>
                   
                    <Col span={18}>
                        <FormItem  label="说明">
                                {getFieldDecorator('memo', {rules:[...lenValidator]})(
                                    <Input  disabled={!this.state.canEditBase}/>
                                    )}
                        </FormItem>
                    </Col>
                </Row>

                
                <Row className="form-row">
                    <Col span={6}>
                        <FormItem label="报销总额">
                                {getFieldDecorator('chargeAmount', {})(
                                    <Input  style={{textAlign:'right'}} readOnly disabled />
                                    )}
                        </FormItem>
                    </Col>
                    <Col span={18}>
                        <FormItem  label="收款单位">
                                {getFieldDecorator('payee', {rules:[...lenValidator]})(
                                    <Input  disabled={!this.state.canEditBase}/>
                                    )}
                        </FormItem>
                    </Col>
                </Row>    
                
            </Form>
            </div>
            <div style={{marginTop:'1rem'}}>
                <Button disabled={!(this.state.canEditFee && canAddFeeItem)} onClick={this.addItem}>添加费用项</Button>
            </div>
            <div style={{marginTop:'0.5rem'}}>
            <Form>
                <Table columns={columns} dataSource={this.state.feeList}
                    bordered={true} pagination={false}/>
            </Form>
            </div>
            <Row className="form-row" style={{marginTop:'1rem'}}>
                <Col span={6}>                       
                    <Button disabled={!(this.state.canEditBill && canAddBillItem)} onClick={this.addBill}>添加发票</Button>
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
            {
                (this.state.canEditBase || this.state.canEditBill || this.state.canEditFee)?
                <Button size="large" loading={this.state.saveing} type="primary" onClick={this.submit}>提交</Button>:null
            }
                  
            </div>

            <Modal visible={this.state.previewVisible} footer={null} onCancel={this.handleCancel}>
                <img alt="example" style={{ width: '100%' }} src={this.state.previewImage} />
            </Modal>
        </Layer>
    }
}

const mapStateToProps = (state, props) => ({
    dic: state.basicData.dicList
})
const mapActionToProps = (dispatch) => ({
    getDicParList: (...args) => dispatch(getDicParList(...args))
})

export default connect(mapStateToProps, mapActionToProps)(Form.create()(AddCharge))