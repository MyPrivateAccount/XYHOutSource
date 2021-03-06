import { connect } from 'react-redux';
import { savePictureAsync, contractPicView,
  deletePicAsync, saveCompleteFileList, saveDeletePicList,attchLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import './editCommon.less'
import {
  Layout, Button, message, Icon, Row, Col, Form, Input,
  Radio, Select, Upload, Modal, notification, Tabs,BackTop
} from 'antd'
import { NewGuid } from '../../../../utils/appUtils';
import WebApiConfig from '../../../constants/webapiConfig'
import myAuditListPage from '../../../../auditCenter/pages/myAuditListPage';


const { Header, Sider, Content } = Layout;
const Option = Select.Option;
const FormItem = Form.Item;
const TabPane = Tabs.TabPane;

class AttachEdit extends Component {
  state = {
    previewVisible: false,
    previewImage: '',
    fileList: [],
    attachments: [], //附件
    picUploading: false,
    // completeFileList: [],
    // deletePicList: [],
    deleteIdArr: [],
    midifyFileArr:[],
    picGroup: '1', // 图片分类值
    imgFiles: {}
  }

  componentWillMount(){
    if(this.props.basicData.contractAttachTypes.length === 0){
      //this.props.dispatch(getDicParList(['CONTRACT_ATTACHMENT_CATEGORIES']));
    }
  }

  componentDidMount() {
    let fileList = [];

    if (this.props.attachInfo.fileList) {
          let curFileList = this.props.attachInfo.fileList;//直接修改会改变state

          this.getGroup(curFileList);
    }
    this.setState({ fileList: fileList });
    
  }
  componentWillReceiveProps(newProps) {
   // console.log("newProps:", newProps);
   // console.log("this.state.deleteIdArr:", this.state.deleteIdArr);
    let fileList = [];
    try {
      if (newProps.attachInfo.fileList) {
        let curFileList = newProps.attachInfo.fileList;
        let temp = [];
        if(!this.initFiles){
          if(this.state.deleteIdArr.length > 0)
          {
            this.state.deleteIdArr.forEach((item, index)=>{
              
              curFileList.forEach((_item, index) =>{
                  if(_item.fileGuid !== item){
                    temp.push(_item);
                  }
              })
                //console.log('需要去掉的uid:', item);

            });
          }
          this.getGroup(temp)
        }
        this.initFiles=true;   // true
      }
    } catch (e) {
      console.log("e:", e);
    }
    //  if (newProps.attachInfo.fileList) {
    //     if(!this.initFiles){
    //       this.getGroup(newProps.attachInfo.fileList)
    //     }
    //     this.initFiles=true;   // true
    // }
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
      //console.log(this.state.imgFiles, list, 'hahahahh') // list 是reducer的，imgFiles 是才传的
      let myObj = Object.assign({}, this.state.imgFiles)
      if (Object.keys(myObj).length !== 0){
        for(let i in list) {
          if (list[i]){
            let temp = [];
            let myTemp = myObj[i] || [];
            list[i].forEach((item, index)=>{//去重操作保证传入的列表key唯一
                let isSame = false;
                for(let j =0;j < myTemp.length; j ++){
                  if(item.uid === myTemp[j].uid){
                    isSame = true;
                  }
                }
                if(!isSame){
                  temp.push(item);
                }
             }
          
            );
            myObj[i] = myObj[i].concat(temp);
          
            
          }
        }
        this.setState({
          imgFiles: myObj
          })
      } else {
        this.setState({
            imgFiles: list
        })
      }
    }
  }


  handleCancel = () => this.setState({ previewVisible: false })

  hanldeRemove = (file) => {
    // console.log(file, '删除图片')
    let uid = file.uid, index;
    let { fileList, deleteIdArr, imgFiles } = this.state;
    let { deletePicList } = this.props
    deleteIdArr.push(file.uid);
    deletePicList.push(file)
    // console.log(deleteIdArr, deletePicList, '删除的数据')


      let arr = imgFiles[this.state.picGroup] || []
      // console.log(imgFiles, arr, '删除图片组的这个是哪一个')
      arr.forEach((v, i) => {
        if (v.uid === uid) {
          index = i
        }
      })
      // console.log(index, '9')
      arr.splice(index, 1)
      // console.log(arr, 'arrArr')
      imgFiles[this.state.picGroup] = arr.slice()
      this.props.dispatch(saveDeletePicList({ deletePicList: deletePicList }))
      this.setState({ deleteIdArr: deleteIdArr, imgFiles: imgFiles }, () => {
        // console.log(this.state.imgFiles, '删除后的')
      })
    
    
    
    return true;
  }
  handleCancelBtn = () => {
      this.props.dispatch(contractPicView({ filelist: this.props.attachInfo.fileList, type: 'cancel' }))
  }
 
  handleBeforeUpload = (uploadFile) => {
    let reader = new FileReader();
    reader.readAsDataURL(uploadFile);

    reader.onloadend = function () {
      let nowImgFiles = this.state.imgFiles[this.state.picGroup] || []
      console.log("nowImgFiles:", nowImgFiles);
      let projectArr = nowImgFiles.concat([{
        uid: uploadFile.uid,
        name: uploadFile.name,
        status: 'uploading',
        url: reader.result
      }])
      console.log('projectArr:', projectArr);
      let imgFiles =  Object.assign({}, this.state.imgFiles)
      imgFiles[this.state.picGroup] = projectArr
      this.setState({ imgFiles: imgFiles });
        
    }.bind(this);

    if (!uploadFile.type.startsWith("image/")) {
      notification.error({
        message: '只能上传图片！',
        duration: 3
      });
      return false;
    }

    this.UploadFile(uploadFile, (ufile) => {
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
      console.log(completeFileList, '7')
      this.props.dispatch(saveCompleteFileList({ completeFileList: completeFileList }))
    });

    return false;
  }


  //图片保存
  handPictureSave = () => {
    this.props.dispatch(attchLoadingStart())
    const { deleteIdArr } = this.state;
    const { completeFileList, deletePicList } = this.props;
    let modifyArr = this.state.midifyFileArr;
    
    for(let i = 0; i < deletePicList.length >0; i ++){
      modifyArr = modifyArr.filter(item => item.fileGuid === deletePicList[i]);
    }
    modifyArr.map(item =>{
      completeFileList.map(file => {
        if(file.fileGuid === item.fileGuid){
          file = item;
        }
      })
    })
    let newModifyArr =  modifyArr.filter(item =>{
      if((completeFileList.find(file => file.fileGuid === item.fileGuid)) === undefined){
        return true;
      }
      return false;
    })
    
    console.log(completeFileList, deletePicList,newModifyArr, '???s提交图片？？？？？？？？')
    let id = this.props.basicInfo.id;
     if(completeFileList.length !== 0 || deletePicList.length !== 0 || newModifyArr.length !== 0){
       this.props.save({
        fileInfo: {addFileList:completeFileList || [], deleteFileList:deletePicList || [], modifyFileList: newModifyArr || []},
        completeFileList: completeFileList,
        id: id,
    //     //type: this.props.type, // shops  building  updataRecord
       });
     }

  }

  UploadFile = (file, callback) => {
    // console.log(file);
    let id = this.props.basicInfo.id;
    let uploadUrl = `${WebApiConfig.attach.uploadUrl}${id}`;
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
         console.log("返回结果：", this.response);
        if (r.code === "0") {
          let uf = {
            fileGuid: fileGuid,
            from: 'pc-upload',
            WXPath: r.extension,
            sourceId: id,
            appId: 'contractManagement',
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


  handlePreview = (file) => {
    this.setState({
        previewImage: file.url || file.localUrl,
        previewVisible: true,
    });
  }

  changeTabKey = (key) => {
    // console.log(key, 'key')
    this.setState({picGroup: key})
  }
  onDescChange = (e, item) =>{
    item.ext1  = e.target.value;
    if(this.props.attachInfo.fileList){
      let fileList = this.props.attachInfo.fileList;

     
      let modifyArr =this.state.midifyFileArr;
      let isFind = false;
      modifyArr.map((file) => {
          if(file.fileGuid === item.uid){
            file.ext1 = e.target.value;
            isFind = true;
          }
        })
      if(!isFind){
        let temp = this.props.completeFileList.find(file => file.fileGuid === item.uid);
        if(temp !== undefined){
          modifyArr =  [...this.state.midifyFileArr, temp];
        }
        else{
          let curFile = fileList.find(file => file.fileGuid === item.uid);
          if(curFile !== undefined){
            modifyArr =  [...this.state.midifyFileArr, curFile];
          }
        }
      }

      
      this.setState({midifyFileArr: modifyArr});
    }
  }
  render() {
    let attachPicOperType = this.props.operInfo.attachPicOperType;
    let { previewVisible, previewImage, fileList } = this.state;

    const uploadButton = (
      <div>
        <Icon type="plus" />
        <div className="ant-upload-text">添加图片</div>
      </div>
    );
 
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

    console.log( "imgFiles图片:", this.state.imgFiles);
  

    return (
      <div className="">
        <Layout>
          <Content className='' style={{ padding: '25px 0', margin: '20px 0', backgroundColor: "#ECECEC" }}>
            {
                <div>
                  <Icon type="tags-o" className='content-icon' /> <span className='content-title'>附加信息</span>
                </div>
            }
            <Row type="flex" justify="space-between">
              <Col span={24}>
                {
                    <div className='picture'>图片</div>
                }
                <div className='picBox'>

                  {
                    <Tabs defaultActiveKey="1" onChange={this.changeTabKey}>
                        {
                          this.props.basicData.contractAttachTypes.map((item, i) => {
                                return (
                                  <TabPane tab={item.key} key={item.value} >
					
                                      <div className='picBox'>
                                        <div className="clearfix">
                                        {
                                              this.state.imgFiles[item.value] ?  (this.state.imgFiles[item.value] || []).map((fileItem, index) =>{
                                              let arr = [];
                                              arr.push(fileItem);
                                              return(            
                                                <Row type='flex' align="bottom" key={fileItem.uid}>        
                                                  <Col span={4}>
                                                    <Upload  listType="picture-card"
                                                      fileList= {arr}
                                                      onPreview={this.handlePreview} 
                                                      beforeUpload={this.handleBeforeUpload} 
                                                      onRemove={this.hanldeRemove} >
                                                    
                                                    </Upload>
                                                  </Col>
                                                  <Col span={12}>
                                                 
                                                    <span>{"附件备注:"}</span>
                                                    <Input type="textarea" placeholder="备注信息" size='default'  style={{marginBottom: '10px'}} defaultValue={fileItem.ext1} onChange={(e) =>this.onDescChange(e, fileItem) } maxLength="200" autosize={{ minRows: 2, maxRows: 4 }}/>
                                                  </Col>
                                                </Row>        
                                                )
                                            }) 
                                            : null
                                        }
                                            
                                            <br/>
                                            <Upload  listType="picture-card"
                                                  fileList= {[]}
                                                  onPreview={this.handlePreview} 
                                                  beforeUpload={this.handleBeforeUpload} 
                                                  onRemove={this.hanldeRemove}>
                                                   {uploadButton}
                                                </Upload>
                                           
                                            <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                                                <img alt="example" style={{ width: '100%' }} src={previewImage} />
                                            </Modal>
                                        </div>
                                    </div>
                                </TabPane>  
                                )
                            })
                        }
                    </Tabs> 
                  }
                </div>
              </Col>
            </Row>
            {/*
                this.props.isDisabled}
                          onClick={this.handPictureSave}>保存
                        </Button>
                        {
                          attachPicOperType !== 'add' ?
                            <Button size='default'
                              className='oprationBtn'
                              onClick={this.handleCancelBtn}>取消
                           </Button> : null
                        }
                      </div>
                  }

                </Row>
            */}
              <Row type="flex" justify="space-between" className='BtnTop'>
                    {
                      (this.props.isCurShowContractDetail  || [ 1].includes(this.props.attachInfo.examineStatus))  ? null :
                      <Col  span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button type="primary" size='default'
                          style={{ width: "8rem" }}
                          onClick={this.handPictureSave}>提交
                        </Button>
                      </Col>
                    }
              </Row>
          </Content>
          <div>
              <BackTop visibilityHeight={400} />
          </div>
          {/*
            <Row type="flex" justify="space-between" className='BtnTop'>
                    {
                      (this.props.isCurShowContractDetail  || [8, 1].includes(this.props.attachInfo.examineStatus))  ? null :
                      <Col  span={24} style={{ textAlign: 'right' }} className='BtnTop'>
                        <Button type="primary" size='default'
                          style={{ width: "8rem" }}
                          onClick={this.handPictureSave}>提交
                        </Button>
                      </Col>
                    }
              </Row>
                  */}
        </Layout>
      </div>
    )
  }
}

function mapStateToProps(state) {
 
  return {
    isDisabled: state.contractData.isDisabled,
    basicData: state.basicData,
    attachInfo: state.contractData.contractInfo.attachInfo,
    basicInfo:state.contractData.contractInfo.baseInfo,
    
    operInfo: state.contractData.operInfo,
    completeFileList: state.contractData.completeFileList,
    deletePicList: state.contractData.deletePicList,
    isCurShowContractDetail: state.contractData.isCurShowContractDetail,

  }
}

function mapDispatchToProps(dispatch) {
  return {
    save: (...args) => dispatch(savePictureAsync(...args)),
    dispatch
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(AttachEdit));