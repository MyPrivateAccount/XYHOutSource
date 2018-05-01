import React, {Component} from 'react';
import {connect} from 'react-redux';
import {contractInfoGroup} from '../../../constants/commonDefine';
import { Layout, Table, Button, Checkbox, Radio, Row, Col, Icon, Anchor, BackTop, Modal, notification } from 'antd'
import { getDicParList, openContractRecord,submitContractInfo } from '../../../actions/actionCreator';

import '../edit/contract.less';
//import BasicEdit from '../edit/basicEdit';
import BasicInfo from './basicInfo';
//import AttachEdit from './attachEdit';
import AttachInfo from './attachInfo';
import ComplementInfo from './complementInfo';
const { Header, Sider, Content } = Layout;
class ContractInfo extends Component{
    componentWillMount(){
        if(this.props.basicData.contractCategories.length === 0 || this.props.basicData.firstPartyCatogories.length === 0
        || this.props.basicData.commissionCatogories.length === 0 || this.props.basicData.contractProjectCatogories.length === 0){
          this.props.dispatch(getDicParList(['CONTRACT_CATEGORIES', 'FIRST_PARTT_CATEGORIES', 'COMMISSION_CATEGORIES', 'XK_SELLER_TYPE']));
        }
       
      }

    handleAnchorChange = (e) =>{
        window.location.href = '#' + e.target.value;
    }

    render(){

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
                                <Radio.Group defaultValue='basicInfo' onChange={this.handleAnchorChange} size='large'>
                                    {
                                        contractInfoGroup.map((info) => <Radio.Button value={info.id} key={info.id}>{info.name}<Icon type="check" /></Radio.Button>)
                                    }
                                </Radio.Group>
                            </Col>
                        </Row>

                        {/* <Row>
                            {
                                this.props.buildInfo.examineStatus !== 16 ? null :
                                <p style={{color: 'red', fontWeight: 'bold'}}>
                                    驳回原因：XXXXXXX
                                </p>
                            }
                        </Row> */}
                        <Row id="basicInfo">
                            {
                                
                                <Col span={24}><BasicInfo /></Col>

                            }

                        </Row>
                        <Row id="additionalInfo">
                            {
                                
                                //<Col span={24}>{basicOperType === 'view' ? <BasicInfo /> : ((attachPicOperType === "add" || additionOperType === "edit") ? null : <BasicEdit />)}</Col>
                                <Col span={24}><ComplementInfo /></Col>
                            }

                        </Row>
                        <Row id="attchInfo">
                            {
                                
                                <Col span={24}><AttachInfo /></Col>

                            }

                        </Row>
                        {/* <div>
                            <BackTop visibilityHeight={400} />
                        </div>
                        <Row type="flex" justify="space-between">
                            <Col span={24} style={{ display: 'flex', justifyContent: 'flex-end',margin: '5px  0 25px 0' }}>
                                {
                                    //[8, 1].includes(this.props.buildInfo.examineStatus)  ? null :
                                    <Button type="primary" size='large'
                                        style={{ width: "10rem", display: this.props.contractDisplay }}
                                        onClick={this.handleOk} loading={this.props.submitLoading}>提交</Button>
                                }
                            </Col>
                        </Row> */}
                    </Content>
                </Layout>
            </div>
        );
    }
}


export default ContractInfo;