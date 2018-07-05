//业绩分配组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import moment from 'moment'
import { dealFpSave, acmentParamListGet ,searchHuman} from '../../../actions/actionCreator'
import { DatePicker, notification, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect, InputNumber } from 'antd'
import TradeWyTable from './tradeWyTable'
import TradeNTable from './tradeNTable'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import SearchCondition from '../../../constants/searchCondition'
import uuid from 'uuid'

const FormItem = Form.Item;
class TradePerDis extends Component {

    constructor(props) {
        super(props);
        this.state = {
            isDataLoading: false,
            wyItems:[],
            nyItems:[],
            outsideList:[],
            insideList:[],
            totalyj: 0,
            yjKhyjdqr: '',
            yjYzyjdqr: '',
            yjFtItems: [],//业绩分摊项
            humanList:[],//员工列表
        }
     
    }

    componentWillMount = () => {

    }
    componentDidMount = () => {
      this.initEntity(this.props);
      this._getFtItems();
    }
    componentWillReceiveProps = (nextProps) => {
      if (this.props.entity !== nextProps.entity && nextProps.entity) {
  
        this.initEntity(nextProps)
      }
    }
  
    initEntity = (nextProps) => {
      var entity = nextProps.entity;
      if (!entity) {
        return;
      }
  
      let mv = {};
      Object.keys(entity).map(key => {
        mv[key] = entity[key];
      })
      this.props.form.setFieldsValue(mv);
    }

    _getFtItems = async () => {
        let report = this.props.report;
        if(!report.gsmc){
            return;
        }  
        let url = `${WebApiConfig.baseset.acmentlistget}${report.gsmc}`
        let r = await ApiClient.get(url);
        r = (r||{}).data ||{};
        if(r.code==='0'){
            let list = r.extension ||[];
            let wyItems =[], nyItems=[];
            list.forEach(item=>{
                if(item.type===1){
                    wyItems.push(item)
                }else if(item.type === 2){
                    nyItems.push(item)
                }
            })
            this.setState({wyItems: wyItems, nyItems:nyItems})
        }else{
            notification.error({message:'获取分摊项列表失败'})
        }
    }

    //添加内佣项目
    handleAddNbFp = () => {
        let item = {
            id: uuid.v1(),
            type: null,
            remark:'',
            uid:null,
            money:0,
            odd_num:null,
            errors:{

            }
        }

        let nyList  = [...this.state.insideList, item]
        this.setState({insideList: nyList})
    }

    //内佣列表变更
    onNyRowChanged = (row, key, value)=>{
        let ol  = this.state.insideList;
        let idx = ol.findIndex(x=>x.id === row.id);
        if(idx<0){
            return;
        }
        
        let newRow = {...ol[idx]}
        newRow[key] = value;
        if(key==='uid'){
            if(value){
                newRow.uid = [{key:value.id, label: value.name}];
                newRow.sectionId = value.departmentId;
                newRow.sectionName = value.organizationFullName;
                newRow.workNumber = value.userID;
            }else{
                newRow.uid = [];
                newRow.sectionId='';
                newRow.sectionName = '';
                newRow.workNumber = '';
            }
            
        }

        



        ol[idx] = newRow;
        this.setState({insideList: [...ol]},()=>{
          
        })

    }
    //删除内佣项
    onNyDelRow = (row)=>{
        let ol  = this.state.insideList;
        let idx = ol.findIndex(x=>x.id === row.id);
        if(idx<0){
            return;
        }
        ol.splice(idx,1);
        this.setState({insideList: [...ol]},()=>{
            
        })
    }
    
    //计算业绩浄佣金
    _calcYjJyj = ()=>{
        let zyj = (this.props.form.getFieldValue("yjZcjyj") ||'0')*1;
        //扣减外佣
        let wyJe= 0;
        this.state.outsideList.forEach(item=>{
            wyJe = wyJe + (item.money*1)
        })
        let jyj = zyj - wyJe

        this.props.form.setFieldsValue({yjJyj: jyj})
    }
    calcZyj = ()=>{
       setTimeout(()=>{
            var yj = this.props.form.getFieldsValue(["yjYzys","yjKhys"]);
            var zyj = (yj.yjYzys||0)*1 + (yj.yjKhys||0)*1;
            this.props.form.setFieldsValue({yjZcjyj: zyj})
            setTimeout(() => {
                this._calcYjJyj()
            }, 0);
       },0)
      
    }

    //添加外佣项
    handleAddWy = () => {
        let item = {
            id: uuid.v1(),
            moneyType: null,
            remark:'',
            object:'',
            money:0,
            errors:{

            }
        }

        let wyList  = [...this.state.outsideList, item]
        this.setState({outsideList: wyList})
    }

