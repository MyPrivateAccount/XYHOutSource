import { connect } from 'react-redux';
import { setLoadingVisible, adduserPage } from '../../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Spin, Checkbox, Button, notification } from 'antd';
//import {getDicParList} from '../actions/actionCreator';
import SearchCondition from './searchCondition';
import SearchResult from './searchResult';
import './search.less';

const buttonDef = [
    { buttonID:"addnew", buttonName:"新建", icon:'', type:'primary', size:'large',},
    { buttonID:"modify", buttonName:"修改", icon:'', type:'primary', size:'large',},
    { buttonID:"delete", buttonName:"删除", icon:'', type:'primary', size:'large',},
];


class MainIndex extends Component {
    state = {
    }

    componentWillMount() {
        this.props.dispatch(setLoadingVisible(false));//测试
    }

    handleClickFucButton = (e) => {
        if (e.target.id === "checkin") {
            this.props.dispatch(adduserPage({menuID: 'checkininfo', disname: '录入信息', type:'item'}));
        } else if (e.target.id === "modify") {
             if (this.props.chargeList.length > 0)
                this.props.dispatch(adduserPage({menuID: 'addreciept', disname: '后补发票', type:'item'}));
             else {
                notification.error({
                    message: '未选择指定发票',
                    description: "请选择指定发票",
                    duration: 3
                });
             }
        } else if (e.target.id === "delete") {
            if (this.props.chargeList.length > 0)
                this.props.dispatch(adduserPage({menuID: 'costcharge', disname: '付款信息', type:'item'}));
             else {
                notification.error({
                    message: '未选择指定发票',
                    description: "请选择指定发票",
                    duration: 3
                });
             }
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
        chargeList: state.basicData.selchargeList,
        showLoading: state.basicData.showLoading,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);