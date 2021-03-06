//调佣对话框
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Row, Col, Form, Modal, TreeSelect, Select, Spin, Input, Button } from 'antd'
import {dealFpGet} from '../../actions/actionCreator'
import TradeWyTable from '../dealRp/rpdetails/tradeWyTable'
import TradeNTable from '../dealRp/rpdetails/tradeNTable'
const FormItem = Form.Item;
const Option = Select.Option;

class TYComponet extends Component {

    state = {
        isDataLoading: false,
        vs: false,
        empList: [],
        rpData: {
            yjYzys: 0,
            yjKhys: 0,
            reportInsides:[],
            reportOutsides:[]
        },
        totalyj: 0,
    }
    componentDidMount() {
        this.props.onTYSelf(this);
    }
    componentWillReceiveProps = (newProps) => {
        this.setState({ isDataLoading: false });
        if (newProps.operInfo.operType === 'FPGET_UPDATE') {//信息获取成功
            if (JSON.stringify(newProps.ext) !== '[]') {
                this.setState({ rpData: newProps.ext },()=>{
                    this.reCountZyj()
                });
                
            }
            newProps.operInfo.operType = ''
        }
    }
    handleOk = (e) => {

    }
    handleCancel = (e) => {
        this.setState({ vs: false })
    }
    show = (e) => {
        this.setState({ vs: true,rpId: e.id},()=>{
            this.setState({isDataLoading:true})
            this.props.dispatch(dealFpGet(this.state.rpId));
        })
    }
    handleAddWy = (e) => {
        e.preventDefault();
        this.wytb.handleAdd();
    }
    handleAddNbFp = (e) => {
        e.preventDefault();
        this.fptb.handleAdd();
    }
    onWyTableRef = (e) => {
        this.wytb = e
    }
    onFpTableRef = (e) => {
        this.fptb = e
    }
    getInvalidDate = (dt) => {
        var newdt = '' + dt;
        if (newdt.indexOf('T') !== -1) {
            newdt = newdt.substr(0, newdt.length - 9);
            console.log("newdt:" + newdt)
            return newdt;
        }
        return dt
    }
    handleYzchange = (e) => {
        console.log("handleYzchange");
        console.log(e.target.value)
        let temp = this.state.rpData;
        temp.yjYzys = e.target.value;
        this.setState({ rpData: temp });
        this.reCountZyj()
    }
    handleKhchange = (e) => {
        console.log("handleKhchange");
        console.log(e.target.value)
        let temp = this.state.rpData;
        temp.yjKhys = e.target.value
        this.setState({ rpData: temp })
        this.reCountZyj()
    }
    //计算总佣金
    reCountZyj = (e) => {
        let totalyj = parseFloat(this.state.rpData.yjYzys, 10) + parseFloat(this.state.rpData.yjKhys, 10);
        this.props.form.setFieldsValue({ 'yjZcjyj': totalyj })
        this.wytb.setZyj(totalyj)
        this.fptb.setZyj(totalyj)
        this.reCountJyj()
    }
    //计算净佣金
    reCountJyj = (e) => {
        let zwyj = this.wytb.getTotalWyj()
        let totalyj = this.props.form.getFieldValue('yjZcjyj')
        let Jyj = totalyj - zwyj
        this.props.form.setFieldsValue({ 'yjJyj': Jyj })
    }
    /////////////////////////////////////////////////
    onWyTableRef1 = (e) => {
        this.wytable1 = e
    }
    onFpTableRef1 = (e) => {
        this.fptable1 = e
    }
    //////////////////////////////////
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        return (
            <Modal title={'调佣'} width={800} maskClosable={false} visible={this.state.vs}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Spin spinning={this.state.isDataLoading}>
                <Row>
                    <Col span={8}>
                        <label>业主佣金:</label>
                        <label>{this.state.yzyj}</label>
                    </Col>
                    <Col span={8}>
                        <label>客户佣金:</label>
                        <label>{this.state.khyj}</label>
                    </Col>
                    <Col span={8}>
                        <label>净佣金:</label>
                        <label>{this.state.jyj}</label>
                    </Col>
                </Row>
                <Row>
                    <TradeWyTable onWyTableRef={this.onWyTableRef1} dataSource={this.state.rpData.reportInsides} isTy={false}/>
                </Row>
                <Row>
                    <TradeNTable onFpTableRef={this.onFpTableRef1} dataSource={this.state.rpData.reportOutsides} isTy={false}/>
                </Row>
                <Row style={{ margin: 10 }}>
                    <Col span={12}>
                        <label>本次调整后业绩分配</label>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={(<span>业主佣金</span>)}>
                            {
                                getFieldDecorator('yjYzys', {
                                    initialValue: this.state.rpData.yjYzys,
                                })(
                                    <Input style={{ width: 200 }} onChange={this.handleYzchange}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={(<span>客户佣金</span>)}>
                            {
                                getFieldDecorator('yjKhys', {
                                    initialValue: this.state.rpData.yjKhys,
                                })(
                                    <Input style={{ width: 200 }} onChange={this.handleKhchange}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={(<span>总成交佣金</span>)}>
                            {
                                getFieldDecorator('yjZcjyj', {
                                    rules: [{ required: false, message: '请选择成交日期!' }],
                                    initialValue: this.state.totalyj
                                })(
                                    <Input style={{ width: 200 }} disabled={true}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={(<span>净佣金</span>)}>
                            {
                                getFieldDecorator('yjJyj', {
                                    rules: [{ required: false, message: '请选择成交日期!' }],
                                    initialValue: this.state.totalyj
                                })(
                                    <Input style={{ width: 200 }} disabled={true}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                            {
                                getFieldDecorator('bz', {
                                    rules: [{ required: false, message: '请填写备注' }],
                                    initialValue: this.state.rpData.bz,
                                })(
                                    <Input.TextArea rows={4} style={{ width: 600 }}></Input.TextArea>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={3}><Button type='primary' onClick={this.handleAddWy}>新增外佣</Button></Col>
                </Row>
                <Row>
                    <TradeWyTable onWyTableRef={this.onWyTableRef} totalyj={this.state.yjZcjyj} onCountJyj={this.reCountJyj} branchId={"1"} dataSource={this.state.rpData.reportInsides} isTy = {true}/>
                </Row>
                <Row>
                    <Col span={3}><Button type='primary' onClick={this.handleAddNbFp}>新增内部分配</Button></Col>
                </Row>
                <Row>
                    <TradeNTable onFpTableRef={this.onFpTableRef} branchId={"1"} dataSource={this.state.rpData.reportOutsides} isTy={true}/>
                </Row>
                </Spin>
            </Modal>
        )
    }
}
function MapStateToProps(state) {

    return {
        permissionOrgTree: state.org.permissionOrgTree,
        operInfo:state.rp.operInfo,
        ext:state.rp.ext
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TYComponet);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);