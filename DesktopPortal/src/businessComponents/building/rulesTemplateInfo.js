// import { connect } from 'react-redux';
// import { rulesTemplateEdit , getDynamicInfoList,getDynamicInfoDetail, gotoThisBuild} from '../../../actions/actionCreator';
import React, {Component} from 'react'
import {Form, Icon, Row, Col} from 'antd'
// import BasicInfo from './basicInfo';
import {getReport} from '../../utils/utils';


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

    render() {
        const {buildingInfo} = this.props
        let ruleInfo, template, templateData;
        ruleInfo = buildingInfo.ruleInfo || {};
        template = ruleInfo.reportedTemplate;
        templateData = [];
        if (template) {
            templateData = JSON.parse(template);
        }

        let p = this.props.buildingInfo || {};
        if (!p.buildingBasic || !p.buildingBasic.name) {
            p.buildingBasic = {name: '楼盘名称'}
        }
        let templateDataStr
        if (templateData) {
            console.log(templateData, '展示的模板数据是不是对的')
            templateDataStr = getReport(templateData, expCustomer, this.props.user, p)
        }


        return (
            <Form layout="horizontal" style={{paddingBottom: '25px', backgroundColor: "#ECECEC"}}>
                <Row type="flex">
                    <Col span={20}>
                        <Icon type="tags-o" className='content-icon' /><span className='content-title'>报备模板</span>
                    </Col>
                    <Col span={4}>
                    </Col>
                </Row>
                <Row className='viewRow' style={{paddingBottom: '25px'}}>
                    {
                        templateDataStr ? <pre>{templateDataStr}</pre> : '暂无模板'
                    }
                </Row>
            </Form>
        )
    }
}

/*function mapStateToProps(state) {
    return {
        buildInfo: state.search.activeBuilding,
        user: (state.oidc.user || {}).profile || {},
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(RelShopsInfo);*/
export default RelShopsInfo;