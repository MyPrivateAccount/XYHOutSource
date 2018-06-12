const defaultPageSize = 10;

const SearchCondition = {

    ppFtListCondition: {
        pageIndex: 0,
        pageSize: defaultPageSize,
        keyWords: '',
        OrganizationId:''
    },
    orgParamListCondition: {
        branchId:''
    },
    incomeScaleListCondition:{
        pageIndex: 0,
        pageSize: defaultPageSize,
        keyWords: '',
        OrganizationId:'',//组织id
        rankPos:''//职位等级
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