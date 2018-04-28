// import { connect } from 'react-redux';
//import { editProjectInfo } from '../../../actions/actionCreator';
import React, {Component} from 'react'
import {Layout, Form, Icon, Row, Col} from 'antd'
// import BasicInfo from './basicInfo';

const {Header, Sider, Content} = Layout;

class ProjectInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }

    render() {
        let projectInfo = (this.props.buildingInfo || {});
        return (
            <Layout>
                <Content className='content' >
                    <Form layout="horizontal" >
                        <Row type="flex" style={{padding: '1rem 0'}}>
                            <Col span={23}>
                                <Icon type="tags-o" className='content-icon' /> <span className='content-title'>项目简介</span>
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                        </Row>
                        <Row className='viewRow' style={{display: this.state.expandStatus ? "block" : "none"}}>
                            <Col span={24}>{projectInfo.summary || '暂无'}</Col>
                        </Row>
                    </Form>
                </Content>
            </Layout>
        )
    }
}

export default ProjectInfo;