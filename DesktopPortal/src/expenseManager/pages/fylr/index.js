import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Modal, Button,Radio, Select, Table, Row, Col, Form, Input, Checkbox, TreeSelect, DatePicker, notification} from 'antd'
import {AuthorUrl, basicDataServiceUrl} from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import {getDicPars, getOrganizationTree} from '../../../utils/utils'
import {getDicParList} from '../../../actions/actionCreators'
// import FixedTable from '../../../components/FixedTable'
import Layer, { LayerRouter } from '../../../components/Layer'
import {Route } from 'react-router'
import AddCharge from './AddCharge'
import uuid from 'uuid'
import moment from 'moment'
import {chargeStatus, billStatus, permission} from './const'

const FormItem = Form.Item;
const ButtonGroup = Button.Group;
const RadioGroup = Radio.Group;
const confirm = Modal.confirm;
const Option = Select.Option;

class FylrIndex extends Component{

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
            sl.push({key: chargeStatus[chargeStatus.UnSubmit], value: chargeStatus.UnSubmit})
            sl.push({key: chargeStatus[chargeStatus.Reject], value: chargeStatus.Reject})
            sl.push({key: chargeStatus[chargeStatus.Submit], value: chargeStatus.Submit})
            sl.push({key: chargeStatus[chargeStatus.Confirm], value: chargeStatus.Confirm})
        }
        this.setState({statusList: sl});

        this.props.getDicParList(['CHARGE_COST_TYPE']);
        this.getNodes();

        this.getPermission();

        
        this.props.form.setFieldsValue({
            status:sl[0].value, 
            billStatus:-1, 
            isBackup:null, 
            isPayment: (typeof initState.isPayment==='boolean') ?initState.isPayment: null
        })
        
    }

    getPermission = async ()=>{
        let url = `${AuthorUrl}/api/Permission/each`
        let r = await ApiClient.post(url, [permission.qr, permission.fk, permission.gl])
        if( r && r.data && r.data.code==='0'){
            let p = {};
            (r.data.extension||[]).forEach(pi=>{
                if(pi.permissionItem===permission.qr){
                    p.qr = pi.isHave;
                }else if(pi.permissionItem===permission.fk){
                    p.fk = pi.isHave;
                }else if(pi.permissionItem===permission.gl){
                    p.gl = pi.isHave;
                }
            })
            console.log(p);
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
        this.props.history.push(`${this.props.match.url}/chargeInfo`, {entity: item, op: op||'view', pagePar: this.state.pagePar})
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
        if(condition.billStatus===-1){
            condition.billStatus = null;
        }else{
            condition.billStatus = [condition.billStatus];
        }

        if(this.state.statusList.length===1){
            condition.status = [this.state.statusList[0].value]
        }

        condition.pageSize = this.state.pagination.pageSize;
        condition.pageIndex = this.state.pagination.pageIndex;
        this.setState({loading:true})
        try{
            let url = `${basicDataServiceUrl}/api/chargeinfo/search`;
            let r = await ApiClient.post(url, condition);
            if(r && r.data && r.data.code==='0'){
                this.setState({list: r.data.extension, pagination: {...this.state.pagination, total: r.data.totalCount}});
            }else{
                notification.error({message:'查询报销单列表失败'});
            }
        }catch(e){
            notification.error({message:'查询报销单列表失败'})
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

    addCharge = ()=>{
        let newFee = {
            id: uuid.v1(),
            type:1,
            createTime: new Date(),
            reimburseDepartment: this.props.user.Organization,
            reimburseUser: this.props.user.sub,
            reimburseUserName: this.props.user.nickname
        }
      
        this.props.history.push(`${this.props.match.url}/chargeInfo`, {entity: newFee, op: 'add', pagePar: this.state.pagePar})
    }

    deleteCharge = (record)=>{
        //删除
        confirm({
            title: `您确定要删除报销单[${record.chargeNo}]吗?`,
            content: '',
            onOk: async ()=> {
              let url = `${basicDataServiceUrl}/api/chargeinfo/${record.id}`
              let r=  await ApiClient.post(url, null, null, 'DELETE');
              if(r && r.data && r.data.code==='0'){
                  notification.success({message:'删除成功', description:'报销单已被删除'})
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

    submitCharge=(record)=>{
        confirm({
            title: `您确定要提交报销单[${record.chargeNo}]吗?`,
            content: '提交后不可再进行修改',
            onOk: async ()=> {
              let url = `${basicDataServiceUrl}/api/chargeinfo/submit/${record.id}`
              let r=  await ApiClient.post(url, null, null, 'POST');
              if(r && r.data && r.data.code==='0'){
                  notification.success({message:'提交成功', description:'报销单已提交'})
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

    changeCallback = (entity)=>{
        let l = this.state.list;
        let idx=  l.findIndex(x=>x.id === entity.id);
        if(idx>=0){
            l[idx] = {...entity,...{
                status: entity.status, 
                billStatus: entity.billStatus, 
                isBackup: entity.isBackup, 
                backuped: entity.backuped, 
                isPayment: entity.isPayment,
                paymentAmount: entity.paymentAmount
            }}
            this.setState({list: [...l]})
        }
    }

    render(){
        const { getFieldDecorator } = this.props.form;

        const columns = [
            {
                title: '报销单号',
                dataIndex: 'chargeNo',
                key: 'chargeNo',
                width:'10rem',
                render: (text, record)=>{
                    return <a href="javascript:void();" title="点击查看详情" onClick={()=>this.gotoDetail(record)}>{text}</a>
                }
            },
            {
                title: '报销门店',
                dataIndex: 'reimburseDepartmentName',
                key: 'reimburseDepartmentName'
            },
            {
                title: '报销人',
                dataIndex: 'reimburseUserInfo',
                key: 'reimburseUserInfo',
                width:'6rem',
                render:(text,record)=>{
                    return (record.reimburseUserInfo||{}).userName
                }
            },
            {
                title: '报销日期',
                dataIndex: 'createTime',
                key: 'createTime',
                width:'8rem',
                render:(text,record)=>{
                    return moment(record.createTime).format('YYYY-MM-DD');
                }
            },
            {
                title: '申请人',
                dataIndex: 'createUserInfo',
                key: 'createUserInfo',
                width:'6rem',
                render:(text,record)=>{
                    return (record.createUserInfo||{}).userName
                }
            },
            {
                title: '费用总额',
                dataIndex: 'chargeAmount',
                key: 'chargeAmount',
                className: 'column-money',
                width:'8rem',
                render:(text,record)=>{
                    return <span style={{textAlign:'right'}}>{record.chargeAmount}</span>
                }
            },
            {
                title: '后补发票',
                dataIndex: 'isBackup',
                key: 'isBackup',
                width:'4rem',
                render:(text,record)=>{
                    return record.isBackup?'是':'否'
                }
            },
            {
                title: '发票状态 ',
                dataIndex: 'billStatus',
                key: 'billStatus',
                width:'5rem',
                render:(text,record)=>{
                    return record.isBackup?(billStatus[record.billStatus]||''):'-'
                }
            },
            {
                title: '付款',
                dataIndex: 'isPayment',
                key: 'isPayment',
                width:'4rem',
                render:(text,record)=>{
                    return record.isPayment?'已付':'未付'
                }
            },
            {
                title: '已付款金额',
                dataIndex: 'paymentAmount',
                key: 'paymentAmount',
                className: 'column-money',
                width:'8rem',
                render:(text,record)=>{
                    return <span style={{textAlign:'right'}}>{record.paymentAmount}</span>
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
                title: '操作',
                width:'15rem',
                render: (text,record)=>{
                 let btns = [];
                 if((record.status === chargeStatus.UnSubmit || record.status === chargeStatus.Reject) && 
                    (!this.state.pagePar.noGL && (this.state.permission.gl || record.createUser===this.props.user.sub))
                ){
                     btns.push(<Button onClick={()=>this.deleteCharge(record)}>作废</Button>)
                 }
                 if((record.status === chargeStatus.UnSubmit || record.status === chargeStatus.Reject)
                 && 
                    (!this.state.pagePar.noGL && (this.state.permission.gl || record.createUser===this.props.user.sub))
                ){
                    btns.push(<Button onClick={()=>this.gotoDetail(record, 'edit')}>修改</Button>)
                }
                if((record.status === chargeStatus.UnSubmit || record.status === chargeStatus.Reject) && 
                    (!this.state.pagePar.noGL && (this.state.permission.gl || record.createUser===this.props.user.sub))
                ){
                    btns.push(<Button onClick={()=>this.submitCharge(record, 'edit')}>提交</Button>)
                }
                 if(record.status === chargeStatus.Submit && (!this.state.pagePar.noQR && this.state.permission.qr) ){
                    btns.push(<Button onClick={()=>this.gotoDetail(record, 'confirm')}>确认</Button>)
                 }
                 if(record.status >= chargeStatus.Submit && record.isBackup && (record.billStatus===billStatus.UnSubmit || record.billStatus===billStatus.Reject)
                && (!this.state.pagePar.noGL && (this.state.permission.gl || record.createUser===this.props.user.sub))
                ){
                     btns.push(<Button onClick={()=>this.gotoDetail(record, 'backup')}>补发票</Button>)
                 }
                 if((record.status === chargeStatus.Confirm && !record.isPayment) && (!this.state.pagePar.noFK && this.state.permission.fk )){
                    btns.push(<Button onClick={()=>this.gotoDetail(record, 'payment')}>付款</Button>)
                 }
                 
                 if(record.isBackup && record.backuped && record.billStatus === billStatus.Submit && (!this.state.pagePar.noQR && this.state.permission.qr )){
                    btns.push(<Button onClick={()=>this.gotoDetail(record, 'confirmBill')}>发票确认</Button>)
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
                <Col span={8}>
                    <FormItem label="报销门店">
                            {getFieldDecorator('reimburseDepartment', {
                            })(
                                <TreeSelect
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.state.nodes}
                                    placeholder="请选择报销门店"
                                />
                                )}
                    </FormItem>
                </Col>
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
                    <FormItem label="报销单状态">
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
                <Col span={4}>
                    <FormItem label="发票状态">
                                {
                                    getFieldDecorator('billStatus')(
                                        <Select>
                                            <Option value={-1}>所有</Option>
                                            <Option value={billStatus.UnSubmit}>{billStatus[billStatus.UnSubmit]}</Option>
                                            <Option value={billStatus.Submit}>{billStatus[billStatus.Submit]}</Option>
                                            <Option value={billStatus.Confirm}>{billStatus[billStatus.Confirm]}</Option>
                                        </Select>
                                    )
                                }
                                
                    </FormItem>           
                </Col>
                <Col span={8}>
                <FormItem label="后补发票">
                                {
                                    getFieldDecorator('isBackup')(
                                        <RadioGroup>
                                            <Radio value={null}>不限</Radio>
                                            <Radio value={true}>是</Radio>
                                            <Radio value={false}>否</Radio>
                                        </RadioGroup>
                                    )
                                }
                                
                    </FormItem>         
                </Col>
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
                    
                </Row>
                <Row className="form-row">
                <Col span={16} style={{display:'flex'}}>
                    <FormItem label="关键字" style={{flex:1}}>
                                {
                                    getFieldDecorator('keyword')(
                                        <Input placeholder="报销单号、报销人编号、姓名或报销说明" />
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
                this.state.pagePar.noAdd?null:<Button type="primary" onClick={this.addCharge}>录入报销单</Button>
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
                            <Route path={`${this.props.match.url}/chargeInfo`}  render={(props)=><AddCharge changeCallback={this.changeCallback} {...props}/>}/>
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

export default connect(mapStateToProps, mapActionToProps)(Form.create()(FylrIndex))