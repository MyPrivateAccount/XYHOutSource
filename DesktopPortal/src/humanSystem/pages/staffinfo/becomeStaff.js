import React, {Component} from 'react';
import {Form, Input, InputNumber, DatePicker, Select,  Button, Row, Col, Checkbox,} from 'antd'
import {connect} from 'react-redux';
import moment from 'moment';
import { getworkNumbar, postHumanInfo, setSocialEN} from '../../actions/actionCreator';
import './staff.less';

const Option = Select.Option;
const FormItem = Form.Item;

const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};

class OnBoarding extends Component {
    state = {
        isSocial: true
    }

    componentWillMount() {
        this.state.idCard = this.props.selHumanList[this.props.selHumanList.length-1].idCard;
    }

    componentDidMount() {
    }
    onHandleSubmit() {
        this.props.dispatch(setSocialEN(this.state));
    }
    onChangeInsure = (e) => {
        this.setState({isSocial: !e.target.checked});
    }
    onChangeGiveup = (e) => {
        this.state.giveup = e.target.checked;
    }
    onChangeGiveupSign = (e) => {
        this.state.giveupSign = e.target.checked;
    }
    onChangeTime = (e) => {
        this.state.enTime = e.target.value;
    }
    onChangePlace = (e) => {
        this.state.enPlace = e.target.value;
    }
    onChangePension = (e) => {
        this.state.pension = e.target.value;
    }
    onChangeMedical = (e) => {
        this.state.medical = e.target.value;
    }
    onChangeWorkInjury = (e) => {
        this.state.workInjury = e.target.value;
    }
    onChangeUnemployment = (e) => {
        this.state.unemployment = e.target.value;
    }
    onChangeFertility = (e) => {
        this.state.fertility = e.target.fertility;
    }

    render() {
        return (
            <div className="insureBlock">
                <Row type="flex">
                    <Col span={6}>
                        <label>转正实际生效时间 </label><DatePicker format='YYYY-MM-DD' style={{width: '70%'}} />
                    </Col>
                </Row>
                <Row>
                    <Col span={8}>
                        <Checkbox onChange={this.onChangeInsure} >是否参加社保</Checkbox>
                    </Col>
                </Row>
                <Row >
                    <Col offset={2} span={8}>
                        <div className="insureItem"><label >参保时间&nbsp;&nbsp;</label><DatePicker disabled={this.state.isSocial} format='YYYY-MM-DD' style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >参保地点&nbsp;&nbsp;</label><Input disabled={this.state.isSocial} placeholder="请输入参保地" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >养老保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isSocial} placeholder="请输入养老保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >医疗保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isSocial} placeholder="请输入医疗保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >工伤保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isSocial} placeholder="请输入工伤保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >失业保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isSocial} placeholder="请输入失业保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >生育保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isSocial} placeholder="请输入生育保险" style={{width: '70%'}} /></div>
                    </Col>
                </Row>
                <div className="noinsureItem">
                    <Row>
                        <Col offset={2} span={8}>
                            <Checkbox disabled={!this.state.isSocial} >放弃购买</Checkbox>
                        </Col>
                    </Row>
                    <Row>
                        <Col offset={2} span={8}>
                            <Checkbox disabled={!this.state.isSocial} >放弃购买是否签订承诺书</Checkbox>
                        </Col>
                    </Row>
                    <Button type="primary" htmlType="submit" onClick={this.onHandleSubmit} >提交</Button>
                </div>
            </div>
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

export default connect(stafftableMapStateToProps, stafftableMapDispatchToProps)(OnBoarding);