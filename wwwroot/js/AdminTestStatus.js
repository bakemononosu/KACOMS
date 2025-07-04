const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

// Tableの高さ
$(window).on('load resize', function () {

    resizeTable();
});

// accordionを開閉時
$('#collapseOne').on('hidden.bs.collapse shown.bs.collapse', function () {
    resizeTable();
});

// Tableの高さ調整
function resizeTable() {
    // 高さ調整の計算を止める
    return;

    // ウインドウの高さを取得
    var window_height = (window.innerHeight ? window.innerHeight : $(window).innerHeight()) - 130;

    // tableの位置を取得
    var pos = $('.table-container').offset();

    // フッターまでの高さセット
    $('.content-main').css('height', window_height + 'px');

    // table（スクロール領域）の高さセット
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