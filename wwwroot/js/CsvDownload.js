function handleDownloadButtonClick(controllerName) {
    var downloadButton = document.getElementById('downloadButton');
    downloadButton.disabled = true; // �{�^���𖳌�������

    var userName = document.getElementById('userName').value;
    var email = document.getElementById('email').value;
    var companyName = document.getElementById('companyName').value;
    var departmentName = document.getElementById('departmentName').value;
    var employeeNo = document.getElementById('employeeNo').value;
    var remarks1 = document.getElementById('remarks1').value;
    var remarks2 = document.getElementById('remarks2').value;
    // ��u�ҊǗ���ʗp////////////////////////////////////////
    var userRole;
    var elmUserRole = document.getElementById('userRole');
    if (elmUserRole) {
        userRole = elmUserRole.value;
    }
    var available;
    var elmAvailable = document.getElementById('available');
    if (elmAvailable) {
        // �v�f�����݂���ꍇ�̏���
        available = elmAvailable.value;
    }
    ///////////////////////////////////////////////////////////

    var fd = new FormData();
    fd.append('userName', userName);
    fd.append('email', email);
    fd.append('companyName', companyName);
    fd.append('departmentName', departmentName);
    fd.append('employeeNo', employeeNo);
    fd.append('remarks1', remarks1);
    fd.append('remarks2', remarks2);
    // ��u�ҊǗ���ʗp///////////////////////////////
    if (userRole) fd.append('userRole', userRole);
    if (available) fd.append('available', available);
    //////////////////////////////////////////////////

    fetch(`/${controllerName}/SearchActionCSV`, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': csrfToken,
        },
        body: fd,
    })
        .then(response => {
            if (!response.ok) {
                this.messageBox("", "�t�@�C���_�E�����[�h�Ɏ��s���܂���", []);
                downloadButton.disabled = false; // �G���[�����������ꍇ�̓{�^�����ēx�L��������
                throw new Error('Fetch�G���[');
            }
            return response.json();
        })
        .then(data => {
            downloadCSV(data);
            this.messageBox("", "�t�@�C���_�E�����[�h���������܂���", []);
            downloadButton.disabled = false; // ����Ɋ��������ꍇ�̓{�^�����ēx�L��������
        })
        .catch(error => {
            console.error('Fetch�G���[: ', error);
            downloadButton.disabled = false; // �G���[�����������ꍇ�̓{�^�����ēx�L��������
        });
}

function downloadCSV(data) {
    // �w�肳�ꂽ�J�����݂̂��܂�CSV�w�b�_�[���쐬
    //const headers = ['action', 'userId', 'loginId', 'userName', 'companyName', 'departmentName', 'email', 'employeeNo', 'remarks1', 'remarks2', 'userRole', 'availableFlg', 'deletedFlg'].join(',');
    const headers = ['�A�N�V����', '���[�U���ʎq', '���p��ID', '���p�Җ�', '�@�l��', '����������', '���[���A�h���X', '�Ј��ԍ�', '���l1', '���l2', '���p�ҋ敪', '��u�ۃt���O', '�폜�t���O'].join(',');

    const csvData = data.map(item => {
        const row = [
            item.action || '',
            item.userId || '',
            item.loginId || '',
            item.userName || '',
            item.companyName || '',
            item.departmentName || '',
            item.email || '',
            item.employeeNo || '',
            item.remarks1 || '',
            item.remarks2 || '',
            item.userRole || '',
            item.availableFlg ? '1' : '0',
            item.deletedFlg ? '1' : '0',
        ].map(value => {
            // �_�u���N�I�[�g���G�X�P�[�v���A��������̃J���}���݂͂܂�
            return typeof value === 'string' && value.includes(',') ? `"${value.replace(/"/g, '""')}"` : value;
        });
        return row.join(',');
    }).join('\n');

    // CSV�f�[�^���t�@�C���Ƃ��ă_�E�����[�h
    const csvContent = `${headers}\n${csvData}`;
    const bom = new Uint8Array([0xEF, 0xBB, 0xBF]);
    const blob = new Blob([bom, csvContent], { type: 'text/csv;' });
    const link = document.createElement('a');
    const fileName = `MUser_${getFormattedDateTime()}.csv`;
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
}

function getFormattedDateTime() {
    const now = new Date();
    const year = now.getFullYear();
    const month = ('0' + (now.getMonth() + 1)).slice(-2);
    const day = ('0' + now.getDate()).slice(-2);
    const hours = ('0' + now.getHours()).slice(-2);
    const minutes = ('0' + now.getMinutes()).slice(-2);
    const seconds = ('0' + now.getSeconds()).slice(-2);
    return `${year}${month}${day}${hours}${minutes}${seconds}`;
}
