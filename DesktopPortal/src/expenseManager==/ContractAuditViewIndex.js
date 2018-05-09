import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { connect } from 'react-redux'
import reducers from './reducers'
import { sagaMiddleware } from '../'
import rootSaga from './saga/rootSaga';
import ContractInfo from './pages/contract/detail/contractInfo'
// import Shops from './pages/shops/edit/shopsEdit'
import { gotoThisContract } from './actions/actionCreator';
sagaMiddleware.run(rootSaga);

class ContractAuditViewIndex extends Component {

    state = {
    }
    componentWillMount() {
        //console.log("首次加载 buildID:", this.props.buildingID);
        const contentInfo = this.props.contentInfo;
        if (contentInfo.contentType === "ContractCommit") {
            let record = { id: contentInfo.contentID }
            this.props.dispatch(gotoThisContract({record:record} ));
        }
    }

    componentWillReceiveProps(newProps) {

    }



    render() {
        const contentInfo = this.props.contentInfo;
        return (
            <div>
                 <ContractInfo />
            </div>
        )
    }
}
function mapStateToProps(state) {
    //console.log(state, '首页加载我的楼盘')
    return {
        //buildInfo: state.building.buildInfo,
    }
}
function mapDispatchToProps(dispatch) {
    return {
        dispatch
    }
}

export default withReducer(reducers, 'ContractManagementIndex', { mapExtraState: (state, rootState) => ({ oidc: rootState.oidc }) })(connect(mapStateToProps, mapDispatchToProps)(ContractAuditViewIndex));