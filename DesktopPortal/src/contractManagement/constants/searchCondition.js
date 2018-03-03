
const SearchCondition = {
    mySubmit: {//我提交的
        examineStatus: [1],
        contentTypes: ['TransferCustomer'],
        pageIndex: 0,
        pageSize: 10,
        listType: "mySubmit"
    },
    sourceCondition: {//调客归属部门条件
        type: 'source',
        pageIndex: 0,
        pageSize: 10000,
        userID: ''
    },
    targetCondition: {//调客目标部门条件
        type: 'target',
        pageIndex: 0,
        pageSize: 10000,
        userID: ''
    }
}

export default SearchCondition;