import React, {Component} from 'react';
import {withReducer} from 'react-redux-dynamic-reducer'
import {Layout, Menu, Icon, Button} from 'antd'
import {connect} from 'react-redux'
import reducers from './reducers'
import SearchIndex from './pages/searchIndex'
import {sagaMiddleware} from '../'
import rootSaga from './saga/rootSaga';


sagaMiddleware.run(rootSaga);

const {Header, Sider, Content} = Layout;

class RecommendToolIndex extends Component {

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
    return {

    }
}
export default withReducer(reducers, 'ZYWRecommendToolIndex', {mapExtraState: (state, rootState) => ({oidc: rootState.oidc, judgePermissions: rootState.app.judgePermissions, rootBasicData: rootState.basicData})})(connect(mapStateToProps)(RecommendToolIndex));