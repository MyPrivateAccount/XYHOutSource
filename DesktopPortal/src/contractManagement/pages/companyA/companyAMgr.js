import { connect } from 'react-redux';
import { getDicParList, companyAAdd, companyListGet, companyADelete, companyAEdit, companyAListUpdate, companyACloseDialog } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import CompanyAEdit from './companyAEdit';

import SearchCondition from '../../constants/searchCondition';

const { Header, Content } = Layout;
const Option = Select.Option;
const TreeNode = TreeSelect.TreeNode;

class CompanyAMgr extends Component {
    state = {
        checkList: [],
        condition: { isSearch: true, keyWord: '', searchType: '', address: '' },//条件
        dataLoading: false,
        pagination: {}
    }

    componentWillMount = () => {

        this.handleSearch();
    }

    handleCompanyAClick = (Info) => {
        console.log("编辑：", Info);
        this.props.dispatch(companyAEdit(Info));
    }

    appTableColumns = [
        {
            title: '选择', dataIndex: 'clientID', key: 'check', render: (text, record) => (
                <span>
                    <Checkbox defaultChecked={false} onChange={(e) => this.handleCheckChange(e, record)} />
                </span>
            )
        },
        { title: '甲方公司名称', dataIndex: 'name', key: 'name' },
        {
            title: '类型', dataIndex: 'type', key: 'type', render: (text, record) => {
                let key = '';
                this.props.basicData.firstPartyCatogories.forEach(item => {
                    if (item.value === text) {
                        key = item.key;
                    }
                });
                return key;
            }
        },
        { title: '地址', dataIndex: 'address', key: 'address' },
        { title: '联系电话', dataIndex: 'phoneNum', key: 'phoneNum' },
        // { title: '邮箱', dataIndex: 'email', key: 'email' },
        {
            title: '编辑', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="修改">
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleEditClick(recored)} />
                    </Tooltip>
                    {/* <Tooltip title='删除'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleEmpRoleClick(recored)} />
                    </Tooltip> */}
                    <Tooltip title="删除">
                        <Popconfirm title="确认要删除选中公司吗?" onConfirm={(e) => this.handleCompanyADelete(recored)} onCancel={this.cancelDelete} okText="确定" cancelText="取消">
                            &nbsp;<Button type='primary' shape='circle' size='small' icon='delete' />
                        </Popconfirm>
                    </Tooltip>
                </span>
            )
        }
    ];
    componentWillReceiveProps(newProps) {
        this.setState({ dataLoading: false });
        let paginationInfo = {
            pageSize: newProps.companySearchResult.pageSize,
            current: newProps.companySearchResult.pageIndex,
            total: newProps.companySearchResult.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });
    }
    handleCompanyADeleteAll = (e) => {//删除
        let deleteIds = [];
        this.state.checkList.map((item, i) => {
            if (item.status == true) {
                deleteIds.push(item.id);
            }
        });
        console.log("删除列表：", deleteIds);
        this.props.dispatch(companyADelete(deleteIds));
    }
    handleCompanyADelete = (e) => {//删除
        let deleteIds = [];
        deleteIds.push(e.id);
        console.log("删除列表：", deleteIds);
        this.props.dispatch(companyADelete(deleteIds));
    }
    // handleResetPwd = (empInfo) => {//重置密码
    //     console.log("重置密码：", empInfo);
    //     this.props.dispatch(empRestPwd(empInfo.userName));
    // }
    handleAddClick = (event) => {
        console.log('handleAddClick');
        this.props.dispatch(companyAAdd());
    }
    handleEditClick = (info) => {
        console.log("handleEditClick", info);
        this.props.dispatch(companyAEdit(info));
    }
    handleCheckChange = (e, Info) => {
        //console.log("checkbox change：" + JSON.stringify(e.target.checked));
        let compayAId = Info.id;
        let checked = e.target.checked;
        let checkList = this.state.checkList.slice();
        var hasValue = false;
        for (var i in checkList) {
            if (checkList[i].id == compayAId) {
                checkList[i].status = checked;
                hasValue = true;
                break;
            }
        }
        if (!hasValue) {
            checkList.push({ id: compayAId, status: checked });
        }
        this.setState({ checkList: checkList });
    }
    handleNameChange = (e) => {
        //console.log("输入内容：", e.target.value);
        let condition = { ...this.state.condition };
        condition.keyWord = e.target.value;
        this.setState({ condition: condition });
    }
    handleAddressChange = (e) => {
        //console.log("输入内容：", e.target.value);
        let condition = { ...this.state.condition };
        condition.address = e.target.value;
        this.setState({ condition: condition });
    }
    handleSearch = (e) => {
        SearchCondition.companyASearchCondition = this.state.condition;
        SearchCondition.companyASearchCondition.pageIndex = 0;
        SearchCondition.companyASearchCondition.pageSize = 10;
        console.log("查询条件", SearchCondition);
        this.setState({ dataLoading: true });
        this.props.dispatch(companyListGet(SearchCondition.companyASearchCondition));
    }

    handleTableChange = (pagination, filters, sorter) => {
        SearchCondition.companyASearchCondition.pageIndex = (pagination.current - 1);
        SearchCondition.companyASearchCondition.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.props.dispatch(companyListGet(SearchCondition.companyASearchCondition));
    };
    handleTypeChange = (type) => {
        console.log("类型变更:", type);
        let condition = { ...this.state.condition };
        condition.searchType = type;
        this.setState({ condition: condition });
    }
    handleOrgChange = (orgs) => {
        console.log("部门改变:", orgs);
        let condition = { ...this.state.condition };
        condition.OrganizationIds = orgs;
        this.setState({ condition: condition });
    }

    render() {
        let removeBtnDisabled = this.state.checkList.filter((item) => item.status).length > 0 ? false : true;
        let itemOptions = [];
        if (this.props.basicData.firstPartyCatogories) {
            this.props.basicData.firstPartyCatogories.map(item => itemOptions.push(<Option key={item.value}>{item.key}</Option>));
        }
        //this.props.judgePermissions.includes('RECORD_FUC')
        let judgePermissions = this.props.judgePermissions || [];
        return (
            <div className="relative">
                <Layout>

                    <Content>
                        <Row>
                            <Col style={{ margin: '7px 5px' }}>
                                <div >
                                    <Row>
                                        <Col span={6}>
                                            甲方名称: <Input value={this.state.condition.keyWord} style={{ width: '70%', verticalAlign: 'middle' }} onPressEnter={this.handleSearch} onChange={this.handleNameChange} />
                                        </Col>
                                        <Col span={6}>
                                            所属类型: <Select style={{ width: '70%' }} value={this.state.condition.searchType} allowClear
                                                onChange={this.handleTypeChange}>
                                                {itemOptions}
                                            </Select>
                                        </Col>
                                        <Col span={6}>
                                            所在地址:<Input value={this.state.condition.address} style={{ width: '70%', verticalAlign: 'middle' }} onPressEnter={this.handleSearch} onChange={this.handleAddressChange} />

                                        </Col>
                                        <Col span={6}>
                                            <Button type="primary" icon="search" onClick={this.handleSearch}>查询</Button>
                                        </Col>
                                    </Row>
                                </div>
                            </Col>
                        </Row>
                        <Header style={{ backgroundColor: '#ececec' }}>
                            甲方列表&nbsp;
                            {
                                judgePermissions.includes('HT_ADD_JF') ? <Button type='primary' shape='circle' icon='plus' onClick={this.handleAddClick} /> : null
                            }
                            {
                                judgePermissions.includes('HT_JF_DELETE') ? <Popconfirm title="确认要删除选中甲方?" onConfirm={this.handleCompanyADeleteAll} onCancel={this.cancelDelete} okText="确定" cancelText="取消">
                                    &nbsp;<Button type='primary' shape='circle' icon='delete' disabled={removeBtnDisabled} />
                                </Popconfirm> : null
                            }
                        </Header>
                        <Spin spinning={this.state.dataLoading}>
                            {<Table rowKey={record => record.id} pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.companySearchResult.extension} onChange={this.handleTableChange} />}
                        </Spin>
                        <CompanyAEdit />

                    </Content>
                </Layout>
            </div >
        )
    }
}

function tableMapStateToProps(state) {
    //console.log("companyAData:" + JSON.stringify(state.companyAData.companySearchResult));
    return {
        operInfo: state.companyAData.operInfo,
        basicData: state.basicData,
        companySearchResult: state.companyAData.companySearchResult,
        //permissionOrgTree: state.org.permissionOrgTree,
        judgePermissions: state.judgePermissions
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(CompanyAMgr);