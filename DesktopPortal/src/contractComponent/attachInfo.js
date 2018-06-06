
import React, { Component } from 'react'
import { Layout, Table, Button, Icon, Popconfirm, Tooltip, Col, Row, Modal, Upload ,Tabs } from 'antd'


const { Header, Sider, Content } = Layout;
const TabPane = Tabs.TabPane;

class AttachInfo extends Component {
    state = {
        previewVisible: false,
        previewImage: '',
        fileList: [],
        projectId: '',
        
        group: [],
        picGroup: '', // 图片分类值
        imgFiles: {},
        expandStatus: true
    }

    componentDidMount() {
        let fileList = [];
    
        if (this.props.contractAttachInfo.fileList) {
            let curFileList = this.props.contractAttachInfo.fileList;//直接修改会改变state
            this.getGroup(curFileList);
        }
        this.setState({ fileList: fileList });
        
        
    }

    componentWillReceiveProps(newProps) {
        let fileList = [];
       // console.log('newProps.contractAttachInfo.fileList:', newProps.contractAttachInfo.fileList);
        if (newProps.contractAttachInfo.fileList) {
            let curFileList = newProps.contractAttachInfo.fileList;//直接修改会改变state
            this.getGroup(curFileList);
           
        }    
    }

    getGroup = (fl)  => {
        if(fl && fl.length>0){
          let list= {}
          fl.forEach(v => {
              v.group = v.group ? v.group : '5'
              if (!list.hasOwnProperty(v.group)) {
                  list[v.group] = [{
                    uid: v.fileGuid,
                    name: v.name || '',
                    status: 'done',
                    url: v.icon || v.localUrl
                  }]
              } else {
                  list[v.group].push({
                    uid: v.fileGuid,
                    name: v.name || '',
                    status: 'done',
                    url: v.icon || v.localUrl
                  })
              }
          })
          //console.log("imgFiles:", list);
          this.setState({
              imgFiles: list
          })
        }
      }
    

    handlePreview = (file) => {
        this.setState({
            previewImage: file.url || file.localUrl,
            previewVisible: true,
        });
    }
    handleCancel = () => this.setState({ previewVisible: false })
    render() {
        let { contractAttachInfo } = this.props;
        let { previewVisible, previewImage, fileList, group } = this.state;
        
        let propsPic = {
            multiple: true,
            listType: "picture-card",
            fileList: fileList,
            onPreview: (file) => { // 图片预览
                this.setState({
                    previewImage: file.url || file.localUrl,
                    previewVisible: true,
                });
            }
        }
        let contractFileList = contractAttachInfo.fileList || [];
        console.log('contractFileList:', contractFileList);
   
        return (
            <div className="">
                <Layout>
                    <Content className='attchInfo' style={{ padding: '12px 0', margin: '20px 0', backgroundColor: "#ECECEC" }}>

                        <Row type="flex" >
                            <Col span={23}>
                            {
                            
                                 <div>
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>附加信息</span>
                                 </div>
                            }
                               
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                        </Row>
                        <div style={{display: this.state.expandStatus ? "block" : "none"}}>
                            <Row type="flex" justify="space-between">

                                {
                                    <Col span={24}>
                                        {contractFileList.length !== 0 ?
                                            <div>
                                                <div className='picture'>图片</div>

                                                <Tabs defaultActiveKey="1" className='picBox'>
                                                    {
                                                        this.props.basicData.contractAttachTypes.map((item, i) => {
                                                            return (
                                                                <TabPane tab={item.key} key={item.value}>
                                                            
                                                                    <div className='picBox'>
                                                                        {
                                                                        this.state.imgFiles[item.value] ? 
                                                                        (this.state.imgFiles[item.value] || []).map((fileItem, index) =>{
                                                                            let arr = [];
                                                                            arr.push(fileItem);
                                                                            return(            
                                                                            <Row type='flex' align="middle" key={fileItem.uid}>        
                                                                                <Col span={4}>
                                                                                <Upload  listType="picture-card"
                                                                                    fileList= {arr}
                                                                                    onPreview={this.handlePreview} 
                                                                                    beforeUpload={this.handleBeforeUpload} 
                                                                                    onRemove={this.hanldeRemove} >
                                                                                
                                                                                </Upload>
                                                                                </Col>
                                                                                <Col span={12} style={{marginBottom: '10px'}}>附件备注:{fileItem.ext1 || "无" }</Col>
                                                                            </Row>        
                                                                            )
                                                                        })  :
                                                                        <div style={{display: 'flex', alignItems: 'center', justifyContent: 'center'}}>暂无数据</div>
                                                                    }
                                                                    </div> 

                                                                
                                                                
                                                            </TabPane>  
                                                            )
                                                        })
                                                    }
                                                </Tabs>
                                            </div>
                                            : <div style={{ marginLeft: '50px' }}>无图片</div>
                                        }
                                    </Col>
                                }

                            </Row>
                        </div>
                    </Content>
                </Layout>
            </div>
        )
    }
}

export default AttachInfo;