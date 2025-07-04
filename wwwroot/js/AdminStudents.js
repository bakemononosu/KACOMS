const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

// Table�̍���
$(window).on('load resize', function () {
    var elmTable = document.querySelector('.table-container');
    if (elmTable) resizeTable();
});

// accordion���J��
$('#collapseOne').on('hidden.bs.collapse shown.bs.collapse', function () {
    resizeTable();
});

// Table�̍�������
function resizeTable() {
    // ���������̌v�Z���~�߂�
    return;
    // �E�C���h�E�̍������擾
    var window_height = (window.innerHeight ? window.innerHeight : $(window).innerHeight()) - 130;

    // table�̈ʒu���擾
    var pos = $('.table-container').offset();

    // �t�b�^�[�܂ł̍����Z�b�g
    $('.content-main').css('height', window_height + 'px');

    // table���{�^���̍���
    var elemBlkupdate = document.querySelector('.blk_update');
    if (elemBlkupdate) blkupdateHeight = elemBlkupdate.clientHeight;

    // table�i�X�N���[���̈�j�̍����Z�b�g
    $('.table-container').css('height', ((window_height - pos.top + 50) - blkupdateHeight) + 'px');
}

// ���p�ۂ̐ؑցi�X�V�j
async function handleToggleAvailable(userId) {
    var buttons = [];
    buttons[0] = { type: "primary", text: "�͂�" };
    buttons[1] = { type: "secondary", text: "������" };
    var result = await messageBox('�X�V�m�F', '���p�ۂ��X�V���܂��B��낵���ł���?', buttons);

    if (result == '�͂�') {
        // �u�͂��v���I�����ꂽ�Ƃ��̏���
        var switchElement = document.getElementById('switch_' + userId);
        var fd = new FormData();
        fd.append('userId', userId);
        fd.append('swichInput', switchElement.checked);

        fetch('/AdminStudents/ToggleAvailable', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                if (!response.ok) {
                    messageBox("", "���p�ۂ̍X�V�Ɏ��s���܂���", []);
                }
            })
            .catch(error => {
                messageBox("", error, []);
            });
    }
    else {
        // �u�������v���I�����ꂽ�Ƃ��̏���
        var switchElement = document.getElementById('switch_' + userId);
        switchElement.checked = !switchElement.checked;
        return;
    }
}

// �ʓo�^�i�V�K�܂��͍X�V�j��ʂւ̑J��
function handleShowIndividual(userId) {
    document.getElementById('userId').value = userId;
    var fm = document.getElementById("searchedKeyWords");
    if (!userId || userId == undefined || userId == "" || userId.length == 0) {
        fm.setAttribute('action', 'ShowIndividualNew');
    } else {
        fm.setAttribute('action', 'ShowIndividual');
    }

    fm.submit();
}

