import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Select, Icon, Button, Row, Col, Upload, message} from 'antd';
import { getDicParList } from '../actions/actionCreator';
import ContractEdit from './contract/edit/contractEdit';

class ContractRecord extends Component{
    componentWillMount(){
        if(this.props.basicData.contractCategories.length === 0 || this.props.basicData.firstPartyCatogories.length === 0){
          this.props.dispatch(getDicParList(['CONTRACT_CATEGORIES', 'FIRST_PARTT_CATEGORIES', 'COMMISSION_CATEGORIES', 'XK_SELLER_TYPE']));
        }
      }
    state = {
        curOperType: 0
    }

    render(){
        let curOperType = this.state.curOperType;
        return(
            <div>
                {
                    curOperType === 0 ? <ContractEdit /> : null
                }
            </div>
        )
    }
}

function mapStateToProps(state){
    return{
        basicData: state.basicData,
    }
    
}

function mapDispathToProps(dispatch){
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispathToProps)(ContractRecord);

