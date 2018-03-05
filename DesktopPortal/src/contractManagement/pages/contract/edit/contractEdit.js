import React, {Component} from 'react';
import {connect} from 'react-redux';
import {contractInfoGroup} from '../../../constants/commonDefine';
import { Layout, Table, Button, Checkbox, Radio, Row, Col, Icon, Anchor, BackTop, Modal, notification } from 'antd'
import './contract.less'
import BasicEdit from '../../../../contractManagement/pages/contract/edit/basicEdit';

const { Header, Sider, Content } = Layout;
class ContractEdit extends Component{
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
                            {/**
                                * 基本信息
                                
                            <Col span={24}>{operInfo.basicOperType === 'view' ? <BasicInfo /> : <BasicEdit />}</Col>

                            */}
                            <Col span={24}>{<BasicEdit/>}</Col>
                        </Row>

                    </Content>
                </Layout>
            </div>
        );
    }
}

function mapStateToProps(state){
    return{

    };
}

function mapDispatchToProps(dispatch){
    return {
        dispatch
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ContractEdit);