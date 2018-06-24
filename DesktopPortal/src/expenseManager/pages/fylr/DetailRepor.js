import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Modal, Button,Radio, Select, Table, Row, Col, Form, Input,  TreeSelect, DatePicker, notification} from 'antd'
import {AuthorUrl, basicDataServiceUrl} from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import {getDicPars, getOrganizationTree} from '../../../utils/utils'
import {getDicParList} from '../../../actions/actionCreators'
import Layer, { LayerRouter } from '../../../components/Layer'
import {Route } from 'react-router'
import AddCharge from './AddCharge'
import moment from 'moment'
import {permission} from './const'

const FormItem = Form.Item;
const RadioGroup = Radio.Group;

class DetailRepor extends Component{

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
        

        this.props.getDicParList(['CHARGE_COST_TYPE']);
        this.getNodes();

        this.props.form.setFieldsValue({
            isBackup:null, 
            isPayment:  null
        })
        
    }

    getNodes=async ()=>{
        let url = `${AuthorUrl}/api/Permission/${permission.mxb}`;
        let r = await ApiClient.get(url, true);
        if(r && r.data && r.data.code==='0'){
            var nodes = getOrganizationTree(r.data.extension);
            this.setState({nodes: nodes});
        }else{
            notification.error(`获取部门列表失败:${((r||{}).data||{}).message||''}`);
        }
    }

    gotoDetail = (item, op)=>{
        this.props.history.push(`${this.props.match.url}/chargeInfo`, {entity: item.chargeInfo, op: op||'view', pagePar: this.state.pagePar})
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
        condition = {...condition};
        condition.pageSize = this.state.pagination.pageSize;
        condition.pageIndex = this.state.pagination.pageIndex;
        console.log(condition);
        this.setState({loading:true})
        try{
            let url = `${basicDataServiceUrl}/api/chargeinfo/searchdetail`;
            let r = await ApiClient.post(url, condition);
            if(r && r.data && r.data.code==='0'){
                if(!this._dicMap){
                    let dicGroup = getDicPars('CHARGE_COST_TYPE', this.props.dic)||[];
                    this._dicMap = {};
                    dicGroup.forEach(item=>{
                        this._dicMap[item.value*1] = item.key;
                    });
                }
               

                r.data.extension.forEach(item=>{
                    item.typeName= this._dicMap[item.type];
                })
                this.setState({list: r.data.extension});
            }else{
                notification.error({message:'查询费用详细列表失败'});
            }
        }catch(e){
            notification.error({message:'查询费用详细列表失败'})
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


    render(){
        const { getFieldDecorator } = this.props.form;

        const columns = [
            {
                title: '报销单号',
                dataIndex: 'chargeInfo.chargeNo',
                key: 'chargeInfo.chargeNo',
                width:'10rem',
                render: (text, record)=>{
                    return <a href="javascript:void();" title="点击查看详情" onClick={()=>this.gotoDetail(record)}>{text}</a>
                }
            },
            {
                title: '报销门店',
                dataIndex: 'chargeInfo.reimburseDepartmentName',
                key: 'chargeInfo.reimburseDepartmentName'
            },
            {
                title: '报销人',
                dataIndex: 'chargeInfo.reimburseUserInfo',
                key: 'chargeInfo.reimburseUserInfo',
                width:'6rem',
                render:(text,record)=>{
                    return (record.chargeInfo.reimburseUserInfo||{}).userName
                }
            },
            {
                title: '报销日期',
                dataIndex: 'createTime',
                key: 'createTime',
                width:'8rem',
                render:(text,record)=>{
                    return moment(record.chargeInfo.createTime).format('YYYY-MM-DD');
                }
            },  
            {
                title: '费用项目',
                dataIndex: 'typeName',
                key: 'typeName',
                width:'8rem'
            },
            {
                title: '报销金额',
                dataIndex: 'amount',
                key: 'amount',
                className: 'column-money',
                width:'8rem',
                render:(text,record)=>{
                    return <span style={{textAlign:'right'}}>{record.amount}</span>
                }
            },
            {
                title: '付款',
                dataIndex: 'chargeInfo.isPayment',
                key: 'chargeInfo.isPayment',
                width:'4rem',
                render:(text,record)=>{
                    return record.chargeInfo.isPayment?'已付':'未付'
                }
            },
            {
                title: '备注',
                dataIndex: 'memo',
                key: 'memo'
            }
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

export default connect(mapStateToProps, mapActionToProps)(Form.create()(DetailRepor))