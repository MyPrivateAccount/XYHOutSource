//结佣对话框
import React, { Component } from 'react'
import {Popconfirm } from 'antd'

class JYComponet extends Component {
    state = {
        vs:false
    }
    confirm = (e) => {
        
    };
    cancel = (e) => {
        
    };
    render() {
        return (
            <Popconfirm title="请确认xxx报告的结佣资料是否已经收齐?" onConfirm={this.confirm} onCancel={this.cancel} okText="Yes" cancelText="No">
               
          </Popconfirm>
        )
    }
}
export default JYComponet