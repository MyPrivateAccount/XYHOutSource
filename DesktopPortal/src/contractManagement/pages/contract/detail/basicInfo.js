import { connect } from 'react-redux';
import { editBuildingBasic } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Table, Button, Checkbox, Row, Col, Form } from 'antd'
import moment from 'moment';

class BasicInfo extends Component {
    state = {

    }
    componentWillMount() {

    }

    handleEdit = (e) => {
        //this.props.dispatch(editBuildingBasic());
    }

    render(){
        const basicInfo = this.props.contractInfo.contractBasicInfo;
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
                        <Col span={12}>{basicInfo.id} | {basicInfo.areaFullName}</Col>
                    </Row>
                </Form>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        contractInfo: state.contractBasicData.contractInfo,
        basicData: state.basicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BasicInfo);