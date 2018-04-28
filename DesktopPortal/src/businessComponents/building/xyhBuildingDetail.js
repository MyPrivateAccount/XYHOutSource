import {connect} from 'react-redux';
import React, {Component} from 'react'
import {Layout, Radio, Row, Col, Icon} from 'antd'
import BasicInfo from './basicInfo';
import ProjectInfo from './projectInfo';
import RelShopsInfo from './relShopsInfo';
import SupportInfo from './supportInfo';
import BuildingNoInfo from './buildingNoInfo';
import RulesInfo from './rulesInfo';
import RulesTemplateInfo from './rulesTemplateInfo';
import CommissionInfo from './commissionInfo';
import AttachInfo from '../shop/attachInfo'
import './buildingDish.less';

const {Header, Sider, Content} = Layout;

class XYHBuildingDishInfo extends Component {
    state = {
    }
    componentWillMount() {
    }

    render() {
        let buildingInfo = this.props.buildingInfo || {};

        return (
            <div className="relative">
                <Layout>
                    <Content className='content buildingDish' style={{marginTop: '25px'}}>
                        <Row type="flex" justify="space-between">
                            <Col span={4} style={{textAlign: 'left'}}>
                            </Col>
                        </Row>
                        <Row id="basicInfo">
                            {/** * 基本信息 */}
                            <Col span={24}> <BasicInfo buildingInfo={buildingInfo} /></Col>
                        </Row>
                        <Row id="rulesInfo">
                            {/*** 报备规则*/}
                            <Col span={24}>
                                <RulesInfo buildingInfo={buildingInfo} />
                                <RulesTemplateInfo buildingInfo={buildingInfo} />
                            </Col>
                        </Row>
                        <Row id="commissionInfo">
                            {/*** 佣金方案*/}
                            <Col span={24}> <CommissionInfo buildingInfo={buildingInfo} /></Col>
                        </Row>
                        <Row id="batchBuildingInfo">
                            {/*** 楼栋批次信息 */}
                            <Col span={24}><BuildingNoInfo buildingInfo={buildingInfo} /> </Col>
                        </Row>

                        <Row id="supportInfo">
                            {/*** 配套信息 */}
                            <Col span={24}> <SupportInfo buildingInfo={buildingInfo} /></Col>
                        </Row>

                        <Row id="relShopsInfo">
                            {/*** 商铺整体概况 */}
                            <Col span={24}><RelShopsInfo buildingInfo={buildingInfo} /></Col>
                        </Row>

                        <Row id="projectInfo">
                            {/*** 项目简介 */}
                            <Col span={24}><ProjectInfo buildingInfo={buildingInfo} /></Col>
                        </Row>

                        {/* <Row id="attachInfo"> */}
                            {/*** 附加信息 */}
                            {/* <Col span={24}><AttachInfo buildingInfo={buildingInfo} /></Col> */}
                        {/* </Row> */}
                    </Content>
                </Layout>
            </div>
        )
    }
}

export default XYHBuildingDishInfo;