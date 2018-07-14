import React, {Component} from 'react'
import Layer from '../../../../components/Layer';
import TyPanel from './tyPanel'


class TyPage extends Component{

    state={
        canEdit:false,
        distribute:{},
        opType:'view'
    }

    componentWillMount=()=>{
        let initState = (this.props.location || {}).state || {};
        let canEdit  = false;
        let entity = initState.entity || {}
        if(initState.op === 'add' || initState.op=='edit'){
            let s = entity.status;
            if(s === 0 || s===16){
                canEdit = true;
            }
        }
        this.setState({ opType: initState.op, canEdit: canEdit,distribute: entity })

    }

    render(){
        return (
            <Layer>
                <TyPanel {...this.state}/>
            </Layer>
        )
    }
}

export default TyPage;