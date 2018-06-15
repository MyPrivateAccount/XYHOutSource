//交易合同组件
//合同列表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import moment from 'moment'
import { getDicParList,dealRpSave,syncYJDate} from '../../../actions/actionCreator'
import {notification, DatePicker, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect,Modal} from 'antd'
import './trade.less'

const RadioGroup = Radio.Group;
const FormItem = Form.Item;
class TradeContract extends Component {
    state = {
        isDataLoading:false,
        rpData:{},
        cjrq:'',
        yxsqyrq:'',
        yjfksj:'',
        kflfrq:'',
        htqyrq:'',
        isShowChooseReport:false
    }
    componentWillMount = () => {
        this.setState({isDataLoading:true,tip:'信息初始化中...'})
        this.props.dispatch(getDicParList(['COMMISSION_BSWY_CATEGORIES', 'COMMISSION_CJBG_TYPE', 'COMMISSION_JY_TYPE', 'COMMISSION_PAY_TYPE', 'COMMISSION_PROJECT_TYPE', 'COMMISSION_CONTRACT_TYPE', 'COMMISSION_OWN_TYPE', 'COMMISSION_TRADEDETAIL_TYPE', 'COMMISSION_SFZJJG_TYPE']));
    }
    componentDidMount=()=>{
    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false });

        if(newProps.operInfo.operType === 'HTSAVE_UPDATE'){//信息保存成功
            notification.success({
                message: '提示',
                description: '保存成交报告交易合同信息成功!',
                duration: 3
            });
            newProps.operInfo.operType = ''
        }
        else if(newProps.operInfo.operType === 'HTGET_UPDATE'){//信息获取成功
            this.setState({ rpData: newProps.ext});
            newProps.operInfo.operType = ''
        }
        else if(newProps.syncRpOp.operType === 'DEALRP_SYNC_RP'){
            let newdata = newProps.syncRpData
            this.props.form.setFieldsValue({'fyzId':newdata.fyzId})
            this.props.form.setFieldsValue({'cjrId':newdata.cjrId})
            this.props.form.setFieldsValue({'cjrq':moment(newdata.cjrq)})
            this.props.form.setFieldsValue({'cjzj':newdata.cjzj})
            this.props.form.setFieldsValue({'ycjyj':newdata.ycjyj})
            newProps.syncRpOp.operType = ''
            this.setState({rpData:newdata})
        }
    }
    handleSave = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = this.props.rpId;
                if(this.state.cjrq!==''){
                    values.cjrq = this.state.cjrq;
                }
                else{
                    values.cjrq = this.state.rpData.cjrq;
                }
                if(this.state.yxsqyrq!==''){
                    values.yxsqyrq = this.state.yxsqyrq;
                }
                else{
                    values.yxsqyrq = this.state.rpData.yxsqyrq;
                }
                if(this.state.yjfksj!==''){
                    values.yjfksj = this.state.yjfksj;
                }
                else{
                    values.yjfksj = this.state.rpData.yjfksj;
                }
                if(this.state.kflfrq!==''){
                    values.kflfrq = this.state.kflfrq;
                }
                else{
                    values.kflfrq = this.state.rpData.kflfrq;
                }
                if(this.state.htqyrq!==''){
                    values.htqyrq = this.state.htqyrq;
                }
                else{
                    values.htqyrq = this.state.rpData.htqyrq;
                }
                console.log(values);
                this.setState({isDataLoading:true,tip:'保存信息中...'})
                this.props.dispatch(dealRpSave(values));
            }
        });
    }
    cjrq_dateChange=(value,dateString)=>{
        console.log(dateString)
        this.setState({cjrq:dateString})
        this.props.dispatch(syncYJDate(dateString))//同步日期給业绩分配页面
    }
    wqrq_dateChange=(value,dateString)=>{
        console.log(dateString)
        this.setState({yxsqyrq:dateString})
    }
    yjfksj_dateChange=(value,dateString)=>{
        this.setState({yjfksj:dateString})
    }
    kflfrq_dateChange=(value,dateString)=>{
        this.setState({kflfrq:dateString})
    }
    htqyrq_dateChange=(value,dateString)=>{
        this.setState({htqyrq:dateString})
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
    //选择成交报备
    chooseReport=()=>{
        this.props.onShowCjbbtb()
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        let bswyTypes = this.props.basicData.bswyTypes;
        let cjbgTypes = this.props.basicData.cjbgTypes;
        let tradeTypes = this.props.basicData.tradeTypes;
        let projectTypes = this.props.basicData.projectTypes;
        let tradeDetailTypes = this.props.basicData.tradeDetailTypes;
        let ownTypes = this.props.basicData.ownTypes;
        let payTypes = this.props.basicData.payTypes;
        let contractTypes = this.props.basicData.contractTypes;
        let sfzjjgTypes = this.props.basicData.sfzjjgTypes;

        return (
            <Layout>
                <div style={{ marginLeft: 12 }}>
                    <Row>
                        <Col span={12} pull={1}>
                            <FormItem {...formItemLayout} label={(<span>成交报备</span>)}>
                                {
                                        <Input style={{ width: 200 }}></Input>
                                }
                            </FormItem>
                        </Col>
                        <Col span={12} pull={5}>
                            <Button onClick={this.chooseReport}>选择</Button>
                        </Col>
                    </Row>
                    <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>报数物业分类</span>)}>
                                {
                                    getFieldDecorator('bswylx', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.bswylx,
                                    })(
                                        <RadioGroup>
                                            {
                                                bswyTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>成交报告类型</span>)}>
                                {
                                    getFieldDecorator('cjbglx', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.cjbglx,
                                    })(
                                        <RadioGroup>
                                            {
                                                cjbgTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>公司名称</span>)}>
                                {
                                    getFieldDecorator('gsmc', {
                                        rules: [{ required: false, message: '请填写公司名称!' }],
                                        initialValue: this.state.rpData.gsmc,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>分行名称</span>)}>
                                {
                                    getFieldDecorator('fyzId', {
                                        rules: [{ required: true, message: '请填写分行名称!' }],
                                        initialValue: this.state.rpData.fyzId,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>成交人</span>)}>
                                {
                                    getFieldDecorator('cjrId', {
                                        rules: [{ required: true, message: '请填写成交人!' }],
                                        initialValue: this.state.rpData.cjrId,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>成交日期</span>)}>
                                {
                                    getFieldDecorator('cjrq', {
                                        rules: [{ required: true, message: '请选择成交日期!' }],
                                        initialValue: moment(this.getInvalidDate(this.state.rpData.cjrq)),
                                    })(
                                        <DatePicker style={{ width: 200 }}  onChange={this.cjrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>成交报告编号</span>)}>
                                {
                                    getFieldDecorator('cjbgbh', {
                                        rules: [{ required: true, message: '请填写成交编号'}],
                                        initialValue: this.state.rpData.cjbgbh,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>附加说明</span>)}>
                                {
                                    getFieldDecorator('fjsm', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.fjsm,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                                {
                                    getFieldDecorator('bz', {
                                        rules: [{ required: true ,message: '请填写备注'}],
                                        initialValue: this.state.rpData.bz,
                                    })(
                                        <Input.TextArea rows={4} style={{ width: 510 }}></Input.TextArea>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>交易类型</span>)}>
                                {
                                    getFieldDecorator('jylx', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.jylx,
                                    })(
                                        <RadioGroup>
                                            {
                                                tradeTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>项目类型</span>)}>
                                {
                                    getFieldDecorator('xmlx', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.xmlx,
                                    })(
                                        <RadioGroup>
                                            {
                                                projectTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>详细交易类型</span>)}>
                                {
                                    getFieldDecorator('xxjylx', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.xxjylx,
                                    })(
                                        <RadioGroup>
                                            {
                                                tradeDetailTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>产权类型</span>)}>
                                {
                                    getFieldDecorator('cqlx', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.cqlx,
                                    })(
                                        <RadioGroup>
                                            {
                                                ownTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>成交总价</span>)}>
                                {
                                    getFieldDecorator('cjzj', {
                                        rules: [{ required: true, message: '请填写成交总价!' }],
                                        initialValue: this.state.rpData.cjzj,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>佣金</span>)}>
                                {
                                    getFieldDecorator('ycjyj', {
                                        rules: [{ required: true, message: '请填写佣金金额!' }],
                                        initialValue: this.state.rpData.ycjyj,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>付款方式</span>)}>
                                {
                                    getFieldDecorator('fkfs', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.fkfs,
                                    })(
                                        <RadioGroup>
                                            {
                                                payTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>网签日期</span>)}>
                                {
                                    getFieldDecorator('yxsqyrq', {
                                        rules: [{ required: false }],
                                        initialValue: moment(this.getInvalidDate(this.state.rpData.yxsqyrq)),
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.wqrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>预计放款日期</span>)}>
                                {
                                    getFieldDecorator('yjfksj', {
                                        rules: [{ required: false }],
                                        initialValue: moment(this.getInvalidDate(this.state.rpData.yjfksj)),
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.yjfksj_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>预计放款金额</span>)}>
                                {
                                    getFieldDecorator('yjfkje', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.yjfkje,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>是否资金监管</span>)}>
                                {
                                    getFieldDecorator('sfzjjg', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.sfzjjg,
                                    })(
                                        <RadioGroup>
                                            {
                                                sfzjjgTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>客户来访日期</span>)}>
                                {
                                    getFieldDecorator('kflfrq', {
                                        rules: [{ required: false }],
                                        initialValue: moment(this.getInvalidDate(this.state.rpData.kflfrq)),
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.kflfrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>合同签约日期</span>)}>
                                {
                                    getFieldDecorator('htqyrq', {
                                        rules: [{ required: false }],
                                        initialValue: moment(this.getInvalidDate(this.state.rpData.htqyrq)),
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.htqyrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>合同类型</span>)}>
                                {
                                    getFieldDecorator('htlx', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.htlx,
                                    })(
                                        <RadioGroup>
                                            {
                                                contractTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>资金监管协议编号</span>)}>
                                {
                                    getFieldDecorator('jjjgxybh', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.jjjgxybh,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>买卖居间合同编号</span>)}>
                                {
                                    getFieldDecorator('mmjjhtbh', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.mmjjhtbh,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>自制合同</span>)}>
                                {
                                    getFieldDecorator('zzhtbh', {
                                        rules: [{ required: true,message:'请填写自制合同编号' }],
                                        initialValue: this.state.rpData.zzhtbh,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <Button type='primary' onClick={this.handleSave}>保存</Button>
                        </Col>
                    </Row>
                    </Spin>
                </div>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        operInfo:state.rp.operInfo,
        ext:state.rp.ext,
        syncRpData:state.rp.syncRpData,
        syncRpOp:state.rp.syncRpOp
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradeContract);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);