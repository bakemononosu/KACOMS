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

function CallShowCourseStudentTestStatus(userId) {
    document.getElementById('userId').value = userId;
    var fm = document.getElementById("searchedKeyWords");
    fm.setAttribute('action', 'ShowCourseStudentTestStatus');

    fm.submit();
}

function backShowCourseTestStatus() {
    var fm = document.getElementById("returnSearchedKeyWords")
    fm.submit();
}