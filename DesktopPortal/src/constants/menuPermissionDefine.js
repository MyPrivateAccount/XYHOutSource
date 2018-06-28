//权限定义
const ToolMenuPermissionDefine = [
    {//权限中心
        id: 'privilegeManager',
        permissions: [{id: 'ApplicationCreate', name: '创建应用'},
        {id: 'PermissionItemCreate', name: '创建权限项'}]
    },
    {//客源管理
        id: 'customerManager',
        permissions: []
    },
    {//驻场工具
        id: 'houseSource',
        permissions: [{id: 'APPOINT_SCENE', name: '指派驻场'}]
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
            {id: 'RECOMMEND_REGION', name: '大区推荐'},
            {id: 'RECOMMEND_FILIALE', name: '公司推荐'}]
    },
    {//合同管理
        id: 'contractManagement',
        permissions: [
            {id: 'RECORD_FUC', name: '录入'},
            {id: 'UPLOAD_FILE', name: '上传附件'},
            {id: 'EXPORT_CONTRACT', name: '导出'},
            {id: 'EXPORT_ALL_CONTRACT', name: '全部导出'},
            {id: 'COMPANYA_MANAGE', name: '甲方管理'},
            {id: 'INVALIDATE_CONTRACT', name: '合同作废'},
            {id: 'HT_BCXY', name: '补充协议'},
            {id: 'HT_ADD_JF', name: '新增甲方'},
            {id: 'HT_JF_DELETE', name: '删除甲方'},
        ]
    },
    {//人事系统
        id: 'HumanIndex',
        permissions: [
            {id: 'SALARY_VIEW', name: '薪资构成查看'},
            {id: 'SOCIAL_SECURITY_VIEW', name: '社保信息查看'}
        ]
    },
    {//佣金系统
        id:'commissionManager',
        permissions:[
            {id:'YJ_YJFTSZ_CK',name:'业绩分摊项设置查看'},
            {id:'YJ_YJFTSZ',name:'业绩分摊项设置'},
            {id:'YJ_RSFTZZSZ_CK',name:'人数分摊组织设置查看'},
            {id:'YJ_RSFTZZSZ',name:'人数分摊组织设置'},
            {id:'YJ_SZ_TCBLSZ_CK',name:'提成比例设置查看'},
            {id:'YJ_SZ_TCBLSZ',name:'提成比例设置'},
            {id:'YJ_ZZCSSZ_CK',name:'组织参数设置查看'},
            {id:'YJ_ZZCSSZ',name:'组织参数设置'},
        ]

    }
]

export default ToolMenuPermissionDefine;