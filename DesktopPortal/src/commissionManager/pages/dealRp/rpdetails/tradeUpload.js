import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Upload, Icon, message, Modal, notification } from 'antd';
import { NewGuid } from '../../../../utils/appUtils';
import WebApiConfig from '../../../constants/webApiConfig';
import { uploadFile } from '../../../actions/actionCreator'

class Avatar extends React.Component {

  state = {
    id: '',
    previewVisible: false,
    previewImage: '',
    fileList: [{
      uid: -1,
      name: 'xxx.png',
      status: 'done',
      url: 'https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png',
    }],
  };
  componentDidMount() {
    if (this.props.fileList!==null&&this.props.fileList.length !== 0) {
      this.setState({ fileList: this.props.fileList })
    }
  }
  componentWillReceiveProps(newProps) {
    if (this.state.id !== newProps.id) {
      this.setState({ id: newProps.id })
      let fl = this.props.fileList();
      this.setState({ fileList: fl })
    }
    if (newProps.operInfo.operType === 'DEALRP_ATTACT_UPLOAD_COMPLETE') {//上传成功
      newProps.operInfo.operType = ''
    }
  }
  handleCancel = () => this.setState({ previewVisible: false })

  handlePreview = (file) => {
    this.setState({
      previewImage: file.url || file.thumbUrl,
      previewVisible: true,
    });
  }

  handleChange = ({ fileList }) => this.setState({ fileList })

  handleBeforeUpload = (pf) => {
    this.UploadFile(pf, (ufile, id, preview) => {
      this.props.dispatch(uploadFile(ufile));
      ufile.type = id
      ufile.uri = 'https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png'
      this.props.append(ufile)
    });
    return true;
  }
  UploadFile = (file, callback) => {
    let id = this.props.id
    let uploadUrl = `${WebApiConfig.attach.uploadUrl}${id}`;
    let fileGuid = NewGuid();
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
            appId: 'commissionManager',
            localUrl: file.url,
            name: file.name
          }
          if (callback) {
            callback(uf, id, file.thumbUrl);
          }
        } else {
          notification.error({
            message: '上传失败：',
            duration: 3
          });
        }
      } else {
        notification.error({
          message: '文件上传失败!',
          duration: 3
        });
      }
    }
    xhr.onerror = function (e) {
      notification.error({
        message: '文件上传失败!',
        duration: 3
      });
    }
    xhr.onabort = function () {
      notification.error({
        message: '文件上传异常终止!',
        duration: 3
      });
    }
  }


  render() {
    const { previewVisible, previewImage, fileList } = this.state;
    const uploadButton = (
      <div>
        <Icon type={this.state.loading ? 'loading' : 'plus'} />
        <div className="ant-upload-text">Upload</div>
      </div>
    );
    return (
      <div className="clearfix">
        <Upload
          action="//jsonplaceholder.typicode.com/posts/"
          listType="picture-card"
          fileList={fileList}
          onPreview={this.handlePreview}
          onChange={this.handleChange}
          beforeUpload={this.handleBeforeUpload}
        >
          {uploadButton}
        </Upload>
        <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
          <img alt="example" style={{ width: '100%' }} src={previewImage} />
        </Modal>
      </div>
    );
  }
}
function tableMapStateToProps(state) {
  return {
    operInfo:state.rp.operInfo,
  }
}

function tableMapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Avatar);