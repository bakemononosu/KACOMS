const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

/**
 * tableの高さ調整
 * 
 */
$(window).on('load resize', function () {
    var window_height = window.innerHeight ? window.innerHeight : $(window).innerHeight();
    $('.content-mainr').css('height', (window_height * 0.8) + 'px');
    var fileUploadAreaHeight = document.querySelector('#upFileWrap').clientHeight;
    $('.table-responsive').css('height', (window_height * 0.8) - fileUploadAreaHeight - 70);
});


// アップロード、クリアアイコンの初期表示を非表示に
var iconElement = document.getElementsByClassName('bi');
$(window).on('load', function () {
    for (var i = 0, l = iconElement.length; i < l; i++) {
        iconElement.item(i).style.display = 'none';
    }
});


// ドラッグ&ドロップエリアの取得
var fileArea = document.getElementById('dropArea');

// input[type=file]の取得
var fileInput = document.getElementById('uploadFile');

/**
 * ドラッグオーバー時の処理
 * 
 * @param {any} e
 */
fileArea.addEventListener('dragover', function (e) {
    e.preventDefault();
    fileArea.classList.add('dragover');
});

/**
 * ドラッグアウト時の処理
 * 
 * @param {any} e
 */
fileArea.addEventListener('dragleave', function (e) {
    e.preventDefault();
    fileArea.classList.remove('dragover');
});

/**
 * ドロップ時の処理
 * 
 * @param {any} e
 */
fileArea.addEventListener('drop', function (e) {
    e.preventDefault();
    fileArea.classList.remove('dragover');

    // テーブルを初期化
    const errorTable = document.getElementById('error-table');
    errorTable.style.display = 'none';
    const errorTableContent = document.getElementById('error-table-content');
    errorTableContent.querySelector('tbody').innerHTML = '';

    // アップロード、クリアアイコンの非表示
    for (var i = 0, l = iconElement.length; i < l; i++) {
        iconElement.item(i).style.display = 'none';
    }

    // ドロップしたファイルの取得
    var files = e.dataTransfer.files;

    // 取得したファイルをinput[type=file]へ
    fileInput.files = files;

    if (typeof files[0] !== 'undefined') {
        var fileData = files[0];
        if (fileData.type.indexOf('csv') == -1) {
            document.getElementById('uploadFile').value = '';
            messageBox("", "拡張子はCSVにしてください。", []);
        } else {
            var el = document.getElementById('selectedFileName');
            el.textContent = fileData.name;

            // アップロード、クリアアイコンを表示
            for (var i = 0, l = iconElement.length; i < l; i++) {
                iconElement.item(i).style.display = '';
            }
        }
    } else {
        //ファイルが受け取れなかった際の処理
        messageBox("", "ファイル読込に失敗しました。", []);
    }
});

/**
 * input[type=file]に変更があれば実行
 * 
 * @param {any} e
 */
fileInput.addEventListener('change', function (e) {
    // テーブルを初期化
    const errorTable = document.getElementById('error-table');
    errorTable.style.display = 'none';
    const errorTableContent = document.getElementById('error-table-content');
    errorTableContent.querySelector('tbody').innerHTML = '';

    // アップロード、クリアアイコンの非表示
    for (var i = 0, l = iconElement.length; i < l; i++) {
        iconElement.item(i).style.display = 'none';
    }

    // ドロップしたファイルの取得
    var files = e.target.files;

    // 取得したファイルをinput[type=file]へ
    fileInput.files = files;
    if (typeof files[0] !== 'undefined') {
        var fileData = files[0];
        if (fileData.type.indexOf('csv') == -1) {
            document.getElementById('uploadFile').value = '';
            messageBox("", "拡張子はCSVにしてください。", []);
        } else {
            var el = document.getElementById('selectedFileName');
            el.textContent = fileData.name;

            // アップロード、クリアアイコンを表示
            for (var i = 0, l = iconElement.length; i < l; i++) {
                iconElement.item(i).style.display = '';
            }
        }
    } else {
        // ファイルが受け取れなかった際の処理
        messageBox("", "ファイル読込に失敗しました。", []);
    }
}, false);

/**
 * 取得ファイルクリア
 * 
 */
function clearFileData() {
    // ファイル情報を初期化
    document.getElementById('uploadFile').value = '';
    var el = document.getElementById('selectedFileName');
    el.textContent = "選択されていません";

    // 一時ファイルのPathをリセット
    document.getElementById('tempFileName').value = "";

    // テーブルを初期化
    const errorTable = document.getElementById('error-table');
    errorTable.style.display = 'none';
    const errorTableContent = document.getElementById('error-table-content');
    errorTableContent.querySelector('tbody').innerHTML = '';

    // アップロード、クリアアイコンの非表示
    for (var i = 0, l = iconElement.length; i < l; i++) {
        iconElement.item(i).style.display = 'none';
    }

    // 更新ボタンを非活性
    document.getElementById('sbmBtn').setAttribute('disabled', 'true');
}

/**
 * CSVファイルバリデーション
 * 
 */
