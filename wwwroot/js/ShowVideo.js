const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;
var video = document.getElementById('videotag');

// �p�������u�����X�g��selected�ύX
$(window).on('load', function () {
    var index = 0;
    var orderNo = document.getElementById('orderNo').value;
    if (orderNo) {
        index = orderNo - 1;
    }

    var selectList = document.getElementById('breadcrumb_chapter');
    if (selectList) {
        var currentIndex = selectList.selectedIndex;
        if ((currentIndex != undefined) && (currentIndex >= 0)) {
            if (currentIndex != index) {
                selectList.options[index].selected = true;
            }
        }
    }
});

if (video) {
    // �Đ��J�n�C�x���g�̃��X�i�[��ǉ�
    video.addEventListener("play", function () {
        var userId = document.getElementById('userId').value;
        var chapterId = document.getElementById('chapterId').value;
        var type = 'start'
        UpdateStartEndDatetime(userId, chapterId, type);
    }, false);

    // �I���C�x���g�̃��X�i�[��ǉ�
    video.addEventListener("ended", function () {
        var userId = document.getElementById('userId').value;
        var chapterId = document.getElementById('chapterId').value;
        var type = 'end'
        UpdateStartEndDatetime(userId, chapterId, type);
    }, false);
}

// �Đ��J�n�A�I���Ŏ�u�ҍu���f�[�^�̍X�V
async function UpdateStartEndDatetime(userId, chapterId, type) {
    var fd = new FormData();
    fd.append('userId', userId)
    fd.append('chapterId', chapterId)
    fd.append('type', type)

    fetch('/StudentMyCourse/UpdateUserChapter', {
        method: 'POST',
        headers: {
            RequestVerificationToken: csrfToken,
        },
        body: fd,
    })
        .then(response => {
            if (!response.ok) {
                displayErrorMessage(['��u�f�[�^�̍X�V�Ɏ��s���܂����B�Ǘ��҂ɖ₢���킹�Ă��������B']);
            }
        })
        .catch(error => {
            messageBox("", error, []);
        });
}

async function ShowPreviousNextChapter(type) {
    var userId = document.getElementById('userId').value;
    var courseId = document.getElementById('courseId').value;
    var chapterId = document.getElementById('chapterId').value;

    // URL��g�ݗ��Ă�
    var url = `/StudentMyCourse/ShowPreviousNextChapter/${userId}/${courseId}/${chapterId}/${type}`;
    // ���_�C���N�g
    window.location.href = url;
};