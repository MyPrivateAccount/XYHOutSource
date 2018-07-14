import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Tabs, Spin, Modal, Row, notification, Button } from 'antd'
import { getDicParList } from '../../../../actions/actionCreators'
import validations from '../../../../utils/validations'
import reportValidation from '../../../constants/reportValidation'
import UploadPanel from '../../uploadPanel'
import FkComponent from './fKComponet'
import SJComponent from './sJComponet'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import {dicKeys, examineStatusMap} from '../../../constants/const'
import '../rp.less'
import DistributePanel from '../ty/distributePanel'

const TabPane = Tabs.TabPane;
const confirm = Modal.confirm;

class FKPanel extends Component {
    state = {
        loading: false,
        factGet: {},
        report: {},
        saving: false,
        wyItems:[],
        nyItems:[]
    }

    componentWillMount = () => {
        this.initEntity(this.props)
        this.props.getDicParList([
            dicKeys.sfdx
        ])
        this._getFtItems();
    }

    componentWillReceiveProps = (nextProps) => {
        if (this.props.factGet !== nextProps.factGet && nextProps.factGet) {
            this.initEntity(nextProps);
        }
    }

    _getFtItems = async () => {

        let url = `${WebApiConfig.baseset.acmentlistget}${this.props.user.Filiale}`
        let r = await ApiClient.get(url);
        r = (r || {}).data || {};
        if (r.code === '0') {
            let list = r.extension || [];
            let wyItems = [], nyItems = [];
            list.forEach(item => {
                if (item.type === 1) {
                    wyItems.push(item)
                } else if (item.type === 2) {
                    nyItems.push(item)
                }
            })
            this.setState({ wyItems: wyItems, nyItems: nyItems })
        } else {
            notification.error({ message: '获取分摊项列表失败' })
        }
    }


    initEntity = (props) => {
        let f = props.factGet || {};

        let scopes = f.files || [];
        let files = [];
        scopes.forEach(item => {
            if (item.fileItem && item.fileList && item.fileList.length > 0) {
                let f = item.fileList[0];
                f.url = item.fileItem.original;
                f.uid = f.fileGuid;
                f.group = item.group;
                files.push(f);
            }
        })
        this.setState({ files: files })


        this.setState({ factGet: f, report: f.report || {} }, () => {
            // this.props.form.setFieldsValue({

            // })
        })

    }

    getValues = () => {
        let r = this.skForm.getValues();
        if (r.hasError) {
            notification.error({ message: '验证失败, 请检查输入是否正常' })
            return;
        }
        let factGet = this.state.factGet;
        factGet = { ...factGet, ...r.values }

        if (this.sjForm) {
            r = this.sjForm.getValues();
            if (r.hasError) {
                notification.error({ message: '验证失败, 请检查输入是否正常' })
                return;
            }
            factGet = { ...factGet, ...r.values };
        }

        var files = null;

        if (this.filesForm) {
            files = this.filesForm.getFiles();
        } else {
            files = this.state.files;
        }
        let scopes = [];
        files.forEach(item => {
            let si = { id: factGet.id, fileGuid: item.fileGuid, group: item.group }
            si.fileList = [{ ...item }]
            si.fileItem = { original: item.url }
            scopes.push(si)
        })

        factGet.files = scopes;


        this.setState({ factGet: {...factGet} })

        if(factGet.report){
            factGet.reportId = factGet.report.id;
            
            delete factGet.report;
        }
        

        return factGet;
    }

    submit = () => {
        let values = this.getValues();
        if (!values) {
            return;
        }


        let errors = validations.validate(values, reportValidation.fk);

        if (validations.checkErrors(errors)) {
            let msg = [];
            for (let k in errors) {
                msg.push(<li>{errors[k]}</li>)
            }
            notification.error({ message: '验证失败，无法保存', description: <ol>{msg}</ol> })
            return;
        }


        confirm({
            title: `您确定要提交此收款申请么?`,
            content: '提交后不可再进行修改',
            onOk: async () => {

                await this._submit(values);
            },
            onCancel() {

            },
        });
    }

