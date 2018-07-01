import {connect} from 'react-redux';
import {setSearchLoadingVisible, getDicParList, adduserPage, searchRewardPunishment, deleteRewardPunishment} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Input, Spin, Upload, Checkbox, Button, notification, Modal, Row, Col, InputNumber, Table} from 'antd';
import Layer, {LayerRouter} from '../../../components/Layer'
import {Route} from 'react-router'
import './search.less';
import InputRPInfo from './inputRPInfo'

const buttonDef = [
    {buttonID: "import", buttonName: "录入", icon: '', type: 'primary', size: 'large', },
    {buttonID: "delete", buttonName: "删除", icon: '', type: 'primary', size: 'large', },
];

const columns = [
    {title: '工号', dataIndex: 'userID', key: 'userID', },
    {title: '姓名', dataIndex: 'name', key: 'name'},
    {title: '有效日期', dataIndex: 'workDate', key: 'workDate'},
    {title: '金额', dataIndex: 'money', key: 'money'},
    {title: '类型', dataIndex: 'typename', key: 'typename', },
    {title: '详细类型', dataIndex: 'detailname', key: 'detailname', },
    {title: '备注', dataIndex: 'comments', key: 'comments'},
];

class MainIndex extends Component {
    state = {
        selList: [],
    }

    componentWillMount() {//ADMINISTRATIVE_REWARD  ADMINISTRATIVE_PUNISHMENT  ADMINISTRATIVE_DEDUCT
        this.props.dispatch(setSearchLoadingVisible(true));
        this.props.dispatch(searchRewardPunishment(this.props.searchInfo));
        this.props.dispatch(getDicParList(["ADMINISTRATIVE_REWARD", "ADMINISTRATIVE_PUNISHMENT", "ADMINISTRATIVE_DEDUCT"]));
    }
    
    handleClickFucButton = (e) => {
        if (e.target.id === "import") {
            this.props.dispatch(adduserPage({id: "2", menuID: "awpuinput", displayName: '行政奖惩录入', type: 'item'}));
            this.gotoSubPage("inputRPInfo", {});
        } else if (e.target.id === "delete") {
            if (this.state.selList.length > 0) {
                this.props.dispatch(deleteRewardPunishment(this.state.selList[this.state.selList.length-1].id));
            }
            else {
                notification.error({
                    message: '未选择指定奖惩',
                    description: "请选择指定奖惩",
                    duration: 3
                });
            }
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

    handleKeyChangeWord = (e) => {
        this.props.searchInfo.keyWord = e.target.value;
    }

    handleSearch = () => {
        this.props.dispatch(searchRewardPunishment(this.props.searchInfo));
    }

    gotoSubPage = (path, params) => {
        this.props.history.push(`${this.props.match.url}/${path}`, {...params})
    }

    render() {
        let self = this;
        let showLoading = this.props.showLoading;

        let lst =["","行政奖励","行政惩罚","行政扣款"];
        let datalist = this.props.searchInfo.rewardpunishmenList.extension.map(function(v, i) {
            let detailtype = null;
            switch (v.type) {
                case 1:detailtype = this.props.administrativereward;break;
                case 2:detailtype = this.props.administrativepunishment;break;
                case 3:detailtype = this.props.administrativededuct;break;
                default:;break;
            }
            return {key: i+"", ...v, typename:lst[v.type], detailname:detailtype?detailtype[v.detail].key:null};
        });
        
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                self.setState({selList: selectedRows});
            },
            getCheckboxProps: record => ({
                disabled: record.name === 'Disabled User', // Column configuration not to be checked
                name: record.name,
            }),
        }

        return (
            <Layer>
                <Spin spinning={showLoading}>
                    <div className="searchBox">
                        <Row type="flex">
                            <Col span={12}>
                                <Input placeholder={'请输入名称'} onChange={this.handleKeyChangeWord} />
                            </Col>
                            <Col span={8}>
                                <Button type='primary' className='searchButton' onClick={this.handleSearch}>查询</Button>
                            </Col>
                        </Row>
                    </div>
                    {
                        buttonDef.map(
                            (button, i) => this.hasPermission(button) ?
                                <Button key={i} id={button.buttonID} style={{ marginTop:"10px",marginBottom: '10px', marginRight: '10px', border:0}}
                                    onClick={this.handleClickFucButton}
                                    icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button>
                                : null)
                    }
                    <div>
                        <p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{datalist.length}</b>条考勤信息</p>
                        <div id="searchResult">
                            <Table id={"table"} rowKey={record => record.key}
                                columns={columns}
                                pagination={this.props.searchInfo}
                                onChange={this.handleChangePage}
                                dataSource={datalist} bordered size="middle"
                                rowSelection={rowSelection} />
                        </div>
                    </div>
                </Spin>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/inputRPInfo`} render={(props) => <InputRPInfo  {...props} />} />
                </LayerRouter>
            </Layer>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchInfo: state.search,
        showLoading: state.search.showLoading,
        administrativereward: state.basicData.administrativereward,
        administrativepunishment: state.basicData.administrativepunishment,
        administrativededuct: state.basicData.administrativededuct,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);