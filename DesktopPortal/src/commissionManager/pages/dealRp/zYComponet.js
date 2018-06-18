//转移对话框
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Row, Col, Form, Input, Modal, TreeSelect, Select,Spin } from 'antd'
import { orgGetPermissionTree, getEmpList } from '../../actions/actionCreator'
import SearchCondition from '../../constants/searchCondition'
const FormItem = Form.Item;
const Option = Select.Option;

class ZYComponet extends Component {

    state = {
        isDataLoading: false,
        vs: false,
        empList: []
    }
    componentDidMount() {

        this.props.onZYSelf(this);

        if (this.props.permissionOrgTree.AddUserTree.length == 0) {
            this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
        }
    }
    componentWillReceiveProps = (newProps) => {
        this.setState({ isDataLoading: false });
        if (newProps.empList !== null && newProps.empList.length >= 0) {
            this.setState({ empList: newProps.empList })
        }
    }
    handleOrgChange = (e) => {
        this.setState({ isDataLoading: true });
        //组织改变,再查询该组织下面的员工
        SearchCondition.ppFtListCondition.OrganizationIds = []
        SearchCondition.ppFtListCondition.OrganizationIds.push(e)
        this.props.dispatch(getEmpList(SearchCondition.ppFtListCondition))
    }
    handleOk = (e) => {

    }
    handleCancel = (e) => {
        this.setState({ vs: false })
    }
    show = () => {
        this.setState({ vs: true })
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        let empList = this.state.empList
        return (
            <Modal title={'转移'} width={400} maskClosable={false} visible={this.state.vs}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Spin spinning={this.state.isDataLoading}>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    组织
                                </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('branchId', {

                                initialValue: '',
                            })(
                                <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.AddUserTree}
                                    placeholder="所属组织"
                                    defaultValue={this.props.orgid}
                                    onChange={this.handleOrgChange}>
                                </TreeSelect>
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>员工</span>)}>
                            {getFieldDecorator('parCode', {
                                initialValue: ''
                            })(
                                <Select style={{ width: 300 }}>
                                    {
                                        empList.map(tp => <Option key={tp.id} value={tp.id}>{tp.userName}</Option>)
                                    }
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                </Row>
                </Spin>
            </Modal>
        )
    }
}
function MapStateToProps(state) {

    return {
        permissionOrgTree: state.org.permissionOrgTree,
        empList: state.org.empList
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(ZYComponet);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);