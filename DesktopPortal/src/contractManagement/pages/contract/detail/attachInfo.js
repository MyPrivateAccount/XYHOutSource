import { connect } from 'react-redux';
import { shopPicEdit, contractPicEdit } from '../../../actions/actionCreator';
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
        console.log("进入info");
        if (this.props.contractAttachInfo.fileList) {
            this.getGroup(this.props.contractAttachInfo.fileList)
        }
        this.setState({ fileList: fileList });
        
        
    }

    componentWillReceiveProps(newProps) {
        let fileList = [];
        if (newProps.contractAttachInfo.fileList) {
            this.getGroup(newProps.contractAttachInfo.fileList)
        }    
    }


    handleEdit = (e) => {
        this.props.dispatch(contractPicEdit());//这里后续添加contractPicEdit()
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
                                    //[1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                    <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                            
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
                                                                <div className="clearfix">
                                                                    <Upload  multiple= {true}
                                                                            listType="picture-card"
                                                                            fileList= {this.state.imgFiles[item.value]}
                                                                            onPreview={this.handlePreview}>
                                                                    </Upload>
                                                                    <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                                                                        <img alt="example" style={{ width: '100%' }} src={previewImage} />
                                                                    </Modal>
                                                                </div> :
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
    // console.log(state.shop.shopsInfo.attachInfo, state.building.buildInfo.attachInfo, '图片展示列表' )
    return {
        contractAttachInfo: state.contractData.contractInfo.annexInfo,
        basicData: state.basicData
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AttachInfo);