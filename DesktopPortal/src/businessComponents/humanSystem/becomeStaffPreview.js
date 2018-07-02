import React, { Component } from 'react'
import { Layout, Icon, Row, Col, Form } from 'antd'
import SocialSecurity from './socialSecurity'
class BecomeStaffPreview extends Component {
    state = {

    }
    componentDidMount() {

    }

    render() {
        let entityInfo = this.props.entityInfo;
        let disabled = (this.props.readOnly || false);
        return (
            <div>
                <Row>
                    <Col span={21}>
                        <div className="page-title" style={{ marginBottom: '10px' }}>转正</div>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <label class="ant-form-item-label">员工编号:</label><Input style={{ width: '200px' }} disabled value={entityInfo.userID} />
                    </Col>
                    <Col span={7}>
                        <label class="ant-form-item-label">姓名:</label><Input style={{ width: '200px' }} disabled value={entityInfo.name} />
                    </Col>
                    <Col span={7}>
                        <label class="ant-form-item-label">转正实际生效时间:</label>
                        <DatePicker disabled={true} value={socialSecurityInfo.entryTime} format='YYYY-MM-DD' style={{ width: '100%' }} />
                    </Col>
                </Row>
                <SocialSecurity readOnly entityInfo={entityInfo} />
            </div>
        )
    }

}
export default BecomeStaffPreview;
