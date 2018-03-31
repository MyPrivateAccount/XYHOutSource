import React, {Component} from 'react';
import {connect} from 'react-redux';
import {contractInfoGroup} from '../constants/commonDefine';
import { Layout, Table, Button, Checkbox, Radio, Row, Col, Icon, Anchor, BackTop, Modal, notification } from 'antd'
import { getDicParList, openContractRecord,submitContractInfo,changeContractMenu } from '../actions/actionCreator';
import { isEmptyObject } from '../../utils/utils'
import './contract/edit/contract.less';
import BasicEdit from './contract/edit/basicEdit';
import BasicInfo from './contract/detail/basicInfo';
import AttachEdit from './contract/edit/attachEdit';
import AttachInfo from './contract/detail/attachInfo';
import ComplementInfo  from './contract/detail/complementInfo';
import ComplementEdit  from './contract/edit/complementEdit';
import { isNullOrUndefined } from 'util';

const { Header, Sider, Content } = Layout;
class ContractDetail extends Component{
    state = {
        isBasicNeedSubmit:true,
        isAdditionNeesSubmit:true,
        isAttachNeedSubmit:true,
    }
    componentWillMount(){
    

      }
    
    

    componentWillReceiveProps(newProps){

    }
    handleAnchorChange = (e) =>{
        window.location.href = '#' + e.target.value;
    }
    handleSubmit = (type) => {
        return ()=> {
            if(type === "basicInfo"){
                const basicInfo = this.props.contractInfo.baseInfo||{};
                const hasBasicInfo = !isEmptyObject(basicInfo); 
                console.log(hasBasicInfo)
        
                if(!hasBasicInfo){
                    notification.warning({
                        message: '请先完善信息，再提交',
                        duration: 3
                    })
                    return;
                }
                this.props.dispatch(submitContractInfo({ entity: this.props.contractInfo, type:"basicInfo"  }));
            }else if(type === "attachInfo"){

            }else if(type === "addtionalInfo"){

            }
        }
    }
    handleReturn = ()=>{
        this.props.dispatch(changeContractMenu("menu_index"));
    }
    render(){
        let basicOperType = this.props.operInfo.basicOperType;
        let attachPicOperType = this.props.operInfo.attachPicOperType;
        let complementOperType = this.props.operInfo.complementOperType;
        let isEdit = false;
        let infoGroup = [];
        if(basicOperType === "edit" || complementOperType ==="edit" || complementOperType ==="edit" ){
            isEdit = true;
        }


        return(
            <div className="relative">
                <Layout>
                    <Content className='content contract' style={{ marginTop: '25px' }}>
                        <Row type="flex" justify="space-between">
                            <Col span={4} style={{ textAlign: 'left' }}>
                                {/* {this.props.buildInfo.examineStatus === 8 ? null :
                                    <Button type="primary" size='large'
                                        style={{ width: "10rem", display: this.props.buildDisplay }}
                                        onClick={this.handleOk} loading={submitLoading}>提交</Button>
                                } */}
                            </Col>
                            <Col span={20} style={{ textAlign: 'right' }}>
                            {
                                isEdit? null :
                                <Radio.Group defaultValue='basicInfo' onChange={this.handleAnchorChange} size='large'>
                                    {
                                        infoGroup.map((info) => <Radio.Button value={info.id} key={info.id}>{info.name}<Icon type="check" /></Radio.Button>)
                                    }
                                </Radio.Group>
                            }
                            </Col>
                        </Row>
                        <Row id="basicInfo">
                            {
                                
                                //<Col span={24}>{basicOperType === 'view' ? <BasicInfo /> : ((attachPicOperType === "add" || additionOperType === "edit") ? null : <BasicEdit />)}</Col>
                                <Col span={24}>{(basicOperType === 'edit') ? <BasicEdit/> : (isEdit ? null : <BasicInfo />)}</Col>
                            }

                        </Row>
                        <Row id="additionalInfo">
                            {
                                
                                //<Col span={24}>{basicOperType === 'view' ? <BasicInfo /> : ((attachPicOperType === "add" || additionOperType === "edit") ? null : <BasicEdit />)}</Col>
                                <Col span={24}>{(complementOperType === 'edit') ? <ComplementEdit/> : (isEdit ? null : <ComplementInfo />)}</Col>
                            }

                        </Row>

                    
                        <Row id="attachInfo">
      
                            {
                                //<Col span={24}>{(attachPicOperType === 'view') ? <AttachInfo /> : ((basicOperType === "add" || basicOperType === "edit" ) ?  null : <AttachEdit /> )}</Col>
                                <Col span={24}>{(attachPicOperType === 'edit') ? <AttachInfo /> : (isEdit ? null : <AttachInfo />)}</Col>
                            }

                        </Row>
                                                 

                        <div>
                            <BackTop visibilityHeight={400} />
                        </div>
                        {
                            <Row type="flex" justify="space-between">
                                <Col  span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                                        <Button type="primary" size='large'
                                        style={{ width: "10rem", display: this.props.contractDisplay }}
                                        onClick={this.handleReturn} loading={this.props.submitLoading}>返回主页</Button>
    
    
                                    
                                </Col>
                            </Row>
                        }

                    </Content>
                </Layout>
            </div>
        );
    }
}

function mapStateToProps(state){
    return{
        basicData: state.basicData,
        operInfo:state.contractData.operInfo,
        contractInfo: state.contractData.contractInfo,
        submitLoading: state.contractData.contractInfo.submitLoading,
        contractDisplay:state.contractData.contractInfo.contractDisplay,
        attachInfo:state.contractData.contractInfo.complementInfo,
        basicInfo: state.contractData.contractInfo.baseInfo,
    }
    
}

function mapDispathToProps(dispatch){
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispathToProps)(ContractDetail);