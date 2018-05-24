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
export const PriceRanges = [
    {key:0, value: 1, label: '不限'}, 
    {key:1, value: 1000, label: '1000以上'},
    {key:2, value: 2000, label: '2000以上'},
    {key:3, value: 3000, label: '3000以上'}
]

class SearchCondition extends Component {
    state = {
        expandSearchCondition: true
    }

    componentWillMount() {
        //this.props.dispatch(postSearchCondition(this.props.searchInfo));
    }

    render() {
        let expandSearchCondition = this.state.expandSearchCondition;

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
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchCondition);