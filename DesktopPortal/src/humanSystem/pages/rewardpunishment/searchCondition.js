import {connect} from 'react-redux';
import { setLoadingVisible, postSearchCondition, updateSearchStatu, updateChargePrice} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Input, InputNumber, Select, Icon, Button, Row, Col, Checkbox, Tag, Spin} from 'antd'
import './search.less';

const Option = Select.Option;
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

class SearchCondition extends Component {
    state = {
        expandSearchCondition: true
    }

    componentWillMount() {
        //this.props.dispatch(searchAttendenceList(this.props.searchInfo));
    }

    handleKeyChangeWord = (e) => {
        this.props.searchInfo.keyWord = e.target.value;
    }

    handleSearch = () => {
        //this.props.dispatch(searchAttendenceList(this.props.searchInfo));
    }

    render() {
        return (
            <div>
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