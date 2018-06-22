import { connect } from 'react-redux';
import {
    savePictureAsync, contractPicView,
    deletePicAsync, saveCompleteFileList, saveDeletePicList, attchLoadingStart
} from '../../../actions/actionCreator';
import React, { Component } from 'react'
import './editCommon.less'
import {
    Layout, Button, message, Icon, Row, Col, Form, Input,
    Radio, Select, Upload, Modal, notification, Tabs, BackTop
} from 'antd'
import { NewGuid } from '../../../../utils/appUtils';
import WebApiConfig from '../../../constants/webapiConfig'
import myAuditListPage from '../../../../auditCenter/pages/myAuditListPage';

const TabPane = Tabs.TabPane;

class AttachEdit extends Component {
    state = {
        previewVisible: false,
        previewImage: '',
        // fileList: [],
        picUploading: false,
        deleteIdArr: [],
        picGroup: '1', // 当前图片分类值
        imgFiles: {}
    }

    componentDidMount() {
        // let fileList = [];
        // if (this.props.attachInfo && this.props.attachInfo.fileList) {
        //     let curFileList = this.props.attachInfo.fileList;//直接修改会改变state

        //     this.getGroup(curFileList);
        // }
        // this.setState({ fileList: fileList });
    }

    hanldeRemove = (file) => {
        let uid = file.uid;
        let { deleteIdArr, imgFiles } = this.state;
        let { deletePicList } = this.props
        deleteIdArr.push(file.uid);
        deletePicList.push(file)
        let arr = imgFiles[this.state.picGroup] || []
        let index = arr.find(item => item.uid == uid);
        arr.splice(index, 1)
        imgFiles[this.state.picGroup] = arr.slice()
        // this.props.dispatch(saveDeletePicList({ deletePicList: deletePicList }))
        this.setState({ deleteIdArr: deleteIdArr, imgFiles: imgFiles }, () => {
            if (this.props.onAttachChange) {
                this.props.onAttachChange({ deleteIdArr: deleteIdArr, imgFiles: imgFiles });
            }
        })
        return true;
    }

    handleBeforeUpload = (uploadFile) => {
        if (!uploadFile.type.startsWith("image/")) {
            notification.error({
                message: '只能上传图片！',
                duration: 3
            });
            return false;
        }
        let reader = new FileReader();
        reader.readAsDataURL(uploadFile);
        reader.onloadend = function () {
            let nowImgFiles = this.state.imgFiles[this.state.picGroup] || []
            console.log("uploadFile:::", uploadFile);
            let projectArr = nowImgFiles.concat([{
                uid: uploadFile.uid,
                name: uploadFile.name,
                status: 'uploading',
                url: reader.result
            }])
            console.log('projectArr:', projectArr, this.state.picGroup);
            let imgFiles = this.state.imgFiles
            imgFiles[this.state.picGroup] = projectArr
            this.setState({ imgFiles: imgFiles });
        }.bind(this);

        this.UploadFile(uploadFile, (ufile) => {
            console.log("回调信息：", ufile);
            let imgFiles = this.state.imgFiles;
            imgFiles[this.state.picGroup].map(file => {
                if (file.uid === ufile.fileGuid) {
                    file.status = 'done';
                    ufile.localUrl = file.url;
                    ufile.group = this.state.picGroup
                }
            });
            this.setState({ imgFiles: imgFiles }, () => {
                // console.log(this.state.imgFiles, '上传之后的回调信息：')
                if (this.props.onAttachChange) {
                    this.props.onAttachChange({ deleteIdArr: this.state.deleteIdArr, imgFiles: this.state.imgFiles });
                }
            });
        });
        return false;
    }

    UploadFile = (file, callback) => {
        let id = this.props.basicInfo.id;
        let uploadUrl = `${WebApiConfig.attach.uploadUrl}${id}`;
        let fileGuid = file.uid;
        let fd = new FormData();
        fd.append("fileGuid", fileGuid)
        fd.append("name", file.name)
        fd.append("file", file);

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
                        message: '上传失败',
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

    onDescChange = (e, item) => {
        // console.log("修改描述:", item, this.state.imgFiles[this.state.picGroup]);
        item.ext1 = e.target.value;
        if (this.props.onAttachChange) {
            this.props.onAttachChange({ deleteIdArr: this.state.deleteIdArr, imgFiles: this.state.imgFiles });
        }
    }
    render() {
        let { previewVisible, previewImage } = this.state;

        console.log("imgFiles图片:", this.state.imgFiles);
        let contractAttachTypes = this.props.basicData.contractAttachTypes || [];

        return (
            <div className='' style={{ padding: '25px 0', margin: '20px 0', backgroundColor: "#ECECEC" }}>
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
                                <Tabs onChange={(key) => this.setState({ picGroup: key })}>
                                    {
                                        contractAttachTypes.map((item, i) => {
                                            return (
                                                <TabPane tab={item.key} key={item.value} >
                                                    <div className='picBox'>
                                                        <div className="clearfix">
                                                            {
                                                                this.state.imgFiles[item.value] ? (this.state.imgFiles[item.value] || []).map((fileItem, index) => {
                                                                    return (
                                                                        <Row type='flex' align="bottom" key={fileItem.uid}>
                                                                            <Col span={4}>
                                                                                <Upload listType="picture-card"
                                                                                    fileList={[fileItem]}
                                                                                    onPreview={this.handlePreview}
                                                                                    beforeUpload={this.handleBeforeUpload}
                                                                                    onRemove={this.hanldeRemove} >
                                                                                </Upload>
                                                                            </Col>
                                                                            <Col span={12}>
                                                                                <span>{"附件备注:"}</span>
                                                                                <Input type="textarea" placeholder="备注信息" size='default' style={{ marginBottom: '10px' }} defaultValue={fileItem.ext1} onChange={(e) => this.onDescChange(e, fileItem)} maxLength="200" autosize={{ minRows: 2, maxRows: 4 }} />
                                                                            </Col>
                                                                        </Row>
                                                                    )
                                                                })
                                                                    : null
                                                            }
                                                            <Upload listType="picture-card"
                                                                fileList={[]}
                                                                beforeUpload={this.handleBeforeUpload}>
                                                                <div>
                                                                    <Icon type="plus" />
                                                                    <div className="ant-upload-text">添加图片</div>
                                                                </div>
                                                            </Upload>
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
                {/*单个图片预览*/}
                <Modal visible={previewVisible} footer={null} onCancel={() => this.setState({ previewVisible: false })}>
                    <img alt="example" style={{ width: '100%' }} src={previewImage} />
                </Modal>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        basicData: state.basicData,
        basicInfo: state.contractData.contractInfo.baseInfo,
        completeFileList: state.contractData.completeFileList,
        deletePicList: state.contractData.deletePicList,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        save: (...args) => dispatch(savePictureAsync(...args)),
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(AttachEdit));