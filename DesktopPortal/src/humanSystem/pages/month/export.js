//import XLSX from 'xlsx';

import XLSX from "xlsx-style";
//允许对角线合并
//subhead数组情况必须相同
const AllHead = [//以第一层为基准, 必须平行
    {
        t: "s", 
        v: "新汇耀有限公司",//基本工资
        s: {
            font: {
                name: 'Times New Roman',
                sz: 25,
                color: {rgb: "#FFFFFFFF"},
                bold: true,
                italic: false,
                underline: false
            },
            alignment: {
                vertical: "center",
                horizontal: "center",
            },
            border: {
                top: {style: "thin", color: {auto: 1}},
                right: {style: "thin", color: {auto: 1}},
                bottom: {style: "thin", color: {auto: 1}},
                left: {style: "thin", color: {auto: 1}}
            }
        },
        row: 1,
        col: 7,
        subhead: [
            {
                t: "s", v: "序号", row: 2, col: 1,
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
            },
            {
                t: "s", v: "身份证号", row: 2,col: 1,
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
            },
            {
                t: "s", v: "工号", row: 2,col: 1,
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
            },
            {
                t: "s", v: "姓名", row: 2,col: 1,
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
            },
            {
                t: "s", v: "部门", row: 2,col: 1,
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
            },
            {
                t: "s", v: "职位", row: 2,col: 1,
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
            },
            {
                t: "s", v: "应出勤天数", row: 2,col: 1,
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
            },
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
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                },
                row: 1,
                col: 6,
                subhead: [
                    {
                        t: "s", v: "基本工资",row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "交通补贴",row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "通信补贴",row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "其它补贴",row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "加班",row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "绩效奖励",row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
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
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                },
                row: 1,
                col: 5,
                subhead: [
                    {
                        t: "s", v: "迟到", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "事假", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "旷工", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "行政扣款", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "端口扣款", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
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
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
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
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
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
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
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
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                },
                row: 1,
                col: 5,
                subhead: [
                    {
                        t: "s", v: "养老", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "失业", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "医疗", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "工伤", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
                    {
                        t: "s", v: "公积金", row: 1, col: 1,
                        s: {
                            fill: {
                                patternType: "solid",
                                bgColor: { indexed: 64 },
                                fgColor: {rgb: "BFBFBF"}
                            },
                            font: {
                                name: "Calibri",
                                sz: 10,
                                color: {rgb: "FFFF00"},
                                bold: false,
                                italic: false,
                                underline: false
                            },
                            border: {
                                top: {style: "thin", color: {auto: 1}},
                                right: {style: "thin", color: {auto: 1}},
                                bottom: {style: "thin", color: {auto: 1}},
                                left: {style: "thin", color: {auto: 1}}
                            }
                        }
                    },
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
                s: {
                    fill: {
                        patternType: "solid",
                        bgColor: { indexed: 64 },
                        fgColor: {rgb: "BFBFBF"}
                    },
                    font: {
                        name: "Calibri",
                        sz: 10,
                        color: {rgb: "FFFF00"},
                        bold: false,
                        italic: false,
                        underline: false
                    },
                    border: {
                        top: {style: "thin", color: {auto: 1}},
                        right: {style: "thin", color: {auto: 1}},
                        bottom: {style: "thin", color: {auto: 1}},
                        left: {style: "thin", color: {auto: 1}}
                    }
                }
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

function saveAs(obj, fileName) {
    var tmpa = document.createElement("a");
    tmpa.download = fileName || "下载";
    tmpa.href = URL.createObjectURL(obj);
    tmpa.click();
    setTimeout(function () {
        URL.revokeObjectURL(obj);
    }, 100);
}
function s2ab(s) {
    if (typeof ArrayBuffer !== 'undefined') {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    } else {
        var buf = new Array(s.length);
        for (var i = 0; i != s.length; ++i) buf[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }
}
export function writeFile(merge, data) {

    let na = "A1:"+Letter[merge.clen]+(data.length+merge.level+1);
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
    let tmpWB = {
            SheetNames:["工资表"],
            Sheets: {
                工资表: sh
            }
        }

    let tmpDown = new Blob([s2ab(XLSX.write(tmpWB,
        { bookType:'xlsx', bookSST: false, type: 'binary' }//这里的数据是用来定义导出的格式类型
    ))], {
            type: ""
        });
    saveAs(tmpDown, 'test.xlsx');

    // let na = "A1:"+Letter[merge.clen]+(data.length+merge.level+1);
    // let obj = {};
    // for (const it of data) {
    //     obj = Object.assign(obj, it);
    // }
    // let sh = {
    //     "!ref": na,
    //     ...merge.head,
    //     ...obj,
    //     "!merges":[...merge],
    // };
    // XLSX.writeFileSync({
    //     SheetNames:["工资表"],
    //     Sheets: {
    //         工资表: sh
    //     }
    // }, 'test.xlsx');
}

export function Test() {
    let f = createMergeHead(AllHead);
    let ret = insertColum(f, [{a:1,b:1,c:1,d:1,e:1,f:1,g:1,h:1,i:1,j:1,k:1,l:1,m:1,n:1,
    o:1,p:1,q:1,r:1,s:1,t:1,u:1,v:1,w:1,x:1,y:1,z:1,aa:1,ab:1,
    ac:1,ad:1,ae:1,af:1,ag:1,ah:1,ai:1,aj:1,ak:1,al:1,am:1}]);
    writeFile(f, ret);
}



// jquery@1.11.0
// babel-polyfill@6.9.1
// xlsx-style@0.8.13

// node

// externals: [
//     {
//         './cptable': 'var cptable'
//     }
// ],