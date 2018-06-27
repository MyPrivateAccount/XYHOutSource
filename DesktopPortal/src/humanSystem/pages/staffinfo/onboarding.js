import React, {Component} from 'react';
import {Form, Modal, Cascader, Input, InputNumber, DatePicker, notification, Select, Icon, Upload, Button, Row, Col, Checkbox, Tag, Spin, Table} from 'antd'
import {connect} from 'react-redux';
import moment from 'moment';
import WebApiConfig from '../../constants/webapiConfig';
import './staff.less';
import {postHumanInfo, getcreateOrgStation, getcreateStation, getSalaryItem} from '../../actions/actionCreator';
import {NewGuid} from '../../../utils/appUtils';
import ApiClient from '../../../utils/apiClient';
import FormerCompany from '../dialog/formerCompany';
import Education from '../dialog/education';
import PositionalTitle from '../dialog/positionalTitle';
import '../dialog/dialog.less'
import {getDicPars} from '../../../utils/utils'
import SocialSecurity from './form/socialSecurity'
import Salary from './form/salary'
import Layer from '../../../components/Layer'

const Option = Select.Option;
const FormItem = Form.Item;
const {TextArea} = Input;

const formItemLayout = {
    labelCol: {span: 12},
    wrapperCol: {span: 12},
};
const entryType = [
    {label: '新入职', key: '0'},
    {label: '重复入职', key: '1'}
];
const marriages = [
    {label: '未婚', key: '0'},
    {label: '已婚', key: '1'},
];
const styles = {
    subHeader: {
        padding: '5px',
        marginBottom: '10px',
        backgroundColor: '#e0e0e0'
    }
}
//前公司列表
const formerCompanyColumns = [{
    title: '所在单位',
    dataIndex: 'company',
    key: 'company',
}, {
    title: '岗位',
    dataIndex: 'position',
    key: 'position',
}, {
    title: '起始时间',
    dataIndex: 'startTime',
    key: 'startTime',
}, {
    title: '终止时间',
    dataIndex: 'endTime',
    key: 'endTime',
}, {
    title: '证明人',
    dataIndex: 'proveMan',
    key: 'proveMan',
}, {
    title: '证明人联系电话',
    dataIndex: 'provePhone',
    key: 'provePhone',
}];
//学历信息
const educationColumns = [
    {
        title: '学历',
        dataIndex: 'education',
        key: 'education',
    },
    {
        title: '所学专业',
        dataIndex: 'major',
        key: 'major',
    },
    {
        title: '学习形式',
        dataIndex: 'learningType',
        key: 'learningType',
    },
    {
        title: '毕业证书',
        dataIndex: 'graduationCertificate',
        key: 'graduationCertificate',
    },
    {
        title: '入学时间',
        dataIndex: 'enrolmentTime',
        key: 'enrolmentTime',
    },
    {
        title: '毕业时间',
        dataIndex: 'graduationTime',
        key: 'graduationTime',
    },
    {
        title: '获得学位',
        dataIndex: 'getDegree',
        key: 'getDegree',
    },
    {
        title: '学位授予时间',
        dataIndex: 'getDegreeTime',
        key: 'getDegreeTime',
    },
    {
        title: '学位授予单位',
        dataIndex: 'getDegreeCompany',
        key: 'getDegreeCompany',
    },
    {
        title: '毕业学校',
        dataIndex: 'graduationSchool',
        key: 'graduationSchool',
    }
];
//职称信息
const titleColumns = [
    {
        title: '职称',
        dataIndex: 'titleName',
        key: 'titleName',
    },
    {
        title: '取得时间',
        dataIndex: 'getTitleTime',
        render: (text, record) => <div>{record.getTitleTime ? record.getTitleTime.format('YYYY-MM-DD') : null}</div>,
        key: 'getTitleTime',
    }
];
class OnBoarding extends Component {

    state = {
        department: "",
        loading: false,
        fileList: [],
        previewVisible: false,
        previewImage: '',
        fileinfo: {},
        userinfo: {},
        formerCompanyDgShow: false,//上单位对话框
        educationDgShow: false,//学历对话框
        positionalDgShow: false,//职称对话框
        formerCompanyList: [],//上单位列表
        educationList: [],//学历列表
        positionalTitleList: [],//职称列表
        dicSource: {},//字典列表
        SocialSecurity: {},//社保信息
        SocialSecurityForm: null,
        Salary: {},//薪资构成信息
        SalaryForm: null
    }

