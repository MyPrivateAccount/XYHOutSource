import { connect } from 'react-redux';
import { editShopSummaryInfo } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Form, Icon, Button, Checkbox, Row, Col } from 'antd'
import BasicInfo from './basicInfo';

class ShopSummaryInfo extends Component {
    state = {

    }
    componentWillMount() {

    }
    handleEdit = (e) => {
        this.props.dispatch(editShopSummaryInfo());
    }
    render() {
        let summary = this.props.shopsInfo.summary;
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal" >
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>商铺简介</span>
                        </Col>
                        <Col span={4}>
                            {
                                [1, 8].includes(this.props.shopsInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            }
                        </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={24}>{summary || '暂无'}</Col>
                    </Row>
                </Form>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
      shopsInfo: state.shop.shopsInfo
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(ShopSummaryInfo);