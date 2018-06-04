//按揭过户组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import moment from 'moment'
import { getDicParList,dealGhSave } from '../../../actions/actionCreator'
import { notification, DatePicker, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;

class TradeTransfer extends Component {

    state={
        ghFkrq:'',//付款日期
        ghGhrq:'',//过户日期
        ghJbrq:'',//解保日期
        isDataLoading:false,
        rpData:{}
    }
    componentWillMount = () => {
        this.setState({isDataLoading:true,tip:'信息初始化中...'})
        this.props.dispatch(getDicParList(['COMMISSION_GH_DKNX','COMMISSION_WY_CQ','COMMISSION_PAY_TYPE']));
    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false });
        if(newProps.operInfo.operType === 'GHSAVE_UPDATE'){
            notification.success({
                message: '提示',
                description: '保存成交报告过户信息成功!',
                duration: 3
            });
            newProps.operInfo.operType = ''
        }
        else if(newProps.operInfo.operType === 'GHGET_UPDATE'){//信息获取成功
            this.setState({ rpData: newProps.ext});
            newProps.operInfo.operType = ''
        }
    }
    handleSave = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = this.props.rpId;
                if(this.state.ghFkrq!==''){
                    values.ghFkrq = this.state.ghFkrq;
                }
                else{
                    values.ghFkrq = this.state.rpData.ghFkrq;
                }
                if(this.state.ghGhrq!==''){
                    values.ghGhrq = this.state.ghGhrq;
                }
                else{
                    values.ghGhrq = this.state.rpData.ghGhrq;
                }
                if(this.state.ghJbrq!==''){
                    values.ghJbrq = this.state.ghJbrq;
                }
                else{
                    values.ghJbrq = this.state.rpData.ghJbrq;
                }
                ///////
                if(values.ghZzyh === 'true'){
                    values.ghZzyh = 1
                }
                else{
                    values.ghZzyh = 2
                }
                if(values.ghZzdb === 'true'){
                    values.ghZzdb = 1
                }
                else{
                    values.ghZzdb = 2
                }
                if(values.ghQydbgs === '1'){
                    values.ghQydbgs = 1
                }
                else{
                    values.ghQydbgs = 2
                }

                console.log(values);
                this.setState({isDataLoading:true,tip:'保存信息中...'})
                this.props.dispatch(dealGhSave(values))
            }
        });
    }
    ghFkrq_dateChange=(value,dateString)=>{
        console.log(dateString)
        this.setState({ghFkrq:dateString})
    }
    ghJbrq_dateChange=(value,dateString)=>{
        console.log(dateString)
        this.setState({ghJbrq:dateString})
    }
    ghGhrq_dateChange=(value,dateString)=>{
        this.setState({ghGhrq:dateString})
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
    render() {
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const { getFieldDecorator } = this.props.form;
        let ghDknxTypes = this.props.basicData.ghDknxTypes;
        let wyCqTypes = this.props.basicData.wyCqTypes;
        let payTypes = this.props.basicData.payTypes;
        const dateFormat = 'YYYY-MM-DD'

        return (
            <Layout>
                <div>
                    <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>所属区域</span>)}>
                                {
                                    getFieldDecorator('ghSsqy', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.ghSsqy,
                                    })(
                                        <Select style={{ width: 200 }}>
                                        {
                                            wyCqTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                        }
                                    </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>付款方式</span>)}>
                                {
                                    getFieldDecorator('ghFkfs', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.ghFkfs,
                                    })(
                                        <Select style={{ width: 200 }}>
                                        {
                                            payTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                        }
                                    </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>签约担保公司</span>)}>
                                {
                                    getFieldDecorator('ghQydbgs', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: this.state.rpData.ghQydbgs,
                                    })(
                                        <Select style={{ width: 200 }}>
                                          <Option key='1' value='1'>是</Option>
                                          <Option key='1' value='2'>否</Option>
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>担保公司</span>)}>
                                {
                                    getFieldDecorator('ghDbgs', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: this.state.rpData.ghDbgs,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款银行</span>)}>
                                {
                                    getFieldDecorator('ghDkyh', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.ghDkyh,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>保费金额</span>)}>
                                {
                                    getFieldDecorator('ghDbfje', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: this.state.rpData.ghDbfje,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款支行</span>)}>
                                {
                                    getFieldDecorator('ghDkzh', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.ghDkzh,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款成数</span>)}>
                                {
                                    getFieldDecorator('ghDkcs', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: this.state.rpData.ghDkcs,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款金额</span>)}>
                                {
                                    getFieldDecorator('ghDkje', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: this.state.rpData.ghDkje,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>贷款年限</span>)}>
                                {
                                    getFieldDecorator('ghDknx', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.ghDknx,
                                    })(
                                            <Select style={{ width: 80 }}>
                                            {
                                              ghDknxTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                          </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>评估公司</span>)}>
                                {
                                    getFieldDecorator('ghPggs', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: this.state.rpData.ghPggs,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>资金监管金额</span>)}>
                                {
                                    getFieldDecorator('ghZjjgje', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: this.state.rpData.ghZjjgje,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>业主欠款</span>)}>
                                {
                                    getFieldDecorator('ghYzqk', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.ghYzqk,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>赎楼罚息</span>)}>
                                {
                                    getFieldDecorator('ghSlfx', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>赎楼利息</span>)}>
                                {
                                    getFieldDecorator('ghSllx', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: this.state.rpData.ghSllx,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>担保金额</span>)}>
                                {
                                    getFieldDecorator('ghDbje', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.ghDbje,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>抵押回执号</span>)}>
                                {
                                    getFieldDecorator('ghDyhzh', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: this.state.rpData.ghDyhzh,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户回执号</span>)}>
                                {
                                    getFieldDecorator('ghGhhzh', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: this.state.rpData.ghGhhzh,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>自找银行</span>)}>
                                {
                                    getFieldDecorator('ghZzyh', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.ghZzyh,
                                    })(
                                        <Select style={{ width: 200 }}>
                                        <Option key='1' value='true'>是</Option>
                                        <Option key='1' value='false'>否</Option>
                                      </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>放款日期</span>)}>
                                {
                                    getFieldDecorator('ghFkrq', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: moment(this.getInvalidDate(this.state.rpData.ghFkrq))
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.ghFkrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>解保日期</span>)}>
                                {
                                    getFieldDecorator('ghJbrq', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: moment(this.getInvalidDate(this.state.rpData.ghJbrq)),
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.ghJbrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>自找担保</span>)}>
                                {
                                    getFieldDecorator('ghZzdb', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.ghZzdb,
                                    })(
                                        <Select style={{ width: 200 }}>
                                          <Option key='1' value='true'>是</Option>
                                          <Option key='1' value='false'>否</Option>
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户日期</span>)}>
                                {
                                    getFieldDecorator('ghGhrq', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: moment(this.getInvalidDate(this.state.rpData.ghGhrq)),
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.ghGhrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户产证地址</span>)}>
                                {
                                    getFieldDecorator('ghGhczdz', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.ghGhczdz,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户价格</span>)}>
                                {
                                    getFieldDecorator('ghGhjg', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: this.state.rpData.ghGhjg,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户业主电话</span>)}>
                                {
                                    getFieldDecorator('ghGhyzdh', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: this.state.rpData.ghGhyzdh,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>过户客户电话</span>)}>
                                {
                                    getFieldDecorator('ghGhkhdh', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.ghGhkhdh,
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
                                    getFieldDecorator('ghBz', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.ghBz,
                                    })(
                                        <Input.TextArea rows={4} style={{ width: 510 }}></Input.TextArea>
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
        ext:state.rp.ext
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradeTransfer);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);