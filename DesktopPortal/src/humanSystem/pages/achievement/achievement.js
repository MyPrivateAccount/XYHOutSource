import {connect} from 'react-redux';
import React, {Component} from 'react';
import {Input, Table, Button, notification, Row, Col, Popconfirm} from 'antd';
import './search.less';
import Layer, {LayerRouter} from '../../../components/Layer'
import {Route} from 'react-router'
import Achievement from './addachievement'
import ApiClient from '../../../utils/apiClient'
import WebApiConfig from '../../constants/webapiConfig';
const buttonDef = [
    {buttonID: "addnew", buttonName: "新建", icon: '', type: 'primary', size: 'large', },
    // {buttonID: "modify", buttonName: "修改", icon: '', type: 'primary', size: 'large', },
    // {buttonID: "delete", buttonName: "删除", icon: '', type: 'primary', size: 'large', },
];


class MainIndex extends Component {
    state = {
        condition: {
            keyWord: '',
            pageIndex: 0,
            pageSize: 10
        },
        showLoading: false,
        salaryList: {extension: [], pageIndex: 0, pageSize: 10, totalCount: 0}
    }

    componentWillMount() {
        this.handleSearch();
    }
    componentDidMount() {
        this.handleSearch();
    }
    gotoSubPage = (path, params) => {
        this.props.history.push(`${this.props.match.url}/${path}`, params)
    }

    handleSearch = () => {
        let condition = this.state.condition;
        let result = {isOk: false, extension: {}, msg: '获取薪酬列表失败！'};
        let url = WebApiConfig.search.getSalaryList;
        ApiClient.post(url, condition).then(res => {
            if (res.data.code == 0) {
                result.isOk = true;
                this.setState({
                    salaryList: {
                        extension: res.data.extension || [],
                        pageIndex: res.data.pageIndex,
                        pageSize: res.data.pageSize,
                        totalCount: res.data.totalCount
                    }
                });
            }
            if (!result.isOk) {
                notification.error({
                    description: result.msg,
                    duration: 3
                });
            }
        }).catch(e => {
            result.msg = '薪酬列表接口调用异常!';
            notification.error({
                description: result.msg,
                duration: 3
            });
        });
    }

    handleClickFucButton = (e) => {
        if (e.target.id === "addnew") {
            this.gotoSubPage('achievement', {});
        }
    }

    handleOperClick = (record, type) => {
        if (type === "modify") {
            this.gotoSubPage('achievement', record)
        } else if (type === "delete") {
            this.handleDelete(record.id);
        }
    }
    //删除薪酬记录
    handleDelete = (blackInfoId) => {
        let url = WebApiConfig.server.deleteSalary;
        let huResult = {isOk: false, msg: '删除薪酬失败！'};
        ApiClient.post(url, null, null, 'DELETE').then(res => {
            if (res.data.code == 0) {
                huResult.msg = '删除薪酬成功';
                notification.success({
                    message: huResult.msg,
                    duration: 3
                });
            }
            this.handleSearch();
        }).catch(e => {
            huResult.msg = "删除薪酬接口调用异常!";
            notification.error({
                message: huResult.msg,
                duration: 3
            });
        })
    }
    //是否有权限
    hasPermission(buttonInfo) {
        let hasPermission = false;
        if (this.props.judgePermissions && buttonInfo.requirePermission) {
            for (let i = 0; i < buttonInfo.requirePermission.length; i++) {
                if (this.props.judgePermissions.includes(buttonInfo.requirePermission[i])) {
                    hasPermission = true;
                    break;
                }
            }
        } else {
            hasPermission = true;
        }
        return hasPermission;
    }

    getColumns() {
        let columns = [
            {title: '组织(分公司)', dataIndex: 'organize', key: 'organize', },
            {title: '职位', dataIndex: 'positionName', key: 'positionName'},
            {title: '基本工资', dataIndex: 'baseSalary', key: 'baseSalary'},
            {title: '岗位补贴', dataIndex: 'subsidy', key: 'subsidy'},
            {title: '工装扣款', dataIndex: 'clothesBack', key: 'clothesBack'},
            {title: '行政扣款', dataIndex: 'administrativeBack', key: 'administrativeBack'},
            {title: '端口扣款', dataIndex: 'portBack', key: 'portBack'},
            {
                title: "操作", dataIndex: "operation", key: "operation",
                render: (text, record) => {
                    let hasModifyPermission = this.hasPermission({buttonID: "modify", buttonName: "修改"});
                    let hasDeletePermission = this.hasPermission({buttonID: "delete", buttonName: "删除"});
                    return (
                        <span>
                            {hasModifyPermission ? <Button type="primary" size='small' shape="circle" icon="edit" style={{marginRight: '5px'}} onClick={() => this.handleOperClick(record, 'modify')} /> : null}
                            {hasDeletePermission ? <Button type="primary" size='small' shape="circle" icon="idcard" style={{marginRight: '5px'}} onClick={() => this.showDetail(record)} /> : null}
                            <Popconfirm title="确定要删除该记录?" onConfirm={() => this.handleOperClick(record, 'delete')} okText="是" cancelText="否">
                                <Button type="primary" shape="circle" size='small' icon="delete" style={{marginRight: '5px'}} />
                            </Popconfirm>
                        </span>
                    );
                }
            }
        ];
        return columns;
    }

    //翻页
    handleChangePage = (pageIndex) => {
        let condition = this.state.condition;
        condition.pageIndex = pageIndex;
        this.setState({condition: condition}, () => {
            this.handleSearch();
        });
    }
    handleKeyChangeWord = (e) => {
        let condition = this.state.condition;
        condition.keyWord = e.target.value;
        this.setState({condition: condition})
    }
    render() {
        let salaryList = this.state.salaryList;
        let paginationInfo = {current: salaryList.pageIndex, pageSize: salaryList.pageSize, total: salaryList.totalCount}
        return (
            <Layer showLoading={this.state.showLoading}>
                <div className="searchBox">
                    <Row type="flex">
                        <Col span={12}>
                            <Input placeholder={'请输入名称'} onChange={this.handleKeyChangeWord} onPressEnter={this.handleSearch} />
                        </Col>
                        <Col span={8}>
                            <Button type='primary' className='searchButton' onClick={this.handleSearch}>查询</Button>
                        </Col>
                    </Row>
                </div>
                {
                    buttonDef.map(
                        (button, i) => this.hasPermission(button) ?
                            <Button key={i} id={button.buttonID} style={{marginTop: '10px', marginBottom: '10px', marginRight: '10px', border: 0}}
                                onClick={this.handleClickFucButton}
                                icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button> : null
                    )
                }
                <div>
                    {<p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{salaryList.totalCount}</b>条职位薪酬信息</p>}
                    <div id="searchResult">
                        <Table id={"table"} rowKey={record => record.id}
                            columns={this.getColumns()}
                            pagination={paginationInfo}
                            onChange={this.handleChangePage}
                            dataSource={salaryList.extension} bordered size="middle"
                        />
                    </div>
                </div>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/achievement`} render={(props) => <Achievement  {...props} />} />
                </LayerRouter>
            </Layer>
        )
    }
}

function mapStateToProps(state) {
    return {
        selAchievementList: state.basicData.selAchievementList
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);