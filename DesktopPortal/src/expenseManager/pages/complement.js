import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Select, Icon, Button, Row, Col, Upload, message} from 'antd';
import ComplementEdit from './contract/edit/complementEdit';
import ComplementInfo from './contract/detail/complementInfo'
class Complement extends Component{


    render(){
        let complementOperType = this.props.complementOperType;
       
        return(
            <div>
                {complementOperType === 'view'  ? <ComplementInfo/> : <ComplementEdit/> }
            </div>
        );

        
    }
}

function mapStateToProps(state){
    return{
        complementOperType: state.contractData.operInfo.complementOperType,
    }
}

function mapDispathToProps(dispatch){
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispathToProps)(Complement);

