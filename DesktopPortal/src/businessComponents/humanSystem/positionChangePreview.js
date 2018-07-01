import React, { Component } from 'react'
import { Layout, Icon, Row, Col, Form, TreeSelect } from 'antd'
import SocialSecurity from './socialSecurity'

class PositionChangePreview extends Component {
    state = {

    }
    componentDidMount() {

    }

    render() {
        let entityInfo = this.props.entityInfo;
        let disabled = (this.props.readOnly || false);
        let setDepartmentOrgTree = this.props.departmentTree || [];
        let positionData = this.props.positionData || [];
        return (
            <div>
                <Row style={{ marginTop: '10px' }}>
                    <Col span={7}>
                        <label class="ant-form-item-label">员工编号:</label>
                        <Input style={{ width: '200px' }} disabled value={entityInfo.userID} />
                    </Col>
                    <Col span={7}>
                        <label class="ant-form-item-label">姓名:</label>
                        <Input style={{ width: '200px' }} disabled value={entityInfo.name} />
                    </Col>
                    <Col span={7}></Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <label class="ant-form-item-label">原部门:</label>
                        <TreeSelect disabled treeData={setDepartmentOrgTree} value={entityInfo.departmentId} />
                    </Col>
                    <Col span={7}>
                        <label class="ant-form-item-label">原职位:</label>
                        <Select disabled={true} value={entityInfo.position} placeholder="选择职位">
                            {
                                (positionData || []).map(item => <Option key={item.key} value={item.id}>{item.stationname}</Option>)
                            }
                        </Select>
                    </Col>
                    <Col span={7}></Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <label class="ant-form-item-label">新部门:</label>
                        <TreeSelect disabled treeData={setDepartmentOrgTree} value={entityInfo.newDepartmentId} />
                    </Col>
                    <Col span={7}>
                        <label class="ant-form-item-label">新职位:</label>
                        <Select disabled={true} value={entityInfo.newStation} placeholder="选择职位">
                            {
                                (positionData || []).map(item => <Option key={item.key} value={item.id}>{item.stationname}</Option>)
                            }
                        </Select>
                    </Col>
                    <Col span={7}></Col>
                </Row>

                <SocialSecurity readOnly entityInfo={entityInfo} />
                <Salary readOnly entityInfo={entityInfo} />

            </div>
        )
    }

}
export default PositionChangePreview;
