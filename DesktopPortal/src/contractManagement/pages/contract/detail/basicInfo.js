import { connect } from 'react-redux';
import { editContractBasic } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Table, Button, Checkbox, Row, Col, Form } from 'antd'
import moment from 'moment';

class BasicInfo extends Component {
    state = {

    }
    componentWillMount() {

    }

    handleEdit = (e) => {
        this.props.dispatch(editContractBasic());
    }

    render(){
        const basicInfo = this.props.contractInfo.contractBasicInfo;
        if (basicInfo.StartTime && basicInfo.StartTime !== "") {
            basicInfo.StartTime = moment(basicInfo.StartTime).format("YYYY-MM-DD");
        }
        if (basicInfo.EndTime && basicInfo.EndTime !== "") {
            basicInfo.EndTime = moment(basicInfo.EndTime).format("YYYY-MM-DD");
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
                        <Col span={12}>合同类型:{basicInfo.ContractType }</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>申请时间:{basicInfo.CreateTime}</Col>
                        <Col span={12}>申请部门:{basicInfo.CreateDepartment}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>合同名称:{basicInfo.Name}</Col>
                        <Col span={12}>项目名称:{basicInfo.ProjectName}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>项目类型:{basicInfo.ProjectType}</Col>
                        <Col span={12}>项目负责人:{basicInfo.ProprincipalPepole}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>甲方类型:{basicInfo.CompanyAType}</Col>
                        <Col span={12}>甲方公司全称:{basicInfo.CompanyA}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>甲方负责人:{basicInfo.PrincipalpepoleA}</Col>
                        <Col span={12}>乙方负责人:{basicInfo.PrincipalpepoleB}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>合同开始时间:{basicInfo.StartTime}</Col>
                        <Col span={12}>合同结束时间:{basicInfo.EndTime}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>份数:{basicInfo.Count}</Col>
                        <Col span={12}>返回原件:{basicInfo.ReturnOrigin}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>佣金方式:{basicInfo.CommisionType}</Col>
                        <Col span={12}>续签合同:{basicInfo.Follow}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>审核人:{basicInfo.CheckPeople}</Col>
                        <Col span={12}>审核状态:{basicInfo.CheckState}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>是否作废:{(basicInfo.IsCancel && basicInfo.IsCancel === '1') ? "是" : "否"}</Col>
                        <Col span={12}>备注:{basicInfo.Remark}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={24}>更改记录:{basicInfo.Remark}</Col>
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