import React, { Component } from 'react'
import Layer from '../../../../components/Layer';
import FkPanel from './FKPanel'
import { Spin, notification } from 'antd'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import { Route } from 'react-router'
import TradeReview from '../rpdetails/tradeReview'
import { convertFactGet } from '../../../constants/utils'

class FkReview extends Component {

    state = {
        canEdit: false,
        loading: false,
        distribute: {},
        opType: 'view'
    }

    componentDidMount = () => {
        this._getFactGet();
        if (this.props.loaded) {
            this.props.loaded(this);
        }
    }
    _getFactGet = async () => {
        let rv = this.props.review || {};
        if (!rv.contentId) {
            return;
        }


        this.setState({ loading: true })
        try {
            let url = `${WebApiConfig.rp.factget}${rv.contentId}`;
            let r = await ApiClient.get(url, true);
            r = (r || {}).data;
            if (r.code === '0') {
                if (r.extension) {
                    let fg = convertFactGet(r.extension)
                    this.setState({ factGet: fg })
                } else {
                    notification.error({ message: '获取收付款明细失败' });
                }
            } else {
                notification.error({ message: '获取收付款明细失败', description: r.message || '' });
            }
        } catch (e) {
            notification.error({ message: '获取收付款明细失败' + (e.message || '') })
        }
        this.setState({ loading: false })
    }

    getRouter = () => {
        return <Route path={`${this.props.match.url}/reportInfo`} render={(props) => <TradeViewPage {...props} hideYjfp={false} hideSfkCk={true} reportId={this.props.review.ext7} />} />
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

                <FkPanel {...this.state} />
            </Spin>
        )
    }
}

class TradeViewPage extends Component {
    render() {
        return (
            <Layer>
                <TradeReview {...this.props} />
            </Layer>
        )
    }
}

export default FkReview;