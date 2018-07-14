//收款组件
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Select, Row, Col, Form, Input,  Layout,  DatePicker, InputNumber, Radio } from 'antd'
import moment from 'moment'
import {getDicPars} from '../../../../utils/utils'
import { getDicParList } from '../../../../actions/actionCreators'
import { dicKeys, examineStatusMap } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient';
import validations from '../../../../utils/validations'

const FormItem = Form.Item;
const Option = Select.Option;
const RadioGroup = Radio.Group;

class FKCp extends Component {

    state={
        entity:{},
        zhList: []
    }

    _lastFgs = '';

    componentWillMount=()=>{
        this.props.getDicParList([
            dicKeys.wyfkfs
        ])
    }

    componentDidMount = ()=>{
        this.initEntity(this.props)
        this.initFkyt(this.props.wyItems)
    }

    componentWillReceiveProps =(nextProps)=>{
        if(this.props.entity !== nextProps.entity){
            this.initEntity(nextProps)
        }
        if(this.props.wyItems!== nextProps.wyItems && nextProps.wyItems){
            this.initFkyt(nextProps.wyItems)
        }
    }

    _getGszh = async ()=>{
        let fgs = (this.props.entity||{}).fgs;
        if(!fgs){
            fgs = this.props.user.Filiale
        }
        if(fgs === this._lastFgs){
            return;
        }
        let url  = `${WebApiConfig.baseset.orgget}${fgs}/GSZH`
        let r = await ApiClient.get(url);
        r = (r||{}).data||{};
        if(r.code==='0'){
            this._lastFgs = fgs;
            let pv = (r.extension||{}).parValue||'';
            let pvs = pv.split('|')
            if(pvs.length===0){
                pvs.push('默认账号')
            }
            this.setState({zhList: pvs})
            let gszh = this.props.form.getFieldValue('gszh');
            if(!gszh){
                this.props.form.setFieldsValue({
                    gszh: pvs[0]
                })
            }

        }
    }

    initEntity=(props)=>{
        let entity = props.entity ||{};
        this.setState({entity: entity},()=>{
            this.props.form.setFieldsValue(entity);
        })
        this._getGszh();


    }

    initFkyt = (items)=>{
        if(!items || items.length===0){
            return;
        }
        
        this.props.form.setFieldsValue({
            yt: items[0].code
        })
        
    }

    getValues = ()=>{
       let r = validations.validateForm(this.props.form)
       return r;
    }

   
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };

        let fkfsList= getDicPars(dicKeys.wyfkfs, this.props.dic)
        let fkytList = this.props.wyItems || [];
        let entity = this.state.entity;
        let canEdit = this.props.canEdit;

        return (
            <Layout>
                <Layout.Content>
                    <Row className="form-row">
                        <Col span={12} >
                            <FormItem {...formItemLayout} label={(<span>部门</span>)}>
                                {
                                    getFieldDecorator('sectionId')(
                                        <Select disabled style={{width:'15rem'}}>
                                            <Option key={entity.sectionId} value={entity.sectionId}>{entity.sectionName}</Option>
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout}  label={(<span>付款人</span>)}>
                                {
                                    getFieldDecorator('skrId')(
                                        <Select disabled style={{width:'15rem'}}>
                                            <Option key={entity.skrId} value={entity.skrId}>{entity.skr}</Option>
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>收据日期</span>)}>
                                {
                                    getFieldDecorator('sjrq', {
                                        rules: [{ required: false, message: '必须选择收据日期' }],
                                    })(
                                        <DatePicker disabled={!canEdit} style={{ width: 120 }}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>付款日期</span>)}>
                                {
                                    getFieldDecorator('jzrq', {
                                        rules: [{ required: true, message: '必须选择付款日期' }],
                                    })(
                                        <DatePicker disabled={!canEdit} style={{ width: 120 }}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>金额</span>)}>
                                {
                                    getFieldDecorator('je', {
                                        rules: [{ required: true, message: '请输入金额' }],
                                    })(
                                        <InputNumber disabled={!canEdit} precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>用途</span>)}>
                                {
                                    getFieldDecorator('yt')(
                                        <Select disabled={!canEdit}>
                                         {
                                             fkytList.map(x=><Option key={x.code} value={x.code}>{x.name}</Option>)
                                         }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        
                        <Col span={12} className="form-row">
                            <FormItem {...formItemLayout} label={(<span>付款账号</span>)}>
                                {
                                    getFieldDecorator('gszh')(
                                        <Select defaultActiveFirstOption disabled={!canEdit}  style={{width:'15rem'}}>
                                          {
                                              this.state.zhList.map(x=><Option key={x} value={x}>{x}</Option>)
                                          }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>付款方式</span>)}>
                                {
                                    getFieldDecorator('skfs')(
                                        <RadioGroup disabled={!canEdit} >
                                            {
                                                fkfsList.map(x=><Radio key={x.value} value={x.value}>{x.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        
                    </Row>
                    <Row className="form-row">
                        <Col span={12} >
                            <FormItem {...formItemLayout} className="auto-width" label={(<span>进账单日志号/付款人</span>)}>
                                {
                                    getFieldDecorator('jzdrzh', {
                                        rules: [{ required: false, message: '请输入付款人' }],
                                    })(
                                        <Input disabled={!canEdit} ></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={12} >
                            <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                                {
                                    getFieldDecorator('bz', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input.TextArea disabled={!canEdit} rows={4} style={{ width: 510 }}></Input.TextArea>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>买方/承租方</span>)}>
                                {
                                    getFieldDecorator('buy', {
                                        rules: [{ required: false, message: '必须输入买方，承租方' }],
                                    })(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>[买]/证件号码</span>)}>
                                {
                                    getFieldDecorator('buyCode', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input disabled={!canEdit} style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>卖方/承租方</span>)}>
                                {
                                    getFieldDecorator('sell', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input disabled={!canEdit} style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>[卖]/证件号码</span>)}>
                                {
                                    getFieldDecorator('sellCode', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input disabled={!canEdit} style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                </Layout.Content>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        dic: state.basicData.dicList,
        user: state.oidc.user.profile || {}
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch,
        getDicParList: (...args) => dispatch(getDicParList(...args))
    };
}
const WrappedRegistrationForm = Form.create()(FKCp);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);