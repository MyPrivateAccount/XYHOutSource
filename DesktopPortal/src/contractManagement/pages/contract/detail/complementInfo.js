import { connect } from 'react-redux';
import { editContractBasic } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Table, Button, Checkbox, Row, Col, Form } from 'antd'
import moment from 'moment';

class ComplementInfo extends Component {

    handleEdit = (e) => {
        e.preventDefault();
        this.props.dispatch(editContractBasic());
    }
    render(){

        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal">
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>基础信息</span>
                        </Col>
                        <Col span={4}>
                            {
                                //[1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                    </Row>
  
                    <Row className='viewRow'>
                        <Col span={24}>补充协议:{this.props.complementInfo.contentInfo}</Col>   
                    </Row>
                </Form>
              
            </div>
        )
    }
}

function mapStateToProps(state) {

    return {
        contractInfo: state.contractData.contractInfo,
        basicData: state.basicData,
        complementInfo: state.contractData.contractInfo.complementInfo,
        
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(ComplementInfo);