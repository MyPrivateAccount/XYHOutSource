import React, { Component } from 'react';
import { Layout, Row, Col ,Tooltip,Button} from 'antd';
import DealRpTable  from './dealRpTable'

class DRpSearchResult extends Component {
    render() {
        return (
            <div>
                <Tooltip title="查询">
                    <Button type='primary' onClick={this.handleNew} style={{ 'margin': '10' }} >查询</Button>
                </Tooltip>
                <Tooltip title="重置">
                    <Button type='primary' onClick={this.handleNew} style={{ 'margin': '10' }} >重置</Button>
                </Tooltip>
                <Tooltip title="导出">
                    <Button type='primary' onClick={this.handleNew} style={{ 'margin': '10' }} >导出</Button>
                </Tooltip>
                <DealRpTable/>
            </div>
        )
    }
}
export default DRpSearchResult