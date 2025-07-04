const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

// Tableの高さ調整
$(window).on('load resize', function () {
    // 高さ調整の計算を止める
    return;

    var elemtMain = document.querySelector('main');
    var mainHeight;

    if (elemtMain) mainHeight = elemtMain.clientHeight;

    $('.table-container').css('height', mainHeight - 150);
});

// コース個別登録画面への遷移
async function handleShowCourseChapters(courseId) {
    // URLを組み立てる
    if (courseId == undefined) {
        var url = `/AdminCourses/ShowCourseChapters`;
    } else {
        var url = `/AdminCourses/ShowCourseChapters/${courseId}`;
    }
    // リダイレクト
    window.location.href = url;
};

// 優先参照先区分または公開フラグの更新
function changeLineData(courseId, registType, syscodeClass, avalableFlg) {
    var targetElement = document.getElementById(registType + courseId)

    var fd = new FormData();
    fd.append('courseId', courseId)
    if (syscodeClass == "") {
        fd.append('publicFlg', targetElement.checked)
    } else {
        fd.append('primaryReference', targetElement.value)
    }

    fetch('/AdminCourses/UpdateMCourse', {
        method: 'POST',
        headers: {
            RequestVerificationToken: csrfToken,
        },
        body: fd,
    })
        .then(response => {
            if (response.ok) {
                // 見た目（画面）上の公開/非公開を切り替える
                changePublic(courseId, avalableFlg);
            } else {
                messageBox("", "更新に失敗しました", []);
            }
        })
        .catch(error => {
            messageBox("", error, []);
        });
}

// 見た目（画面）上の公開/非公開を切り替える
function changePublic(courseId, avalableFlg) {
    // 公開非公開
    var changeClasses = ['bg-secondary', 'bg-warning']
    var changeTexts = ['非公開', '公開中']

    // 公開/非公開エレメント
    var parent = '#isPublic_' + courseId;
    var elmIsPublic = document.querySelector(parent + ' > .badge');

    // 優先参照先エレメント
    var targetId = '#primaryReference_' + courseId;
    var elmPrimaryReference = document.querySelector(targetId);

    // 削除指定インデックス
    var deleteIndex = 0;
    // 追加指定インデックス
    var addIndex = 0;

    if (elmPrimaryReference.value == 1) {
        // 公開フラグ
        var pubFlgId = '#publicFlg_' + courseId;
        var elmPubFlg = document.querySelector(pubFlgId);
        if (elmPubFlg.checked) {
            deleteIndex = Number(elmPubFlg.checked)
            addIndex = Number(elmPubFlg.checked)

        } else {
            deleteIndex = Number(!elmPubFlg.checked)
            addIndex = Number(elmPubFlg.checked)
        }
    } else {
        // 公開期間
        if (avalableFlg == 1) {
            deleteIndex = Number(avalableFlg)
            addIndex = Number(avalableFlg)
        } else {
            deleteIndex = Number(!avalableFlg)
            addIndex = Number(avalableFlg)
        }
    }

    // DOM操作
    elmIsPublic.classList.remove(changeClasses[deleteIndex]);
    elmIsPublic.classList.add(changeClasses[addIndex]);
    elmIsPublic.innerText = changeTexts[addIndex];
}

// 削除フラグの更新
async function handleDeleteCourse(courseId) {
    var buttons = [];
    buttons[0] = { type: "primary", text: "はい" };
    buttons[1] = { type: "secondary", text: "いいえ" };
    var result = await messageBox('削除確認', '講座とそれに紐づくセクションを削除します。</br>よろしいですか？', buttons)

    if (result == 'はい') {
        // 「はい」が選択されたときの処理
        var fd = new FormData();
        fd.append('courseId', courseId)

        try {
            let response = await fetch('/AdminCourses/DeleteCourse', {
                method: 'POST',
                headers: {
                    RequestVerificationToken: csrfToken,
                },
                body: fd,
            })

            if (response.ok) {
                document.getElementById('tr_' + courseId).remove()
                await messageBox("", "削除に成功しました", []);
            } else {
                const resjson = await response.json();
                const error = resjson.errData;
                await messageBox("", error.errorMessage, []);
            }
        } catch (error) {
            await messageBox("", error, []);
        }
    }
    else {
        // 「いいえ」が選択されたときの処理
        return
    }
}