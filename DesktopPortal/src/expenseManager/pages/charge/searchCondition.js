import {connect} from 'react-redux';
import { setLoadingVisible, postSearchCondition, updateSearchStatu, updateChargePrice} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Input, InputNumber, Select, Icon, Button, Row, Col, Checkbox, Tag, Spin, Radio, DatePicker} from 'antd'
import './search.less';
import moment from 'moment';

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
        this.props.dispatch(postSearchCondition(this.props.searchInfo));
    }

    handleSearchBoxToggle = (e) => {//筛选条件展开、收缩
        let visible = !this.state.expandSearchCondition;
        this.setState({expandSearchCondition: visible});
    }

    handleSearch = () => {
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(postSearchCondition(this.props.searchInfo));
    }

    handleCheckChange = (e, type) => {
    }

    handleKeyChangeWord = (e) =>{
        this.props.searchInfo.keyWord = e.target.value;
    }

    handleCheckStatus = (e) => {
        this.props.dispatch(updateSearchStatu(e.target.id));
    }

    handlePriceRange = (e) => {
        this.props.dispatch(updateChargePrice(e));
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
                <div className='searchCondition'>
                    <Row>
                        <Col span={12}>
                            <span> 所有费用></span>
                            {/* <span> {this.state.filterTags.map((tag, i) => <Tag closable onClose={e => this.handleTagClose(tag, i)} key={tag.label + i}>{tag.label}</Tag>)}</span> */}
                        </Col>
                        <Col span={4}>
                            <Button onClick={this.handleSearchBoxToggle}>{expandSearchCondition ? "收起筛选" : "展开筛选"}<Icon type={expandSearchCondition ? "up-square-o" : "down-square-o"} /></Button>
                        </Col>
                    </Row>
                    <div style={{display: expandSearchCondition ? "block" : "none"}}>   
                        <Row className="normalInfo">
                            <Col span={4}>
                                <label>未审核：</label>
                                    <Checkbox checked={this.props.searchInfo.checkStatu === 1?true:false} id={1} onChange={this.handleCheckStatus}></Checkbox>
                            </Col>
                            <Col span={4}>
                                <label>已审核：</label>
                                    <Checkbox checked={this.props.searchInfo.checkStatu === 2?true:false} id={2} onChange={this.handleCheckStatus}></Checkbox>
                            </Col>
                        </Row>
                        <Row className="normalInfo">
                            <Col span={24}>
                                <label style={styles.conditionRow}>金额 ：</label>
                                {
                                    PriceRanges.map(pri =>
                                        <Button className={pri.value === this.props.searchInfo.chargePrice ? "RangeBtn BtnActive" : "RangeBtn"}
                                         key={pri.value} onClick={(e) => this.handlePriceRange(pri.value)}>{pri.label}</Button>
                                    )
                                }
                            </Col>
                        </Row>
                    </div>
                </div>
                {<p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{this.props.resultList.length}</b>条费用信息</p>}
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        chargePrice: state.search.chargePrice,
        checkStatu: state.search.checkStatu,
        searchInfo: state.search,
        resultList: state.search.searchResult.extension//我也不想，但是列表更新了只能通过这个来重新渲染
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchCondition);