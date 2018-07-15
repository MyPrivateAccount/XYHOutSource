import React, { Component } from 'react'
import Layer from '../../../../components/Layer';
import TyPanel from './tyPanel'
import { Spin, notification } from 'antd'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import { Route } from 'react-router'
import TradeReview from '../rpdetails/tradeReview'

class TyReview extends Component {

    state = {
        canEdit: false,
        loading: false,
        distribute: {},
        opType: 'view'
    }

    componentDidMount = () => {
        this._getDistribute();
        if(this.props.loaded){
            this.props.loaded(this);
        }
    }

    _getDistribute = async () => {

        let rv = this.props.review || {};
        if (!rv.contentId) {
            return;
        }

        this.setState({ loading: true })
        try {
            let url = `${WebApiConfig.rp.rpDis}${rv.ext7}`;
            let r = await ApiClient.get(url, true, { distributeId: rv.contentId });
            r = (r || {}).data;
            if (r.code === '0') {
                if (r.extension) {
                    this.setState({ distribute: r.extension })
                } else {
                    notification.error({ message: '调佣申请不存在' });
                }
            } else {
                notification.error({ message: '获取调佣申请详情失败', description: r.message || '' });
            }
        } catch (e) {
            notification.error({ message: '获取调佣申请详情失败，' + (e.message || '') })
        }
        this.setState({ loading: false })
    }

    getRouter = () => {
        return <Route path={`${this.props.match.url}/reportInfo`} render={(props) => <TradeViewPage showYjtz={true} hideYjtzCk={true} {...props} reportId={this.props.review.ext7} />} />
    }

    viewReport = () => {
        this.props.history.push(`${this.props.match.url}/reportInfo`, {})
    }

    render() {
        return (
            <Spin spinning={this.state.loading}>
                <div>
                    <a className="ap-view-report-link" onClick={this.viewReport}>查看成交报告</a>
                </div>

                <TyPanel {...this.state} hidePre={this.props.hidePre} />
            </Spin>
        )
    }
}

class TradeViewPage extends Component{
    render(){
        return(
            <Layer>
                <TradeReview {...this.props}/>
            </Layer>
        )
    }
}

export default TyReview;