import { connect } from 'react-redux';
import { getDicParList } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Radio, Row, Col, Icon } from 'antd'
import BasicInfo from '../detail/basicInfo';
import ProjectInfo from '../detail/projectInfo';
import RelShopsInfo from '../detail/relShopsInfo';
import SupportInfo from '../detail/supportInfo';
import BuildingNoInfo from '../detail/buildingNoInfo';
import RulesInfo from '../detail/rulesInfo';
import RulesTemplateInfo from '../detail/rulesTemplateInfo';
import CommissionInfo from '../detail/commissionInfo';
import AttachInfo from '../../shops/detail/attachInfo'
import { buildInfoGroup } from '../../../constants/commonDefine';
import '../edit/buildingDish.less';

const { Header, Sider, Content } = Layout;

class BuildingDishInfo extends Component {
    state = {
        visible: false,
    }
    componentWillMount() {
        if (this.props.basicData.saleStatus.length === 0
            && this.props.basicData.saleModel.length === 0) {
            this.props.dispatch(getDicParList(["TRADE_MIXPLANNING", "SALE_MODE", "PROJECT_SALE_STATUS", "SHOP_CATEGORY"]));
        }
    }
    handleAnchorChange = (e) => {
        console.log(e);
        window.location.href = "#" + e.target.value;
    }


    render() {
        return (
            <div className="relative">
                <Layout>
                    <Content className='content buildingDish' style={{ marginTop: '25px' }}>
                        <Row type="flex" justify="space-between">
                            <Col span={4} style={{ textAlign: 'left' }}>
                            </Col>
                            <Col span={20} style={{ textAlign: 'right' }}>
                                <Radio.Group defaultValue='basicInfo' onChange={this.handleAnchorChange} size='large'>
                                    {
                                        buildInfoGroup.map((info) => <Radio.Button value={info.id} key={info.id}>{info.name}<Icon type="check" /></Radio.Button>)
                                    }
                                </Radio.Group></Col>
                        </Row>
                        <Row id="basicInfo">
                            {/** * 基本信息 */}
                            <Col span={24}> <BasicInfo /></Col>
                        </Row>
                        <Row id="rulesInfo">
                            {/*** 报备规则*/}
                            <Col span={24}>
                                <RulesInfo />
                                <RulesTemplateInfo />
                            </Col>
                        </Row>
                        <Row id="commissionInfo">
                            {/*** 佣金方案*/}
                            <Col span={24}> <CommissionInfo /></Col>
                        </Row>
                        <Row id="batchBuildingInfo">
                            {/*** 楼栋批次信息 */}
                            <Col span={24}><BuildingNoInfo /> </Col>
                        </Row>

                        <Row id="supportInfo">
                            {/*** 配套信息 */}
                            <Col span={24}> <SupportInfo /></Col>
                        </Row>

                        <Row id="relShopsInfo">
                            {/*** 商铺整体概况 */}
                            <Col span={24}><RelShopsInfo /></Col>
                        </Row>

                        <Row id="projectInfo">
                            {/*** 项目简介 */}
                            <Col span={24}><ProjectInfo /></Col>
                        </Row>

                        <Row id="attachInfo">
                            {/*** 附加信息 */}
                            <Col span={24}><AttachInfo parentPage="building" /></Col>
                        </Row>
                    </Content>
                </Layout>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        buildInfo: state.building.buildInfo,
        basicData: state.basicData,
        operInfo: state.building.operInfo
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BuildingDishInfo);