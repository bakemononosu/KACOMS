
// Tableの高さ調整
$(window).on('load resize', function () {
    // 高さ調整の計算を止める
    return;
    var window_height = (window.innerHeight ? window.innerHeight : $(window).innerHeight()) - 130;

    // フッターまでの高さ
    $('.detail-area').css('height', window_height + 'px');

    // 講座一覧（スクロール領域）の高さ
    $('.table-container').css('height', (window_height - 50) + 'px');

});


async function MoveView(methodName) {
    var userId = document.getElementById('userId').value;
    var courseId = document.getElementById('courseId').value;
    var chapterId = document.getElementById('chapterId').value;

    // URLを組み立てる
    if (methodName == "ShowMyChapter") {
        var url = `/StudentMyCourse/${methodName}/${userId}/${courseId}`;
    } else {
        var url = `/StudentMyCourse/${methodName}/${userId}/${courseId}/${chapterId}`;
    }

    if (methodName == 'ShowTestContents') {
        processingMessage('問題作成中です。しばらくお待ちください。');
    }

    // リダイレクト
    window.location.href = url;
};