import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Button,Radio, Table, Row, Col, Form, Input, Checkbox, TreeSelect, DatePicker, notification} from 'antd'
import {AuthorUrl, basicDataServiceUrl} from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import {getDicPars, getOrganizationTree} from '../../../utils/utils'
import {getDicParList} from '../../../actions/actionCreators'
import uuid from 'uuid'
import moment from 'moment'

const FormItem = Form.Item;
const ButtonGroup = Button.Group;
const RadioGroup = Radio.Group;
class FylrIndex extends Component{

    state={
        nodes:[],
        pagination:{pageSize:10, pageIndex: 1},
        list:[],
        loading:false
    }

    componentDidMount=()=>{
        this.props.getDicParList(['CHARGE_COST_TYPE']);
        this.getNodes();
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
        condition.pageSize = this.state.pagination.pageSize;
        condition.pageIndex = this.state.pagination.pageIndex;
        this.setState({loading:true})
        try{
            let url = `${basicDataServiceUrl}/api/chargeinfo/search`;
            let r = await ApiClient.post(url, condition);
            if(r && r.data && r.data.code==='0'){
                this.setState({list: r.data.extension});
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
            createTime: new Date()
        }
        this.props.history.push('/dd', {entity: newFee, op: 'add'})
    }

    render(){
        const { getFieldDecorator } = this.props.form;

        const columns = [
            {
                title: '报销单号',
                dataIndex: 'chargeNo',
                key: 'chargeNo',
                width:'16rem'
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
                title: '发票',
                dataIndex: 'isBackup',
                key: 'isBackup',
                width:'4rem',
                render:(text,record)=>{
                    return record.isBackup?'是':'否'
                }
            },
            {
                title: '付款',
                dataIndex: 'isPayment',
                key: 'isPayment',
                width:'4rem',
                render:(text,record)=>{
                    return record.isPayment?'是':'否'
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
                width:'5rem'
            },
            {
                title: '操作',
                width:'15rem',
                render: (text,record)=>{
                  return <span>
                    <ButtonGroup>
                        <Button>作废</Button>
                        <Button>确认</Button>
                        <Button>补发票</Button>
                        <Button>付款</Button>
                    </ButtonGroup>
                    
                  </span>
                }
            },
        ]
        return <div className="content-page">
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
                    <Col span={8} style={{display:'flex'}}>
                    <FormItem label="关键字" style={{flex:1}}>
                                {
                                    getFieldDecorator('keyword')(
                                        <Input placeholder="报销单号、报销人编号、姓名或报销说明" />
                                    )
                                }
                                
                    </FormItem>    
                    <Button onClick={this.search}>搜索</Button>  
                    </Col>
                </Row>
                </Form>
            </div>
            <div className="page-btn-bar">
                <Button type="primary" onClick={this.addCharge}>录入报销单</Button>
            </div>
            <div className="page-fill">
                <Table columns={columns} dataSource={this.state.list} 
                    loading={this.state.loading}
                    rowKey="id"
                    pagination={this.state.pagination}
                    onChange={this.handleTableChange}
                    />
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

export default connect(mapStateToProps, mapActionToProps)(Form.create()(FylrIndex))