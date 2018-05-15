import { connect } from 'react-redux';
import { setLoadingVisible, adduserPage } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Menu, Icon, Row, Col, Spin, Checkbox, Button } from 'antd';
//import {getDicParList} from '../actions/actionCreator';

import SearchCondition from './searchCondition';
import SearchResult from './searchResult';


const buttonDef = [
    { buttonID:"checkin", buttonName:"录入", icon:'', type:'primary', size:'large',},
    { buttonID:"addcharge", buttonName:"后补发票", icon:'', type:'primary', size:'large',},
    { buttonID:"inport", buttonName:"导入", icon:'', type:'primary', size:'large',},
    { buttonID:"cost", buttonName:"付款", icon:'', type:'primary', size:'large',},
];


class MainIndex extends Component {
    state = {
    }

    componentWillMount() {
    }

    handleClickFucButton = (e) => {
        if (e.target.id === "checkin") {
            this.props.dispatch(adduserPage({menuID: 'basicinfo', disname: '录入信息'}));
        }
    }
       //是否有权限
    hasPermission(buttonInfo) {
        let hasPermission = false;
        if (this.props.judgePermissions && buttonInfo.requirePermission) {
            for (let i = 0; i < buttonInfo.requirePermission.length; i++) {
                if (this.props.judgePermissions.includes(buttonInfo.requirePermission[i])) {
                    hasPermission = true;
                    break;
                }
            }
        } else {
            hasPermission = true;
        }
        return hasPermission;
    }

    render() {
        let showLoading = this.props.showLoading;
        return (
            <div >
                <Spin spinning={showLoading}>
                    <SearchCondition />
                    {
                        buttonDef.map(
                            (button, i)=>this.hasPermission(button) ?
                             <Button key = {i} id= {button.buttonID} style={{marginBottom: '10px', marginRight: '10px', border:0}}
                             onClick={this.handleClickFucButton} 
                             icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button> : null
                        )
                    }
                    <SearchResult />
                </Spin>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        basicData:state.basicData,
        searchResult: state.search.searchResult,
        showLoading: state.search.showLoading,
        searchCondition: state.search.searchCondition,
        judgePermissions: state.judgePermissions
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);