import { connect } from 'react-redux';
import { } from '../actions/actionCreator';
import React, { Component } from 'react';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory'
import getToolComponent from '../../tools';


class AuditContract extends Component {
    state = {

    }
    componentWillMount() {
        console.log("当前审核信息:", this.props.activeAuditInfo);
    }
    componentWillReceiveProps(newProps) {
        //console.log("audit houseSource componentWillReceiveProps:", newProps);
    }


    render() {
        let ContractToolComponent = getToolComponent("contractInfo");
        let contentInfo = { contentID: this.props.activeAuditInfo.contentId, contentType: this.props.activeAuditInfo.contentType };
        return (
            <div>
                <b>合同审核</b>
                <ContractToolComponent dispatch={this.props.dispatch} contentInfo={contentInfo} />
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
export default connect(mapStateToProps, mapDispatchToProps)(AuditContract);