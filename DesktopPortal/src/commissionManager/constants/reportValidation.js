import validations from '../../utils/validations'

const reportValidation = {
    jyht:{
        fyzId:[[validations.isRequired, '必须选择分行']],
        cjrId:[[validations.isRequired, '必须选择成交人']],
        cjrq:[[validations.isRequired, '必须选择成交日期']],
        bz:[[validations.isRequired, '必须填写备注']],
        cjzj:[[validations.isRequired, '必须填写成交总价'], [validations.isGreaterThanOrEqual,'成交总价填写有误', 10000]],
        ycjyj:[[validations.isRequired, '必须填写佣金'], [validations.isGreaterThanOrEqual,'佣金填写有误', 1000]] ,
        zzhtbh:[[validations.isRequired, '必须填写自制合同编号']] ,
    },
    cjwy:{
        wyCq:[[validations.isRequired,'必须选择物业城区']],
        wyPq:[[validations.isRequired,'必须选择物业片区']],
        wyMc:[[validations.isRequired,'必须填写物业名称']],
        wyWz:[[validations.isRequired,'必须填写物业楼栋']],
        wyLc:[[validations.isRequired,'必须填写物业楼层']],
        wyFh:[[validations.isRequired,'必须填写物业房号']],
        wyCzwydz:[[validations.isRequired,'必须填写产证物业地址']],
        wyJzmj:[[validations.isRequired,'必须填写填写建筑面积'], [validations.isGreaterThan,'请填写正确的建筑面积',0]],
        wyWyJj:[[validations.isRequired,'必须填写均价'], [validations.isGreaterThan,'请填写正确的均价',1000]]   
    },
    yzxx:{
        yzMc:[[validations.isRequired,'必须填写业主名称']]
    },
    khxx:{
        khMc:[[validations.isRequired,'必须填写客户姓名']],
        khZjhm:[[validations.isRequired,'必须填写客户证件号码']],
        khSj:[[validations.isRequired,'必须填写客户手机号']],
        khKhly:[[validations.isRequired,'必须选择客户客户来源']],
    },
    yjfp:{
        yjZcjyj: [[(value, values, report)=>{
            if(value !== report.ycjyj){
                return false;
            }
            return true;
        }, '业绩分配中总佣金与成交合同中成交佣金不一致'], 
        [
            (value, values, report)=>{
                let yzys = (report.reportYjfp||{}).yjYzys||0;
                let khys = (report.reportYjfp||{}).yjKhys||0;
                return value === (yzys + khys);
            }, '业主应收加上客户应收不等于总佣金'
        ]],
        yjJyj:[[validations.isGreaterThan, '净佣金必须大于0',0],[
            (value, values, report)=>{
                let ri = (report.reportYjfp||{}).reportOutsides || [];
                let yjZcjyj = (report.reportYjfp||{}).yjZcjyj || 0;
                let wyJe= 0;
                ri.forEach(item=>{
                    wyJe = wyJe + (item.money*1)
                    wyJe = Math.round(wyJe*100)/100;
                })
                let jyj = yjZcjyj - wyJe
                
                return jyj === value;

            }, '总成交佣金减去外佣不等于浄佣金'
        ],[
            (value, values, report)=>{
                let ri = (report.reportYjfp||{}).reportInsides || [];
            
                let nyJe= 0;
                ri.forEach(item=>{
                    nyJe = nyJe + (item.money*1)
                    nyJe = Math.round(nyJe*100)/100;
                })
                
                
                return nyJe === value;

            }, '内部分摊金额合计不等于净佣金'
        ],[
            (value, values, report)=>{
                let ri = (report.reportYjfp||{}).reportInsides || [];
            
                let nyJe= 0;
                ri.forEach(item=>{
                    nyJe = nyJe + (item.percent*1)
                    nyJe = Math.round(nyJe*100)/100;
                })
                
                
                return nyJe === 100;

            }, '内部分摊比例合计必须等于100'
        ],[
            (value, values,report)=>{
                let ri = (report.reportYjfp||{}).reportInsides || [];
            
                let gl= [];
                ri.forEach(item=>{
                    let o = gl.find(x=>x.uid === item.uid && x.type === item.type);
                    if(o){
                        o.count=o.count+1;
                    }else{
                        o = {uid: item.uid, type: item.type}
                        o.count = 1;
                        gl.push(o)
                    }
                })

                let idx = gl.findIndex(x=>x.count>1)
                
                
                return idx < 0;

            }, '内部分摊项重复'
        ],[
            (value, values,report)=>{
                let ri = (report.reportYjfp||{}).reportOutsides || [];
            
                let gl= [];
                ri.forEach(item=>{
                    let o = gl.find(x=>x.object === item.object && x.moneyType === item.moneyType);
                    if(o){
                        o.count=o.count+1;
                    }else{
                        o={object: item.object, moneyType: item.moneyType}
                        o.count = 1;
                        gl.push(o)
                    }
                })

                let idx = gl.findIndex(x=>x.count>1)
                
                
                return idx < 0;

            }, '外部分摊项重复'
        ]]
    },
    wyItem:{
        moneyType:[[validations.isRequired, '款项类别不可为空']],
        object:[[validations.isRequired, '收付对象不可为空']],
        money:[[validations.isRequired, '金额不可为空'], [validations.isGreaterThan, '金额必须大于0', 0]],
    },
    nrItem:{
        sectionId:[[validations.isRequired, '部门ID不可为空']],
        sectionName:[[validations.isRequired, '部门名称不可为空']],
        uid:[[validations.isRequired, '员工不可为空']],
        workNumber:[[validations.isRequired, '员工工号不可为空']],
        type:[[validations.isRequired, '身份不可为空']],
        percent:[[validations.isRequired, '比例不可为空'], 
                 [validations.isGreaterThan, '比例必须大于0',0],    
                 [validations.isLessThanOrEqual, '比例必须小于等于100', 100]
                ],
        money:[[validations.isRequired, '金额不可为空']],
    },
    distribute:{
        yjZcjyj: [[(value, values)=>{
            return value === ((values.ownerMoney||0) + (values.customMoney||0))
        },'总佣金不等于业主佣金加客户佣金']],
        updateReason: [[validations.isRequired, '备注不可为空']],
        jyj: [[(value, values)=>{
            let ri = (values||{}).reportOutsides || [];
                let yjZcjyj = (values||{}).yjZcjyj || 0;
                let wyJe= 0;
                ri.forEach(item=>{
                    wyJe = wyJe + (item.money*1)
                    wyJe = Math.round(wyJe*100)/100;
                })
                let jyj = yjZcjyj - wyJe
                
                return jyj === value;
        }, '净佣金不等于总佣金减去外佣扣除'],[
            (value, values)=>{
                let ri = (values||{}).reportInsides || [];
            
                let nyJe= 0;
                ri.forEach(item=>{
                    nyJe = nyJe + (item.percent*1)
                    nyJe = Math.round(nyJe*100)/100;
                })
                
                
                return nyJe === 100;

            }, '内部分摊比例合计必须等于100'
        ],[
            (value, values)=>{
                let ri = (values||{}).reportInsides || [];
            
                let gl= [];
                ri.forEach(item=>{
                    let o = gl.find(x=>x.uid === item.uid && x.type === item.type);
                    if(o){
                        o.count=o.count+1;
                    }else{
                        o = {uid: item.uid, type: item.type}
                        o.count = 1;
                        gl.push(o)
                    }
                })

                let idx = gl.findIndex(x=>x.count>1)
                
                
                return idx < 0;

            }, '内部分摊项重复'
        ],[
            (value, values)=>{
                let ri = (values||{}).reportOutsides || [];
            
                let gl= [];
                ri.forEach(item=>{
                    let o = gl.find(x=>x.object === item.object && x.moneyType === item.moneyType);
                    if(o){
                        o.count=o.count+1;
                    }else{
                        o={object: item.object, moneyType: item.moneyType}
                        o.count = 1;
                        gl.push(o)
                    }
                })

                let idx = gl.findIndex(x=>x.count>1)
                
                
                return idx < 0;

            }, '外部分摊项重复'
        ],[
            (value, values)=>{
                let ri = (values||{}).reportInsides || [];
            
                let nyJe= 0;
                ri.forEach(item=>{
                    nyJe = nyJe + (item.money*1)
                    nyJe = Math.round(nyJe*100)/100;
                })
                
                
                return nyJe === value;

            }, '内部分摊金额合计不等于净佣金'
        ]]
    }
}

export default reportValidation;