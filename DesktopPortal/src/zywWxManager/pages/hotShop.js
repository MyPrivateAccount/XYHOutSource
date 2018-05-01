import React, {Component} from 'react'
import {Row, Col, Icon, Button, AutoComplete, Input, Spin, Select, Tag} from 'antd'
import {connect} from 'react-redux'
import {searchBuilding} from '../actions'
import {TAG_COLOR} from '../../constants/uiColor'

// const Option = Select.Option;
const Option = AutoComplete.Option;
const reportStyle = {
    row: {
        margin: '10px',
    },
    tagRow: {
        marginTop: '10px'
    },
    input: {
        width: '200px'
    },
    tipInfo: {
        color: '#999',
        fontSize: '12px',
        marginBottom: '5px'
    },
    select: {
        width: '200px',
        marginRight: '10px'
    }
}
let timer = null;
class HotShop extends Component {
    state = {
        showDialog: false,
        operType: 'add',
        current: {},
        hotBuildingCondition: {
            keyWord: '',
            pageIndex: 0,
            pageSize: 15
        },
        hotSection: {
            cityList: [],
            areaList: [],
            selectedCity: '',
            selectedArea: ''
        }

    }
    componentWillMount() {
        let hotSection = this.state.hotSection || {};
        let areaList = this.props.areaList || [];
        let userInfo = ((this.props.oidc || {}).user || {}).profile;
        if (userInfo && userInfo.City !== '') {
            areaList = areaList.filter(area => area.value === userInfo.City);
        }
        hotSection.cityList = areaList;
        this.setState({hotSection: hotSection});
    }
    //城市切换
    handleCityChange = (value) => {
        let hotSection = this.state.hotSection || {};
        let areaList = this.props.areaList || [];
        if (value) {
            let area = areaList.find(area => area.value === value);
            if (area) {
                hotSection.areaList = area.children || [];
                this.setState({hotSection: hotSection});
            }
        }
    }
    //地区切换
    handleAreaChange = (value) => {

    }
    //处理楼盘变更
    handleHotBuildingChange = (value) => {
        let {hotBuildingSearchList} = this.props.hotShop;
        let result = (hotBuildingSearchList || []).find(b => b.id === value);
        if (result) {
            return;
        }
        let condition = this.state.hotBuildingCondition;
        condition.keyWord = value;
        if (timer) {
            clearInterval(timer);
        }
        setTimeout(() => {this.props.dispatch(searchBuilding(condition));}, 500);
    }
    //推荐
    recommend = (value, recommendType) => {
        if (recommendType === "building") {//热门楼盘

        } else if (recommendType === "area") {//热门地段

        } else if (recommendType === "shop") {//热门商铺

        }
    }

    handleTagClose = (key, tagType) => {
        if (tagType === "building") {//热门楼盘

        } else if (tagType === "area") {//热门地段

        } else if (tagType === "shop") {//热门商铺

        }
    }

    render() {
        let {hotBuildingSearchList} = this.props.hotShop;
        let hotOptions = [];
        (hotBuildingSearchList || {}).map(b => {
            hotOptions.push(<Option key={b.id} value={b.name}>{b.name}</Option>);
        });
        console.log("选择列表:", hotBuildingSearchList);
        let {cityList, areaList} = this.state.hotSection;
        return (
            <div className="inner-page full">
                <Row>
                    <Col span={24}><h2><Icon type="tags-o" />热门楼盘</h2></Col>
                </Row>
                <Row style={reportStyle.row}>
                    <Col>热门楼盘：
                        <AutoComplete
                            className="certain-category-search"
                            dropdownClassName="certain-category-search-dropdown"
                            dropdownMatchSelectWidth={false}
                            dropdownStyle={{width: 300}}
                            style={{width: '200px'}}
                            dataSource={hotOptions}
                            placeholder="input here"
                            optionLabelProp="value"
                            onChange={this.handleHotBuildingChange}
                        >
                            <Input suffix={<Icon type="search" className="certain-category-icon" />} />
                        </AutoComplete>
                        <Button icon="like-o">推荐</Button>
                    </Col>
                    <Col style={reportStyle.tagRow}>
                        <Tag closable color={TAG_COLOR} onClose={(e) => this.handleTagClose(e, 'building')}>保利悦都</Tag>
                        <Tag closable color={TAG_COLOR} onClose={(e) => this.handleTagClose(e, 'building')}>保利悦都</Tag>
                    </Col>
                </Row>
                <Row>
                    <Col span={24}><h2><Icon type="tags-o" />热门地段</h2></Col>
                </Row>
                <Row style={reportStyle.row}>
                    <Col>热门地段：
                        <Select style={reportStyle.select} onChange={this.handleCityChange}>
                            <Option key='0'>不限</Option>
                            {(cityList || []).map(area => <Option key={area.value}>{area.label}</Option>)}
                        </Select>
                        <Select style={reportStyle.select}>
                            <Option key='0'>不限</Option>
                            {(areaList || []).map(area => <Option key={area.value}>{area.label}</Option>)}
                        </Select>
                    </Col>
                    <Col style={reportStyle.tagRow}>
                        <Tag closable color={TAG_COLOR} onClose={(e) => this.handleTagClose(e, 'area')}>高新-金融城</Tag>
                    </Col>
                </Row>
                <Row>
                    <Col span={20} style={{textAlign: 'center'}}><Button type="primary">保存</Button> <Button type="primary">取消</Button></Col>
                </Row>
                <Row>
                    <Col span={24}><h2><Icon type="tags-o" />热门商铺</h2></Col>
                </Row>
                <Row style={reportStyle.row}>
                    <Col>楼盘：
                        <Select style={reportStyle.select} onChange={this.handleCityChange}><Option key='0'>不限</Option></Select>
                        商铺：
                        <Select style={reportStyle.select}><Option key='0'>不限</Option></Select>
                    </Col>
                    <Col style={reportStyle.tagRow}>
                        <Tag closable color={TAG_COLOR} onClose={(e) => this.handleTagClose(e, 'shop')}>保利悦都</Tag>
                    </Col>
                </Row>
                <Row>
                    <Col span={20} style={{textAlign: 'center'}}><Button type="primary">保存</Button> <Button type="primary">取消</Button></Col>
                </Row>
            </div >
        )
    }
}

const mapStateToProps = (state, props) => {
    return {
        hotShop: state.hotShop,
        oidc: state.oidc,
        areaList: state.rootBasicData.areaList
    }
}

const mapDispatchToProps = (dispatch) => {
    return {
        dispatch
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(HotShop);