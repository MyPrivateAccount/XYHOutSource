import React, { Component } from 'react';
import { notification, Row, Col, Button, Icon, DatePicker, Input, Select, Tooltip, Spin, Form, TreeSelect } from 'antd';
import './search.less'
import { connect } from 'react-redux';
import { getDicParList } from '../../../actions/actionCreators'
import { dicKeys, permission, examineStatusMap } from '../../constants/const'
import { getDicPars, getOrganizationTree } from '../../../utils/utils'
import WebApiConfig from '../../constants/webApiConfig'
import ApiClient from '../../../utils/apiClient'

const FormItem = Form.Item;
const Option = Select.Option;

class DRpSearchCondition extends Component {

    state = {
        expandSearchCondition: true,
        isDataLoading: false,
        nodes: [],
        spZtList: [
            { key: '全部', value: null },
            { key: examineStatusMap[0], value: 0 },
            { key: examineStatusMap[1], value: 1 },
            { key: examineStatusMap[8], value: 8 },
            { key: examineStatusMap[16], value: 16 }
        ]
    }

    componentWillMount() {
        this.props.getDicParList([
            dicKeys.wyfl,
            dicKeys.cjbglx,
            dicKeys.jylx,

            dicKeys.fkfs,
            dicKeys.wylx,
            dicKeys.khly,
        ]);
        this.getNodes();
    }

    componentDidMount = () => {
        this.props.form.setFieldsValue({
            type: null,
            jylx: null,
            bswlfl: null,
            fkfs: null,
            kgxxly: null,
            wylx: null,
            examineStatus: null
        })
    }


    handleSearch = (e) => {
        let condition = this.props.form.getFieldsValue();
        if (this.props.search) {
            this.props.search(condition);
        }
    }
    handleExport = (e)=>{
        let condition = this.props.form.getFieldsValue();
        if (this.props.export) {
            this.props.export(condition);
        }
    }
    handleReset = (e) => {
        this.props.form.resetFields()
        this.props.form.setFieldsValue({
            type: null,
            jylx: null,
            bswlfl: null,
            fkfs: null,
            kgxxly: null,
            wylx: null,
            examineStatus: null
        })
    }

    handleSearchBoxToggle = (e) => {//筛选条件展开、收缩
        let visible = !this.state.expandSearchCondition;
        this.setState({ expandSearchCondition: visible });
    }

    getNodes = async () => {
        let url = `${WebApiConfig.org.permissionOrg}${permission.reportQuery}`;
        let r = await ApiClient.get(url, true);
        if (r && r.data && r.data.code === '0') {
            var nodes = getOrganizationTree(r.data.extension);
            this.setState({ nodes: nodes });
        } else {
            notification.error(`获取组织失败:${((r || {}).data || {}).message || ''}`);
        }
        this.getNoded = true;
    }


