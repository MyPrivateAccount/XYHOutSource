import {connect} from 'react-redux';
import {postBlackLst, setHumanInfo, getHumanImage, exportHumanForm, getHumanDetail} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Table, Input, Select, Icon, Button, Row, Col, Slider, TreeSelect, Spin, notification, Popconfirm, Modal} from 'antd'
import '../search.less'
import Layer, {LayerRouter} from '../../../components/Layer'
import {Route} from 'react-router'
import OnBoarding from './onboarding'
import BecomeStaff from './becomeStaff'
import Change from './change'
import Leave from './leave'
import PartTimeJob from './partTimeJob'
import {getDicPars} from '../../../utils/utils'
import {getHumanList} from '../../serviceAPI/staffService'
import {addBlackLst} from '../../serviceAPI/blackService'
import moment from 'moment'
import {NewGuid} from '../../../utils/appUtils';
import {createMergeHead, createColumData, writeHumanFile, HumanHead} from '../../constants/export';

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
const confirm = Modal.confirm;
class Staffinfo extends Component {

    constructor(pro) {
        super(pro);
        this.state = {
            expandSearchBox: false,
            condition: {
                keyWord: '',
                staffStatuses: '0',
                departmentId: '',
                birthdayStart: null,
                birthdayEnd: null,
                pageIndex: 0,
                pageSize: 10
            },
            showLoading: false,
            humanList: {extension: [], pageIndex: 0, pageSize: 10, totalCount: 0},
            checkedList: []//选中列表
        }
    }
    getColumns() {
        let dicEmpStatus = this.state.dicEmpStatus || [];
        let listColums = [
            {title: '工号', dataIndex: 'userID', key: 'userID'},
            {title: '用户名', dataIndex: 'name', key: 'name'},
            {title: '性别', dataIndex: 'sex', key: 'sex', render: (text, record) => text == '1' ? '男' : '女'},
            {title: '身份证号', dataIndex: 'idCard', key: 'idCard'},
            {title: '职位', dataIndex: 'position', key: 'position'},
            {
                title: '状态', dataIndex: 'staffStatus', key: 'staffStatus', render: (text, record) => {
                    let statusObj = dicEmpStatus.find(dic => dic.value == text);
                    return (<div>{statusObj ? statusObj.key : text}</div>)
                }
            },
            {title: '入职时间', dataIndex: 'entryTime', key: 'entryTime', render: (text, record) => text ? moment(text).format("YYYY-MM-DD") : ''},
            {title: '转正时间', dataIndex: 'becomeTime', key: 'becomeTime', render: (text, record) => text ? moment(text).format("YYYY-MM-DD") : ''},
            {title: '基本薪水', dataIndex: 'baseWages', key: 'baseWages'},
            {title: '是否参加社保', dataIndex: 'isHaveSocialSecurity', key: 'isHaveSocialSecurity', render: (text, record) => text == true ? '是' : '否'},
            {title: '是否签订合同', dataIndex: 'isSignContracInfo', key: 'isSignContracInfo', render: (text, record) => text == true ? '是' : '否'},
            {
                title: "操作", dataIndex: "operation", key: "operation",
                render: (text, record) => {
                    return (
                        <Button type="primary" size='small' shape="circle" icon="idcard" style={{marginRight: '5px'}} onClick={() => this.show(record)} />
                        // <span> <a onClick={() => this.show(record)}>显示详细</a> </span>
                    );
                }
            },
        ]
        return listColums;
    }

    show = (record) => {
        this.props.dispatch(getHumanDetail(record.id))
        this.gotoSubPage('onBoardingDetail', record);
    }

    handleKeyChangeWord = (e) => {
        let condition = this.state.condition;
        condition.keyWord = e.target.value;
        this.setState({condition: condition});
    }

