import {connect} from 'react-redux';
import {changeCurCity} from '../actions/index'
import React, {Component} from 'react';
import {Button, Icon, Menu, Checkbox} from 'antd';

const {SubMenu} = Menu;
const itemStyle = {
    itemBorder: {
        position: 'relative',
        zIndex: '100',
        background: '#fff',
        width: '300px'
    },
    autoHeight: {
        height: '100%'
    }
}

class CitySelect extends Component {
    state = {
        checkedCity: []
    }

    handleCityChecked = ({item, key, keyPath}) => {
        const cityList = this.props.areaList || [];
        let city = cityList.find(c => c.value === key);
        if (city) {
            this.props.dispatch(changeCurCity(city));
        }
        if (this.props.hideCitySelect) {
            this.props.hideCitySelect();
        }
    }

    render() {
        let {curCity} = this.props;
        curCity = curCity || {};
        const cityList = this.props.areaList || [];
        return (
            <div style={itemStyle.itemBorder}>
                <Menu selectedKeys={this.state.checkedCity} onClick={this.handleCityChecked} style={itemStyle.autoHeight} >
                    {/* <Menu.Item key="0">不限</Menu.Item> */}
                    {cityList.map(city =>
                        <Menu.Item key={city.value}>{curCity === city.value ? <Icon type="check" /> : null}{city.label}</Menu.Item>
                    )}
                </Menu>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        curCity: state.footPrint.curCity,
        areaList: state.rootBasicData.areaList
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(CitySelect);