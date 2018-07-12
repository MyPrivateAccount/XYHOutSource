
// export const SearchHumanTypes = [
//     { value: 0, label: '不限'},
//     { value: 1, label: '未入职'},
//     { value: 2, label: '在职'},
//     { value: 3, label: '离职'},
//     { value: 4, label: '黑名单'}
// ]

export const MonthListColums = [
    { title: '上一月结时间', dataIndex: 'last', key: 'last' },
    { title: '月结时间', dataIndex: 'monthtime', key: 'monthtime' },
    { title: '操作人', dataIndex: 'operater', key: 'operater' }
]

// export const AgeRanges = [
//     { value: 0, label: '不限'}, 
//     { value: 1, label: '20岁以上'},
//     { value: 2, label: '30岁以上'},
//     { value: 3, label: '40岁以上'}
// ]

//前公司列表
export const formerCompanyColumns = [{
    title: '所在单位',
    dataIndex: 'company',
    key: 'company',
}, {
    title: '岗位',
    dataIndex: 'position',
    key: 'position',
}, {
    title: '起始时间',
    dataIndex: 'startTime',
    key: 'startTime',
}, {
    title: '终止时间',
    dataIndex: 'endTime',
    key: 'endTime',
}, {
    title: '证明人',
    dataIndex: 'witness',
    key: 'witness',
}, {
    title: '证明人联系电话',
    dataIndex: 'witnessPhone',
    key: 'witnessPhone',
}];
//学历信息
export const educationColumns = [
    {
        title: '学历',
        dataIndex: 'education',
        key: 'education',
    },
    {
        title: '所学专业',
        dataIndex: 'major',
        key: 'major',
    },
    {
        title: '学习形式',
        dataIndex: 'learningType',
        key: 'learningType',
    },
    {
        title: '毕业证书',
        dataIndex: 'graduationCertificate',
        key: 'graduationCertificate',
    },
    {
        title: '入学时间',
        dataIndex: 'enrolmentTime',
        key: 'enrolmentTime',
    },
    {
        title: '毕业时间',
        dataIndex: 'graduationTime',
        key: 'graduationTime',
    },
    {
        title: '获得学位',
        dataIndex: 'getDegree',
        key: 'getDegree',
    },
    {
        title: '学位授予时间',
        dataIndex: 'getDegreeTime',
        key: 'getDegreeTime',
    },
    {
        title: '学位授予单位',
        dataIndex: 'getDegreeCompany',
        key: 'getDegreeCompany',
    },
    {
        title: '毕业学校',
        dataIndex: 'graduationSchool',
        key: 'graduationSchool',
    }
];
