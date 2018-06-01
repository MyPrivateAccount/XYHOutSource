//业绩分配组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import moment from 'moment'
import {dealFpSave} from '../../../actions/actionCreator'
import {DatePicker,notification, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradeWyTable from './tradeWyTable'
import TradeNTable from './tradeNTable'

const FormItem = Form.Item;
class TradePerDis extends Component {

    constructor(props) {
        super(props);
        this.state={
            isDataLoading:false,
            rpData:{
                yjYzys:0,
                yjKhys:0
            },
            totalyj:0,
            yjKhyjdqr:'',
            yjYzyjdqr:''
        }
        this.handleYzchange = this.handleYzchange.bind(this)
        this.handleKhchange = this.handleKhchange.bind(this)
    }
    componentWillMount = () => {
    }
    componentDidMount=()=>{
    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false });
        if(newProps.operInfo.operType === 'FPSAVE_UPDATE'){
            notification.success({
                message: '提示',
                description: '保存成交报告业绩分配信息成功!',
                duration: 3
            });
            newProps.operInfo.operType = ''
        }
        else if(newProps.operInfo.operType === 'FPGET_UPDATE'){//信息获取成功
            if(JSON.stringify(newProps.ext)!=='[]'){
                this.setState({ rpData: newProps.ext});
                this.reCountZyj()
            }
            newProps.operInfo.operType = ''
        }
    }
    handleSave = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = this.props.rpId;
                let wyDatas = this.wytb.getData(values.id);
                let fpDatas = this.fptb.getData(values.id);
                values.reportOutsides = wyDatas;
                values.reportInsides = fpDatas;

                if(this.state.yjYzyjdqr!==''){
                    values.yjYzyjdqr = this.state.yjYzyjdqr;
                }
                else{
                    values.yjYzyjdqr = this.state.rpData.yjYzyjdqr;
                }
                if(this.state.yjKhyjdqr!==''){
                    values.yjKhyjdqr = this.state.yjKhyjdqr;
                }
                else{
                    values.yjKhyjdqr = this.state.rpData.yjKhyjdqr;
                }

                console.log(values);
                this.setState({isDataLoading:true,tip:'保存信息中...'})
                this.props.dispatch(dealFpSave(values))
            }
        });
    }
    handleAddWy = (e) => {
        e.preventDefault();
        this.wytb.handleAdd();
    }
    handleAddNbFp = (e)=>{
        e.preventDefault();
        this.fptb.handleAdd();
    }
    onWyTableRef = (ref) => {
        this.wytb = ref
    }
    onFpTableRef = (ref) =>{
        this.fptb = ref
    }
    getInvalidDate=(dt)=>{
        var newdt = ''+dt;
        if(newdt.indexOf('T')!==-1){
            newdt = newdt.substr(0,newdt.length-9);
            console.log("newdt:"+newdt)
            return newdt;
        }
        return dt
    }
    handleYzchange=(e)=>{
        console.log("handleYzchange");
        console.log(e.target.value)
        let temp = this.state.rpData;
        temp.yjYzys = e.target.value;
        this.setState({rpData:temp});
        this.reCountZyj()
    }
    handleKhchange=(e)=>{
        console.log("handleKhchange");
        console.log(e.target.value)
        let temp = this.state.rpData;
        temp.yjKhys = e.target.value
        this.setState({rpData:temp})
        this.reCountZyj()
    }
    //计算总佣金
    reCountZyj=(e)=>{
        let totalyj = parseFloat(this.state.rpData.yjYzys,10)+parseFloat(this.state.rpData.yjKhys,10);
        this.props.form.setFieldsValue({'yjZcjyj':totalyj})
        this.wytb.setZyj(totalyj)
        this.fptb.setZyj(totalyj)
        this.reCountJyj()
    }
    //计算净佣金
    reCountJyj=(e)=>{
        let zwyj = this.wytb.getTotalWyj()
        let totalyj = this.props.form.getFieldValue('yjZcjyj')
        let Jyj = totalyj-zwyj
        this.props.form.setFieldsValue({'yjJyj':Jyj})
    }
    yjYzyjdqr_dateChange=(value,dateString)=>{
        console.log(dateString)
        this.setState({yjYzyjdqr:dateString})
    }
    yjKhyjdqr_dateChange=(value,dateString)=>{
        console.log(dateString)
        this.setState({yjKhyjdqr:dateString})
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const formItemLayout2 = {
            labelCol: { span: 10 },
            wrapperCol: { span: 10 },
        };
        return (
            <Layout>
                <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                <Row>
                    <Col span={8}>
                        <FormItem {...formItemLayout} label={(<span>业主应收</span>)}>
                            {
                                getFieldDecorator('yjYzys', {
                                    initialValue: this.state.rpData.yjYzys,
                                })(
                                    <Input style={{ width: 200 }} onChange={this.handleYzchange}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                    <Col span={8}>
                        <FormItem {...formItemLayout2} label={(<span>业主佣金到期日</span>)}>
                            {
                                getFieldDecorator('yjYzyjdqr', {
                                    rules: [{ required: false, message: '请选择成交日期!' }],
                                    initialValue: moment(this.getInvalidDate(this.state.rpData.yjYzyjdqr)),
                                })(
                                    <DatePicker style={{ width: 200 }} onChange={this.yjYzyjdqr_dateChange}></DatePicker>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={8}>
                        <FormItem {...formItemLayout} label={(<span>客户应收</span>)}>
                            {
                                getFieldDecorator('yjKhys', {
                                    initialValue: this.state.rpData.yjKhys,
                                })(
                                    <Input style={{ width: 200 }} onChange={this.handleKhchange}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                    <Col span={8}>
                        <FormItem {...formItemLayout2} label={(<span>客户佣金到期日</span>)}>
                            {
                                getFieldDecorator('yjKhyjdqr', {
                                    rules: [{ required: false, message: '请选择成交日期!' }],
                                    initialValue: moment(this.getInvalidDate(this.state.rpData.yjKhyjdqr)),
                                })(
                                    <DatePicker style={{ width: 200 }} onChange={this.yjKhyjdqr_dateChange}></DatePicker>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={24} pull={4}>
                        <FormItem {...formItemLayout} label={(<span>总成交佣金</span>)}>
                            {
                                getFieldDecorator('yjZcjyj', {
                                    rules: [{ required: false, message: '请选择成交日期!' }],
                                    initialValue:this.state.totalyj
                                })(
                                    <Input style={{ width: 200 }} disabled={true}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={3}><Button type='primary' onClick={this.handleAddWy}>新增外佣</Button></Col>
                </Row>
                <Row>
                    <TradeWyTable onWyTableRef={this.onWyTableRef} totalyj = {this.state.yjZcjyj} onCountJyj={this.reCountJyj}/>
                </Row>
                <Row style={{margin:10,marginLeft:-30}}>
                    <Col span={12} pull={1}>
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
                    <Col span={3}><Button type='primary' onClick={this.handleAddNbFp}>新增内部分配</Button></Col>
                </Row>
                <Row>
                    <TradeNTable onFpTableRef={this.onFpTableRef}/>
                </Row>
                <Row>
                    <Col span={24} style={{ textAlign: 'center' }}>
                        <Button type='primary' onClick={this.handleSave}>保存</Button>
                    </Col>
                </Row>
                </Spin>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        operInfo: state.rp.operInfo,
        ext:state.rp.ext
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradePerDis);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
