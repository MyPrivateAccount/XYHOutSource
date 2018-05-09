const defaultPageSize = 10;
const SearchCondition = {
    mySubmit: {//我提交的
        examineStatus: [1],
        contentTypes: ['TransferCustomer'],
        pageIndex: 0,
        pageSize: 10,
        listType: "mySubmit"
    },
    companyASearchCondition: {
        pageIndex: 0,
        pageSize: defaultPageSize,
        keyWord: '',
        searchType:'',
        address:'',
        type:'',
    },
    contractSearchCondition: {
        keyWord: '',
        checkStatu: null,//审核状态
        //organizationName: [],//
        createDateStart: null,//录入时间
        createDateEnd: null,
        //isOverTime:false,
        type:'',
        discard:0,
        follow:0,
        orderRule: 0,
        pageIndex: 0,
        pageSize: 5
    }
}

export default SearchCondition;