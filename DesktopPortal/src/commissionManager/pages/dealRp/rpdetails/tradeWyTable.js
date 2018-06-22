//外佣表格组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button, Tooltip, Input, Select } from 'antd'
import { getDicParList} from '../../../actions/actionCreator'
import EditableCell from './tradeEditCell'

class TradeWyTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
            dataSource: [],
            count: 0,
            kxlxItems: [],
            sfdxItems: [],
            totalyj: 0
        }
        this.onCellChange = this.onCellChange.bind(this)
        this.onCellChange2 = this.onCellChange2.bind(this)
    }
    appTableColumns = [
        {
            title: '款项类型', dataIndex: 'moneyType', key: 'moneyType',
            render: (text, recored) => (
                <span>
                    <Select style={{ width: 80 }} onChange={this.onCellChange(recored.key, 'moneyType')}>
                        {
                            text.map(tp => <Select.Option key={tp.name} value={tp.name}>{tp.name}</Select.Option>)
                        }
                    </Select>
                </span>
            )
        },
        {
            title: '收付对象', dataIndex: 'object', key: 'object',
            render: (text, recored) => (
                <span>
                    <Select style={{ width: 80 }} onChange={this.onCellChange2(recored.key, 'object')}>
                        {
                            text.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                        }
                    </Select>
                </span>
            )
        },
        {
            title: '备注', dataIndex: 'remark', key: 'remark',
            render: (text, recored) => (
                <span>
                    <Input style={{ width: 80 }} />
                </span>
            )
        },
        {
            title: '金额', dataIndex: 'money', key: 'money',
            render: (text, recored) => (
                <span>
                    <Input onChange={this.onMoneyEdit(recored.key,'money')} style={{ width: 80 }} value={text} />
                </span>
            )
        },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

                    <Tooltip title='删除'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleDelete(recored)}/>
                    </Tooltip>
                </span>
            )
        }
    ];
    componentWillMount() {
        this.setState({ isDataLoading: true, tip: '信息初始化中...' })
        this.props.dispatch(getDicParList(['COMMISSION_FP_SFDX']));
    }
    componentDidMount() {
        this.props.onWyTableRef(this)
        if(this.props.dataSource!==null&&this.props.dataSource.length!==0){
            let newList = this.props.dataSource;
            for(let i=0;i<newList.length;i++){
                const { count, dataSource } = this.state;
                const newData = {
                    key: count,
                    moneyType: newList[i].moneyType,
                    object: newList[i].object,
                    remark: newList[i].remark,
                    money: newList[i].money,
                };
                this.setState({
                    dataSource: [...dataSource, newData],
                    count: count + 1,
                });
            }
        }
    }
    componentWillReceiveProps(newProps) {
    }
    //编辑了金额
    onMoneyEdit = (key, dataIndex)=>{
        return (value) => {
            console.log("onMoneyEdit:" + value)
            const dataSource = [...this.state.dataSource];
            const target = dataSource.find(item => item.key === key);
            if (target) {
                target['money'] = value.target.value;
                this.setState({ dataSource });
                this.props.onCountJyj()
            }
        };
    }
    //选择了款项类型
    onCellChange = (key, dataIndex) => {
        return (value) => {
            console.log("key" + key)
            console.log("dataIndex" + dataIndex)
            const dataSource = [...this.state.dataSource];
            const target = dataSource.find(item => item.key === key);
            if (target) {
                console.log("onCellChange: totalyj>>"+this.state.totalyj)
                console.log("onCellChange: getPercent>>"+this.getPercent(value))
                target['money'] = this.getPercent(value) * (this.state.totalyj == null ? 0 : this.state.totalyj);
                target['selectMoneyType'] = value
                this.setState({ dataSource });
                this.props.onCountJyj()
            }
        };
    }
    onCellChange2 = (key, dataIndex) => {
        return (value) => {
            console.log("key" + key)
            console.log("dataIndex" + dataIndex)
            const dataSource = [...this.state.dataSource];
            const target = dataSource.find(item => item.key === key);
            if (target) {
                target['selectObject'] = value
                this.setState({ dataSource });
            }
        };
    }
    setKxlxItems=(items)=>{
        this.setState({kxlxItems:items})
    }
    //新增
    handleAdd = () => {
        const { count, dataSource } = this.state;
        const newData = {
            key: count,
            moneyType: this.state.kxlxItems,
            object: this.props.basicData.sfdxTypes,
            selectMoneyType: '',
            selectObject: '',
            remark: "",
            money: 0,
            edit: ""
        };
        this.setState({
            dataSource: [...dataSource, newData],
            count: count + 1,
        });
        console.log("datasource:" + this.state.dataSource)
    }
    //删除
    handleDelete = (info) => {
        const dataSource = [...this.state.dataSource];
        const target = dataSource.find(item => item.key === info.key);
        if (target) {
            dataSource.splice(target,1)
            this.setState({ dataSource });
            this.props.onCountJyj()
        }
    }
    //设置总佣金
    setZyj = (yj) => {
        this.setState({ totalyj: yj })
    }
    //获取总的外佣
    getTotalWyj = () => {
        let Wyj = 0;
        const dataSource = [...this.state.dataSource];
        for (var i = 0; i < dataSource.length; i++) {
            Wyj = Wyj + dataSource[i].money
        }
        return Wyj;

    }
    //获取表格数据
    getData = (id) => {
        var dt = [];
        const dataSource = [...this.state.dataSource];
        for (var i = 0; i < dataSource.length; i++) {
            let temp = { id: id, moneyType: dataSource[i].selectMoneyType, money: dataSource[i].money, object: dataSource[i].selectObject, remark: dataSource[i].remark }
            dt[i] = temp
        }
        return dt;
    }
    getPercent(key) {
        let items = this.state.kxlxItems;
        console.log("getPercent:" + key)
        for (let i = 0; i < items.length; i++) {
            if (items[i].name === key) {
                console.log(items[i].percent)
                return items[i].percent
            }
        }
        return 0
    }
    render() {
        const { dataSource } = this.state;
        return (
            <Table bordered columns={this.appTableColumns} dataSource={dataSource}></Table>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        operInfo: state.acm.operInfo,
        ext: state.rp.ext,
        scaleSearchResult:state.acm.scaleSearchResult
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(TradeWyTable);