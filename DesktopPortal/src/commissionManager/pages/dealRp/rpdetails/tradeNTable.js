//内部分配表格
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Select, Table, Button, Tooltip, Input } from 'antd'

class TradeNTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
            dataSource: [],
            count: 0,
            nbfpItems: [],
            totalyj: 0,
            humanList: []
        }
        this.onCellChange = this.onCellChange.bind(this)
    }
    appTableColumns = [
        {
            title: '部门', dataIndex: 'sectionName', key: 'sectionName',
            render: (text, recored) => (
                <span>
                    <Input style={{ width: 80 }} />
                </span>
            )
        },
        {
            title: '员工', dataIndex: 'username', key: 'username',
            render: (text, recored) => (
                <Select style={{ width: 80 }} onChange={this.onHumanChange(recored.key, 'username')}>
                    {
                        text.map(tp => <Select.Option key={tp.name} value={tp.name}>{tp.name}</Select.Option>)
                    }
                </Select>
            )
        },
        {
            title: '工号', dataIndex: 'workNumber', key: 'workNumber',
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
                    <Input style={{ width: 50 }} value={text} onChange={this.onMoneyEdit(recored.key, 'money')} />
                </span>
            )
        },
        {
            title: '比例', dataIndex: 'percent', key: 'percent',
            render: (text, recored) => (
                <span>
                    <Input style={{ width: 50 }} value={text} onChange={this.onPerEdit(recored.key, 'percent')} />%
                </span>
            )
        },
        {
            title: '单数', dataIndex: 'oddNum', key: 'oddNum',
            render: (text, recored) => (
                <span>
                    <Input style={{ width: 80 }} />
                </span>
            )
        },
        {
            title: '身份', dataIndex: 'type', key: 'type',
            render: (text, recored) => (
                <span>
                    <Select style={{ width: 80 }} onChange={this.onCellChange(recored.key, 'type')}>
                        {
                            text.map(tp => <Select.Option key={tp.name} value={tp.name}>{tp.name}</Select.Option>)
                        }
                    </Select>
                </span>
            )
        },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

                    <Tooltip title='删除'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleDelete(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    componentWillMount() {
    }
    componentDidMount() {
        this.props.onFpTableRef(this)
        if (this.props.dataSource !== null && this.props.dataSource.length !== 0) {
            let newList = this.props.dataSource;
            for (let i = 0; i < newList.length; i++) {
                const { count, dataSource } = this.state;
                const newData = {
                    key: count,
                    sectionName: newList[i].sectionName,
                    username: newList[i].username,
                    workNumber: newList[i].workNumber,
                    money: newList[i].money,
                    percent: newList[i].percent,
                    oddNum: newList[i].oddNum,
                    type: newList[i].type,
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
    getPercent(key) {
        let items = this.state.nbfpItems;
        console.log("getPercent:" + key)
        for (let i = 0; i < items.length; i++) {
            if (items[i].name === key) {
                console.log(items[i].percent)
                return items[i].percent
            }
        }
        return 0
    }
    setNbfpItems = (items) => {
        this.setState({ nbfpItems: items })
    }
    setHumanList = (items) => {
        this.setState({ humanList: items })
    }
    //设置总佣金
    setZyj = (yj) => {
        this.setState({ totalyj: yj })
    }
    //选择了款项类型
    onCellChange = (key, dataIndex) => {
        return (value) => {
            console.log("key" + key)
            console.log("dataIndex" + dataIndex)
            const dataSource = [...this.state.dataSource];
            const target = dataSource.find(item => item.key === key);
            if (target) {
                target['money'] = this.getPercent(value) * (this.state.totalyj == null ? 0 : this.state.totalyj);
                target['percent'] = this.getPercent(value) * 100;
                target['selectType'] = value
                this.setState({ dataSource });
            }
        };
    }
    //选择了员工，需要自动填充部门和员工id
    onHumanChange = (key, dataIndex) => {
        return (value) => {
            const dataSource = [...this.state.dataSource];
            const target = dataSource.find(item => item.key === key);
            if (target) {
                target['selectUser'] = value
                this.setState({ dataSource });
            }
        }
    }
    //编辑了金额
    onMoneyEdit = (key, dataIndex) => {
        return (value) => {
            console.log("onMoneyEdit:" + value)
            const dataSource = [...this.state.dataSource];
            const target = dataSource.find(item => item.key === key);
            if (target) {
                target['money'] = value.target.value;
                this.setState({ dataSource });
            }
        };
    }
    //编辑了比例
    onPerEdit = (key, dataIndex) => {
        return (value) => {
            console.log("onPerEdit:" + value)
            const dataSource = [...this.state.dataSource];
            const target = dataSource.find(item => item.key === key);
            if (target) {
                target['percent'] = value.target.value;
                target['money'] = ((value.target.value) / 100) * (this.state.totalyj == null ? 0 : this.state.totalyj);
                this.setState({ dataSource });
            }
        };
    }
    //新增
    handleAdd = () => {
        const { count, dataSource } = this.state;
        const newData = {
            key: count,
            sectionName: "",
            username: this.state.humanList,
            selectUser:'',
            workNumber: "",
            money: 0,
            percent: 0,
            oddNum: 0,
            type: this.state.nbfpItems,
            selectType: '',
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
            dataSource.splice(target, 1)
            this.setState({ dataSource });
        }
    }
    //获取表格数据
    getData = (id) => {
        var dt = []
        const dataSource = [...this.state.dataSource];
        for (var i = 0; i < dataSource.length; i++) {
            let temp = {}
            temp.id = id
            temp.sectionName = dataSource[i].sectionName
            temp.username = dataSource[i].selectUser
            temp.workNumber = dataSource[i].workNumber
            temp.money = dataSource[i].money
            temp.percent = dataSource[i].percent / 100
            temp.oddNum = dataSource[i].oddNum
            temp.type = dataSource[i].selectType

            dt[i] = temp
        }
        return dt;
    }
    render() {
        const { dataSource } = this.state
        return (
            <Table bordered columns={this.appTableColumns} dataSource={dataSource}></Table>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        operInfo: state.rp.operInfo,
        ext: state.rp.ext
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(TradeNTable);