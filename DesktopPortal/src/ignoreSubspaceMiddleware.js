//禁止全局ACTION使用subspace，如 router， redux-form
export default function ignoreSubspaceMiddleware(globalActions) {
    return () => next => action => {
  
        let r =  globalActions.find(x=> x.test(action.type));
        if(r){
            let ms = action.type.match(r);
            if(ms.length===2){
                action.type = ms[1];
            }
        }
        
        return next(action)
    }
  }