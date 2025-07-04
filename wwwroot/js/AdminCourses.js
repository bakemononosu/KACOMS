const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

//--------------------------------------------------------------------------------------------
// 講座一覧の高さ調整
//--------------------------------------------------------------------------------------------
// 初期表示、リサイズ時
$(window).on('load resize', function () {

    resizeCourseChapter();
});

// コース情報を開閉時
$('#accordionExample').on('hidden.bs.collapse shown.bs.collapse', function () {
    resizeCourseChapter();
});

function resizeCourseChapter() {
    // 高さ調整の計算を止める
    return;

    // ウインドウの高さを取得
    var window_height = (window.innerHeight ? window.innerHeight : $(window).innerHeight()) - 130;

    // 講座一覧の位置を取得
    var pos = $('.table-container').offset();

    // フッターまでの高さセット
    $('.sub-container').css('height', window_height + 'px');

    // 講座一覧（スクロール領域）の高さセット
    $('.table-container').css('height', (window_height - pos.top + 25) + 'px');
}

//--------------------------------------------------------------------------------------------
// 各編集処理
//--------------------------------------------------------------------------------------------
function changePublic(publicFlg) {
    $('#PublicFlg').val(publicFlg);
}

//--------------------------------------------------------------------------------------------
// 各登録処理
//--------------------------------------------------------------------------------------------

// 「登録」ボタン押下
async function updateCourse() {

    // 入力チェック
    if (!validateCourse()) {
        return;
    }

    // 送信パラメータを生成
    var courseId = document.getElementById('CourseId').value;
    var courseName = document.getElementById('CourseName').value;
    var courseExplaination = document.getElementById('CourseExplaination').value;
    var begineDateTime = document.getElementById('BegineDateTime').value;
    var endDateTime = document.getElementById('EndDateTime').value;
    var publicFlg = document.getElementById('PublicFlg').value;
    var primaryReference = $('input[name="PrimaryReference"]:checked').val();

    var fd = new FormData();
    fd.append('CourseId', courseId);
    fd.append('CourseName', courseName);
    fd.append('CourseExplaination', courseExplaination);
    fd.append('BegineDateTime', begineDateTime);
    fd.append('EndDateTime', endDateTime);
    fd.append('PublicFlg', publicFlg);
    fd.append('PrimaryReference', primaryReference);

    // 送信実行
    fetch('/AdminCourses/UpdateCourse', {
        method: 'POST',
        headers: {
            RequestVerificationToken: csrfToken,
        },
        body: fd,
    })
        .then(response => {
            if (response.ok) {

                if (courseId == '') {
                    response.json().then(data => {
                        // 採番したコース識別子にて、更新モードへ移動
                        location.href = '/AdminCourses/ShowCourseChapters/' + data.newCourseId;
                    });
                } else {
                    this.messageBox('', 'データが正常に登録されました', []);
                }

            } else {
                displayErrorMessage(['登録できませんでした']);
            }
        })
        .catch(error => {
            displayErrorMessage(['登録できませんでした']);
        });
}


// 講座一覧「削除」押下
async function deleteChapter(chapterId, courseId) {

    // 確認ダイアログを表示
    var buttons = [];
    buttons[0] = { type: "primary", text: "はい" };
    buttons[1] = { type: "secondary", text: "いいえ" };
    var result = await messageBox('削除確認', 'セクション及びそれに紐づくコンテンツ情報を削除します。よろしいですか?', buttons);

    // 「はい」が選択された場合のみ実行
    if (result == 'はい') {

        // 送信パラメータを生成
        var fd = new FormData();
        fd.append('ChapterId', chapterId);
        fd.append('CourseId', courseId);

        try {
            // 送信を実行
            let response = await fetch('/AdminCourses/DeleteChapter', {
                method: 'POST',
                headers: {
                    RequestVerificationToken: csrfToken,
                },
                body: fd,
            })

            if (response.ok) {
                // 対象行を削除
                $('#tr_' + chapterId).remove();
                await messageBox("", "データが正常に削除されました", []);
            } else {
                const resjson = await response.json();
                const error = resjson.errData;
                displayErrorMessage([error.errorMessage]);
            }
        } catch (error) {
            displayErrorMessage(['削除できませんでした']);
        }
    }

}


//------------------------------------------------------------------
// 入力チェック
//------------------------------------------------------------------
function validateCourse() {

    // 公開フラグが指定されている
    var priFlag = document.getElementById("PriFlag").checked;

    // 入力チェック一覧
    var elements = [
        { element: CourseName, maxLength: 32, pattern: null, notNullChecker: true, errorMessage: null },
        { element: CourseExplaination, maxLength: 128, pattern: null, notNullChecker: true, errorMessage: null },
        { element: BegineDateTime, maxLength: null, pattern: null, notNullChecker: !priFlag, errorMessage: null },
        { element: EndDateTime, maxLength: null, pattern: null, notNullChecker: !priFlag, errorMessage: null },
        { element: PublicFlg, maxLength: null, pattern: null, notNullChecker: priFlag, errorMessage: null },
    ];

    var isValid = true;

    clearValidateError();
    elements.forEach(function (item) {
        if (!validateItem(item)) {
            isValid = false;
        }
    });
    createValidateError();

    if (isValid) {
        //公開期間の開始、終了日の正当性確認
        if (BegineDateTime.value > EndDateTime.value) {
            elements[2].element.classList.add('error');//BegineDateTime
            elements[3].element.classList.add('error');//EndDateTime
            displayErrorMessage(["公開期間の開始日、終了日が前後しています。"]);
            createValidateError();
            return false;
        }
        return true;
    } else {
        displayErrorMessage(["入力値に異常があります、修正してください"]);
        return false;
    }
}

