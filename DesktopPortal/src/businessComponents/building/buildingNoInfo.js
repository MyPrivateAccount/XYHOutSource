import React, {Component} from 'react'
import {Icon, Table, Layout, Row, Col} from 'antd'
import moment from 'moment';


const {Column, ColumnGroup} = Table;
const {Header, Sider, Content} = Layout;

class BuildingNoInfo extends Component {
    state = {
        projectId: '',
        expandStatus: true
    }
    componentWillMount() {

    }

    render() {
        let buildingNoInfo = this.props.buildingInfo.buildingNoInfos || [];
        if (Object.keys(buildingNoInfo).length > 0) {
            buildingNoInfo.forEach(v => {
                if (v.openDate && v.openDate !== "") {
                    v.openDate = moment(v.openDate).format("YYYY-MM-DD");
                }
                if (v.openDate === 'Invalid date') {v.openDate = ''}
                v.deliveryDate = v.deliveryDate ? moment(v.deliveryDate).format('YYYY-MM-DD') : null;
            })
            buildingNoInfo.sort((obj1, obj2) => {
                return obj1.storied > obj2.storied
            })
        } else {
            buildingNoInfo = [];
        }
        return (
            <Layout>
                <Content className='content' >
                    <div style={{marginTop: '15px', backgroundColor: "#ECECEC"}} className='buildingInfo'>
                        <Row type="flex" style={{padding: '1rem 0'}}>
                            <Col span={23} >
                                <Icon type="tags-o" className='content-icon' /> <span className='content-title'>楼栋批次信息</span>
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                        </Row>
                        <Row style={{display: this.state.expandStatus ? "block" : "none"}}>
                            <Table dataSource={buildingNoInfo} style={{padding: ' 20px'}}>
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
                </Content>
            </Layout>
        )
    }
}

export default BuildingNoInfo;