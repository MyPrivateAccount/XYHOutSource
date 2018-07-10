//交易合同组件
//合同列表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import moment from 'moment'
import { dealRpSave, syncYJDate } from '../../../actions/actionCreator'
import { notification, DatePicker, Form, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect, Modal, InputNumber } from 'antd'
import { getDicPars, getOrganizationTree } from '../../../../utils/utils'
import validations from '../../../../utils/validations'
import { dicKeys, permission } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import './trade.less'
import ApiClient from '../../../../utils/apiClient'

const RadioGroup = Radio.Group;
const FormItem = Form.Item;
class TradeContract extends Component {
    constructor(props){
        super(props)
        this.state = {
            cjUserList: [],
            loading: false,
            tip: ''
        }
        this.entity = {};
        this.getNoded= false;
    }
    
    componentWillMount = () => {
        
    }
    componentDidMount = () => {
        this.getNodes();
    }

    componentWillReceiveProps = (nextProps) => {
        if (this.props.entity !== nextProps.entity && nextProps.entity) {
            if(this.getNoded){
                this.initEntity(nextProps.entity)
            }
            


        }
    }

    initEntity=(entity)=>{
        if(!entity){
            return;
        }
        let mv = {};
        Object.keys(entity).map(key => {
            mv[key] = entity[key];
        })
        
        if (entity.fyzId && this.entity.fyzId!==entity.fyzId ) {
            this.getCjUserList(entity.fyzId, entity);
        }
        this.entity = entity;
        this.props.form.setFieldsValue(mv);


    }

    getNodes = async () => {
        let url = `${WebApiConfig.org.permissionOrg}${permission.cjfhxz}`;
        let r = await ApiClient.get(url, true);
        if (r && r.data && r.data.code === '0') {
            var nodes = getOrganizationTree(r.data.extension);
            this.setState({ nodes: nodes },()=>{
                this.initEntity(this.props.entity);
            });
        } else {
            notification.error(`获取分行组织失败:${((r || {}).data || {}).message || ''}`);
        }
        this.getNoded = true;
    }

    getCjUserList = async (id, entity) => {
        let url = `${WebApiConfig.human.orgUser}`;
        let r = await ApiClient.get(url, true, { permissionId: permission.cjfhxz, branchId: id, pageSize: 0, pageIndex: 0 });
        r = (r || {}).data || {};

        let ul = [];
        if (r.code === '0') {
            ul = r.extension || [];
        } else {
            notification.error({ message: `获取成交用户列表失败:${((r || {}).data || {}).message || ''}` });
        }
        if (entity.cjrId && this.props.opType === 'view') {
            let ou = ul.find(x => x.id === entity.cjrId);
            if (!ou) {
                ul.push({ id: entity.cjrId, name: entity.cjrName || '' })
            }
        }
        this.setState({ cjUserList: ul })
    }

    getValues = () => {
        this.props.form.validateFieldsAndScroll();
        var errors = this.props.form.getFieldsError();

        if (validations.checkErrors(errors)) {
            notification.error({ message: '验证失败', description: '表单验证失败，请检查' });
            return;
        }
        let values = this.props.form.getFieldsValue();
        return values;
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
        const { cjUserList, loading, tip } = this.state;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };

        let bswyTypes = getDicPars(dicKeys.wyfl, this.props.dic)
        let cjbgTypes = getDicPars(dicKeys.cjbglx, this.props.dic)
        let tradeTypes = getDicPars(dicKeys.jylx, this.props.dic)
        let projectTypes = getDicPars(dicKeys.xmlx, this.props.dic)
        let tradeDetailTypes = getDicPars(dicKeys.xxjylx, this.props.dic)
        let ownTypes = getDicPars(dicKeys.cqlx, this.props.dic)
        let payTypes = getDicPars(dicKeys.fkfs, this.props.dic)
        let contractTypes = getDicPars(dicKeys.htlx, this.props.dic)
        let sfzjjgTypes = getDicPars(dicKeys.zjjg, this.props.dic)
        let cqjybgj = getDicPars(dicKeys.sfxycqjybgj, this.props.dic);

        const canEdit = this.props.canEdit;