// �ꗗ�ł̍s�폜�i�X�V�j
async function handleDeleteStudent(userId) {
    var buttons = [];
    buttons[0] = { type: "primary", text: "�͂�" };
    buttons[1] = { type: "secondary", text: "������" };
    var result = await messageBox('�폜�m�F', '�폜���܂��B��낵���ł���?', buttons);

    if (result == '�͂�') {
        // �u�͂��v���I�����ꂽ�Ƃ��̏���
        var fd = new FormData();
        fd.append('userId', userId);

        fetch('/AdminStudents/DeleteStudent', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                if (response.ok) {
                    document.getElementById('tr_' + userId).remove();
                    messageBox("", "�폜�ɐ������܂���", []);
                } else {
                    messageBox("", "�폜�Ɏ��s���܂���", []);
                }
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

//�V�K�ǉ��ɂăo���f�[�V������Insert���s��
function newUserInsert() {

    var resultForValidate = newValidateForm();
    if (resultForValidate == true) {
        newDataInsert();
    } else {
        return;
    }
};

//MUser�ւ̐V�K�f�[�^�ǉ��@�\
async function newDataInsert() {
    var email = document.getElementById('emailPersonal').value;
    var userName = document.getElementById('userNamePersonal').value;
    var companyName = document.getElementById('companyNamePersonal').value;
    var departmentName = document.getElementById('departmentNamePersonal').value;
    var employeeNo = document.getElementById('employeeNoPersonal').value;
    var remarks1 = document.getElementById('remarks1Personal').value;
    var remarks2 = document.getElementById('remarks2Personal').value;
    var passwordChangeRequest = document.getElementById('passwordChangeRequest').value;
    var availableFlg = document.getElementById('availableFlgPersonal').value;
    var userRole = document.getElementById('userRolePersonal').value;

    var fd = new FormData();
    fd.append('email', email);
    fd.append('userName', userName);
    fd.append('companyName', companyName);
    fd.append('departmentName', departmentName);
    fd.append('employeeNo', employeeNo);
    fd.append('remarks1', remarks1);
    fd.append('remarks2', remarks2);
    fd.append('passwordChangeRequest', passwordChangeRequest);
    fd.append('availableFlg', availableFlg == 1 ? true : false);
    fd.append('userRole', userRole);

    try {
        let response = await fetch('/AdminStudents/InsertStudent', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })

        if (response.ok) {
            await messageBox("", "�f�[�^������ɍX�V����܂���", []);
        } else {
            const resjson = await response.json();
            const error = resjson.errData;
            await messageBox("", error.errorMessage, []);
        }

    } catch (error) {
        await messageBox("", error, []);
    }
}

//MUser�̊����f�[�^���X�V����@�\
async function dataUpdate() {
    var email = document.getElementById('emailPersonal').value;
    var userId = document.getElementById('userIdPersonal').value;
    var userName = document.getElementById('userNamePersonal').value;
    var companyName = document.getElementById('companyNamePersonal').value;
    var departmentName = document.getElementById('departmentNamePersonal').value;
    var employeeNo = document.getElementById('employeeNoPersonal').value;
    var remarks1 = document.getElementById('remarks1Personal').value;
    var remarks2 = document.getElementById('remarks2Personal').value;
    var passwordChangeRequest = document.getElementById('passwordChangeRequest').checked;
    var availableFlg = document.getElementById('availableFlgPersonal').value;
    var userRole = document.getElementById('userRolePersonal').value;

    var fd = new FormData();
    fd.append('email', email);
    fd.append('userId', userId);
    fd.append('userName', userName);
    fd.append('companyName', companyName);
    fd.append('departmentName', departmentName);
    fd.append('employeeNo', employeeNo);
    fd.append('remarks1', remarks1);
    fd.append('remarks2', remarks2);
    fd.append('passwordChangeRequest', passwordChangeRequest);
    fd.append('availableFlg', availableFlg == 1 ? true : false);
    fd.append('userRole', userRole);

    try {
        let response = await fetch('/AdminStudents/UpdateStudent', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })

        if (response.ok) {
            await messageBox("", "�f�[�^������ɍX�V����܂���", []);
        } else {
            await messageBox("", "�X�V�Ɏ��s���܂���", []);
        }

    } catch (error) {
        await messageBox("", error, []);
    }
}

//���[�U�ڍ׉�ʂ���폜���s���ۂ̏���
async function deleteAccountForDetail() {
    var buttons = [];
    buttons[0] = { type: "primary", text: "�͂�" };
    buttons[1] = { type: "secondary", text: "������" };
    var result = await messageBox('�폜�m�F', '�f�[�^���폜���܂��B��낵���ł���?', buttons);
    if (result == '�͂�') {
        // �u�͂��v���I�����ꂽ�Ƃ��̏���
        var userId = document.getElementById('userIdPersonal').value;
        var fd = new FormData();
        fd.append('userId', userId);

        fetch('/AdminStudents/DeleteStudent', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                if (response.ok) {
                    buttons = [{ type: "primary", text: "�͂�" }];
                    var successResult = messageBox('�폜����', '���[�U�폜�ɐ������܂����B', buttons);
                    successResult.then(() => {
                        backShowAdminStudents();
                    })
                } else {
                    messageBox("", "�폜�Ɏ��s���܂���", []);
                }
            })
            .catch(error => {
                messageBox("", error, []);
            });
    } else {
        return;
    }
}

//�ҏW�@�\�ɂăo���f�[�V������Update���s��
function newUserUpdate() {

    var resultForValidate = newValidateForm();
    if (resultForValidate == true) {
        dataUpdate();
    } else {
        return;
    }
};

