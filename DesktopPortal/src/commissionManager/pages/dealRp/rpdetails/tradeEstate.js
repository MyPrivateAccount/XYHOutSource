//成交物业组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import moment from 'moment'
import { getDicParList ,dealWySave} from '../../../actions/actionCreator'
import {DatePicker, notification,Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;

class TradeEstate extends Component {
    state = {
        isDataLoading:false,
        wyCqzqdsj:'',
        rpData:{}
    }
    componentWillMount = () => {

        this.setState({isDataLoading:true,tip:'信息初始化中...'})
        //获取字典项
        this.props.dispatch(getDicParList(['COMMISSION_WY_CQ', 'COMMISSION_WY_PQ', 'COMMISSION_WY_WYLX', 'COMMISSION_WY_KJLX', 'COMMISSION_WY_ZXZK', 'COMMISSION_WY_ZXND', 'COMMISSION_WY_ZXJJ', 'COMMISSION_WY_WYCX','COMMISSION_PAY_TYPE']));
    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false });
        if(newProps.operInfo.operType === 'WYSAVE_UPDATE'){
            notification.success({
                message: '提示',
                description: '保存成交报告物业信息成功!',
                duration: 3
            });
            newProps.operInfo.operType = ''
        }
        else if(newProps.operInfo.operType === 'WYGET_UPDATE'){//信息获取成功
            this.setState({ rpData: newProps.ext});
            newProps.operInfo.operType = ''
        }
    }
    handleSave = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = this.props.rpId;
                console.log(values);
                if(values.wySfhz === '1'){
                    values.wySfhz = 1
                }
                else{
                    values.wySfhz = 2
                }
                if(this.state.wyCqzqdsj!==''){
                    values.wyCqzqdsj = this.state.wyCqzqdsj;
                }
                else{
                    values.wyCqzqdsj = this.state.rpData.wyCqzqdsj;
                }
                this.setState({isDataLoading:true,tip:'保存信息中...'})
                this.props.dispatch(dealWySave(values));
            }
        });
    }
    wyCqzqdsj_dateChange=(value,dateString)=>{
        this.setState({wyCqzqdsj:dateString})
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
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const formItemLayout2 = {
            labelCol: { span: 10 },
            wrapperCol: { span: 10 }
        }
        let wyCqTypes = this.props.basicData.wyCqTypes;
        let wyPqTypes = this.props.basicData.wyPqTypes;
        let wyWylxTypes = this.props.basicData.wyWylxTypes;
        let wyKjlxTypes = this.props.basicData.wyKjlxTypes;
        let wyZxTypes = this.props.basicData.wyZxTypes;
        let wyZxndTypes = this.props.basicData.wyZxndTypes;
        let wyJjTypes = this.props.basicData.wyJjTypes;
        let wyCxTypes = this.props.basicData.wyCxTypes;
        let payTypes = this.props.basicData.payTypes;
        return (
            <Layout>
                <div>
                <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                    <Row style={{ textAlign: 'center', marginLeft: 30 }}>
                        <Col span={3}><span>城区</span></Col>
                        <Col span={3}><span>片区</span></Col>
                        <Col span={3}><span>物业名称</span></Col>
                        <Col span={4}><span>位置/栋/座/单元</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>楼层</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>房号</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>总楼层</span></Col>
                    </Row>
                    <Row>
                        <Col span={4}>
                            <FormItem {...formItemLayout2} label={(<span>物业地址</span>)}>
                                {
                                    getFieldDecorator('wyCq', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyCq,
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyCqTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyPq', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wqRq,
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyPqTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyMc', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wqMc,
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyWz', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyWz,
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyLc', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyLc,
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyFh', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyFh,
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyZlc', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyZlc,
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>产证物业地址</span>)}>
                                {
                                    getFieldDecorator('wyCzwydz', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyCzwydz,
                                    })(
                                        <Input style={{ width: 300 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={2} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>房</span>)}>
                                {
                                    getFieldDecorator('wyF', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyF,
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={2} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>厅</span>)}>
                                {
                                    getFieldDecorator('wyT', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyT,
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={2} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>卫</span>)}>
                                {
                                    getFieldDecorator('wyW', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyW,
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>阳台</span>)}>
                                {
                                    getFieldDecorator('wyYt', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyYt,
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>露台</span>)}>
                                {
                                    getFieldDecorator('wyLt', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyLt,
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>观景台</span>)}>
                                {
                                    getFieldDecorator('wyJgf', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyJgf,
                                    })(
                                        <Input style={{ width: 40 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>物业类型</span>)}>
                                {
                                    getFieldDecorator('wyWylx', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyWylx,
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyWylxTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>空间类型</span>)}>
                                {
                                    getFieldDecorator('wyKjlx', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyKjlx,
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyKjlxTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>建筑面积</span>)}>
                                {
                                    getFieldDecorator('wyJzmj', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyJzmj,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>均价</span>)}>
                                {
                                    getFieldDecorator('wyWyJj', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyJj,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>电梯数</span>)}>
                                {
                                    getFieldDecorator('wyDts', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyDts,
                                    })(
                                        <Input style={{ width: 120 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>每层户数</span>)}>
                                {
                                    getFieldDecorator('wyMchs', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyMchs,
                                    })(
                                        <Input style={{ width: 120 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>装修年代</span>)}>
                                {
                                    getFieldDecorator('wyZxnd', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyZxnd,
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyZxndTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>家具</span>)}>
                                {
                                    getFieldDecorator('wyJj', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyJj,
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                wyJjTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>产权取得时间</span>)}>
                                {
                                    getFieldDecorator('wyCqzqdsj', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: moment(this.getInvalidDate(this.state.rpData.wyCqzqdsj)),
                                    })(
                                        <DatePicker style={{ width: 180 }} onChange={this.wyCqzqdsj_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房产按揭号码</span>)}>
                                {
                                    getFieldDecorator('wyFcajhm', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: this.state.rpData.wyFcajhm,
                                    })(
                                        <Input style={{ width: 180 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>是否合租</span>)}>
                                {
                                    getFieldDecorator('wySfhz', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.wySfhz===1?'1':'2',
                                    })(
                                        <Select style={{ width: 180 }}>
                                            <Option key='1' value='1'>是</Option>
                                            <Option key='2' value='2'>否</Option>
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源付款方式</span>)}>
                                {
                                    getFieldDecorator('wyFyfkfs', {
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: this.state.rpData.wyFyfkfs,
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                payTypes.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源贷款年限</span>)}>
                                {
                                    getFieldDecorator('wyFydknx', {
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: this.state.rpData.wyFydknx,
                                    })(
                                        <Input style={{ width: 180 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout2} label={(<span>房源贷款剩余年限</span>)}>
                                {
                                    getFieldDecorator('wyFydksynx', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: this.state.rpData.wyFydksynx,
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源贷款金额</span>)}>
                                {
                                    getFieldDecorator('wyFydkje', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyFydkje,
                                    })(
                                        <Input style={{ width: 120 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源已还金额</span>)}>
                                {
                                    getFieldDecorator('wyFyyhje', {
                                        rules: [{ required: false }],
                                        initialValue: this.state.rpData.wyFyyhje,
                                    })(
                                        <Input style={{ width: 120 }}></Input>
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
const WrappedRegistrationForm = Form.create()(TradeEstate);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);