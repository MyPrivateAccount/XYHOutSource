import React from 'react';
import userManager from '../utils/userManager';
import Spinner from './Spinner';
import { Form, Icon, Input, Button, Checkbox, Row, Col } from 'antd';
import './Spinner.less'


const FormItem = Form.Item;

class LoginPage extends React.Component {
  onLoginButtonClick = (event) => {
    event.preventDefault();
    userManager.signinRedirect();

  };
  componentWillMount() {
    userManager.signinRedirect();
  };

  render() {
    const loginStyle = {
      position: 'absolute',
      width: '400px',
      height: '200px',
      left: '50%',
      top: '50%',
      marginLeft: '-200px',
      marginTop: '-100px',
      border: '1px solid #ccc'
    }
    const { getFieldDecorator } = this.props.form;
    return (
      <Spinner  />
    );
  }
}
const WrappedNormalLoginForm = Form.create()(LoginPage);

export default WrappedNormalLoginForm;