    handleSearch() {
        this.setState({showLoading: true});
        console.log("当前检索条件:", this.state.condition);
        let condition = {...this.state.condition}
        getHumanList(condition).then(res => {
            console.log("这是请求结果:", res);
            this.setState({showLoading: false});
            if (res.isOk) {
                this.setState({
                    humanList: {
                        extension: res.extension,
                        pageIndex: res.pageIndex,
                        pageSize: res.pageSize,
                        totalCount: res.totalCount
                    }
                });
            }
        });
    }

    componentDidMount() {
        this.handleSearch();
        let dicEmpStatus = getDicPars("HUMEN_EMP_STATUS", this.props.rootBasicData);
        this.setState({dicEmpStatus: dicEmpStatus});
    }
    componentWillReceiveProps(newProps) {
        if (!this.state.dicEmpStatus || this.state.dicEmpStatus.length == 0) {
            let dicEmpStatus = getDicPars("HUMEN_EMP_STATUS", this.props.rootBasicData);
            this.setState({dicEmpStatus: dicEmpStatus});
        }
    }

    handleSearchConditionChange = (value, fieldName) => {
        let condition = this.state.condition;
        if (fieldName == 'keyWord') {
            condition.keyWord = fieldName;
        } else if (fieldName == 'staffStatuses') {
            condition.staffStatuses = value;
        }
        else if (fieldName == 'departmentId') {
            condition.departmentId = value;
        }
        else if (fieldName == 'birthdayStart') {
            condition.birthdayStart = value[0];
            condition.birthdayEnd = value[1];
        }
        this.setState({condition: condition});
    }

    //翻页
    handleTablePageChange = (pagination, filters, sorter) => {
        let condition = this.state.condition;
        console.log("翻页前条件:", condition);
        condition.pageIndex = pagination.current - 1;
        this.setState({condition: condition}, () => {
            console.log("翻页后条件:", this.state.condition);
            this.handleSearch();
        });
    }

    gotoSubPage = (path, params) => {
        this.props.history.push(`${this.props.match.url}/${path}`, params)
    }

    handleOnboarding = (e) => {
        this.gotoSubPage('onBoarding', {});
    }

    handleBecome = () => {
        if (this.state.checkedList.length > 0) {
            //this.props.dispatch(adduserPage({id: "1", menuID: "BecomeStaff", displayName: '转正', type: 'item'}));
            let humanInfo = this.state.checkedList[0];
            if (humanInfo.staffStatus != 3) {
                this.gotoSubPage('becomeStaff', this.state.checkedList[0])
            } else {
                notification.error({
                    message: "已经是正式员工",
                    duration: 3
                });
            }
        } else {
            notification.error({
                message: "请选择员工",
                duration: 3
            });
        }
    }

    handleChangeSalary = () => {
        if (this.state.checkedList.length > 0) {
            this.props.dispatch(getHumanDetail(this.state.checkedList[0].id))
            this.gotoSubPage('change', this.state.checkedList[0])
        } else {
            notification.error({
                message: "请选择员工",
                duration: 3
            });
        }
    }

    handleLeft = () => {
        if (this.state.checkedList.length > 0) {
            this.gotoSubPage('leave', this.state.checkedList[0])
        } else {
            notification.error({
                message: "请选择员工",
                duration: 3
            });
        }

    }

    handlePartTimeJob = () => {
        if (this.state.checkedList.length > 0) {
            // this.props.dispatch(getHumanDetail(this.state.checkedList[0].id))
            this.gotoSubPage('partTimeJob', this.state.checkedList[0])
        } else {
            notification.error({
                message: "请选择员工",
                duration: 3
            });
        }
    }

