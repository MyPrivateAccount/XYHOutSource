import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Spin } from 'antd';
import { auditType } from './item/auditRecordItem';
import Layer, { LayerRouter } from '../../components/Layer';

class AuditDetail extends Component {
    state = {
        showLoading: false
    }

    getAuditPage() {
        if (!this.props.activeAuditHistory) {
            return;
        }
        let auditInfo = this.props.activeAuditHistory
        let ai = auditType[auditInfo.contentType];
        if (ai) {
            if(this.props.setPageTitle){
                this.props.setPageTitle(ai.name+'审核')
            }
            return ai.component;
        } else {
            return <div>未知的审核类型</div>
        }
    }


    render() {
        const { showLoading } = this.state;
        return (
            <Layer>
                <Spin spinning={showLoading}>

                    {
                        this.getAuditPage()
                    }
                </Spin>
                <LayerRouter>
                </LayerRouter>
            </Layer>

        )
    }
}

function mapStateToProps(state) {
    return {
        activeAuditInfo: state.audit.activeAuditInfo,
        activeAuditHistory: state.audit.activeAuditHistory
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditDetail);