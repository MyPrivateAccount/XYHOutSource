import { connect } from 'react-redux';
import { commissionEdit,getDynamicInfoList } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Form, Icon, Button, Checkbox, Row, Col } from 'antd'
import BasicInfo from './basicInfo';

class CommissionInfo extends Component {
    state = {
        projectId: ''
    }
    initData = (id) => {
        this.setState({ projectId: id }, () => {
            // console.log('佣金方案请求接口没有？？？？')
            if (this.props.type === 'dynamic') {
                // 如果是动态房源，则需要获取最后一个审核的信息
                let condition = {
                    pageSize: 1,
                    isCurrent:true,
                    contentTypes: ['CommissionType'],
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
    handleEdit = (e) => {
        this.props.dispatch(commissionEdit());
    }
    render() {
        let a = ((this.props.statusArr || []).find(v => {
            return v.action === 'CommissionType'
        })) || {}
        let commissionPlan;
        const {buildInfo, dynamicBuildInfo, dynamicStatusArr } = this.props
        // console.log(buildInfo, this.props.dynamicBuildInfo, this.props.dynamicStatusArr, '??佣金方案')
        if (this.props.type === 'dynamic') { // 房源动态页面，在审核中并且驳回,就显示审核中的数据。
            // console.log('动态页面哟')
            if (JSON.stringify(a) === '{}') { // 没有进行审核的
                commissionPlan = buildInfo.commissionPlan || '';
            } else { // 进行过审核的
                if ([1, 16].includes(dynamicStatusArr.examineStatus)) {
                    commissionPlan = dynamicBuildInfo.commissionPlan
                    // console.log(commissionPlan, '审核中的数据')
                } else { // 审核通过以及未提交审核就显示本来的数据
                    commissionPlan = buildInfo.commissionPlan || '';
                }
            }
            
        } else {
            commissionPlan = buildInfo.commissionPlan || '';
        }
        
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal" >
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>佣金方案</span>
                        </Col>
                        <Col span={4}>
                        { // this.props.type = 'dynamic' 说明这个页面是从动态房源哪里引用的。因为动态房源都是审核通过的页面，但是可以进行修改，所以要加以判断
                            this.props.type === 'dynamic' ? 
                            [1].includes(a.examineStatus) ? null : 
                            <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} /> :
                            // 下面的判断是因为在新增房源那里，1和8状态的楼盘都不可修改
                            [1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                        }
                        </Col>
                    </Row>
                    <Row style={{padding: '20px 4rem'}}>
                        <Col span={24}>{commissionPlan || '暂无'}</Col>
                    </Row>
                </Form>
            </div>
        )
    }
}

function mapStateToProps(state) {
    //console.log('BuildingDishDetail MapStateToProps:' + JSON.stringify(state));
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
export default connect(mapStateToProps, mapDispatchToProps)(CommissionInfo);