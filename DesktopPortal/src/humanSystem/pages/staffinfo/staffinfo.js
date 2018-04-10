import { connect } from 'react-redux';
import { getDicParList, expandSearchbox, getHumanList, searchKeyWord,setbreadPage } from '../../actions/actionCreator';
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

    handleSearch(condite) {
        let condition = {...this.state.condition, pageIndex:0, keyWord:condite};
        this.props.dispatch(searchKeyWord(condition));
    }

    handleTableChange = (pagination, filters, sorter) => {
        // SearchCondition.empSearchCondition.pageIndex = (pagination.current - 1);
        // SearchCondition.empSearchCondition.pageSize = pagination.pageSize;
        // console.log("table改变，", pagination);
        // this.props.dispatch(empListGet(SearchCondition.empSearchCondition));
    };

    componentWillMount() {
        //this.props.dispatch(getHumanList(SearchCondition.topteninfo));
    }

    handleSaleStatusChange = (value, text) => {
        // let condition = {...this.state.condition, saleStatus: value};
        // let tagArray = this.state.filterTags;
        // for (let i = tagArray.length - 1; i > -1; i--) {
        //     if (tagArray[i].type === "saleStatus" || tagArray[i].type === "leaseStatus") {
        //         tagArray.splice(i, 1);
        //     }
        // }
        // if (value !== "0") {
        //     tagArray.push({value: this.state.condition.saleStatus, label: text, type: 'saleStatus'});
        // }
        // this.setState({condition: condition, filterTags: tagArray, pageIndex: 0});
        // this.handleSearch(condition);
    }

    handleOrderChange = (e) => {
        // console.log(e.target.value);
        // let condition = {...this.state.condition, priceIsAscSort: e.target.value};
        // this.setState({condition: condition});
        // this.handleSearch(condition);
    }

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

    handleOnboarding(e) {
        this.dispatch(setbreadPage(e));
    }

    render() {
        const searchInfo = this.props.searchInfo || {};
        const showLoading = searchInfo.showLoading;
        return (
            <div>
                <div style={{display: "block"}}>
                    <Row className='searchBox'>
                        <Col span={12}>
                            <Input addonBefore="新耀行" prefix={<Icon type="search" />} onPressEnter={(e) => this.handleSearch()} style={{ paddingRight: '10px', marginLeft: '5px'}} placeholder='请输入姓名' />
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
                                        (t, i) => <Button type="primary" size='small' key={t.value} className={this.props.searchInfo.searchHumanType === t.value ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange(t.value)}>{t.label}</Button>
                                    )}
                                </Col>
                            </Row>
                            <Row className="normalInfo">
                                <Col span={24}>
                                    <label style={styles.conditionRow}>年龄 ：</label>
                                    {
                                        AgeRanges.map(age =>
                                            <Button className={age.value === this.props.searchInfo.agesCondition ? "staffRangeBtn staffBtnActive" : "staffRangeBtn"} key={age.value} onClick={(e) => this.handlePriceRange('xyh', age)}>{age.label}</Button>
                                        )
                                    }
                                </Col>
                            </Row>
                            <Row className="normalInfo">
                                <Col span={24}>
                                    <label style={styles.conditionRow}>排序 ：</label>
                                    <ButtonGroup onClick={this.handleOrderChange}>
                                        <Button type={this.props.searchInfo.searchSortType === 0 ? "primary" : ""} value="0">不排序</Button>
                                        <Button type={this.props.searchInfo.searchSortType === 1 ? "primary" : ""} icon="arrow-up" value="1">升序</Button>
                                        <Button type={this.props.searchInfo.searchSortType === 2 ? "primary" : ""} icon="arrow-down" value="2">降序</Button>
                                    </ButtonGroup>
                                </Col>
                            </Row>
                        </div>
                    </div>
                    <Row className="groupButton">
                        <Col span={24}>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleOnboarding()}>入职</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleSearch()}>转正</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleSearch()}>异动调薪</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleSearch()}>离职</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleSearch()}>合同上传</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleSearch()}>加入黑名单</Button>
                            <Button type="primary" className="statuButton" onClick={(e) => this.handleSearch()}>历史信息</Button>
                        </Col>
                    </Row>
                    <p style={{padding: '15px 10px', borderBottom: '1px solid #e0e0e0', fontSize: '1.4rem', fontWeight: 'bold'}}>目前已为你筛选出<b style={{color: '#f36366'}}> {this.props.humanList.length || 0} </b>条员工信息</p>
                    <Spin spinning={showLoading} delay={200} tip="查询中...">
                        {
                            this.props.humanList.length>0 ? <div className='searchResult'>
                                {/**搜索结果**/}
                                <Row>
                                    <Col span={24}>
                                        <Layout>
                                            <Header style={{ backgroundColor: '#ececec' }}>
                                                人事列表
                                                    &nbsp;
                                            </Header>
                                            <Content>
                                                <Table rowSelection={rowSelection} rowKey={record => record.key} pagination={this.props.searchInfo.searchResult} columns={ListColums} dataSource={this.props.humanList} onChange={this.handleTableChange} />
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
        humanList: state.basicData.humanList,
        showLoading: state.basicData.showLoading
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(stafftableMapStateToProps, stafftableMapDispatchToProps)(Staffinfo);