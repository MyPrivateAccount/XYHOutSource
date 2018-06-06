import React, { Component } from 'react';
import {Tooltip,Button} from 'antd';
import FyxqTable  from './fyxqTable'

class FyxqSearchResult extends Component {
    constructor(props) {
        super(props);
        this.state={
        }
    }
    componentDidMount() {
        this.props.onRSTableRef(this)
    }
    onRpTable=(ref)=> {
        this.rptb = ref
    }
    handleSearch=(cd)=>{
        this.rptb.handleSearch(cd)
    }
    render() {
        return (
            <div>
                <Tooltip title="导出">
                    <Button type='primary' onClick={this.handleNew} style={{ 'margin': '10' }} >导出</Button>
                </Tooltip>
                <FyxqTable SearchCondition={this.props.cd}  onRpTable={this.onRpTable}/>
            </div>
        )
    }
}
export default FyxqSearchResult