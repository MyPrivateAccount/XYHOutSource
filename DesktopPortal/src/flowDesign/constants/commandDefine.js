const NwfCommand = {
    GetWorkflowList: {//流程列表
        CommandName: "NWorkflow.GetWorkflowListRequest"
    },
    GetWorkflowDefine: {//流程详细定义
        CommandName: "NWorkflow.GetWorkflowDefineRequest",
        workflowID: '100'
    },
    GetStepList: {//获取所有流程步骤列表
        CommandName: "NWorkflow.GetFlowServiceListRequest"
    },
    GetSetpConfig: {//获取步骤配置信息
        CommandName: "NWorkflow.GetFlowServiceConfigRequest",
        serviceID: ''
    },
    SaveWorkflow: {//保存工作流
        CommandName: 'NWorkflow.SaveWorkflowDefineRequest',
        Workflow: ''
    },
    GetCommonParamList: {//获取通用参数配置
        CommandName: 'NWorkflow.GetParameterListRequest'
    },
    DeleteWorkFlow: {//删除流程
        CommandName: 'NWorkflow.DeleteWorkflowDefineRequest',
        WorkflowID: ''
    },
    ImportWorkFLow: {
        CommandName: 'NWorkflow.ImportWorkflowRequest',
        WorkflowList: ''
    }
}

export default NwfCommand;