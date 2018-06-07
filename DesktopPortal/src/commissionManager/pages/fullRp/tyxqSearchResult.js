import React, { Component } from 'react';
import TyxqTable  from './tyxqTable'

class TyxqSearchResult extends Component {
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
                <TyxqTable SearchCondition={this.props.cd}  onRpTable={this.onRpTable}/>
            </div>
        )
    }
}
export default TyxqSearchResult