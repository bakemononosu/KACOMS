/** 動画ファイル */
const selectFile = document.getElementById('select_file');
/** ファイル選択ボタン */
const browseFile = document.getElementById('browse_file');
/** ファイルドロップ領域 */
const dropArea = document.getElementById('drop_area');

/** 動画再生領域 */
const playVideo = document.getElementById('play_video');
/** input：ファイル */
const videoFile = document.getElementById('video_file');
/** video */
const player = document.getElementById('player');

const takeExam = document.getElementById('take_exam');

const videoContents = document.getElementById('contents_video').value;
const testContents = document.getElementById('contents_test').value;
const mimeType = document.getElementById('video_mime_type').value;

/** 問題カタログ選択ダイアログ */
const selectQdialog = document.getElementById('modal_q_selector');
/** 問題カタログ表示ダイアログ */
const displayQdialog = document.getElementById('modal_q_reference');

const validateCommon = [{ id: 'ChapterName', type: 'string', max: 64,max: 0, min: 0 }, { id:'OrderNo', type:'number', max:99, min:1}];
const validateVideo = [];
const validateTest = [{ id: 'Exam_Questions', type: 'number', max: 999, min:1 }, { id:'Exam_LimitTime', type:'number',max:3600, min:0}];

// CSS分離識別ID
let scopeId = browseFile.outerHTML.match(/ b-\w{10}/)[0];

// CSRFトークン
const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

/** 全選択チェッククリア */
const ClearSelectAll = () => $('#select_all').prop('checked', false);

let modalQcList = undefined;
/**
 * ファイルドロップイベントハンドラ
 */
dropArea.addEventListener('drop', (e) => {
    e.preventDefault();
    if (e.dataTransfer.items[0].kind != "file") {
        return;
    }

    let file = e.dataTransfer.items[0].getAsFile();
    if (file.type != mimeType) {
        return;
    }
    videoFile.files = e.dataTransfer.files;

    // 選択されたビデオをセット
    setVideoSouece();

});

/**
 * ファイルドラッグオーバーイベントハンドラ
 */
dropArea.addEventListener('dragover', (e) => {
    e.preventDefault();
});

/**
 * ファイル選択ボタンクリックイベントハンドラ
 */
browseFile.addEventListener('click', () => {
    videoFile.click();
});

/**
 * 選択ファイル変更イベントハンドラ
 */
videoFile.addEventListener('change', (e) => {
    if (e.target.files[0].type != mimeType) {
        return;
    }

    // 選択されたビデオをセット
    setVideoSouece();
});

/**
 * 学習コンテンツ区分変更イベントハンドラ
 */
$(`input[name = 'contentsType']:radio`).change(function() {
    if ($(this).val() == videoContents) {
        // テストコンテンツ非表示
        takeExam.classList.add('d-none');

        //console.log(videoFile.files);
        // 動画ファイルの選択状況によってVideoコンテンツの表示を切替
        if (videoFile.files.length != 0) {
            selectFile.classList.add('d-none');
            playVideo.classList.remove('d-none');
        } else {
            selectFile.classList.remove('d-none');
            playVideo.classList.add('d-none');
        }

    } else if ($(this).val() == testContents) {
        // テストコンテンツ表示
        takeExam.classList.remove('d-none');

        // Videoコンテンツ非表示
        selectFile.classList.add('d-none');
        playVideo.classList.add('d-none');
    }
})

/**
 * 問題カタログ追加(+)アイコンクリックイベントハンドラ
 */
$('.gc-plus-icon').click(function () {
    $('#modal_q_major').val('');
    $('#modal_q_middle').val('');
    $('#modal_q_minor').val('');
    $('#modal_q_body').html('');
    // ヘッダのチェックボックスのクリア
    ClearSelectAll();
    var modalQcList = new bootstrap.Modal(selectQdialog);
    modalQcList.show();
});

/**
 * 全選択/全削除チェックボックス変更イベントハンドラ
 */
$('#select_all').change(function () {
    var checked = $(this).prop('checked');

    $(`input[name='dialog_q_check']`).prop('checked', checked);
})

