import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Select, Icon, Button, Row, Col, Upload, message} from 'antd';
import ContractEdit from './contract/edit/contractEdit';

class ContractRecord extends Component{
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

}

function mapDispathToProps(dispatch){
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispathToProps)(ContractRecord);

