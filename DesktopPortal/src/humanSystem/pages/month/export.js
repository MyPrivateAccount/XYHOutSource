import XLSX from 'xlsx';

//允许对角线合并
//subhead数组情况必须相同
const head = [//以第一层为基准, 必须平行
    {
        t: "s", 
        name: "新汇耀有限公司",//基本工资
        row: 1,
        subhead: [
            {t: "n", name: "序号", row: 2,},
            {t: "s", name: "身份证号", row: 2,},
            {t: "s", name: "工号", row: 2,},
            {t: "s", name: "姓名", row: 2,},
            {t: "s", name: "部门", row: 2,},
            {t: "s", name: "职位", row: 2,},
            {t: "n", name: "应出勤天数", row: 2,},
        ],
    },
    {
        t: "",
        name: "",
        row: 1,
        subhead: [
            {
                t: "s",
                name: "应发",
                row: 1,
                subhead: [
                    {t: "n", name: "基本工资",row: 1},
                    {t: "n", name: "交通补贴",row: 1},
                    {t: "n", name: "通信补贴",row: 1},
                    {t: "n", name: "其它补贴",row: 1},
                    {t: "n", name: "加班",row: 1},
                    {t: "n", name: "绩效奖励",row: 1},
                ],
            }
        ],
    },
    {
        t: "",
        name: "",
        row: 1,
        subhead: [
            {
                t: "s",
                name: "应扣",
                row: 1,
                subhead: [
                    {t: "n", name: "迟到", row: 1},
                    {t: "n", name: "事假", row: 1},
                    {t: "n", name: "旷工", row: 1},
                    {t: "n", name: "行政扣款", row: 1},
                    {t: "n", name: "端口扣款", row: 1},
                ],
            }
        ],
    },
    {
        t: "",
        name: "",
        row: 1,
        subhead: [
            {
                t: "s",
                row: 2,
                name: "应发合计",
            }
        ],
    },
    {
        t: "",
        name: "",
        row: 1,
        subhead: [
            {
                t: "s",
                row: 2,
                name: "意外险",
            }
        ],
    },
    {
        t: "",
        name: "",
        row: 1,
        subhead: [
            {
                t: "s",
                row: 2,
                name: "工作服",
            }
        ],
    },
    {
        t: "",
        name: "",
        row: 1,
        subhead: [
            {
                t: "s",
                name: "代扣",
                row: 1,
                subhead: [
                    {t: "n", name: "养老", row: 1},
                    {t: "n", name: "失业", row: 1},
                    {t: "n", name: "医疗", row: 1},
                    {t: "n", name: "工伤", row: 1},
                    {t: "n", name: "公积金", row: 1},
                ],
            }
        ],
    },
    {
        t: "",
        name: "",
        row: 1,
        subhead: [
            {
                t: "s",
                row: 2,
                name: "实发工资",
            }
        ],
    },
];

//以第一层为绝对
function findSubhead(head, lst, level) {
    let v = {s: {r:0,c:0}, e: {r:0,c:0}};
    let bm = false;
    if (head.subhead) {//subhead算横向，row算纵向
        if (head.subhead instanceof Array) {
            ;
        } else {
            findSubhead(head.subhead, lst, level+1);
        }
        
    }
    if (head.row > 1) {
        bm = true;
        v.s.r = level;
        v.e.r = level+head.row;
    }
    return true;
}
export function createMerge(head) {
    let lst = [];
    lst["sumcolum"] = 0;
    head.every(e => findSubhead(e, lst, 0));
    return lst;
}