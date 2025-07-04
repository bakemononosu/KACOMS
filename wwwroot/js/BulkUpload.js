const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

/**
 * table�̍�������
 * 
 */
$(window).on('load resize', function () {
    var window_height = window.innerHeight ? window.innerHeight : $(window).innerHeight();
    $('.content-mainr').css('height', (window_height * 0.8) + 'px');
    var fileUploadAreaHeight = document.querySelector('#upFileWrap').clientHeight;
    $('.table-responsive').css('height', (window_height * 0.8) - fileUploadAreaHeight - 70);
});


// �A�b�v���[�h�A�N���A�A�C�R���̏����\�����\����
var iconElement = document.getElementsByClassName('bi');
$(window).on('load', function () {
    for (var i = 0, l = iconElement.length; i < l; i++) {
        iconElement.item(i).style.display = 'none';
    }
});


// �h���b�O&�h���b�v�G���A�̎擾
var fileArea = document.getElementById('dropArea');

// input[type=file]�̎擾
var fileInput = document.getElementById('uploadFile');

/**
 * �h���b�O�I�[�o�[���̏���
 * 
 * @param {any} e
 */
fileArea.addEventListener('dragover', function (e) {
    e.preventDefault();
    fileArea.classList.add('dragover');
});

/**
 * �h���b�O�A�E�g���̏���
 * 
 * @param {any} e
 */
fileArea.addEventListener('dragleave', function (e) {
    e.preventDefault();
    fileArea.classList.remove('dragover');
});

/**
 * �h���b�v���̏���
 * 
 * @param {any} e
 */
fileArea.addEventListener('drop', function (e) {
    e.preventDefault();
    fileArea.classList.remove('dragover');

    // �e�[�u����������
    const errorTable = document.getElementById('error-table');
    errorTable.style.display = 'none';
    const errorTableContent = document.getElementById('error-table-content');
    errorTableContent.querySelector('tbody').innerHTML = '';

    // �A�b�v���[�h�A�N���A�A�C�R���̔�\��
    for (var i = 0, l = iconElement.length; i < l; i++) {
        iconElement.item(i).style.display = 'none';
    }

    // �h���b�v�����t�@�C���̎擾
    var files = e.dataTransfer.files;

    // �擾�����t�@�C����input[type=file]��
    fileInput.files = files;

    if (typeof files[0] !== 'undefined') {
        var fileData = files[0];
        if (fileData.type.indexOf('csv') == -1) {
            document.getElementById('uploadFile').value = '';
            messageBox("", "�g���q��CSV�ɂ��Ă��������B", []);
        } else {
            var el = document.getElementById('selectedFileName');
            el.textContent = fileData.name;

            // �A�b�v���[�h�A�N���A�A�C�R����\��
            for (var i = 0, l = iconElement.length; i < l; i++) {
                iconElement.item(i).style.display = '';
            }
        }
    } else {
        //�t�@�C�����󂯎��Ȃ������ۂ̏���
        messageBox("", "�t�@�C���Ǎ��Ɏ��s���܂����B", []);
    }
});

/**
 * input[type=file]�ɕύX������Ύ��s
 * 
 * @param {any} e
 */
fileInput.addEventListener('change', function (e) {
    // �e�[�u����������
    const errorTable = document.getElementById('error-table');
    errorTable.style.display = 'none';
    const errorTableContent = document.getElementById('error-table-content');
    errorTableContent.querySelector('tbody').innerHTML = '';

    // �A�b�v���[�h�A�N���A�A�C�R���̔�\��
    for (var i = 0, l = iconElement.length; i < l; i++) {
        iconElement.item(i).style.display = 'none';
    }

    // �h���b�v�����t�@�C���̎擾
    var files = e.target.files;

    // �擾�����t�@�C����input[type=file]��
    fileInput.files = files;
    if (typeof files[0] !== 'undefined') {
        var fileData = files[0];
        if (fileData.type.indexOf('csv') == -1) {
            document.getElementById('uploadFile').value = '';
            messageBox("", "�g���q��CSV�ɂ��Ă��������B", []);
        } else {
            var el = document.getElementById('selectedFileName');
            el.textContent = fileData.name;

            // �A�b�v���[�h�A�N���A�A�C�R����\��
            for (var i = 0, l = iconElement.length; i < l; i++) {
                iconElement.item(i).style.display = '';
            }
        }
    } else {
        // �t�@�C�����󂯎��Ȃ������ۂ̏���
        messageBox("", "�t�@�C���Ǎ��Ɏ��s���܂����B", []);
    }
}, false);

/**
 * �擾�t�@�C���N���A
 * 
 */
function clearFileData() {
    // �t�@�C������������
    document.getElementById('uploadFile').value = '';
    var el = document.getElementById('selectedFileName');
    el.textContent = "�I������Ă��܂���";

    // �ꎞ�t�@�C����Path�����Z�b�g
    document.getElementById('tempFileName').value = "";

    // �e�[�u����������
    const errorTable = document.getElementById('error-table');
    errorTable.style.display = 'none';
    const errorTableContent = document.getElementById('error-table-content');
    errorTableContent.querySelector('tbody').innerHTML = '';

    // �A�b�v���[�h�A�N���A�A�C�R���̔�\��
    for (var i = 0, l = iconElement.length; i < l; i++) {
        iconElement.item(i).style.display = 'none';
    }

    // �X�V�{�^����񊈐�
    document.getElementById('sbmBtn').setAttribute('disabled', 'true');
}

/**
 * CSV�t�@�C���o���f�[�V����
 * 
 */
