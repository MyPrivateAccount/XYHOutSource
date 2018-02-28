const defaultPageSize = 10;

const SearchCondition = {

    empListCondition: {
        pageIndex: 0,
        pageSize: defaultPageSize,
        keyWords: '',
        OrganizationIds: []
    },
    empSearchCondition: {
        pageIndex: 0,
        pageSize: defaultPageSize,
        keyWords: '',
        OrganizationIds: []
    }
}

export default SearchCondition;