    handleAddBlack = () => {
        let len = this.state.checkedList.length;
        if (len > 0) {
            let humanInfo = this.state.checkedList[0];
            confirm({
                title: '确认',
                content: `确认要将'${humanInfo.name}'加入黑名单?`,
                onOk: () => {
                    let entity = {
                        id: NewGuid(),
                        userId: humanInfo.id,
                        idCard: humanInfo.idCard,
                        name: humanInfo.name,
                        phone: humanInfo.phone,
                        sex: humanInfo.sex,
                        email: humanInfo.email
                    }
                    this.setState({showLoading: true})
                    addBlackLst(entity).then(res => {
                        this.setState({showLoading: false});
                    })
                }
            });
        } else {
            notification.error({
                message: "请选择员工",
                duration: 3
            });
        }
    }

    handleExport = () => {
        this.props.searchInfo.pageIndex = -1;
        this.props.searchInfo.pageSize = -1;
        let condition = {...this.state.condition}
        getHumanList(condition).then(res => {
            console.log("这是请求结果:", res);
            this.setState({showLoading: false});
            if (res.isOk) {
                // this.setState({
                //     humanList: {
                //         extension: res.extension,
                //         pageIndex: res.pageIndex,
                //         pageSize: res.pageSize,
                //         totalCount: res.totalCount
                //     }
                // });
                const PositionStatus = ["未入职", "离职", "入职", "转正"];
                let exportData = res.extension.map((v, k) => {
                    let sn = "", fn = "";
                    (v.sex == 1) && (sn = "男");
                    (v.sex == 2) && (sn = "女");
                    fn = v.staffStatus ? PositionStatus[v.staffStatus] : "未入职";
                    return {
                        a1: k, a2: v.id, a3: v.name, a4: sn, a5: v.idCard, a6: '',
                        a7: v.position, a8: fn, a9: v.entryTime ? v.entryTime.replace("T", " ") : "",
                        a10: v.becomeTime ? v.becomeTime.replace("T", " ") : "",
                        a11: v.baseWages, a12: v.isHaveSocialSecurity ? "是" : "否", a13: v.isSignContracInfo ? "是" : "否"
                    }
                });
                console.log("导出数据:", exportData);
                let f = createMergeHead(HumanHead);
                let ret = createColumData(f, exportData);
                writeHumanFile(f, ret, "人事员工表", "人事员工.xlsx");
            }
        });
        // this.props.dispatch(exportHumanForm({
        //     data: this.props.searchInfo,
        //     tree: this.props.setDepartmentOrgTree
        // }));
    }
    render() {
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                //self.props.dispatch(setHumanInfo(selectedRows));
                // console.log("选中行:", selectedRowKeys, selectedRows);
                this.setState({checkedList: selectedRows});
            },
            getCheckboxProps: record => ({
                disabled: record.name === 'Disabled User',
                name: record.name,
            }),
        };

        const searchInfo = this.props.searchInfo || {};
        const showLoading = searchInfo.showLoading;
        const humanList = this.state.humanList;
        let paginationProps = {total: humanList.totalCount, pageSize: humanList.pageSize, current: humanList.pageIndex + 1}
        return (
            <Layer className="content-page" showLoading={this.state.showLoading}>
                <div style={{marginTop: '0.5rem'}}>
                    <Row className='searchBox'>
                        <Col span={12}>
                            <Input addonBefore="新耀行" prefix={<Icon type="search" />}
                                onPressEnter={(e) => this.handleSearch()}
                                style={{paddingRight: '10px'}}
                                placeholder='请输入姓名'
                                onChange={this.handleKeyChangeWord} />
                        </Col>
                        <Col span={8}>
                            <Button type="primary" className='searchButton' onClick={(e) => this.handleSearch()}>搜索</Button>
                        </Col>
                    </Row>
                    <div className='searchCondition'>
                        <Row>
                            <Col style={{textAlign: 'center'}}>
                                <Button onClick={() => this.setState({expandSearchBox: !this.state.expandSearchBox})}>{this.props.searchInfo.expandSearchBox ? "收起筛选" : "展开筛选"}<Icon type={this.props.searchInfo.expandSearchBox ? "up-square-o" : "down-square-o"} /></Button>
                            </Col>
                        </Row>
                        <div style={{display: this.state.expandSearchBox ? "block" : "none"}}>
                            <Row className="normalInfo">
                                <Col span={24}>
                                    <label style={styles.conditionRow}>状态 ：</label>
                                    {(this.state.dicEmpStatus || []).map(
                                        (t, i) => <Button type="primary" size='small' key={t.key} className={this.state.condition.staffStatuses === t.value ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSearchConditionChange(t.value, "staffStatuses")}>{t.key}</Button>
                                    )}
                                </Col>
                            </Row>
                            <Row className="normalInfo">
                                <Col span={24}>
                                    <label style={styles.conditionRow}>年龄 ：</label>
                                    {/* {
                                        AgeRanges.map(age =>
                                            <Button className={age.value === this.props.searchInfo.ageCondition ? "staffRangeBtn staffBtnActive" : "staffRangeBtn"} key={age.value} onClick={(e) => this.handlePriceRange(age.value)}>{age.label}</Button>
                                        )
                                    } */}
                                    {/* </Col>
                                <Col span={20}> */}
                                    <label>不限</label>
                                    <Slider range style={{width: '300px', display: 'inline-block', marginBottom: '0px'}} min={0} max={130} defaultValue={[0, 0]} onChange={(value) => this.handleSearchConditionChange(value, 'birthdayStart')} />
                                    <label>130岁</label>
                                </Col>
                            </Row>
                            <Row className="normalInfo">
                                <Col>
                                    <label style={styles.conditionRow}>部门 ：</label>
                                    <TreeSelect allowClear style={{width: '200px'}} treeData={this.props.setDepartmentOrgTree || []} onChange={(e) => this.handleSearchConditionChange(e, 'departmentId')} />
                                </Col>
                            </Row>
                        </div>
                    </div>
                    <Row className="groupButton">
                        <Col span={24}>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleOnboarding(0)}>入职</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleBecome()}>转正</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleChangeSalary()}>异动调薪</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleLeft()}>离职</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handlePartTimeJob()}>兼职</Button>
                            {/* <Button type="primary" className="statuButton" onClick={(e) => this.handleUploadContract()}>合同上传</Button> */}
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleAddBlack()}>加入黑名单</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleExport()}>导出花名册</Button>
                        </Col>
                    </Row>
                    <p style={{fontSize: '1.4rem', fontWeight: 'bold'}}>目前已为你筛选出<b style={{color: '#f36366'}}> {humanList.totalCount || 0} </b>条员工信息</p>
                    <Spin spinning={showLoading} delay={200} tip="查询中...">
                        <Table rowSelection={rowSelection} rowKey={record => record.key} pagination={paginationProps} columns={this.getColumns()} dataSource={humanList.extension} onChange={this.handleTablePageChange} />
                    </Spin>
                </div>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/onBoarding`} render={(props) => <OnBoarding  {...props} />} />
                    <Route path={`${this.props.match.url}/onBoardingDetail`} render={(props) => <OnBoarding  {...props} isReadOnly />} />
                    <Route path={`${this.props.match.url}/becomeStaff`} render={(props) => <BecomeStaff  {...props} />} />
                    <Route path={`${this.props.match.url}/change`} render={(props) => <Change  {...props} />} />
                    <Route path={`${this.props.match.url}/leave`} render={(props) => <Leave  {...props} />} />
                    <Route path={`${this.props.match.url}/partTimeJob`} render={(props) => <PartTimeJob  {...props} />} />
                </LayerRouter>
            </Layer>
        );
    }
}

function stafftableMapStateToProps(state) {
    return {
        searchInfo: state.search,
        setDepartmentOrgTree: state.basicData.searchOrgTree,
        selHumanList: state.basicData.selHumanList,
        rootBasicData: (state.rootBasicData || {}).dicList,
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(stafftableMapStateToProps, stafftableMapDispatchToProps)(Staffinfo);