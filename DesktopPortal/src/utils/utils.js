import moment from 'moment'


const converters = [
    (cus, user, pi, val) => {
        return (val || '').replace(/\$CUS_NAME\$/g, cus.customerName || '');
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$CUS_MOBILE\$/g, cus.mainPhone || '');
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$COMPANY\$/g, '新耀行');
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$TODAY\$/g, moment().format('YYYY年MM月DD日'));
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$TODAY2\$/g, moment().format('YYYY-MM-DD'));
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$TODAY3\$/g, moment().format('YYYY.MM.DD'));
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$REPORTTIME\$/g, moment(cus.expectedBeltTime).format('YYYY年MM月DD日 HH:mm'));
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$REPORTTIME2\$/g, moment(cus.expectedBeltTime).format('YYYY-MM-DD HH:mm'));
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$REPORTTIME3\$/g, moment(cus.expectedBeltTime).format('YYYY.MM.DD HH:mm'));
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$USERNAME\$/g, cus.userTrueName);
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$DEPARTMENT\$/g, cus.departmentName);
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$USER_MOBILE\$/g, cus.userPhone);
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$ZC_MOBILE\$/g, cus.userPhone || '');
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$ZC_NAME\$/g, user.nickname || '');
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\$PROJECT_NAME\$/g, ((pi || {}).buildingBasic || {}).name);
    },
    (cus, user, pi, val) => {
        return (val || '').replace(/\[项目名称\]/g, ((pi || {}).buildingBasic || {}).name);
    }
]

export const getReport = (template, cus, user, pi) => {
    let lines = [];
    for (let i = 0; i < template.length; i++) {
        let line = template[i];

        let label = line.label;
        let value = line.value || '';
        converters.forEach(c => {
            value = c(cus, user, pi, value)
        })
        let lineStr = `${label}${value}`;

        lines.push(lineStr);
    }

    return lines.join('\r\n');
}


export function isEmptyObject(obj, def) {
    if (!obj) {
        return true;
    }

    def = def || {}

    let keys = Object.keys(obj);
    for (let i = 0; i < keys.length; i++) {
        let key = keys[i];
        let sv = obj[key];
        let dv = def[key];

        if (sv instanceof Array) {
            if (sv.length > 0) {
                return false;
            }
        }

        if (sv instanceof Date) {

        } else if (sv instanceof Object) {
            return isEmptyObject(sv, dv);
        }


        if (typeof dv !== "undefined") {
            if (dv !== sv) {
                return false;
            } else {
                continue;
            }
        }

        if (sv) {
            return false;
        }

    }

    return true;
}
//电话号码前三后四
export function formatPhoneNo(phoneNum) {
    if (phoneNum) {
        return phoneNum.replace(/ /g, '').replace(/([0-9]{3})[0-9]+([0-9]{4})/g, '$1****$2');
    }
    return phoneNum;
}
//获取商铺全名
export const getShopName = (shop) => {
    if (shop) {
        let fn = shop.floorNo;
        if ((fn * 1) < 0) {
            fn = '负' + Math.abs(fn * 1);
        }
        return `${shop.buildingNo || '未命名'}-${fn || '未命名'}层${shop.floorInformation ? "(" + shop.floorInformation + ")" : ''}-${shop.number || '未命名'}`;
    }
    return '';
}
//获取字典配置
export const getDicPars = (dicKey, dicList) => {
    let findList = [];
    if (dicList) {
        let result = dicList.find(t => t.groupId === dicKey);
        if (result) {
            findList = result.dicPars;
        }
    }
    return findList;
}

export function groupShops(shopList){
    let buildings = [];
    let groupCount = [{key:"1",count:0},{key:"2",count:0},{key:"3",count:0},{key:"10",count:0},{key:"18",count:0},{key:"35",count:0}];
    
    if(!shopList || shopList.length===0)
    {
        return {buildings, groupCount};
    }

    shopList.forEach(shop=>{
        let bn = shop.buildingNo || '';
        bn = bn.toString();
        let old = buildings.find(x=>x.storied=== bn);
        if(!old){
            old={storied: bn, shops:[]}
            buildings.push(old)
        }

        old.shops.push(shop);

        let ss = shop.saleStatus;
        let g = groupCount.find(x=>x.key === ss);
        if(!g){
            g = {key: ss, count:0}
            groupCount.push(g);
        }
        g.count = g.count+1;

    })

  
    buildings = sortBuildingNo(buildings);

    buildings.forEach(b=>{
        b.shops = sortShops(b.shops);
    })

    return {buildings, groupCount};

}

