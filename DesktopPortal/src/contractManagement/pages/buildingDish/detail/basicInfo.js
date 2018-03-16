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
        this.props.dispatch(editBuildingBasic());
    }
   
    render() {
        const basicInfo = this.props.buildInfo.buildingBasic;
        if (basicInfo.landExpireDate && basicInfo.landExpireDate !== "") {
            basicInfo.landExpireDate = moment(basicInfo.landExpireDate).format("YYYY-MM-DD");
        }
        if (basicInfo.openDate && basicInfo.openDate !== "") {
            basicInfo.openDate = moment(basicInfo.openDate).format("YYYY-MM-DD");
        }
        if (basicInfo.deliveryDate && basicInfo.deliveryDate !== "") {
            basicInfo.deliveryDate = moment(basicInfo.deliveryDate).format("YYYY-MM-DD");
        }
        // console.log(this.props.buildInfo,basicInfo,'456')
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal">
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>基础信息</span>
                        </Col>
                        <Col span={4}>
                            {
                                [1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>{basicInfo.name} | {basicInfo.areaFullName}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>{basicInfo.builtupArea}㎡ | 共{basicInfo.houseHolds}户</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>开盘时间：{basicInfo.openDate || '无'}</Col>
                        <Col span={12}>交房时间：{basicInfo.deliveryDate || '无'}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>产权年限：{basicInfo.propertyTerm}年</Col>
                        <Col span={12}>土地到期时间：{basicInfo.landExpireDate}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>占地面积：{basicInfo.floorSurface}㎡</Col>
                        <Col span={12}>建筑面积：{basicInfo.builtupArea}㎡</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>容积率：{basicInfo.plotRatio ? basicInfo.plotRatio : '无'}%</Col>
                        <Col span={12}>绿化率：{basicInfo.greeningRate ? basicInfo.greeningRate : '无'}%</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>地下停车位：{basicInfo.basementParkingSpace}个</Col>
                        <Col span={12}>地面停车位：{basicInfo.parkingSpace}个</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>楼栋总数：{basicInfo.buildingNum}栋</Col>
                        <Col span={12}>商铺总数：{basicInfo.shops}个</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>物业公司：{basicInfo.pmc}</Col>
                        <Col span={12}>物业费：{basicInfo.pmf}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>开发商：{basicInfo.developer}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={24}>地址:{basicInfo.address}</Col>
                    </Row>
                </Form>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        buildInfo: state.building.buildInfo,
        basicData: state.basicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BasicInfo);