// CSRFトークン
const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

const userId = document.getElementById('user_id').value;
const courseId = document.getElementById('course_id').value;
const chapterId = document.getElementById('chapter_id').value;
const videoContents = document.getElementById('contents_type_video').value;
const testContents = document.getElementById('contents_type_test').value;
const isDisplayMode = document.getElementById('is_display_mode').value;

let timeOut = parseInt($('#limit_time').val()) * 1000;
let timeoutId = undefined;
let startTime = undefined; 

/**
 * 制限時間タイムアウトイベントハンドラ
 */
setTimeoutFunc();

if (!isDisplayMode) {
    $(window).on('beforeunload', function (e) {
        e.preventDefault();
        return 'ページ移動します。採点結果は全て0点になりますがよろしいですか？';
    });
}

/**
 * 解答送信ボタンクリックイベントハンドラ
 */
$('#send_answer').click(async function () {
    console.log('==>解答送信ボタンクリック');

    // タイマー解除
    clearTimeoutFunc();

    // 採点確認メッセージ
    var buttons = [{ type: 'primary', text: 'はい' }, { type: 'secondary', text: 'いいえ' }];
    var ans = await messageBox('採点処理実施確認', '採点処理を実施します。解答が未入力の問題も採点されます。よろしいですか。', buttons);
    if (ans === 'はい') {
        requestGrading();
    } else {
        // タイマー再設定
        setTimeoutFunc();
    }
});

// 前講座ボタンクリックイベントハンドラ
$('#prev_chapter').click(function () {
    var prevChapterId = $('#prev_chapter_id').val();
    var type = $('#prev_contents_type').val();

    var action = (type == videoContents)? "ShowVideoContents" : "ConfirmExamination";

    console.log(`type:${type} action:${action}`);

    location.href = `/StudentMyCourse/${action}/${userId}/${courseId}/${prevChapterId}`;
});


// 次講座ボタンクリックイベントハンドラ
$('#next_chapter').click(function () {
    var nextChapterId = $('#next_chapter_id').val();
    var type = $('#next_contents_type').val();

    var action = (type == videoContents) ? "ShowVideoContents" : "ConfirmExamination";

    console.log(`type:${type} action:${action}`);

    location.href = `/StudentMyCourse/${action}/${userId}/${courseId}/${nextChapterId}`;
});


/**
 * 採点する
 */
function requestGrading() {
    // 解答送信リクエストデータ作成
    var request = createSendRequest();
    $('#json_param').val(JSON.stringify(request));

    $(window).off('beforeunload');
    document.form.submit();
}

/**
 * 解答送信リクエストデータを作成する
 * 
 * @returns
 */
function createSendRequest() {
    var request = {};
    var qList = [];

    var q = 1;
    $(`ul[id='myTab'] > li > button`).each(function (index, element) {
        $(`div[id='${$(element).attr('id')}_pane']`).each(function (index, element) {
            var question = {};
            var aList = [];
            question['QText'] = '';                 // $(element).find(`div[id^='text_q'] > h3`).text();
            question['QImage'] = '';                // $(element).find(`div[id^='img_q'] > img`).attr('src');
            question['QId'] = $(element).find(`div[id^='guid_q']`).text();
            $(element).find(`div[id^='a_${q}_'`).each(function (index, element) {
                var answer = {};
                answer['AValue'] = $(element).find(`input`).prop('checked');
                answer['AId'] = $(element).find(`div[id^='guid_']`).text();
                answer['AText'] = '';               // $(element).find(`div[id^='text_'] > h3`).text();
                answer['AImage'] = '';              // $(element).find(`div[id^='text_'] > img`).attr('src');
                answer['Status'] = '';
                aList.push(answer);
            });
            question['Answers'] = aList;
            qList.push(question);
        });
        q++;
    });

    request['IsDisplayMode'] = false;
    request['UserChapterId'] = $('#user_chapter_id').val();
    request['UserId'] = $('#user_id').val();
    request['CourseId'] = $('#course_id').val();;
    request['ChapterId'] = $('#chapter_id').val();;
    request['Times'] = parseInt($('#times').val());
    request['QuestionCount'] = q - 1;
    request['LimitTime'] = parseInt($('#limit_time').val());
    request['Questions'] = qList;
    request['ErrorMessage'] = '';
    request['PrevChaper'] = { ChapterId: $('#prev_chapter_id').val(), ContentsType: $('#prev_contents_type').val() };
    request['NextChaper'] = { ChapterId: $('#next_chapter_id').val(), ContentsType: $('#next_contents_type').val() };

    return request;
}

/**
 * タイムアウトイベントハンドラを設定する
 * @param {any} sec
 */
function setTimeoutFunc() {

    if ($('#limit_time').val() != "0") {
        startTime = Date.now();
        timeoutId = setTimeout(async () => {
            // 確認メッセージ表示
            var buttons = [{ type: 'primary', text: 'OK' }];
            var ans = await messageBox('制限時間タイムアウト', '制限時間になりました。採点を実施します。', buttons);
            // 解答送信
            requestGrading();
        }, timeOut);
    }
}

/**
 * タイムアウトを解除する
 */
function clearTimeoutFunc() {
    if ($('#limit_time').val() != "0") {
        if (timeoutId != undefined) {
            clearTimeout(timeoutId);

            // 残時間を算出
            timeOut -= (Date.now() - startTime);
        }
    }
}
