//附件组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Modal, Upload, Menu, Icon, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import Avatar from './rpdetails/tradeUpload'
const { Header, Content } = Layout;

class SKAttact extends Component {
    state = {
        reportFiles:this.props.fileList,
        type:this.props.type
    }
    appendData = (data)=>{
        let reportFiles = this.state.reportFiles
        reportFiles.push(data)
        this.setState({reportFiles})
    }
    getFileList=()=>{
        let list = []
        let reportFiles = this.state.reportFiles
        for(let i=0;i<reportFiles.length;i++){
                let fl = {uid:'',name:'',status:'done',url:''}
                fl.uid = reportFiles[i].fileGuid
                fl.name = reportFiles[i].name
                fl.status = 'done'
                fl.url = reportFiles[i].uri
                list.push(fl)
        }
        return list
    }
    render() {
        return (
                <Layout>
                    <Content>
                        <div className="clearfix" style={{ margin: 10 }}>
                            <Avatar id={this.state.type} fileList={this.getFileList} append={this.appendData}/>
                        </div>
                    </Content>
                </Layout>
        )
    }
}
export default SKAttact