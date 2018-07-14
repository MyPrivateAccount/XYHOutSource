import React, {Component} from 'react'
import Layer from '../../../../components/Layer';
import ZyPanel from './zyPanel'

class ZyPage extends Component{

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
        this.setState({ report: entity, opType: initState.op, canEdit: canEdit })

    }

    onCancel = ()=>{
        if(this.props.onCancel){
            this.props.onCancel();
        }
    }

    onOk = ()=>{
        if(this.props.onOk && this.zyForm){
            let values = this.zyForm.getValues();
            if(!values){
                return;
            }
            this.props.onOk(values);
        }
    }

    render(){
        return (
            <Layer >
                <ZyPanel ref={(ins)=>this.zyForm = ins} {...this.state}/>
            </Layer>
        )
    }
}

export default ZyPage;