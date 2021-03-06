import { connect } from 'react-redux';
import { editContractBasic, openModifyHistory, openFollowHistoryDialog } from '../../../actions/actionCreator';
import ModifyHistory from '../../dialog/modifyHistory';
import FollowHistory from '../../dialog/followHistory';
import React, { Component } from 'react'
import { Icon, Table, Button, Checkbox, Row, Col, Form } from 'antd'
import moment from 'moment';

class BasicInfo extends Component {
    state = {

    }
    componentWillMount() {

    }

    handleEdit = (e) => {
        e.preventDefault();
        this.props.dispatch(editContractBasic());
    }
    getTextByCode(dic, code) {
        let text = code;
        let isArray = Array.isArray(code);
        if (!dic || !code) {
            return "";
        }
        dic.map(item => {
            if (isArray) {
                if (code.find((c) => c === item.value)) {
                    text = item.key;
                }
            } else {
                if (item.value === code) {
                    text = item.key;
                }
            }
        });
        return text;
    }

    handleViewModifyHistory = ()=>{
        this.props.dispatch(openModifyHistory())
    }
    handleViewFollowHistory = () =>{
        this.props.dispatch(openFollowHistoryDialog())
    }
    render(){
        const basicInfo = this.props.contractInfo.baseInfo;
        if (basicInfo.startTime && basicInfo.startTime !== "") {
            basicInfo.startTime = moment(basicInfo.startTime).format("YYYY-MM-DD");
        }
        if (basicInfo.endTime && basicInfo.endTime !== "") {
            basicInfo.endTime = moment(basicInfo.endTime).format("YYYY-MM-DD");
        }
        if (basicInfo.createTime && basicInfo.createTime !== "") {
            basicInfo.createTime = moment(basicInfo.createTime).format("YYYY-MM-DD");
        }
        const contractId = basicInfo.id;
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal">
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>基础信息</span>
                        </Col>
                        <Col span={4}>
                            {
                               [1].includes(basicInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                //<Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>合同编号:{basicInfo.num}</Col>
                        
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>合同类型:{this.getTextByCode(this.props.basicData.contractCategories, basicInfo.type) }</Col>
                        <Col span={12}>申请人:{basicInfo.ext1}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>申请时间:{basicInfo.createTime}</Col>
                        <Col span={12}>申请部门:{basicInfo.organizate}</Col>
                    </Row>
                    <Row className='viewRow'>   
                        <Col span={12}>合同名称:{basicInfo.name}</Col>
                        <Col span={12}>项目名称:{basicInfo.projectName}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>项目类型:{this.getTextByCode(this.props.basicData.contractProjectCatogories, basicInfo.projectType) }</Col>
                        <Col span={12}>项目负责人:{basicInfo.proprincipalPepole}</Col>
                    </Row>
                    <Row className='viewRow'>
                        {/* //甲方类型:{this.getTextByCode(this.props.basicData.firstPartyCatogories, basicInfo.companyAT)}</Col> */}
                        <Col span={12}>甲方类型:{this.getTextByCode(this.props.basicData.firstPartyCatogories, basicInfo.companyAT)}</Col>
                        <Col span={12}>甲方公司全称:{basicInfo.companyA}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>甲方负责人:{basicInfo.principalpepoleA}</Col>
                        <Col span={12}>乙方负责人:{basicInfo.principalpepoleB}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>合同开始时间:{basicInfo.startTime}</Col>
                        <Col span={12}>合同结束时间:{basicInfo.endTime}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>份数:{basicInfo.count}</Col>
                        <Col span={12}>返回原件:{basicInfo.returnOrigin === 1 ? "是" : '否'}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>佣金方式:{this.getTextByCode(this.props.basicData.commissionCatogories, basicInfo.commisionType)}</Col>
                        <Col span={12}>续签合同:{basicInfo.follow}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>是否提交铺号:{basicInfo.isSubmmitShop ? '是' : '否'}</Col>
                        <Col span={12}>是否提交关系证明:{basicInfo.isSubmmitRelation ? '是' : '否'}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>结算方式:{basicInfo.settleaccounts}</Col>
                        <Col span={12}>佣金方案:{basicInfo.commission}</Col>
                    </Row>
                    {/*
                    <Row className='viewRow'>
                        <Col span={12}>审核人:{basicInfo.checkPeople}</Col>
                        <Col span={12}>审核状态:{basicInfo.checkState}</Col>
                    </Row>
                    */}
                    <Row className='viewRow'>
                        {/* <Col span={12}>是否作废:{(basicInfo.discard && basicInfo.discard === '1') ? "是" : "否"}</Col> */}
                        <Col span={12}>项目地址:{basicInfo.projectAddress}</Col>
                        <Col span={2} onClick={this.handleViewModifyHistory} style={{color:'blue'}} title="点击获取更改记录">更改记录</Col>
                        <Col span={2} onClick={this.handleViewFollowHistory} style={{color:'blue'}} title="点击获取续签记录">续签记录</Col>
                    </Row>
                    <Row className='viewRow'>
                        
                        <Col span={12}>备注:{basicInfo.remark}</Col>   
                    </Row>
                    {/* <Row className='viewRow'>
                        <Col span={24}>补充协议:{this.props.complementInfo.contentInfo}</Col>   
                    </Row> */}
                </Form>
                <ModifyHistory />
                <FollowHistory />
            </div>
        )
    }
}

function mapStateToProps(state) {

    return {
        contractInfo: state.contractData.contractInfo,
        basicData: state.basicData,
        complementInfo: state.contractData.contractInfo.complementInfo,
        
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BasicInfo);