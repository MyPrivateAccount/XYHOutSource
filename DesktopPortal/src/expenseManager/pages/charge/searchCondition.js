import {connect} from 'react-redux';
import { setLoadingVisible, postSearchCondition} from '../actions/actionCreator';
import React, {Component} from 'react'
import {Input, InputNumber, Select, Icon, Button, Row, Col, Checkbox, Tag, Spin, Radio, DatePicker} from 'antd'
import './search.less';
import moment from 'moment';
import SearchBox from './searchBox';

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
    { value: 0, label: '不限'}, 
    { value: 1, label: '1000以上'},
    { value: 2, label: '2000岁以上'},
    { value: 3, label: '3000岁以上'}
]

class SearchCondition extends Component {
    state = {
        expandSearchCondition: true
    }

    componentWillMount() {
    }

    componentDidMount() {
    }

    componentWillReceiveProps(newProps) {
    }

    handleSearchBoxToggle = (e) => {//筛选条件展开、收缩
        let visible = !this.state.expandSearchCondition;
        this.setState({expandSearchCondition: visible});
    }

    handleSearch = () => {
        this.props.dispatch(postSearchCondition(this.props.searchInfo));
    }

    handleCheckChange = (e, type) => {
    }

    handleKeyChangeWord = (e) =>{
        this.state.searchText = e.target.value;
    }

    handleCheckStatus = (e) => {
        this.props.searchInfo.checkStatus = e.target.id;
        this.setState(this.state);
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
                            <span>所有费用></span>
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
                                    <Checkbox checked={this.props.searchInfo.checkStatus === 1?true:false} id={1} onChange={this.handleCheckStatus}></Checkbox>
                            </Col>
                            <Col span={4}>
                                <label>已审核：</label>
                                    <Checkbox checked={this.props.searchInfo.checkStatus === 2?true:false} id={2} onChange={this.handleCheckStatus}></Checkbox>
                            </Col>
                        </Row>
                        <Row className="normalInfo">
                            <Col span={24}>
                                <label style={styles.conditionRow}>金额 ：</label>
                                {
                                    PriceRanges.map(age =>
                                        <Button className={age.value === this.props.searchInfo.ageCondition ? "staffRangeBtn staffBtnActive" : "staffRangeBtn"}
                                         key={age.value} onClick={(e) => this.handlePriceRange(age.value)}>{age.label}</Button>
                                    )
                                }
                            </Col>
                        </Row>
                        {      
                            <Row className="normalInfo">
                                <Col>
                                    {activeMenu !== "menu_invalid" ?
                                        <label><span style={{marginRight: '10px'}}>录入日期：</span>
                                            <DatePicker disabledDate={this.disabledDate} value={createDateStart} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateStart')} />- <DatePicker disabledDate={this.disabledDate} value={createDateEnd} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateEnd')} />
                                        </label> : null}
                                </Col>
                            </Row>
                       }
                    </div>
                </div>
                {activeMenu === "menu_index" ? <p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{dataSourceTotal}</b>条合同信息</p> : null}
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