    _submit = async (values) => {
        this.setState({  saving: true })
        let url = `${WebApiConfig.rp.factgetSave}`
        let  r = await ApiClient.post(url, values, null, 'PUT');
        r = (r || {}).data || {};
        if (r.code === '0') {
            let newFact = r.extension;
            let factGet = this.state.factGet;
            factGet.status  = newFact.status;
            
            this.setState({ factGet: { ...factGet } })

            notification.success({message:'付款申请已提交'})
            
        } else {
            notification.error({ message: '保存失败', description: r.message || '' })
        }

        this.setState({ saving: false })
    }

    render() {
        let { factGet, report } = this.state;
        let yjfp = report.reportYjfp || {};
        let { canEdit } = this.props;
        if(factGet.status!==0 && factGet.status!==16){
            canEdit = false;
        }
        let statusText = examineStatusMap[factGet.status] || '';
        let files = this.state.files;
        return (
            <Spin spinning={this.state.loading}>
                <div style={{ paddingTop: '1rem', paddingBottom: '1rem' }}>
                    {canEdit ? <div>
                        <Button disabled={this.state.saving} type="primary" onClick={this.submit} style={{ marginLeft: '0.5rem' }}>提交审核</Button>
                    </div> : null}
                    <div className="rp-yj-statustext">{statusText}</div>
                </div >
                <Row>
                    <span className="rp-yj">成交编号：</span><span className="rp-yj-je"> {factGet.cjbgbh} </span>
                    <span className="rp-yj">物业名称：</span><span className="rp-yj-je"> {factGet.wymc} </span>
                </Row>
                <DistributePanel 
                    hideYj={true}
                    hideNy={true}
                    onlyTable={true}
                    distribute={this.state.report.reportYjfp}
                    wyItems = {this.state.wyItems}
                    nyItems = {this.state.nyItems}
                    dic = {this.props.dic}
                    />
                {/* <Row>
                    <span className="rp-yj">业主佣金：</span><span className="rp-yj-je"> {yjfp.yjYzys || 0} </span>
                    <span className="rp-yj">已收取：</span><span className="rp-yj-je"> {yjfp.yjYsyy || 0} </span>
                    <span className="rp-yj">余额：</span><span className="rp-yj-je"> {yjfp.yjYyyk || 0} </span>

                </Row>
                <Row>
                    <span className="rp-yj">客户佣金：</span><span className="rp-yj-je"> {yjfp.yjKhys || 0} </span>
                    <span className="rp-yj">已收取：</span><span className="rp-yj-je"> {yjfp.yjYsky || 0} </span>
                    <span className="rp-yj">余额：</span><span className="rp-yj-je"> {yjfp.yjKyyk || 0} </span>

                </Row> */}
                
                <Tabs>
                    <TabPane tab="代收代付款" key="dsdfk">
                        <FkComponent
                            wrappedComponentRef={(inst) => this.skForm = inst}
                            canEdit={canEdit}
                            wyItems = {this.state.wyItems}
                            entity={this.state.factGet} />
                    </TabPane>
                    <TabPane tab="收据" key="sj">
                        <SJComponent
                            type="fk"
                            wrappedComponentRef={(inst) => this.sjForm = inst}
                            canEdit={canEdit}
                            entity={this.state.factGet}
                        />
                    </TabPane>
                    <TabPane tab="附件" key="fj">
                        <UploadPanel
                            ref={(ins) => this.filesForm = ins}
                            files={files}
                            entityId = {this.state.factGet.id}
                            canEdit={canEdit} />
                    </TabPane>
                </Tabs>
            </Spin>
        )
    }
}

function MapStateToProps(state) {
    return {
        dic: state.basicData.dicList,
        user: state.oidc.user.profile || {}
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch,
        getDicParList: (...args) => dispatch(getDicParList(...args))
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(FKPanel);