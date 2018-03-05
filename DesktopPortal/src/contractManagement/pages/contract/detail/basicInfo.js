import { connect } from 'react-redux';
import { editBuildingBasic } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Table, Button, Checkbox, Row, Col, Form } from 'antd'
import moment from 'moment';

class BasicInfo extends Component {
    state = {

    }
    componentWillMount() {

    }

    handleEdit = (e) => {
        //this.props.dispatch(editBuildingBasic());
    }

    render(){
        return (

        )
    }
}

function mapStateToProps(state) {
    return {
        buildInfo: state.building.buildInfo,
        basicData: state.basicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BasicInfo);