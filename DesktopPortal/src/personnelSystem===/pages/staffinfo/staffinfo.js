import { connect } from 'react-redux';
import { getDicParList, expandSearchbox } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Layout, Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import '../search.less'

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

const appTableColumns = [
    { title: 'ID', dataIndex: 'id', key: 'id' },
    { title: '用户名', dataIndex: 'userName', key: 'userName' },
    { title: '身份证号', dataIndex: 'idcard', key: 'idcard' }
]

const rowSelection = {
    onChange: (selectedRowKeys, selectedRows) => {
      console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
    },
    getCheckboxProps: record => ({
      disabled: record.name === 'Disabled User', // Column configuration not to be checked
      name: record.name,
    }),
  };

class Staffinfo extends Comment {
    state = {
        humanList:[],
        condition: {name: ''},
        pagination: {}
    }

    componentWillMount() {
    }

    render() {
        const searchInfo = this.props.searchInfo || {};
        const showResult = searchInfo.showResult;
        const paginationInfo = searchInfo.pagination;
        const showLoading = searchInfo.showLoading;
        const expandSearchBox = searchInfo.expandSearchBox;
        const searchResult = searchInfo.searchResult;
        return (
            <div>
                <div style={{display: showResult.navigator.length === 0 ? "block" : "none"}}>
                    <Row className='searchBox'>
                        <Col span={24}>
                            <img src='../../../favicon.ico' alt="新耀行" />
                            <Input addonBefore="新耀行" prefix={<Icon type="search" />} onChange={this.handleSearchKeyChange} onPressEnter={(e) => this.handleSearch()} style={{width: '400px', marginLeft: '5px'}} placeholder='请输入姓名' />
                            <Button type="primary" onClick={(e) => this.handleSearch()}>搜索</Button>
                        </Col>
                    </Row>
                    <div className='searchConditon'>
                        <Row>
                            <Col span={12}>
                                <span style={styles.bSpan}>所有员工 > </span>
                                <span> {this.state.filterTags.map((tag, i) => <Tag closable onClose={e => this.handleTagClose(tag, i)} key={tag.value}>{tag.label}</Tag>)}</span>
                            </Col>
                            <Col span={4}>
                                <Button onClick={this.handleSearchBoxToggle}>{expandSearchBox ? "收起筛选" : "展开筛选"}<Icon type={expandSearchBox ? "up-square-o" : "down-square-o"} /></Button>
                            </Col>
                        </Row>
                        <div style={{display: expandSearchBox ? "block" : "none"}}>
                            {
                                this.state.condition.searchType === "xyh" ? <Row>
                                    <Col span={24}>
                                        <label style={styles.conditionRow}>销售状态 ：</label> <Button type="primary" size='small' className={this.state.condition.saleStatus === '0' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('0')}>不限</Button>
                                        <Button type="primary" size='small' className={this.state.condition.saleStatus === '2' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('2', '在售')}>在售</Button>
                                        <Button type="primary" size='small' className={this.state.condition.saleStatus === '1' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('1', '待售')}>待售</Button>
                                        <Button type="primary" size='small' className={this.state.condition.saleStatus === '10' ? 'saleStatusBtn' : 'saleStatusBtn statusBtnDefault'} onClick={(e) => this.handleSaleStatusChange('10', '已售')}>已售</Button>
                                    </Col>
                                </Row> : null
                            }
                            <Row style={{display: this.state.condition.searchType === "xyh" ? "block" : "none"}}>
                                <Col span={24}>
                                    <label style={styles.conditionRow}>年龄 ：</label> <Button className="priceRangeBtn" onClick={(e) => this.handlePriceRange('xyh', '0')}>不限</Button>
                                    {
                                        this.props.basicData.searchPriceXYH.map(price =>
                                            <Button className={price.value === this.state.condition.priceRange ? "priceRangeBtn priceBtnActive" : "priceRangeBtn"} key={price.value} onClick={(e) => this.handlePriceRange('xyh', price)}>{price.key}</Button>
                                        )
                                    }
                                </Col>
                            </Row>
                            <Row>
                                <Col span={24}>
                                    <label style={styles.conditionRow}>是否入职 ：</label>
                                    <CheckboxGroup options={tagsOptionData} defaultValue={[]} value={this.state.checkedTag} onChange={this.handleTagChange} style={{display: 'inline'}} />
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
                    <p style={{padding: '15px 10px', borderBottom: '1px solid #e0e0e0', fontSize: '1.4rem', fontWeight: 'bold'}}>目前已为你筛选出<b style={{color: '#f36366'}}> {paginationInfo.totalCount || 0} </b>条员工信息</p>
                    <Spin spinning={showLoading} delay={200} tip="查询中...">
                        {
                            showResult.showBuildingList ? <div className='searchResult'>
                                {/**搜索结果**/}
                                <Row>
                                    <Col span={24}>
                                        {
                                            (searchResult.length === 1 && searchResult[0].id === "00000000") ? <div style={{marginTop: '10px'}}>{searchResult[0].name}</div> :
                                            (<Layout>
                                                <Header style={{ backgroundColor: '#ececec' }}>
                                                    人事列表
                                                        &nbsp;{/* <Button type='primary' shape='circle' icon='plus' onClick={this.handleAddClick} />
                                                    <Popconfirm title="确认要删除选中用户?" onConfirm={this.handleEmpDelete} onCancel={this.cancelDelete} okText="确定" cancelText="取消">
                                                        &nbsp;<Button type='primary' shape='circle' icon='delete' disabled={removeBtnDisabled} />
                                                    </Popconfirm> */}
                                                </Header>
                                                <Content>
                                                    <Table rowSelection={rowSelection} rowKey={record => record.userName} pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.empList.extension} onChange={this.handleTableChange} />
                                                </Content>
                                            </Layout>)
                                        }
                                    </Col>
                                </Row>
                                {
                                    (searchResult.length > 0 && searchResult[0].id !== "00000000") ? <Pagination showQuickJumper current={this.state.condition.pageIndex + 1} total={paginationInfo.totalCount} onChange={this.handlePageChange}
                                        style={{display: 'flex', justifyContent: 'flex-end'}} /> : null
                                }
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
        operInfo: state.emp.operInfo,
        activeTreeNode: state.org.activeTreeNode,
        empList: state.emp.empList,
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(stafftableMapStateToProps, stafftableMapDispatchToProps)(Staffinfo);