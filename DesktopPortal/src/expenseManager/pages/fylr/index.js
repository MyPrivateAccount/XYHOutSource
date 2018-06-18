import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Button} from 'antd'

class FylrIndex extends Component{
    addCharge = ()=>{
        this.props.history.push('/dd')
    }

    render(){
        return <div className="content-page">
            <div>
                搜索区域
            </div>
            <div className="page-btn-bar">
                <Button type="primary" onClick={this.addCharge}>录入报销单</Button>
            </div>
            <div className="page-fill" style={{backgroundColor:'red'}}>
                表格区域
            </div>
        </div>
    }
}

export default connect()(FylrIndex)