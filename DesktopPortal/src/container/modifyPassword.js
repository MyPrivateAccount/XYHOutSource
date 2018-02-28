import React, { Component } from 'react';
import XWindow from '../components/XWindow'
import { withStyles } from 'material-ui/styles';
import TextField from 'material-ui/TextField';
import Button from 'material-ui/Button';
import Grid from 'material-ui/Grid';
import { connect } from 'react-redux';
import { closeWindow } from '../actions/actionCreators';

const paneStyle = {
    width: '30rem',
    height: '30rem',
    top: '15%',
    left: '40%',
};

const styles = theme => ({
    container: {
        display: 'flex',
        flexWrap: 'wrap',
    },
    textField: {
        marginLeft: theme.spacing.unit,
        marginRight: theme.spacing.unit,
    },
});


class ModifyPasswordWindow extends Component {
    state = {
        oldPassword: '',
        newPassword: '',
        confirmPassword: '',
        error: {
            oldPassword: {
                error: false,
                message: ''
            },
            newPassword: {
                error: false,
                message: ''
            },
            confirmPassword: {
                error: false,
                message: ''
            }

        }
    };

    handleChangeMultiline = event => {
        this.setState({
            multiline: event.target.value,
        });
    };

    handleChange = name => event => {
        this.setState({
            [name]: event.target.value,
        }, () => {
            if( name == 'oldPassword'){
                if(!this.state.oldPassword){
                    this.setErrorMessage("oldPassword",true,"必须输入原密码");
                }else{
                    this.clearError("oldPassword");
                }
                return;
            }
      

            if (name === 'newPassword' || name === 'confirmPassword') {
                if(!this.state.newPassword){
                    this.setErrorMessage("newPassword",true,"必须设置新密码");
                    return;
                }else if(this.state.newPassword===this.state.oldPassword)
                {
                    this.setErrorMessage("newPassword",true,"新密码不可与旧密码相同");
                    return;
                }
                else{
                    this.clearError("newPassword");
                }
                if (this.state.newPassword !== this.state.confirmPassword) {
                    this.setErrorMessage("newPassword", true, "两次密码输入不一致");
                    this.setErrorMessage("confirmPassword", true, "两次密码输入不一致");
                } else {
                    this.setErrorMessage("newPassword", false, "");
                    this.setErrorMessage("confirmPassword", false, "");
                }
            }
        });


    };

    setErrorMessage(name, error, message) {
        this.setState((preState, props) => {
            let obj = Object.assign({}, preState.error);
            obj[name].error = error;
            obj[name].message = message;
            return { error: obj };
        })

    }
    clearError(name){
        this.setState((preState,props)=>{
            if(name){
                preState.error[name].error=false;
                preState.error[name].message='';
                return preState;
            }
            for(var k in preState.error){
                if(preState.error.hasOwnProperty(k)){
                    preState.error[k].error=false;
                    preState.error[k].message='';
                }
            }
            return preState;
        })
    }

    closeWindow(){
        this.props.dispatch(closeWindow(this.props.id));
    }
    submit(){
        if(this.state.error.oldPassword.error || 
            this.state.error.newPassword.error || 
            this.state.error.confirmPassword.error){
                console.log(this.state.error)
                return;
            }

        if(!this.state.oldPassword){
            this.setErrorMessage("oldPassword",true,"必须输入原密码");
            this.forceUpdate();
            return;
        }
        if(!this.state.newPassword){
            this.setErrorMessage("newPassword",true,"必须设置新密码");
            this.forceUpdate();
            return;
        }
        
        this.clearError();

        alert('提交')


    }

    render() {
        const { classes } = this.props;
        const error = this.state.error;
        return (
            <XWindow ref="wnd" {...this.props} style={paneStyle}>
                <form className={classes.container} noValidate autoComplete="off">
                    <TextField
                        required
                        fullWidth
                        id="old_password"
                        label="原密码"
                        type="password"
                        error={error.oldPassword.error}
                        placeholder="请输入原密码"
                        className={classes.textField}
                        value={this.state.oldPassword}
                        onChange={this.handleChange('oldPassword')}
                        margin="normal"
                        helperText={error.oldPassword.message}
                    />
                    <TextField
                        required
                        fullWidth
                        error={error.newPassword.error}
                        id="new_password"
                        label="新密码"
                        type="password"
                        placeholder="请输入新密码"
                        className={classes.textField}
                        value={this.state.newPassword}
                        onChange={this.handleChange('newPassword')}
                        margin="normal"
                        helperText={error.newPassword.message}
                    />
                    <TextField
                        required
                        fullWidth
                        error={error.confirmPassword.error}
                        id="confirm_password"
                        label="密码确认"
                        type="password"
                        placeholder="请再次输入新密码"
                        className={classes.textField}
                        value={this.state.confirmPassword}
                        onChange={this.handleChange('confirmPassword')}
                        margin="normal"
                        helperText={error.confirmPassword.message}
                    />
                    <Grid spacing={0} container justify="flex-end" style={{marginTop:'1rem'}}>
                        <Button dense color="accent" onClick={()=> this.submit()}>确定</Button>
                        <Button dense color="primary" onClick={()=> this.closeWindow() }>取消</Button>
                    </Grid>
                </form>
            </XWindow>
        )
    }
}
function mapDispatchToProps(dispatch) {
    return {
      dispatch
    };
  }
export default connect(null,mapDispatchToProps)(withStyles(styles)(ModifyPasswordWindow));