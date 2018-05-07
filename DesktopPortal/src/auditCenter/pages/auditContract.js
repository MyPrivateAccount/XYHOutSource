import { connect } from 'react-redux';
import { } from '../actions/actionCreator';
import React, { Component } from 'react';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory'
import getToolComponent from '../../tools';
import {getDicParList,} from '../../actions/actionCreators';
import {getContractDetail, setLoadingVisible} from '../actions/actionCreator';
import ContractBasicInfo from '../../contractComponent/contractBasicInfo';
import ComplementInfo from '../../contractComponent/complementInfo';
import AttachInfo from '../../contractComponent/attachInfo';
import {globalAction} from 'redux-subspace';

class AuditContract extends Component {
    state = {

    }
    componentWillMount() {
        console.log("当前审核信息:", this.props.activeAuditInfo);
        this.props.dispatch(globalAction(getDicParList(['CONTRACT_CATEGORIES', 'FIRST_PARTT_CATEGORIES', 'COMMISSION_CATEGORIES', 'XK_SELLER_TYPE', 'CONTRACT_ATTACHMENT_CATEGORIES', 'CONTRACT_SETTLEACCOUNTS'])));
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getContractDetail(this.props.activeAuditInfo.submitDefineId));
       // this.props.dispatch(getContractDetail("973cbad1-307b-48a5-72da-d418d1c8147b"));
        
    }
    componentWillReceiveProps(newProps) {
        //console.log("audit houseSource componentWillReceiveProps:", newProps);
       // this.props.dispatch(globalAction(getDicParList(['CONTRACT_CATEGORIES', 'FIRST_PARTT_CATEGORIES', 'COMMISSION_CATEGORIES', 'XK_SELLER_TYPE'])));
        
    }

    getPageByContentType = () =>{
        const contractInfo = this.props.contractInfo || {};
        const rootBasicData = (this.props.rootBasicData || {}) || [];
        //const curType = this.props.activeAuditInfo.contentName || '';
        console.log('contractInfo:', contractInfo)
        
  
        // if(curType === 'AddContract' || curType === 'Modify'){
        //     let basicInfo = contractInfo.baseInfo || {};
        //     return (<ContractBasicInfo basicData={rootBasicData} basicInfo={basicInfo}/>);
        // }else if(curType === 'AddComplement' || curType === 'ModifyComplement'){
        //     return (<ComplementInfo basicData={rootBasicData} complementInfo={contractInfo.complementInfo || []}/>);
        // }else if(curType === 'UploadFiles'){
        //     let contractAttachInfo = {};
        //     contractAttachInfo.fileList = contractInfo.fileList || [];
        //     return (<AttachInfo basicData={rootBasicData} contractAttachInfo={contractAttachInfo}/>);
        // }
        

     
        let curType = contractInfo.type;
        if(curType === 1 || curType === 2){
            let curContractInfo = contractInfo.baseInfo || {};
            curContractInfo.discard = contractInfo.discard;
            return (<ContractBasicInfo basicData={rootBasicData} basicInfo={contractInfo.baseInfo}/>);
        }else if(curType === 3){
            let contractAttachInfo = {};
            contractAttachInfo.fileList = contractInfo.fileList || [];
            return (<AttachInfo basicData={rootBasicData} contractAttachInfo={contractAttachInfo}/>);
        }else if(curType === 4){
            return (<ComplementInfo basicData={rootBasicData} complementInfo={contractInfo.complementInfo || []} />);
        }
    }
    render() {
    
        return (
            <div>
                <b>合同审核</b>
                {this.getPageByContentType()}
                <AuditHistory />
                {
                    this.props.activeAuditInfo.examineStatus === 1 ? <AuditForm /> : null
                }
            </div>
        )
    }
}

function mapStateToProps(state) {
    //console.log('state.rootBasicData:', state.rootBasicData);
    return {
        rootBasicData: state.rootBasicData,
        activeAuditInfo: state.audit.activeAuditInfo,
        contractInfo: state.audit.contractInfo,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditContract);