import { connect } from 'react-redux';
import { editBatchBuilding,getDynamicInfoList } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Table, Button, Checkbox, Row, Col, Form } from 'antd'
import moment from 'moment';
import '../components/edit.less'

const { Column, ColumnGroup } = Table;

class BuildingNoInfo extends Component {
    state = {
        projectId: ''
    }
    componentWillMount() {

    }

    handleEdit = (e) => {
        this.props.dispatch(editBatchBuilding());
    }

    compare = (property) => {
        return function (a, b) {
            let value1 = a[property]
            let value2 = b[property]
        }
    }

    initData = (id) => {
        this.setState({ projectId: id }, () => {
            // console.log('漏洞批次请求接口没有？？？？')
            if (this.props.type === 'dynamic') {
                // 如果是动态房源，则需要获取最后一个审核的信息
                let condition = {
                    pageSize: 1,
                    isCurrent:true,
                    contentTypes: ['BuildingNo'],
                    contentIds: [this.state.projectId]
                }
                this.props.dispatch(getDynamicInfoList({ 
                    condition: condition, 
                    id: this.state.projectId, 
                    updateType: 1 
                }))
            }
        })
    }
    componentWillMount() {
        // console.log(this.state.projectId, '本地id')
        this.initData(this.props.projectId)
    }
    componentWillReceiveProps(newProps) {
        if (newProps.projectId === this.state.projectId) {
            return
        }
        // console.log('变了id', newProps.projectId)
        this.initData(newProps.projectId)
    }

    render() {
        let a = {};
        if (this.props.statusArr.length !== 0 ) {
            a = ((this.props.statusArr || []).find(v => {
                return v.action === 'BuildingNo'
            })) || {}
        } 
        let buildingNoInfo = [];
        const {buildInfo, dynamicBuildInfo, dynamicStatusArr } = this.props
        if (this.props.type === 'dynamic') { // 房源动态页面，在审核中并且驳回,就显示审核中的数据。
            // console.log('动态页面哟')
            if (JSON.stringify(a) === '{}') { // 没有进行审核的
                buildingNoInfo = buildInfo.buildingNoInfos || [];
            } else { // 进行过审核的
                if ([1, 16].includes(dynamicStatusArr.examineStatus)) {
                    buildingNoInfo = dynamicBuildInfo.buildingNoInfos || [];
                } else {
                    buildingNoInfo = buildInfo.buildingNoInfos || [];
                }
            }
        } else {
            buildingNoInfo = buildInfo.buildingNoInfos || [];
        }
        buildingNoInfo.forEach(v => {
            if (v.openDate && v.openDate !== "") {
                v.openDate = moment(v.openDate).format("YYYY-MM-DD");
            }
            if (v.openDate === 'Invalid date') { v.openDate = '' }
            // v.openDate = v.openDate ? moment(v.openDate).format('YYYY-MM-DD') : null
            v.deliveryDate = moment(v.deliveryDate).format('YYYY-MM-DD')
        })
        buildingNoInfo.sort((obj1, obj2) => {
            return moment(obj1.openDate) >= moment(obj2.openDate) && moment(obj1.deliveryDate) >= moment(obj2.deliveryDate)
        })
        // console.log(buildingNoInfo, '楼栋批次')
        buildingNoInfo.sort((obj1, obj2) => {
            return parseInt(obj1.storied) - parseInt(obj2.storied)
        })
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }} className='buildingInfo'>
                <Row type="flex" style={{ padding: '1rem 0' }}>
                    <Col span={20} >
                        <Icon type="tags-o" className='content-icon' /> <span className='content-title'>楼栋批次信息</span>
                    </Col>
                    <Col span={4}>
                        { // this.props.type = 'dynamic' 说明这个页面是从动态房源哪里引用的。因为动态房源都是审核通过的页面，但是可以进行修改，所以要加以判断
                            this.props.type === 'dynamic' ? 
                            [1].includes(a.examineStatus) ? null : 
                            <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} /> :
                                // 下面的判断是因为在新增房源那里，1和8状态的楼盘都不可修改
                                [1, 8].includes(buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                        }
                    </Col>
                </Row>
                <Row>
                    <Table dataSource={buildingNoInfo} style={{ padding: ' 20px' }}>
                        <Column
                            title="编号"
                            dataIndex="storied"
                            key="storied"
                        />
                        <Column
                            title="开盘时间"
                            dataIndex="openDate"
                            key="openDate"
                        />
                        <Column
                            title="交房时间"
                            dataIndex="deliveryDate"
                            key="deliveryDate"
                        />
                    </Table>
                </Row>
            </div>
        )
    }
}

function mapStateToProps(state) {
    //console.log('shopsMapStateToProps:' + JSON.stringify(state));
    return {
        buildInfo: state.building.buildInfo,
        statusArr: state.active.statusArr,
        dynamicStatusArr: state.active.dynamicStatusArr,
        dynamicBuildInfo: state.building.dynamicBuildInfo,
        projectId: state.active.projectId
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BuildingNoInfo);