/**
 * 検索ボタン(問題カタログ選択ダイアログ)クリックイベントハンドラ
 */
$('#modal_q_search').click(function () {
    // ヘッダのチェックボックスのクリア
    ClearSelectAll();

    var mj = $('#modal_q_major').val();
    var md = $('#modal_q_middle').val();
    var mn = $('#modal_q_minor').val();

    var formData = new FormData();
    formData.append('majorCd', mj);
    formData.append('middleCd', md);
    formData.append('minorCd', mn);

    fetch('/AdminCourses/GetQuestionCatalogList', {
        method: 'POST',
        headers: {
            RequestVerificationToken: csrfToken,
        },
        body: formData,
    })
    .then(response => {
        return response.json();
    })
    .then(res => {
        var data = JSON.stringify(res);
        // 問題カタログ一覧の再描画
        refreshQuestionCatalogList(res.data);
    })
    .catch(error => {
        displayErrorMessage([error]);
    });
});

/**
 * 動画クリアボタンクリックイベントハンドラ
 */
$('#clear_video').click(function () {
    // Videoコンテンツクリア
    clearSelectedVideoContents();

    selectFile.classList.remove('d-none');
    playVideo.classList.add('d-none');
});

/**
 * 登録ボタンクリックイベントハンドラ
 */
$('[id^=register_').click(async function () {   
    // バリデーションチェック
    if (!inputValidation()) {
        displayErrorMessage(['入力に誤りがあります']);
        return;
    }

    document.body.parentElement.style.cursor = "wait";

    var request = {};
    var videoOld = {};
    var videoNew = {};
    var test = {};
    var qData = {};
    var qList = [];

    // コース識別子
    var courseId = $(`#course_id`).val();
    // 講座識別子
    var chapterId = $(`#chapter_id`).val();
    // 講座名
    var chapterName = $(`#ChapterName`).val();
    // 学習コンテンツ区分
    var contentsType = $(`input[name = 'contentsType']:checked`).val();
    // 学習順序
    var orderNo = $(`#OrderNo`).val();
    // 学習コンテンツ識別子
    var contentsId = $(`#contents_id`).val();


    // Videoコンテンツ情報
    var oldVName = $('#v_old_contents_name').val();
    var oldVPath = $('#v_old_contents_path').val();
    var oldVTime = $('#v_old_playback_time').val();
    var newVName = $('#v_new_contents_name').val();
    var newVPath = $('#v_new_contents_path').val();
    var newVTime = $('#v_new_playback_time').val();

    // Testコンテンツ情報
    var tName = $('#t_contents_name').val();
    var tQues = $('#Exam_Questions').val();
    var tTime = $('#Exam_LimitTime').val();

    videoOld['contentsName'] = oldVName;
    videoOld['contentsPath'] = oldVPath;
    videoOld['playbackTime'] = parseInt(oldVTime);
    videoNew['contentsName'] = newVName;
    videoNew['contentsPath'] = newVPath;
    videoNew['playbackTime'] = parseInt(newVTime);
    test['contentsName'] = tName;
    test['questions'] = tQues;
    test['limitTime'] = tTime;
    test['questionList'] = getExamList();

    request["courseId"] = courseId;
    request["chapterId"] = chapterId;
    request["contentsId"] = contentsId;
    request["chapterName"] = chapterName;
    request["contentsType"] = contentsType;
    request["orderNo"] = orderNo;
    request["videoOld"] = videoOld;
    request["videoNew"] = videoNew;
    request["exam"] = test;
    request["majorList"] = [];
    request["middleList"] = [];
    request["minorList"] = [];

    $('#post_params').val(JSON.stringify(request));

    // 動画ファイルアップロード
    if ((contentsType == videoContents) && (newVName != '')) {
        var result = await uploadVideo();
        if (result.status == 'NG') {
            displayErrorMessage(['動画ファイルのアップロードに失敗しました']);
            return;
        }

        request.videoNew.contentsPath = result.folderPath;
        //console.log(result);
    }

    fetch('/AdminCourses/RegisterChapter', {
        method: 'POST',
        headers: {
            RequestVerificationToken: csrfToken,
            'content-type': 'application/json',
        },
        body: JSON.stringify(request),
    })
    .then(response => {
        if (response.ok) {
            location.href = `/AdminCourses/ShowCourseChapters/${courseId}`;
        } else {
            displayErrorMessage(['セクション情報の更新に失敗しました']);
            document.body.parentElement.style.cursor = "default";
        }
    })
    .catch(error => {
        disolayErrorMessage([error]);
    });
});

