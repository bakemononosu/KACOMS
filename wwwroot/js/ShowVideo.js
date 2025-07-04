const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;
var video = document.getElementById('videotag');

// パンくず講座リストのselected変更
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
    // 再生開始イベントのリスナーを追加
    video.addEventListener("play", function () {
        var userId = document.getElementById('userId').value;
        var chapterId = document.getElementById('chapterId').value;
        var type = 'start'
        UpdateStartEndDatetime(userId, chapterId, type);
    }, false);

    // 終了イベントのリスナーを追加
    video.addEventListener("ended", function () {
        var userId = document.getElementById('userId').value;
        var chapterId = document.getElementById('chapterId').value;
        var type = 'end'
        UpdateStartEndDatetime(userId, chapterId, type);
    }, false);
}

// 再生開始、終了で受講者講座データの更新
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
                displayErrorMessage(['受講データの更新に失敗しました。管理者に問い合わせてください。']);
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

    // URLを組み立てる
    var url = `/StudentMyCourse/ShowPreviousNextChapter/${userId}/${courseId}/${chapterId}/${type}`;
    // リダイレクト
    window.location.href = url;
};