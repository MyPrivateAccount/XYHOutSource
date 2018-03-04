import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Select, Icon, Button, Row, Col, Upload, message} from 'antd';


class ContractRecord extends Component{
    state = {
        fileList:[],
        imgFiles:{}
    }

    render(){
        return(
            <Upload>
                
            </Upload>
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