    render() {
        const { getFieldDecorator } = this.props.form;
        const { expandSearchCondition } = this.state;
        const allItem = { key: '全部', value: null };

        let bswyTypes = [...getDicPars(dicKeys.wyfl, this.props.dic)]; //报数物业类型
        bswyTypes.splice(0, 0, allItem)
        let cjbgTypes = [...getDicPars(dicKeys.cjbglx, this.props.dic)]; //成交报告类型
        cjbgTypes.splice(0, 0, allItem)
        let tradeTypes = [...getDicPars(dicKeys.jylx, this.props.dic)]; //交易类型
        tradeTypes.splice(0, 0, allItem)
        let payTypes = [...getDicPars(dicKeys.fkfs, this.props.dic)];  //付款方式
        payTypes.splice(0, 0, allItem)
        let wyWylxTypes = [...getDicPars(dicKeys.wylx, this.props.dic)]; //物业类型
        wyWylxTypes.splice(0, 0, allItem)
        // let cjTypes = getDicPars(dicKeys.wylx, this.props.dic);
        //let ghTypes = this.props.basicData.ghTypes;
        let khTypes = [...getDicPars(dicKeys.khly, this.props.dic)]; //客户类型
        khTypes.splice(0, 0, allItem)
        const spTypes = this.state.spZtList;
        return (
            <div className='searchCondition'>
                <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                    <Row>
                        <Col span={12}>

                        </Col>
                        <Col span={4}>
                            <Button onClick={this.handleSearchBoxToggle}>{expandSearchCondition ? "收起筛选" : "展开筛选"}<Icon type={expandSearchCondition ? "up-square-o" : "down-square-o"} /></Button>
                        </Col>
                    </Row>
                    <div style={{ display: expandSearchCondition ? "block" : "none" }}>
                        <Row className="form-row" >
                            <Col span={12} style={{ display: 'flex' }}>
                                <FormItem label="审批通过日期">
                                    {
                                        getFieldDecorator('examineStartTime')(
                                            <DatePicker />
                                        )
                                    }

                                </FormItem>
                                -
                    <FormItem>
                                    {
                                        getFieldDecorator('examineEndTime')(
                                            <DatePicker />
                                        )
                                    }

                                </FormItem>
                            </Col>
                            <Col span={6}>
                                <FormItem label="成交编号">
                                    {
                                        getFieldDecorator('cJBH')(
                                            <Input />
                                        )
                                    }

                                </FormItem>
                            </Col>
                            <Col span={6}>
                                <FormItem label="定金编号">
                                    {
                                        getFieldDecorator('dJBH')(
                                            <Input />
                                        )
                                    }

                                </FormItem>

                            </Col>
                            {/* <Col span={4}>
                                <label>
                                    <span style={{ marginRight: '10px' }}>片区</span>
                                    <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'pq')}></Input>
                                </label>
                            </Col> */}
                        </Row>
                        <Row className="form-row">
                            <Col span={12} style={{ display: 'flex' }}>
                                <FormItem label="上业绩日期">
                                    {
                                        getFieldDecorator('sYJRQStartTime')(
                                            <DatePicker />
                                        )
                                    }

                                </FormItem>
                                -
                    <FormItem>
                                    {
                                        getFieldDecorator('sYJRQEndTime')(
                                            <DatePicker />
                                        )
                                    }

                                </FormItem>


                            </Col>
                            <Col span={6}>
                                <FormItem label="所属部门">
                                    {getFieldDecorator('organizationId')(
                                        <TreeSelect
                                            dropdownStyle={{ maxHeight: 400, minWidth: 400, overflow: 'auto' }}
                                            treeData={this.state.nodes}
                                            placeholder="请选择所属部门"
                                        />
                                    )}
                                </FormItem>

                            </Col>
                            <Col span={6}>
                                <FormItem label="客户名称">
                                    {
                                        getFieldDecorator('customerName')(
                                            <Input />
                                        )
                                    }

                                </FormItem>

                            </Col>
                            {/* <Col span={4}>
                            </Col> */}

                        </Row>
                        <Row className="form-row">
                            <Col span={6}>
                                <FormItem label="报告类型">
                                    {
                                        getFieldDecorator('type')(

                                            <Select>
                                                {
                                                    cjbgTypes.map(x => (
                                                        <Option key={x.value} value={x.value}>{x.key}</Option>
                                                    ))
                                                }
                                            </Select>
                                        )
                                    }

                                </FormItem>

                            </Col>
                            <Col span={6}>
                                <FormItem label="交易类型">
                                    {
                                        getFieldDecorator('jylx')(

                                            <Select>
                                                {
                                                    tradeTypes.map(x => (
                                                        <Option key={x.value} value={x.value}>{x.key}</Option>
                                                    ))
                                                }
                                            </Select>
                                        )
                                    }

                                </FormItem>

                            </Col>
                            {/* <Col span={6}>

                                <label>
                                    <span style={{ marginRight: '10px' }}>成交状态</span>
                                    <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'cjzt')}>
                                        {
                                            cjTypes.map(tp => <Select.Option key={tp.key} value={tp.value}>{tp.key}</Select.Option>)
                                        }
                                    </Select>
                                </label>
                            </Col> */}
                            <Col span={6}>
                                <FormItem label="报数物业分类">
                                    {
                                        getFieldDecorator('bswlfl')(

                                            <Select>
                                                {
                                                    bswyTypes.map(x => (
                                                        <Option key={x.value} value={x.value}>{x.key}</Option>
                                                    ))
                                                }
                                            </Select>
                                        )
                                    }

                                </FormItem>

                            </Col>
                        </Row>
                        <Row className="form-row">

                            <Col span={6}>
                                <FormItem label="付款方式">
                                    {
                                        getFieldDecorator('fkfs')(

                                            <Select>
                                                {
                                                    payTypes.map(x => (
                                                        <Option key={x.value} value={x.value}>{x.key}</Option>
                                                    ))
                                                }
                                            </Select>
                                        )
                                    }

                                </FormItem>


                            </Col>
                            <Col span={6}>
                                <FormItem label="合同编号" style={{ flex: 1 }}>
                                    {
                                        getFieldDecorator('htbh')(
                                            <Input placeholder="合同编号" />
                                        )
                                    }

                                </FormItem>

                            </Col>
                            {/* <Col span={6}>

                                <label>
                                    <span style={{ marginRight: '10px' }}>过户状态</span>
                                    <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'ghzt')}>
                                        {
                                            ghTypes.map(tp => <Select.Option key={tp.key} value={tp.value}>{tp.key}</Select.Option>)
                                        }
                                    </Select>
                                </label>
                            </Col> */}
                            <Col span={6}>
                                <FormItem label="客户信息来源">
                                    {
                                        getFieldDecorator('kgxxly')(

                                            <Select>
                                                {
                                                    khTypes.map(x => (
                                                        <Option key={x.value} value={x.value}>{x.key}</Option>
                                                    ))
                                                }
                                            </Select>
                                        )
                                    }

                                </FormItem>

                            </Col>
                        </Row>
                        <Row className="form-row">
                            <Col span={6}>
                                <FormItem label="楼盘名称" style={{ flex: 1 }}>
                                    {
                                        getFieldDecorator('lpmc')(
                                            <Input placeholder="楼盘名称" />
                                        )
                                    }

                                </FormItem>

                            </Col>
                            <Col span={6}>
                                <FormItem label="楼盘栋数" style={{ flex: 1 }}>
                                    {
                                        getFieldDecorator('dz')(
                                            <Input placeholder="楼盘栋数" />
                                        )
                                    }

                                </FormItem>

                            </Col>
                            <Col span={6}>
                                <FormItem label="楼盘房号" style={{ flex: 1 }}>
                                    {
                                        getFieldDecorator('fh')(
                                            <Input placeholder="楼盘房号" />
                                        )
                                    }

                                </FormItem>


                            </Col>
                            <Col span={6}>
                                <FormItem label="物业类型">
                                    {
                                        getFieldDecorator('wylx')(

                                            <Select>
                                                {
                                                    wyWylxTypes.map(x => (
                                                        <Option key={x.value} value={x.value}>{x.key}</Option>
                                                    ))
                                                }
                                            </Select>
                                        )
                                    }

                                </FormItem>

                            </Col>
                        </Row>
                        <Row className="form-row">
                            <Col span={6} >
                                <FormItem label="录入人" >
                                    {
                                        getFieldDecorator('lrr')(
                                            <Input placeholder="录入人" />
                                        )
                                    }

                                </FormItem>

                            </Col>
                            <Col span={6}>
                                <FormItem label="成交人" >
                                    {
                                        getFieldDecorator('cjr')(
                                            <Input placeholder="成交人" />
                                        )
                                    }

                                </FormItem>

                            </Col>
                            <Col span={6}>
                                <FormItem label="审批状态">
                                    {
                                        getFieldDecorator('examineStatus')(

                                            <Select>
                                                {
                                                    spTypes.map(x => (
                                                        <Option key={x.value} value={x.value}>{x.key}</Option>
                                                    ))
                                                }
                                            </Select>
                                        )
                                    }

                                </FormItem>

                            </Col>
                            <Col span={6}>
                            </Col>
                        </Row>
                        <Row style={{display:'flex', justifyContent:'flex-end'}}>
                            <Tooltip title="查询">
                                <Button type='primary' onClick={this.handleSearch} style={{ 'margin': '5' }} >查询</Button>
                            </Tooltip>
                            <Tooltip title="重置">
                                <Button type='primary' onClick={this.handleReset} style={{ 'margin': '5' }} >重置</Button>
                            </Tooltip>
                            <Tooltip title="导出">
                                <Button type='primary' onClick={this.handleExport} style={{ 'margin': '5' }} >导出</Button>
                            </Tooltip>
                        </Row>
                    </div>
                </Spin>
            </div>
        )
    }
}
function MapStateToProps(state) {

    return {

        dic: state.basicData.dicList,
        // basicData: state.base,
        // operInfo: state.rp.operInfo,
        // ext: state.rp.ext,
        // searchCondition:state.rp.searchCondition
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch,
        getDicParList: (...args) => dispatch(getDicParList(...args))
    };
}


const WrapDRpSearchCondition = Form.create()(DRpSearchCondition)
export default connect(MapStateToProps, MapDispatchToProps)(WrapDRpSearchCondition);