import { connect } from 'react-redux';
import { createStation, getOrgList, getDicParList, setStation, deleteStation, getcreateStation, setSearchLoadingVisible } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Table, notification, Button, Row, Col, Spin, TreeSelect, Popconfirm, Modal, Tooltip } from 'antd'
import './station.less';
import Layer, { LayerRouter } from '../../../components/Layer'
import { Route } from 'react-router'
import AddStation from './addstation'
import { getDicPars } from '../../../utils/utils';
import ApiClient from '../../../utils/apiClient'
import WebApiConfig from '../../constants/webapiConfig';
import Achievement from './achievementForm'
import { editSalary, addSalary, getSalaryDetail } from '../../serviceAPI/salaryService'
const styles = {
    conditionRow: {
        width: '80px',
        display: 'inline-block',
        fontWeight: 'bold',
    },
    bSpan: {
        fontWeight: 'bold',
    },
    otherbtn: {
        padding: '0px, 5px',
    }
}

const rowSelection = {
    onChange: (selectedRowKeys, selectedRows) => {
        console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
    },
    getCheckboxProps: record => ({
        disabled: record.name === 'Disabled User', // Column configuration not to be checked
        name: record.name,
    }),
};


class Station extends Component {
    state = {
        stationList: [],
        showLoading: false,
        condition: {
            departmentId: ''
        },
        showSalaryDetail: false,
        curStation: {},
        curAchievementInfo: {},
        achievementForm: null,
        confirmLoading: false
    }

    gotoSubPage = (path, params) => {
        this.props.history.push(`${this.props.match.url}/${path}`, { ...params })
    }

