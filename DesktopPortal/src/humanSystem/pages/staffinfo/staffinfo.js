import {connect} from 'react-redux';
import {postBlackLst, setHumanInfo, searchConditionType, setSearchLoadingVisible, getHumanImage, exportHumanForm, setbreadPageIndex, searchHumanType, searchAgeType, searchOrderType, adduserPage} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Table, Input, Select, Icon, Button, Row, Col, Slider, TreeSelect, Pagination, Spin, notification} from 'antd'
import '../search.less'
// import SearchCondition from '../../constants/searchCondition'
import {SearchHumanTypes, AgeRanges} from '../../constants/tools'
import Layer, {LayerRouter} from '../../../components/Layer'
import {Route} from 'react-router'
import OnBoarding from './onboarding'
import BecomeStaff from './becomeStaff'
import Change from './change'
import Leave from './leave'
import PartTimeJob from './partTimeJob'
import {getDicPars} from '../../../utils/utils'

const ButtonGroup = Button.Group;
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
            checkedList: []//选中列表
        }
    }
    getColumns() {
        let dicEmpStatus = this.state.dicEmpStatus || [];
        let listColums = [
            {title: '工号', dataIndex: 'userID', key: 'userID'},
            {title: '用户名', dataIndex: 'name', key: 'name'},
            {title: '性别', dataIndex: 'sexname', key: 'sexname'},
            {title: '身份证号', dataIndex: 'idCard', key: 'idCard'},
            {title: '职位', dataIndex: 'positionName', key: 'positionName'},
            {
                title: '状态', dataIndex: 'staffStatus', key: 'staffStatus', render: (text, record) => {
                    let statusObj = dicEmpStatus.find(dic => dic.value == text);
                    return (<div>{statusObj ? statusObj.key : text}</div>)
                }
            },
            {title: '入职时间', dataIndex: 'entryTime', key: 'entryTime'},
            {title: '转正时间', dataIndex: 'becomeTime', key: 'becomeTime'},
            {title: '基本薪水', dataIndex: 'baseSalary', key: 'baseSalary'},
            {title: '是否参加社保', dataIndex: 'socialInsurance', key: 'socialInsurance'},
            {title: '是否签订合同', dataIndex: 'contract', key: 'contract'},
            {
                title: "操作", dataIndex: "operation", key: "operation",
                render: (text, record) => {
                    return (
                        <span> <a onClick={() => this.show(record)}>显示详细</a> </span>
                    );
                }
            },
        ]
        return listColums;
    }

    show = (record) => {
        //this.props.dispatch(adduserPage({menuID: 'chargedetailinfo', disname: '费用信息', type:'item', extra: e.id}));
        this.props.dispatch(setHumanInfo([record]));
        this.props.dispatch(getHumanImage(record.id));
        this.props.dispatch(adduserPage({id: "4", menuID: "OnboardingShow", displayName: '详情', type: 'item'}));
        this.gotoSubPage('onBoarding', record);
    }

    handleKeyChangeWord = (e) => {
        this.props.searchInfo.keyWord = e.target.value;
    }

    handleSearch(condite) {
        this.props.dispatch(searchConditionType(this.state.condition));
    }

    componentDidMount() {
        this.props.dispatch(setSearchLoadingVisible(true));
        this.props.dispatch(setHumanInfo([]));
        this.props.dispatch(searchConditionType(this.state.condition));
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

    handleTableChange = (pagination, filters, sorter) => {
        this.props.searchInfo.pageIndex = (pagination.current - 1);
    };
    handleTablePageChange = (pagination, filters, sorter) => {
        console.log("翻页:", pagination);
        let condition = this.state.condition;
        // condition.pageIndex=
        // this.setState({condition: condition}, () => {
        //     this.props.dispatch(searchConditionType(this.state.condition));
        // });
    }

    gotoSubPage = (path, params) => {
        this.props.history.push(`${this.props.match.url}/${path}`, params)
    }

    handleOnboarding = (e) => {
        // this.props.dispatch(adduserPage({id: "0", menuID: "Onboarding", displayName: '入职', type: 'item'}));
        this.gotoSubPage('onBoarding', {});
    }

    handleBecome = () => {
        if (this.state.checkedList.length > 0) {
            //this.props.dispatch(adduserPage({id: "1", menuID: "BecomeStaff", displayName: '转正', type: 'item'}));
            let humenInfo = this.state.checkedList[0];
            if (humenInfo.staffStatus != 3) {
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
            // this.props.dispatch(adduserPage({id: "2", menuID: "changestation", displayName: '异动调薪', type: 'item'}));
            this.gotoSubPage('change', this.state.checkedList[0])
        } else {
            notification.error({
                message: "请选择员工",
                duration: 3
            });
        }
    }

    handleLeft = () => {
        if (this.props.selHumanList.length > 0) {
            // this.props.dispatch(adduserPage({id: "3", menuID: "leftstation", displayName: '离职', type: 'item'}));
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
            // this.props.dispatch(adduserPage({id: "3", menuID: "partTimeJob", displayName: '离职', type: 'item'}));
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
            this.props.dispatch(postBlackLst({
                id: this.state.checkedList[len - 1].id,
                idCard: this.state.checkedList[len - 1].idCard, name: this.state.checkedList[len - 1].name
            }));
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
        this.props.dispatch(exportHumanForm({
            data: this.props.searchInfo,
            tree: this.props.setDepartmentOrgTree
        }));
    }


    // changeCallback = (entity) => {
    //     let l = this.state.list;
    //     let idx = l.findIndex(x => x.id === entity.id);
    //     if (idx >= 0) {
    //         l[idx] = {
    //             ...entity, ...{
    //                 status: entity.status,
    //                 billStatus: entity.billStatus,
    //                 isBackup: entity.isBackup,
    //                 backuped: entity.backuped,
    //                 isPayment: entity.isPayment,
    //                 paymentAmount: entity.paymentAmount
    //             }
    //         }
    //         this.setState({list: [...l]})
    //     }
    // }

    render() {
        // let self = this;
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
        const humanList = this.props.searchInfo.searchResult.extension;
        let paginationProps = {total: searchInfo.searchResult.totalCount, pageSize: searchInfo.searchResult.pageSize, current: searchInfo.searchResult.pageIndex}
        return (
            <Layer className="content-page">
                <div style={{marginTop: '1.5rem'}}>
                    <Row className='searchBox'>
                        <Col span={12}>
                            <Input addonBefore="新耀行" prefix={<Icon type="search" />}
                                onPressEnter={(e) => this.handleSearch()}
                                style={{paddingRight: '10px'}}
                                placeholder='请输入姓名'
                                onChange={this.handleKeyChangeWord} />
                        </Col>
                        <Col span={8}>
                            <Button type="primary" size="large" onClick={(e) => this.handleSearch()}>搜索</Button>
                        </Col>
                    </Row>
                    <div className='searchCondition'>
                        <Row>
                            <Col span={12}>
                                {/* <span style={styles.bSpan}>所有人员 > </span> */}
                            </Col>
                            <Col span={4}>
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
                                    <label style={styles.conditionRow}>排序 ：</label>
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
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleUploadContract()}>合同上传</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleAddBlack()}>加入黑名单</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleExport()}>导出花名册</Button>
                        </Col>
                    </Row>
                    <p style={{fontSize: '1.4rem', fontWeight: 'bold'}}>目前已为你筛选出<b style={{color: '#f36366'}}> {humanList.length || 0} </b>条员工信息</p>
                    <Spin spinning={showLoading} delay={200} tip="查询中...">
                        <Table rowSelection={rowSelection} rowKey={record => record.key} pagination={paginationProps} columns={this.getColumns()} dataSource={this.props.searchInfo.searchResult.extension} onChange={this.handleTablePageChange} />
                    </Spin>
                </div>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/onBoarding`} render={(props) => <OnBoarding  {...props} />} />
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