//�o���f�[�V�����@�\
//�V�K�f�[�^�o�^�@�\
//element = �v�f��
//maxLength = ����������
//pattern = ������o���f�[�V�������[��
//errorMessage = �`�F�b�N���ʕs�ł������ꍇ�̃��b�Z�[�W
//notNullChecker = �󗓐���
function newValidateForm() {
    // �G���[���b�Z�[�W�G���A�����ɂ���΍폜
    var common_message_area = document.getElementById('common_message_area');
    if (common_message_area) {
        document.getElementById('common_message_area').remove();
    }

    var elements = [
        // ���[�UID�i���[���A�h���X�j
        { element: emailPersonal, maxLength: null, pattern: null, errorMessage: "�K�{���ڂł�", notNullChecker: true },
        { element: emailPersonal, maxLength: null, pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, errorMessage: "�������`���̃��[���A�h���X����͂��Ă�������", notNullChecker: false },
        { element: emailPersonal, maxLength: 255, pattern: null, errorMessage: "255�����ȓ��œ��͂��Ă�������", notNullChecker: false },

        // ����
        { element: userNamePersonal, maxLength: null, pattern: null, errorMessage: "�K�{���ڂł�", notNullChecker: true },
        { element: userNamePersonal, maxLength: 32, pattern: null, errorMessage: "32�����ȓ��œ��͂��Ă�������", notNullChecker: false },

        // �@�l��
        { element: companyNamePersonal, maxLength: null, pattern: null, errorMessage: "�K�{���ڂł�", notNullChecker: true },
        { element: companyNamePersonal, maxLength: 128, pattern: null, errorMessage: "128�����ȓ��œ��͂��Ă�������", notNullChecker: false },

        // ������
        { element: departmentNamePersonal, maxLength: 128, pattern: null, errorMessage: "128�����ȓ��œ��͂��Ă�������", notNullChecker: false },

        // �Ј��ԍ�
        { element: employeeNoPersonal, maxLength: null, pattern: /^[a-zA-Z0-9]*$/, errorMessage: "���p�p���œ��͂��Ă�������", notNullChecker: false },
        { element: employeeNoPersonal, maxLength: 16, pattern: null, errorMessage: "16�����ȓ��œ��͂��Ă�������", notNullChecker: false },

        // ���l-1
        { element: remarks1Personal, maxLength: 64, pattern: null, errorMessage: "64�����ȓ��œ��͂��Ă�������", notNullChecker: false },

        // ���l-2
        { element: remarks2Personal, maxLength: 64, pattern: null, errorMessage: "64�����ȓ��œ��͂��Ă�������", notNullChecker: false },

        // ��u��
        { element: availableFlgPersonal, maxLength: null, pattern: null, errorMessage: "�K�{���ڂł��B", notNullChecker: true },

        // �Ǘ��O���[�v
        { element: userRolePersonal, maxLength: null, pattern: null, errorMessage: "�K�{���ڂł��B", notNullChecker: true }
    ];

    var isValid = true;
    var target = "";
    var isRequiredErr = false;
    var isSame = false;

    elements.forEach(function (item) {
        if (target == "") {
            target = item.element;
            item.element.title = "";
            isRequiredErr = false;
        } else {
            if (target.name.localeCompare(item.element.name) == 0) {
                isSame = true;
            } else {
                isSame = false;
                isRequiredErr = false;
                target = item.element;
                item.element.title = "";
            }
        }

        $(".error").css({
            border: '1px solid #ced4da',
        });
        if (!isSame) {
            item.element.classList.remove('error');
        }

        // �K�{�`�F�b�N
        if (item.notNullChecker) {
            if (item.element.value.trim() === "") {
                item.element.classList.add("error");
                item.element.title = "���̍��ڂ͕K�{�ł�";
                isValid = false;
                isRequiredErr = true;
            }
        }

        // �K�{�G���[�Ȃ�`�F�b�N���Ȃ�
        if (!isRequiredErr) {
            // �p�^�[���`�F�b�N
            if (item.pattern && !item.pattern.test(item.element.value)) {
                item.element.classList.add("error");
                item.element.title = isSame ? (item.element.title != '') ? (item.element.title + '\n' + item.errorMessage) : item.errorMessage : item.errorMessage;
                isValid = false;
            }

            // �����`�F�b�N
            if (item.maxLength && item.element.value.length > item.maxLength) {
                item.element.classList.add("error");
                item.element.title = isSame ? (item.element.title != '') ? (item.element.title + '\n' + item.errorMessage) : item.errorMessage : item.errorMessage;
                isValid = false;
            }
        }
    });

    if (isValid) {
        return true;
    } else {
        $(".error").css({
            border: '2px solid red'
        });
        displayErrorMessage(['���͒l�Ɉُ킪����܂��A�C�����Ă�������']);
        return false;
    }
}

// �ʕҏW��ʁi�V�K�E�X�V�j����ꗗ��ʂ֖߂�i�L�����Z���{�^�������j
function backShowAdminStudents() {
    var fm = document.getElementById("returnSearchedKeyWords");
    fm.submit();
}