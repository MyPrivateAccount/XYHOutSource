import {connect} from 'react-redux';
import {getActiveDetail, setLoadingVisible} from '../actions/actionCreator';
import {getDicParList} from '../../actions/actionCreators'
import React, {Component} from 'react';
import {Row, Col, Tabs, Modal, Upload} from 'antd';
import AuditForm from './item/auditForm';
import AuditHistory from './auditHistory';
import moment from 'moment'

const styles = {
    rowLabel: {
        // fontWeight:'bold'
    }
}
const TabPane = Tabs.TabPane;
class AuditPicture extends Component {
    state = {
        review: null,
        content: null,
        record: null,
        buildingImgList: {},
        shopImgList: [],
        previewVisible: '',
        previewImage: '',
        photoCategories: []
    }
    componentWillMount() {
        this.props.dispatch(getDicParList(['PHOTO_CATEGORIES']));
    }
    componentWillReceiveProps(newProps) {
        const isBuildingPic = (this.props.picType === "building");
        const isShopPic = (this.props.picType === "shop");
        const curActiveInfo = newProps.curActiveInfo || {};
        //字典
        if (isBuildingPic && this.state.photoCategories.length == 0) {
            let photoCategories = [];
            if (this.props.rootBasicData.dicList) {
                let categories = this.props.rootBasicData.dicList.find(dic => dic.groupId == 'PHOTO_CATEGORIES');
                if (categories) {
                    photoCategories = categories.dicPars;
                }
            }
            this.setState({photoCategories: photoCategories});
        }
        if (curActiveInfo.updateContent) {
            let content = curActiveInfo.updateContent;
            try {
                content = JSON.parse(curActiveInfo.updateContent);
            } catch (e) {}
            if (Array.isArray(content)) {
                if (isBuildingPic) {
                    let imgFiles = {};
                    content.map(img => {
                        if (imgFiles[img.Group]) {
                            if (img.FileInfo) {
                                imgFiles[img.Group].push({uid: img.FileGuid, url: img.FileInfo.Icon, status: img.UpdateType == 'Add' ? 'done' : 'error'});
                            }
                        } else {
                            imgFiles[img.Group] = [{uid: img.FileGuid, url: img.FileInfo.Icon, status: img.UpdateType == 'Add' ? 'done' : 'error'}];
                        }
                    });
                    this.setState({buildingImgList: imgFiles});
                } else if (isShopPic) {
                    let files = [];
                    content.map(img => {
                        files.push({uid: img.FileGuid, url: img.FileInfo.Icon, status: img.UpdateType == 'Add' ? 'done' : 'error'});
                    });
                    this.setState({shopImgList: files});
                }
            }
        }
    }

    componentDidMount = () => {
        // const curActiveInfo = this.props.curActiveInfo;
        // if (curActiveInfo.updateContent === undefined) {
        //     this.props.dispatch(setLoadingVisible(true));
        //     this.props.dispatch(getActiveDetail(activeAuditInfo.submitDefineId));
        // }

    }
    getDicName = (dicList, group, value) => {
        let dicGroup = dicList.find(x => x.groupId === group);
        if (dicGroup && dicGroup.dictionaryDefines) {
            let dic = dicGroup.dictionaryDefines.find(x => x.value === value);
            if (dic) {
                return dic.key;
            }
        }
        return '';
    }

    render() {
        const activeAuditInfo = this.props.activeAuditInfo;
        const isBuildingPic = (this.props.picType === "building");
        const isShopPic = (this.props.picType === "shop");
        let {previewVisible, previewImage, buildingImgList, shopImgList, photoCategories} = this.state;
        return (<div>
            {
                isBuildingPic ? <div>
                    <Tabs defaultActiveKey="1" className='picBox'>
                        {
                            photoCategories.map((item, i) => {
                                return (
                                    <TabPane tab={item.key} key={item.value}>
                                        <div className='picBox'>
                                            {
                                                buildingImgList[item.value] ?
                                                    <div className="clearfix">
                                                        <Upload multiple={true}
                                                            listType="picture-card"
                                                            fileList={buildingImgList[item.value]}
                                                            onPreview={this.handlePreview}>
                                                        </Upload>
                                                        <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                                                            <img alt="example" style={{width: '100%'}} src={previewImage} />
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
                </div> : null
            }
            <Row>
                <Col>
                    {
                        isShopPic ? <div>
                            <Upload multiple={true}
                                listType="picture-card"
                                fileList={shopImgList}
                                onPreview={this.handlePreview}>
                            </Upload>
                        </div> : null
                    }
                </Col>
            </Row>
            <Row>
                <Col>*红色边框的图片为本次删除的图片</Col>
            </Row>
            <Row>
                <Col>
                    <div style={{border: '1px solid red'}}>
                        {/**审核记录**/}
                        <AuditHistory />
                    </div>
                </Col>
            </Row>
            {
                activeAuditInfo.examineStatus === 1 ? <AuditForm /> : null
            }
        </div>)
    }
}
function mapStateToProps(state) {
    console.log("rootBasicDatarootBasicDatarootBasicData:", state.audit);
    return {
        activeAuditInfo: state.audit.activeAuditInfo,
        dealInfo: state.audit.dealInfo,
        rootBasicData: state.rootBasicData,
        curActiveInfo: state.audit.curActiveInfo
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditPicture);