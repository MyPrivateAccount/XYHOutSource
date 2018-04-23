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
    }
}

export default SearchCondition;