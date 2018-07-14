import React, { Component } from 'react';
import Layer from '../../../../components/Layer';
import FKPanel from './FKPanel'

class FKPage extends Component{

    state={
        canEdit:false,
        factGet:{},
        report: {},
        opType:'view'
    }

    componentWillMount=()=>{
        let initState = (this.props.location || {}).state || {};
        let canEdit  = false;
        let entity = initState.entity || {}
        if(initState.op === 'add' || initState.op=='edit'){
            let s = entity.status||0;
            if(s === 0 || s===16){
                canEdit = true;
            }
        }
        this.setState({ opType: initState.op, canEdit: canEdit,factGet: entity, report: entity.report||{} })

    }

    render (){
        return (
            <Layer>
                <FKPanel {...this.state}/>
            </Layer>
        )
    }
}


export default FKPage;