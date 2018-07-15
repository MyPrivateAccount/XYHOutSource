//成交物业组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import {DatePicker, Form, Layout,  Row, Col, Input, Spin, Select,  InputNumber, Radio } from 'antd'
import { getDicPars } from '../../../../utils/utils'
import { dicKeys } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import RadioGroup from 'antd/lib/radio/group';

const FormItem = Form.Item;
const Option = Select.Option;

let now = new Date();
let zxndList =[];
for(let i = 1990;i<=now.getFullYear();i++){
    zxndList.push({key: i, value: i})
}

let llList = [];
for(let i =0;i<=100;i++){
    llList.push({key: i, value: i})
}

const styles = {
    unitLabel:{
        verticalAlign: 'middle',
        lineHeight: '32px',
        paddingLeft: '12px',
        marginRight:'1rem'
    }
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
        this.initEntity(this.props, true);
    }
    componentWillReceiveProps = (nextProps) => {
        if (this.props.entity !== nextProps.entity && nextProps.entity) {

            this.initEntity(nextProps)
        }
    }

    initEntity = (nextProps,isLoad)=>{
        let oldEntity = this.props.entity||{};
        var entity = nextProps.entity;

        if(isLoad || oldEntity.wyCq !== entity.wyCq || oldEntity.wyPq !== entity.wyPq || !entity.wyCq){
            if(!nextProps.showBbSelector){
                this.getDistrictList();
                    }
        }
            
        
        
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
                if( isLoad || oldEntity.wyCq !== entity.wyCq || oldEntity.wyPq !== entity.wyPq){
                this.getInitAreaList(codes);
                }
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
        let zxzkList = getDicPars(dicKeys.zxzk, this.props.dic);
        let cxList = getDicPars(dicKeys.cx, this.props.dic);

        let payTypes = getDicPars(dicKeys.fkfs, this.props.dic);;

        const canEdit = this.props.canEdit;

        return (
            <Layout>
                <div>
                <Spin spinning={this.state.loading} tip={this.state.tip}>
                    <Row >
                        <Col span={6}>
                            <span style={{width:'9rem', display:'inline-block'}}></span>
                            <span>城区</span>
                        </Col>
                        <Col span={3}><span>片区</span></Col>
                        <Col span={3}><span>物业名称</span></Col>
                        <Col span={3}><span>位置/栋/座/单元</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>楼层</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>房号</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>总楼层</span></Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={6}>
                            <FormItem {...formItemLayout2} label={(<span>物业地址</span>)}>
                                {
                                    getFieldDecorator('wyCq', {
                                        rules: [{ required: true,message:'请选择城区' }]
                                    })(
                                        <Select onChange={this.getAreaList} disabled={showBbSelector || !canEdit}  style={{ width: 80 }}>
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
                                        <Select disabled={showBbSelector || !canEdit}  style={{ width: 80 }}>
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
                                        <Input disabled={showBbSelector || !canEdit} style={{ width: 80 }}></Input>
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
                                        <Input disabled={showBbSelector || !canEdit} style={{ width: 80 }}></Input>
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
                                        <InputNumber disabled={showBbSelector || !canEdit} style={{ width: 80 }}></InputNumber>
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
                                        <Input disabled={showBbSelector || !canEdit}  style={{ width: 80 }}></Input>
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
                                        <InputNumber disabled={!canEdit}  style={{ width: 80 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={20}>
                            <FormItem {...formItemLayout} label={(<span>产证物业地址</span>)}>
                                {
                                    getFieldDecorator('wyCzwydz', {
                                        rules: [{ required: true,message:'请填写产证物业地址'}]
                                    })(
                                        <Input disabled={!canEdit} ></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row" style={{display:'flex'}}>
                        
                            <FormItem style={{width:'15rem'}} {...formItemLayout2} label={(<span>户型</span>)}>
                                {
                                    getFieldDecorator('wyF', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber disabled={!canEdit} min={0} style={{ width: '6rem' }}></InputNumber>
                                    )
                                }
                               
                            </FormItem>
                            <div style={styles.unitLabel}>房</div>
                       
                            <FormItem style={{width:'6rem'}} {...formItemLayout2}>
                                {
                                    getFieldDecorator('wyT', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber  disabled={!canEdit} min={0}></InputNumber>
                                    )
                                }
                            </FormItem>
                            <div style={styles.unitLabel}>厅</div>
                      
                       
                            <FormItem style={{width:'6rem'}} {...formItemLayout2} >
                                {
                                    getFieldDecorator('wyW', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber disabled={!canEdit} min={0}></InputNumber>
                                    )
                                }
                            </FormItem>
                            <div style={styles.unitLabel}>卫</div>
                        
                            <FormItem style={{width:'6rem'}} {...formItemLayout2}>
                                {
                                    getFieldDecorator('wyYt', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber disabled={!canEdit} min={0}></InputNumber>
                                    )
                                }
                            </FormItem>
                            <div style={styles.unitLabel}>阳台</div>
                        
                            <FormItem style={{width:'6rem'}} {...formItemLayout2} >
                                {
                                    getFieldDecorator('wyLt', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber disabled={!canEdit} min={0}></InputNumber>
                                    )
                                }
                            </FormItem>
                            <div style={styles.unitLabel}>露台</div>
                        
                            <FormItem style={{width:'6rem'}} {...formItemLayout2} >
                                {
                                    getFieldDecorator('WyJgf', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber disabled={!canEdit} min={0}></InputNumber>
                                    )
                                }
                            </FormItem>
                            <div style={styles.unitLabel}>观景台</div>
                         <Col span={4}>           
                            <FormItem  {...formItemLayout2} label="房源编号">
                                {
                                    getFieldDecorator('wyFybh', {
                                        rules: [{ required: false }]
                                    })(
                                        <Input disabled={!canEdit} ></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={9}>
                            <FormItem {...formItemLayout} label={(<span>物业类型</span>)}>
                                {
                                    getFieldDecorator('wyWylx', {
                                        rules: [{ required: false }]
                                    })(
                                        <Select style={{ width: 80 }} disabled={!canEdit} >
                                            {
                                                wyWylxTypes.map(tp => <Option key={tp.key} value={tp.value}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={9} >
                            <FormItem {...formItemLayout} label={(<span>空间类型</span>)}>
                                {
                                    getFieldDecorator('wyKjlx', {
                                        rules: [{ required: false }]
                                    })(
                                        <Select style={{ width: 80 }} disabled={!canEdit} >
                                            {
                                                wyKjlxTypes.map(tp => <Option key={tp.key} value={tp.value}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={6} >
                            <FormItem {...formItemLayout} label={(<span>楼龄</span>)}>
                                {
                                    getFieldDecorator('wyLl', {
                                        rules: [{ required: false }]
                                    })(
                                        <Select disabled={!canEdit} >
                                            {
                                                llList.map(tp => <Option key={tp.key} value={tp.value}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    
                    <Row className="form-row">
                        <Col span={9}>
                            <FormItem {...formItemLayout} label={(<span>建筑面积</span>)}>
                                {
                                    getFieldDecorator('wyJzmj', {
                                        rules: [{ required: true,message:'请填写建筑面积'}]
                                    })(
                                        <InputNumber min={0} disabled={showBbSelector || !canEdit}  precision={2}  style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={9}>
                            <FormItem {...formItemLayout} label={(<span>均价</span>)}>
                                {
                                    getFieldDecorator('wyWyJj', {
                                        rules: [{ required: true,message:'请填写均价'}]
                                    })(
                                        <InputNumber min={0} disabled precision={2} style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={6}>
                            <FormItem {...formItemLayout} label={(<span>实用面积</span>)}>
                                {
                                    getFieldDecorator('wySymj')(
                                        <InputNumber min={0} disabled={showBbSelector || !canEdit}  precision={2}  style={{ width: 200 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={9}>
                            <FormItem {...formItemLayout} label={(<span>电梯数</span>)}>
                                {
                                    getFieldDecorator('wyDts', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber disabled={!canEdit} style={{ width: 120 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={9}>
                            <FormItem {...formItemLayout} label={(<span>每层户数</span>)}>
                                {
                                    getFieldDecorator('wyMchs', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber disabled={!canEdit} style={{ width: 120 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={6}>
                            <FormItem {...formItemLayout} label={(<span>装修状况</span>)}>
                                {
                                    getFieldDecorator('wyZxzk', {
                                        rules: [{ required: false }]
                                    })(
                                        <Select disabled={!canEdit} style={{ width: 80 }}>
                                            {
                                                zxzkList.map(tp => <Option key={tp.key} value={tp.value}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={9}>
                            <FormItem {...formItemLayout} label={(<span>装修年代</span>)}>
                                {
                                    getFieldDecorator('wyZxnd', {
                                        rules: [{ required: false }]
                                    })(
                                        <Select disabled={!canEdit} style={{ width: 80 }}>
                                            {
                                                zxndList.map(tp => <Option key={tp.key} value={tp.value}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={9}>
                            <FormItem {...formItemLayout} label={(<span>家具</span>)}>
                                {
                                    getFieldDecorator('wyJj', {
                                        rules: [{ required: false }]
                                    })(
                                        <Select disabled={!canEdit} style={{ width: 80 }}>
                                            {
                                                wyJjTypes.map(tp => <Option key={tp.key} value={tp.value}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={6}>
                            <FormItem {...formItemLayout} label={(<span>朝向</span>)}>
                                {
                                    getFieldDecorator('wyCx', {
                                        rules: [{ required: false }]
                                    })(
                                        <Select disabled={!canEdit} style={{ width: 80 }}>
                                            {
                                                cxList.map(tp => <Option key={tp.key} value={tp.key}>{tp.key}</Option>)
                                            }
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <div className="divider"></div>
                    <Row className="form-row">
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>产权取得时间</span>)}>
                                {
                                    getFieldDecorator('wyCqzqdsj', {
                                        rules: [{ required: false, message: '请填写产权取得时间!' }]
                                    })(
                                        <DatePicker disabled={!canEdit} style={{ width: 180 }} ></DatePicker>
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
                                        <Input disabled={!canEdit} style={{ width: 180 }}></Input>
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
                                        <RadioGroup disabled={!canEdit} style={{ width: 180 }}>
                                            <Radio key='1' value={true}>是</Radio>
                                            <Radio key='0' value={false}>否</Radio>
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源付款方式</span>)}>
                                {
                                    getFieldDecorator('wyFyfkfs', {
                                        rules: [{ required: false, message: '请选择房源付款方式' }]
                                    })(
                                        <Select disabled={!canEdit} style={{ width: 80 }}>
                                            {
                                                payTypes.map(tp => <Option key={tp.key} value={tp.value}>{tp.key}</Option>)
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
                                        <InputNumber disabled={!canEdit} style={{ width: 180 }}></InputNumber>
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
                                        <InputNumber disabled={!canEdit}  style={{ width: 80 }}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>房源贷款金额</span>)}>
                                {
                                    getFieldDecorator('wyFydkje', {
                                        rules: [{ required: false }]
                                    })(
                                        <InputNumber disabled={!canEdit} precision={2} style={{ width: 120 }}></InputNumber>
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
                                        <InputNumber disabled={!canEdit} precision={2} style={{ width: 120 }}></InputNumber>
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