function fetchData() {
    var files = fileInput.files;
    if (typeof files[0] !== 'undefined') {
        var fileData = files[0];
        var el = document.getElementById('selectedFileName');
        el.textContent = fileData.name;
        //ファイルが正常に受け取れた際の処理
        var fd = new FormData();
        fd.append('fileData', fileData)

        // 更新ボタンを非活性
        document.getElementById('sbmBtn').setAttribute('disabled', 'true');
        // 一時ファイルのPathをリセット
        document.getElementById('tempFileName').value = "";

        // アップロード、クリアアイコンの非表示
        for (var i = 0, l = iconElement.length; i < l; i++) {
            iconElement.item(i).style.display = 'none';
        }

        // ローディングスクリーンを表示
        document.getElementById('loading-screen').style.display = 'block';

        var controllerName = document.getElementById('controllerName').value
        fetch(`/${controllerName}/ValidateCsv`, {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                return response.json()
            })
            .then(data => {
                // ローディングスクリーンを非表示にする
                document.getElementById('loading-screen').style.display = 'none';
                const errorTable = document.getElementById('error-table');
                // テーブルを初期化
                const errorTableContent = document.getElementById('error-table-content');
                errorTableContent.querySelector('tbody').innerHTML = '';

                const errors = data.errData;
                // エラーリストが空でない場合
                if (errors.length > 0) {
                    // エラーテーブルを表示
                    errorTable.style.display = 'block';

                    // エラーメッセージをテーブルに追加
                    const tbody = errorTable.querySelector('tbody');
                    errors.forEach(error => {
                        const row = document.createElement('tr');
                        row.innerHTML = `<td>${error.lineNumber}</td><td>${error.errorMessage}</td>`;
                        tbody.appendChild(row);
                    });
                    // ファイル情報を初期化
                    document.getElementById('uploadFile').value = '';
                    el.textContent = "選択されていません";

                } else {
                    // 更新ボタンを活性
                    document.getElementById('sbmBtn').removeAttribute('disabled');
                    // 一時ファイルのPathをhidden要素にセット
                    document.getElementById('tempFileName').value = data.tempFileName;
                    // エラーリストが空の場合、テーブルを非表示にする
                    errorTable.style.display = 'none';
                    // ファイル情報を初期化
                    document.getElementById('uploadFile').value = '';
                    // クリアアイコンを表示
                    iconElement.item(3).style.display = '';

                    messageBox("", "正常にアップロードされました。</br>更新ボタンから一括取込みを実行してください。", []);
                }
            })
            .catch(error => {
                // ローディングスクリーンを非表示にする
                document.getElementById('loading-screen').style.display = 'none';
                messageBox("", error, []);
            });
    } else {
        // ファイルが受け取れなかった際の処理
        messageBox("", "ファイル読込に失敗しました。", []);
    }

}

/**
 * CSVファイル一括取込み処理
 * 
 */
async function bulkUpload() {
    var buttons = [];
    buttons[0] = { type: "primary", text: "はい" };
    buttons[1] = { type: "secondary", text: "いいえ" };
    var result = await messageBox('更新確認', '一括更新を開始します。よろしいですか?', buttons)

    if (result == 'はい') {
        // 「はい」が選択されたときの処理
        var controllerName = document.getElementById('controllerName').value
        var tempFileName = document.getElementById('tempFileName').value
        var fd = new FormData();
        fd.append('tempFileName', tempFileName)

        fetch(`/${controllerName}/BulkUpload`, {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                return response.json()
            })
            .then(data => {
                // ローディングスクリーンを非表示にする
                document.getElementById('loading-screen').style.display = 'none';
                const errorTable = document.getElementById('error-table');
                // テーブルを初期化
                const errorTableContent = document.getElementById('error-table-content');
                errorTableContent.querySelector('tbody').innerHTML = '';

                const errors = data.errData;
                // エラーリストが空でない場合
                if (errors.length > 0) {
                    // エラーテーブルを表示
                    errorTable.style.display = 'block';

                    // エラーメッセージをテーブルに追加
                    const tbody = errorTable.querySelector('tbody');
                    errors.forEach(error => {
                        const row = document.createElement('tr');
                        row.innerHTML = `<td>${error.lineNumber}</td><td>${error.errorMessage}</td>`;
                        tbody.appendChild(row);
                    });
                    messageBox("", "エラー発生行まで取込み完了しています。", []);
                } else {
                    // エラーリストが空の場合、テーブルを非表示にする
                    errorTable.style.display = 'none';

                    messageBox("", "一括取込みは完了しました。", []);
                }
                // 更新ボタンを非活性
                document.getElementById('sbmBtn').setAttribute('disabled', 'true');
                // クリアアイコンを非表示
                iconElement.item(3).style.display = 'none';
                // 一時ファイルのPathをリセット
                document.getElementById('tempFileName').value = "";
                // ファイル情報を初期化
                document.getElementById('uploadFile').value = '';
                // 選択ファイル名を未選択に
                var el = document.getElementById('selectedFileName');
                el.textContent = "選択されていません";
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