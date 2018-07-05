import {connect} from 'react-redux';
import React, {Component} from 'react'
import {Select, Input, Form, Cascader, Button, Row, Col, Checkbox, DatePicker, TreeSelect} from 'antd'
import Layer from '../../../components/Layer'
import LeaveForm from '../../../businessComponents/humanSystem/leavePreview'
import {leavePosition} from '../../serviceAPI/staffService'
import {getOrgUserList} from '../../serviceAPI/orgService'

class Leave extends Component {
    state = {
        department: '',
        showLoading: false,
        condition: {
            pageIndex: 0,
            pageSize: 30,
            organizationIds: [],
            isDeleted: false
        },
        colleagueList: []//同事列表
    }

    componentDidMount() {
        let humanInfo = this.props.location.state;
        if (humanInfo.departmentId) {
            let condition = this.state.condition;
            condition.organizationIds = [humanInfo.departmentId];
            getOrgUserList(condition).then(res => {
                let list = (res.extension || []).filter(item => item.id != humanInfo.id);
                this.setState({colleagueList: list})
            })
        }
    }

    handleSubmit = (e) => {
        e.preventDefault();
        let humanInfo = this.props.location.state;
        this.state.leaveForm.validateFields((err, values) => {
            if (!err) {
                values.id = humanInfo.id;
                values.idCard = humanInfo.idcard;
                this.setState({showLoading: true});
                let entity = {
                    humanId: humanInfo.id,
                    leaveTime: values.leaveTime,
                    newHumanId: values.handover,
                    isProcedure: values.isFormalities
                }
                leavePosition(entity).then(res => {
                    this.setState({showLoading: false});
                })
            }
        });
    }
    //子页面回调
    subPageLoadCallback = (formObj, pageName) => {
        console.log("表单对象:", formObj, pageName);
        if (pageName == "leave") {
            this.setState({leaveForm: formObj});
        }
    }

    render() {
        let humanInfo = this.props.location.state;
        return (
            <Layer>
                <div className="page-title" style={{marginBottom: '10px'}}>离职</div>
                <LeaveForm
                    entityInfo={humanInfo}
                    handleOverList={this.state.colleagueList}
                    subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)}
                    setDepartmentOrgTree={this.props.setDepartmentOrgTree || []}
                />
                <Row>
                    <Col span={20} style={{textAlign: 'center'}}>
                        <Button type="primary" htmlType="submit" onClick={this.handleSubmit} >提交</Button>
                    </Col>
                </Row>
            </Layer >
        );
    }
}

function tableMapStateToProps(state) {
    return {
        setDepartmentOrgTree: state.basicData.searchOrgTree,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Leave);