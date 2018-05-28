import React, {Component} from 'react';
import {Form, Input, InputNumber, DatePicker, Select,  Button, Row, Col, Checkbox,} from 'antd'
import {connect} from 'react-redux';
import moment from 'moment';
import { getworkNumbar, postHumanInfo, getallOrgTree} from '../../actions/actionCreator';
import './staff.less';

const Option = Select.Option;
const FormItem = Form.Item;

const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};

class OnBoarding extends Component {
    state = {
        isInsurance: true
    }

    componentWillMount() {
        //this.props.dispatch(getallOrgTree('PublicRoleOper'));
    }

    componentDidMount() {
    }

    onChangeInsure = (e) => {
        this.setState({isInsurance: !e.target.checked});
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
                        <div className="insureItem"><label >参保时间&nbsp;&nbsp;</label><DatePicker disabled={this.state.isInsurance} format='YYYY-MM-DD' style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >参保地点&nbsp;&nbsp;</label><Input disabled={this.state.isInsurance} placeholder="请输入参保地" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >养老保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isInsurance} placeholder="请输入养老保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >医疗保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isInsurance} placeholder="请输入医疗保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >工伤保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isInsurance} placeholder="请输入工伤保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >失业保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isInsurance} placeholder="请输入失业保险" style={{width: '70%'}} /></div>
                        <div className="insureItem"><label >生育保险&nbsp;&nbsp;</label><InputNumber disabled={this.state.isInsurance} placeholder="请输入生育保险" style={{width: '70%'}} /></div>
                    </Col>
                </Row>
                <div className="noinsureItem">
                    <Row>
                        {/* <Divider /> */}
                        <Col offset={2} span={8}>
                            <Checkbox disabled={!this.state.isInsurance} >放弃购买</Checkbox>
                        </Col>
                    </Row>
                    <Row>
                        {/* <Divider /> */}
                        <Col offset={2} span={8}>
                            <Checkbox disabled={!this.state.isInsurance} >放弃购买是否签订承诺书</Checkbox>
                        </Col>
                    </Row>
                </div>
            </div>

            
        );
    }
}

;

function stafftableMapStateToProps(state) {
    return {
        setDepartmentOrgTree: state.basicData.searchOrgTree
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

export default connect(null, stafftableMapDispatchToProps)(OnBoarding);