    componentWillMount() {
        this.state.userinfo.id = NewGuid();
        if (this.props.ismodify == 1) {
            this.props.dispatch(getcreateOrgStation(this.props.selHumanList[this.props.selHumanList.length - 1].departmentId));
        }
        //this.props.dispatch(getallOrgTree('PublicRoleOper'));
    }

    componentDidMount() {
        let len = this.props.selHumanList.length;
        if (this.props.ismodify == 1) {//修改界面
            if (len > 0) {
                this.state.userinfo.id = this.state.id = this.props.selHumanList[len - 1].id;
                this.props.form.setFieldsValue({name: this.props.selHumanList[len - 1].name});
                this.props.form.setFieldsValue({sex: this.props.selHumanList[len - 1].sex});
                this.props.form.setFieldsValue({idcard: this.props.selHumanList[len - 1].idcard});

                let lstvalue = [];
                this.props.setDepartmentOrgTree.findIndex(e => this.findCascaderLst(this.props.selHumanList[len - 1].departmentId, e, lstvalue));
                this.props.form.setFieldsValue({departmentId: lstvalue});

                this.props.form.setFieldsValue({entryTime: this.props.selHumanList[len - 1].entryTime});
                this.props.form.setFieldsValue({becomeTime: this.props.selHumanList[len - 1].becomeTime});
                this.props.form.setFieldsValue({baseSalary: this.props.selHumanList[len - 1].baseSalary});
                this.props.form.setFieldsValue({socialInsurance: this.props.selHumanList[len - 1].socialInsurance});
                this.props.form.setFieldsValue({contract: this.props.selHumanList[len - 1].contract});
            }
        }
        else {
            let formerCompanyExtColumn = this.getOperColumn('formerCompany');
            let educationExtColumn = this.getOperColumn('education');
            let positionTitleExtColumn = this.getOperColumn('positionalTitle');
            this.setState({formerCompanyColumns: formerCompanyColumns.concat(formerCompanyExtColumn)});
            this.setState({educationColumns: educationColumns.concat(educationExtColumn)})
            this.setState({titleColumns: titleColumns.concat(positionTitleExtColumn)})
        }
        let dicNation = getDicPars("HUMEN_Nation", this.props.rootBasicData);
        let dicHouseRegister = getDicPars("HUMEN_HOUSE_REGISTER", this.props.rootBasicData);
        let dicEducation = getDicPars("HUMEN_EDUCATION", this.props.rootBasicData);
        let dicHealth = getDicPars("HUMENT_HEALTH", this.props.rootBasicData);
        let dicPolitics = getDicPars("HUMEN_POLITICS", this.props.rootBasicData);
        let dicContractCategories = getDicPars("CONTRACT_CATEGORIES", this.props.rootBasicData);
        let dicDegree = getDicPars("HUMEN_DEGREE", this.props.rootBasicData);
        this.setState({dicNation: dicNation});
        this.setState({dicHouseRegister: dicHouseRegister});
        this.setState({dicEducation: dicEducation});
        this.setState({dicHealth: dicHealth});
        this.setState({dicPolitics: dicPolitics});
        this.setState({dicContractCategories: dicContractCategories});
        this.setState({dicDegree: dicDegree});
        console.log("字典列表:", dicNation, dicHouseRegister);
    }

    getOperColumn(tableType) {
        let operColumn = {
            title: '操作',
            dataIndex: 'oper',
            render: (text, record) => {
                return (
                    <div>
                        <Button type="primary" size='small' style={{marginRight: '5px'}} shape="circle" icon="edit" onClick={() => this.tableOperate(tableType, 'edit', record)} />
                        <Button type="primary" size='small' shape="circle" icon="delete" onClick={() => this.tableOperate(tableType, 'delete', record)} />
                    </div>
                )
            },
        };
        return [operColumn];
    }

    tableOperate = (listType, operType, record) => {
        if (operType == 'delete') {
            let tableList = [];
            if (listType == 'formerCompany') {
                tableList = this.state.formerCompanyList || [];
            } else if (listType == 'education') {
                tableList = this.state.educationList || [];
            } else if (listType == 'positionalTitle') {
                tableList = this.state.positionalTitleList || [];
            }
            let index = tableList.findIndex(item => item.id == record.id);
            if (index != -1) {
                tableList.splice(index, 1);
                if (listType == 'formerCompany') {
                    this.setState({formerCompanyList: tableList});
                } else if (listType == 'education') {
                    this.setState({educationList: tableList});
                } else if (listType == 'positionalTitle') {
                    this.setState({positionalTitleList: tableList});
                }
            }
        } else if (operType == 'edit') {

        }
    }

    findCascaderLst(id, tree, lst) {
        if (tree) {
            if (tree.id === id) {
                lst.unshift(tree.id);
                return true;
            } else if (tree.children && tree.children.length > 0) {
                if (tree.children.findIndex(org => this.findCascaderLst(id, org, lst)) !== -1) {
                    lst.unshift(tree.id);
                    return true;
                }
            }
        }
        return false;
    }

    isCardID(rule, value, callback) {
        var reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
        if (reg.test(value) === false) {
            callback('Wrong type')
            return false;
        }
        callback();
        return true;
    }

    handleCancel = () => this.setState({previewVisible: false})

    handlePreview = (file) => {
        this.setState({
            previewImage: file.url || file.thumbUrl,
            previewVisible: true,
        });
    }
    handleChange = ({fileList}) => this.setState({fileList})

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    getWorkNumber() {
        let tempthis = this;
        let url = WebApiConfig.server.GetWorkNumber;
        ApiClient.get(url).then(function (f) {
            if (f.data.code == 0) {
                tempthis.props.form.setFieldsValue({id: f.data.extension});
            }
        });
    }

    UploadFile(file, callback) {
        let id = this.state.userinfo.id;
        let uploadUrl = `${WebApiConfig.attach.uploadUrl}${id}`;
        let fileGuid = NewGuid();
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
                        appId: 'HumanIndex',
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

    handleBeforeUpload = (uploadFile) => {
        let reader = new FileReader();
        let the = this;
        reader.readAsDataURL(uploadFile);

        reader.onloadend = function () {
            the.UploadFile(uploadFile, (ufile) => {
                let filelist = [{
                    uid: -1,
                    name: uploadFile.name,
                    status: 'done',
                    url: reader.result,
                }]
                the.setState({fileinfo: ufile, fileList: filelist});
            });
        }
        return true;
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                if (values.departmentId instanceof Array) {
                    values.departmentId = values.departmentId[values.departmentId.length - 1];
                }
                if (this.state.SocialSecurityForm) {
                    let socialSecurityValues = this.state.SocialSecurityForm.getFieldsValue();
                    values = {...values, humanSocialSecurity: socialSecurityValues}
                }
                if (this.state.SalaryForm) {
                    let salaryValues = this.state.SalaryForm.getFieldsValue();
                    values = {...values, humanSalaryStructure: salaryValues}
                }
                values.humanTitleInfos = this.state.positionalTitleList || []
                values.humanWorkHistories = this.state.formerCompanyList || []
                values.humanEducationInfos = this.state.educationList || []
                console.log("提交内容", values);
                this.props.dispatch(postHumanInfo({humaninfo: values, fileinfo: this.state.fileinfo}));
            }
        });
    }

    handleReset = () => {
        this.props.form.resetFields();
    }

    handleDepartmentChange = (e) => {
        if (!e) {
            this.props.dispatch(getcreateStation(this.state.department));
        }
    }

    handleChooseDepartmentChange = (e) => {
        this.state.department = e[e.length - 1];
    }

    handleSelectChange = (e) => {
        this.props.dispatch(getSalaryItem(e));
    }
    //对话框信息回调
    dialogConfirmCallback = (info, type) => {
        console.log("表单对象:", this.props.form, info, type);
        if (type == 'formerCompany') {
            let formerCompanyList = this.state.formerCompanyList;
            formerCompanyList.push(info);
            this.setState({formerCompanyList: formerCompanyList});
        } else if (type == 'education') {
            let educationList = this.state.educationList;
            educationList.push(info);
            this.setState({educationList: educationList});
        } else if (type == 'positionalTitle') {
            let positionalTitleList = this.state.positionalTitleList;
            positionalTitleList.push(info);
            this.setState({positionalTitleList: positionalTitleList});
        }
    }
    //子页面回调
    subPageLoadCallback = (formObj, pageName) => {
        console.log("表单对象:", formObj, pageName);
        if (pageName == "socialSecurity") {
            this.setState({SocialSecurityForm: formObj});
        } else if (pageName == "salary") {
            this.setState({SalaryForm: formObj});
        }
    }
    onIdCardBlur = (e) => {
        let idCard = e.target.value;
        if (idCard.length > 0 && !this.props.form.getFieldError('idcards')) {
            let birthday = moment(idCard.substr(6, 4) + "-" + idCard.substr(10, 2) + "-" + idCard.substr(12, 2));
            let age = moment().diff(birthday, 'year');
            let sex = (idCard.substr(17, 1) % 2 == 1 ? "1" : "2");//1:男,2:女
            this.props.form.setFieldsValue({age: age, birthday: birthday, sex: sex});
        }
    }
    render() {
        let self = this;
        let fileList = this.state.fileList;
        const {previewVisible, previewImage, formerCompanyColumns, educationColumns, titleColumns} = this.state;
        const {getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched} = this.props.form;

        let psition = this.props.selHumanList.length > 0 ? this.props.selHumanList[this.props.selHumanList.length - 1].position : 0;

        if (this.props.ismodify == 1) {
            fileList = this.props.humanImage;
        }

        const uploadButton = (this.props.ismodify == 1) ? null : (
            <div>
                <Icon type='plus' />
                <div className="ant-upload-text">Upload</div>
            </div>
        );
        let judgePermissions = this.props.judgePermissions || [];
        return (
            <Layer>
                <div className="page-title" style={{marginBottom: '10px'}}>员工信息表</div>
                <Form layout="horizontal" onSubmit={this.handleSubmit} style={{paddingTop: '5px'}}>
                    <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />入职信息</h3>
                    <Row>
                        <Col span={14}>
                            <Row>
                                <Col span={12}>
                                    <FormItem {...formItemLayout} label="工号">
                                        {getFieldDecorator('userID', {
                                            initialValue: this.state.userinfo.worknumber,
                                            rules: [{
                                                required: true,
                                                message: 'please entry Worknumber',
                                            }]
                                        })(
                                            <Input disabled={this.props.ismodify == 1} />
                                        )}
                                    </FormItem>
                                </Col>
                                <Col span={12}>
                                    <FormItem {...formItemLayout} label="姓名">
                                        {getFieldDecorator('name', {
                                            rules: [{
                                                required: true, message: '请填写姓名',
                                            }]
                                        })(
                                            <Input disabled={this.props.ismodify == 1} placeholder="请输入姓名" />
                                        )}
                                    </FormItem>
                                </Col>
                            </Row>
                            <Row>
                                <Col span={12}>
                                    <FormItem {...formItemLayout} label="身份证号" >
                                        {getFieldDecorator('idCard', {
                                            rules: [{
                                                required: true, message: '请输入身份证号',
                                            }, {
                                                pattern: /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/, message: '不是有效的身份证号'
                                            }]
                                        })(
                                            <Input disabled={this.props.ismodify == 1} onBlur={this.onIdCardBlur} placeholder="请输入身份证号码" />
                                        )}
                                    </FormItem>
                                </Col>
                                <Col span={12}>
                                    <FormItem {...formItemLayout} label="生日">
                                        {getFieldDecorator('birthday', {
                                            rules: [{
                                                required: true, message: '请输入生日',
                                            }]
                                        })(
                                            <DatePicker disabled={this.props.ismodify == 1} format='YYYY-MM-DD' style={{width: '100%'}} />
                                        )}
                                    </FormItem>
                                </Col>
                            </Row>
                            <Row>
                                <Col span={12}>
                                    <FormItem {...formItemLayout} label="性别">
                                        {getFieldDecorator('sex', {
                                            rules: [{
                                                required: true, message: '请选择性别',
                                            }]
                                        })(
                                            <Select disabled={this.props.ismodify == 1} placeholder="选择性别">
                                                <Option key='1' value="1">男</Option>
                                                <Option key='2' value="2">女</Option>
                                            </Select>
                                        )}
                                    </FormItem>
                                </Col>
                                <Col span={12}>
                                    <FormItem {...formItemLayout} label="手机号码" >
                                        {getFieldDecorator('idcard', {
                                            rules: [{
                                                required: true, message: '请输入手机号码',
                                            }, {pattern: '^((1[0-9][0-9])|(14[5|7])|(15([0-3]|[5-9]))|(18[0,5-9]))\\d{8}$', message: '不是有效的手机号码!'}]
                                        })(
                                            <Input disabled={this.props.ismodify == 1} placeholder="请输入手机号码" />
                                        )}
                                    </FormItem>
                                </Col>
                            </Row>

                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="图片">
                                <div className="clearfix">
                                    <Upload
                                        action="//jsonplaceholder.typicode.com/posts/"
                                        listType="picture-card"
                                        fileList={fileList}
                                        onPreview={this.handlePreview}
                                        onChange={this.handleChange}
                                        beforeUpload={this.handleBeforeUpload}
                                    >
                                        {fileList.length >= 1 ? null : uploadButton}
                                    </Upload>
                                    <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                                        <img alt="example" style={{width: '100%'}} src={previewImage} />
                                    </Modal>
                                </div>
                            </FormItem>
                        </Col>
                    </Row>



                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="公司" >
                                {getFieldDecorator('company', {
                                    rules: [{
                                        required: true, message: '请输入公司',
                                    }]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入公司" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="所属部门">
                                {getFieldDecorator('departmentId', {
                                    rules: [{
                                        required: true,
                                        message: 'please entry',
                                    }]
                                })(
                                    <Cascader disabled={this.props.ismodify == 1} options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect placeholder="归属部门" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="年龄">
                                {getFieldDecorator('age', {
                                    rules: [{
                                        required: true, message: 'please entry Age',
                                    }]
                                })(
                                    <InputNumber disabled style={{width: '100%'}} />
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="职位名称">
                                {getFieldDecorator('position', {
                                    rules: [{
                                        required: true,
                                        message: 'please entry Position',
                                    }],

                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            (self.props.stationList && self.props.stationList.length > 0) ?
                                                self.props.stationList.map(
                                                    function (params) {
                                                        return <Option key={params.key} value={params.id}>{params.stationname}</Option>;
                                                    }
                                                ) : null
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="入职类型">
                                {getFieldDecorator('positionType', {
                                    rules: [{
                                        required: true,
                                        message: '请选择入职类型',
                                    }],

                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            entryType.map(item => <Option key={item.key} value={item.key}>{item.label}</Option>)
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="入职日期">
                                {getFieldDecorator('entryTime', {
                                    initialValue: moment(),
                                    rules: [{
                                        required: true,
                                        message: '请选择入职日期'
                                    }]
                                })(
                                    <DatePicker disabled={this.props.ismodify == 1} format='YYYY-MM-DD' style={{width: '100%'}} />
                                )}
                            </FormItem>
                        </Col>
                    </Row>

                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="婚姻状况">
                                {getFieldDecorator('position', {
                                    rules: [{
                                        required: true,
                                        message: '请选择婚姻状况',
                                    }],
                                    initialValue: "1"
                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            marriages.map(item => <Option key={item.key} value={item.key}>{item.label}</Option>)
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="民族">
                                {getFieldDecorator('nationality', {
                                    rules: [{
                                        required: true,
                                        message: '请选择民族',
                                    }],
                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            (this.state.dicNation || []).map(item => <Option key={item.value} value={item.value}>{item.key}</Option>)
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="户籍类型">
                                {getFieldDecorator('householdType', {
                                    rules: [{
                                        required: true,
                                        message: '请选择户籍类型',
                                    }],

                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            (this.state.dicHouseRegister || []).map(item => <Option key={item.value} value={item.value}>{item.key}</Option>)
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="最高学历">
                                {getFieldDecorator('highestEducation', {
                                    rules: [{
                                        required: true,
                                        message: '请选择最高学历',
                                    }],
                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            (this.state.dicEducation || []).map(item => <Option key={item.value} value={item.value}>{item.key}</Option>)
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="健康状况">
                                {getFieldDecorator('healthCondition', {
                                    rules: [{
                                        required: true,
                                        message: '请选择健康状况',
                                    }],

                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            (this.state.dicHealth || []).map(item => <Option key={item.value} value={item.value}>{item.key}</Option>)
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="籍贯" >
                                {getFieldDecorator('nativePlace', {
                                    rules: [{
                                        required: true, message: '请输入籍贯',
                                    }]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请填写籍贯" />
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={14}>
                            <FormItem labelCol={{span: 6}} wrapperCol={{span: 12}} label="家庭住址" >
                                {getFieldDecorator('familyAddress', {
                                    rules: [{
                                        required: true, message: '请输入家庭住址',
                                    }]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入家庭住址" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}></Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="政治面貌">
                                {getFieldDecorator('policitalStatus', {

                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            (this.state.dicPolitics || []).map(item => <Option key={item.value} value={item.value}>{item.key}</Option>)
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="户口地址" >
                                {getFieldDecorator('domicilePlace', {
                                    rules: [{
                                    }]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入户口地址" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}></Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="紧急联系人" >
                                {getFieldDecorator('emergencyContact', {
                                    rules: [{
                                        required: true, message: '请输入紧急联系人',
                                    }]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入紧急联系人" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="紧急联系人电话" >
                                {getFieldDecorator('emergencyContactPhone', {
                                    rules: [{
                                        required: true, message: '请输入手机号码',
                                    }, {pattern: '^((1[0-9][0-9])|(14[5|7])|(15([0-3]|[5-9]))|(18[0,5-9]))\\d{8}$', message: '不是有效的手机号码!'}]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入手机号码" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="紧急联系人关系" >
                                {getFieldDecorator('emergencyContactType', {
                                    rules: [{
                                        required: true, message: '请输入紧急联系人关系',
                                    }]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入紧急联系人关系" />
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={14}>
                            <FormItem labelCol={{span: 6}} wrapperCol={{span: 12}} label="Email地址" >
                                {getFieldDecorator('emailAddress', {
                                    rules: [{
                                        required: true, message: 'Email地址',
                                    }, {type: 'email', message: '请输入正确的email地址'}]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入Email地址" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}></Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="银行名称" >
                                {getFieldDecorator('bankName', {
                                    rules: []
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入银行名称" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="银行账号" >
                                {getFieldDecorator('bankAccount', {
                                    rules: []
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入银行账号" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}></Col>
                    </Row>
                    <Row>
                        <Col span={14}>
                            <FormItem labelCol={{span: 6}} wrapperCol={{span: 12}} label="备注" >
                                {getFieldDecorator('desc', {
                                    rules: []
                                })(
                                    <TextArea rows={4} disabled={this.props.ismodify == 1} placeholder="请输入备注" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}></Col>
                    </Row>
                    <Row>
                        <Col span={14}>
                            <FormItem labelCol={{span: 6}} wrapperCol={{span: 12}} label="附件" >
                                <Upload >
                                    <Button><Icon type="upload" />上传</Button>
                                </Upload>
                            </FormItem>
                        </Col>

                    </Row>

                    <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />合同信息</h3>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="合同编号" >
                                {getFieldDecorator('contractNo', {
                                    rules: [{
                                        required: true, message: '请输入合同编号',
                                    }]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入合同编号" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="签订单位" >
                                {getFieldDecorator('contractCompany', {
                                    rules: [{
                                        required: true, message: '请输入签订单位',
                                    }]
                                })(
                                    <Input disabled={this.props.ismodify == 1} placeholder="请输入签订单位" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="合同类型">
                                {getFieldDecorator('contractType', {

                                })(
                                    <Select disabled={this.props.ismodify == 1} onChange={this.handleSelectChange} placeholder="选择职位">
                                        {
                                            (this.state.dicContractCategories || []).map(item => <Option key={item.value} value={item.value}>{item.key}</Option>)
                                        }
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="合同签订日期">
                                {getFieldDecorator('contractSignDate', {
                                    initialValue: moment(),
                                    rules: [{
                                        required: true,
                                        message: '请选择合同签订日期'
                                    }]
                                })(
                                    <DatePicker disabled={this.props.ismodify == 1} format='YYYY-MM-DD' style={{width: '100%'}} />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="合同有效期">
                                {getFieldDecorator('contractStartDate', {
                                    rules: [{
                                        required: true,
                                        message: '请选择合同有效期'
                                    }]
                                })(
                                    <DatePicker disabled={this.props.ismodify == 1} format='YYYY-MM-DD' style={{width: '100%'}} />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="合同到期日">
                                {getFieldDecorator('contractEndDate', {
                                    rules: [{
                                        required: true,
                                        message: '请选择合同到期日'
                                    }]
                                })(
                                    <DatePicker disabled={this.props.ismodify == 1} format='YYYY-MM-DD' style={{width: '100%'}} />
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />上单位职位信息 <Button type="primary" size='small' shape="circle" icon="plus" onClick={() => this.setState({formerCompanyDgShow: true})} /></h3>
                    <Row>
                        <Col span={2}></Col>
                        <Col span={20}>
                            <Table dataSource={this.state.formerCompanyList || []} columns={formerCompanyColumns} style={{marginBottom: '10px'}} />
                        </Col>
                        <Col span={2}></Col>
                    </Row>


                    <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />学历信息 <Button type="primary" size='small' shape="circle" icon="plus" onClick={() => this.setState({educationDgShow: true})} /></h3>
                    <Row>
                        <Col span={2}></Col>
                        <Col span={20}>
                            <Table dataSource={this.state.educationList || []} columns={educationColumns} style={{marginBottom: '10px'}} />
                        </Col>
                        <Col span={2}></Col>
                    </Row>

                    <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />职称信息 <Button type="primary" size='small' shape="circle" icon="plus" onClick={() => this.setState({positionalDgShow: true})} /></h3>

                    <Row>
                        <Col span={2}></Col>
                        <Col span={20}>
                            <Table dataSource={this.state.positionalTitleList || []} columns={titleColumns} style={{marginBottom: '10px'}} />
                        </Col>
                        <Col span={2}></Col>
                    </Row>
                    {judgePermissions.includes('SOCIAL_SECURITY_VIEW') || this.props.ismodify != 1 ? <SocialSecurity subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)} /> : null}
                    {judgePermissions.includes('SALARY_VIEW') || this.props.ismodify != 1 ? <Salary subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)} /> : null}

                    {/* <FormItem {...formItemLayout} label="岗位补贴">
                    {getFieldDecorator('subsidy', {
                        initialValue: self.props.selSalaryItem ? self.props.selSalaryItem.subsidy : null
                    })(
                        <InputNumber disabled={this.props.ismodify == 1}  />
                    )}
                </FormItem>
                <FormItem {...formItemLayout} label="工装扣款">
                    {getFieldDecorator('clothesBack', {
                        initialValue: self.props.selSalaryItem ? self.props.selSalaryItem.clothesBack : null
                    })(
                        <InputNumber disabled={this.props.ismodify == 1}  />
                    )}
                </FormItem>
                <FormItem {...formItemLayout} label="行政扣款">
                    {getFieldDecorator('administrativeBack', {
                        initialValue: self.props.selSalaryItem ? self.props.selSalaryItem.administrativeBack : null
                    })(
                        <InputNumber disabled={this.props.ismodify == 1}  />
                    )}
                </FormItem>
                <FormItem {...formItemLayout} label="端口扣款">
                    {getFieldDecorator('portBack', {
                        initialValue: self.props.selSalaryItem ? self.props.selSalaryItem.portBack : null
                    })(
                        <InputNumber disabled={this.props.ismodify == 1}  />
                    )}
                </FormItem> */}
                    <Row>
                        <Col style={{textAlign: 'center'}}>
                            <Button type="primary" htmlType="submit" style={{marginRight: '20px'}} disabled={this.hasErrors(getFieldsValue())} onClick={(e) => this.handleSubmit(e)}>提交</Button>
                            <Button type="primary" onClick={this.handleReset}>清空</Button>
                        </Col>
                    </Row>
                    <FormerCompany showDialog={this.state.formerCompanyDgShow} closeDialog={() => this.setState({formerCompanyDgShow: false})} confirmCallback={(info) => this.dialogConfirmCallback(info, 'formerCompany')} />
                    <Education showDialog={this.state.educationDgShow} closeDialog={() => this.setState({educationDgShow: false})} confirmCallback={(info) => this.dialogConfirmCallback(info, 'education')} dicEducation={this.state.dicEducation} dicDegree={this.state.dicDegree} />
                    <PositionalTitle showDialog={this.state.positionalDgShow} closeDialog={() => this.setState({positionalDgShow: false})} confirmCallback={(info) => this.dialogConfirmCallback(info, 'positionalTitle')} />
                </Form>
            </Layer>
        );
    }
}



function stafftableMapStateToProps(state) {
    return {
        setDepartmentOrgTree: state.basicData.searchOrgTree,
        stationList: state.search.stationList,
        selSalaryItem: state.basicData.selSalaryItem,
        selHumanList: state.basicData.selHumanList,
        humanImage: state.basicData.humanImage,
        rootBasicData: (state.rootBasicData || {}).dicList,
        judgePermissions: state.judgePermissions
    }
}

function stafftableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

export default connect(stafftableMapStateToProps, stafftableMapDispatchToProps)(Form.create()(OnBoarding));