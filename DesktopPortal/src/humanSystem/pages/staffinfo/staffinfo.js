import { connect } from 'react-redux';
import { getDicParList, expandSearchbox, searchConditionType, searchCondition,setbreadPageIndex, searchHumanType,searchAgeType,searchOrderType, adduserPage } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Layout, Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import '../search.less'
import SearchCondition from '../../constants/searchCondition'
import { SearchHumanTypes, ListColums, AgeRanges} from '../../constants/tools'

const { Header, Sider, Content } = Layout;
const CheckboxGroup = Checkbox.Group;
const ButtonGroup = Button.Group;
const tagsOptionData = [{value: 'nolimit', label: '不限'}, {value: 'yes', label: '是'}, {value: 'no', label: '否'}]
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

class Staffinfo extends Component {

    handleKeyChangeWord = (e) => {
        this.props.searchInfo.keyWord = e.target.value;
    }

    handleSearch(condite) {
        this.props.dispatch(searchConditionType(this.props.searchInfo));
    }

    componentWillMount() {
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
        console.log("移除的tag：", tag, this.state.checkedTag);
        let tagArray = this.state.filterTags;
        let condition = this.state.condition;
        let checkedTag = this.state.checkedTag;
        let removeTag = tagArray.splice(i, 1)[0];
        if (removeTag.type === "tag") {
            //delete checkedTag[removeTag.value];
            for (let i = checkedTag.length - 1; i > -1; i--) {
                if (checkedTag[i] === removeTag.value) {
                    checkedTag.splice(i, 1);
                    break;
                }
            }
        } else {
            condition[removeTag.type] = '0';
        }
        console.log("tagArray", tagArray, checkedTag);
        this.setState({condition: condition, filterTags: tagArray, checkedTag: checkedTag, pageIndex: 0});
        this.handleSearch(condition);
    }

    handleOnboarding =(e)=> {
        this.props.dispatch(adduserPage({id: "0", menuID: "Onboarding", displayName: '入职', type: 'item'}));
    }

    handleBecome = ()=> {
        this.props.dispatch(adduserPage({id: "1", menuID: "BecomeStaff", displayName: '转正', type: 'item'}));
    }

    handleChangeSalary = () => {
        this.props.dispatch(adduserPage({id: "1", menuID: "changestation", displayName: '异动调薪', type: 'item'}));
    }

    handleLeft = () => {
        this.props.dispatch(adduserPage({id: "1", menuID: "leftstation", displayName: '离职', type: 'item'}));
    }

    render() {
        const searchInfo = this.props.searchInfo || {};
        const showLoading = searchInfo.showLoading;
        const humanList = this.props.searchInfo.searchResult.extension;
        return (
            <div>
                <div style={{display: "block"}}>
                    <Row className='searchBox'>
                        <Col span={12}>
                            <Input addonBefore="新耀行" prefix={<Icon type="search" />} 
                            onPressEnter={(e) => this.handleSearch()} 
                            style={{ paddingRight: '10px', marginLeft: '5px'}} 
                            placeholder='请输入姓名'
                            onChange = {this.handleKeyChangeWord} />
                        </Col>
                        <Col span={8}>
                            <Button type="primary" onClick={(e) => this.handleSearch()}>搜索</Button>
                        </Col>
                    </Row>
                    <div className='searchCondition'>
                        <Row>
                            <Col span={12}>
                                <span style={styles.bSpan}>所有人员 > </span>
                            </Col>
                            <Col span={4}>
                                <Button onClick={this.handleSearchBoxToggle}>{this.props.searchInfo.expandSearchBox ? "收起筛选" : "展开筛选"}<Icon type={this.props.searchInfo.expandSearchBox ? "up-square-o" : "down-square-o"} /></Button>
                            </Col>
                        </Row>
                        <div style={{display: this.props.searchInfo.expandSearchBox ? "block" : "none"}}>
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
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleUploadContract()}>合同上传</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleAddBlack()}>加入黑名单</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleHistory()}>历史信息</Button>
                        </Col>
                    </Row>
                    <p style={{padding: '15px 10px', borderBottom: '1px solid #e0e0e0', fontSize: '1.4rem', fontWeight: 'bold'}}>目前已为你筛选出<b style={{color: '#f36366'}}> {humanList.length || 0} </b>条员工信息</p>
                    <Spin spinning={showLoading} delay={200} tip="查询中...">
                        {
                            humanList.length>0 ? <div className='searchResult'>
                                {/**搜索结果**/}
                                <Row>
                                    <Col span={24}>
                                        <Layout>
                                            <Header style={{ backgroundColor: '#ececec' }}>
                                                人事列表
                                                    &nbsp;
                                            </Header>
                                            <Content>
                                                <Table rowSelection={rowSelection} rowKey={record => record.key} pagination={this.props.searchInfo.searchResult} columns={ListColums} dataSource={this.props.searchInfo.searchResult.extension} onChange={this.handleTableChange} />
                                            </Content>
                                        </Layout>
                                    </Col>
                                </Row>
                                {/* {
                                    (searchResult.length > 0 && searchResult[0].id !== "00000000") ? <Pagination showQuickJumper current={this.state.condition.pageIndex + 1} total={paginationInfo.totalCount} onChange={this.handlePageChange}
                                        style={{display: 'flex', justifyContent: 'flex-end'}} /> : null
                                } */}
                            </div> : null
                        }
                    </Spin>
                </div>
            </div>
        );
    }
}

function stafftableMapStateToProps(state) {
    return {
        searchInfo: state.search,
        showLoading: state.basicData.showLoading
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(stafftableMapStateToProps, stafftableMapDispatchToProps)(Staffinfo);