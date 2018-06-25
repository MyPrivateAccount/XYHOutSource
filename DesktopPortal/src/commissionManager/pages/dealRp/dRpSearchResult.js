import React, { Component } from 'react';
import { Layout, Row, Col ,Tooltip,Button} from 'antd';
import DealRpTable  from './dealRpTable'

class DRpSearchResult extends Component {
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
    onOpenDlg=(e)=>{
        if(this.props.onOpenDlg!==null){
            this.props.onOpenDlg(e)
        }
    }
    onOpenZy=(e)=>{
        if(this.props.onOpenZy!==null){
            this.props.onOpenZy(e)
        }
    }
    onOpenTy=(e)=>{
        if(this.props.onOpenTy!==null){
            this.props.onOpenTy(e)
        }
    }
    render() {
        return (
            <div>
                <Tooltip title="导出">
                    <Button type='primary' onClick={this.handleNew} style={{ 'margin': '10' }} >导出</Button>
                </Tooltip>
                <DealRpTable SearchCondition={this.props.cd} onRpTable={this.onRpTable} onOpenDlg={this.onOpenDlg} onOpenZy={this.onOpenZy} onOpenTy={this.onOpenTy} type='query'/>
            </div>
        )
    }
}
export default DRpSearchResult