import {connect} from 'react-redux';
import {setLoadingVisible, setSearchLoadingVisible, adduserPage, deleteBlackInfo, getBlackList, selBlackList} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Input, Spin, Popconfirm, Button, notification, Row, Col, Table, Modal} from 'antd';
import './search.less';
import Layer, {LayerRouter} from '../../../components/Layer'
import {Route} from 'react-router'
import Addblack from './addblack'
import BlackForm from '../../../businessComponents/humanSystem/blackInfo'
import ApiClient from '../../../utils/apiClient'
import WebApiConfig from '../../constants/webapiConfig';
const buttonDef = [
    {buttonID: "addnew", buttonName: "新建", icon: '', type: 'primary', size: 'large', }
];


class MainIndex extends Component {
    state = {
        condition: {
            keyWord: "",
            pageIndex: 0,
            pageSize: 10
        },
        showLoading: false,
        blackList: {extension: [], pageIndex: 0, pageSize: 10, totalCount: 0},
        showDetail: false,//详情
        checkList: [],//选中列表
        editblackInfo: null//当前编辑黑名单
    }

    componentWillMount() {
        this.handleSearch()
    }
    _getColumns() {
        const columns = [
            {title: '身份证', dataIndex: 'idCard', key: 'idCard', },
            {title: '姓名', dataIndex: 'name', key: 'name'},
            {title: '性别', dataIndex: 'sex', key: 'sex', render: (text, record) => <div>{text == 1 ? "男" : '女'}</div>},
            {title: '电话', dataIndex: 'phone', key: 'phone'},
            {title: 'email', dataIndex: 'email', key: 'email'},
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

    gotoSubPage = (path, params) => {
        this.props.history.push(`${this.props.match.url}/${path}`, params || {})
    }

    handleClickFucButton = (e) => {
        console.log("操作按钮:", e);
        if (e.target.id === "addnew") {
            this.gotoSubPage('addblack', {})
        }
    }

    handleOperClick = (record, type) => {
        if (type === "modify") {
            record.sex = record.sex + '';
            this.gotoSubPage('editblack', record)
        } else if (type === "delete") {
            this.handleDelete(record.id);
        }
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

    showDetail = (record) => {
        this.setState({showDetail: true, editblackInfo: record});
    }
    handleSearch = () => {
        let result = {isOk: false, extension: {}, msg: '获取黑名单列表失败！'};
        let url = WebApiConfig.search.getBlackList;
        let _this = this;
        let blackList = this.state.blackList;
        this.setState({showLoading: true});
        ApiClient.post(url, this.state.condition).then(res => {
            this.setState({showLoading: false});
            if (res.data.code == 0) {
                result.isOk = true;
                blackList = {
                    extension: res.data.extension, pageIndex: res.data.pageIndex, pageSize: res.data.pageSize, totalCount: res.data.totalCount
                }
                _this.setState({blackList: blackList});
            }
        }).catch(e => {
            this.setState({showLoading: false});
            result.msg = '检索关键字接口调用异常';
            notification.error({
                description: result.msg,
                duration: 3
            });
        })
    }

    handleDelete = (blackInfoId) => {
        let url = WebApiConfig.server.DeleteBlack + blackInfoId;
        let huResult = {isOk: false, msg: '删除黑名单失败！'};
        ApiClient.post(url, null, null, 'DELETE').then(res => {
            if (res.data.code == 0) {
                huResult.msg = '删除黑名单成功';
                notification.success({
                    message: huResult.msg,
                    duration: 3
                });
            }
            this.handleSearch();
        }).catch(e => {
            huResult.msg = "删除黑名单接口调用异常!";
            notification.error({
                message: huResult.msg,
                duration: 3
            });
        })
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
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                console.log("选中行:", selectedRows);
                this.setState({checkList: selectedRows});
            }
        };
        let showLoading = this.state.showLoading;
        let columns = this._getColumns();
        let blackList = this.state.blackList;
        let paginationInfo = {current: blackList.pageIndex, pageSize: blackList.pageSize, total: blackList.totalCount};
        return (
            <Layer showLoading={showLoading}>
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
                    buttonDef.map((button, i) => {
                        let hasPermission = this.hasPermission(button);
                        return (hasPermission ? <Button id={button.buttonID} key={button.buttonID} style={{marginTop: '10px', marginBottom: '10px', marginRight: '10px', border: 0}}
                            onClick={this.handleClickFucButton}
                            icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button> : null)
                    })
                }
                <div>
                    {<p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{blackList.totalCount}</b>条费用信息</p>}
                    <div id="searchResult">
                        <Table id={"table"} rowKey={record => record.id}
                            columns={columns}
                            pagination={paginationInfo}
                            onChange={this.handleChangePage}
                            dataSource={blackList.extension} bordered size="middle"
                            rowSelection={rowSelection} />
                    </div>
                </div>
                <Modal title="黑名单详情" width='850px'
                    visible={this.state.showDetail}
                    onCancel={() => this.setState({showDetail: false})}
                    footer={<Button type="primary" onClick={() => this.setState({showDetail: false})}>关闭</Button>}
                >
                    <BlackForm entityInfo={this.state.editblackInfo} readOnly />
                </Modal>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/addblack`} render={(props) => <Addblack  {...props} />} />
                    <Route path={`${this.props.match.url}/editblack`} render={(props) => <Addblack  {...props} />} />
                </LayerRouter>
            </Layer>
        )
    }
}

function mapStateToProps(state) {
    return {
        selBlacklist: state.basicData.selBlacklist,
        showLoading: state.basicData.showLoading,
        searchInfoResult: state.search,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);