//离职人员业绩确认表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Layout, Row, Col, Spin, Checkbox } from 'antd'
import SearchCondition from './searchCondition';
import { searchLzryyjqrb,yjQrEmpResult} from '../../actions/actionCreator'

class LZRYTJTable extends Component {
    appTableColumns = [
        { title: '员工编号', dataIndex: 'userInfo.userName', key: 'userInfo.userName' },
        { title: '员工姓名', dataIndex: 'userInfo.trueName', key: 'userInfo.trueName' },
        { title: '归属组织', dataIndex: 'userInfo.orgFullname', key: 'userInfo.orgFullname' },
        { title: '成交报告编号', dataIndex: 'cjbgbh', key: 'cjbgbh' },
        { title: '业绩产生人', dataIndex: 'cjUserName', key: 'cjUserName' },
        { title: '成交日期', dataIndex: 'cjbgCjTime', key: 'cjbgCjTime' },
        { title: '上业绩日期', dataIndex: 'cjbgAuditTime', key: 'cjbgAuditTime' },
        { title: '离职日期', dataIndex: 'userInfo.lzDate', key: 'userInfo.lzDate' },
        { title: '业绩金额', dataIndex: 'distribute.ftJe', key: 'distribute.ftJe' },
        { title: '是否包含', dataIndex: 'isInclude', key: 'isInclude',render: (text, recored) =>(
            <Checkbox value={text} onChange={this.handleInclude(recored.key, 'isInclude')}/>
        ) },
        

    ];
    state = {
        isDataLoading: false,
        pagination: {},
        dataSource:{extension:[]},
        searchCondition:{}
    }
    handleInclude = (key, dataIndex) => {
        return (value) => {
            console.log("handleInclude:" + value)
            const dataSource = [...this.state.dataSource];
            const target = dataSource.find(item => item.key === key);
            if (target) {
                target['isInclude'] = value.target.checked;
                this.setState({ dataSource });
            }
        };
    }
    handleSearch = (e) => {
        this.setState({ isDataLoading: true ,searchCondition:e});
        this.props.dispatch(searchLzryyjqrb(e))
    }
    handleTableChange = (pagination, filters, sorter) => {
        let cd = {...this.state.searchCondition};
        cd.pageIndex = (pagination.current - 1);
        cd.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.handleSearch(cd)
    };
    getCheckEmps=()=>{
        var dt = []
        const dataSource = [...this.state.dataSource];
        for (var i = 0; i < dataSource.length; i++) {
            if(dataSource[i].isInclude)
            {
                dt[i] = dataSource[i]
            }
        }
        return dt;
    }
    componentWillReceiveProps(newProps) {
        console.log("new Props:" + newProps.dataSource)
        this.setState({ isDataLoading: false });
        if(newProps.dataSource){
            let paginationInfo = {
                pageSize: newProps.dataSource.pageSize,
                current: newProps.dataSource.pageIndex,
                total: newProps.dataSource.totalCount
            };
            console.log("分页信息：", paginationInfo);
            this.setState({ pagination: paginationInfo });
    
            this.setState({dataSource:newProps.dataSource})
        }
        if(newProps.ext){
            let paginationInfo = {
                pageSize: newProps.ext.pageSize,
                current: newProps.ext.pageIndex,
                total: newProps.ext.totalCount
            };
            console.log("分页信息：", paginationInfo);
            this.setState({ pagination: paginationInfo });
    
            this.setState({dataSource:newProps.ext})
        }

        if(newProps.operInfo.operType === 'FINA_QUERY_YJQR_EMP'){
            //月结页面查询勾选的确认业绩的员工
            let emps = this.getCheckEmps()
            if(emps.length>0){
                this.props.dispatch(yjQrEmpResult(emps))
            }
            newProps.operInfo.operType = ''
        }
    }
    render() {
        return (
            <Layout>
                <Layout.Content>
                    {
                        this.props.showSearch ? <Row style={{ margin: 10 }}>
                            <Col span={24}>
                                <SearchCondition handleSearch={this.handleSearch} orgPermission={'YJ_CW_LZRYYJQRB'}/>
                            </Col>
                        </Row> : null
                    }

                    <Row style={{ margin: 10 }}>
                        <Col span={24}>
                            <Spin spinning={this.state.isDataLoading}>
                                <Table columns={this.appTableColumns} dataSource={this.state.dataSource.extension} pagination={this.state.pagination} onChange={this.handleTableChange}></Table>
                            </Spin>
                        </Col>
                    </Row>
                </Layout.Content>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        searchCondition: state.fina.SearchCondition,
        operInfo:state.fina.operInfo,
        ext:state.fina.ext
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(LZRYTJTable);