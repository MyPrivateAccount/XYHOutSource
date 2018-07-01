//成交物业组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import {DatePicker, Form, Layout,  Row, Col, Input, Spin, Select,  InputNumber } from 'antd'
import { getDicPars } from '../../../../utils/utils'
import { dicKeys } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'

const FormItem = Form.Item;
const Option = Select.Option;

let now = new Date();
let zxndList =[];
for(let i = 1990;i<=now.getFullYear();i++){
    zxndList.push({key: i, value: i})
}

class TradeEstate extends Component {
    state = {
        loading:false,
        wyCqzqdsj:'',
        rpData:{},
        districtList:[],
        areaList:[]
    }
    componentWillMount = () => {
    }
    componentDidMount=()=>{
        this.initEntity(this.props);
    }
    componentWillReceiveProps = (nextProps) => {
        if (this.props.entity !== nextProps.entity && nextProps.entity) {

            this.initEntity(nextProps)
        }
    }

    initEntity = (nextProps)=>{
             if(!nextProps.showBbSelector){
            this.getDistrictList();
                }
        
        var entity = nextProps.entity;
        if(!entity){
            return;
        }

        if(nextProps.showBbSelector){
            let codes = [];
            if(entity.wyCq){
                codes.push(entity.wyCq)
            }
            if(entity.wyPq){
                codes.push(entity.wyPq)
            }
            if(codes.length>0){
                this.getInitAreaList(codes);
            }
        }

   
            let mv = {};
            Object.keys(entity).map(key => {
                mv[key] = entity[key];
            })
            this.props.form.setFieldsValue(mv); 
    }

    getInitAreaList = async (codes)=>{
        let url = `${WebApiConfig.area.list}`
        let r = await ApiClient.post(url, {codes: codes});
        r = (r||{}).data||{};
        if(r.code==='0' && r.extension){
            let dl = [],al = [];
            r.extension.forEach(item=>{
                if(item.level=='2'){
                    dl.push(item);
                }else if(item.level=='3'){
                    al.push(item)
                }
            })
            this.setState({districtList: dl, areaList: al})
        }
    }

    getDistrictList = async ()=>{
        let city  = this.props.user.City;
        if(!city){
            return;
        }

        let url = `${WebApiConfig.area.get}${city}`
        let r = await ApiClient.get(url);
        r = (r||{}).data||{};
        if(r.code==='0' && r.extension){
            this.setState({districtList: r.extension})
           
        }
    }

    getAreaList = async (value)=>{
        this.props.form.setFieldsValue({wyPq:null})
        let url = `${WebApiConfig.area.get}${value}`
        let r = await ApiClient.get(url);
        r = (r||{}).data||{};
        if(r.code==='0' && r.extension){
            this.setState({areaList: r.extension})
        }
    }


