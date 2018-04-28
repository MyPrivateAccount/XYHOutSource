import {connect} from 'react-redux';
import {getRepeatJudgeInfo, setLoadingVisible} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Spin, Input, Steps, notification} from 'antd';
import './search.less';
import moment from 'moment';

const Step = Steps.Step;

const repeatCustomerStyle = {
    // color: '#f04134',
    margin: '10px 0',
    fontWeight: '700',
    fontSize: '1.2rem'
}

class RepeatCustomer extends Component {
    state = {
        keyword: '',
        buildingName: ''
    }
    componentWillMount() {

    }
    handleKeyWordChange = (e) => {
        this.setState({keyword: e.target.value.trim()});
    }
    handleBuildingNameChange = (e) => {
        this.setState({buildingName: e.target.value.trim()});
    }
    //重客检索
    handleSearch = (e) => {
        console.log("查询条件:", this.state.keyword, this.state.buildingName);
        if (this.state.keyword === "") {
            notification.warning({
                message: '客户电话号码不能为空',
                duration: 3
            });
            return;
        }
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getRepeatJudgeInfo({
            phone: this.state.keyword,
            buildingName: this.state.buildingName
        }));
    }

    render() {
        const showLoading = this.props.showLoading;
        let repeatJudgeInfo = this.props.repeatJudgeInfo || {};
        if (repeatJudgeInfo) {
            repeatJudgeInfo.followUpResponses = repeatJudgeInfo.followUpResponses || [];
            if (repeatJudgeInfo.mainPhone) {
                repeatJudgeInfo.mainPhone = repeatJudgeInfo.mainPhone.replace(/([0-9]{3})[0-9]+([0-9]{4})/g, '$1****$2');
            }
        }
        return (
            <Spin spinning={showLoading}>
                <div className="searchBox" style={{display: 'flex', flexDirection: 'row', }}>
                    <p>
                        <span style={{color:'#EF3236',marginRight:'10px',fontWeight:'700' }}>*</span><Input style={{width: '300px'}} placeholder='请输入客户电话号码' value={this.state.keyword} onChange={this.handleKeyWordChange} onPressEnter={this.handleSearch} />
                        {/* <Button type="primary" className='searchButton' onClick={this.handleSearch}>搜索</Button> */}
                    </p>
                    <p style={{marginLeft: '30px'}}>
                        <Input style={{width: '300px'}} placeholder='请输入楼盘名称' value={this.state.buildingName} onChange={this.handleBuildingNameChange} onPressEnter={this.handleSearch} />
                        <Button type="primary" className='searchButton' onClick={this.handleSearch}>搜索</Button>
                    </p>
                </div>
                <div style={{marginLeft: '20px'}}>
                    {
                        (!repeatJudgeInfo || repeatJudgeInfo.followUpResponses.length === 0) ? 

                            <p style={repeatCustomerStyle}>暂无记录!</p> :

                            <p>
                                <p style={repeatCustomerStyle}><b style={{marginRight: '20px'}}>{repeatJudgeInfo.customerName}</b><b>{repeatJudgeInfo.mainPhone}</b></p>
                                {
                                    (repeatJudgeInfo.followUpResponses || []).map((v, index) => {
                                        return (
                                            <p key={index} style={{borderBottom: '1px dashed #dcdcdc'}}>
                                                <p style={{fontWeight: '700', margin: '15px 0', color: '#f04134', fontSize: '1.1rem'}}>{v.buildingName}</p>
                                                <p>
                                                    <Steps direction="vertical" current={1}>
                                                        {
                                                            (v.transactionsFollowUpResponse || []).map((result, index) => {
                                                                let desc = <div><p style={{padding: '15px 0'}}>业务员：{v.userTrueName || '无'}</p><p>分行（组）名称：{v.departmentName || '无'}</p></div>;
                                                                let title = moment(result.createTime).format("YYYY-MM-DD HH:mm:ss") + " " + result.contents;
                                                                return (<Step key={index} status='finish' title={title} description={desc} />)
                                                            })
                                                        }
                                                    </Steps>
                                                </p>
                                            </p>
                                        )
                                    })
                                }
                            </p>
                    }
                </div>
            </Spin>
        )
    }

}

function mapStateToProps(state) {
    return {
        showLoading: state.search.showLoading,
        repeatJudgeInfo: state.search.repeatJudgeInfo
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(RepeatCustomer);