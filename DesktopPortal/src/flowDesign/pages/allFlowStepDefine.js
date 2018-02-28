import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Menu, Icon } from 'antd'

const SubMenu = Menu.SubMenu;
//流程步骤定义列表
class AllFlowStepDefine extends Component {
    state = {

    }
    handFlowDrag = (event, step) => {
        console.log("开始拖动：", step);
        event.dataTransfer.setData("dropStep", JSON.stringify(step));
    }
    render() {
        const allFlowSteps = this.props.allDefineFlowSteps || [];
        const draggAble = this.props.activeFlowDefine.workflowID ? true : false;
        return (
            <Menu mode="inline" style={{ width: '100%', height: '100%' }} selectable={false}>
                {
                    allFlowSteps.map((step, i) => <SubMenu key={i} title={<span><Icon type="bars" /><span>{step.category}</span></span>}>
                        {
                            step.children.map((item) => <Menu.Item key={item.id} value={item.id}><div onDragStart={(e) => this.handFlowDrag(e, item)} draggable={draggAble}><Icon type="share-alt" />{item.name}</div></Menu.Item>)
                        }
                    </SubMenu>
                    )
                }
            </Menu>)
    }
}

function mapStateToProps(state) {
    return {
        allDefineFlowSteps: state.flowChart.allDefineFlowSteps,
        activeFlowDefine: state.flowChart.activeFlowDefine
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AllFlowStepDefine);