import {connect} from 'react-redux';
import {getDicParList, postBlackLst} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Table, Input, Select, Form, Button, Row, Col, Checkbox, Pagination, Spin} from 'antd'
import Layer from '../../../components/Layer'
import BlackForm from '../../../businessComponents/humanSystem/blackInfo'
import {NewGuid} from '../../../utils/appUtils';
const FormItem = Form.Item;
const ButtonGroup = Button.Group;
const formItemLayout1 = {
    labelCol: {span: 6},
    wrapperCol: {span: 6},
};


class Black extends Component {
    state = {
        blackForm: null
    }
    componentWillMount() {
    }

    componentDidMount() {

    }

    //子页面回调
    subPageLoadCallback = (formObj, pageName) => {
        console.log("表单对象:", formObj, pageName);
        if (pageName == "black") {
            this.setState({blackForm: formObj});
        }
    }
    handleSubmit = (e) => {
        e.preventDefault();
        this.state.blackForm.validateFields((err, values) => {
            if (!err) {
                values.id = NewGuid();
                console.log("提交的黑名单信息:", values);
                this.props.dispatch(postBlackLst(values));
            }
        });
    }

    render() {
        return (
            <Layer className="content-page">
                <BlackForm subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)} />
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