export function sortBuildingNo(buildingNos){
    let r1 = /([^0-9]?)([0-9]+)/;
    let r2 = /([^A-Z]?)([A-Z]+)/;

    return buildingNos.sort((a,b)=>{
        let m1 = (a.storied || '').match(r1);
        let t1 = 0;
        if(!m1){
            m1 = (a.storied || '').match(r2);
            t1 = 1;
        }
        let m2 = (b.storied || '').match(r1);
        let t2 = 0;
        if(!m2){
            m2 = (b.storied || '').match(r2);
            t2 = 1;
        }

        if(t1 === t2 && m1 && m2){
            if(t1===0){
                return (m1[2]*1 - m2[2]*1);
            }else if(t1===1){
                return m1[2].localeCompare(m2[2]);
            }
        }
        
        return (a.storied || '').localeCompare((b.storied || ''));

    })
}

export function sortShops(shops){
    return  shops.sort((a,b)=>{
        let fna = a.floorNo*1;
        let fnb = b.floorNo * 1;

        let numa = a.number||'';
        let numb = b.number||'';

        let r1 = /([^0-9]?)([0-9]+)/;
        
        let m1 = numa.match(r1);
        let m2 = numb.match(r1);
       
        

        if(m1 && m2){
            return (fna*10000+ m1[2]*1) - (fnb*10000+ m2[2]*1);
        }

        if(fna === fnb){
            return numa.localeCompare(numb);
        }

        return fna - fnb;

    })
}

export function getOrganizationTree(nodes){
    let l1 = [];

    nodes.forEach(x=>{
        if(!x.parentId){
            l1.push(x);
        }else{
            var p = nodes.find(y=>y.id === x.parentId);
            if(!p){
                l1.push(x);
            }
        }   
    });

    let treeRoot = [];
    l1.forEach(item=>{
        var newItem ={
            label:item.organizationName,
            value: item.id,
            key:item.id,
            id: item.id,
            data: item
        }
        treeRoot.push(newItem)
        _getChildren(nodes, newItem);
    })

    
    return treeRoot;
}

const orgTypeMap = {
    Bloc: {order: 0, topOrder: 0},
    Subsidiary: {order: 100, topOrder: 100},
    "Non business": {order: 100, topOrder: 0, ignore: true},
    Filiale: {order: 200, topOrder: 200},
    Normal: {order: 300, topOrder: 200, ignore: true},
    Business: {order: 300, topOrder: 200, ignore: true},
    Region: {order: 400, topOrder: 400},
    Area: {order: 500, topOrder: 400},
    Group: {order: 600, topOrder: 400},
}

function _getChildren(nodes, parent){
    if(!parent.children){
        parent.children=[];
    }

    nodes.forEach(item=>{
        if(item.parentId === parent.id){
            var newItem ={
                name:item.organizationName,
                value: item.id,
                key:item.id,
                id: item.id,
                data: item,
                parent: parent
            }

            newItem.label = _getFullName(newItem);


            parent.children.push(newItem);
            _getChildren(nodes, newItem);
        }
    })
}

function _getFullName(item){
    let type = item.data.type;
    let sortInfo = orgTypeMap[type]
    if(!sortInfo){
        return item.data.organizationName;
    }

    let parents = [];
    parents.push(item);
    let cur = item;
    let curSortInfo = sortInfo;
    while( curSortInfo && curSortInfo.order> sortInfo.topOrder ){
        cur = cur.parent
        if(cur){
            curSortInfo = orgTypeMap[cur.data.type]
            if(curSortInfo && !curSortInfo.ignore){
                parents.splice(0,0, cur)
            }
        }else{
            curSortInfo = null;
        }
    }

    let fn = '';
    parents.forEach(item=>{
        if(fn){
            fn = fn + "-"
        }
        fn = fn + item.data.organizationName
    })

    return fn;
}