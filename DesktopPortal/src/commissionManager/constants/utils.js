import moment from 'moment'
//小数四舍五入
export function keepTwoDecimalFull(num) {
    var f = parseFloat(num);
    if (isNaN(f)) {
        return;
    }
    f = Math.round(num * 100) / 100;
    return f;
}

const dateFields ={
    ht:['cjrq','yjfksj','yxsqyrq','khlfrq','htqyrq'],
    wy: ['wyCqzqdsj'],
    yj:['yjYzyjdqr','yjKhyjdqr'],
    gh:['ghFkrq','ghJbrq','ghGhrq']
}
export function convertReport(report){
    if(!report){
        return ;
    }

    _convertOne(report,dateFields.ht);
    _convertOne(report.reportWy, dateFields.wy);
    _convertOne(report.reportYjfp, dateFields.yj);
    _convertOne(report.reportGh, dateFields.gh);

    let wy = report.reportWy;
    if(wy){
        if(wy.wyJzmj){
            wy.wyWyJj = Math.round(((report.cjzj||0) / (wy.wyJzmj||0))*100) / 100  //均价
        }
    }
    let yj = report.reportYjfp;
    if(yj){
        yj.yjZcjyj = (yj.yjYzys||0) + (yj.yjKhys||0)

        let wyJe = 0;
        (yj.reportOutsides||[]).forEach(item=>{
            wyJe = wyJe + (item.money||0)
            item.errors = {};
        })
        yj.yjJyj = yj.yjZcjyj - wyJe;

        let nyJe=  0;
        (yj.reportInsides||[]).forEach(item=>{
            nyJe = nyJe + (item.money||0)
            item.errors = {};
        })

    }

    return report;
}

function _convertOne(entity, fields){
    if(!entity){
        return;
    }
    for(let k in entity){
        let idx = fields.findIndex(x=>x===k);
        if(idx>=0){
            if(entity[k]){
                entity[k] = moment(entity[k])
            }
        }
    }
}