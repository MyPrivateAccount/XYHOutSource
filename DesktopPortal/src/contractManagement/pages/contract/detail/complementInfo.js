import { connect } from 'react-redux';
import { contractComplementEdit, openComplementStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Table, Button, Checkbox, Row, Col, Form } from 'antd'
import moment from 'moment';

class ComplementInfo extends Component {

    handleEdit = (e) => {
        e.preventDefault();
        this.props.dispatch(contractComplementEdit());
        this.props.dispatch(openComplementStart({ id: 2 }));
    }
    render() {
        console.log('this.props.complementInfos.examineStatus:', this.props.complementInfos.examineStatus);
        let complementInfo = this.props.complementInfo || [];
        let hasBCXYPermission = false;//补充协议权限
        if (this.props.judgePermissions && this.props.judgePermissions.includes('HT_BCXY')) {
            hasBCXYPermission = true;
        }
        return (
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal">
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>补充协议</span>
                        </Col>
                        {hasBCXYPermission ?
                            <Col span={4}>
                                {
                                    [1].includes(this.props.complementInfos.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                    //<Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                }
                            </Col> : null
                        }

                    </Row>

                    <Row className='viewRow'>
                        {
                            complementInfo.length === 0 ? <div style={{ marginLeft: '20px' }}>{'暂无信息'}</div> :

                                complementInfo.map((item, i) => {
                                    return <Col span={24} key={i}>补充内容{i + 1}:{item.contentInfo}</Col>
                                })

                        }

                    </Row>
                </Form>

            </div>
        )
    }
}

function mapStateToProps(state) {

    return {
        contractInfo: state.contractData.contractInfo,
        basicData: state.basicData,
        complementInfo: state.contractData.contractInfo.complementInfos.complementInfo,
        complementInfos: state.contractData.contractInfo.complementInfos,
        judgePermissions: state.judgePermissions
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(ComplementInfo);