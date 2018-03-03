import moment from 'moment'


const converters = [
    (cus,user,pi, val)=>{
        return (val||'').replace(/\$CUS_NAME\$/g, cus.customerName||'');
    },
    (cus, user,pi,val)=>{
        return (val||'').replace(/\$CUS_MOBILE\$/g, cus.mainPhone||'');
    },
    (cus, user,pi,val)=>{
        return (val||'').replace(/\$COMPANY\$/g, '新耀行');
    },
    (cus, user,pi,val)=>{
        return (val||'').replace(/\$TODAY\$/g, moment().format('YYYY年MM月DD日'));
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$TODAY2\$/g, moment().format( 'YYYY-MM-DD'));
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$TODAY3\$/g, moment().format( 'YYYY.MM.DD'));
    },
    (cus, user,pi,val)=>{
        return (val||'').replace(/\$REPORTTIME\$/g, moment(cus.expectedBeltTime).format('YYYY年MM月DD日 HH:mm'));
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$REPORTTIME2\$/g, moment(cus.expectedBeltTime).format('YYYY-MM-DD HH:mm'));
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$REPORTTIME3\$/g, moment(cus.expectedBeltTime).format('YYYY.MM.DD HH:mm'));
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$USERNAME\$/g, cus.userTrueName);
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$DEPARTMENT\$/g, cus.departmentName);
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$USER_MOBILE\$/g, cus.userPhone);
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$ZC_MOBILE\$/g, cus.userPhone||'');
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$ZC_NAME\$/g, user.nickname||'');
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\$PROJECT_NAME\$/g, ((pi||{}).buildingBasic||{}).name);
    },
    (cus,user, pi,val)=>{
        return (val||'').replace(/\[项目名称\]/g, ((pi||{}).buildingBasic||{}).name);
    }
]

export const getReport=(template, cus,user, pi)=> {
    let lines = [];
    for(let i =0;i<template.length;i++){
        let line = template[i];

        let label = line.label;
        let value = line.value||'';
        converters.forEach(c=>{
            value = c(cus,user, pi, value )
        })
        let lineStr = `${label}${value}`;

        lines.push(lineStr);
    }

    return lines.join('\r\n');
}


export function isEmptyObject(obj,def){
    if(!obj ){
        return true;
    }
    
    def = def || {}

    let keys = Object.keys(obj);
    for(let i = 0;i<keys.length;i++){
        let key = keys[i];
        let sv = obj[key];
        let dv = def[key];

        if( sv instanceof Array){
            if( sv.length >0 ){
                return false;
            }
        }

        if( sv instanceof Date){

        }else if(sv instanceof Object){
            return isEmptyObject(sv, dv);
        }


        if( typeof dv !== "undefined" ){
            if( dv !== sv ){
                return false;
            }else{
                continue;
            }
        }

        if( sv){
            return false;
        }

    }

    return true;
}