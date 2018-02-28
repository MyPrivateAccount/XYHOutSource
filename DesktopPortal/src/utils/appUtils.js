//在saga中使用，自动在action前加入appname
const appAction = (appName) => {

    return {
        action: (actionCreator, ...args) => {
            let a = actionCreator(...args);
            a.type = `${appName}/${a.type}`;
            return a;
        },
        getActionType: (actionType) => {
            return `${appName}/${actionType}`;
        }
    }

}
export default appAction;

/*guid生成*/
function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}
export function NewGuid() {
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}