/**
 * 反映ボタン(問題カタログ選択ダイアログ)クリックイベントハンドラ
 */
$('#modal_q_refrect').click(function () {
    var src = [];
    var dst = [];

    // チェックされている行を取得
    var chkRows = $('input[name=dialog_q_check]:checked').parents('tr');

    for (var idx = 0; idx < chkRows.length; idx++) {
        var row = chkRows[idx];
        src.unshift({
            qNo: $(row).children('td').eq(1).text(),
            qTitle: $(row).children('td').eq(2).text(),
            qType: $(row).children('td').eq(3).text(),
            qLevel: $(row).children('td').eq(4).text(),
            qScore: $(row).children('td').eq(5).text(),
            qId: $(row).children('td').eq(6).text()
        });
    }

    // 出題リストを取得
    var examRows = $('#exam_body').children('tr');
    for (var idx = 0; idx < examRows.length; idx++) {
        var row = examRows[idx];
        dst.push({
            qNo: $(row).children('td').eq(1).text(),
            qTitle: $(row).children('td').eq(2).text(),
            qType: $(row).children('td').eq(3).text(),
            qLevel: $(row).children('td').eq(4).text(),
            qScore: $(row).children('td').eq(5).text(),
            qId: $(row).children('td').eq(6).text(),
        });
    }

    // マージ
    var after = mergeExamList(dst, src);

    // 出題リスト再描画
    refreshExamList(after);

    $('#modal_close_btn').click();
});

/**
 * 選択された動画を表示する。
 * 
 */
function setVideoSouece() {
    var url = URL.createObjectURL(videoFile.files[0]);
    player.src = url;
    player.ondurationchange = function () {
        // 再生時間をセット
        $('#v_new_playback_time').val(this.duration);
    }
    $('#v_new_contents_name').val(videoFile.files[0].name);

    selectFile.classList.add('d-none');
    playVideo.classList.remove('d-none');
}

/**
 * 問題カタログ一覧を再描画する。
 * 
 * @param {any} data
 */
function refreshQuestionCatalogList(data) {

    $('#modal_q_body').html(createCatalogList(data, true));
}

/**
 * 出題リストを再表示する
 * @param {any} data    出題リスト
 */
function refreshExamList(data) {

    $('#exam_body').html(createCatalogList(data, false));

    // 登録数更新
    UpdateExamListNum();
}

/**
 * 問題カタログ、または出題リスト一覧を作成する
 * 
 * @param {any} data        問題カタログリスト
 * @param {any} isQCatalog  用途(問題カタログ一覧、出題リスト一覧)
 * @returns
 */
function createCatalogList(data, isQCatalog) {
    var html = '';
    for (var idx = 0; idx < data.length; idx++) {
        html = `${html}<tr${scopeId} id='r_${idx}'>`
        if (isQCatalog) {
            html = `${html}<td${scopeId} class='td-edit'><input type='checkbox'class='form-check-input' name='dialog_q_check' /></td>`
        } else {
            html = `${html}<td${scopeId} class='td-edit'><i class='bi bi-trash3' onclick='deleteQuestion(r_${idx});'></i>&nbsp;&nbsp;<i class='bi bi-clipboard-check' onclick='displayQAnswers(r_${idx});'></i></td>`
        }
        html = `${html}<td${scopeId} class='td-q-no'>${data[idx].qNo}</td>`
        html = `${html}<td${scopeId} class='td-title'>${data[idx].qTitle}</td>`
        html = `${html}<td${scopeId} class='td-format'>${data[idx].qType}</td>`
        html = `${html}<td${scopeId} class='td-level'>${data[idx].qLevel}</td>`
        html = `${html}<td${scopeId} class='td-score'>${data[idx].qScore}</td>`
        html = `${html}<td${scopeId} class='d-none'>${data[idx].qId}</td>`
        html = `${html}</tr>`
    }

    return html;
}

