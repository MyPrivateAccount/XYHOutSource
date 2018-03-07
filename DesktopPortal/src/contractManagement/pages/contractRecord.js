import React, {Component} from 'react';
import {connect} from 'react-redux';
import {contractInfoGroup} from '../constants/commonDefine';
import { Layout, Table, Button, Checkbox, Radio, Row, Col, Icon, Anchor, BackTop, Modal, notification } from 'antd'
import { getDicParList, openContractRecord } from '../actions/actionCreator';
import './contract/edit/contract.less';
import BasicEdit from './contract/edit/basicEdit';
import BasicInfo from './contract/detail/basicInfo';
import AttachEdit from './contract/edit/attachEdit';
import AttachInfo from './contract/detail/attachInfo';
const { Header, Sider, Content } = Layout;
class ContractRecord extends Component{
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
        let basicOperType = this.props.operInfo.basicOperType;
        let attachPicOperType = this.props.operInfo.attachPicOperType;
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
                                
                                <Col span={24}>{basicOperType === 'view' ? <BasicInfo /> : <BasicEdit />}</Col>

                            }

                        </Row>
                        <Row id="attchInfo">
                            {
                                
                                <Col span={24}>{attachPicOperType === 'view' ? <AttachInfo /> : <AttachEdit />}</Col>

                            }

                        </Row>
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
    }
    
}

function mapDispathToProps(dispatch){
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispathToProps)(ContractRecord);