import React, {Component } from 'react';
import Spinner from 'react-spinkit';
import './Spinner.less'

class SpinnerEx extends Component{
    render(){
        let color = 'red'
        let name = 'ball-clip-rotate-multiple';
        if(this.props.color){
            color =this.props.color;
        }
        if(this.props.name){
            name = this.props.name;
        }
        if(this.props.theme && this.props.theme.palette){
            color = this.props.theme.palette.primary[500]
          }
        return (
            <div className="spinner root">
                <Spinner className="inner" color={color} name={name} />
            </div>
        )
    }
}

  export default SpinnerEx;