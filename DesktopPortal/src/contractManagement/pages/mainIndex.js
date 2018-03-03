import { connect } from 'react-redux';
import { setLoadingVisible, saveSearchCondition, searchCustomer } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Menu, Icon, Row, Col, Spin, Checkbox, Button } from 'antd';
import SearchCondition from './searchCondition';
import SearchResult from './searchResult';


const buttonDef = [
    { buttonID:"record", buttonName:"录入", icon:'', type:'primary', size:'small', requirePermission:['RECORD_FUC']},
    { buttonID:"outport", buttonName:"导出", icon:'', type:'primary', size:'small', requirePermission:['RECORD_FUC']},
    { buttonID:"uploadFile", buttonName:"附件上传", icon:'', type:'primary', size:'small',requirePermission:['RECORD_FUC']},
];
class MainIndex extends Component {
    state = {

    }
    componentWillMount() {
        //this.props.dispatch(setLoadingVisible(true));//后面打开
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
    //录入
    onClickRecord = (e) =>{
        console.log('录入');
    }
    //导出
    onClickOutPort = (e) =>{
        
    }
    //文件上传
    onClickUploadFile = (e)=>{

    }
    handleClickFucButton = (buttonID) =>{
        switch(buttonID){
            case 'record':
                return this.onClickRecord;
            case 'outport':
                return this.onClickOutPort;
            case 'uploadFile':
                return this.onClickUploadFile;
            default:
                return null;
        }
    }
    render() {
        let showLoading = false;//this.props.showLoading;
        return (
            <div id='contractManagement'>
                <Spin spinning={showLoading}>
                    <SearchCondition />
                    {
                        buttonDef.map(
                            (button, i)=>this.hasPermission(button) ? <Button key = {i} id= {button.buttonID} style={{marginBottom: '10px', marginRight: '10px', border:0}}  onClick = {this.handleClickFucButton(button.buttonID)} icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button> : null

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