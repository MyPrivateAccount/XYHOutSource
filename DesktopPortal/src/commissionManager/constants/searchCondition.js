const defaultPageSize = 10;

const SearchCondition = {

    ppFtListCondition: {
        pageIndex: 0,
        pageSize: defaultPageSize,
        keyWords: '',
        OrganizationId:''
    },
    orgParamListCondition: {
        pageIndex: 0,
        pageSize: defaultPageSize,
        keyWords: '',
        OrganizationId:''
    },
    incomeScaleListCondition:{
        pageIndex: 0,
        pageSize: defaultPageSize,
        keyWords: '',
        OrganizationId:'',//组织id
        rankPos:''//职位等级
    }
}

export default SearchCondition;