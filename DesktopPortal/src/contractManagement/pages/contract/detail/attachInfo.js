import { connect } from 'react-redux';
import { shopPicEdit, contractPicEdit,openAttachMentStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Icon, Popconfirm, Tooltip, Col, Row, Modal, Upload ,Tabs } from 'antd'
import '../edit/editCommon.less'

const { Header, Sider, Content } = Layout;
const TabPane = Tabs.TabPane;

class AttachInfo extends Component {
    state = {
        previewVisible: false,
        previewImage: '',
        fileList: [],
        projectId: '',
        shopId: '',
        group: [],
        picGroup: '', // 图片分类值
        imgFiles: {}
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


    handleEdit = (e) => {
        this.props.dispatch(contractPicEdit());
        this.props.dispatch(openAttachMentStart({id:3}));
    }
    handleCancel = () => this.setState({ previewVisible: false })

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
                    url: v.icon || v.localUrl,
                    ext1: v.ext1,
                  }]
              } else {
                  list[v.group].push({
                    uid: v.fileGuid,
                    name: v.name || '',
                    status: 'done',
                    url: v.icon || v.localUrl,
                    ext1: v.ext1,
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
        return (
            <div className="">
                <Layout>
                    <Content className='attchInfo' style={{ padding: '12px 0', margin: '20px 0', backgroundColor: "#ECECEC" }}>

                        <Row type="flex" >
                            <Col span={20}>
                            {
                            
                                 <div>
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>附加信息</span>
                                 </div>
                            }
                               
                            </Col>
                            <Col span={4}>
                                {
                                    // 下面的判断是因为在新增房源那里，1和8状态的楼盘都不可修改
                                    [1].includes(contractAttachInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                    //<Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            
                                }
                            </Col>
                        </Row>

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
                                                                    console.log('fileItem:', fileItem);
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
                    </Content>
                </Layout>
            </div>
        )
    }
}

function mapStateToProps(state) {
    //console.log(state.contractData.contractInfo.attachInfo, '图片展示列表' )
    return {
        contractAttachInfo: state.contractData.contractInfo.attachInfo,
        basicData: state.basicData
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AttachInfo);