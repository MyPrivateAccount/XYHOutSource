//交易合同组件
//合同列表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import moment from 'moment'
import { dealRpSave, syncYJDate } from '../../../actions/actionCreator'
import { notification, DatePicker, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect, Modal, InputNumber } from 'antd'
import { getDicPars, getOrganizationTree } from '../../../../utils/utils'
import validations from '../../../../utils/validations'
import { dicKeys, permission } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import './trade.less'
import ApiClient from '../../../../utils/apiClient'

const RadioGroup = Radio.Group;
const FormItem = Form.Item;
class TradeContract extends Component {
    state = {
        cjUserList:[],
        loading:false,
        tip:''
    }
    componentWillMount = () => {
        //   this.setState({isDataLoading:true,tip:'信息初始化中...'})
        //  this.props.dispatch(getDicParList(['COMMISSION_BSWY_CATEGORIES', 'COMMISSION_CJBG_TYPE', 'COMMISSION_JY_TYPE', 'COMMISSION_PAY_TYPE', 'COMMISSION_PROJECT_TYPE', 'COMMISSION_CONTRACT_TYPE', 'COMMISSION_OWN_TYPE', 'COMMISSION_TRADEDETAIL_TYPE', 'COMMISSION_SFZJJG_TYPE']));
    }
    componentDidMount = () => {
        this.getNodes();
    }

    componentWillReceiveProps = (nextProps) => {
        if (this.props.entity !== nextProps.entity && nextProps.entity) {

            var entity = nextProps.entity;

            let mv = {};
            Object.keys(entity).map(key => {
                mv[key] = entity[key];
            })

            if(entity.fyzId){
                this.getCjUserList(entity.fyzId, entity);
            }

           // mv.gsmc = { value: { key: entity.gsmc, label: entity.gsmcName } }
            this.props.form.setFieldsValue(mv);

            

            
        }
    }

    getNodes = async () => {
        let url = `${WebApiConfig.org.permissionOrg}${permission.cjfhxz}`;
        let r = await ApiClient.get(url, true);
        if (r && r.data && r.data.code === '0') {
            var nodes = getOrganizationTree(r.data.extension);
            this.setState({ nodes: nodes });
        } else {
            notification.error(`获取分行组织失败:${((r || {}).data || {}).message || ''}`);
        }
    }

    getCjUserList = async (id, entity)=>{
        let url = `${WebApiConfig.human.orgUser}`;
        let r = await ApiClient.get(url, true, { permissionId: permission.cjfhxz, branchId:id, pageSize: 0, pageIndex: 0 });
        r = (r||{}).data||{};

        let ul = [];
        if (r.code === '0') {
            ul = r.extension || [];
        } else {
            notification.error({ message: `获取成交用户列表失败:${((r || {}).data || {}).message || ''}`});
        }
        if(entity.cjrId && this.props.opType==='view'){
            let ou = ul.find(x=>x.id === entity.cjrId);
            if(!ou){
                ul.push({id:entity.cjrId, name: entity.cjrName||'' })
            }
        }
        this.setState({cjUserList: ul})
    }

    getValues = ()=>{
        this.props.form.validateFieldsAndScroll();
        var errors = this.props.form.getFieldsError();

        if (validations.checkErrors(errors)) {
            notification.error({ message: '验证失败', description: '表单验证失败，请检查' });
            return;
        }
        let values = this.props.form.getFieldsValue();
        return values;
    }

