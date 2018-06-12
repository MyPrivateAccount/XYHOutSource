import XLSX from 'xlsx';

//允许对角线合并
//subhead数组情况必须相同
const AllHead = [//以第一层为基准, 必须平行
    {
        t: "s", 
        v: "新汇耀有限公司",//基本工资
        s: { 
            fill : {
                fgColor : {
                    theme : 8,
                    tint : 0.3999755851924192,
                    rgb : '08CB26'
                }
            },
            font : {
                color : {
                    rgb : "FFFFFF"
                },
                bold : true
            },
        },
        row: 1,
        col: 7,
        subhead: [
            {t: "s", v: "序号", row: 2, col: 1,},
            {t: "s", V: "身份证号", row: 2,col: 1,},
            {t: "s", v: "工号", row: 2,col: 1,},
            {t: "s", v: "姓名", row: 2,col: 1,},
            {t: "s", v: "部门", row: 2,col: 1},
            {t: "s", v: "职位", row: 2,col: 1},
            {t: "s", v: "应出勤天数", row: 2,col: 1},
        ],
    },
    {
        t: "",
        v: "",
        row: 1,
        col: 6,
        subhead: [
            {
                t: "s",
                v: "应发",
                row: 1,
                col: 6,
                subhead: [
                    {t: "s", v: "基本工资",row: 1, col: 1},
                    {t: "s", v: "交通补贴",row: 1, col: 1},
                    {t: "s", v: "通信补贴",row: 1, col: 1},
                    {t: "s", v: "其它补贴",row: 1, col: 1},
                    {t: "s", v: "加班",row: 1, col: 1},
                    {t: "s", v: "绩效奖励",row: 1, col: 1},
                ],
            }
        ],
    },
    {
        t: "",
        v: "",
        row: 1,
        col: 5,
        subhead: [
            {
                t: "s",
                v: "应扣",
                row: 1,
                col: 5,
                subhead: [
                    {t: "s", v: "迟到", row: 1, col: 1},
                    {t: "s", v: "事假", row: 1, col: 1,},
                    {t: "s", v: "旷工", row: 1, col: 1},
                    {t: "s", v: "行政扣款", row: 1, col: 1,},
                    {t: "s", v: "端口扣款", row: 1, col: 1,},
                ],
            }
        ],
    },
    {
        t: "",
        v: "",
        row: 1,
        col: 1,
        subhead: [
            {
                t: "s",
                row: 2,
                col: 1,
                v: "应发合计",
            }
        ],
    },
    {
        t: "",
        v: "",
        row: 1,
        col: 1,
        subhead: [
            {
                t: "s",
                row: 2,
                col: 1,
                v: "意外险",
            }
        ],
    },
    {
        t: "",
        v: "",
        row: 1,
        col: 1,
        subhead: [
            {
                t: "s",
                row: 2,
                col: 1,
                v: "工作服",
            }
        ],
    },
    {
        t: "",
        v: "",
        row: 1,
        col: 5,
        subhead: [
            {
                t: "s",
                v: "代扣",
                row: 1,
                col: 5,
                subhead: [
                    {t: "s", v: "养老", row: 1, col: 1,},
                    {t: "s", v: "失业", row: 1, col: 1,},
                    {t: "s", v: "医疗", row: 1, col: 1,},
                    {t: "s", v: "工伤", row: 1, col: 1,},
                    {t: "s", v: "公积金", row: 1, col: 1,},
                ],
            }
        ],
    },
    {
        t: "",
        v: "",
        row: 1,
        col: 1,
        subhead: [
            {
                t: "s",
                row: 2,
                col: 1,
                v: "实发工资",
            }
        ],
    },
];
const Letter = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K",
"L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
"AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM",
"AN", "AO", "AP", "AQ", "AR", "AS", "AT"];
//以第一层为绝对
function findSubhead(head, lst, r, c) {
    let mv = {s: {r:0,c:0}, e: {r:0,c:0}};
    let bm = false;
    let cm = c;

    (r > lst.level) && (lst.level = r);
    (c > lst.clen)&& (lst.clen = c);

    if (head.subhead) {
        head.subhead.every((e, i) => findSubhead(e, lst, r+1, i>0?(cm+=head.subhead[i-1].col):cm));
    }
    if (head.row > 1) {
        bm = true;
    }
    mv.s.r = r;//少一个，0计数
    mv.e.r = r+head.row-1;
    if (head.col > 1) {
        bm = true;
    }
    mv.s.c = c;
    mv.e.c = c+head.col-1;

    if (bm) {
        lst.push(mv);
    }

    if (head.t && head.v) {
        let h = lst["head"];
        let pos = Letter[mv.s.c]+(mv.s.r+1);

        let {t, v, s} = head;
        h[pos] = {t, v, s};
    }

    return true;
}

export function createMergeHead(head) {
    let lst = [];
    lst["head"] = {};
    lst["level"] = 0;//层级
    lst["clen"] = 0;//行数
    lst["row"] = {};

    let num = 0;
    head.every((e, i) => findSubhead(e, lst, 0, i>0?(num+=head[i-1].col):0));
    return lst;
}

export function insertColum(head, data) {
    let row = head.level+2;
    let ret = [];
    
    for (const itm of data) {
        let i = 0;
        let va = {};
        for (const key in itm) {
            if (itm.constructor === String) {
                va[Letter[i++]+row] = {t: 's', v: itm[key]};
            } else {
                va[Letter[i++]+row] = {t: 'n', v: itm[key]};
            }
        }
        ret.push(va);
        row++;
    }

    return ret;
}

export function writeFile(merge, data) {
    let na = "A1:"+Letter[merge.clen+1]+(data.length+merge.level+1);
    let obj = {};
    for (const it of data) {
        obj = Object.assign(obj, it);
    }
    let sh = {
        "!ref": na,
        ...merge.head,
        ...obj,
        "!merges":[...merge],
    };
    XLSX.writeFile({
        SheetNames:["工资表"],
        Sheets: {
            工资表: sh
        }
    }, 'test.xlsx');
}

export function Test() {
    let f = createMergeHead(AllHead);
    let ret = insertColum(f, [{a:1,b:1,c:1,d:1,e:1,f:1,g:1,h:1,i:1,j:1,k:1,l:1,m:1,n:1,
    o:1,p:1,q:1,r:1,s:1,t:1,u:1,v:1,w:1,x:1,y:1,z:1,aa:1,ab:1,
    ac:1,ad:1,ae:1,af:1,ag:1,ah:1,ai:1,aj:1,ak:1,al:1,am:1}]);
    writeFile(f, ret);
}