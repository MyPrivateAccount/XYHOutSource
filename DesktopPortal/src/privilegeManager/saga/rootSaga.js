import { watchGetAppList, watchAppSave, watchAppDelete } from './appSaga';
import watchAllRoleAsync from './roleSaga';
import watchAllOrgAsync from './orgSaga';
import { watchPrivilegeGet, watchPrivilegeSave, watchPrivilegeDelete } from './privilegeSaga';
import { watchEmpAll } from './empSaga';

export default function* rootSaga() {
    yield [
        watchGetAppList(),
        watchAppSave(),
        watchAppDelete(),

        watchAllRoleAsync(),

        watchAllOrgAsync(),

        watchEmpAll(),

        watchPrivilegeGet(),
        watchPrivilegeSave(),
        watchPrivilegeDelete()
    ]
}