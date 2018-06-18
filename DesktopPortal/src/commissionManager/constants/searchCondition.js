const defaultPageSize = 10;

const SearchCondition = {

    ppFtListCondition: {
        pageIndex: 0,
        pageSize: defaultPageSize,
        keyWords: '',
        OrganizationIds:[]
    },
    orgParamListCondition: {
        branchId:''
    },
    incomeScaleListCondition:{
        branchId:'',
        code:''
    },
    acmentListCondition:{
        branchId:''
    },
    rpListCondition:{
        pageIndex: 0,
        pageSize: defaultPageSize,
    }
}

export default SearchCondition;