    selectChange(key, v) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            target.positionType = v;
        }
    }

    handleChange(value, key, column) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            target[column] = value;
            this.forceUpdate();
        }
    }

    edit(recordInfo) {
        this.gotoSubPage("addStation", recordInfo);
    }
    //职位薪资信息
    salaryEdit = (record) => {
        this.setState({ showSalaryDetail: true, curAchievementInfo: record, confirmLoading: true });
        getSalaryDetail(record.id).then(res => {
            this.setState({ confirmLoading: false });
            if (res.isOk) {
                this.setState({ curStation: res.extension });
            }
        })
    }

    //子页面回调
    subPageLoadCallback = (formObj, pageName) => {
        if (pageName == "achievement") {
            this.setState({ achievementForm: formObj });
        }
    }

    submitSalary = () => {
        let { curStation, curAchievementInfo } = this.state;
        if (this.state.achievementForm) {
            this.state.achievementForm.validateFields((err, values) => {
                if (!err) {
                    values.id = curStation.id ? curStation.id : null;
                    console.log("表单内容:", values);
                    this.setState({ showLoading: true, confirmLoading: true });
                    if (values.id) {
                        editSalary(values).then(res => {
                            this.setState({ showLoading: false, confirmLoading: false });
                            if (res.isOk) {
                                this.props.form.resetFields();
                            }
                        });
                    } else {
                        addSalary(values).then(res => {
                            this.setState({ showLoading: false, confirmLoading: false });
                            if (res.isOk) {
                                this.props.form.resetFields();
                            }
                        });
                    }
                }
            });
        }
    }

    achievementClose = () => {
        this.setState({ showSalaryDetail: false });
        if (this.state.achievementForm) {
            this.state.achievementForm.resetFields()
        }
    }

    delete = (record) => {
        let url = WebApiConfig.server.DeleteStation;
        let huResult = { isOk: false, msg: '删除职位失败！' };

        ApiClient.post(url, record).then(res => {
            if (res.data.code == 0) {
                huResult.isOk = true;
                huResult.msg = '删除职位成功！';
                this.search();
            }
            notification[huResult.isOk ? 'success' : 'error']({
                description: huResult.msg,
                duration: 3
            });
        }).catch(e => {
            huResult.msg = "删除职位接口调用异常!";
            notification.error({
                description: huResult.msg,
                duration: 3
            });
        });
    }

    cancel(key) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            if (target.isnew) {
                this.props.stationList.splice(this.props.stationList.findIndex(item => key === item.key), 1);
                this.forceUpdate();
                return;
            }
            Object.assign(target, this.cacheData.filter(item => key === item.key)[0]);
            delete target.editable;
            this.forceUpdate();
        }
    }

    componentWillMount() {
        this.props.dispatch(getDicParList(["POSITION_TYPE"]));
        this.props.dispatch(setSearchLoadingVisible(false));
    }

    componentDidMount() {
        let dicPositions = getDicPars("POSITION_TYPE", this.props.rootBasicData);
        this.setState({ dicPositions: dicPositions }, () => {
            let ListColums = [
                {
                    title: '职位名称',
                    dataIndex: 'positionName',
                    key: 'positionName'
                },
                {
                    title: '职位类型',
                    dataIndex: 'positionType',
                    key: 'positionType',
                    render: (text, record) => {
                        let positionObj = this.state.dicPositions.find(item => item.value === record.positionType);
                        return (
                            <div>
                                {positionObj ? positionObj.key : ''}
                            </div>
                        );
                    }
                },
                {
                    title: '所属分公司',
                    dataIndex: 'parentID',
                    key: 'parentID',
                    render: (text, record) => {
                        return (
                            <TreeSelect disabled value={text}
                                treeData={this.props.setContractOrgTree}
                            />
                        );
                    }
                },
                {
                    title: '操作',
                    dataIndex: 'operation',
                    render: (text, record) => {
                        const { editable } = record;
                        return (
                            <div className="editable-row-operations">
                                <Button type="primary" size='small' shape="circle" icon="edit" style={{ marginRight: '10px' }} onClick={() => this.edit(record)}></Button>
                                <Popconfirm title="确认删除该记录?" onConfirm={() => this.delete(record)} okText="确认" cancelText="取消">
                                    <Button type="primary" size='small' shape="circle" icon="delete" style={{ marginRight: '10px' }}></Button>
                                </Popconfirm >
                                <Tooltip title="职位薪酬管理">
                                    <Button type="primary" size='small' shape="circle" icon="red-envelope" style={{ marginRight: '10px' }} onClick={() => this.salaryEdit(record)}></Button>
                                </Tooltip>
                            </div>
                        );
                    },
                },
            ]
            this.setState({ ListColums: ListColums });
        });

    }

    handleChooseDepartmentChange = (e) => {
        let condition = this.state.condition;
        condition.departmentId = e;
        this.setState({ condition: condition });
    }

    handleAddNew = (e) => {
        //暂时写个测试
        let nkey = 1;
        if (this.props.stationList.length > 0) {
            nkey = +this.props.stationList[this.props.stationList.length - 1].key + 2;
        }

        this.props.stationList.push({ key: nkey + '', stationname: "test", positionType: "", editable: true, isnew: true });
        this.forceUpdate();
        //this.props.dispatch(adduserPage({id: 11, menuID: 'menu_blackaddnew', disname: '新建黑名单', type:'item'}));
        this.gotoSubPage("addStation", {})
    }
    search = () => {
        let departmentId = this.state.condition.departmentId;
        if (departmentId) {
            this.setState({ showLoading: true });
            let url = WebApiConfig.search.getStationList + "/" + departmentId;
            let huResult = { isOk: false, msg: '获取职位失败！' };
            ApiClient.get(url).then(res => {
                console.log("请求结果:", res);
                this.setState({ showLoading: false });
                if (res.data.code == 0) {
                    huResult.msg = '获取职位成功';
                    this.setState({ stationList: res.data.extension });
                    // yield put({type: actionUtils.getActionType(actionTypes.UPDATE_STATIONLIST), payload: huResult.data.extension});
                }
            }).catch(e => {
                this.setState({ showLoading: false });
                notification.error({
                    message: huResult.msg,
                    duration: 3
                });
            });
        }
    }

    render() {
        let ListColums = this.state.ListColums || [];
        return (
            <Layer showLoading={this.state.showLoading}>
                <div className="searchBox">
                    <Row>
                        <Col style={{ marginTop: '10px' }}>
                            <label style={styles.conditionRow}>选择分公司 ：</label>
                            <TreeSelect style={{ width: '300px', marginRight: '10px' }}
                                allowClear
                                treeData={this.props.setContractOrgTree}
                                onChange={this.handleChooseDepartmentChange}
                                placeholder="请选择所属分公司"
                            />
                            <Button type="primary" className='searchButton' onClick={this.search}>查询</Button>
                        </Col>
                    </Row>
                </div>
                <Row className="btnBlock">
                    <Col style={{ marginBottom: '15px', marginTop: '15px' }}>
                        <Button type="primary" onClick={this.handleAddNew}>新建</Button>
                    </Col>

                </Row>
                <Spin spinning={this.props.showLoading} delay={200} tip="查询中...">
                    <Table rowKey={record => record.key} dataSource={this.state.stationList} columns={ListColums} onChange={this.handleTableChange} bordered />
                </Spin>

                <Modal title="职位对应薪酬" width={720}
                    visible={this.state.showSalaryDetail}
                    confirmLoading={this.state.confirmLoading}
                    onOk={() => this.submitSalary()}
                    onCancel={() => this.achievementClose()}
                >
                    <div>
                        <Achievement location={{ state: this.state.curAchievementInfo }} subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)} />
                    </div>
                </Modal>

                <LayerRouter>
                    <Route path={`${this.props.match.url}/addStation`} render={(props) => <AddStation  {...props} />} />
                </LayerRouter>
            </Layer>
        );
    }
}

function tableMapStateToProps(state) {
    return {
        stationTypeList: state.basicData.stationTypeList,
        showLoading: state.search.showLoading,
        stationList: state.search.stationList,
        setContractOrgTree: state.basicData.searchOrgTree,
        rootBasicData: (state.rootBasicData || {}).dicList,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Station);