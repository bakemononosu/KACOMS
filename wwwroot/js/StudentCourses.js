const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;


// Tableの高さ調整
$(window).on('load resize', function () {
    // 高さ調整の計算を止める
    return;

    var window_height = (window.innerHeight ? window.innerHeight : $(window).innerHeight()) - 130;

    // フッターまでの高さ
    $('.detail-area').css('height', window_height + 'px');

    // コース一覧（スクロール領域）の高さ
    $('.table-container').css('height', (window_height - 50) + 'px');

});

// 受講ボタン押下時の処理
async function addMyCourse(courseId, courseName) {
    // エラーメッセージエリアが既にあれば削除
    var common_message_area = document.getElementById('common_message_area');
    if (common_message_area) {
        document.getElementById('common_message_area').remove();
    }
    // 確認ダイアログを表示
    var buttons = [];
    buttons[0] = { type: "primary", text: "はい" };
    buttons[1] = { type: "secondary", text: "いいえ" };
    var result = await messageBox('講座受講確認', '「' + courseName + '」をマイ講座に追加します。よろしいですか?', buttons);

    // 「はい」が選択された場合のみ実行
    if (result == 'はい') {

        // パラメータを設定        
        var fd = new FormData();
        fd.append('courseId', courseId);
        fd.append('deletedFlg', false);

        fetch('/StudentCourses/UpdateMyCourse', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
        .then(response => {

            if (response.ok) {
                return response.json()
            } else {
                //messageBox('', '追加に失敗しました', []);
                displayErrorMessage(['追加に失敗しました']);
            }
        })
        .then(res => {
            if (res.status == 'OK') {
                // 受講・解除ボタンの活性・非活性を切り替え
                switchActive($('#add_' + courseId), 'bg-primary', false);
                switchActive($('#del_' + courseId), 'bg-danger', true);
            }
            messageBox('', res.message, []);
        })
        .catch(error => {
            messageBox('', error, []);
        });
    }
}

// 解除ボタン押下時の処理
async function deleteMyCourse(courseId, courseName, availableFlg) {
    // エラーメッセージエリアが既にあれば削除
    var common_message_area = document.getElementById('common_message_area');
    if (common_message_area) {
        document.getElementById('common_message_area').remove();
    }
    // 確認ダイアログを表示
    var buttons = [];
    buttons[0] = { type: "primary", text: "はい" };
    buttons[1] = { type: "secondary", text: "いいえ" };
    var result = await messageBox('講座解除確認', '「' + courseName + '」をマイ講座から削除します。よろしいですか?', buttons);

    // 「はい」が選択された場合のみ実行
    if (result == 'はい') {

        // パラメータを設定        
        var fd = new FormData();
        fd.append('courseId', courseId);
        fd.append('deletedFlg', true);

        fetch('/StudentCourses/UpdateMyCourse', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
        .then(response => {

            if (response.ok) {
                return response.json();
            } else {
                //messageBox('', '削除に失敗しました', []);
                displayErrorMessage(['削除に失敗しました']);

            }
        })
        .then(res => {
            if (res.status == 'OK') {
                // 受講・解除ボタンの活性・非活性を切り替え
                if (availableFlg) {  // 受講可能な場合のみ
                    switchActive($('#add_' + courseId), 'bg-primary', true);
                }
                switchActive($('#del_' + courseId), 'bg-danger', false);
            }
            messageBox('', res.message, []);
        })
        .catch(error => {
            messageBox('', error, []);
        });
    }
}

// ボタンの活性・非活性の切り替え 共通
function switchActive(btn, onclass, active) {
    if (active) {
        btn.addClass(onclass + ' play-on');
        btn.removeClass('bg-secondary play-off');
    } else {
        btn.addClass('bg-secondary play-off');
        btn.removeClass(onclass + ' play-on');
    }
}



