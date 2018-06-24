import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, Button, Radio, Select, Table, Row, Col, Form, Input, TreeSelect, DatePicker, notification } from 'antd'
import { AuthorUrl, basicDataServiceUrl } from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import { getDicPars, getOrganizationTree } from '../../../utils/utils'
import { getDicParList } from '../../../actions/actionCreators'
import Layer, { LayerRouter } from '../../../components/Layer'
import { Route } from 'react-router'
import LimitDialog from './LimitDialog'
import moment from 'moment'
import { permission } from './const'

const FormItem = Form.Item;
const RadioGroup = Radio.Group;
const ButtonGroup = Button.Group;
const confirm = Modal.confirm;

class FeeLimitSetting extends Component {

    state = {
        nodes: [],
        pagination: { pageSize: 10, pageIndex: 1 },
        list: [],
        permission: {},
        loading: false,
        pagePar: {},
        canEdit: false
    }

    componentDidMount = () => {
        let initState = (this.props.location || {}).state || {};
        this.setState({ pagePar: initState })


        this.getNodes();

        this.props.form.setFieldsValue({
            isBackup: null,
            isPayment: null
        })

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

    gotoDetail = (item, op) => {
        this.props.history.push(`${this.props.match.url}/chargeInfo`, { entity: item.chargeInfo, op: op || 'view', pagePar: this.state.pagePar })
    }

    clickSearch = () => {
        this.setState({
            pagination: { ...this.state.pagination, ...{ pageIndex: 1 } }
        }, () => {
            this.search();
        })
    }

    search = async () => {
        let condition = this.props.form.getFieldsValue();
        condition = { ...condition };
        condition.pageSize = this.state.pagination.pageSize;
        condition.pageIndex = this.state.pagination.pageIndex;
        console.log(condition);
        this.setState({ loading: true })
        try {
            let url = `${basicDataServiceUrl}/api/chargeinfo/limit/search`;
            let r = await ApiClient.post(url, condition);
            if (r && r.data && r.data.code === '0') {
                this.setState({ list: r.data.extension });
            } else {
                notification.error({ message: '查询费用限额列表失败' });
            }
        } catch (e) {
            notification.error({ message: '查询费用限额列表失败' })
        }
        this.setState({ loading: false })
    }

    handleTableChange = (pagination, filters, sorter) => {
        this.setState({
            pagination: { ...this.state.pagination, ...{ pageIndex: pagination.current } }
        }, () => {
            this.search();
        })
    }

    addLimit = () => {
        this.setState({curItem: null, visible: true, canEdit: true })
    }
    closeDialog = () => {
        this.setState({ visible: false })
    }
    submit = async (entity) => {

        try {
            let url = `${basicDataServiceUrl}/api/chargeinfo/limit`;
            let r = await ApiClient.post(url, entity);
            if (r && r.data && r.data.code === '0') {
                this.search();
            } else {
                notification.error({ message: '保存费用限额失败' });
            }
        } catch (e) {
            notification.error({ message: '保存费用限额失败' })
        }
        this.setState({ visible: false, canEdit: false })
    }

    delete = async (record) => {
        //删除
        confirm({
            title: `您确定要删除此项费用限额设定么?`,
            content: '',
            onOk: async () => {
                let url = `${basicDataServiceUrl}/api/chargeinfo/limit/${record.userId}`
                let r = await ApiClient.post(url, null, null, 'DELETE');
                if (r && r.data && r.data.code === '0') {
                    notification.success({ message: '删除成功', description: '费用限额设定已被删除' })
                    let l = this.state.list;
                    let idx = l.findIndex(x => x.userId === record.userId);
                    if (idx >= 0) {
                        l.splice(idx, 1);
                        this.setState({ list: [...l] })
                    }
                } else {
                    notification.error({ message: `删除失败${((r || {}).data || {}).message || ''}` })
                }
            },
            onCancel() {

            },
        });
    }

    edit = (record) => {
        this.setState({ curItem: record, visible: true, canEdit: true })
    }


    render() {
        const { getFieldDecorator } = this.props.form;

        const columns = [
            {
                title: '部门',
                dataIndex: 'departmentName',
                key: 'departmentName'
            },
            {
                title: '员工编号',
                dataIndex: 'userInfo.userId',
                key: 'userInfo.userId',
                width: '12rem'
            },
            {
                title: '姓名',
                dataIndex: 'userInfo.name',
                key: 'userInfo.name',
                width: '10rem'
            },
            {
                title: '职位',
                dataIndex: 'userInfo.positionInfo.positionName',
                key: 'userInfo.positionInfo.positionName',
                width: '10rem'
            },
            {
                title: '限额',
                dataIndex: 'amount',
                key: 'amount',
                className: 'column-money',
                width: '8rem',
                render: (text, record) => {
                    return <span style={{ textAlign: 'right' }}>{record.amount}</span>
                }
            },
            {
                title: '备注',
                dataIndex: 'memo',
                key: 'memo'
            }, {
                title: '操作',
                width: '12rem',
                render: (text, record) => (
                    <ButtonGroup>
                        <Button onClick={() => this.edit(record)}>修改</Button>
                        <Button style={{ marginLeft: '0.5rem' }} onClick={() => this.delete(record)}>删除</Button>
                    </ButtonGroup>
                )
            }

        ]
        return <Layer className="content-page">
            <div style={{ marginTop: '1.5rem' }}>
                <Form>
                    <Row className="form-row">
                        <Col span={8}>
                            <FormItem label="部门">
                                {getFieldDecorator('departmentId', {

                                })(
                                    <TreeSelect
                                        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                        treeData={this.state.nodes}
                                        placeholder="部门"
                                    />
                                )}
                            </FormItem>
                        </Col>

                        <Col span={16} style={{ display: 'flex' }}>
                            <FormItem label="关键字" style={{ flex: 1 }}>
                                {
                                    getFieldDecorator('keyword')(
                                        <Input placeholder="员工编号、姓名" />
                                    )
                                }

                            </FormItem>
                            <Button style={{ marginLeft: '1rem' }} onClick={this.search}>搜索</Button>
                        </Col>
                    </Row>



                </Form>
            </div>
            <div className="page-btn-bar">
                {
                    this.state.pagePar.noAdd ? null : <Button type="primary" onClick={this.addLimit}>添加限额</Button>
                }
            </div>
            <div className="page-fill">
                <Table style={{ width: '100%' }} columns={columns} dataSource={this.state.list}
                    loading={this.state.loading}
                    rowKey="id"
                    pagination={this.state.pagination}
                    onChange={this.handleTableChange}
                />
            </div>
            <LimitDialog visible={this.state.visible} 
                canEdit={this.state.canEdit} 
                onCancel={this.closeDialog} 
                onSubmit={this.submit} 
                entity = {this.state.curItem}
                title="费用限额" />
        </Layer>
    }
}

const mapStateToProps = (state, props) => ({
    dic: state.basicData.dicList,
    user: state.oidc.user.profile
})
const mapActionToProps = (dispatch) => ({
    getDicParList: (...args) => dispatch(getDicParList(...args))
})

export default connect(mapStateToProps, mapActionToProps)(Form.create()(FeeLimitSetting))