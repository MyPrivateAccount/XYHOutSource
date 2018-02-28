//权限定义
const ToolMenuPermissionDefine = [
    {//权限中心
        id: 'privilegeManager',
        permissions: [{ id: 'ApplicationCreate', name: '创建应用' },
        { id: 'PermissionItemCreate', name: '创建权限项' }]
    },
    {//客源管理
        id: 'customerManager',
        permissions: []
    },
    {//驻场工具
        id: 'houseSource',
        permissions: [{ id: 'APPOINT_SCENE', name: '指派驻场' }]
    },
    // {//房源检索
    //     id: 'searchTool',
    //     permissions: [
    //         { id: 'RECOMMEND_REGION', name: '大区推荐' },
    //         { id: 'RECOMMEND_FILIALE', name: '公司推荐' }]
    // },
    {//房源检索
        id: 'recommendTool',
        permissions: [
            { id: 'RECOMMEND_REGION', name: '大区推荐' },
            { id: 'RECOMMEND_FILIALE', name: '公司推荐' }]
    }
]

export default ToolMenuPermissionDefine;