/** 前、次セクション移動情報 */
const userId = document.getElementById('user_id').value;
const courseId = document.getElementById('course_id').value;
const chapterId = document.getElementById('chapter_id').value;
const videoContents = document.getElementById('contents_type_video').value;
const testContents = document.getElementById('contents_type_test').value;

// テスト実施回数
const nTimes = $('#total_times').val();
pageControllerInit(nTimes, 1, showPage);

function showPage(page) {
    //console.log(page);

    // 全ての採点結果を非表示にする
    $(`div[id^='result_p_']`).each(function (i, e) {
        $(e).addClass('d-none');
    });

    // 選択された採点結果を表示する
    $(`div[id^='result_p_${page}']`).removeClass('d-none');


    // 全てのテスト履歴を非表示にする
    $(`div[id^='history_p_']`).each(function (i, e) {
        $(e).addClass('d-none');
    });

    // 選択された履歴を表示する
    $(`div[id^='history_p_${page}']`).removeClass('d-none');
}

/**
 * 問題番号クリックイベントハンドラ登録
 */
document.querySelectorAll('.test-no').forEach((elm) => {
    elm.addEventListener('click', ChangeTest);
})

/**
 * 解答チェックボックス変更イベントハンドラ
 */
function ChangeTest() {
    var idxs = this.id.match(/t_no_(\d+)_(\d+)/);
    var testContentsId = `p_${idxs[1]}_q${idxs[2]}_tab_pane`;

    // 問題を全て非表示
    $(`div[id^='p_${idxs[1]}_q']`).each(function (i, e) {
        $(e).addClass('d-none');
    });

    // 選択された問題を表示
    $(`#${testContentsId}`).removeClass('d-none');

    // 
    $(`[id^='t_no_${idxs[1]}']`).each(function (i, e) {
        $(e).removeClass('test-no-selected');
    })
    $(`#${this.id}`).addClass('test-no-selected');
}

// 前講座ボタンクリックイベントハンドラ
$('#prev_chapter').click(function () {
    var prevChapterId = $('#prev_chapter_id').val();
    var type = $('#prev_contents_type').val();

    var action = (type == videoContents) ? "ShowVideoContents" : "ConfirmExamination";

    location.href = `/StudentMyCourse/${action}/${userId}/${courseId}/${prevChapterId}`;
});


// 次講座ボタンクリックイベントハンドラ
$('#next_chapter').click(function () {
    var nextChapterId = $('#next_chapter_id').val();
    var type = $('#next_contents_type').val();

    var action = (type == videoContents) ? "ShowVideoContents" : "ConfirmExamination";

    location.href = `/StudentMyCourse/${action}/${userId}/${courseId}/${nextChapterId}`;
});
