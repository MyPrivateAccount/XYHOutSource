import {connect} from 'react-redux';
import React, {Component} from 'react'
import {Button, Row, Col, notification} from 'antd'
import Layer from '../../../components/Layer'
import BlackForm from '../../../businessComponents/humanSystem/blackInfo'
import {NewGuid} from '../../../utils/appUtils';
import ApiClient from '../../../utils/apiClient'
import WebApiConfig from '../../constants/webapiConfig';
import {addBlackLst} from '../../serviceAPI/blackService'

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
        this.setState({showLoading: true});
        addBlackLst(entity).then(res => {
            this.setState({showLoading: false});
        })
    }

    //子页面回调
    subPageLoadCallback = (formObj, pageName) => {
        console.log("表单对象:", formObj, pageName);
        if (pageName == "black") {
            this.setState({blackForm: formObj});
        }
    }
    handleSubmit = (e) => {
        let blackInfo = this.props.location.state;
        e.preventDefault();
        this.state.blackForm.validateFields((err, values) => {
            if (!err) {
                if (blackInfo.id) {
                    values.id = blackInfo.id;
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
                    <Col style={{textAlign: 'center'}} span={21}>
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