import {connect} from 'react-redux';
// import {createStation, getOrgList, } from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Select, Input, Form, Cascader, Button, Row, Col, Checkbox, DatePicker, TreeSelect} from 'antd'
import Layer from '../../../components/Layer'
import LeaveForm from '../../../businessComponents/humanSystem/leavePreview'
import {leavePosition} from '../../serviceAPI/staffService'

class Leave extends Component {
    state = {
        department: '',
        showLoading: false
    }

    componentWillMount() {
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        e.preventDefault();
        let humanInfo = this.props.location.state;
        this.state.leaveForm.validateFields((err, values) => {
            if (!err) {
                values.id = humanInfo.id;
                values.idCard = humanInfo.idcard;
                this.setState({showLoading: true});
                leavePosition(values).then(res => {
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