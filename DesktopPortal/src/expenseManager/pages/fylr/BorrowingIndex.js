import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Modal, Button,Radio, Select, Table, Row, Col, Form, Input, Checkbox, TreeSelect, DatePicker, notification} from 'antd'
import {AuthorUrl, basicDataServiceUrl} from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import {getDicPars, getOrganizationTree} from '../../../utils/utils'
import {getDicParList} from '../../../actions/actionCreators'
import Layer, { LayerRouter } from '../../../components/Layer'
import {Route } from 'react-router'
import BorrowingInfo from './BorrowingInfo'
import AddCharge from './AddCharge'
import Repayment from './Repayment'
import uuid from 'uuid'
import moment from 'moment'
import {chargeStatus, recordingStatus, permission, billType, borrowingStatus} from './const'

const FormItem = Form.Item;
const ButtonGroup = Button.Group;
const RadioGroup = Radio.Group;
const confirm = Modal.confirm;
const Option = Select.Option;

class BorrowingIndex extends Component{

    state={
        nodes:[],
        pagination:{pageSize:10, pageIndex: 1},
        list:[],
        permission:{},
        loading:false,
        pagePar:{},
        statusList:[]
    }

    componentDidMount=()=>{
        let initState= (this.props.location||{}).state ||{};
        this.setState({pagePar: initState})
        let sl = [];
        if(initState.status ){
            if(initState.status.length>=1){
                
                let all = [];
                initState.status.forEach(item=>{
                    sl.push({key: chargeStatus[item], value: item})
                    all.push(item);
                })
                if(initState.status.length>1){
                    sl.splice(0,0, {key:'所有', value: null});
                }
            }
        }
        if(sl.length===0){
            sl.push({key: '所有', value: null})
            sl.push({key: chargeStatus[borrowingStatus.UnSubmit], value: borrowingStatus.UnSubmit})
            sl.push({key: chargeStatus[borrowingStatus.Reject], value: borrowingStatus.Reject})
            sl.push({key: chargeStatus[borrowingStatus.Submit], value: borrowingStatus.Submit})
            sl.push({key: chargeStatus[borrowingStatus.Confirm], value: borrowingStatus.Confirm})
        }
        this.setState({statusList: sl});

        this.props.getDicParList(['CHARGE_COST_TYPE']);
        this.getNodes();

        this.getPermission();

        
        this.props.form.setFieldsValue({
            status:sl[0].value, 
            isReimbursed:null, 
            isBackup:null, 
            isPayment: (typeof initState.isPayment==='boolean') ?initState.isPayment: null
        })
        
    }

