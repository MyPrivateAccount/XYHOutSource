import React, { Component } from 'react';
import { DatePicker, Modal, Icon, Select, Form, Checkbox, Input, InputNumber, TreeSelect, Row, Col, Button, notification, Spin } from 'antd'
import { AuthorUrl, basicDataServiceUrl } from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import { getDicPars, getOrganizationTree } from '../../../utils/utils'
import { getDicParList } from '../../../actions/actionCreators'
import validations from '../../../utils/validations'
import { permission } from './const'

const FormItem = Form.Item;
const Option = Select.Option;

const PaymentForm = Form.create()(
    class extends React.Component {

        state = {
            nodes: [],
            userList: [],
            fetchingUser: false
        }

        componentDidMount = () => {
            this.getNodes();
        }

        getNodes = async () => {
            let url = `${AuthorUrl}/api/Permission/${permission.bxxe}`;
            let r = await ApiClient.get(url, true);
            if (r && r.data && r.data.code === '0') {
                var nodes = getOrganizationTree(r.data.extension);
                this.setState({ nodes: nodes });
            } else {
                notification.error(`获取部门列表失败:${((r || {}).data || {}).message || ''}`);
            }
        }
        fetchUser = async (value) => {
            let dp = this.props.form.getFieldsValue(["department"])["department"];
            let url = `${basicDataServiceUrl}/api/humaninfo/simpleSearch`;
            var r = await ApiClient.get(url, true, { permissionId: permission.bxxe, branchId: (dp||''), keyword: value, pageSize: 0, pageIndex: 0 });
            if (r && r.data && r.data.code === '0') {
                this.setState({ userList: r.data.extension })
            }
        }
        selectedUser = (value) => {
            this.setState({
                data: [],
                fetching: false,
            });
            if(!value || (value && value.length===0)){
                this.props.form.setFieldsValue({ userId: [] })
            }else{
                setTimeout(() => {
                    this.props.form.setFieldsValue({ userId: [value[value.length - 1]] })
    
                }, 0);
            }
            
            this.setState({userList:[]})
        }
        onCancel = () => {
            if (this.props.onCancel) {
                this.props.onCancel();
            }
        }

        confirm = () => {
            const { onSubmit, form } = this.props;
            this.props.form.validateFieldsAndScroll();
            var errors = this.props.form.getFieldsError();
            
            if(validations.checkErrors(errors)){
                notification.error({message:'验证失败', description:'表单验证失败，请检查'});
                return;
            }
            var vals = form.getFieldsValue();
            var entity = {...vals};
            entity.userId = entity.userId[0].key;

            if (onSubmit) {
                onSubmit(entity);
            }
        }

        departmentChanged = (...args) => {
            this.props.form.setFieldsValue({ userId: [] })
        }

        componentWillReceiveProps = (nextProps) => {
            if (this.props.visible !== nextProps.visible && nextProps.visible) {
                this.props.form.resetFields();

                this.initData(nextProps);
            }
        }

        initData = async (nextProps)=>{
            if(nextProps.entity){
                await this.fetchUser(nextProps.entity.userId);
                let item = {...nextProps.entity};
                item.userId = [{key: item.userId}]
                item.department = item.userInfo.departmentId;

                this.props.form.setFieldsValue(item);
                
            }

           
        }


        render() {
            const { visible, onCancel, form, title, loading } = this.props;
            const { fetchingUser, userList } = this.state;
            const { getFieldDecorator } = form;
            const formItemLayout = {
                labelCol: {
                    xs: { span: 24 },
                    sm: { span: 4 },
                },
                wrapperCol: {
                    xs: { span: 24 },
                    sm: { span: 18 },
                },
            };

            let canSelectUser = form.getFieldsValue(["department"])["department"];
            let btns = [];
            if (this.props.canEdit) {
                btns.push(<Button key="back" onClick={onCancel}>取消</Button>)
                btns.push(<Button key="submit" type="primary" disabled={loading} onClick={this.confirm}>
                    确认
          </Button>)
            } else {
                btns.push(<Button key="back" onClick={onCancel}>关闭</Button>)
            }
            return (
                <Modal
                    visible={visible}
                    title={title}
                    onCancel={onCancel}
                    footer={btns}
                >
                    <Form>
                        <FormItem label="部门"  {...formItemLayout}>
                            {getFieldDecorator('department',{
                                 rules:[{required:true, message:"请选择部门"}]
                            })(
                                <TreeSelect
                                    disabled={!this.props.canEdit}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.state.nodes}
                                    onChange={this.departmentChanged}
                                    placeholder="请选择部门"
                                />
                            )}
                        </FormItem>
                        <FormItem label="人员" {...formItemLayout}>
                            {getFieldDecorator('userId',{
                                 rules:[{required:true, message:"请选择人员"}]
                            })(
                                <Select
                                    disabled={!this.props.canEdit || !canSelectUser}
                                    mode="multiple"
                                    maxTagCount={1}
                                    labelInValue
                                    placeholder="输入姓名、员工编号或手机号码"
                                    notFoundContent={fetchingUser ? <Spin size="small" /> : null}
                                    filterOption={false}
                                    onSearch={this.fetchUser}
                                    onChange={this.selectedUser}
                                    style={{ width: '100%' }}
                                >
                                    {userList.map(d => <Option key={d.id}>{`${d.name}(${d.organizationFullName})`}</Option>)}
                                </Select>
                            )}
                        </FormItem>
                        <FormItem label="限额" {...formItemLayout}>
                            {getFieldDecorator('amount', {
                                 rules:[{required:true, message:"请输入限额"}]
                            })(<InputNumber style={{ minWidth: '10rem' }} min={0} precision={2} disabled={!this.props.canEdit} />)}
                        </FormItem>
                        <FormItem label="备注" {...formItemLayout}>
                            {getFieldDecorator('memo',{
                                 rules:[{max:200, message:"备注不超过200字"}]
                            })(<Input maxLength={200} disabled={!this.props.canEdit} type="textarea" placeholder="请输入备注" />)}
                        </FormItem>

                    </Form>
                </Modal>
            );
        }
    }
);

export default PaymentForm;