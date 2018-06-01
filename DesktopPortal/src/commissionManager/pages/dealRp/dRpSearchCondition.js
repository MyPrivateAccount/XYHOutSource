import React, { Component } from 'react';
import { Row, Col, Button, Icon, DatePicker, Input, Select, Tooltip ,Spin} from 'antd';
import './search.less'
import { connect } from 'react-redux';
import { getDicParList } from '../../actions/actionCreator'

class DRpSearchCondition extends Component {

    state = {
        expandSearchCondition: false,
        isDataLoading: false
    }

    componentWillMount() {
        this.setState({ isDataLoading: true, tip: '信息初始化中...' })
        this.props.dispatch(getDicParList(['COMMISSION_BSWY_CATEGORIES', 'COMMISSION_CJBG_TYPE', 'COMMISSION_JY_TYPE', 'COMMISSION_PAY_TYPE', 'COMMISSION_PROJECT_TYPE', 'COMMISSION_CONTRACT_TYPE', 'COMMISSION_OWN_TYPE', 'COMMISSION_TRADEDETAIL_TYPE', 'COMMISSION_SFZJJG_TYPE']));
    }
    componentWillReceiveProps(newProps){
        this.setState({isDataLoading:false})
    }

    handleSearch = (e) => {
        this.props.searchCondition.pageSize = 10
        this.props.searchCondition.pageIndex = 1
        this.props.handleSearch(this.props.searchCondition)
    }
    handleReset = (e) => {

    }

