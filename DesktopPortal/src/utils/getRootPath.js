function getRootPath(path){
    if(!path)
        return;
    let urls = path.split('/');
    let urls2 = [];

    for(var i = 0;i<urls.length;i++){
        if( urls[i]){
            urls2.push(urls[i]);
        }
    }

    if(urls2.length>0){
        if( urls2[0]==='callback'){
            return '/'
        }
        return '/' + urls2[0] + '/';
    }

    return '/';
}

export default getRootPath;