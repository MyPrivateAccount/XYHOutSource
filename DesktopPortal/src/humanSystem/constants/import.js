import XLSX from "xlsx-style";

const AttendenceHead = {
    A: "key",//是否循环
    B: "userID",
    C: "name",
    J: "comments",
};
const LastHead = [
    "",
    "absent",
    "late",
    "funeral",
    "marry",
    "annual",
    "illness",
    "matter",
    "relaxation",
    "normal"
];
const Letter = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K",
"L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
"AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM",
"AN", "AO", "AP", "AQ", "AR", "AS", "AT","AU", "AV", "AW", "AX", "AY"];

export function exceltoobj(result, startrow) {//月份还没写
    var redata = [];
    var binary = "";
    var bytes = new Uint8Array(result);
    var length = bytes.byteLength;
    for (var i = 0; i < length; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    var obj = XLSX.read(binary, {type: 'binary', cellDates:true, cellStyles:true});
    if (obj) {
        let sheet = obj.Sheets[obj.SheetNames[0]];

        while(true) {
            let item = {};
            let i = startrow;
            let bf = false;

            let lastcolumn = 10;

            let j = 0;
            while(true) {
                if (j === 0) {
                    if (!sheet[Letter[j]+i].v) {
                        bf = true;
                        break;
                    }
                } else if (j === 1) {
                    item.userID = sheet[Letter[j]+i].v;
                } else if (j === 2) {
                    item.name = sheet[Letter[j]+i].v;
                } else if (j === 9) {
                    item.comments = sheet[Letter[j]+i].v;
                } else if (j > 9) {
                    let lastitem = sheet[Letter[j]+i];
                    let ittype = lastitem.t;
                    if (ittype === "n") {
                        item[LastHead[--lastcolumn]] = lastitem.v;
                    } else if (ittype === "s") {
                        switch (lastitem.v) 
                        {
                            case '√': 
                                if(!item.normalDate) {
                                    item.normalDate = [];
                                }
                                item.normalDate.push(j-9);
                            break;
                            case '○': 
                                if(!item.relaxationDate) {
                                    item.relaxationDate = [];
                                }
                                item.relaxationDate.push(j-9);
                             break;
                            case '×': 
                                if(!item.matterDate) {
                                    item.matterDate = [];
                                }
                                item.matterDate.push(j-9);
                             break;
                            case '△': 
                                if(!item.illnessDate) {
                                    item.illnessDate = [];
                                }
                                item.illnessDate.push(j-9);
                             break;
                            case '年': 
                                if(!item.annualDate) {
                                    item.annualDate = [];
                                }
                                item.annualDate.push(j-9);
                             break;
                            case '婚': 
                                if(!item.marryDate) {
                                    item.marryDate = [];
                                }
                                item.marryDate.push(j-9);
                             break;
                            case '丧': 
                                if(!item.funeralDate) {
                                    item.funeralDate = [];
                                }
                                item.funeralDate.push(j-9);
                             break;
                            case '迟': 
                                if(!item.lateDate) {
                                    item.lateDate = [];
                                }
                                item.lateDate.push(j-9);
                             break;
                            case '旷': 
                                if(!item.absentDate) {
                                    item.absentDate = [];
                                }
                                item.absentDate.push(j-9);
                             break;
                            default: break;
                        }
                    }
                    if (lastcolumn<=0) {
                        break;
                    }
                }
                j++;
                if (item) {
                    redata.push(item);
                }
            }
            
            i++;
            if (bf) {
                break;
            }
           
        }
    }
    return redata;
}