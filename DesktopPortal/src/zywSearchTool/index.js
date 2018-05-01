import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer'
import {Layout, Menu, Icon, Button} from 'antd'
import {connect} from 'react-redux'
import reducers from './reducers'
import SearchIndex from './pages/searchIndex'
import {sagaMiddleware} from '../'
import rootSaga from './saga/rootSaga';
import RecommendDateDialog from './pages/recommendDateDialog'

sagaMiddleware.run(rootSaga);

const {Header, Sider, Content} = Layout;

class PrivilegeManagerIndex extends Component {

    state = {

    }
    render() {
        return (
            <div style={{height: '99%'}}>
                <SearchIndex />
            </div>
        )
    }
}
function mapStateToProps(state, props) {
    //console.log("权限管理mapStateToProps:" + JSON.stringify(props));
    return {

    }
}
export default withReducer(reducers, 'ZYWSearchToolIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc, judgePermissions: rootState.app.judgePermissions, rootBasicData: rootState.basicData})})(connect(mapStateToProps)(PrivilegeManagerIndex));