        return (
            <Layout>
                <div style={{ marginLeft: 12 }}>
                    {
                       ( showBbSelector && canEdit) ?
                            <Row>
                                <Col span={16} style={{ display: 'flex' }}>
                                    {/* <FormItem {...formItemLayout} label={(<span>成交报备</span>)}>
                                        {
                                            <Input readOnly disabled style={{ width: 200 }}></Input>
                                        }
                                    </FormItem> */}
                                    <Button onClick={this.chooseReport}>选择成交信息</Button>
                                </Col>

                            </Row> : null
                    }
                        {
                            getFieldDecorator('cjbbId')(<Input type="hidden"/>)
                            
                        }
                        {getFieldDecorator('lpId')(<Input type="hidden"/>)}
                        {getFieldDecorator('spId')(<Input type="hidden"/>)}
                    <Spin spinning={loading} tip={tip}>
                        <Row className="form-row">
                            <Col span={24}>
                                <FormItem {...formItemLayout} label={(<span>报数物业分类</span>)}>
                                    {
                                        getFieldDecorator('bswylx', {
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup disabled={!canEdit}>
                                                {
                                                    bswyTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                                }
                                            </RadioGroup>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <Row className="form-row">
                            <Col span={24}>
                                <FormItem {...formItemLayout} label={(<span>成交报告类型</span>)}>
                                    {
                                        getFieldDecorator('cjbglx', {
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup disabled={!canEdit}>
                                                {
                                                    cjbgTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                                }
                                            </RadioGroup>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <Row className="form-row">
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>公司名称</span>)}>
                                    {
                                        getFieldDecorator('gsmc')(
                                            <Select disabled style={{ width: '15rem' }}>
                                                <Select.Option key={entity.gsmc} value={entity.gsmc}>{entity.gsmcName}</Select.Option>
                                            </Select>

                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>成交报告编号</span>)}>
                                    {
                                        getFieldDecorator('cjbgbh')(
                                            <Input disabled/>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <Row className="form-row">
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>分行名称</span>)}>
                                    {
                                        getFieldDecorator('fyzId', {
                                            rules: [{ required: true, message: '请填写分行名称!' }]
                                        })(
                                            <TreeSelect
                                                style={{ width: 200 }}
                                                disabled={showBbSelector || !canEdit}
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
                                            <Select disabled style={{ width: '15rem' }}>
                                                {
                                                    cjUserList.map(u => (<Select.Option key={u.id} value={u.id}>{u.name}</Select.Option>))
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
                                            <DatePicker   onChange={(v)=> this.props.inputChanged('cjrq',v)}  style={{ width: 200 }} disabled={showBbSelector || !canEdit}></DatePicker>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                
                        <Row className="form-row">
                            <Col span={24}>
                                <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                                    {
                                        getFieldDecorator('bz', {
                                            rules: [{ required: true, message: '请填写备注' }]
                                        })(
                                            <Input.TextArea rows={4} disabled={!canEdit}></Input.TextArea>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <div className="divider"></div>
                        <Row className="form-row">
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>交易类型</span>)}>
                                    {
                                        getFieldDecorator('jylx', {
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup disabled={!canEdit}>
                                                {
                                                    tradeTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                                }
                                            </RadioGroup>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>项目类型</span>)}>
                                    {
                                        getFieldDecorator('xmlx', {
                                            rules: [{ required: false }]
                                        })(
                                            <Select disabled={!canEdit}>
                                                {
                                                    projectTypes.map(tp => <Select.Option key={tp.key} value={tp.value}>{tp.key}</Select.Option>)
                                                }
                                            </Select>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>

                        <Row className="form-row">
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>详细交易类型</span>)}>
                                    {
                                        getFieldDecorator('xxjylx', {
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup disabled={!canEdit}>
                                                {
                                                    tradeDetailTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                                }
                                            </RadioGroup>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>产权类型</span>)}>
                                    {
                                        getFieldDecorator('cqlx', {
                                            rules: [{ required: false }]
                                        })(
                                            <Select disabled={!canEdit}>
                                                {
                                                    ownTypes.map(tp => <Select.Option key={tp.key} value={tp.value}>{tp.key}</Select.Option>)
                                                }
                                            </Select>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>

                        <Row className="form-row">
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>成交总价</span>)}>
                                    {
                                        getFieldDecorator('cjzj', {
                                            rules: [{ required: true, message: '请填写成交总价!' }]
                                        })(
                                            <InputNumber disabled={!canEdit} onChange={(v)=> this.props.inputChanged('cjzj',v)} precision={2} style={{ width: 200 }}></InputNumber>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>佣金</span>)}>
                                    {
                                        getFieldDecorator('ycjyj', {
                                            rules: [{ required: true, message: '请填写佣金金额!' }]
                                        })(
                                            <InputNumber disabled={!canEdit} onChange={(v)=> this.props.inputChanged('yj',v)} precision={2} style={{ width: 200 }}></InputNumber>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <Row className="form-row">
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>付款方式</span>)}>
                                    {
                                        getFieldDecorator('fkfs', {
                                            rules: [{ required: false }]
                                        })(
                                            <Select disabled={!canEdit}>
                                                {
                                                    payTypes.map(tp => <Select.Option key={tp.key} value={tp.value}>{tp.key}</Select.Option>)
                                                }
                                            </Select>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={12}>
                                <FormItem className="auto-width" {...formItemLayout} label={(<span>是否需要产权交易部跟进</span>)}>
                                    {
                                        getFieldDecorator('sfxcqjybgj', {
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup disabled={!canEdit}>
                                                {
                                                    cqjybgj.map(tp => <Radio key={tp.key} value={tp.value*1}>{tp.key}</Radio>)
                                                }
                                            </RadioGroup>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <Row className="form-row">
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>预计放款日期</span>)}>
                                    {
                                        getFieldDecorator('yjfksj', {
                                            rules: [{ required: false }],
                                        })(
                                            <DatePicker disabled={!canEdit} style={{ width: 200 }} onChange={this.yjfksj_dateChange}></DatePicker>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>预计放款金额</span>)}>
                                    {
                                        getFieldDecorator('yjfkje', {
                                            rules: [{ required: false }]
                                        })(
                                            <Input disabled={!canEdit} style={{ width: 200 }}></Input>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>

                        <Row className="form-row">
                            <Col span={12}>
                                <FormItem {...formItemLayout} label={(<span>网签日期</span>)}>
                                    {
                                        getFieldDecorator('yxsqyrq', {
                                            rules: [{ required: false }]
                                        })(
                                            <DatePicker disabled={!canEdit} style={{ width: 200 }} onChange={this.wqrq_dateChange}></DatePicker>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={12} >
                                <FormItem {...formItemLayout} label={(<span>是否资金监管</span>)}>
                                    {
                                        getFieldDecorator('sfzjjg', {
                                            rules: [{ required: false }]
                                        })(
                                            <RadioGroup disabled={!canEdit}>
                                                {
                                                    sfzjjgTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                                }
                                            </RadioGroup>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <div className="divider"></div>
                        <Row className="form-row">
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>客户来访日期</span>)}>
                                    {
                                        getFieldDecorator('khlfrq', {
                                            rules: [{ required: false }]
                                        })(
                                            <DatePicker disabled={!canEdit} style={{ width: 200 }} onChange={this.kflfrq_dateChange}></DatePicker>
                                        )
                                    }
                                </FormItem>
                            </Col>
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>合同签约日期</span>)}>
                                    {
                                        getFieldDecorator('htqyrq', {
                                            rules: [{ required: false }]
                                        })(
                                            <DatePicker disabled={!canEdit} style={{ width: 200 }} onChange={this.htqyrq_dateChange}></DatePicker>
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
                                            <RadioGroup disabled={!canEdit}>
                                                {
                                                    contractTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                                }
                                            </RadioGroup>
                                        )
                                    }
                                </FormItem>
                            </Col>
                        </Row>
                        <Row className="form-row">
                            <Col span={8}>
                                <FormItem {...formItemLayout} label={(<span>资金监管协议编号</span>)}>
                                    {
                                        getFieldDecorator('zjjgxybh', {
                                            rules: [{ required: false }]
                                        })(
                                            <Input disabled={!canEdit} style={{ width: 200 }}></Input>
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
                                            <Input  disabled={!canEdit} style={{ width: 200 }}></Input>
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
                                            <Input disabled={!canEdit} style={{ width: 200 }}></Input>
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