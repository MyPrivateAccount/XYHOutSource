import { connect } from 'react-redux';
import { contractComplementEdit,openComplementStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Table, Button, Checkbox, Row, Col, Form } from 'antd'
import moment from 'moment';

class ComplementInfo extends Component {
    state = {
        expandStatus: true
    }
    render(){
     
        let complementInfo = this.props.complementInfo || [];
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal">
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>补充协议</span>
                        </Col>
                        <Col span={4}>
                            {
                                [1].includes(this.props.complementInfos.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                //<Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                        <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                        </Row>
                        <div style={{display: this.state.expandStatus ? "block" : "none"}}>
  
                            <Row className='viewRow'>
                                {
                                complementInfo.length === 0 ? <div style={{ marginLeft: '20px' }}>{'暂无信息'}</div> :
                                
                                    complementInfo.map((item, i)=>{
                                        return <Col span={24} key= {i}>补充内容{i + 1}:{item.contentInfo}</Col>
                                    })
                                    
                                }
                                    
                            </Row>
                        </div>
                </Form>
              
            </div>
        )
    }
}

export default ComplementInfo;