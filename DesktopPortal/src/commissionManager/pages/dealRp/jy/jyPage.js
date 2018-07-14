import React, {Component} from 'react'
import Layer from '../../../../components/Layer';
import JyPanel from './jyPanel'

class JyPage extends Component{

    state={
        canEdit:false,
        report:{},
        opType:'view'
    }

    componentWillMount=()=>{
        let initState = (this.props.location || {}).state || {};
        let canEdit  = false;
        let entity = initState.entity || {}
        if(initState.op === 'add' || initState.op=='edit'){
            canEdit= true
        }
        this.setState({ report: entity, opType: initState.op, list: initState.list||[], canEdit: canEdit })

    }

    

    render(){
        return (
            <Layer >
                <JyPanel  {...this.state}/>
            </Layer>
        )
    }
}

export default JyPage;