/**
 * 問題カタログ検索で選択された問題を出題リストにマージする
 * 
 * @param {any} examList    出題リスト
 * @param {any} questList   選択された問題カタログリスト
 * @returns
 */
function mergeExamList(examList, questList) {
    var result = [];
    var idxQ = 0;
    var idxE = 0;
    for ( ; idxQ < questList.length; ) {
        for ( ; idxE < examList.length; ) {
            if (questList[idxQ].qNo == examList[idxE].qNo) {
                // 問題識別番号が同じ場合は何もしない
                result.push(examList[idxE]);
                idxQ++;
                idxE++;
                break;
            }
            if ((questList[idxQ].qNo < examList[idxE].qNo)) {
                result.push(questList[idxQ]);
                idxQ++;
                break;
            }
            if ((questList[idxQ].qNo > examList[idxE].qNo)) {
                result.push(examList[idxE]);
                idxE++;
                break;
            }
        }
        if (idxE >= examList.length) {
            break;
        }
    }


    if (idxQ < questList.length) {
        for (; idxQ < questList.length; idxQ++) {
            result.push(questList[idxQ]);
        }
    } else if (idxE < examList.length) {
        for (; idxE < examList.length; idxE++) {
            result.push(examList[idxE]);
        }
    }

    return result;
}

/**
 * 画面に表示されている出題リストを取得する
 * 
 * @returns
 */
function getExamList() {
    var buf = [];
    // 出題リストを取得
    var examRows = $('#exam_body').children('tr');
    for (var idx = 0; idx < examRows.length; idx++) {
        var row = examRows[idx];
        buf.push({
            qId: $(row).children('td').eq(6).text(),
            qNo: $(row).children('td').eq(1).text(),
            qTitle: $(row).children('td').eq(2).text(),
            qType: $(row).children('td').eq(3).text(),
            qLevel: $(row).children('td').eq(4).text(),
            qScore: parseInt($(row).children('td').eq(5).text()),
        });
    }

    return buf;
}

/**
 * 選択されている動画をアップロードする
 * 
 * @returns
 */
async function uploadVideo() {
    var fd = new FormData();

    fd.append('video', videoFile.files[0]);

    var result = { status: '', folderPath: '' };
    try {
        var response = await fetch('/AdminCourses/UploadVideo', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        });

        if (response.ok) {
            var res = await response.json();
            result.folderPath = res.folderPath;
        } else {
            result.status = 'NG';
        }
    } catch (error) {
        displayErrorMessage([error]);
        result.status='NG';
    }

    return result;
}

/**
 * 出題リストから問題を削除する
 * 
 * @param {any} index 削除対象の問題のtr
 */
async function deleteQuestion(index) {
    var buttons = [{ type: 'primary', text: 'はい' }, { type: 'secondary', text: 'いいえ' }];
    var ans = await messageBox('削除確認', `出題リストから${$(index).children('td').eq(1).text()}の問題を削除します。よろしいですか。`, buttons);
    if (ans == buttons[0].text) {
        $(index).remove();
        // 登録数更新(1減算)
        UpdateExamListNum();
    }
}

/**
 * 問題カタログと解答を表示する
 * 
 * @param {any} row 選択行
 */
function displayQAnswers(row) {
    var fd = new FormData();

    fd.append('questionId', $(row).children('td').eq(6).text());

    fetch('/AdminCourses/GetAnswers', {
        method: 'POST',
        headers: {
            RequestVerificationToken: csrfToken,
        },
        body: fd,
    })
    .then((response) => {
        if (response.ok) {
            return response.json();
        }
    })
    .then((res) => {
        refreshQAnswers(res.data);

        var modalQaList = new bootstrap.Modal(displayQdialog);
        modalQaList.show();
    })
    .catch((error) => {
        displayErrorMessage([error]);
    })
}