// エラーのスタイルの変更
// aspのタグ生成で個別スタイルが効かなくなるので、直でスタイル指定を行っている
function createValidateError() {
    $(".error").css({
        border: '2px solid red',
    });
}
function clearValidateError() {
    $(".error").css({
        border: '1px solid #ced4da',
    });
}

// 項目チェック
function validateItem(item) {
    var isValid = true;

    // 前回のエラー判定があれば解除
    item.element.classList.remove('error');

    // 入力形式、桁数チェック
    var valueLength = item.element.value.length;
    if (item.pattern && !item.pattern.test(item.element.value)) {
        // 入力形式の不正
        item.element.classList.add('error');
        item.element.title = item.errorMessage;
        isValid = false;
    } else if (item.maxLength && item.element.value.length > item.maxLength) {
        // 長さが不正
        item.element.classList.add('error');
        item.element.title = item.errorMessage ?? item.maxLength + '文字以下で入力してください';
        isValid = false;
    } else {
        item.element.title = '';
    }

    // 必須チェック
    if (item.notNullChecker) {
        if (item.element.value.trim() === '') {
            item.element.classList.add('error');
            item.element.title = '必須です';
            isValid = false;
        }
    }

    // 日付の不正値チェック
    if (item.element.id == 'BegineDateTime' || item.element.id == 'EndDateTime') {
        var isBadDateTime = document.getElementById(item.element.id).validity.badInput;
        if (isBadDateTime) {
            item.element.title = '日付が不正です';
            isValid = false;
        }
    }

    return isValid;
}

// 講座一覧「学習順序」の変更
//テーブルをリストにする機能
function extractDataFromTable() {
    let tableId = "chapterTablelData";
    const tableElement = document.getElementById(tableId);
    const rows = tableElement.querySelectorAll('tr');
    const extractedDataArray = [];
    rows.forEach(rowElement => {
        const rowId = rowElement.id;
        const chapterId = rowId.replace('tr_', '');

        let orderNo = '';
        const orderNoElement = rowElement.querySelector(`input[id^="OrderNo_"]`);
        if (orderNoElement) {
            orderNo = orderNoElement.value.trim();
        }
        const extractedData = {
            ChapterId: chapterId,
            OrderNo: orderNo,
        };
        // 空の要素を排除する
        if (chapterId && orderNo) {
            extractedDataArray.push(extractedData);
        }
    });
    return extractedDataArray;
}

//controllerに投げる機能
async function updateOrderNoByList(courseId) {

    // エラーメッセージエリアが既にあれば削除
    var common_message_area = document.getElementById('common_message_area');
    if (common_message_area) {
        document.getElementById('common_message_area').remove();
    }

    clearValidateError();
    const fd = new FormData();
    let tableCourseChapterData = extractDataFromTable();
    let isValid = true; // isValidをループの外で定義する
    for (let i = 0; tableCourseChapterData.length > i; i++) {
        var OrderNo = document.getElementById('OrderNo_' + i);
        if (OrderNo != null) {
            isValid = validateItem({ element: OrderNo, maxLength: 2, pattern: /^([1-9]|[1-9][0-9])$/, notNullChecker: true, errorMessage: '2桁以下の数字で入力してください' });
            // エラーがあればここで終了
            if (!isValid) {
                displayErrorMessage(['入力値に異常があります、修正してください']);
                resizeCourseChapter();
                return;
            }
        }
    }
    createValidateError();
    var orderNo = [];
    var listCount = Number($('#listCount').val());
    var orderError = false;

    // 全ての「学習順序」を配列に取得
    for (var i = 0; i < listCount; i++) {
        var txtno = document.getElementById('OrderNo_' + i);
        if (txtno) { // 行削除されていたら除く
            orderNo[orderNo.length] = Number(txtno.value);
        }
    }
    // 順番に並び替え
    orderNo.sort(function (a, b) { return a - b; });
    for (var i = 0; i < orderNo.length; i++) {
        // 1からの連番であることをチェック
        if (orderNo[i] != i + 1) {
            orderError = true;
        }
    }
    if (orderError) {
        displayErrorMessage(['学習順が重複しているか、または連続していません']);
        resizeCourseChapter();
        return;
    }
    //リストにしたテーブルをJSON形式に変換しcontrollerへ
    const fixedTableCourseChapterData = JSON.stringify(tableCourseChapterData);
    fd.append('fixedTableCourseChapterData', fixedTableCourseChapterData);

    fd.append('CourseId', courseId);

    try {
        let response = await fetch('/AdminCourses/UpdateChapterOrderNo', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })

        if (response.ok) {
            // personalCourseIdの要素を取得
            let personalCourseElement = document.getElementById('CourseId').value;
            if (personalCourseElement) {
                var url = `/AdminCourses/ShowCourseChapters/${personalCourseElement}`;
                // リダイレクト
                window.location.href = url;
            } else {
                console.error('リダイレクト処理が正常に完了しませんでした');
            }
        } else {
            const resjson = await response.json();
            const error = resjson.errData;
            displayErrorMessage([error.errorMessage]);
            resizeCourseChapter();
        }
    } catch (error) {
        displayErrorMessage(['学習順を更新できませんでした']);
        resizeCourseChapter();
    }
}
