
// Table�̍�������
$(window).on('load resize', function () {
    // ���������̌v�Z���~�߂�
    return;
    var window_height = (window.innerHeight ? window.innerHeight : $(window).innerHeight()) - 130;

    // �t�b�^�[�܂ł̍���
    $('.detail-area').css('height', window_height + 'px');

    // �u���ꗗ�i�X�N���[���̈�j�̍���
    $('.table-container').css('height', (window_height - 50) + 'px');

});


async function MoveView(methodName) {
    var userId = document.getElementById('userId').value;
    var courseId = document.getElementById('courseId').value;
    var chapterId = document.getElementById('chapterId').value;

    // URL��g�ݗ��Ă�
    if (methodName == "ShowMyChapter") {
        var url = `/StudentMyCourse/${methodName}/${userId}/${courseId}`;
    } else {
        var url = `/StudentMyCourse/${methodName}/${userId}/${courseId}/${chapterId}`;
    }

    if (methodName == 'ShowTestContents') {
        processingMessage('���쐬���ł��B���΂炭���҂����������B');
    }

    // ���_�C���N�g
    window.location.href = url;
};