    getPermission = async ()=>{
        let url = `${AuthorUrl}/api/Permission/each`
        let r = await ApiClient.post(url, [permission.yjk, permission.yjkgl, permission.yjkqr, permission.yjkfk])
        if( r && r.data && r.data.code==='0'){
            let p = {};
            (r.data.extension||[]).forEach(pi=>{
                if(pi.permissionItem===permission.yjkqr){
                    p.qr = pi.isHave;
                }else if(pi.permissionItem===permission.yjkfk){
                    p.fk = pi.isHave;
                }else if(pi.permissionItem===permission.yjkgl){
                    p.gl = pi.isHave;
                }else if(pi.permissionItem===permission.yjk){
                    p.yjk = pi.isHave;
                }
            })
            this.setState({permission: p})
        }

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

    gotoDetail = (item, op)=>{
        this.props.history.push(`${this.props.match.url}/borrowingInfo`, {entity: item, op: op||'view', pagePar: this.state.pagePar})
    }
    gotoRepaymentDetail = (item, op)=>{
        this.props.history.push(`${this.props.match.url}/repayment`, {entity: item, op: op||'view', pagePar: this.state.pagePar})
    }
    clickSearch=()=>{
        this.setState({
            pagination:{...this.state.pagination,...{pageIndex:1}}
        },()=>{
            this.search();
        })
    }

    search = async ()=>{
        let condition = this.props.form.getFieldsValue();
        console.log(condition);
        condition = {...condition};
        if(condition.status!=null){
            condition.status = [condition.status];
        }else{
            condition.status = [];
            this.state.statusList.forEach(x=>{
                if(x.value!=null){
                    condition.status.push(x.value);
                }
            })
        }
       

        if(this.state.statusList.length===1){
            condition.status = [this.state.statusList[0].value]
        }

        condition.pageSize = this.state.pagination.pageSize;
        condition.pageIndex = this.state.pagination.pageIndex;
        this.setState({loading:true})
        try{
            let url = `${basicDataServiceUrl}/api/borrowing/search`;
            let r = await ApiClient.post(url, condition);
            if(r && r.data && r.data.code==='0'){
                this.setState({list: r.data.extension});
            }else{
                notification.error({message:'查询预借款列表失败'});
            }
        }catch(e){
            notification.error({message:'查询预借款列表失败'})
        }
        this.setState({loading:false})
    }

    handleTableChange = (pagination, filters, sorter) => {
        this.setState({
            pagination:{...this.state.pagination,...{pageIndex:pagination.current}}
        },()=>{
            this.search();
        })
    }

    addBorrowing = ()=>{
        let newFee = {
            id: uuid.v1(),
            type:2,
            createTime: new Date(),
            reimburseDepartment: this.props.user.Organization,
            reimburseUser: this.props.user.sub,
            reimburseUserName: this.props.user.nickname
        }
      
        this.props.history.push(`${this.props.match.url}/borrowingInfo`, {entity: newFee, op: 'add', pagePar: this.state.pagePar})
    }

    deleteBorrowing = (record)=>{
        let tn = billType[record.type];
        //删除
        confirm({
            title: `您确定要删除${tn}[${record.chargeNo}]吗?`,
            content: '',
            onOk: async ()=> {
              let url = `${basicDataServiceUrl}/api/borrowing/${record.id}`
              let r=  await ApiClient.post(url, null, null, 'DELETE');
              if(r && r.data && r.data.code==='0'){
                  notification.success({message:'删除成功', description:'单据已被删除'})
                  let l = this.state.list;
                  let idx = l.findIndex(x=>x.id === record.id);
                  if(idx>=0){
                      l.splice(idx,1);
                      this.setState({list: [...l]})
                  }
              }else{
                  notification.error({message:`删除失败${((r||{}).data||{}).message||''}`})
              }
            },
            onCancel() {
             
            },
          });
    }

    submitBorrowing=(record)=>{
        let tn = billType[record.type];
        confirm({
            title: `您确定要提交${tn}[${record.chargeNo}]吗?`,
            content: '提交后不可再进行修改',
            onOk: async ()=> {
              let url = `${basicDataServiceUrl}/api/borrowing/submit/${record.id}`
              let r=  await ApiClient.post(url, null, null, 'POST');
              if(r && r.data && r.data.code==='0'){
                  notification.success({message:'提交成功', description:'单据已提交'})
                  let l = this.state.list;
                  let idx = l.findIndex(x=>x.id === record.id);
                  if(idx>=0){
                      l[idx] = {...record,...{status: chargeStatus.Submit}}
                      this.setState({list: [...l]})
                  }
              }else{
                if(r.data.code==="410"){
                    Modal.error({
                        title: '费用超限',
                        content: r.data.message||'',
                      });
                    return;
                }
                  notification.error({message:`提交失败${((r||{}).data||{}).message||''}`})
              }
            },
            onCancel() {
             
            },
          });
    }

    addReimburse = (record)=>{
        let newFee = {
            id: uuid.v1(),
            type:1,
            createTime: new Date(),
            reimburseDepartment: record.reimburseDepartment,
            reimburseUser: record.reimburseUser,
            reimburseUserName: record.reimburseUserInfo.userName,
            chargeId: record.id
        }
      
        this.props.history.push(`${this.props.match.url}/chargeInfo`, {entity: newFee, op: 'add', pagePar: this.state.pagePar})
    }

    addRepayment=(record)=>{
        let newFee = {
            id: uuid.v1(),
            type:3,
            createTime: new Date(),
            reimburseDepartment: record.reimburseDepartment,
            reimburseUser: record.reimburseUser,
            reimburseUserName: record.reimburseUserInfo.userName,
            chargeId: record.id
        }
      
        this.props.history.push(`${this.props.match.url}/repayment`, {entity: newFee, op: 'add', pagePar: this.state.pagePar})
    }

    changeCallback = (entity)=>{
        let l = this.state.list;
        let idx=  l.findIndex(x=>x.id === entity.id);
        if(idx>=0){
            l[idx] = {...entity,...{
                status: entity.status, 
                isPayment: entity.isPayment,
                paymentAmount: entity.paymentAmount,
                recordingStatus: entity.recordingStatus,
            }}
            this.setState({list: [...l]})
        }
    }

    render(){
        const { getFieldDecorator } = this.props.form;

        const columns = [
            {
                title: '类型',
                dataIndex: 'type',
                key: 'type',
                width:'5rem',
                render: (text, record)=>{
                    return billType[record.type];
                }
            },
            {
                title: '单据号',
                dataIndex: 'chargeNo',
                key: 'chargeNo',
                width:'10rem',
                render: (text, record)=>{

                    return  record.type===2? <a href="javascript:void();" title="点击查看详情" onClick={()=>this.gotoDetail(record)}>{text}</a>
                        : <a href="javascript:void();" title="点击查看详情" onClick={()=>this.gotoRepaymentDetail(record)}>{text}</a>
                }
            },
            {
                title: '申请部门',
                dataIndex: 'reimburseDepartmentName',
                key: 'reimburseDepartmentName'
            },
            {
                title: '申请人',
                dataIndex: 'reimburseUserInfo',
                key: 'reimburseUserInfo',
                width:'6rem',
                render:(text,record)=>{
                    return (record.reimburseUserInfo||{}).userName
                }
            },
            {
                title: '申请日期',
                dataIndex: 'createTime',
                key: 'createTime',
                width:'8rem',
                render:(text,record)=>{
                    return moment(record.createTime).format('YYYY-MM-DD');
                }
            },
            {
                title: '填写人',
                dataIndex: 'createUserInfo',
                key: 'createUserInfo',
                width:'6rem',
                render:(text,record)=>{
                    return (record.createUserInfo||{}).userName
                }
            },
            {
                title: '申请金额',
                dataIndex: 'chargeAmount',
                key: 'chargeAmount',
                className: 'column-money',
                width:'8rem',
                render:(text,record)=>{
                    return  record.type===2? <span style={{textAlign:'right'}}>{record.chargeAmount}</span>:'-'
                }
            },
            {
                title: '状态',
                dataIndex: 'status',
                key: 'status',
                width:'5rem',
                render:(text,record)=>{
                    return chargeStatus[record.status]||''
                }
            },           
            {
                title: '付款/还款',
                dataIndex: 'isPayment',
                key: 'isPayment',
                width:'4rem',
                render:(text,record)=>{
                    if(record.type===3){
                        return recordingStatus[record.recordingStatus]
                    }
                    return record.isPayment?'已付':'未付'
                }
            },
            {
                title: '还款',
                dataIndex: 'isReimbursed',
                key: 'isReimbursed',
                width:'4rem',
                render:(text,record)=>{
                    return record.isReimbursed?'已还款':'未还款'
                }
            },
            {
                title: '已还款(元)',
                dataIndex: 'reimbursedAmount',
                key: 'reimbursedAmount',
                className: 'column-money',
                width:'8rem',
                render:(text,record)=>{
                    return <span style={{textAlign:'right'}}>{record.reimbursedAmount}</span>
                }
            },
            {
                title: '未还款(元）',
                dataIndex: 'reimbursedAmountYe',
                key: 'reimbursedAmountYe',
                className: 'column-money',
                width:'8rem',
                render:(text,record)=>{
                    return record.type===2? <span style={{textAlign:'right'}}>{record.chargeAmount - record.reimbursedAmount}</span>:'-'
                }
            },
            {
                title: '操作',
                width:'15rem',
                render: (text,record)=>{
                 let btns = [];
                 let s = record.status;
                 let {pagePar, permission} = this.state;
                 if(record.type===2){
                     //预借款
                     if( (s === borrowingStatus.UnSubmit || s === borrowingStatus.Reject) &&
                        !pagePar.noGL && ( permission.gl || record.reimburseUser === this.props.user.sub )){
                            btns.push(<Button onClick={()=>this.deleteBorrowing(record)}>作废</Button>)
                            btns.push(<Button onClick={()=>this.gotoDetail(record, 'edit')}>修改</Button>)
                            btns.push(<Button onClick={()=>this.submitBorrowing(record)}>提交</Button>)
                        } 
                        if(s === borrowingStatus.Submit && (!pagePar.noQR && permission.qr) ){
                            btns.push(<Button onClick={()=>this.gotoDetail(record, 'confirm')}>确认</Button>)
                        }
                        if(s === borrowingStatus.Confirm && !record.isPayment && (!pagePar.noFK && permission.fk) ){
                            btns.push(<Button onClick={()=>this.gotoDetail(record, 'payment')}>财务确认</Button>)
                        }
                        if( record.isPayment && !record.isReimbursed && 
                            !pagePar.noGL && ( permission.gl || record.reimburseUser === this.props.user.sub )){
                                btns.push(<Button onClick={()=>this.addReimburse(record)}>报销</Button>)
                                btns.push(<Button onClick={()=>this.addRepayment(record)}>还款</Button>)
                        }
                 }else if(record.type === 3){
                     //还款
                     if( (s === borrowingStatus.UnSubmit || s === borrowingStatus.Reject) &&
                     !pagePar.noGL && ( permission.gl || record.reimburseUser === this.props.user.sub )){
                         btns.push(<Button onClick={()=>this.deleteBorrowing(record)}>作废</Button>)
                         btns.push(<Button onClick={()=>this.gotoRepaymentDetail(record, 'edit')}>修改</Button>)
                         btns.push(<Button onClick={()=>this.submitBorrowing(record)}>提交</Button>)
                     } 
                     if(s === borrowingStatus.Submit && (!pagePar.noQR && permission.qr) ){
                         btns.push(<Button onClick={()=>this.gotoRepaymentDetail(record, 'confirm')}>确认</Button>)
                     }
                     if(s === borrowingStatus.Confirm && record.recordingStatus!==recordingStatus.Confirm && (!pagePar.noFK && permission.fk) ){
                         btns.push(<Button onClick={()=>this.gotoRepaymentDetail(record, 'payment')}>财务确认</Button>)
                     }
                    
                 }
                
                  return <span>
                    <ButtonGroup>
                        {btns}
                    </ButtonGroup>
                    
                  </span>
                }
            },
        ]
        return <Layer className="content-page">
            <div style={{marginTop: '1.5rem'}}>
                <Form>
                <Row  className="form-row">
                {
                    this.state.permission.gl?
                    <Col span={8}>
                    <FormItem label="申请部门">
                            {getFieldDecorator('reimburseDepartment', {
                            })(
                                <TreeSelect
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.state.nodes}
                                    placeholder="请选择报销门店"
                                />
                                )}
                    </FormItem>
                </Col>:null
                }
                
                <Col span={16} style={{display:'flex'}}>
                    <FormItem label="申请日期">
                                {
                                    getFieldDecorator('startDate')(
                                        <DatePicker />
                                    )
                                }
                                
                    </FormItem> 
                    -
                    <FormItem>
                                {
                                    getFieldDecorator('endDate')(
                                        <DatePicker />
                                    )
                                }
                                
                    </FormItem>    
                </Col>
                
                </Row>
                <Row className="form-row">
                { this.state.statusList.length===1?null:
                <Col span={4}>
                    <FormItem label="预借款状态">
                                {
                                    getFieldDecorator('status')(

                                        <Select>
                                            {
                                                this.state.statusList.map(x=>(
                                                    <Option key={x.value} value={x.value}>{x.key}</Option>
                                                ))
                                            }
                                        </Select>
                                    )
                                }
                                
                    </FormItem>           
                </Col>
                }
               
                    <Col span={8}>
                    <FormItem label="是否付款">
                                {
                                    getFieldDecorator('isPayment')(
                                        <RadioGroup>
                                            <Radio value={null}>不限</Radio>
                                            <Radio value={true}>已付</Radio>
                                            <Radio value={false}>未付</Radio>
                                        </RadioGroup>
                                    )
                                }
                                
                    </FormItem>      
                    </Col>
                    <Col span={8}>
                    <FormItem label="是否还款">
                                {
                                    getFieldDecorator('isReimbursed')(
                                        <RadioGroup>
                                            <Radio value={null}>不限</Radio>
                                            <Radio value={true}>已还款</Radio>
                                            <Radio value={false}>未还款</Radio>
                                        </RadioGroup>
                                    )
                                }
                                
                    </FormItem>      
                    </Col>
                </Row>
                <Row className="form-row">
                <Col span={16} style={{display:'flex'}}>
                    <FormItem label="关键字" style={{flex:1}}>
                                {
                                    getFieldDecorator('keyword')(
                                        <Input placeholder="预借款单号、接口人员工编号、姓名或预借款说明" />
                                    )
                                }
                                
                    </FormItem>    
                    <Button style={{marginLeft:'1rem'}} onClick={this.search}>搜索</Button>  
                    </Col>
                </Row>
                </Form>
            </div>
            <div className="page-btn-bar">
            {
                this.state.pagePar.noAdd?null:<Button type="primary" onClick={this.addBorrowing}>预借款申请</Button>
            }
                
            </div>
            <div className="page-fill">
                <Table style={{width:'100%'}} columns={columns} dataSource={this.state.list} 
                    loading={this.state.loading}
                    rowKey="id"
                    pagination={this.state.pagination}
                    onChange={this.handleTableChange}
                    />
            </div>
            <LayerRouter>
                <Route path={`${this.props.match.url}/borrowingInfo`}  render={(props)=><BorrowingInfo changeCallback={this.changeCallback} {...props}/>}/>
                <Route path={`${this.props.match.url}/chargeInfo`}  render={(props)=><AddCharge  {...props}/>}/>
                <Route path={`${this.props.match.url}/repayment`}  render={(props)=><Repayment  {...props}/>}/>
            </LayerRouter>
        </Layer>
    }
}

const mapStateToProps = (state, props) => ({
    dic: state.basicData.dicList,
    user: state.oidc.user.profile
})
const mapActionToProps = (dispatch) => ({
    getDicParList: (...args) => dispatch(getDicParList(...args))
})

export default connect(mapStateToProps, mapActionToProps)(Form.create()(BorrowingIndex))