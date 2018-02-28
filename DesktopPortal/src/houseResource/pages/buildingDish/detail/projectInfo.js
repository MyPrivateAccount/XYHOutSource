import { connect } from 'react-redux';
import { editProjectInfo } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Form, Icon, Button, Checkbox, Row, Col } from 'antd'
import BasicInfo from './basicInfo';

class ProjectInfo extends Component {
    state = {

    }
    componentWillMount() {

    }
    handleEdit = (e) => {
        this.props.dispatch(editProjectInfo());
    }
    render() {
        let projectInfo = this.props.buildInfo.projectInfo;
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal" >
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>项目简介</span>
                        </Col>
                        <Col span={4}>
                            {
                                [1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={24}>{projectInfo.summary || '暂无'}</Col>
                    </Row>
                </Form>
            </div>
        )
    }
}

function mapStateToProps(state) {
    //console.log('BuildingDishDetail MapStateToProps:' + JSON.stringify(state));
    return {
        buildInfo: state.building.buildInfo
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(ProjectInfo);