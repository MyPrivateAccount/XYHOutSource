import { connect } from 'react-redux';
import { getDicParList, expandSearchbox, getHumanList, searchKeyWord } from '../../actions/actionCreator';
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

    render() {
        const searchInfo = this.props.searchInfo || {};
        const showLoading = searchInfo.showLoading;
        const expandSearchBox = searchInfo.expandSearchBox;
        return (
            <div>
                <div style={{display: "block"}}>
                    <Row className='searchBox'>
                        <Col span={24}>
                            <img src='../../../favicon.ico' alt="新耀行" />
                            <Input addonBefore="新耀行" prefix={<Icon type="search" />} onPressEnter={(e) => this.handleSearch()} style={{width: '400px', marginLeft: '5px'}} placeholder='请输入姓名' />
                            <Button type="primary" onClick={(e) => this.handleSearch()}>搜索</Button>
                        </Col>
                    </Row>
                    <div className='searchConditon'>
                        <Row>
                            <Col span={12}>
                                <span>{SearchHumanTypes.map((t, i) => <Tag closable onClose={e => this.handleTagClose(t, i)} key={t.value}>{t.label}</Tag>)}</span>
                            </Col>
                            <Col span={4}>
                                <Button onClick={this.handleSearchBoxToggle}>{expandSearchBox ? "收起筛选" : "展开筛选"}<Icon type={expandSearchBox ? "up-square-o" : "down-square-o"} /></Button>
                            </Col>
                        </Row>
                        <div style={{display: expandSearchBox ? "block" : "none"}}>
                            {/* {为了添加条件而准备的
                                <Row>
                                    <Col span={24}>
                                        <label style={styles.conditionRow}>销售状态 ：</label> <Button type="primary" size='small' className={this.state.condition.saleStatus === '0' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('0')}>不限</Button>
                                        <Button type="primary" size='small' className={this.state.condition.saleStatus === '2' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('2', '在售')}>在售</Button>
                                        <Button type="primary" size='small' className={this.state.condition.saleStatus === '1' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('1', '待售')}>待售</Button>
                                        <Button type="primary" size='small' className={this.state.condition.saleStatus === '10' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('10', '已售')}>已售</Button>
                                    </Col>
                                </Row>
                            } */}
                            <Row style={{display: "block"}}>
                                <Col span={24}>
                                    <label style={styles.conditionRow}>年龄 ：</label> <Button className="priceRangeBtn" onClick={(e) => this.handlePriceRange('xyh', '0')}>不限</Button>
                                    {
                                        AgeRanges.map(age =>
                                            <Button className={age.value === this.props.searchInfo.agesCondition ? "priceRangeBtn priceBtnActive" : "priceRangeBtn"} key={age.value} onClick={(e) => this.handlePriceRange('xyh', age)}>{age.key}</Button>
                                        )
                                    }
                                </Col>
                            </Row>
                        </div>
                    </div>
                    <Row className='searchBox'>
                        <Col span={24}>
                            <Button type="primary" onClick={(e) => this.handleSearch()}>入职</Button>
                            <Button type="primary" onClick={(e) => this.handleSearch()}>转正</Button>
                            <Button type="primary" onClick={(e) => this.handleSearch()}>异动调薪</Button>
                            <Button type="primary" onClick={(e) => this.handleSearch()}>离职</Button>
                            <Button type="primary" onClick={(e) => this.handleSearch()}>合同上传</Button>
                            <Button type="primary" onClick={(e) => this.handleSearch()}>加入黑名单</Button>
                            <Button type="primary" onClick={(e) => this.handleSearch()}>历史信息</Button>
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
                                                <Table rowSelection={rowSelection} rowKey={record => record.userName} pagination={this.state.pagination} columns={ListColums} dataSource={this.props.humanList} onChange={this.handleTableChange} />
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