function fetchData() {
    var files = fileInput.files;
    if (typeof files[0] !== 'undefined') {
        var fileData = files[0];
        var el = document.getElementById('selectedFileName');
        el.textContent = fileData.name;
        //�t�@�C��������Ɏ󂯎�ꂽ�ۂ̏���
        var fd = new FormData();
        fd.append('fileData', fileData)

        // �X�V�{�^����񊈐�
        document.getElementById('sbmBtn').setAttribute('disabled', 'true');
        // �ꎞ�t�@�C����Path�����Z�b�g
        document.getElementById('tempFileName').value = "";

        // �A�b�v���[�h�A�N���A�A�C�R���̔�\��
        for (var i = 0, l = iconElement.length; i < l; i++) {
            iconElement.item(i).style.display = 'none';
        }

        // ���[�f�B���O�X�N���[����\��
        document.getElementById('loading-screen').style.display = 'block';

        var controllerName = document.getElementById('controllerName').value
        fetch(`/${controllerName}/ValidateCsv`, {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                return response.json()
            })
            .then(data => {
                // ���[�f�B���O�X�N���[�����\���ɂ���
                document.getElementById('loading-screen').style.display = 'none';
                const errorTable = document.getElementById('error-table');
                // �e�[�u����������
                const errorTableContent = document.getElementById('error-table-content');
                errorTableContent.querySelector('tbody').innerHTML = '';

                const errors = data.errData;
                // �G���[���X�g����łȂ��ꍇ
                if (errors.length > 0) {
                    // �G���[�e�[�u����\��
                    errorTable.style.display = 'block';

                    // �G���[���b�Z�[�W���e�[�u���ɒǉ�
                    const tbody = errorTable.querySelector('tbody');
                    errors.forEach(error => {
                        const row = document.createElement('tr');
                        row.innerHTML = `<td>${error.lineNumber}</td><td>${error.errorMessage}</td>`;
                        tbody.appendChild(row);
                    });
                    // �t�@�C������������
                    document.getElementById('uploadFile').value = '';
                    el.textContent = "�I������Ă��܂���";

                } else {
                    // �X�V�{�^��������
                    document.getElementById('sbmBtn').removeAttribute('disabled');
                    // �ꎞ�t�@�C����Path��hidden�v�f�ɃZ�b�g
                    document.getElementById('tempFileName').value = data.tempFileName;
                    // �G���[���X�g����̏ꍇ�A�e�[�u�����\���ɂ���
                    errorTable.style.display = 'none';
                    // �t�@�C������������
                    document.getElementById('uploadFile').value = '';
                    // �N���A�A�C�R����\��
                    iconElement.item(3).style.display = '';

                    messageBox("", "����ɃA�b�v���[�h����܂����B</br>�X�V�{�^������ꊇ�捞�݂����s���Ă��������B", []);
                }
            })
            .catch(error => {
                // ���[�f�B���O�X�N���[�����\���ɂ���
                document.getElementById('loading-screen').style.display = 'none';
                messageBox("", error, []);
            });
    } else {
        // �t�@�C�����󂯎��Ȃ������ۂ̏���
        messageBox("", "�t�@�C���Ǎ��Ɏ��s���܂����B", []);
    }

}

/**
 * CSV�t�@�C���ꊇ�捞�ݏ���
 * 
 */
async function bulkUpload() {
    var buttons = [];
    buttons[0] = { type: "primary", text: "�͂�" };
    buttons[1] = { type: "secondary", text: "������" };
    var result = await messageBox('�X�V�m�F', '�ꊇ�X�V���J�n���܂��B��낵���ł���?', buttons)

    if (result == '�͂�') {
        // �u�͂��v���I�����ꂽ�Ƃ��̏���
        var controllerName = document.getElementById('controllerName').value
        var tempFileName = document.getElementById('tempFileName').value
        var fd = new FormData();
        fd.append('tempFileName', tempFileName)

        fetch(`/${controllerName}/BulkUpload`, {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                return response.json()
            })
            .then(data => {
                // ���[�f�B���O�X�N���[�����\���ɂ���
                document.getElementById('loading-screen').style.display = 'none';
                const errorTable = document.getElementById('error-table');
                // �e�[�u����������
                const errorTableContent = document.getElementById('error-table-content');
                errorTableContent.querySelector('tbody').innerHTML = '';

                const errors = data.errData;
                // �G���[���X�g����łȂ��ꍇ
                if (errors.length > 0) {
                    // �G���[�e�[�u����\��
                    errorTable.style.display = 'block';

                    // �G���[���b�Z�[�W���e�[�u���ɒǉ�
                    const tbody = errorTable.querySelector('tbody');
                    errors.forEach(error => {
                        const row = document.createElement('tr');
                        row.innerHTML = `<td>${error.lineNumber}</td><td>${error.errorMessage}</td>`;
                        tbody.appendChild(row);
                    });
                    messageBox("", "�G���[�����s�܂Ŏ捞�݊������Ă��܂��B", []);
                } else {
                    // �G���[���X�g����̏ꍇ�A�e�[�u�����\���ɂ���
                    errorTable.style.display = 'none';

                    messageBox("", "�ꊇ�捞�݂͊������܂����B", []);
                }
                // �X�V�{�^����񊈐�
                document.getElementById('sbmBtn').setAttribute('disabled', 'true');
                // �N���A�A�C�R�����\��
                iconElement.item(3).style.display = 'none';
                // �ꎞ�t�@�C����Path�����Z�b�g
                document.getElementById('tempFileName').value = "";
                // �t�@�C������������
                document.getElementById('uploadFile').value = '';
                // �I���t�@�C�����𖢑I����
                var el = document.getElementById('selectedFileName');
                el.textContent = "�I������Ă��܂���";
            })
            .catch(error => {
                messageBox("", error, []);
            });
    }
    else {
        // �u�������v���I�����ꂽ�Ƃ��̏���
        return;
    }
}