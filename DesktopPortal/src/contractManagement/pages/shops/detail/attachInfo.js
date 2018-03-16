import { connect } from 'react-redux';
import { shopPicEdit, buildingPicEdit,getDynamicInfoList, editBatchBuilding } from '../../../actions/actionCreator';
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
        if (this.props.parentPage === 'building') {
            if (this.props.buildingAttachInfo.fileList) {
                this.getGroup(this.props.buildingAttachInfo.fileList)
            }
            this.setState({ fileList: fileList });
        } else {
            if (this.props.shopAttachInfo.fileList) {
                this.props.shopAttachInfo.fileList.map(file => {
                    fileList.push({
                        uid: file.fileGuid,
                        name: file.name || '',
                        status: 'done',
                        url: file.icon || file.localUrl
                    });
                });
            }
            this.setState({ fileList: fileList });
        }
        
    }

    componentWillReceiveProps(newProps) {
        let fileList = [];
        if (this.props.parentPage === 'building') {
            if (newProps.buildingAttachInfo.fileList) {
                this.getGroup(newProps.buildingAttachInfo.fileList)
            }
        } else {
            console.log(newProps.shopAttachInfo.fileList, '图片详情INfolist')
            if (newProps.shopAttachInfo.fileList.length) {
                newProps.shopAttachInfo.fileList.map(file => {
                    fileList.push({
                        uid: file.fileGuid,
                        name: file.name || '',
                        status: 'done',
                        url: file.icon || file.localUrl
                    });
                });
                 this.setState({ fileList: fileList });
            }
           
        }
    }


    handleEdit = (e) => {
        this.props.parentPage === "building" ? this.props.dispatch(buildingPicEdit()) : this.props.dispatch(shopPicEdit())
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
        let { shopAttachInfo, buildingAttachInfo } = this.props;
        let shopfileList, buildingfileList;
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
        shopfileList = shopAttachInfo.fileList || [];
        buildingfileList = buildingAttachInfo.fileList || [];
        console.log(shopfileList, '商铺列表')
        return (
            <div className="">
                <Layout>
                    <Content className='attchInfo' style={{ padding: '12px 0', margin: '20px 0', backgroundColor: "#ECECEC" }}>

                        <Row type="flex" >
                            <Col span={20}>
                            {
                                 this.props.type === 'dynamic' ? null : 
                                 <div>
                                 <Icon type="tags-o" className='content-icon' /> <span className='content-title'>附加信息</span>
                                 </div>
                            }
                               
                            </Col>
                            <Col span={4}>
                                {
                                     // this.props.type = 'dynamic' 说明这个页面是从动态房源哪里引用的。因为动态房源都是审核通过的页面，但是可以进行修改，所以要加以判断
                                    this.props.type === 'dynamic' ? 
                                    // [1].includes(a.examineStatus) ? null : 
                                    <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} /> 
                                    :
                                    // 下面的判断是因为在新增房源那里，1和8状态的楼盘都不可修改
                                    (this.props.parentPage === "building" ?
                                    [1, 8].includes(this.props.buildInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                    :
                                    [1, 8].includes(this.props.shopInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />)
                                }
                            </Col>
                        </Row>

                        <Row type="flex" justify="space-between">

                            {
                                this.props.parentPage === "building" ?
                                    <Col span={24}>
                                        {buildingfileList.length !== 0 ?
                                            <div>
                                                <div className='picture'>图片</div>

                                                <Tabs defaultActiveKey="1" className='picBox'>
                                                    {
                                                       this.props.basicData.photoCategories.map((item, i) => {
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
                                    </Col> :
                                    <Col span={24}>
                                        { shopfileList.length !== 0 ?
                                            <div>
                                                <div className='picture'>图片</div>

                                                <div className='picBox'>
                                                    <div className="clearfix">
                                                        <Upload  {...propsPic} ></Upload>
                                                        <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                                                            <img alt="example" style={{ width: '100%' }} src={previewImage} />
                                                        </Modal>
                                                    </div>
                                                </div>
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
        shopAttachInfo: state.shop.shopsInfo.attachInfo,
        buildingAttachInfo: state.building.buildInfo.attachInfo,
        buildInfo: state.building.buildInfo,
        shopInfo: state.shop.shopsInfo,
        statusArr: state.active.statusArr,
        dynamicShopAttachInfo: state.shop.dynamicShopsInfo.attachInfo,
        dynamicBuildingAttachInfo: state.building.dynamicBuildInfo.attachInfo,
        projectId: state.active.projectId,
        shopId: state.active.shopId,
        dynamicStatusArr: state.active.dynamicStatusArr,
        basicData: state.basicData
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AttachInfo);