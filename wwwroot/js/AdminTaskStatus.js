const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

// Table�̍���
$(window).on('load resize', function () {

    resizeTable();
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

    // table�i�X�N���[���̈�j�̍����Z�b�g
    $('.table-container').css('height', (window_height - pos.top + 50) + 'px');
}

// ���������̑I����Ԃɂ���ʑJ�ڕ���
function showStudentCoursesStatus(userId) {
    document.getElementById('userId').value = userId;
    selectedCourse = document.getElementById('enteredCourse').value;
    var fm = document.getElementById("searchedKeyWords");
    if (selectedCourse) {
        fm.setAttribute('action', 'ShowStudentChaptersStatus');
    } else {
        fm.setAttribute('action', 'ShowStudentCoursesStatus');
    }

    fm.submit();
}

/**
 * CSV�_�E�����[�h�N���b�N�C�x���g�n���h��
 */
$('#download_csv').on('click', async function (e) {
    var fd = new FormData();
    // �@�l��
    fd.append('corpName', $('#companyName').val());
    fd.append('userName', $('#userName').val());
    fd.append('courseId', $(`[id='courseId'] option:selected`).val());
    fd.append('pRateS', $(`[name='minRate']`).val());
    fd.append('pRateE', $(`[name='maxRate']`).val());

    try {
        var response = await fetch(`/AdminTaskStatus/DownloadCsv`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': csrfToken,
            },
            body: fd,
        });

        if (response.ok) {
            var dt = new Date();
            var today = `${dt.getFullYear()}${dt.getMonth() + 1}${dt.getDate()}${dt.getHours()}${dt.getMinutes()}${dt.getSeconds()}`;
            var data = await response.blob();
            let a = document.createElement('a');
            a.href = URL.createObjectURL(data);
            a.download = `�u���i��_${today}.csv`;
            a.click();
            a.remove();
        }
    }
    catch (e) {
        Console.log(e.message);
    }
});