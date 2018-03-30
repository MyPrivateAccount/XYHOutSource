import {connect} from 'react-redux';
import {closeAdjustCustomer, getUserByOrg, adjustCustomer, setLoadingVisible} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Row, Col, Modal, Select, TreeSelect, Form, Table} from 'antd';

const Option = Select.Option;
const FormItem = Form.Item;
class AdjustCustomer extends Component {
    state = {
        targetOrg: {},//目标部门

    }
    componentWillMount() {
        let userInfo = this.props.oidc.user.profile;
        // console.log(this.props.orgInfo, 'aa', userInfo)
        if (this.props.orgInfo.orgList.length === 0) {
            this.props.dispatch(getUserByOrg({
                organizationIds: [userInfo.Organization],
                pageIndex: 0,
                pageSize: 10000
            }));
            this.setState({targetOrg: {id: userInfo.Organization}});
        }
    }
    componentWillReceiveProps(newProps) {
        // if (newProps.userList && newProps.userList.length > 0) {
        //     this.setState({ targetEmp: newProps.userList[0].id });
        // } else {
        //     this.setState({ targetEmp: '' });
        // }
    }

    handleOk = (e) => {
        e.preventDefault();
        let userList = this.props.userList;
        let activeCustomers = this.props.activeCustomers;
        let customerIds = [];
        activeCustomers.map(customer => {
            customerIds.push({id: customer.id, name: customer.customerName});
        });
        this.props.form.validateFields((err, values) => {
            if (!err) {
                let condition = {...this.props.searchCondition};
                let requestInfo = {
                    sourceUserId: activeCustomers[0].userId,
                    sourceUserName: activeCustomers[0].userName,
                    sourceDepartmentId: activeCustomers[0].departmentId,
                    sourceDepartmentName: activeCustomers[0].departmentName,
                    terUserId: values.targetUserID,
                    terUserName: userList.find(u => u.id === values.targetUserID).trueName,
                    terDepartmentId: values.targetDepartmentID ? values.targetDepartmentID : this.state.targetOrg.id,
                    terDepartmentName: this.state.targetOrg.departmentName,
                    customers: customerIds,
                    isKeep: true,
                    searchCondition: condition
                };
                // console.log("调客请求requestInfo:", requestInfo);
                this.props.dispatch(setLoadingVisible(true));
                this.props.dispatch(adjustCustomer(requestInfo));
            }
        });
    }
    handleCancel = () => {
        this.props.dispatch(closeAdjustCustomer());
    }

    handleOrgChange = (e, label, extra) => {
        // console.log("部门切换:", e, label, extra);
        this.setState({targetOrg: {id: e, departmentName: label[0]}});
        this.props.dispatch(getUserByOrg({organizationIds: [e], pageIndex: 0, pageSize: 10000}));
    }
    getCustomerColumns() {
        let columms = [{
            title: '客户姓名',
            dataIndex: 'customerName',
        }, {
            title: '客户性别',
            dataIndex: 'sex',
            render: (text, record) => (<span>{text ? "男" : "女"}</span>)
        }, {
            title: '当前部门',
            dataIndex: 'departmentName',
        }, {
            title: '当前归属人',
            dataIndex: 'userName',
        }];
        return columms;
    }
    render() {
        let showLoading = this.props.showLoading;
        let orgList = this.props.orgInfo.orgList;
        let activeCustomers = this.props.activeCustomers;
        let showAdjustCustomer = this.props.showAdjustCustomer;
        let userList = this.props.userList;
        // console.log(userList, 'userList')
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 4},
            wrapperCol: {span: 20},
        };
        return (
            <Modal title="调客" confirmLoading={showLoading} className='adjustCustomer' maskClosable={false} visible={showAdjustCustomer} onOk={this.handleOk} onCancel={this.handleCancel}>
                <Table columns={this.getCustomerColumns()} dataSource={activeCustomers} bordered size="middle" pagination={false} />
                <Row>
                    {
                        orgList.length > 0 ?
                            <Col span={24} pull={1}>
                                <FormItem {...formItemLayout} label={(<span>目标部门</span>)}>
                                    {getFieldDecorator('targetDepartmentID', {
                                        rules: [{required: true, message: '请选择目标部门!'}],
                                    })(
                                        <TreeSelect style={{width: 300}}
                                            dropdownStyle={{maxHeight: 400, overflow: 'auto'}}
                                            treeData={orgList} placeholder="请选择目标部门"
                                            onChange={this.handleOrgChange}
                                        />
                                        )}
                                </FormItem>
                            </Col> : null
                    }

                </Row>
                <Row>
                    <Col span={24} pull={1}>
                        <FormItem {...formItemLayout} label={(<span>归属人</span>)}>
                            {getFieldDecorator('targetUserID', {
                                rules: [{required: true, message: '请选择归属人!'}],
                            })(
                                <Select style={{width: 120}} placeholder="请选择归属人">
                                    {
                                        userList.map(user => <Option key={user.id} value={user.id}>{user.trueName}</Option>)
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>
                </Row>
            </Modal>
        )
    }
}

function mapStateToProps(state) {
    return {
        activeCustomers: state.search.activeCustomers,
        searchCondition: state.search.searchCondition,
        showAdjustCustomer: state.search.showAdjustCustomer,
        orgInfo: state.basicData.orgInfo,
        userList: state.basicData.userList,
        showLoading: state.search.showLoading,
        oidc: state.oidc,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(AdjustCustomer);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedRegistrationForm);