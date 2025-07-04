const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

// Tableの高さ
$(window).on('load resize', function () {
    var elmTable = document.querySelector('.table-container');
    if (elmTable) resizeTable();
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

    // table下ボタンの高さ
    var elemBlkupdate = document.querySelector('.blk_update');
    if (elemBlkupdate) blkupdateHeight = elemBlkupdate.clientHeight;

    // table（スクロール領域）の高さセット
    $('.table-container').css('height', ((window_height - pos.top + 50) - blkupdateHeight) + 'px');
}

// 利用可否の切替（更新）
async function handleToggleAvailable(userId) {
    var buttons = [];
    buttons[0] = { type: "primary", text: "はい" };
    buttons[1] = { type: "secondary", text: "いいえ" };
    var result = await messageBox('更新確認', '利用可否を更新します。よろしいですか?', buttons);

    if (result == 'はい') {
        // 「はい」が選択されたときの処理
        var switchElement = document.getElementById('switch_' + userId);
        var fd = new FormData();
        fd.append('userId', userId);
        fd.append('swichInput', switchElement.checked);

        fetch('/AdminStudents/ToggleAvailable', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                if (!response.ok) {
                    messageBox("", "利用可否の更新に失敗しました", []);
                }
            })
            .catch(error => {
                messageBox("", error, []);
            });
    }
    else {
        // 「いいえ」が選択されたときの処理
        var switchElement = document.getElementById('switch_' + userId);
        switchElement.checked = !switchElement.checked;
        return;
    }
}

// 個別登録（新規または更新）画面への遷移
function handleShowIndividual(userId) {
    document.getElementById('userId').value = userId;
    var fm = document.getElementById("searchedKeyWords");
    if (!userId || userId == undefined || userId == "" || userId.length == 0) {
        fm.setAttribute('action', 'ShowIndividualNew');
    } else {
        fm.setAttribute('action', 'ShowIndividual');
    }

    fm.submit();
}

// 一覧での行削除（更新）
async function handleDeleteStudent(userId) {
    var buttons = [];
    buttons[0] = { type: "primary", text: "はい" };
    buttons[1] = { type: "secondary", text: "いいえ" };
    var result = await messageBox('削除確認', '削除します。よろしいですか?', buttons);

    if (result == 'はい') {
        // 「はい」が選択されたときの処理
        var fd = new FormData();
        fd.append('userId', userId);

        fetch('/AdminStudents/DeleteStudent', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                if (response.ok) {
                    document.getElementById('tr_' + userId).remove();
                    messageBox("", "削除に成功しました", []);
                } else {
                    messageBox("", "削除に失敗しました", []);
                }
            })
            .catch(error => {
                messageBox("", error, []);
            });
    }
    else {
        // 「いいえ」が選択されたときの処理
        return;
    }
}

//新規追加にてバリデーションとInsertを行う
function newUserInsert() {

    var resultForValidate = newValidateForm();
    if (resultForValidate == true) {
        newDataInsert();
    } else {
        return;
    }
};

//MUserへの新規データ追加機能
async function newDataInsert() {
    var email = document.getElementById('emailPersonal').value;
    var userName = document.getElementById('userNamePersonal').value;
    var companyName = document.getElementById('companyNamePersonal').value;
    var departmentName = document.getElementById('departmentNamePersonal').value;
    var employeeNo = document.getElementById('employeeNoPersonal').value;
    var remarks1 = document.getElementById('remarks1Personal').value;
    var remarks2 = document.getElementById('remarks2Personal').value;
    var passwordChangeRequest = document.getElementById('passwordChangeRequest').value;
    var availableFlg = document.getElementById('availableFlgPersonal').value;
    var userRole = document.getElementById('userRolePersonal').value;

    var fd = new FormData();
    fd.append('email', email);
    fd.append('userName', userName);
    fd.append('companyName', companyName);
    fd.append('departmentName', departmentName);
    fd.append('employeeNo', employeeNo);
    fd.append('remarks1', remarks1);
    fd.append('remarks2', remarks2);
    fd.append('passwordChangeRequest', passwordChangeRequest);
    fd.append('availableFlg', availableFlg == 1 ? true : false);
    fd.append('userRole', userRole);

    try {
        let response = await fetch('/AdminStudents/InsertStudent', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })

        if (response.ok) {
            await messageBox("", "データが正常に更新されました", []);
        } else {
            const resjson = await response.json();
            const error = resjson.errData;
            await messageBox("", error.errorMessage, []);
        }

    } catch (error) {
        await messageBox("", error, []);
    }
}

//MUserの既存データを更新する機能
async function dataUpdate() {
    var email = document.getElementById('emailPersonal').value;
    var userId = document.getElementById('userIdPersonal').value;
    var userName = document.getElementById('userNamePersonal').value;
    var companyName = document.getElementById('companyNamePersonal').value;
    var departmentName = document.getElementById('departmentNamePersonal').value;
    var employeeNo = document.getElementById('employeeNoPersonal').value;
    var remarks1 = document.getElementById('remarks1Personal').value;
    var remarks2 = document.getElementById('remarks2Personal').value;
    var passwordChangeRequest = document.getElementById('passwordChangeRequest').checked;
    var availableFlg = document.getElementById('availableFlgPersonal').value;
    var userRole = document.getElementById('userRolePersonal').value;

    var fd = new FormData();
    fd.append('email', email);
    fd.append('userId', userId);
    fd.append('userName', userName);
    fd.append('companyName', companyName);
    fd.append('departmentName', departmentName);
    fd.append('employeeNo', employeeNo);
    fd.append('remarks1', remarks1);
    fd.append('remarks2', remarks2);
    fd.append('passwordChangeRequest', passwordChangeRequest);
    fd.append('availableFlg', availableFlg == 1 ? true : false);
    fd.append('userRole', userRole);

    try {
        let response = await fetch('/AdminStudents/UpdateStudent', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })

        if (response.ok) {
            await messageBox("", "データが正常に更新されました", []);
        } else {
            await messageBox("", "更新に失敗しました", []);
        }

    } catch (error) {
        await messageBox("", error, []);
    }
}

//ユーザ詳細画面から削除を行う際の処理
async function deleteAccountForDetail() {
    var buttons = [];
    buttons[0] = { type: "primary", text: "はい" };
    buttons[1] = { type: "secondary", text: "いいえ" };
    var result = await messageBox('削除確認', 'データを削除します。よろしいですか?', buttons);
    if (result == 'はい') {
        // 「はい」が選択されたときの処理
        var userId = document.getElementById('userIdPersonal').value;
        var fd = new FormData();
        fd.append('userId', userId);

        fetch('/AdminStudents/DeleteStudent', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                if (response.ok) {
                    buttons = [{ type: "primary", text: "はい" }];
                    var successResult = messageBox('削除成功', 'ユーザ削除に成功しました。', buttons);
                    successResult.then(() => {
                        backShowAdminStudents();
                    })
                } else {
                    messageBox("", "削除に失敗しました", []);
                }
            })
            .catch(error => {
                messageBox("", error, []);
            });
    } else {
        return;
    }
}

//編集機能にてバリデーションとUpdateを行う
function newUserUpdate() {

    var resultForValidate = newValidateForm();
    if (resultForValidate == true) {
        dataUpdate();
    } else {
        return;
    }
};

//バリデーション機能
//新規データ登録機能
//element = 要素名
//maxLength = 文字数制限
//pattern = 文字列バリデーションルール
//errorMessage = チェック結果不可であった場合のメッセージ
//notNullChecker = 空欄制限
function newValidateForm() {
    // エラーメッセージエリアが既にあれば削除
    var common_message_area = document.getElementById('common_message_area');
    if (common_message_area) {
        document.getElementById('common_message_area').remove();
    }

    var elements = [
        // ユーザID（メールアドレス）
        { element: emailPersonal, maxLength: null, pattern: null, errorMessage: "必須項目です", notNullChecker: true },
        { element: emailPersonal, maxLength: null, pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, errorMessage: "正しい形式のメールアドレスを入力してください", notNullChecker: false },
        { element: emailPersonal, maxLength: 255, pattern: null, errorMessage: "255文字以内で入力してください", notNullChecker: false },

        // 氏名
        { element: userNamePersonal, maxLength: null, pattern: null, errorMessage: "必須項目です", notNullChecker: true },
        { element: userNamePersonal, maxLength: 32, pattern: null, errorMessage: "32文字以内で入力してください", notNullChecker: false },

        // 法人名
        { element: companyNamePersonal, maxLength: null, pattern: null, errorMessage: "必須項目です", notNullChecker: true },
        { element: companyNamePersonal, maxLength: 128, pattern: null, errorMessage: "128文字以内で入力してください", notNullChecker: false },

        // 部署名
        { element: departmentNamePersonal, maxLength: 128, pattern: null, errorMessage: "128文字以内で入力してください", notNullChecker: false },

        // 社員番号
        { element: employeeNoPersonal, maxLength: null, pattern: /^[a-zA-Z0-9]*$/, errorMessage: "半角英数で入力してください", notNullChecker: false },
        { element: employeeNoPersonal, maxLength: 16, pattern: null, errorMessage: "16文字以内で入力してください", notNullChecker: false },

        // 備考-1
        { element: remarks1Personal, maxLength: 64, pattern: null, errorMessage: "64文字以内で入力してください", notNullChecker: false },

        // 備考-2
        { element: remarks2Personal, maxLength: 64, pattern: null, errorMessage: "64文字以内で入力してください", notNullChecker: false },

        // 受講可否
        { element: availableFlgPersonal, maxLength: null, pattern: null, errorMessage: "必須項目です。", notNullChecker: true },

        // 管理グループ
        { element: userRolePersonal, maxLength: null, pattern: null, errorMessage: "必須項目です。", notNullChecker: true }
    ];

    var isValid = true;
    var target = "";
    var isRequiredErr = false;
    var isSame = false;

    elements.forEach(function (item) {
        if (target == "") {
            target = item.element;
            item.element.title = "";
            isRequiredErr = false;
        } else {
            if (target.name.localeCompare(item.element.name) == 0) {
                isSame = true;
            } else {
                isSame = false;
                isRequiredErr = false;
                target = item.element;
                item.element.title = "";
            }
        }

        $(".error").css({
            border: '1px solid #ced4da',
        });
        if (!isSame) {
            item.element.classList.remove('error');
        }

        // 必須チェック
        if (item.notNullChecker) {
            if (item.element.value.trim() === "") {
                item.element.classList.add("error");
                item.element.title = "この項目は必須です";
                isValid = false;
                isRequiredErr = true;
            }
        }

        // 必須エラーならチェックしない
        if (!isRequiredErr) {
            // パターンチェック
            if (item.pattern && !item.pattern.test(item.element.value)) {
                item.element.classList.add("error");
                item.element.title = isSame ? (item.element.title != '') ? (item.element.title + '\n' + item.errorMessage) : item.errorMessage : item.errorMessage;
                isValid = false;
            }

            // 桁数チェック
            if (item.maxLength && item.element.value.length > item.maxLength) {
                item.element.classList.add("error");
                item.element.title = isSame ? (item.element.title != '') ? (item.element.title + '\n' + item.errorMessage) : item.errorMessage : item.errorMessage;
                isValid = false;
            }
        }
    });

    if (isValid) {
        return true;
    } else {
        $(".error").css({
            border: '2px solid red'
        });
        displayErrorMessage(['入力値に異常があります、修正してください']);
        return false;
    }
}

// 個別編集画面（新規・更新）から一覧画面へ戻る（キャンセルボタン押下）
function backShowAdminStudents() {
    var fm = document.getElementById("returnSearchedKeyWords");
    fm.submit();
}