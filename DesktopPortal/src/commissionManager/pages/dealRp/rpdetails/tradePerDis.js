//业绩分配组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import moment from 'moment'
import { dealFpSave, acmentParamListGet ,searchHuman} from '../../../actions/actionCreator'
import { DatePicker, notification, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradeWyTable from './tradeWyTable'
import TradeNTable from './tradeNTable'
import SearchCondition from '../../../constants/searchCondition'

const FormItem = Form.Item;
class TradePerDis extends Component {

    constructor(props) {
        super(props);
        this.state = {
            isDataLoading: false,
            rpData: {
                yjYzys: 0,
                yjKhys: 0,
                reportInsides:[],
                reportOutsides:[]

            },
            totalyj: 0,
            yjKhyjdqr: '',
            yjYzyjdqr: '',
            yjFtItems: [],//业绩分摊项
            humanList:[],//员工列表
        }
        this.handleYzchange = this.handleYzchange.bind(this)
        this.handleKhchange = this.handleKhchange.bind(this)
    }
    componentWillMount = () => {
    }
    componentDidMount = () => {
        SearchCondition.acmentListCondition.branchId = this.props.branchId;
        this.props.dispatch(acmentParamListGet(SearchCondition.acmentListCondition));

        SearchCondition.humanListCondition.Organizate = this.props.branchId;
        this.props.dispatch(searchHuman(SearchCondition.humanListCondition))
    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false });
        if (newProps.operInfo.operType === 'FPSAVE_UPDATE') {
            notification.success({
                message: '提示',
                description: '保存成交报告业绩分配信息成功!',
                duration: 3
            });
            newProps.operInfo.operType = ''
        }
        else if (newProps.operInfo.operType === 'FPGET_UPDATE') {//信息获取成功
            if (JSON.stringify(newProps.ext) !== '[]') {
                this.setState({ rpData: newProps.ext },()=>{
                    this.reCountZyj()
                });
                
            }
            newProps.operInfo.operType = ''
        }
        else if (newProps.operInfo.operType === 'DEALRP_SYNC_DATE') {

            if (newProps.syncData !== '') {
                console.log('开始同步日期')
                let rpData = { ...this.state.rpData }
                rpData.yjYzyjdqr = moment(newProps.syncData).add(180, 'days').format('YYYY-MM-DD')
                rpData.yjKhyjdqr = moment(newProps.syncData).add(180, 'days').format('YYYY-MM-DD')
                this.setState(rpData)
                this.props.form.setFieldsValue({ 'yjYzyjdqr': moment(newProps.syncData).add(180, 'days') })
                this.props.form.setFieldsValue({ 'yjKhyjdqr': moment(newProps.syncData).add(180, 'days') })
            }
            newProps.operInfo.operType = ''
        }
        else if (newProps.syncFpOp.operType === 'DEALRP_SYNC_FP') {
            let newdata = newProps.syncFpData
            if(JSON.stringify(newdata)==='{}'){
                newdata.yjYzys = 0
                newdata.yjKhys = 0
                newdata.yjYzyjdqr=moment().add(180,'days').format('YYYY-MM-DD')
                newdata.yjKhyjdqr=moment().add(180,'days').format('YYYY-MM-DD')
                this.props.form.setFieldsValue({ 'yjYzys': 0})
                this.props.form.setFieldsValue({ 'yjKhys': 0 })
                this.props.form.setFieldsValue({ 'yjYzyjdqr': moment(newdata.yjYzyjdqr) })
                this.props.form.setFieldsValue({ 'yjKhyjdqr': moment(newdata.yjKhyjdqr) })
                this.setState({ rpData:newdata }, () => this.reCountZyj())
            }
            else{
                let rpData = { ...this.state.rpData }
                rpData.yjYzys = parseFloat(newdata.yjYzys, 10)
                rpData.yjKhys = 0
                this.setState({ rpData }, () => this.reCountZyj())
                this.props.form.setFieldsValue({ 'yjYzys': parseFloat(newdata.yjYzys) })
                this.props.form.setFieldsValue({ 'yjKhys': 0 })
                this.props.form.setFieldsValue({ 'yjYzyjdqr': moment(newdata.yjYzyjdqr) })
                this.props.form.setFieldsValue({ 'yjKhyjdqr': moment(newdata.yjKhyjdqr) })
            }
            
            newProps.syncFpOp.operType = ''
        }
        else if (newProps.acmOperInfo.operType === 'ACMENT_PARAM_LIST_UPDATE') {
            this.setState({ yjFtItems: newProps.scaleSearchResult.extension},()=>{
                this.wytb.setKxlxItems(this.getWyItems())
                this.fptb.setNbfpItems(this.getInnerItems())
            })
            newProps.acmOperInfo.operType = ''
        }
        else if (newProps.humanOperInfo.operType === 'SEARCH_HUMAN_INFO_SUCCESS') {
            this.setState({ humanList: newProps.humanList},()=>{
                this.fptb.setHumanList(newProps.humanList)
            })
            newProps.acmOperInfo.operType = ''
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

                if (this.state.yjYzyjdqr !== '') {
                    values.yjYzyjdqr = this.state.yjYzyjdqr;
                }
                else {
                    values.yjYzyjdqr = this.state.rpData.yjYzyjdqr;
                }
                if (this.state.yjKhyjdqr !== '') {
                    values.yjKhyjdqr = this.state.yjKhyjdqr;
                }
                else {
                    values.yjKhyjdqr = this.state.rpData.yjKhyjdqr;
                }

                console.log(values);
                this.setState({ isDataLoading: true, tip: '保存信息中...' })
                this.props.dispatch(dealFpSave(values))
            }
        });
    }
    handleAddWy = (e) => {
        e.preventDefault();
        this.wytb.handleAdd();
    }
    handleAddNbFp = (e) => {
        e.preventDefault();
        this.fptb.handleAdd();
    }
    onWyTableRef = (ref) => {
        this.wytb = ref
    }
    onFpTableRef = (ref) => {
        this.fptb = ref
    }
    //获取外部分摊项
    getWyItems = () => {
        let allItems = this.state.yjFtItems
        let wyItems = []
        for (let i = 0; i < allItems.length; i++) {
            if (allItems[i].type === 1) {
                wyItems.push(allItems[i])
            }
        }
        return wyItems
    }
    //获取内部分摊项
    getInnerItems = () => {
        let allItems = this.state.yjFtItems
        let innerItems = []
        for (let i = 0; i < allItems.length; i++) {
            if (allItems[i].type === 2) {
                innerItems.push(allItems[i])
            }
        }
        return innerItems
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
    yjYzyjdqr_dateChange = (value, dateString) => {
        console.log(dateString)
        this.setState({ yjYzyjdqr: dateString })
    }
    yjKhyjdqr_dateChange = (value, dateString) => {
        console.log(dateString)
        this.setState({ yjKhyjdqr: dateString })
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
                                        <DatePicker disabled={true} style={{ width: 200 }} onChange={this.yjYzyjdqr_dateChange}></DatePicker>
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
                                        <DatePicker disabled={true} style={{ width: 200 }} onChange={this.yjKhyjdqr_dateChange}></DatePicker>
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
                                        initialValue: this.state.totalyj
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
                        <TradeWyTable onWyTableRef={this.onWyTableRef} totalyj={this.state.yjZcjyj} onCountJyj={this.reCountJyj} branchId={"1"} dataSource={this.state.rpData.reportInsides}/>
                    </Row>
                    <Row style={{ margin: 10, marginLeft: -30 }}>
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
                        <TradeNTable onFpTableRef={this.onFpTableRef} branchId={"1"} dataSource={this.state.rpData.reportOutsides}/>
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
        ext: state.rp.ext,
        acmOperInfo: state.acm.operInfo,
        syncData: state.rp.syncData,
        syncFpOp: state.rp.syncFpOp,
        syncFpData: state.rp.syncFpData,
        scaleSearchResult:state.acm.scaleSearchResult,
        humanList:state.org.humanList,
        humanOperInfo:state.org.operInfo

    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradePerDis);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
