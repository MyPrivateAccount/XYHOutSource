import {connect} from 'react-redux';
import {getRepeatJudgeInfo, setLoadingVisible} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Spin, Input, Steps, notification} from 'antd';
import './search.less';
import moment from 'moment';

const Step = Steps.Step;
const repeatCustomerStyle = {
    color: '#169bd5',
    margin: '10px 0'
}

class RepeatCustomer extends Component {
    state = {
        keyword: ''
    }
    componentWillMount() {

    }
    handleKeyWordChange = (e) => {
        this.setState({keyword: e.target.value});
    }
    //重客检索
    handleSearch = (e) => {
        console.log("查询条件:", this.state.keyword);
        if (this.state.keyword === "") {
            notification.warning({
                description: '客户电话号码不能为空',
            });
            return;
        }
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getRepeatJudgeInfo(this.state.keyword));
    }

    render() {
        const showLoading = this.props.showLoading;
        let repeatJudgeInfo = this.props.repeatJudgeInfo;
        if (repeatJudgeInfo) {
            repeatJudgeInfo.followUpResponses = repeatJudgeInfo.followUpResponses || [];
            if (repeatJudgeInfo.mainPhone) {
                repeatJudgeInfo.mainPhone = repeatJudgeInfo.mainPhone.replace(/([0-9]{3})[0-9]+([0-9]{4})/g, '$1****$2');
            }
        }
        return (
            <Spin spinning={showLoading}>
                <div className="searchBox">
                    <p>
                        <Input style={{width: '450px'}} placeholder='请输入客户电话号码' value={this.state.keyword} onChange={this.handleKeyWordChange} onPressEnter={this.handleSearch} />
                        <Button type="primary" className='searchButton' onClick={this.handleSearch}>搜索</Button>
                    </p>
                </div>
                <div>
                    {
                        (repeatJudgeInfo === null) ? <p style={repeatCustomerStyle}>暂无记录!</p> : <div>
                            <p style={repeatCustomerStyle}><b style={{marginRight: '10px'}}>{repeatJudgeInfo.customerName}</b><b>{repeatJudgeInfo.mainPhone}</b></p>
                            <Steps direction="vertical" current={1}>
                                {
                                    repeatJudgeInfo.followUpResponses.map((result, index) => {
                                        let desc = <div><p>业务员：{result.userTrueName}</p><p>分行（组）名称：{result.departmentName}</p></div>;
                                        let title = moment(result.followUpTime).format("YYYY-MM-DD HH:mm:ss") + " " + result.followUpContents;
                                        return (<Step key={index} status='process' title={title} description={desc} />)
                                    })
                                }
                            </Steps>
                        </div>
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