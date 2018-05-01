import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { Layout, Menu, Icon, Button } from 'antd'
import { connect } from 'react-redux'
import reducers from './reducers'
import { sagaMiddleware } from '../'
import rootSaga from './saga/rootSaga';
import BuildingResultItem from '../businessComponents/buildingSearchItem';
import './pages/searchIndex.less';
sagaMiddleware.run(rootSaga);

const { Header, Sider, Content } = Layout;

class BuildingSearchView extends Component {

    state = {

    }

    render() {
        let buildInfoProps = this.props.contentInfo;
        if (buildInfoProps) {
            buildInfoProps.basicInfo = Object.assign({}, buildInfoProps.basicInfo, buildInfoProps.facilitiesInfo);
        }
        return (
            <div className='searchResult' style={{ margin: '0 1rem' }}>
                <BuildingResultItem buildInfoProps={buildInfoProps.basicInfo} />
            </div>
        )
    }
}
function mapStateToProps(state, props) {
    //console.log("权限管理mapStateToProps:" + JSON.stringify(props));
    return {

    }
}
export default withReducer(reducers, 'SearchToolIndex')(connect(mapStateToProps)(BuildingSearchView));