import { connect } from 'react-redux';
import { rulesEdit,getDynamicInfoList } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Form, Icon, Button, Checkbox, Row, Col } from 'antd'
import BasicInfo from './basicInfo';
import moment from 'moment'


class RulesInfo extends Component {
    state = {
        projectId: ''
    }
    initData = (id) => {
        this.setState({ projectId: id }, () => {
            console.log('请求接口没有？？？？')
            if (this.props.type === 'dynamic') {
                // 如果是动态房源，则需要获取最后一个审核的信息
                let condition = {
                    pageSize: 1,
                    isCurrent:true,
                    contentTypes: ['ReportRule'],
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
        console.log(this.state.projectId, '本地id')
        this.initData(this.props.projectId)
    }
    componentWillReceiveProps(newProps) {
        if (newProps.projectId === this.state.projectId) {
            return
        }
        console.log('变了id', newProps.projectId)
        this.initData(newProps.projectId)
    }

    handleEdit = (e) => {
        console.log('编辑')
        this.props.dispatch(rulesEdit());
    }
    
    render() {
        const {buildInfo, dynamicBuildInfo, dynamicStatusArr } = this.props
        let ruleInfo;
        let a = ((this.props.statusArr || []).find(v => {
            return v.action === 'ReportRule'
        })) || {}
        console.log(JSON.stringify(buildInfo), this.props.dynamicBuildInfo, this.props.dynamicStatusArr, '??其他规则')
          if (this.props.type === 'dynamic') { // 房源动态页面，在审核中并且驳回,就显示审核中的数据。
            //   console.log('动态页面哟')
                if (JSON.stringify(a) === '{}') { // 没有进行审核的
                    ruleInfo = buildInfo.ruleInfo || {};
                    console.log(ruleInfo, 1111)
                    if (ruleInfo.liberatingStart) {
                        ruleInfo.liberatingStart = moment(ruleInfo.liberatingStart).format("HH:mm");
                    }
                    if (ruleInfo.liberatingEnd) {
                        ruleInfo.liberatingEnd = moment(ruleInfo.liberatingEnd).format("HH:mm");
                    }
                } else { // 进行过审核的
                    if ([1, 16].includes(dynamicStatusArr.examineStatus)) {
                        ruleInfo = dynamicBuildInfo.ruleInfo || {};
                        console.log(ruleInfo, 2222)
                        if (ruleInfo.liberatingStart) {
                            ruleInfo.liberatingStart = moment(ruleInfo.liberatingStart).format("HH:mm");
                        }
                        if (ruleInfo.liberatingEnd) {
                            ruleInfo.liberatingEnd = moment(ruleInfo.liberatingEnd).format("HH:mm");
                        }
                    } else { // 审核通过以及未提交审核就显示本来的数据
                        ruleInfo = buildInfo.ruleInfo || {};
                        console.log(ruleInfo, 333)
                        if (ruleInfo.liberatingStart) {
                            ruleInfo.liberatingStart = moment(ruleInfo.liberatingStart).format("HH:mm");
                        }
                        if (ruleInfo.liberatingEnd) {
                            ruleInfo.liberatingEnd = moment(ruleInfo.liberatingEnd).format("HH:mm");
                        }
                    }
                }
             
          } else {
                ruleInfo = buildInfo.ruleInfo || {};
                if (ruleInfo.liberatingStart && ruleInfo.liberatingStart !== "") {
                    ruleInfo.liberatingStart = moment(ruleInfo.liberatingStart).format("HH:mm");
                }
                if (ruleInfo.liberatingEnd && ruleInfo.liberatingEnd !== "") {
                    ruleInfo.liberatingEnd = moment(ruleInfo.liberatingEnd).format("HH:mm");
                }
          }
          console.log(ruleInfo, 'view*****')
          
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal" >
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>报备规则</span>
                        </Col>
                        <Col span={4}>
                            { // this.props.type = 'dynamic' 说明这个页面是从动态房源哪里引用的。因为动态房源都是审核通过的页面，但是可以进行修改，所以要加以判断
                                this.props.type === 'dynamic' ?  // 2 通过， 3 驳回  1 审核中
                                [1].includes(a.examineStatus) ? null : 
                                <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} /> :
                                    // 下面的判断是因为在新增房源那里，1和8状态的楼盘都不可修改
                                    [1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>报备有效期：{ruleInfo.validityDay ? ` ${ruleInfo.validityDay}  天 `: ' 无 '} </Col>
                        <Col span={12}>带看保护期：{ruleInfo.beltProtectDay ? ` ${ruleInfo.beltProtectDay} 天 `: ' 无 '} </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>向开发商报备信息上限：{ruleInfo.maxCustomer ? ` ${ruleInfo.maxCustomer}  条 `: ' 无 '}</Col>
                        <Col span={12}>带看时间：{ruleInfo.liberatingStart ? ruleInfo.liberatingStart : '无'} —— {ruleInfo.liberatingEnd ? ruleInfo.liberatingEnd : '无'}</Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={12}>报备开始日期：{ruleInfo.reportTime ? moment(ruleInfo.reportTime).format('YYYY-MM-DD') : '无'}</Col>
                        <Col span={12}>报备提前时间：{ruleInfo.advanceTime ? ` ${ruleInfo.advanceTime}  分钟 `: ' 无 '}</Col>
                    </Row>
                    {ruleInfo.isCompletenessPhone ?
                        <Row className='viewRow' style={{ color: 'red' }}>
                            <Col span={24}>开发商要求报备时显示客户手机完整号码！</Col>
                        </Row>
                        : null}
                    <Row className='viewRow' style={{ paddingBottom: '25px' }}>
                        <Col span={24}>备注：{ruleInfo.mark || '无'}</Col>
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
export default connect(mapStateToProps, mapDispatchToProps)(RulesInfo);