/**
 * 問題カタログ、及び解答グループを再描画する
 * 
 * @param {any} data    問題カタログ、解答グループリスト情報
 */
function refreshQAnswers(data) {
    $('#modal_q_ref_no').val(data.qNo);
    $('#modal_q_ref_title').val(data.qTitle);
    $('#modal_q_ref_text').val(data.qText);

    if (data.qImage != '') {
        // 問題カタログイメージが登録されている場合
        $('#modal_q_ref_image').removeClass('d-none');
        $('#modal_q_ref_image').find('img').attr('src', data.qImage);
    } else {
        $('#modal_q_ref_image').addClass('d-none');
        $('#modal_q_ref_image').find('img').attr('src', '');
    }

    var html = '';
    for (var idx = 0; idx < data.aList.length; idx++) {
        var checked = (data.aList[idx].errataFlg) ? 'checked' : '';
        html += `<tr${scopeId}>`;
        html += `<td${scopeId}><div>${htmlEscape(data.aList[idx].answerText)}</div>`;
        if (data.aList[idx].answerImage != '') {
            html += `<img src='${data.aList[idx].answerImage}' style='max-width:100%; height:auto;' /></td>`;
        } else {
            html += `</td>`;
        }
        html += `<td${scopeId} class="text-center"><input class="form-check-input" type='checkbox' ${checked} disabled /></td>`;
        html += `<td${scopeId}>${htmlEscape(data.aList[idx].explanationText)}</td>`;
        html += `</tr>`;
    }

    $('#modal_q_ref_abody').html(html);
}

/**
 * 出題リスト登録数を更新する
 */
function UpdateExamListNum() {
    var html = $('#register_num').html();
    $('#register_num').html(html.replace(/\d+\<\/h6\>$/, $('#exam_body').children().length));
}

/**
 * 画面の入力項目をチェックする
 * 
 * @returns
 */
function inputValidation() {
    var error = 0;
    // 共通チェック
    validateCommon.forEach((pattern) => {
        var elm = document.getElementById(pattern.id);
        var check = validateElement(elm, pattern);
        if (!check) {
            error++;
            $(elm).attr('style', 'border: solid 2px red');
        } else {
            $(elm).attr('style', '');
        }
    })


    var contentsType = $(`input[name = 'contentsType']:checked`).val();
    if (contentsType == videoContents) {

    } else {
        validateTest.forEach((pattern) => {
            var elm = document.getElementById(pattern.id);
            var check = validateElement(elm, pattern);
            if (!check) {
                error++;
                $(elm).attr('style', 'border: solid 2px red');
            } else {
                $(elm).attr('style', '');
            }
        })
    }

    return error == 0;
}

/**
 * 選択されているVideoコンテンツをクリアする
 */
function clearSelectedVideoContents() {
    $('#player').attr({ 'src': '' });
    $('#v_new_contents_name').val('');
    $('#v_new_contents_path').val('');
    $('#v_new_playback_time').val(0);
    videoFile.value = '';
}

/**
 * 入力項目のチェック
 * 
 * @param {any} elm
 * @param {any} pattern
 * @returns
 */
function validateElement(elm, pattern) {
    var check = elm.checkValidity();
    if (check && (pattern.type == 'number')) {
        var num = parseInt($(elm).val());
        if ((num > pattern.max) || (num < pattern.min)) {
            check = false;
        }
    }

    return check;
}

/**
 * 受講されているまたは、受講されていたコースのチェック
 * 
 * @param {any} courseId
 */
async function checkExistUserCourse(courseId) {
    // 送信パラメータを生成
    var fd = new FormData();
    fd.append('CourseId', courseId);

    try {
        // 送信を実行
        let response = await fetch('/AdminCourses/CheckExistUserCourse', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })

        if (response) {
            const resjson = await response.json();
            return resjson.result;
        }
    } catch (error) {
        return;
    }
}

/**
 * 動画ファイルのロード
 */
if ($('#v_old_contents_path').val() != '') {
    loadVideo(player, $('#v_old_contents_path').val());
}