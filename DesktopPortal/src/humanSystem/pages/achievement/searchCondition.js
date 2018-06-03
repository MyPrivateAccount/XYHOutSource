import {connect} from 'react-redux';
import { setLoadingVisible, postSearchCondition, getSalaryList} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Input, InputNumber, Select, Icon, Button, Row, Col, Spin} from 'antd'
import './search.less';

class SearchCondition extends Component {

    componentWillMount() {
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getSalaryList(this.props.searchInfo.achievementList));
    }

    handleKeyChangeWord = (e) =>{
        this.props.searchInfo.keyWord = e.target.value;
    }

    handleSearch = () => {
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getSalaryList(this.props.searchInfo));
    }

    render() {
        return (
            <div className="searchBox">
                <Row type="flex">
                    <Col span={12}>
                        <Input placeholder={'请输入名称'} onChange = {this.handleKeyChangeWord}/> 
                    </Col>
                    <Col span={8}>
                        <Button type='primary' className='searchButton' onClick={this.handleSearch}>查询</Button>
                    </Col>
                </Row>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchInfo: state.search,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchCondition);