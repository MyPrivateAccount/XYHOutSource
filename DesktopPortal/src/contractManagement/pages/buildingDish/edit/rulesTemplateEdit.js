import { connect } from 'react-redux';
import { saveRulesInfo, rulesTemplateView, saveTemplateRows,ruleTemplateLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Select, Form, Button, Checkbox, Icon, Row, Col, Input, InputNumber,Modal,Popover,Card , Menu, Dropdown, } from 'antd'
import { getReport } from './utils'

const FormItem = Form.Item;
const Option = Select.Option;
const CheckboxGroup = Checkbox.Group;

const expCustomer = {
    "id": "4fa9fe37-2899-4174-a1c9-ad8e00188b5a",
    "customerId": "fba92d39-2aeb-4d19-88cd-99143e6fa598",
    "customerName": "张三",
    "userId": "5cfe1b85-360b-4014-94bc-3e37cece7339",
    "departmentId": "0",
    "departmentName": "西区一区一组",
    "mainPhone": "150****3636",
    "userPhone": '13122220000',
    "userTrueName": "业务员A",
    "transactionsStatus": 2,
    "reportTime": "2017-11-30T17:59:32.191637",
    "buildingId": "b5a0baae-9691-4a20-9723-c07f9d2b7631",
    "shopsId": "77b11f39-d70f-47d1-9edc-24ab794dbc34",
    "buildingName": "万科金色悦城",
    "shopsName": "3",
    "expectedBeltTime": "2017-11-28T17:48:53.770883"
}
let templateItems = [
    { label: '所属公司 ：', key: '所属公司', value: { defValue: '新耀行', type: 'input' } },
    { label: '项目 ：', key: '项目', value: { defValue: '[项目名称]', type: 'input' } },
    { label: '客户姓名 ：', key: '客户姓名', value: { defValue: '$CUS_NAME$', type: 'readonly' } },
    { label: '客户电话 ：', key: '客户电话', value: { defValue: '$CUS_MOBILE$', type: 'readonly' } },
    {
        label: '报备日期 ：', key: '报备日期', value: {
            defValue: '$TODAY$', valueLabel: '当天(格式：2017年01月01日)', type: 'list', list: [
                { value: '$TODAY$', label: '当天(格式：2017年01月01日)' },
                { value: '$TODAY2$', label: '当天(格式：2017-01-01)' },
                { value: '$TODAY3$', label: '当天(格式：2017.01.01)' },
            ]
        }
    },
    {
        label: '预计带看时间 ：', key: '预计带看时间', value: {
            defValue: '$REPORTTIME$', valueLabel: '(格式：2017年01月01日 13:00)', type: 'list', list: [
                { value: '$REPORTTIME$', label: '(格式：2017年01月01日 13:00)' },
                { value: '$REPORTTIME2$', label: '(格式：2017-01-01 13:00)' },
                { value: '$REPORTTIME3$', label: '(格式：2017.01.01 13:00)' },
            ]
        }
    },
    { label: '业务员 ：', key: '业务员', value: { defValue: '$USERNAME$', type: 'readonly' } },
    { label: '业务员电话 ：', key: '业务员电话', value: { defValue: '$USER_MOBILE$', type: 'readonly' } },
    { label: '业务员组别 ：', key: '业务员组别', value: { defValue: '$DEPARTMENT$', type: 'readonly' } },
    { label: '驻场 ：', key: '驻场', value: { defValue: '$ZC_NAME$', type: 'readonly' } },
    { label: '驻场电话 ：', key: '驻场电话', value: { defValue: '$ZC_MOBILE$', type: 'readonly' } },
]

class RulesTemplateEdit extends Component {
    state = {
        loadingState: false,
        visible: false,
        popoverVisible: false,
        myKey: '',
        defaultValue: '',
        // templateData: [],
        ops: [],
        disabled: false,
        myLine: {},
        myList: [],
    }
    componentWillMount() {

        let template = (this.props.buildInfo.ruleInfo || {}).reportedTemplate;
        console.log(template, '进来第一次的模板有没有？？/')
        let templateData = [];
        if(template){
            templateData = JSON.parse(template);
            this.props.dispatch(saveTemplateRows({ templateData: templateData }))
            // this.setState({templateData: templateData})
        }
    }
    componentWillReceiveProps(newProps) {
        // let template = (newProps.buildInfo.ruleInfo || {}).reportedTemplate;
        // let templateData = [];
        // if(template){
        //     templateData = JSON.parse(template);
        //     // this.props.dispatch(saveTemplateRows({ templateData: templateData }))
        //     this.setState({templateData: templateData})
        // }
        this.setState({ 
            loadingState: false
        });
    }
    handleSave = () => { // 保存模板信息
        let { rulesTemplateOperType } = this.props.operInfo;
        const {templateData} = this.props;
        this.props.dispatch(ruleTemplateLoadingStart())
        // this.setState({ loadingState: true });
        let ruleInfo = this.props.buildInfo.ruleInfo || {}
        let newInfo = {
            ...ruleInfo,
            id: this.props.buildInfo.id,
            reportedTemplate: JSON.stringify(templateData)
        }
        this.props.dispatch(saveRulesInfo({
            rulesOperType: rulesTemplateOperType, 
            entity: newInfo, 
            id: this.props.buildInfo.id,
            template: true,
            ownCity: this.props.user.City 
        }));
    }
    handleCancel = (e) => { // 取消返回展示页面
        this.props.dispatch(rulesTemplateView());
    }
    defaultValueChange = (e) => {
        this.setState({ defaultValue: e.target.value })
    }
    showModal = () => { //点击添加行数显示模态框
        this.setState({
            visible: true,
            defaultValue: '',
        });
    }
    handleOkModal = (e) => { // 点击模态框保存
        this.setState({ visible: false });
        let thisTd = templateItems.find( (x) => { return x.key === this.state.myKey })
        // console.log(this.state.myKey, thisTd, '选中的信息')
        let td = [...this.props.templateData]
        // let td = [...this.state.templateData]
        td.push({
            label: thisTd.label,
            value: this.state.defaultValue ? this.state.defaultValue : thisTd.value.defValue,
            valueLabel: thisTd.value.valueLabel,
            key: thisTd.key,
            type: thisTd.value.type
        })
        this.props.dispatch(saveTemplateRows({ templateData: td }))
        // this.setState({ templateData: td })
    }
    handleCancelModal = (e) => {
        this.setState({visible: false});
    }
    
    selectAfterChange = (v) => { // 选择行类型Option
        this.setState({ myKey: v})
        if (v === '所属公司' || v === '项目') {
            this.setState({disabled: false})
        } else {
            this.setState({disabled: true})
        }
    }

    labelChanged = (line, val) => { // 模板input框改变值
        let td = [...this.props.templateData]
        // let td = [...this.state.templateData]
        let idx = td.indexOf(line);
        if (idx >= 0) {
            td.splice(idx, 1, { ...line, label: val })
            this.props.dispatch(saveTemplateRows({ templateData: td }))
            // this.setState({ templateData: td })
        }
    }
    valueChanged = (line, val) => {
        let td = [...this.props.templateData]
        // let td = [...this.state.templateData]
        let idx = td.indexOf(line);
        if (idx >= 0) {
            td.splice(idx, 1, { ...line, value: val })
            this.props.dispatch(saveTemplateRows({ templateData: td }))
            // this.setState({ templateData: td })
        }
    }
    changeFormat = (line) => {
        let td = [...this.props.templateData]
        // let td = [...this.state.templateData]
        let idx = td.indexOf(line);
        if (idx >= 0) {
            let list = templateItems.find(x => x.key === line.key).value.list;
            let ops = list.map(x => ({
                label: x.label,
                value: x.value
            }))
            this.setState({
                ops: ops,
                myLine: line,
                myList: list,
            })
        }
    }
    menuClick = (e) => {
        let myList = this.state.ops[e.key];
        this._changeFormat(this.state.myLine, myList);
    }
    _changeFormat = (line, fmt) => {
        let td = [...this.props.templateData]
        // let td = [...this.state.templateData]
        let idx = td.indexOf(line);
        if (idx >= 0) {
            td.splice(idx, 1, { ...line, value: fmt.value, valueLabel: fmt.label })
            this.props.dispatch(saveTemplateRows({ templateData: td }))
            // this.setState({ templateData: td })
        }
    }

    delLine = (line) => { // 删除行
        let td = [...this.props.templateData]
        // let td = [...this.state.templateData]
        let idx = td.indexOf(line);
        if (idx >= 0) {
            td.splice(idx, 1)
            this.props.dispatch(saveTemplateRows({ templateData: td }))
            // this.setState({ templateData: td })
        }
    }

    handleVisibleChange = (popoverVisible) => {
        this.setState({ popoverVisible });
    }
    

