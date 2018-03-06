import { connect } from 'react-redux';
import { getDicParList, savePictureAsync, buildingPicView, shopPicView, 
  deletePicAsync, saveCompleteFileList, saveDeletePicList,attchLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import './editCommon.less'
import {
  Layout, Button, message, Icon, Row, Col, Form, Input,
  Radio, Select, Upload, Modal, notification, Tabs
} from 'antd'
import { NewGuid } from '../../../../utils/appUtils';
import WebApiConfig from '../../../constants/webapiConfig'


const { Header, Sider, Content } = Layout;
const Option = Select.Option;
const FormItem = Form.Item;
const TabPane = Tabs.TabPane;

class AttachEdit extends Component {
  state = {
    previewVisible: false,
    previewImage: '',
    fileList: [
    ],
    attachments: [], //附件
    picUploading: false,
    // completeFileList: [],
    // deletePicList: [],
    deleteIdArr: [],
    picGroup: '1', // 图片分类值
    imgFiles: {}
  };
  componentWillMount(){
    if(this.props.basicData.contractAttachTypes.length === 0){
      this.props.dispatch(getDicParList(['CONTRACT_ATTACHMENT_CATEGORIES']));
    }
  }
  attachImgeCategories = [
    {name:'分类1', num:1, order:1, isRequired:1, isDisable:0, comment:''},
    {name:'分类2', num:2, order:1, isRequired:1, isDisable:0, comment:''},
    {name:'分类3', num:3, order:1, isRequired:1, isDisable:0, comment:''},
    {name:'分类4', num:4, order:1, isRequired:1, isDisable:0, comment:''},
    {name:'分类5', num:5, order:1, isRequired:1, isDisable:0, comment:''},

  ];
  handleCancel = () => this.setState({previewVisible:false});
  handlePreview = (file) => {
    this.setState(
      {
        previewImage: file.url || file.thumbUrl,
        previewVisible:true,
      }
    )
  }
  handleChange = ({fileList}) => this.setState({fileList:fileList});
  handleBeforeUpload = (uploadFile) => {
    let reader = new FileReader();
    reader.readAsDataURL(uploadFile);

    reader.onloadend = function () {
      if (uploadFile.type.startsWith("image/")) {
          let nowImgFiles = this.state.imgFiles[this.state.picGroup] || []
          let projectArr = nowImgFiles.concat([{
            uid: uploadFile.uid,
            name: uploadFile.name,
            status: 'uploading',
            url: reader.result
          }])
          let imgFiles =  Object.assign({}, this.state.imgFiles)
          imgFiles[this.state.picGroup] = projectArr
          this.setState({ imgFiles: imgFiles });
      }
    }.bind(this);

    if (!uploadFile.type.startsWith("image/")) {
      notification.error({
        message: '只能上传图片！',
        duration: 3
      });
      return false;
    }

    /*this.UploadFile(uploadFile, (ufile) => {
      console.log("回调信息：", ufile);
        let imgFiles =  Object.assign({}, this.state.imgFiles)
        imgFiles[this.state.picGroup].map(file => {
          if (file.uid === ufile.fileGuid) {
            file.status = 'done';
            ufile.localUrl = file.url;
            ufile.group = this.state.picGroup
          }
        });
        this.setState({ imgFiles: imgFiles }, ()=>{
          console.log(this.state.imgFiles, '上传之后的回调信息：')
        });
      
      let completeFileList = [...this.props.completeFileList, ufile];
      //console.log(completeFileList, '7')
      //this.props.dispatch(saveCompleteFileList({ completeFileList: completeFileList }))
    });*/

    return true;
  }
  UploadFile = (file, callback) => {
    // console.log(file);
    let shopsInfo = this.props.shopsInfo;
    let buildInfo = this.props.buildInfo;
    //let uploadUrl = `${WebApiConfig.attach.uploadUrl}${id}`;
    let id = 900;
    let uploadUrl = `${WebApiConfig.attach.uploadUrl}`;//上载地址需要知道
    let fileGuid = file.uid;//NewGuid();//uuid.v4();
    let fd = new FormData();
    fd.append("fileGuid", fileGuid)
    fd.append("name", file.name)
    fd.append("file", file);

    let _this = this;
    var xhr = new XMLHttpRequest();
    xhr.open('POST', uploadUrl, true);
    xhr.send(fd);
    xhr.onload = function (e) {
      if (this.status === 200) {
        let r = JSON.parse(this.response);
        // console.log("返回结果：", this.response);
        if (r.code === "0") {
          let uf = {
            fileGuid: fileGuid,
            from: 'pc-upload',
            WXPath: r.extension,
            sourceId: id,
            appId: 'houseSource',
            localUrl: file.url,
            name: file.name
          }
          // console.log("pic值", uf);
          if (callback) {
            callback(uf);
          }
        } else {
          notification.error({
            message: '上传失败：',
            duration: 3
          });
        }
      } else {
        notification.error({
          message: '图片上传失败!',
          duration: 3
        });
      }
    }
    xhr.onerror = function (e) {
      notification.error({
        message: '图片上传失败!',
        duration: 3
      });
    }
    xhr.onabort = function () {
      notification.error({
        message: '图片上传异常终止!',
        duration: 3
      });
    }
  }
  handleSubmit = () =>{

  }
  render(){

    const {previewVisible, previewImage, fileList} = this.state;
    const uploadButton = (
      <div>
        <Icon type="plus" />
        <div className="ant-upload-text">添加图片</div>
      </div>
    );
    return (
        
        <Layout>
            <Content className='' style={{padding:'25px 0', margin:'20px 0', backgroundColor:"#ECECEC"}}>
                {
                    <div>
                        <Icon type='tags-o' className = 'content-icon'>
                        </Icon>
                        <span className='content-title'>附加信息</span>
                    </div>
                }
                <Row type='flex' justify='space-between'>
                  <Col span={24}>
                      <div className='picture'>图片</div>
                      <div className='picBox'>
                          <Tabs defaultActiveKey='1'>
                            {
                              this.props.basicData.contractAttachTypes.map((item, i)=>{
                                return (
                                  <TabPane tab={item.key} key={item.value}>
                                    <div className='clearfix'>
                                      <Upload 
                                        beforeUpload={this.handleBeforeUpload}
                                        listType='picture-card'
                                        fileList = {fileList}
                                        onPreview={this.handlePreview}
                                        onChange={this.handleChange}
                                      >
                                        {uploadButton}
                                      </Upload>
                                      <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                                        <img alt="example" style={{ width: '100%' }} src={previewImage} />
                                      </Modal>
                                    </div>
                                  </TabPane>
                                )
                              })
                            }
                          </Tabs>
                      </div>
  
                    </Col>
                </Row>
                <Row type="flex" justify="center" className='BtnTop'>
                  {
                    <div>
                      <Button type="primary" size='default'
                        style={{ width: "8rem" }}
                        onClick={this.handleSubmit}>提交
                      </Button>
                    </div>
                  }
                </Row>
            </Content>
        </Layout>
    );
  }
}

function mapStateToProps(state) {
  // console.log("attachedit props：", state.buildInfo)
  return {
    basicData: state.basicData,
    /*
    buildingOperInfo: state.building.operInfo,
    buildInfo: state.building.buildInfo,
    attachInfo: state.building.buildInfo.attachInfo,
    completeFileList: state.shop.completeFileList,
    deletePicList: state.shop.deletePicList,
    basicData: state.basicData,
    loadingState: state.building.attachloading,
    user: (state.oidc.user || {}).profile || {},
    */
  }
}

function mapDispatchToProps(dispatch) {
  return {
    //save: (...args) => dispatch(savePictureAsync(...args)),
    dispatch
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(AttachEdit));