    //外佣列表变更
    onWyRowChanged = (row, key, value)=>{
        let ol  = this.state.outsideList;
        let idx = ol.findIndex(x=>x.id === row.id);
        if(idx<0){
            return;
        }

        let newRow = {...ol[idx]}
        newRow[key] = value;

        if(key === 'moneyType'){
            let zyj = (this.props.form.getFieldValue("yjZcjyj") ||'0')*1;

            let item = this.state.wyItems.find(x=>x.code ===value);
            if(item && item.percent){
                newRow.money = Math.round((zyj * (item.percent||0))*100) / 100;
            }
        }

        ol[idx] = newRow;
        this.setState({outsideList: [...ol]},()=>{
            this._calcYjJyj()
        })

    }

    onWyDelRow = (row)=>{
        let ol  = this.state.outsideList;
        let idx = ol.findIndex(x=>x.id === row.id);
        if(idx<0){
            return;
        }
        ol.splice(idx,1);
        this.setState({outsideList: [...ol]},()=>{
            this._calcYjJyj()
        })
    }
    

    render() {
        const { getFieldDecorator, getFieldValue } = this.props.form;
        const {report} = this.props;

        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const formItemLayout2 = {
            labelCol: { span: 10 },
            wrapperCol: { span: 10 },
        };

        let yjTotal = report.ycjyj;
        let zyj = (getFieldValue("yjZcjyj") ||'0')*1;


        return (
            <Layout>
                <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                    <Row className="form-row">
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>业主应收</span>)}>
                                {
                                    getFieldDecorator('yjYzys')(
                                        <InputNumber style={{ width: 200 }} onChange={this.calcZyj}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout2} label={(<span>业主佣金到期日</span>)}>
                                {
                                    getFieldDecorator('yjYzyjdqr', {
                                        rules: [{ required: false, message: '请选择成交日期!' }]
                                    })(
                                        <DatePicker disabled={true} style={{ width: 200 }} ></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>客户应收</span>)}>
                                {
                                    getFieldDecorator('yjKhys')(
                                        <InputNumber style={{ width: 200 }} onChange={this.calcZyj}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout2} label={(<span>客户佣金到期日</span>)}>
                                {
                                    getFieldDecorator('yjKhyjdqr', {
                                        rules: [{ required: false, message: '请选择成交日期!' }]
                                    })(
                                        <DatePicker disabled={true} style={{ width: 200 }} ></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={24}>
                            <FormItem {...formItemLayout} style={{ width: '25rem' }} hasFeedback validateStatus={yjTotal === zyj?'':'error'}  label={(<span>总成交佣金</span>)}>
                                {
                                    getFieldDecorator('yjZcjyj')(
                                        <Input  disabled={true}></Input>
                                    )
                                }

                            </FormItem>
                        </Col>
                    </Row>

                    <Row >
                        <Col span={3}><Button onClick={this.handleAddWy}>新增外佣</Button></Col>
                    </Row>
                    <Row>
                        <TradeWyTable canEdit={this.props.opType=='add' || this.props.opType=='edit'} onRowChanged={this.onWyRowChanged} onDelRow= {this.onWyDelRow} dic={this.props.dic} items={this.state.wyItems} dataSource={this.state.outsideList}/>
                    </Row>
                    <Row className="form-row" >
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>净佣金</span>)}>
                                {
                                    getFieldDecorator('yjJyj', {
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 200 }} disabled={true}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={3}><Button onClick={this.handleAddNbFp}>新增内部分配</Button></Col>
                    </Row>
                     <Row>
                        <TradeNTable canEdit={this.props.opType=='add' || this.props.opType=='edit'} 
                            onRowChanged={this.onNyRowChanged} 
                            onDelRow= {this.onNyDelRow} 
                            dic={this.props.dic} 
                            items={this.state.nyItems} 
                            dataSource={this.state.insideList}/>
                    </Row> 
                    {/* <Row>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <Button type='primary' onClick={this.handleSave}>保存</Button>
                        </Col>
                    </Row> */}
                </Spin>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        // basicData: state.base,
        // operInfo: state.rp.operInfo,
        // ext: state.rp.ext,
        // acmOperInfo: state.acm.operInfo,
        // syncData: state.rp.syncData,
        // syncFpOp: state.rp.syncFpOp,
        // syncFpData: state.rp.syncFpData,
        // scaleSearchResult:state.acm.scaleSearchResult,
        // humanList:state.org.humanList,
        // humanOperInfo:state.org.operInfo

    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradePerDis);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
