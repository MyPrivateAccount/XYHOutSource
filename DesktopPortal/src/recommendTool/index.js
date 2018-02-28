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
            // <Layout className="page">
            //     {/* < Header > {this.state.activeMenu.displayName}</Header> */}
            //     <Content >
            //         <SearchIndex />
            //     </Content>
            // </Layout>
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
export default withReducer(reducers, 'RecommendToolIndex', {mapExtraState: (state, rootState) => ({judgePermissions: rootState.app.judgePermissions})})(connect(mapStateToProps)(RecommendToolIndex));