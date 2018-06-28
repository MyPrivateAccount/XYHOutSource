import {connect} from 'react-redux';
import {postBlackLst, setHumanInfo, searchConditionType, setSearchLoadingVisible, getHumanImage, exportHumanForm, setbreadPageIndex, searchHumanType, searchAgeType, searchOrderType, adduserPage} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Table, Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin, notification} from 'antd'
import '../search.less'
import SearchCondition from '../../constants/searchCondition'
import {SearchHumanTypes, AgeRanges} from '../../constants/tools'
import Layer, {LayerRouter} from '../../../components/Layer'
import {Route} from 'react-router'
import OnBoarding from './onboarding'
import BecomeStaff from './becomeStaff'
import Change from './change'
import Leave from './leave'
import PartTimeJob from './partTimeJob'

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
        this.ListColums = [
            {title: 'ID', dataIndex: 'id', key: 'id'},
            {title: '用户名', dataIndex: 'name', key: 'name'},
            {title: '性别', dataIndex: 'sexname', key: 'sexname'},
            {title: '身份证号', dataIndex: 'idCard', key: 'idCard'},
            {title: '职位', dataIndex: 'positionName', key: 'positionName'},
            {title: '状态', dataIndex: 'staffStatus', key: 'staffStatus'},
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
        this.state = {
            expandSearchBox: false
        }
    }

    show = (e) => {
        //this.props.dispatch(adduserPage({menuID: 'chargedetailinfo', disname: '费用信息', type:'item', extra: e.id}));
        this.props.dispatch(setHumanInfo([e]));
        this.props.dispatch(getHumanImage(e.id));
        this.props.dispatch(adduserPage({id: "4", menuID: "OnboardingShow", displayName: '详情', type: 'item'}));
    }

    handleKeyChangeWord = (e) => {
        this.props.searchInfo.keyWord = e.target.value;
    }

    handleSearch(condite) {
        this.props.dispatch(searchConditionType(this.props.searchInfo));
    }

    componentWillMount() {
        this.props.dispatch(setSearchLoadingVisible(true));

        this.props.dispatch(setHumanInfo([]));
        this.props.dispatch(searchConditionType(SearchCondition.topteninfo));

    }

    handleSaleStatusChange = (value, text) => {
        this.props.dispatch(searchHumanType(value));
    }

    handlePriceRange = (value) => {
        this.props.dispatch(searchAgeType(value));
    }

    handleOrderChange = (e) => {
        this.props.dispatch(searchOrderType(e.target.value));
    }

    handleTableChange = (pagination, filters, sorter) => {
        this.props.searchInfo.pageIndex = (pagination.current - 1);
    };

    handleTagClose = (tag, i) => {//过滤标签删除

        let tagArray = this.state.filterTags;
        let condition = this.state.condition;
        let checkedTag = this.state.checkedTag;
        let removeTag = tagArray.splice(i, 1)[0];
        if (removeTag.type === "tag") {
            for (let i = checkedTag.length - 1; i > -1; i--) {
                if (checkedTag[i] === removeTag.value) {
                    checkedTag.splice(i, 1);
                    break;
                }
            }
        } else {
            condition[removeTag.type] = '0';
        }

        this.setState({condition: condition, filterTags: tagArray, checkedTag: checkedTag, pageIndex: 0});
        this.handleSearch(condition);
    }

    gotoSubPage = (path, params) => {
        this.props.history.push(`${this.props.match.url}/${path}`, {entity: params, op: 'add'})
    }

    handleOnboarding = (e) => {
        // this.props.dispatch(adduserPage({id: "0", menuID: "Onboarding", displayName: '入职', type: 'item'}));
        this.gotoSubPage('onBoarding', {});
    }

    handleBecome = () => {
        if (this.props.selHumanList.length > 0) {
            //this.props.dispatch(adduserPage({id: "1", menuID: "BecomeStaff", displayName: '转正', type: 'item'}));
            this.gotoSubPage('becomeStaff', {})
        } else {
            notification.error({
                message: "请选择员工",
                duration: 3
            });
        }
    }

    handleChangeSalary = () => {
        if (this.props.selHumanList.length > 0) {
            // this.props.dispatch(adduserPage({id: "2", menuID: "changestation", displayName: '异动调薪', type: 'item'}));
            this.gotoSubPage('change', {})
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
            this.gotoSubPage('leave', {})
        } else {
            notification.error({
                message: "请选择员工",
                duration: 3
            });
        }

    }

    handlePartTimeJob = () => {
        if (this.props.selHumanList.length > 0) {
            // this.props.dispatch(adduserPage({id: "3", menuID: "partTimeJob", displayName: '离职', type: 'item'}));
            this.gotoSubPage('partTimeJob', {})
        } else {
            notification.error({
                message: "请选择员工",
                duration: 3
            });
        }
    }

    handleAddBlack = () => {
        let len = this.props.selHumanList.length;
        if (len > 0) {
            this.props.dispatch(postBlackLst({
                id: this.props.selHumanList[len - 1].id,
                idCard: this.props.selHumanList[len - 1].idCard, name: this.props.selHumanList[len - 1].name
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


    changeCallback = (entity) => {
        let l = this.state.list;
        let idx = l.findIndex(x => x.id === entity.id);
        if (idx >= 0) {
            l[idx] = {
                ...entity, ...{
                    status: entity.status,
                    billStatus: entity.billStatus,
                    isBackup: entity.isBackup,
                    backuped: entity.backuped,
                    isPayment: entity.isPayment,
                    paymentAmount: entity.paymentAmount
                }
            }
            this.setState({list: [...l]})
        }
    }

    render() {
        let self = this;
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                self.props.dispatch(setHumanInfo(selectedRows));
            },
            getCheckboxProps: record => ({
                disabled: record.name === 'Disabled User',
                name: record.name,
            }),
        };

        const searchInfo = this.props.searchInfo || {};
        const showLoading = searchInfo.showLoading;
        const humanList = this.props.searchInfo.searchResult.extension;
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
                                <span style={styles.bSpan}>所有人员 > </span>
                            </Col>
                            <Col span={4}>
                                <Button onClick={() => this.setState({expandSearchBox: !this.state.expandSearchBox})}>{this.props.searchInfo.expandSearchBox ? "收起筛选" : "展开筛选"}<Icon type={this.props.searchInfo.expandSearchBox ? "up-square-o" : "down-square-o"} /></Button>
                            </Col>
                        </Row>
                        <div style={{display: this.state.expandSearchBox ? "block" : "none"}}>
                            <Row className="normalInfo">
                                <Col span={24}>
                                    <label style={styles.conditionRow}>状态 ：</label>
                                    {SearchHumanTypes.map(
                                        (t, i) => <Button type="primary" size='small' key={t.value} className={this.props.searchInfo.humanType === t.value ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange(t.value)}>{t.label}</Button>
                                    )}
                                </Col>
                            </Row>
                            <Row className="normalInfo">
                                <Col span={24}>
                                    <label style={styles.conditionRow}>年龄 ：</label>
                                    {
                                        AgeRanges.map(age =>
                                            <Button className={age.value === this.props.searchInfo.ageCondition ? "staffRangeBtn staffBtnActive" : "staffRangeBtn"} key={age.value} onClick={(e) => this.handlePriceRange(age.value)}>{age.label}</Button>
                                        )
                                    }
                                </Col>
                            </Row>
                            <Row className="normalInfo">
                                <Col span={24}>
                                    <label style={styles.conditionRow}>排序 ：</label>
                                    <ButtonGroup onClick={this.handleOrderChange}>
                                        <Button type={this.props.searchInfo.orderRule === 0 ? "primary" : ""} value="0">不排序</Button>
                                        <Button type={this.props.searchInfo.orderRule === 1 ? "primary" : ""} icon="arrow-up" value="1">升序</Button>
                                        <Button type={this.props.searchInfo.orderRule === 2 ? "primary" : ""} icon="arrow-down" value="2">降序</Button>
                                    </ButtonGroup>
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
                        <Table rowSelection={rowSelection} rowKey={record => record.key} pagination={this.props.searchInfo.searchResult} columns={this.ListColums} dataSource={this.props.searchInfo.searchResult.extension} onChange={this.handleTableChange} />
                    </Spin>
                </div>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/onBoarding`} render={(props) => <OnBoarding changeCallback={this.changeCallback} {...props} />} />
                    <Route path={`${this.props.match.url}/becomeStaff`} render={(props) => <BecomeStaff changeCallback={this.changeCallback} {...props} />} />
                    <Route path={`${this.props.match.url}/change`} render={(props) => <Change changeCallback={this.changeCallback} {...props} />} />
                    <Route path={`${this.props.match.url}/leave`} render={(props) => <Leave changeCallback={this.changeCallback} {...props} />} />
                    <Route path={`${this.props.match.url}/partTimeJob`} render={(props) => <PartTimeJob changeCallback={this.changeCallback} {...props} />} />
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
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(stafftableMapStateToProps, stafftableMapDispatchToProps)(Staffinfo);