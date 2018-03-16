
export const AuthorUrl = window._authUrl || 'https://testauth.xinyaohangdc.com';//"http://192.168.50.240:5000";认证地址
// export const AuthorUrl = window._authUrl || 'https://auth.xinyaohangdc.com'; // 云上
export const BaseApiUrl = AuthorUrl + '/api/';//"http://192.168.50.240/api/"; //api基础地址
export const basicDataServiceUrl = window._basicDataUrl || 'https://testapi.xinyaohangdc.com';//"http://192.168.50.240:7000";认证地址
// export const basicDataServiceUrl = window._basicDataUrl || 'https://api.xinyaohangdc.com';// 云上
export const basicDataBaseApiUrl = basicDataServiceUrl + '/api/';
export const FlowChartUrl = window._flowChartUrl || 'https://testauth.xinyaohangdc.com';
export const FlowChartApiUrl = FlowChartUrl + '/api/';
export const UploadUrl = window._uploadUrl || 'https://testauth.xinyaohangdc.com';
//应用类型
export const ApplicationTypes = [
    { key: 'pc', value: 'PC端应用', icon: 'desktop' },
    { key: 'app', value: 'APP移动应用', icon: 'tablet' },
    { key: 'wx', value: '微信应用', icon: 'message' },
    { key: 'privApp', value: '内部应用', icon: 'appstore' }];
