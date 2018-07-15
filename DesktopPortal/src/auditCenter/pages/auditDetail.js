import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Spin, notification } from 'antd';
import { auditType } from './item/auditRecordItem';
import Layer, { LayerRouter } from '../../components/Layer';
import { getReviewComponent, getReviewDefine } from '../../tools'
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory';

class AuditDetail extends Component {
    state = {
        showLoading: false,
        isNewPage: false,
        DetailComponent: null
    }

    componentDidMount = () => {
        this.getAuditPage(this.props);
    }

    componentWillReceiveProps = (nextProps)=>{
        if(this.props.activeAuditHistory!== nextProps.activeAuditHistory){
            this.getAuditPage(nextProps);
        }
    }

    getAuditPage(props) {
        if (!props.activeAuditHistory) {
            return;
        }
        let auditInfo = props.activeAuditHistory
        let ai = auditType[auditInfo.contentType];
        let title = '';
        if (ai) {
            title = ai.name;
            this.setState({DetailComponent: ai.component})
        } else {
            //获取组件
            let Detail = getReviewComponent(auditInfo.contentType);
            // console.log('detail component:', res.extension)
            let td = getReviewDefine(auditInfo.contentType) || {}

            if (Detail) {
                title = td.title;
                let cins = <Detail  history={this.props.history} loaded={this.loaded} match={this.props.match} review={auditInfo} parentPage="audit" config={this.props.config} />;
                this.setState({ DetailComponent: cins, isNewPage: true })
            } else {

                notification.error(`无法查看该类型审核详情：${auditInfo.contentType}`)
            }
        }

        if (this.props.setPageTitle) {
            this.props.setPageTitle(title + '审核')
        }
    }


    loaded = (ins) => {
        if (ins.getRouter) {
            let routes = ins.getRouter();

            this.setState({ routes: routes })
        }
    }

    render() {
        const { showLoading, DetailComponent, isNewPage } = this.state;
        return (
            <Layer>
                <Spin spinning={showLoading}>

                    {
                        DetailComponent ? DetailComponent : null
                    }
                    {
                        isNewPage ?
                            <div>

                                <AuditHistory />
                                <div className="divider"></div>
                                {
                                    (this.props.activeAuditHistory||{}).examineStatus === 1 ? <AuditForm history={this.props.history} /> : null
                                }
                            </div> : null

                    }
                </Spin>
                <LayerRouter>
                {this.state.routes}
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