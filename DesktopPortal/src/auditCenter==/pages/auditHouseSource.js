import { connect } from 'react-redux';
import { } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin } from 'antd';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory'
import getToolComponent from '../../tools';


class AuditHouseSource extends Component {
    state = {

    }
    componentWillMount() {
        console.log("当前审核信息:", this.props.activeAuditInfo);
    }
    componentWillReceiveProps(newProps) {
        //console.log("audit houseSource componentWillReceiveProps:", newProps);
    }


    render() {
        let HouseToolComponent = getToolComponent("houseInfo");
        let contentInfo = { contentID: this.props.activeAuditInfo.contentId, contentType: this.props.activeAuditInfo.contentType };
        return (
            <div>
                <b>房源审核</b>
                <HouseToolComponent dispatch={this.props.dispatch} contentInfo={contentInfo} />
                <AuditHistory />
                {
                    this.props.activeAuditInfo.examineStatus === 1 ? <AuditForm /> : null
                }
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        basicData: state.basicData,
        activeAuditInfo: state.audit.activeAuditInfo,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditHouseSource);