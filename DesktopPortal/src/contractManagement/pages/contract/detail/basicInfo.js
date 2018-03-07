import { connect } from 'react-redux';
import { editBuildingBasic } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Table, Button, Checkbox, Row, Col, Form } from 'antd'
import moment from 'moment';

class BasicInfo extends Component {
    state = {

    }
    componentWillMount() {

    }

    handleEdit = (e) => {
        //this.props.dispatch(editBuildingBasic());
    }

    render(){
        const basicInfo = this.props.contractInfo.contractBasicInfo;
        if (basicInfo.landExpireDate && basicInfo.landExpireDate !== "") {
            basicInfo.landExpireDate = moment(basicInfo.landExpireDate).format("YYYY-MM-DD");
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
                                //[1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>合同编号:{contractId}</Col>
                        <Col span={12}>合同名称:{basicInfo.contractName}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>申请时间:{basicInfo.applyTime}</Col>
                        <Col span={12}>申请部门:{basicInfo.organizationName}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>合同类型:{basicInfo.contractType}</Col>
                        <Col span={12}>项目名称:{basicInfo.projectName}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>项目类型:{basicInfo.projectType}</Col>
                        <Col span={12}>项目负责人:{basicInfo.projectPeopleName}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>甲方类型:{basicInfo.firstPartyType}</Col>
                        <Col span={12}>甲方公司全称:{basicInfo.firstPartyFirmName}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>甲方负责人:{basicInfo.firstMainPeople}</Col>
                        <Col span={12}>乙方负责人:{basicInfo.secondMainPeople}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>合同开始时间:{basicInfo.beginTime}</Col>
                        <Col span={12}>合同结束时间:{basicInfo.endTime}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>份数:{basicInfo.contractNumber}</Col>
                        <Col span={12}>返回原件:{basicInfo.returnOrigin}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>佣金方式:{basicInfo.commissionType}</Col>
                        <Col span={12}>续签合同:{basicInfo.renewContract}</Col>
                    </Row>

                </Form>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        contractInfo: state.contractData.contractInfo,
        basicData: state.basicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BasicInfo);