    handleSearchBoxToggle = (e) => {//筛选条件展开、收缩
        let visible = !this.state.expandSearchCondition;
        this.setState({ expandSearchCondition: visible });
    }
    handleCreateTime = (e, field) => {
        if (field === 'createDateStart') {
            this.props.searchCondition.examineStartTime = e
        }
        else if (field === 'createDateEnd') {
            this.props.searchCondition.examineEndTime = e
        }
        else if (field === 'syjrqStartTime') {
            this.props.searchCondition.syjrqStartTime = e
        }
        else if (field === 'syjrqEndTime') {
            this.props.searchCondition.syjrqEndTime = e
        }
    }
    handleInput = (e, field) => {
        if (field === 'cjbh') {
            this.props.searchCondition.cjbh = e.target.value
        }
        else if (field === 'djbh') {
            this.props.searchCondition.djbh = e.target.value
        }
        else if (field === 'customerName') {
            this.props.searchCondition.customerName = e.target.value
        }
        else if (field === 'htbh') {
            this.props.searchCondition.htbh = e.target.value
        }
        else if (field === 'pq') {
            this.props.searchCondition.pq = e.target.value
        }
        else if (field === 'lpmc') {
            this.props.searchCondition.lpmc = e.target.value
        }
        else if (field === 'dz') {
            this.props.searchCondition.dz = e.target.value
        }
        else if (field === 'fh') {
            this.props.searchCondition.fh = e.target.value
        }
        else if (field === 'lrr') {
            this.props.searchCondition.lrr = e.target.value
        }
    }
    handleSelect = (e, field) => {

        if (field === 'organizationId') {
            this.props.searchCondition.organizationId = e
        }
        else if (field === 'type') {
            this.props.searchCondition.type = e
        }
        else if (field === 'jylx') {
            this.props.searchCondition.jylx = e
        }
        else if (field === 'cjzt') {
            this.props.searchCondition.cjzt = e
        }
        else if (field === 'bswlfl') {
            this.props.searchCondition.bswlfl = e
        }
        else if (field === 'fkfs') {
            this.props.searchCondition.fkfs = e
        }
        else if (field === 'ghzt') {
            this.props.searchCondition.ghzt = e
        }
        else if (field === 'kgxxly') {
            this.props.searchCondition.kgxxly = e
        }
        else if (field === 'wylx') {
            this.props.searchCondition.wylx = e
        }
        else if (field === 'examineStatus') {
            this.props.searchCondition.examineStatus = e
        }
    }
    render() {
        let expandSearchCondition = this.state.expandSearchCondition;
        let bswyTypes = this.props.basicData.bswyTypes;
        let cjbgTypes = this.props.basicData.cjbgTypes;
        let tradeTypes = this.props.basicData.tradeTypes;
        let payTypes = this.props.basicData.payTypes;
        let wyWylxTypes = this.props.basicData.wyWylxTypes;
        let cjTypes = [{key:'未成交',value:1},{key:'已成交',value:2}]
        let ghTypes = [{key:'未过户',value:1},{key:'已过户',value:2}]
        let khTypes = [{key:'推荐',value:1},{key:'其它',value:2}]
        let spTypes = [{key:'通过',value:1},{key:'未通过',value:2}]
        return (
            <div className='searchCondition'>
                <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                <Row>
                    <Col span={12}>
                        <span>所有报告></span>
                    </Col>
                    <Col span={4}>
                        <Button onClick={this.handleSearchBoxToggle}>{expandSearchCondition ? "收起筛选" : "展开筛选"}<Icon type={expandSearchCondition ? "up-square-o" : "down-square-o"} /></Button>
                    </Col>
                </Row>
                <div style={{ display: expandSearchCondition ? "block" : "none" }}>
                    <Row className="normalInfo">
                        <Col span={12}>
                            <label><span style={{ marginRight: '10px' }}>审批通过日期：</span>
                                <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateStart')} />- <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateEnd')} />
                            </label>
                        </Col>
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }}>成交编号</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'cjbh')}></Input>
                            </label>
                        </Col>
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }} >定金编号</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'djbh')}></Input>
                            </label>
                        </Col>
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }}>片区</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'pq')}></Input>
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }}>所属部门</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'organizationId')}></Select>
                            </label>
                        </Col>
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }}>客户名称</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'customerName')}></Input>
                            </label>
                        </Col>
                        <Col span={4}>
                        </Col>
                        <Col span={12}>
                            <label><span style={{ marginRight: '10px' }}>上业绩日期：</span>
                                <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'syjrqStartTime')} />- <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'syjrqEndTime')} />
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>报告类型</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'type')}>
                                    {
                                        cjbgTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                    }
                                </Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>交易类型</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'jylx')}>
                                    {
                                        tradeTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                    }
                                </Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>成交状态</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'cjzt')}>
                                    {
                                        cjTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                    }
                                </Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>报数物业分类</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'bswlfl')}>
                                    {
                                        bswyTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                    }
                                </Select>
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>付款方式</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'fkfs')}>
                                    {
                                        payTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                    }
                                </Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>合同编号</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'htbh')}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>过户状态</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'ghzt')}>
                                    {
                                        ghTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                    }
                                </Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>客户信息来源</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'kgxxly')}>
                                    {
                                        khTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                    }
                                </Select>
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>楼盘名称</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'lpmc')}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>楼盘栋数</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'dz')}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>楼盘房号</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'fh')}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>物业类型</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'wylx')}>
                                    {
                                        wyWylxTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                    }
                                </Select>
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={6} style={{ marginLeft: 10 }}>
                            <label>
                                <span style={{ marginRight: '10px' }}>录入人</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'lrr')}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>成交人</span>
                                <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'cjr')}></Input>
                            </label>
                        </Col>
                        <Col span={6} style={{ marginLeft: -10 }}>
                            <label>
                                <span style={{ marginRight: '10px' }}>审批状态</span>
                                <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'examineStatus')}>
                                    {
                                        spTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                    }
                                </Select>
                            </label>
                        </Col>
                        <Col span={6}>
                        </Col>
                    </Row>
                    <Tooltip title="查询">
                        <Button type='primary' onClick={this.handleSearch} style={{ 'margin': '10' }} >查询</Button>
                    </Tooltip>
                    <Tooltip title="重置">
                        <Button type='primary' onClick={this.handleReset} style={{ 'margin': '10' }} >重置</Button>
                    </Tooltip>
                </div>
                </Spin>
            </div>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        operInfo: state.rp.operInfo,
        ext: state.rp.ext,
        searchCondition:state.rp.searchCondition
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(DRpSearchCondition);