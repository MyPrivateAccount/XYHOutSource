//附件组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { notification, Upload, Menu, Icon, Modal, Spin, Layout,Form } from 'antd'
import Avatar from './tradeUpload'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import { dicKeys, permission } from '../../../constants/const'
import { getDicPars, getOrganizationTree } from '../../../../utils/utils'
import uuid from 'uuid'

const { Header, Sider, Content } = Layout;

const confirm = Modal.confirm;

class TradeAttact extends Component {
    state = {
        currentMenu:null,
        reportFiles:[],
        menuList:[],
        uploading: false
    }

    componentDidMount=()=>{
        let dl  = getDicPars(dicKeys.yjfj, this.props.dic);
        this.setState({currentMenu: dl[0], menuList: dl})
        this.initEntity(this.props.report, this.props.entity||[]);

    }
    componentWillReceiveProps=(nextProps)=>{
        if(this.props.dic !== nextProps.dic && nextProps.dic){
            let dl  = getDicPars(dicKeys.yjfj, nextProps.dic);
            this.setState({currentMenu: dl[0], menuList: dl})
        }
        if(this.props.entity!== nextProps.entity && nextProps.entity){
            this.initEntity(nextProps.report, nextProps.entity||[]);
        }
    }

    handleMenuClick = (e) => {
        let m = this.state.menuList.find(x=>x.value === e.key);
        this.setState({currentMenu: m},()=>{

        })
    }

    uploadCallback = (menu, file) => {
        file.group = menu.value;
        let reportFiles = this.state.reportFiles;
        let idx = this.state.reportFiles.find(x=> x.uid === file.uid);
        if(idx>=0){
            reportFiles[idx]=file;
        }else{
            reportFiles.push(file);
        }
        this.setState({reportFiles: [...reportFiles]})
      
    }
    upload = (menu, callback, file, fileList) => {
        let report  = this.props.report;
        if(!report || !report.id){
            return;
        }
        let id = report.id;
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
                        callback(menu, uf);
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
        let canEdit = this.props.opType=='add' || this.props.opType=='edit';
        if(!canEdit){
            return;
        }
        console.log(file);
        let reportFiles = this.state.reportFiles;
        let idx =  reportFiles.findIndex(x=>x.fileGuid === file.fileGuid || x.uid === file.uid);
        if(idx>=0){
            confirm({
                title: `您确定要删除此附件么?`,
                content: '删除后不可恢复',
                onOk: () => {
                    reportFiles.splice(idx,1);
                    this.setState({reportFiles:[...reportFiles]})
                },
                onCancel() {

                },
            });

            
        }
     
    }


    initEntity = (report, scopes)=>{
        let files =  [];
        scopes.forEach(item=>{
            if(item.fileItem && item.fileList && item.fileList.length>0){
                let f = item.fileList[0];
                f.url = item.fileItem.original;
                f.uid = f.fileGuid;
                f.group = item.group;
                files.push(f);
            }
        })
        this.setState({reportFiles: files})

    }

    getValues = ()=>{
        let files = this.state.reportFiles;
        let report= this.props.report;
        let scopes = [];
        files.forEach(item=>{
            let si = {id: report.id, fileGuid: item.fileGuid, group: item.group}
            si.fileList = [{...item}]
            si.fileItem = {original: item.url}
            scopes.push(si)
        })
        return scopes;
    }

    
    render() {
        const {menuList, currentMenu, reportFiles, uploading}  = this.state;
        let fl = [];
        if(currentMenu){
            fl =  reportFiles.filter(item=> item.group === currentMenu.value)
        }
        const canEdit = this.props.canEdit;
        return (
            <Layout>
                <Sider>
                    <Menu mode="inline"
                        onClick={this.handleMenuClick}
                        selectedKeys={[(currentMenu||{}).value]}
                        >
                        {menuList.map((menu, i) =>
                            <Menu.Item key={menu.value}>
                                <span>{menu.key}</span>
                            </Menu.Item>
                        )}
                    </Menu>
                </Sider>
                <Spin spinning={uploading}>
                    {/* <Header>
                        <Input style={{ width: 300 }}></Input>
                        <Button type="primary">选择图片</Button>
                    </Header> */}
                    <Content>
                        <div className="clearfix" style={{ margin: 10 }}>
                        <Upload
                        accept=".jpg,.jpeg,.png,.bmp"
                        listType="picture-card"
                        fileList={fl}
                        disabled={!canEdit}
                        
                        onPreview={this.handlePreview}
                        onRemove={(...args) => this.handleUploadChange(...args)}
                        beforeUpload={(...args) => this.upload(currentMenu, this.uploadCallback, ...args)}
                    >{
                        canEdit ? <div>
                                <Icon type="plus" />
                                <div className="ant-upload-text">上传</div>
                            </div> : null
                        }

                    </Upload>
                            {/* <Avatar id={this.state.activeMenuID} fileList={this.getFileList} append={this.appendData}/> */}
                        </div>
                    </Content>
                    <Modal visible={this.state.previewVisible} footer={null} onCancel={this.handleCancel}>
                        <img alt="图片" style={{ width: '100%' }} src={this.state.previewImage} />
                    </Modal>
                </Spin>
            </Layout>
        )
    }
}

const WrappedRegistrationForm = Form.create()(TradeAttact);
export default WrappedRegistrationForm