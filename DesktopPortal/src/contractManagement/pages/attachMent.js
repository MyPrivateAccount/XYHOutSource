import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Select, Icon, Button, Row, Col, Upload, message} from 'antd';
import AttachEdit from './contract/edit/attachEdit';
import AttachInfo from './contract/detail/attachInfo'
class AttachMent extends Component{


    render(){
        let attachPicOperType = this.props.attachPicOperType;
        console.log("attachPicOperType:", attachPicOperType);
        return(
            <div>
                {attachPicOperType === 'add' ? <AttachEdit/> : <AttachInfo/>}
            </div>
        );

        
    }
}

function mapStateToProps(state){
    return{
        attachPicOperType: state.contractData.operInfo.attachPicOperType,
    }
}

function mapDispathToProps(dispatch){
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispathToProps)(AttachMent);

