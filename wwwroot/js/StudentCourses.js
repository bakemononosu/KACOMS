const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;


// Table�̍�������
$(window).on('load resize', function () {
    // ���������̌v�Z���~�߂�
    return;

    var window_height = (window.innerHeight ? window.innerHeight : $(window).innerHeight()) - 130;

    // �t�b�^�[�܂ł̍���
    $('.detail-area').css('height', window_height + 'px');

    // �R�[�X�ꗗ�i�X�N���[���̈�j�̍���
    $('.table-container').css('height', (window_height - 50) + 'px');

});

// ��u�{�^���������̏���
async function addMyCourse(courseId, courseName) {
    // �G���[���b�Z�[�W�G���A�����ɂ���΍폜
    var common_message_area = document.getElementById('common_message_area');
    if (common_message_area) {
        document.getElementById('common_message_area').remove();
    }
    // �m�F�_�C�A���O��\��
    var buttons = [];
    buttons[0] = { type: "primary", text: "�͂�" };
    buttons[1] = { type: "secondary", text: "������" };
    var result = await messageBox('�u����u�m�F', '�u' + courseName + '�v���}�C�u���ɒǉ����܂��B��낵���ł���?', buttons);

    // �u�͂��v���I�����ꂽ�ꍇ�̂ݎ��s
    if (result == '�͂�') {

        // �p�����[�^��ݒ�        
        var fd = new FormData();
        fd.append('courseId', courseId);
        fd.append('deletedFlg', false);

        fetch('/StudentCourses/UpdateMyCourse', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
        .then(response => {

            if (response.ok) {
                return response.json()
            } else {
                //messageBox('', '�ǉ��Ɏ��s���܂���', []);
                displayErrorMessage(['�ǉ��Ɏ��s���܂���']);
            }
        })
        .then(res => {
            if (res.status == 'OK') {
                // ��u�E�����{�^���̊����E�񊈐���؂�ւ�
                switchActive($('#add_' + courseId), 'bg-primary', false);
                switchActive($('#del_' + courseId), 'bg-danger', true);
            }
            messageBox('', res.message, []);
        })
        .catch(error => {
            messageBox('', error, []);
        });
    }
}

// �����{�^���������̏���
async function deleteMyCourse(courseId, courseName, availableFlg) {
    // �G���[���b�Z�[�W�G���A�����ɂ���΍폜
    var common_message_area = document.getElementById('common_message_area');
    if (common_message_area) {
        document.getElementById('common_message_area').remove();
    }
    // �m�F�_�C�A���O��\��
    var buttons = [];
    buttons[0] = { type: "primary", text: "�͂�" };
    buttons[1] = { type: "secondary", text: "������" };
    var result = await messageBox('�u�������m�F', '�u' + courseName + '�v���}�C�u������폜���܂��B��낵���ł���?', buttons);

    // �u�͂��v���I�����ꂽ�ꍇ�̂ݎ��s
    if (result == '�͂�') {

        // �p�����[�^��ݒ�        
        var fd = new FormData();
        fd.append('courseId', courseId);
        fd.append('deletedFlg', true);

        fetch('/StudentCourses/UpdateMyCourse', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
        .then(response => {

            if (response.ok) {
                return response.json();
            } else {
                //messageBox('', '�폜�Ɏ��s���܂���', []);
                displayErrorMessage(['�폜�Ɏ��s���܂���']);

            }
        })
        .then(res => {
            if (res.status == 'OK') {
                // ��u�E�����{�^���̊����E�񊈐���؂�ւ�
                if (availableFlg) {  // ��u�\�ȏꍇ�̂�
                    switchActive($('#add_' + courseId), 'bg-primary', true);
                }
                switchActive($('#del_' + courseId), 'bg-danger', false);
            }
            messageBox('', res.message, []);
        })
        .catch(error => {
            messageBox('', error, []);
        });
    }
}

// �{�^���̊����E�񊈐��̐؂�ւ� ����
function switchActive(btn, onclass, active) {
    if (active) {
        btn.addClass(onclass + ' play-on');
        btn.removeClass('bg-secondary play-off');
    } else {
        btn.addClass('bg-secondary play-off');
        btn.removeClass(onclass + ' play-on');
    }
}



