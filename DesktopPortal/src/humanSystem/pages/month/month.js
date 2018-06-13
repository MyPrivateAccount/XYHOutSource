import { connect } from 'react-redux';
import { searchConditionType,getAllMonthList, recoverMonth, createMonth,exportMonthForm} from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Layout, Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import SearchCondition from '../../constants/searchCondition'
import { MonthListColums} from '../../constants/tools'
import { Test, } from '../../constants/export';

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
    componentWillMount() {
        this.props.dispatch(getAllMonthList(this.props.monthresult));
    }

    handleTableChange = (pagination, filters, sorter) => {
        this.props.monthresult.pageIndex = (pagination.current - 1);
    };

    createMonth = () => {
        this.props.dispatch(createMonth({last:this.props.monthLast,result:this.props.monthresult}));
    }

    recoverMonth = () => {
        this.props.dispatch(recoverMonth({last:this.props.monthLast,result:this.props.monthresult}));
    }

    createMonthForm = () => {
        this.props.dispatch(exportMonthForm());
    }

    render() {
        let recoverinfo = "恢复到:";
        let nextMonth = new Date(this.props.monthLast);
        const showLoading = this.props.showLoading;
        const monthList = this.props.monthresult.extension;

        let monthinfo = nextMonth.getFullYear() + "." + (nextMonth.getMonth()+1);
        if (monthList.length > 0) {
            nextMonth.setMonth(nextMonth.getMonth()+1);
            recoverinfo += monthinfo;
        }
        nextMonth = nextMonth.getFullYear() + "." + (nextMonth.getMonth()+1);
        

        return (
            <div>
                <div style={{display: "block"}}>
                    <Row>
                        <Col span={8} offset={8}>
                            <p style={{padding: '15px 10px'}}>{"待结月份:"+nextMonth}</p>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8} offset={8}>
                            <Button style={{padding: '5px 12px', margin: '5px 5px', width: '120px'}} type="primary" onClick={(e) => this.createMonth()}>月结</Button>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8} offset={8}>
                            <Button style={{padding:'5px 12px', margin: '5px 5px', width: '120px'}} type="primary" onClick={(e) => this.recoverMonth()}>{recoverinfo}</Button>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8} offset={8}>
                            <Button style={{padding:'5px 12px', margin: '5px 5px', width: '120px'}} type="primary" onClick={(e) => this.createMonthForm()}>生成月结报表</Button>
                        </Col>
                    </Row>
                    <p style={{padding: '15px 10px', borderBottom: '1px solid #e0e0e0', fontSize: '1.4rem', fontWeight: 'bold'}}>目前已为你筛选出<b style={{color: '#f36366'}}> {monthList.length || 0} </b>条月结信息</p>
                    <Spin spinning={showLoading} delay={200} tip="查询中...">
                        {
                            monthList.length>0 ? <div className='searchResult'>
                                {/**搜索结果**/}
                                <Row>
                                    <Col span={24}>
                                        <Layout>
                                            <Header style={{ backgroundColor: '#ececec' }}>
                                                月结列表
                                                    &nbsp;
                                            </Header>
                                            <Content>
                                                <Table rowSelection={rowSelection} rowKey={record => record.key} pagination={this.props.monthresult} columns={MonthListColums} dataSource={this.props.monthresult.extension} onChange={this.handleTableChange} />
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
        showLoading: state.basicData.showLoading,
        monthresult: state.basicData.monthresult,
        monthLast: state.basicData.monthlast
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(stafftableMapStateToProps, stafftableMapDispatchToProps)(Staffinfo);