    render() {
        const { getFieldDecorator } = this.props.form;
        const { showBbSelector } = this.props;
        const {districtList, areaList} = this.state;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const formItemLayout2 = {
            labelCol: { span: 10 },
            wrapperCol: { span: 10 }
        }

        let wyWylxTypes = getDicPars(dicKeys.wylx, this.props.dic);
        let wyKjlxTypes = getDicPars(dicKeys.kjlx, this.props.dic);
        let wyJjTypes = getDicPars(dicKeys.jj, this.props.dic);

        let payTypes = getDicPars(dicKeys.fkfs, this.props.dic);;
        return (
            <Layout>
                <div>
                <Spin spinning={this.state.loading} tip={this.state.tip}>
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
                                        rules: [{ required: true,message:'请选择城区' }]
                                    })(
                                        <Select onChange={this.getAreaList} disabled={showBbSelector}  style={{ width: 80 }}>
                                            {
                                                districtList.map(tp => <Option key={tp.code} value={tp.code}>{tp.name}</Option>)
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
                                        rules: [{ required: true,message:'请选择片区' }]
                                    })(
                                        <Select disabled={showBbSelector}  style={{ width: 80 }}>
                                            {
                                                areaList.map(tp => <Option key={tp.code} value={tp.code}>{tp.name}</Option>)
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
                                        rules: [{ required: true,message:'请填写物业名称'}]
                                    })(
                                        <Input disabled={showBbSelector} style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyWz', {
                                        rules: [{ required: true,message:'楼栋'}]
                                    })(
                                        <Input disabled={showBbSelector} style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyLc', {
                                        rules: [{ required: true,message:'请填写物业楼层'}]
                                    })(
                                        <InputNumber disabled={showBbSelector} style={{ width: 80 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyFh', {
                                        rules: [{ required: true,message:'请填写房号'}]
                                    })(
                                        <Input disabled={showBbSelector}  style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <FormItem>
                                {
                                    getFieldDecorator('wyZlc', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber   style={{ width: 80 }}></InputNumber>
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
                                        rules: [{ required: true,message:'请填写产证物业地址'}]
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
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber  style={{ width: 40 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={2} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>厅</span>)}>
                                {
                                    getFieldDecorator('wyT', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber style={{ width: 40 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={2} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>卫</span>)}>
                                {
                                    getFieldDecorator('wyW', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber style={{ width: 40 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>阳台</span>)}>
                                {
                                    getFieldDecorator('wyYt', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber style={{ width: 40 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>露台</span>)}>
                                {
                                    getFieldDecorator('wyLt', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber style={{ width: 40 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={3} push={1}>
                            <FormItem {...formItemLayout2} label={(<span>观景台</span>)}>
                                {
                                    getFieldDecorator('wyJgf', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber style={{ width: 40 }}></InputNumber>
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
                                        rules: [{ required: false }]
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
                                        rules: [{ required: false }]
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
                                        rules: [{ required: true,message:'请填写建筑面积'}]
                                    })(
                                        <InputNumber disabled={showBbSelector}  precision={2}  style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>均价</span>)}>
                                {
                                    getFieldDecorator('wyWyJj', {
                                        rules: [{ required: true,message:'请填写均价'}]
                                    })(
                                        <InputNumber disabled precision={2} style={{ width: 200 }}></InputNumber>
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
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber style={{ width: 120 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>每层户数</span>)}>
                                {
                                    getFieldDecorator('wyMchs', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber style={{ width: 120 }}></InputNumber>
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
                                        rules: [{ required: false }]
                                    })(
                                        <Select style={{ width: 80 }}>
                                            {
                                                zxndList.map(tp => <Option key={tp.key} value={tp.value}>{tp.key}</Option>)
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
                                        rules: [{ required: false }]
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
                                        rules: [{ required: false, message: '请填写产权取得时间!' }]
                                    })(
                                        <DatePicker style={{ width: 180 }} ></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房产按揭号码</span>)}>
                                {
                                    getFieldDecorator('wyFcajhm', {
                                        rules: [{ required: false, message: '请填写房产按揭号码!' }]
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
                                        rules: [{ required: false, message: '请选择是否合租' }]
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
                                        rules: [{ required: false, message: '请选择房源付款方式' }]
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
                                        rules: [{ required: false, message: '请填写房源贷款年限!' }]
                                    })(
                                        <InputNumber style={{ width: 180 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout2} label={(<span>房源贷款剩余年限</span>)}>
                                {
                                    getFieldDecorator('wyFydksynx', {
                                        rules: [{ required: false, message: '请填写房源贷款剩余年限!' }]
                                    })(
                                        <InputNumber style={{ width: 80 }}></InputNumber>
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
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber precision={2} style={{ width: 120 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源已还金额</span>)}>
                                {
                                    getFieldDecorator('wyFyyhje', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber precision={2} style={{ width: 120 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    {/* <Row>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <Button type='primary' onClick={this.handleSave}>保存</Button>
                        </Col>
                    </Row> */}
                    </Spin>
                </div>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        // basicData: state.base,
        // operInfo:state.rp.operInfo,
        // ext:state.rp.ext,
        // syncWyOp:state.rp.syncWyOp,
        // syncWyData:state.rp.syncWyData
        user: state.oidc.user.profile||{}
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradeEstate);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
