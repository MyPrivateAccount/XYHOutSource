
var JsonDownload = function (content, filename) {
    // 创建隐藏的可下载链接
    var eleLink = document.createElement('a');
    eleLink.download = filename;
    eleLink.style.display = 'none';
    // 字符内容转变成blob地址
    var blob = new Blob([content]);
    eleLink.href = URL.createObjectURL(blob);
    // 触发点击
    document.body.appendChild(eleLink);
    eleLink.click();
    // 然后移除
    document.body.removeChild(eleLink);
};

export function ReadUploadFile(uploadFile, callback) {
    try {
        var reader = new FileReader();//新建一个FileReader
        reader.readAsText(uploadFile.file, "UTF-8");//读取文件 
        reader.onload = function (evt) { //读取完文件之后会回来这里
            var fileString = evt.target.result; // 读取文件内容
            if (callback) {
                callback(fileString);
            }
        }
    } catch (e) {
        callback("[]");
    }
}

export default JsonDownload;