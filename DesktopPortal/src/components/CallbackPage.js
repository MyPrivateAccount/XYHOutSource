import React, {Component } from 'react';
import {connect} from 'react-redux';
import {CallbackComponent} from 'redux-oidc';
import { push} from 'react-router-redux';
import userManager from '../utils/userManager';
import Spinner from './Spinner';

class CallbackPage extends Component{
    successCallback = (user) => {
        this.props.dispatch(push('/'));
    }
    errorCallback =(res)=>{
        console.error(res);
    }

    render(){
        return (
            <CallbackComponent userManager={userManager} successCallback={this.successCallback} errorCallback={this.errorCallback}>
            <Spinner />
          </CallbackComponent>
        )
    }
}

function mapDispatchToProps(dispatch) {
    return {
      dispatch
    };
  }
  
  export default connect(null, mapDispatchToProps)(CallbackPage);