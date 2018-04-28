const defaultPageSize = 10;

const SearchCondition = {
    myAudit: {
        waitAuditListCondition: {//待审核
            pageIndex: 0,
            pageSize: defaultPageSize,
            listType: "myAudit_wait"
        },
        auditedListCondition: {//我参与过的
            examineStatus: [],//[1],
            contentTypes: [],
            pageIndex: 0,
            pageSize: defaultPageSize,
            listType: "myAudit_audited"
        }
    },
    mySubmit: {//我提交的
        examineStatus: [1],
        contentTypes: [],
        pageIndex: 0,
        pageSize: defaultPageSize,
        listType: "mySubmit"
    },
    copyToMe: {//抄送我的(1:未读,2:已读)
        status: [],
        pageIndex: 0,
        pageSize: defaultPageSize,
        listType: "copyToMe"
    }

}

export default SearchCondition;