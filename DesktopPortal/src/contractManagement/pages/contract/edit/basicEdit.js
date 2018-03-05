import { connect } from 'react-redux';
import { getAreaList, saveBuildingBasic, viewBuildingBasic,basicLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Input, InputNumber, Button, Checkbox, Row, Col, Form, DatePicker, Cascader } from 'antd'
import moment from 'moment';

const FormItem = Form.Item;

class BasicEdit extends Component {
    render(){
        return (

        )
    }
}

function mapStateToProps(state) {
    return {
    //   dataSource: state.shop.buildingNoInfos,
    //   basicInfo: state.shop.shopsInfo.basicInfo,
    //   buildingList: state.shop.buildingList,
    //   operInfo: state.shop.operInfo,
    //   shopsInfo: state.shop.shopsInfo,
    //   basicData: state.basicData,
    //   loading: state.shop.basicloading
    }
  }
  
  const mapDispatchToProps = (dispatch) => {
    return {
      dispatch,
    //   save: (...args) => dispatch(saveShopBasicAsync(...args)),
    //   getBuildingsList: () => dispatch(getBuildingsListAsync()),
    //   viewShopBasic: () => dispatch(viewShopBasic())
    }
  }
  
  export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(BasicEdit));