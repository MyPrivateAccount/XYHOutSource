import React, { Component } from 'react';
import YjtzhzTable  from './yjtzhzTable'

class YjtzhzSearchResult extends Component {
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
                <YjtzhzTable SearchCondition={this.props.cd}  onRpTable={this.onRpTable}/>
            </div>
        )
    }
}
export default YjtzhzSearchResult