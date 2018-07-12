


export const permission = {
    parSet: 'YJ_ZZCSSZ',
    parSetQuery: 'YJ_ZZCSSZ_CK',
    allocationSet:'YJ_YJFTSZ',
    allocationQuery:'YJ_YJFTSZ_CK',
    ruleSet:'YJ_TCBLSZ',
    ruleQuery:'YJ_SZ_TCBLSZ_CK',
    shareSet:'YJ_RSFTZZSZ',
    shareQuery:'YJ_RSFTZZSZ_CK',
    shareBranch: 'YJ_SZ_KXFTZZ',
    
    myReport: 'YJ_CJ_WD',
    nyftPepole:'YJ_CJ_NYFT', //内容分摊人
    reportQuery: 'YJ_CJ_CX',

    monthlyClosing: 'YJ_CW_YJ',
    ryftTable: 'YJ_CW_RY_QUERY',
    yftcTable:'YJ_CW_YFTCB',
    sftcTable: 'YJ_CW_SFTCB',
    tccbTable:'YJ_CW_TCCBB',
    yfcbcjTable:'YJ_CW_YFTCCJB',
    lzryyjqrTable: 'YJ_CW_LZRYYJQRB',
    sfkjqrTable: 'YJ_CW_SFKJQRB',

    fyxqTable:'YJ_BB_FYXQB',
    yjtzmxhzTable:'YJ_BB_YJTZMXHZB',
    tymxTable: 'YJ_BB_TYMXB',

    cjfhxz: 'YJ_CJ_FHXZ',
    op_zf: 'YJ_CJ_OP_ZF', //作废
    op_sk: 'YJ_CJ_OP_SK', //收款
    op_fk: 'YJ_CJ_OP_FK', //付款
    op_ty: 'YJ_CJ_OP_TY', //调佣
    op_zy: 'YJ_CJ_OP_ZY', //转移
    op_jy: 'YJ_CJ_OP_JY', //结佣

}

export const dicKeys = {
    wyfl: 'COMMISSION_BSWY_CATEGORIES',
    cjbglx: 'COMMISSION_CJBG_TYPE', 
    jylx: 'COMMISSION_JY_TYPE', 
    fkfs: 'COMMISSION_PAY_TYPE', 
    xmlx: 'COMMISSION_PROJECT_TYPE', 
    htlx: 'COMMISSION_CONTRACT_TYPE', 
    cqlx: 'COMMISSION_OWN_TYPE', 
    xxjylx: 'COMMISSION_TRADEDETAIL_TYPE', 
    zjjg: 'COMMISSION_SFZJJG_TYPE',
    sfxycqjybgj: 'COMMISSION_SFXYCQJYBGJ',

    wylx:'COMMISSION_WY_WYLX',  //物业类型
    kjlx:'COMMISSION_WY_KJLX', //空间类型
    jj:'COMMISSION_WY_ZXJJ', //家具
    zxzk: 'COMMISSION_WY_ZXZK', //装修状况
    cx: 'SHOP_TOWARD', //朝向
    htsc:'COMMISSION_YZ_QHTSC', //
    khxz:'COMMISSION_KH_KHXZ', //客户性质
    khly:'CUSTOMER_SOURCE',

    sfdx:'COMMISSION_FP_SFDX', //收付对象
    yjfj: 'COMMISSION_FJ_TYPES' //附件类型

}

export const branchPar= {
    showBb: 'MUST_SELECT_REPORT_INFO'
}

export const examineStatusMap = {
    0: '未提交',
    1: '审核中',
    8: '审核通过',
    16: '驳回'
}

export const reportOperateAction = {
    zf: {action:'ZF', key:'zf', text: '作废'},
    sk: {action:'SK', key:'sk',text: '收款'},
    fk: {action:'FK', key:'fk',text: '付款'},
    ty: {action:'TY', key:'ty',text: '调佣'},
    zy: {action:'ZY', key:'zy',text: '转移'},
    jy: {action:'JY', key:'jy',text: '结佣'}
}