import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Select, Icon, Button, Row, Col, Upload, message} from 'antd';
import AttachEdit from './contract/edit/attachEdit';

class AttachMent extends Component{
    state = {
        attachShowType: 'add'
    }

    render(){
        let curShowType = this.state.attachShowType;
        return(
            <div>
                <AttachEdit/>
            </div>
        );

        
    }
}

function mapStateToProps(state){

}

function mapDispathToProps(dispatch){
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispathToProps)(AttachMent);

