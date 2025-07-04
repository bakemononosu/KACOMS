const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

// Table�̍�������
$(window).on('load resize', function () {
    // ���������̌v�Z���~�߂�
    return;

    var elemtMain = document.querySelector('main');
    var mainHeight;

    if (elemtMain) mainHeight = elemtMain.clientHeight;

    $('.table-container').css('height', mainHeight - 150);
});

// �R�[�X�ʓo�^��ʂւ̑J��
async function handleShowCourseChapters(courseId) {
    // URL��g�ݗ��Ă�
    if (courseId == undefined) {
        var url = `/AdminCourses/ShowCourseChapters`;
    } else {
        var url = `/AdminCourses/ShowCourseChapters/${courseId}`;
    }
    // ���_�C���N�g
    window.location.href = url;
};

// �D��Q�Ɛ�敪�܂��͌��J�t���O�̍X�V
function changeLineData(courseId, registType, syscodeClass, avalableFlg) {
    var targetElement = document.getElementById(registType + courseId)

    var fd = new FormData();
    fd.append('courseId', courseId)
    if (syscodeClass == "") {
        fd.append('publicFlg', targetElement.checked)
    } else {
        fd.append('primaryReference', targetElement.value)
    }

    fetch('/AdminCourses/UpdateMCourse', {
        method: 'POST',
        headers: {
            RequestVerificationToken: csrfToken,
        },
        body: fd,
    })
        .then(response => {
            if (response.ok) {
                // �����ځi��ʁj��̌��J/����J��؂�ւ���
                changePublic(courseId, avalableFlg);
            } else {
                messageBox("", "�X�V�Ɏ��s���܂���", []);
            }
        })
        .catch(error => {
            messageBox("", error, []);
        });
}

// �����ځi��ʁj��̌��J/����J��؂�ւ���
function changePublic(courseId, avalableFlg) {
    // ���J����J
    var changeClasses = ['bg-secondary', 'bg-warning']
    var changeTexts = ['����J', '���J��']

    // ���J/����J�G�������g
    var parent = '#isPublic_' + courseId;
    var elmIsPublic = document.querySelector(parent + ' > .badge');

    // �D��Q�Ɛ�G�������g
    var targetId = '#primaryReference_' + courseId;
    var elmPrimaryReference = document.querySelector(targetId);

    // �폜�w��C���f�b�N�X
    var deleteIndex = 0;
    // �ǉ��w��C���f�b�N�X
    var addIndex = 0;

    if (elmPrimaryReference.value == 1) {
        // ���J�t���O
        var pubFlgId = '#publicFlg_' + courseId;
        var elmPubFlg = document.querySelector(pubFlgId);
        if (elmPubFlg.checked) {
            deleteIndex = Number(elmPubFlg.checked)
            addIndex = Number(elmPubFlg.checked)

        } else {
            deleteIndex = Number(!elmPubFlg.checked)
            addIndex = Number(elmPubFlg.checked)
        }
    } else {
        // ���J����
        if (avalableFlg == 1) {
            deleteIndex = Number(avalableFlg)
            addIndex = Number(avalableFlg)
        } else {
            deleteIndex = Number(!avalableFlg)
            addIndex = Number(avalableFlg)
        }
    }

    // DOM����
    elmIsPublic.classList.remove(changeClasses[deleteIndex]);
    elmIsPublic.classList.add(changeClasses[addIndex]);
    elmIsPublic.innerText = changeTexts[addIndex];
}

// �폜�t���O�̍X�V
async function handleDeleteCourse(courseId) {
    var buttons = [];
    buttons[0] = { type: "primary", text: "�͂�" };
    buttons[1] = { type: "secondary", text: "������" };
    var result = await messageBox('�폜�m�F', '�u���Ƃ���ɕR�Â��Z�N�V�������폜���܂��B</br>��낵���ł����H', buttons)

    if (result == '�͂�') {
        // �u�͂��v���I�����ꂽ�Ƃ��̏���
        var fd = new FormData();
        fd.append('courseId', courseId)

        try {
            let response = await fetch('/AdminCourses/DeleteCourse', {
                method: 'POST',
                headers: {
                    RequestVerificationToken: csrfToken,
                },
                body: fd,
            })

            if (response.ok) {
                document.getElementById('tr_' + courseId).remove()
                await messageBox("", "�폜�ɐ������܂���", []);
            } else {
                const resjson = await response.json();
                const error = resjson.errData;
                await messageBox("", error.errorMessage, []);
            }
        } catch (error) {
            await messageBox("", error, []);
        }
    }
    else {
        // �u�������v���I�����ꂽ�Ƃ��̏���
        return
    }
}