//调佣详情搜索条件页面
import React, { Component } from 'react';
import { Row, Col, Button, Icon, DatePicker, Input, Select, Tooltip, Spin } from 'antd';
import './search.less'
import { connect } from 'react-redux';
import { getDicParList } from '../../actions/actionCreator'

class TyxqSearchCondition extends Component {

    state = {
        expandSearchCondition: true,
        isDataLoading: false
    }

    componentWillMount() {
        this.setState({ isDataLoading: true, tip: '信息初始化中...' })
        this.props.dispatch(getDicParList(['COMMISSION_JY_TYPE','COMMISSION_RP_STATE']));
    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false })
    }

    handleSearch = (e) => {
        this.props.searchCondition.pageSize = 10
        this.props.searchCondition.pageIndex = 1
        this.props.handleSearch(this.props.searchCondition)
    }
    handleReset = (e) => {
        this.props.searchCondition.examineStartTime = '';
        this.props.searchCondition.cjbh = '';
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
        else if (field === 'tysbDateStart') {
            this.props.searchCondition.tysbDateStart = e
        }
        else if (field === 'tysbDateEnd') {
            this.props.searchCondition.tysbDateEnd = e
        }
    }
    handleInput = (e, field) => {
        if (field === 'cjbh') {
            this.props.searchCondition.cjbh = e.target.value
        }
        else if (field === 'lry') {
            this.props.searchCondition.lry = e.target.value
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
        else if (field === 'fyblStart') {
            this.props.searchCondition.fyblStart = e
        }
        else if (field === 'fyblEnd') {
            this.props.searchCondition.fyblEnd = e
        }
        else if (field === 'status') {
            this.props.searchCondition.status = e
        }
    }
    render() {
        let tradeTypes = this.props.basicData.tradeTypes;
        let statusTypes = this.props.basicData.spTypes;

        return (
            <div className='searchCondition'>
                <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                    <div>
                        <Row className="normalInfo">
                            <Col span={12}>
                                <label><span style={{ marginRight: '10px' }}>申请日期：</span>
                                    <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateStart')} />- <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateEnd')} />
                                </label>
                            </Col>
                            <Col span={12}>
                                <label>
                                    <span style={{ marginRight: '10px' }}>成交编号</span>
                                    <Input style={{ width: 200 }} onChange={(e) => this.handleInput(e, 'cjbh')}></Input>
                                </label>
                            </Col>
                        </Row>
                        <Row className="normalInfo">
                            <Col span={12}>
                                <label style={{marginLeft:20}}>
                                    <span style={{ marginRight: '10px' }}>所属部门</span>
                                    <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'organizationId')}></Select>
                                </label>
                            </Col>
                            <Col span={4}>
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
                                    <span style={{ marginRight: '10px' }}>录入人</span>
                                    <Input style={{ width: 80 }} onChange={(e) => this.handleInput(e, 'lyr')}></Input>
                                </label>
                            </Col>
                        </Row>
                        <Row className="normalInfo">
                            <Col span={5}>
                                <label style={{marginLeft:40}}>
                                    <span style={{ marginRight: '10px' }}>状态</span>
                                    <Select style={{ width: 80 }} onChange={(e) => this.handleSelect(e, 'status')}>
                                        {
                                            statusTypes.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                        }
                                    </Select>
                                </label>
                            </Col>
                            <Col span={6}/>
                            <Col span={12}>
                                <label style={{marginLeft:15}}><span style={{ marginRight: '10px' }}>调佣上报日期：</span>
                                    <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'tysbDateStart')} />- <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'tysbDateEnd')} />
                                </label>
                            </Col>
                        </Row>
                        <Tooltip title="查询">
                            <Button type='primary' onClick={this.handleSearch} style={{ 'margin': '10' }} >查询</Button>
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
        searchCondition: state.fina.searchCondition
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(TyxqSearchCondition);