import { connect } from 'react-redux';
import { setLoadingVisible, openAttachMent, openContractRecord,getAllOrgList } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Menu, Icon, Row, Col, Spin, Checkbox, Button } from 'antd';
import {getDicParList} from '../actions/actionCreator';

import CompanyAMgr from './companyA/companyAMgr';//'../companyA/companyAMgr';


class CompanyA extends Component {

    componentWillMount() {
        if(this.props.basicData.firstPartyCatogories.length === 0)
        {
            this.props.dispatch(getDicParList([ 'FIRST_PARTT_CATEGORIES',]));
        }
    }



    render() {
  
        return (
            <div >
                <CompanyAMgr/>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        basicData:state.basicData,

    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(CompanyA);