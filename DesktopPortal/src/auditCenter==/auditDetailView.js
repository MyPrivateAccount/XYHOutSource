import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { Spin, Menu, Icon, Button } from 'antd'
import { connect } from 'react-redux'
import reducers from './reducers'
import { sagaMiddleware } from '../'
import rootSaga from './saga/rootSaga';
import { getAuditHistory } from './actions/actionCreator';
import AuditCustomer from './pages/auditCustomer';

sagaMiddleware.run(rootSaga);
//房源动态审核查看
class AuditDetailView extends Component {

    state = {
    }
    componentWillMount() {
    }

    componentWillReceiveProps(newProps) {

    }


    render() {
        let contentInfo = this.props.contentInfo || {};
        return (
            <div>
                <AuditCustomer contentInfo={contentInfo} removeCallback={this.props.removeCallback} />
            </div>
        )
    }
}
function mapStateToProps(state) {
    //console.log(state, '首页加载我的楼盘')
    return {

    }
}
function mapDispatchToProps(dispatch) {
    return {
        dispatch
    }
}

export default withReducer(reducers, 'AuditIndex', { mapExtraState: (state, rootState) => ({ oidc: rootState.oidc }) })(connect(mapStateToProps, mapDispatchToProps)(AuditDetailView));