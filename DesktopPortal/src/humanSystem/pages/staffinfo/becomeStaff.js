import React, {Component} from 'react';
import {Form, Input, InputNumber, DatePicker, Select, Button, Row, Col, Checkbox, } from 'antd'
import {connect} from 'react-redux';
import moment from 'moment';
import {getworkNumbar, postHumanInfo, becomeStaff} from '../../actions/actionCreator';
import './staff.less';
import SocialSecurity from '../../../businessComponents/humanSystem/socialSecurity'
import Layer from '../../../components/Layer'
const Option = Select.Option;
const FormItem = Form.Item;

const formItemLayout = {
    labelCol: {span: 6},
    wrapperCol: {span: 17},
};

class OnBoarding extends Component {
    state = {
        isSocial: false,
        staffInfo: {

        }//转正信息
    }

    componentWillMount() {
        // this.state.id = this.props.selHumanList[this.props.selHumanList.length - 1].id;
        // this.state.idCard = this.props.selHumanList[this.props.selHumanList.length - 1].idcard;

    }

    componentDidMount() {
    }
    // onChangeTime = (e) => {
    //     console.log();
    // }
    onHandleSubmit = (e) => {
        if (this.state.SocialSecurityForm) {
            this.state.SocialSecurityForm.validateFields((err, values) => {
                this.props.form.validateFields();
                let curErrs = this.props.form.getFieldsError();
                let hasErr = false;
                for (let prop in curErrs) {
                    if (curErrs[prop]) {
                        hasErr = true;
                        break;
                    }
                }
                if (!err && hasErr) {
                    this.props.dispatch(becomeStaff(this.state));
                }
                console.log("转正表单:", curErrs);
            });
        }
    }

    //子页面回调
    subPageLoadCallback = (formObj, pageName) => {
        console.log("表单对象:", formObj, pageName);
        if (pageName == "socialSecurity") {
            this.setState({SocialSecurityForm: formObj});
        }
    }

    render() {
        let humanInfo = this.props.location.state;
        const {getFieldDecorator} = this.props.form;
        return (
            <Layer >
                <Row>
                    <Col span={21}>
                        <div className="page-title" style={{marginBottom: '10px'}}>转正</div>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <label class="ant-form-item-label">员工编号:</label><Input style={{width: '200px'}} disabled value={humanInfo.userID} />
                    </Col>
                    <Col span={7}>
                        <label class="ant-form-item-label">姓名:</label><Input style={{width: '200px'}} disabled value={humanInfo.name} />
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="转正实际生效时间">
                            {getFieldDecorator('entryTime', {
                                rules: [{
                                    required: true, message: '请输入转正实际生效时间',
                                }]
                            })(
                                <DatePicker disabled={this.props.ismodify == 1} format='YYYY-MM-DD' style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <div className="noinsureItem">
                    <Row>
                        <Col>
                            <SocialSecurity subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)} />
                        </Col>
                        <Col style={{textAlign: 'center'}}>
                            <Button type="primary" htmlType="submit" onClick={this.onHandleSubmit} >提交</Button>
                        </Col>
                    </Row>
                </div>
            </Layer>
        );
    }
};

function stafftableMapStateToProps(state) {
    return {
        selHumanList: state.basicData.selHumanList,
        setDepartmentOrgTree: state.basicData.searchOrgTree
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    }
}
export default connect(stafftableMapStateToProps, stafftableMapDispatchToProps)(Form.create()(OnBoarding));