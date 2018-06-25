import React, {Component} from 'react';
import {Form, Input, InputNumber, DatePicker, Select, Button, Row, Col, Checkbox, } from 'antd'
import {connect} from 'react-redux';
import moment from 'moment';
import {getworkNumbar, postHumanInfo, setSocialEN} from '../../actions/actionCreator';
import './staff.less';
import SocialSecurity from './form/socialSecurity'
const Option = Select.Option;
const FormItem = Form.Item;

const formItemLayout1 = {
    labelCol: {span: 6},
    wrapperCol: {span: 6},
};

class OnBoarding extends Component {
    state = {
        isSocial: false
    }

    componentWillMount() {
        this.state.id = this.props.selHumanList[this.props.selHumanList.length - 1].id;
        this.state.idCard = this.props.selHumanList[this.props.selHumanList.length - 1].idcard;
    }

    componentDidMount() {
    }
    onHandleSubmit = (e) => {
        this.props.dispatch(setSocialEN(this.state));
    }
    onChangeInsure = (e) => {
        this.setState({isSocial: e.target.checked});
    }
    onChangeGiveup = (e) => {
        this.state.giveup = e.target.checked;
    }
    onChangeGiveupSign = (e) => {
        this.state.giveupSign = e.target.checked;
    }
    onChangeTime = (e) => {
        this.state.enTime = e;
    }
    onChangeSureTime = (e) => {
        this.state.sureTime = e;
    }
    onChangePlace = (e) => {
        this.state.enPlace = e.target.value;
    }
    onChangePension = (e) => {
        this.state.pension = e;
    }
    onChangeMedical = (e) => {
        this.state.medical = e;
    }
    onChangeWorkInjury = (e) => {
        this.state.workInjury = e;
    }
    onChangeUnemployment = (e) => {
        this.state.unemployment = e;
    }
    onChangeFertility = (e) => {
        this.state.fertility = e;
    }

    render() {
        return (
            <div className="insureBlock">
                <Row>
                    <Col span={7}>员工编号:<Input style={{width: '200px'}} disabled /></Col>
                    <Col span={7}>姓名:<Input style={{width: '200px'}} disabled /></Col>
                    <Col span={10}>
                        转正实际生效时间:<DatePicker style={{width: '200px'}} onChange={this.onChangeTime} format='YYYY-MM-DD' />
                    </Col>
                </Row>
                <div className="noinsureItem">
                    <Row>
                        <Col>
                            <SocialSecurity />
                        </Col>
                        <Col style={{textAlign: 'center'}}>
                            <Button type="primary" htmlType="submit" onClick={this.onHandleSubmit} >提交</Button>
                        </Col>
                    </Row>
                </div>
                {/* <Row>
                    <Col span={8}>
                        <Checkbox onChange={this.onChangeInsure} >是否参加社保</Checkbox>
                    </Col>
                </Row>
                <Row >
                    <Col offset={2} span={8}>
                        <div className="insureItem"><label >参保时间&nbsp;&nbsp;</label><DatePicker onChange={this.onChangeSureTime} disabled={!this.state.isSocial} format='YYYY-MM-DD' style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >参保地点&nbsp;&nbsp;</label><Input onChange={this.onChangePlace} disabled={!this.state.isSocial} placeholder="请输入参保地" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >养老保险&nbsp;&nbsp;</label><InputNumber onChange={this.onChangePension} disabled={!this.state.isSocial} placeholder="请输入养老保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >医疗保险&nbsp;&nbsp;</label><InputNumber onChange={this.onChangeMedical} disabled={!this.state.isSocial} placeholder="请输入医疗保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >工伤保险&nbsp;&nbsp;</label><InputNumber onChange={this.onChangeWorkInjury} disabled={!this.state.isSocial} placeholder="请输入工伤保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >失业保险&nbsp;&nbsp;</label><InputNumber onChange={this.onChangeUnemployment} disabled={!this.state.isSocial} placeholder="请输入失业保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >生育保险&nbsp;&nbsp;</label><InputNumber onChange={this.onChangeFertility} disabled={!this.state.isSocial} placeholder="请输入生育保险" style={{width: '70%'}} /></div>
                    </Col>
                </Row>
                 <Row>
                        <Col offset={2} span={8}>
                            <Checkbox onChange={this.onChangeGiveup} disabled={this.state.isSocial} >放弃购买</Checkbox>
                        </Col>
                    </Row>
                    <Row>
                        <Col offset={2} span={8}>
                            <Checkbox onChange={this.onChangeGiveupSign} disabled={this.state.isSocial} >放弃购买是否签订承诺书</Checkbox>
                        </Col>
                    </Row>
                */}
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