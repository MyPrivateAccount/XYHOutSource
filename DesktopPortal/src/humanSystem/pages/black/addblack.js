import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Table, Input, Select, Form, Button, Row, Col, Checkbox, Pagination, Spin, notification } from 'antd'
import Layer from '../../../components/Layer'
import BlackForm from '../../../businessComponents/humanSystem/blackInfo'
import { NewGuid } from '../../../utils/appUtils';
import ApiClient from '../../../utils/apiClient'
import WebApiConfig from '../../constants/webapiConfig';
const FormItem = Form.Item;
const ButtonGroup = Button.Group;
const formItemLayout1 = {
    labelCol: { span: 6 },
    wrapperCol: { span: 6 },
};


class Black extends Component {
    state = {
        blackForm: null,
        showLoading: false
    }
    componentWillMount() {
    }

    componentDidMount() {

    }
    //提交黑名单
    submitBlack(entity, type) {
        this.setState({ showLoading: true });
        let url = WebApiConfig.server.SetBlack;
        let huResult = { isOk: false, msg: '黑名单保存失败！' };
        try {
            let res = ApiClient.post(url, entity);
            this.setState({ showLoading: false });
            //弹消息，返回
            if (res.data.code == 0) {
                huResult.isOk = true;
                huResult.msg = '黑名单保存成功';
            }
        } catch (e) {
            huResult.msg = "黑名单接口调用异常!";
        }
        notification[huResult.isOk ? 'success' : 'error']({
            message: huResult.msg,
            duration: 3
        });
    }

    //子页面回调
    subPageLoadCallback = (formObj, pageName) => {
        console.log("表单对象:", formObj, pageName);
        if (pageName == "black") {
            this.setState({ blackForm: formObj });
        }
    }
    handleSubmit = (e) => {
        let blackInfo = this.props.location.state;
        e.preventDefault();
        this.state.blackForm.validateFields((err, values) => {
            if (!err) {
                if (blackInfo.id) {
                    values.id = blackInfo.id;
                } else {
                    values.id = NewGuid();
                }
                console.log("提交的黑名单信息:", values);
                this.submitBlack(values);
            }
        });
    }

    render() {
        let blackInfo = this.props.location.state;
        return (
            <Layer className="content-page" showLoading={this.state.showLoading}>
                <BlackForm subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)} entityInfo={blackInfo} />
                <Row>
                    <Col style={{ textAlign: 'center' }} span={21}>
                        <Button type="primary" onClick={this.handleSubmit} >提交</Button>
                    </Col>
                </Row>
            </Layer>

        );
    }
}

function tableMapStateToProps(state) {
    return {
        selBlacklist: state.basicData.selBlacklist,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Black);