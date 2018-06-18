import React, {Component} from 'react';
import {Select, Table, Form, Checkbox, Input,TreeSelect, Row, Col,Button, notification} from 'antd'
import {connect} from 'react-redux';
import FixedTable from '../../../components/FixedTable'
import {AuthorUrl} from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import {getDicPars, getOrganizationTree} from '../../../utils/utils'
import {getDicParList} from '../../../actions/actionCreators'
import moment from 'moment'

const FormItem = Form.Item;
const Option = Select.Option;

class AddCharge extends Component{
    state={
        nodes:[],
        feeList:[]
    }

    componentDidMount=()=>{
        this.props.getDicParList(['CHARGE_COST_TYPE']);
        this.getNodes();
    }

    changeRowValue = (record, key, e)=>{
        let v= e||'';
        if(v.target){
            v = v.target.value;
        }
        record[key] = v;

        let fl = this.state.feeList;
        let idx = fl.indexOf(record);

        fl[idx] = {...record}
        this.setState({feeList:[...fl]})
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

    addItem=()=>{
        let list = [...this.state.feeList];
        list.push({amount:0, memo:'', type:''});
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
                <Select defaultValue={record.type} 
                    style={{width:'100%'}}
                    onChange={(e)=>{this.changeRowValue(record, 'type',e)}}>
                    {
                        groupList.map((item)=>{
                            return <Option key={item.value} value={item.value}>{item.key}</Option>
                        })
                    }
                </Select>
            )

            
          }, {
            title: '摘要',
            dataIndex: 'memo',
            key: 'memo',
            render:(text,record)=>(
                <Input value={record.memo} onChange={(e)=>this.changeRowValue(record, 'memo',e)}/>
            )
          }, {
            title: '金额',
            dataIndex: 'amount',
            key: 'amount',
            width:'10rem',
            render:(text,record)=>(
                <Input value={record.amount} onChange={(e)=>this.changeRowValue(record, 'amount',e)}
                    style={{textAlign:'right'}}/>
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

        return <div className="content-page">
            <div>
                <div className="page-title">费用报销单</div>
                <div className="page-subtitle">日期</div>
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
                                {getFieldDecorator('reimburseUser', {})(
                                    <Input readOnly />
                                    )}
                        </FormItem>
                    </Col>

                </Row>
                <Row className="form-row">
                    <Col span={6}>
                        <FormItem label="后补发票">
                                {getFieldDecorator('chargeAmount', {})(
                                    <Checkbox  />
                                    )}
                        </FormItem>
                    </Col>
                    <Col span={18}>
                        <FormItem  label="说明">
                                {getFieldDecorator('memo', {})(
                                    <Input  />
                                    )}
                        </FormItem>
                    </Col>
                </Row>

                
                <Row className="form-row">
                    <Col span={6}>
                        <FormItem label="报销总额">
                                {getFieldDecorator('chargeAmount', {})(
                                    <Input  />
                                    )}
                        </FormItem>
                    </Col>
                    
                </Row>    
                
            </Form>
            </div>
            <div style={{marginTop:'1rem'}}>
                <Button onClick={this.addItem}>添加费用项</Button>
            </div>
            <div style={{marginTop:'0.5rem'}}>
                <Table columns={columns} dataSource={this.state.feeList}
                    bordered={true} pagination={false}/>
            </div>
            
            <div>
            
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