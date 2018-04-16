import { connect } from 'react-redux';
import { setLoadingVisible, openAttachMent, openContractRecord,getAllOrgList } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Menu, Icon, Row, Col, Spin, Checkbox, Button } from 'antd';
import {getDicParList} from '../actions/actionCreator';
import {LoadAttatchMentPage} from './contentPage';
import SearchCondition from './searchCondition';
import SearchResult from './searchResult';
import AttachMent from './mainIndex';

const buttonDef = [
    { buttonID:"record", buttonName:"录入", icon:'', type:'primary', size:'small', requirePermission:['RECORD_FUC']},
    { buttonID:"export", buttonName:"导出", icon:'', type:'primary', size:'small', requirePermission:['EXPORT_CONTRACT']},
    { buttonID:"uploadFile", buttonName:"附件上传", icon:'', type:'primary', size:'small',requirePermission:['UPLOAD_FILE']},
];
class MainIndex extends Component {
    state = {

    }
    componentWillMount() {
        if(this.props.basicData.contractCategories.length === 0 || this.props.basicData.firstPartyCatogories.length === 0
            || this.props.basicData.commissionCatogories.length === 0 || this.props.basicData.contractProjectCatogories.length === 0
            || this.props.basicData.settleAccountsCatogories.length === 0){
                this.props.dispatch(getDicParList(['CONTRACT_CATEGORIES', 'FIRST_PARTT_CATEGORIES', 'COMMISSION_CATEGORIES', 'XK_SELLER_TYPE', 'CONTRACT_ATTACHMENT_CATEGORIES', 'CONTRACT_SETTLEACCOUNTS']));
            }
        this.props.dispatch(getAllOrgList('ContractSetOrg'));
        this.props.dispatch(setLoadingVisible(true));//后面打开
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
    // //录入
    // onClickRecord = (e) =>{
    //     console.log('录入');
    //     this.props.dispatch(openContractRecord({id:0}));
    // }
    // //导出
    // onClickExPort = (e) =>{
        
    // }
    // //文件上传
    // onClickUploadFile = (e)=>{
    //     this.props.dispatch(openAttachMent({id:1}));
    // }
        // handleClickFucButton = (buttonID) =>{
        //     switch(buttonID){
        //         case 'record':
        //             return this.onClickRecord;
        //         case 'export':
        //             return this.onClickExPort;
        //         case 'uploadFile':
        //             return this.onClickUploadFile;
        //         default:
        //             return null;
        //     }
        // }
    render() {
        let showLoading = this.props.showLoading;
        return (
            <div id='contractManagement'>
                <Spin spinning={showLoading}>
                    <SearchCondition />
                    {
                        // buttonDef.map(
                        //     (button, i)=>this.hasPermission(button) ? <Button key = {i} id= {button.buttonID} style={{marginBottom: '10px', marginRight: '10px', border:0}}  onClick = {this.handleClickFucButton(button.buttonID)} icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button> : null

                        // )
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