    render() {
        let lines = [];
        templateItems.forEach(x => {
            let used = this.props.templateData.find(y => y.key === x.key);
            if (!used) {
                lines.push(x);
            }
        })
        const selectAfter = (
            <Select  placeholder="请选择类型" style={{ width: 200 }} onSelect={this.selectAfterChange}>
                {lines.map((x) => {
                    return <Option  value={x.key} key={x.key}>{x.key}</Option>
                })}
            </Select>
        );

        let p = this.props.buildInfo || {};
        // let td = this.state.templateData
        let td = this.props.templateData
        if (!p.buildingBasic || !p.buildingBasic.name) {
            p.buildingBasic = { name: '楼盘名称' }
        }
        let tempateStr = getReport(td, expCustomer, this.props.user, p)

        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const Cardtitle = (
            <h4><Icon type="star-o" style={{marginRight: '5px'}}/> 预览</h4>
        );
        const CardTitleLeft = (
            <h4><Icon type="star-o" style={{marginRight: '5px'}}/> 模板</h4>
        );
        const menu = (
            <Menu onClick={this.menuClick}>
            {
                this.state.ops.map((item, index) => {
                    return (
                        <Menu.Item key={index}>{item.label}</Menu.Item>
                    )
                })
            }
            </Menu>
        );
        
        let rulesTemplateinfo = this.props.buildInfo.rulesTemplateinfo;
        let { rulesTemplateOperType } = this.props.operInfo;

        return (
            <Form layout="horizontal" style={{padding: '25px 0', backgroundColor: "#ECECEC" }} className='rulesTemplate'>
                <Icon type="tags-o" className='content-icon'/> <span className='content-title'>报备模板</span>
                <Row type="flex" style={{marginTop: "25px"}}>
                    <Button type='primary' style={{ marginLeft: '50px' }} onClick={this.showModal}>添加行</Button>
                </Row>
                <Row style={{ padding: '25px'}}>
                    <Col span={12}>
                    {
                        td.length === 0 ? null :
                        <div className='centerDiv' style={{width: '95%'}}>
                        <Card title={CardTitleLeft} bordered={false}>
                           
                            <div>
                            {
                                td.map(line => {
                                    return (
                                        <div className='flexDiv' key={line.key}>
                                            <Col span={9}>
                                                <Input type="text" value={line.label} onChange={(e) => this.labelChanged(line, e.target.value)} style={{width: '90%'}} />  

                                            </Col>
                                            <Col span={11}>
                                                <span>
                                                {
                                                    line.type === 'input' ?
                                                        <Input type="text" onChange={(e) => this.valueChanged(line, e.target.value)} value={line.value}/> : null
                                                }
                                                {
                                                    line.type === 'readonly' ?
                                                        <span value={line.key}>[{line.key}]</span> : null
                                                }
                                                {
                                                    line.type === 'list' ?
                                                        <div>[{line.key}]</div> : null
                                                }
                                                </span>
                                            </Col>
                                            <Col span={4}>
                                                {line.type === 'list' ?
                                                <Dropdown overlay={menu} trigger={['click']} placement="bottomCenter">
                                                <span onClick={() => this.changeFormat(line)} style={{cursor: 'pointer', color: 'red'}}>更改格式</span> 
                                                </Dropdown>
                                                : null}
                                                <Button onClick={() => this.delLine(line)} icon="delete" shape="circle" style={{marginLeft: '5px'}} size='small'></Button>
                                            </Col>
                                        </div>
                                    )
                                })
                            }
                            </div>
                            <div style={{clear: 'both'}}></div>
                            </Card>
                            </div>
                    }
                    </Col>
                   
                    <Col span={12}>
                    { tempateStr.length === 0 ? null :
                        <div className='centerDiv'>
                            <Card title={Cardtitle} bordered={false}>
                                <pre>
                                    {tempateStr}
                                </pre>
                            </Card>
                        </div>
                    }
                       
                        
                    </Col>
                </Row>
                {
                    this.props.type === 'dynamic' ? null : 
                    <Row>
                        <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                            <Button type="primary" htmlType="submit" style={{width: "8rem"}} 
                            disabled={this.props.buildInfo.isDisabled} 
                            loading={this.props.loadingState} 
                            onClick={this.handleSave}>保存</Button>
                            {rulesTemplateOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
                        </Col>
                    </Row>
                }
                <Modal title="选择行类型" visible={this.state.visible}  onOk={this.handleOkModal} onCancel={this.handleCancelModal} style={{textAlign: 'center'}}>
                     <Input addonAfter={selectAfter} style={{width: '85%', margin: '20px 0'}} 
                            placeholder='请输入默认值' 
                            onChange={this.defaultValueChange}
                            value={this.state.defaultValue}
                            disabled={this.state.disabled}/>
                </Modal>
            </Form >
        )
    }
}

function mapStateToProps(state) {
    return {
        basicData: state.basicData,
        buildInfo: state.building.buildInfo,
        operInfo: state.building.operInfo,
        loadingState: state.building.rulesTemplateloading,
        user: (state.oidc.user || {}).profile || {},
        templateData: state.building.templateData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

const WrappedForm = Form.create()(RulesTemplateEdit);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);