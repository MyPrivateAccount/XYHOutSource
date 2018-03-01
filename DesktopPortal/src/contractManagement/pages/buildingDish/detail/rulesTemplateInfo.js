import { connect } from 'react-redux';
import { rulesTemplateEdit , getDynamicInfoList,getDynamicInfoDetail, gotoThisBuild} from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Form, Icon, Button, Checkbox, Row, Col } from 'antd'
import BasicInfo from './basicInfo';
import { getReport } from '../../buildingDish/edit/utils';


const expCustomer = {
    "id": "4fa9fe37-2899-4174-a1c9-ad8e00188b5a",
    "customerId": "fba92d39-2aeb-4d19-88cd-99143e6fa598",
    "customerName": "张三",
    "userId": "5cfe1b85-360b-4014-94bc-3e37cece7339",
    "departmentId": "0",
    "departmentName": "西区一区一组",
    "mainPhone": "150****3636",
    "userPhone": '13122220000',
    "userTrueName": "业务员A",
    "transactionsStatus": 2,
    "reportTime": "2017-11-30T17:59:32.191637",
    "buildingId": "b5a0baae-9691-4a20-9723-c07f9d2b7631",
    "shopsId": "77b11f39-d70f-47d1-9edc-24ab794dbc34",
    "buildingName": "万科金色悦城",
    "shopsName": "3",
    "expectedBeltTime": "2017-11-28T17:48:53.770883"
}

class RelShopsInfo extends Component {
    state = {
        projectId: ''
    }

    initData = (id) => {
        this.setState({ projectId: id }, () => {
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
        this.props.dispatch(rulesTemplateEdit());
    }

    render() {
        const {buildInfo, dynamicBuildInfo, dynamicStatusArr } = this.props
        let a = ((this.props.statusArr || []).find(v => {
            return v.action === 'ReportRule'
        })) || {}
        let ruleInfo, template, templateData;
        console.log(buildInfo, this.props.dynamicBuildInfo, this.props.dynamicStatusArr, '??/')
        if (this.props.type === 'dynamic') { // 房源动态页面，在审核中并且驳回,就显示审核中的数据。
            console.log('动态页面哟') 
            if (JSON.stringify(a) === '{}') { // 没有进行审核的
                ruleInfo = buildInfo.ruleInfo || {};
                template = ruleInfo.reportedTemplate;
                templateData = [];
                if (template) {
                    templateData = JSON.parse(template);
                }
            } else { // 进行过审核的
                if ([1, 16].includes(dynamicStatusArr.examineStatus)) {  //  0, 1, 8, 16
                    ruleInfo = dynamicBuildInfo.ruleInfo || {};
                    if (ruleInfo) {
                        templateData =  ruleInfo.reportedTemplate
                    }
                } else { // 审核通过以及未提交审核就显示本来的数据
                    ruleInfo = buildInfo.ruleInfo || {};
                    template = ruleInfo.reportedTemplate;
                    templateData = [];
                    if (template) {
                        templateData = JSON.parse(template);
                    }
                }
            }
            
        } else {
            ruleInfo = buildInfo.ruleInfo || {};
            template = ruleInfo.reportedTemplate;
            templateData = [];
            if (template) {
                templateData = JSON.parse(template);
            }
        }

        
       
        let p = this.props.buildInfo || {};
        if (!p.buildingBasic || !p.buildingBasic.name) {
            p.buildingBasic = { name: '楼盘名称' }
        }
        let templateDataStr
        if (templateData) {
            console.log(templateData, '展示的模板数据是不是对的')
            templateDataStr = getReport(templateData, expCustomer, this.props.user, p)
        }
        
        
        return (
            <Form layout="horizontal" style={{paddingBottom: '25px',backgroundColor: "#ECECEC" }}>
                <Row type="flex">
                    <Col span={20}>
                        <Icon type="tags-o" className='content-icon' /><span className='content-title'>报备模板</span>
                    </Col>
                    <Col span={4}>
                        { // this.props.type = 'dynamic' 说明这个页面是从动态房源哪里引用的。因为动态房源都是审核通过的页面，但是可以进行修改，所以要加以判断
                            this.props.type === 'dynamic' ? 
                            [1].includes(a.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} /> :
                             // 下面的判断是因为在新增房源那里，1和8状态的楼盘都不可修改
                            [1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                        }
                    </Col>
                </Row>
                <Row className='viewRow' style={{ paddingBottom: '25px' }}>
                    {
                        templateDataStr ?  <pre>{templateDataStr}</pre> : '暂无模板'
                    }
                </Row>
            </Form>
        )
    }
}

function mapStateToProps(state) {
    // console.log('BuildingDishDetail MapStateToProps:' + JSON.stringify(state));
    return {
        buildInfo: state.building.buildInfo,
        user: (state.oidc.user || {}).profile || {},
        dynamicStatusArr: state.active.dynamicStatusArr,
        dynamicBuildInfo: state.building.dynamicBuildInfo,
        projectId: state.active.projectId,
        statusArr: state.active.statusArr
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(RelShopsInfo);