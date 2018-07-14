import React, { Component } from 'react'
import { Spin, Modal, Upload, Icon, notification } from 'antd'
import WebApiConfig from '../constants/webApiConfig'
import uuid from 'uuid'

const confirm = Modal.confirm;

class UploadPanel extends Component {

    state={
        uploading: false,
        files:[],
        previewVisible: false
    }


    uploadCallback = (data, file) => {
        let files = this.state.files;
        let idx = this.state.files.find(x=> x.uid === file.uid);
        if(idx>=0){
            files[idx]=file;
        }else{
            files.push(file);
        }
        if( this.props.uploadCallback ){
            this.props.uploadCallback(data,file);
        }
        this.setState({files: [...files]})
      
    }

    upload = (callback, file, fileList) => {
        
        let id = this.props.entityId;
        if(!id){
            notification.error({message:'无法上传，没有指定id'})
            return;
        }

        let data = this.props.data;

        let uploadUrl = `${WebApiConfig.attach.uploadUrl}${id}`;
        let fileGuid = uuid.v1();
        let fd = new FormData();
        fd.append("fileGuid", fileGuid)
        fd.append("name", file.name)
        fd.append("file", file);
        this.setState({uploading: true})
        var xhr = new XMLHttpRequest();
        xhr.open('POST', uploadUrl, true);
        xhr.send(fd);
        let _this= this;
        xhr.onload = function (e) {
            if (this.status === 200) {
                let r = JSON.parse(this.response);
                console.log("返回结果：", this.response);
                _this.setState({uploading: false})
                if (r.code === "0") {
                    let uf = {
                        fileGuid: fileGuid,
                        from: 'pc-upload',
                        WXPath: r.extension,
                        sourceId: id,
                        appId: 'ExpenseManagerIndex',
                        localUrl: file.url,
                        url: `${WebApiConfig.attach.browseUrl}${r.url}`,
                        driver: r.deviceName,
                        uri: r.path,
                        type: 'ORIGINAL',
                        name: file.name,
                        uid: file.uid,
                        fileExt: r.ext
                    }
                    if (callback) {
                        callback(data, uf);
                    }
                } else {
                    notification.error({
                        message: '上传失败：',
                        duration: 3
                    });
                    _this.setState({uploading: false})
                }
            } else {
                notification.error({
                    message: '图片上传失败!',
                    duration: 3
                });
                _this.setState({uploading: false})
            }
        }
        xhr.onerror = function (e) {
            _this.setState({uploading: false})
            notification.error({
                message: '图片上传失败!',
                duration: 3
            });
        }
        xhr.onabort = function () {
            _this.setState({uploading: false})
            notification.error({
                message: '图片上传异常终止!',
                duration: 3
            });
        }
    }
    handleCancel = () => this.setState({ previewVisible: false })

    handlePreview = (file) => {
        this.setState({
            previewImage: file.url || file.thumbUrl,
            previewVisible: true,
        });
    }

    handleUploadChange = (file) => {
        let canEdit = this.props.canEdit;
        if(!canEdit){
            return;
        }
        console.log(file);
        let files = this.state.files;
        let idx =  files.findIndex(x=>x.fileGuid === file.fileGuid || x.uid === file.uid);
        if(idx>=0){
            confirm({
                title: `您确定要删除此附件么?`,
                content: '删除后不可恢复',
                onOk: () => {
                    files.splice(idx,1);
                    this.setState({reportFiles:[...files]})
                },
                onCancel() {

                },
            });

            
        }
     
    }

    componentDidMount=()=>{
        this.initFiles(this.props);
    }

    componentWillReceiveProps = (nextProps)=>{
        if(this.props.files !== nextProps.files ){
            this.initFiles(nextProps);
        }
    }

    initFiles = (props)=>{
        let files = props.files || [];
        this.setState({files: files, canEdit: props.canEdit})
    }

    getFiles = ()=>{
        let fs = this.state.files||[];
        return [...fs]
    }


    render() {
        const {uploading, files} = this.state;
        const {canEdit} = this.props;
        let accept = this.props.accept || '.jpg,.jpeg,.png,.bmp';
        return (
            <Spin spinning={uploading}>
                <div className="clearfix" style={{ margin: 10 }}>
                    <Upload
                        accept={accept}
                        listType="picture-card"
                        fileList={files}
                        disabled={!canEdit}

                        onPreview={this.handlePreview}
                        onRemove={(...args) => this.handleUploadChange(...args)}
                        beforeUpload={(...args) => this.upload(this.uploadCallback, ...args)}
                    >{
                            canEdit ? <div>
                                <Icon type="plus" />
                                <div className="ant-upload-text">上传</div>
                            </div> : null
                        }

                    </Upload>
                </div>

                <Modal visible={this.state.previewVisible} footer={null} onCancel={this.handleCancel}>
                    <img alt="图片" style={{ width: '100%' }} src={this.state.previewImage} />
                </Modal>
            </Spin>
        )
    }
}


export default UploadPanel;