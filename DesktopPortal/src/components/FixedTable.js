import React ,{Component} from 'react'
import {Table} from 'antd'
import ResizeAware from 'react-resize-aware';

class FixedTable extends Component{

    state={
        height: 0
    }
    
    sizeChanged=({width,height})=>{
        let offset = 80;
        if( this.props.offset){
            offset = this.props.offset;
        }
        this.setState({height:height-offset});
    
    }

    render(){
        
        return (
            <ResizeAware className="relative" onResize={this.sizeChanged}>
            <Table {...this.props} scroll={{ y: this.state.height }}>
            </Table>   
            </ResizeAware>
        )
    }
}


export default FixedTable;