    handleSave = (e) => {
        let values = this.props.form.getFieldsValue();
        console.log(values)
        e.preventDefault();
        // this.props.form.validateFields((err, values) => {
        //     if (!err) {
        //         values.id = this.props.rpId;
        //         /*if(this.state.cjrq!==''){
        //             values.cjrq = this.state.cjrq;
        //         }
        //         else{
        //             values.cjrq = this.state.rpData.cjrq;
        //         }
        //         if(this.state.yxsqyrq!==''){
        //             values.yxsqyrq = this.state.yxsqyrq;
        //         }
        //         else{
        //             values.yxsqyrq = this.state.rpData.yxsqyrq;
        //         }
        //         if(this.state.yjfksj!==''){
        //             values.yjfksj = this.state.yjfksj;
        //         }
        //         else{
        //             values.yjfksj = this.state.rpData.yjfksj;
        //         }
        //         if(this.state.kflfrq!==''){
        //             values.kflfrq = this.state.kflfrq;
        //         }
        //         else{
        //             values.kflfrq = this.state.rpData.kflfrq;
        //         }
        //         if(this.state.htqyrq!==''){
        //             values.htqyrq = this.state.htqyrq;
        //         }
        //         else{
        //             values.htqyrq = this.state.rpData.htqyrq;
        //         }*/
        //         values.cjrq = values.cjrq.format('YYYY-MM-DD')
        //         values.yxsqyrq = values.yxsqyrq.format('YYYY-MM-DD')
        //         values.yjfksj = values.yjfksj.format('YYYY-MM-DD')
        //         values.kflfrq = values.kflfrq.format('YYYY-MM-DD')
        //         values.htqyrq = values.htqyrq.format('YYYY-MM-DD')

        //         console.log(values);
        //         this.setState({isDataLoading:true,tip:'保存信息中...'})
        //         this.props.dispatch(dealRpSave(values));
        //     }
        // });
    }
    cjrq_dateChange = (value, dateString) => {
        console.log(dateString)
        this.setState({ cjrq: dateString })
        this.props.dispatch(syncYJDate(dateString))//同步日期給业绩分配页面
    }
    wqrq_dateChange = (value, dateString) => {
        console.log(dateString)
        this.setState({ yxsqyrq: dateString })
    }
    yjfksj_dateChange = (value, dateString) => {
        this.setState({ yjfksj: dateString })
    }
    kflfrq_dateChange = (value, dateString) => {
        this.setState({ kflfrq: dateString })
    }
    htqyrq_dateChange = (value, dateString) => {
        this.setState({ htqyrq: dateString })
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
    //选择成交报备
    chooseReport = () => {
        if (this.props.showDialog) {
            this.props.showDialog(true);
        }
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const { entity, showBbSelector } = this.props;
        const {cjUserList, loading, tip} = this.state;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        // let bswyTypes = this.props.basicData.bswyTypes;
        // let cjbgTypes = this.props.basicData.cjbgTypes;
        // let tradeTypes = this.props.basicData.tradeTypes;
        // let projectTypes = this.props.basicData.projectTypes;
        // let tradeDetailTypes = this.props.basicData.tradeDetailTypes;
        // let ownTypes = this.props.basicData.ownTypes;
        // let payTypes = this.props.basicData.payTypes;
        // let contractTypes = this.props.basicData.contractTypes;
        // let sfzjjgTypes = this.props.basicData.sfzjjgTypes;
        let bswyTypes = getDicPars(dicKeys.wyfl, this.props.dic)
        let cjbgTypes = getDicPars(dicKeys.cjbglx, this.props.dic)
        let tradeTypes = getDicPars(dicKeys.jylx, this.props.dic)
        let projectTypes = getDicPars(dicKeys.xmlx, this.props.dic)
        let tradeDetailTypes = getDicPars(dicKeys.xxjylx, this.props.dic)
        let ownTypes = getDicPars(dicKeys.cqlx, this.props.dic)
        let payTypes = getDicPars(dicKeys.fkfs, this.props.dic)
        let contractTypes = getDicPars(dicKeys.htlx, this.props.dic)
        let sfzjjgTypes = getDicPars(dicKeys.zjjg, this.props.dic)

        

        return (
            <Layout>
                <div style={{ marginLeft: 12 }}>
                    {
                        showBbSelector ?
                            <Row>
                                <Col span={16} style={{display:'flex'}}>
                                    {/* <FormItem {...formItemLayout} label={(<span>成交报备</span>)}>
                                        {
                                            <Input readOnly disabled style={{ width: 200 }}></Input>
                                        }
                                    </FormItem> */}
                                    <Button onClick={this.chooseReport}>选择成交信息</Button>
                                </Col>
     
                            </Row> : null
                    }

                    <Spin spinning={loading} tip={tip}>
                        <Row>
                            <Col span={24} pull={4}>
                                <FormItem {...formItemLayout} label={(<span>报数物业分类</span>)}>
                                    {
                                        getFieldDecorator('bswylx', {
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup>
                                                {
                                                    bswyTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
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
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup>
                                                {
                                                    cjbgTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
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
                                        getFieldDecorator('gsmc')(
                                            <Select disabled  style={{ width: '15rem' }}>
                                                <Select.Option key={entity.gsmc} value={entity.gsmc}>{entity.gsmcName}</Select.Option>
                                            </Select>

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
                                            rules: [{ required: true, message: '请填写分行名称!' }]
                                        })(
                                            <TreeSelect
                                                style={{ width: 200 }}
                                                disabled={showBbSelector}
                                                dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                                treeData={this.state.nodes}
                                                placeholder="请选择分行"
                                            />
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>成交人</span>)}>
                                    {
                                        getFieldDecorator('cjrId', {
                                            rules: [{ required: true, message: '请填写成交人!' }]
                                        })(
                                            <Select disabled  style={{ width: '15rem' }}>
                                            {
                                                cjUserList.map(u=>(<Select.Option key={u.id} value={u.id}>{u.name}</Select.Option>) )
                                            }
                                                
                                            </Select>

                                           
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>成交日期</span>)}>
                                    {
                                        getFieldDecorator('cjrq', {
                                            rules: [{ required: true, message: '请选择成交日期!' }]
                                        })(
                                            <DatePicker onChange={this.props.cjrqChanged} style={{ width: 200 }} disabled={showBbSelector}></DatePicker>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>成交报告编号</span>)}>
                                    {
                                        getFieldDecorator('cjbgbh')(
                                            <Input disabled style={{ width: 200 }}></Input>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>附加说明</span>)}>
                                    {
                                        getFieldDecorator('fjsm', {
                                            rules: [{ required: false }]
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
                                            rules: [{ required: true, message: '请填写备注' }]
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
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup>
                                                {
                                                    tradeTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
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
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup>
                                                {
                                                    projectTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
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
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup>
                                                {
                                                    tradeDetailTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
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
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup>
                                                {
                                                    ownTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
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
                                            rules: [{ required: true, message: '请填写成交总价!' }]
                                        })(
                                            <InputNumber precision={2} style={{ width: 200 }}></InputNumber>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>佣金</span>)}>
                                    {
                                        getFieldDecorator('ycjyj', {
                                            rules: [{ required: true, message: '请填写佣金金额!' }]
                                        })(
                                            <InputNumber precision={2} style={{ width: 200 }}></InputNumber>
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
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup>
                                                {
                                                    payTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
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
                                            rules: [{ required: false }]
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
                                            rules: [{ required: false }]
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
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup>
                                                {
                                                    sfzjjgTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
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
                                            rules: [{ required: false }]                                        })(
                                            <DatePicker style={{ width: 200 }} onChange={this.kflfrq_dateChange}></DatePicker>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>合同签约日期</span>)}>
                                    {
                                        getFieldDecorator('htqyrq', {
                                            rules: [{ required: false }]                                        })(
                                            <DatePicker style={{ width: 200 }} onChange={this.htqyrq_dateChange}></DatePicker>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>合同类型</span>)}>
                                    {
                                        getFieldDecorator('htlx', {
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup>
                                                {
                                                    contractTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
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
                                            rules: [{ required: false }]
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
                                            rules: [{ required: false }]
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
                                            rules: [{ required: true, message: '请填写自制合同编号' }]
                                        })(
                                            <Input style={{ width: 200 }}></Input>
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
        //   basicData: state.base,
        // operInfo: state.rp.operInfo,
        // ext: state.rp.ext,
        // syncRpData: state.rp.syncRpData,
        // syncRpOp: state.rp.syncRpOp
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradeContract);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);