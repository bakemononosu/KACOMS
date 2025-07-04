const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;



let answerGroupTableChangeFlag = "0";

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
    var window_height = (window.innerHeight ? window.innerHeight : $(window).innerHeight()) - 155;

    // tableの位置を取得
    var pos = $('.table-container').offset();

    // フッターまでの高さセット
    $('.content-main').css('height', window_height + 'px');

    // table（スクロール領域）の高さセット
    var pageFlg = document.getElementById("questionPageFlg").value;
    if (pageFlg == "true") {
        $('.table-container').css('height', ((window_height - pos.top + 25)) + 'px');
    } else {
        $('.table-container').css('height', ((window_height - pos.top + 50)) + 'px');
    }
}

window.onload = function () {
    var isPageShowQuestion = document.getElementById('pageShowQuestionIdentifier') !== null;

    if (isPageShowQuestion) {
        updateLabel();
    }
};

//問題詳細を開いた際に走る処理、新規登録向けの画面なのか既存向けの画面なのかを判定
function showQuestionsView(QuestionId) {
    document.getElementById('QuestionId').value = QuestionId;
    var fm = document.getElementById("QuestionData");

    if (!QuestionId || QuestionId == undefined || QuestionId == "" || QuestionId.length == 0) {
        fm.setAttribute('action', 'ShowQuestionsNewView');
    } else {
        fm.setAttribute('action', 'ShowQuestionsView');
    }

    fm.submit();
}

//キャンセルボタンを押下してShowAllQuestions画面に戻る機能、検索条件を保持して戻る
function backToAllQuestionView(QuestionId) {
    document.getElementById('QuestionId').value = QuestionId;
    var fm = document.getElementById("QuestionSearchData");
    fm.setAttribute('action', 'SearchActionForQuestion');
    fm.submit();
}

function newAnswerInsertModal() {
    // モーダルウィンドウを表示します
    // 要素をIDで取得する
    const hiddenInput = document.getElementById('changeOrinsertFlag');
    hiddenInput.value = 'insert';
    //モーダルを表示するタイミングでモーダルのリセットをかける為第2引数に1を設置
    //第1引数はモーダルページ名
    clearModal('showModelNewQuestioninsert', '1')

}

function showAnswerModalView(answerId) {
    clearModal('showModelNewQuestioninsert', '0')
    // 要素をIDで取得する
    const hiddenInput = document.getElementById('changeOrinsertFlag');
    // 要素のvalueを"ABCDE"に変更する
    hiddenInput.value = 'change';

    // クリックされた要素の親行を取得
    var row = document.getElementById("tr_" + answerId);

    // 行情報を取得
    if (row) {
        var answerText = row.querySelector("#answerText").textContent;
        var explanationText = row.querySelector("#explanationText").textContent;
        var answerImageDataElement = row.querySelector("#answerImageData");
        var answerImageNameElement = row.querySelector("#answerImageName"); // answerImageNameの値を取得
        var errataFlagCheckbox = row.querySelector("#errataFlag_" + answerId);
        var modalAnswerIdElement = row.querySelector("#modalAnswerId"); // row内のmodalAnswerId要素を取得

        if (answerImageDataElement && answerImageNameElement && errataFlagCheckbox && modalAnswerIdElement) {
            var answerImageData = answerImageDataElement.getAttribute("value");
            var answerImageName = answerImageNameElement.value;
            var isCorrect = errataFlagCheckbox.checked;
            var modalAnswerId = modalAnswerIdElement.value; // modalAnswerIdの値を取得


            // AnswerFileName要素を取得
            var answerFileNameElement = document.getElementById("AnswerFileName");
            if (answerFileNameElement) {
                answerFileNameElement.textContent = answerImageName; // AnswerFileNameにanswerImageNameを設定
            }

            // モーダル内の対応する要素にmodalAnswerIdを設定
            var modalAnswerIdInput = document.querySelector("#modalAnswerId1"); // IDセレクタを使用して要素を取得
            if (modalAnswerIdInput) {
                modalAnswerIdInput.value = modalAnswerId; // modalAnswerIdの値を設定
            }

            // openModalWithRowData関数に情報を渡す
            openModalWithRowData(answerId, answerText, explanationText, answerImageData, isCorrect);
        } else {
            console.error("Expected DOM elements not found in the row.");
        }
    } else {
        console.error("Row not found with the provided answerId: " + answerId);
    }
}

//表の情報をモーダルに送り表示する機能
function openModalWithRowData(answerId, answerText, explanationText, answerImageData, isCorrect, answerImageName) {
    // モーダルの要素を取得
    var modal = document.getElementById("showModelNewQuestioninsert");

    // モーダル内のテキストエリアにデータを設定
    var modalAnswerText = modal.querySelector("#modalAnswerText");
    var modalExplanationText = modal.querySelector("#modalexplanationText");

    // モーダル内の画像データを設定
    var modalImageDataElement = modal.querySelector("#base64PictureBinaryAnswer");
    modalImageDataElement.value = answerImageData;

    // 正解選択肢のチェック状態を設定
    var correctAnswerRadioButton = modal.querySelector("#correctAnswer");
    var incorrectAnswerRadioButton = modal.querySelector("#incorrectAnswer");

    if (isCorrect) {
        correctAnswerRadioButton.checked = true;
        incorrectAnswerRadioButton.checked = false;
    } else {
        correctAnswerRadioButton.checked = false;
        incorrectAnswerRadioButton.checked = true;
    }

    // テキストエリアに値を設定
    modalAnswerText.value = answerText;
    modalExplanationText.value = explanationText;

    // モーダルを表示
    var modalInstance = new bootstrap.Modal(modal);
    modalInstance.show();
}

//問題の詳細をModal上に表示する。(編集した詳細の内容は直接DBへ登録されるわけではない、一時的に表にて保持される)
function ModalViewForTableSentence() {
    // モーダルの要素を取得
    var modal = document.getElementById("showModelNewQuestioninsert");

    // モーダル内のテキストエリアから値を取得
    var modalAnswerText = modal.querySelector("#modalAnswerText").value;
    var modalExplanationText = modal.querySelector("#modalexplanationText").value;
    var modalImageData = modal.querySelector("#base64PictureBinaryAnswer").value;

    // 正解のチェック状態を取得
    var isCorrect = modal.querySelector("#correctAnswer").checked;

    // モーダルのAnswerIdを取得
    var modalAnswerIdInput = modal.querySelector("#modalAnswerId1");
    if (modalAnswerIdInput) {
        var answerId = modalAnswerIdInput.value;
    } else {
        console.error("modalAnswerId1 element not found in the modal.");
        return; // エラーの場合、関数を終了
    }

    // 対象行を取得
    var row = document.getElementById("tr_" + answerId);

    // モーダル内の AnswerFileName 要素を取得してその値を `modalImageDataName` に代入
    var modalImageDataNameElement = modal.querySelector("#AnswerFileName");
    var modalImageDataName = modalImageDataNameElement ? modalImageDataNameElement.textContent : '';

    // 対象行の要素が存在するか確認
    if (row) {
        // 対象行の要素を取得
        var answerTextElement = row.querySelector("#answerText");
        var explanationTextElement = row.querySelector("#explanationText");
        var answerImageDataElement = row.querySelector("#answerImageData");
        var answerImageNameElement = row.querySelector("#answerImageName");
        var errataFlagCheckbox = row.querySelector("#errataFlag_" + answerId);

        // 対象行の要素に値を反映
        if (answerTextElement) {
            answerTextElement.textContent = modalAnswerText;
        }
        if (explanationTextElement) {
            explanationTextElement.textContent = modalExplanationText;
        }
        if (answerImageDataElement) {
            // answerImageDataElementのテキストを更新
            if (modalImageData) {
                answerImageDataElement.textContent = "あり";
                answerImageDataElement.setAttribute("href", "#");
                answerImageDataElement.setAttribute("value", modalImageData);
                // `pictureViewForTable` 関数の `onclick` 属性を更新
                answerImageDataElement.setAttribute("onclick", `pictureViewForTable('${modalImageData}')`);
            } else {
                answerImageDataElement.textContent = "なし";
                answerImageDataElement.removeAttribute("value");
                // `pictureViewForTable` 関数の `onclick` 属性を削除
                answerImageDataElement.removeAttribute("onclick");
                answerImageDataElement.removeAttribute("href");
            }
        }
        if (answerImageNameElement) {
            answerImageNameElement.value = modalImageDataName; // 説明画像の名前がある場合に設定
        }
        if (errataFlagCheckbox) {
            errataFlagCheckbox.checked = isCorrect;

        } clearModal("showModelNewQuestioninsert");

        // モーダルを閉じる
        var modalInstance = new bootstrap.Modal(modal);
        modalInstance.hide();
    }
    else {
        console.error("対象行が見つかりません: " + answerId);
    }
}

//ユーザ一覧から削除を行う際の処理
async function deleteQuestionForList(QuestionId) {
    var buttons = [
        { type: "primary", text: "はい" },
        { type: "secondary", text: "いいえ" }
    ];
    
    var result = await messageBox('削除確認', 'データを削除します。よろしいですか?', buttons);
    
    if (result === 'はい') {
        // 「はい」が選択されたときの処理
        var fd = new FormData();
        fd.append('QuestionId', QuestionId);

        try {
            let response = await fetch('/AdminQuestions/DeleteAccount', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': csrfToken,
                },
                body: fd,
            });
            
            if (response.ok) {
                document.getElementById('tr_' + QuestionId).remove();
                await messageBox("", "テスト問題削除に成功しました", []);
            } else {
                let errorMessage = await response.text();
                await messageBox("", errorMessage === "削除できませんでした" ? "削除できませんでした" : "削除に失敗しました", []);
            }
        } catch (error) {
            await messageBox("", "削除に失敗しました", []);
        }
    } else {
        // 「いいえ」が選択されたときの処理
        return;
    }
}

async function updateLabel() {
    // ドロップダウンの要素を取得
    var choiceClassMajorElement = document.getElementById('choiceClassMajor');
    var choiceClassMiddleElement = document.getElementById('choiceClassMiddle');
    var choiceClassMinorElement = document.getElementById('choiceClassMinor');
    var seqNoElement = document.getElementById('seqNoPersonal');

    // 現在の選択内容を取得
    var choiceClassMajor = choiceClassMajorElement.options[choiceClassMajorElement.selectedIndex]?.value;
    var choiceClassMiddle = choiceClassMiddleElement.options[choiceClassMiddleElement.selectedIndex]?.value;
    var choiceClassMinor = choiceClassMinorElement.options[choiceClassMinorElement.selectedIndex]?.value;

    var SeqNo = seqNoElement.value;

    // ラベルの要素を取得
    var identificationNumberPersonal = document.getElementById('identificationNumberPersonal');

    // ラベルの内容を更新
    let DefaultSeqNo = "#####";
    if (SeqNo) {
        identificationNumberPersonal.value = `${choiceClassMajor || 0}-${choiceClassMiddle || '00'}-${choiceClassMinor || '00'}-${SeqNo}`;
    } else {
        if (choiceClassMajor && choiceClassMiddle && choiceClassMinor) {
            DefaultSeqNo = await GetMaxSeqNo();
        }
        identificationNumberPersonal.value = `${choiceClassMajor || 0}-${choiceClassMiddle || '00'}-${choiceClassMinor || '00'}-${DefaultSeqNo}`;
    }
}


// DOM要素を取得
const dropZone = document.getElementById('dropZone');
const questionImage = document.getElementById('questionImage');
const answerImage = document.getElementById('answerImage');
const fileName = document.getElementById('fileName');
const QuestionFileName = document.getElementById('QuestionFileName');
const AnswerFileName = document.getElementById('AnswerFileName');
const hiddenFieldAnswer = document.getElementById('base64PictureBinaryAnswer');
const hiddenFieldQuestion = document.getElementById('base64PictureBinaryQuestion');

// 共通のファイルハンドリング関数
async function handleFile(event, fileNameElement, hiddenFieldElement) {
    event.preventDefault();
    var button = document.getElementById("hanneiButton");

    // ドロップまたは選択されたファイルを取得
    const file = event.target.files ? event.target.files[0] : event.dataTransfer.files[0];

    // ファイルタイプチェック
    if (file && (file.type === 'image/jpeg' || file.type === 'image/png')) {
        try {

            // getImageUrl関数を呼び出してURLを取得
            button.disabled = true;
            const url = await getImageUrl({ target: { files: [file] } });
            // 隠しフィールドにURLを設定
            hiddenFieldElement.value = url;
            fileNameElement.textContent = file.name;
            button.disabled = false;
        } catch (error) {
            console.error('画像のURL取得に失敗しました:', error);
            fileNameElement.textContent = '画像のURL取得に失敗しました';
            hiddenFieldElement.value = '';
            button.disabled = false;
        }
    } else {
        fileNameElement.textContent = '画像ファイルを選択してください';
        hiddenFieldElement.value = '';
        button.disabled = false;
    }
}

// ファイルハンドリング関数をイベントリスナーに設定
if (dropZone) {
    dropZone.addEventListener('drop', function (event) {
        handleFile(event, AnswerFileName, hiddenFieldAnswer);
    });
    dropZone.addEventListener('dragover', function (event) {
        event.preventDefault();
        dropZone.style.backgroundColor = '#f0f0f0';
    });
    dropZone.addEventListener('dragleave', function () {
        dropZone.style.backgroundColor = 'white';
    });
}

if (questionImage) {
    questionImage.addEventListener('change', function (event) {
        handleFile(event, QuestionFileName, hiddenFieldQuestion);
    });
}

if (answerImage) {
    answerImage.addEventListener('change', function (event) {
        handleFile(event, AnswerFileName, hiddenFieldAnswer);
    });
}

function QuestionUpdate(updateOrInsert) {
    if (updateOrInsert === "update") {
        dataUpdateQuestionCatalog(updateOrInsert);
    } else if (updateOrInsert === "insert") {
        dataUpdateQuestionCatalog(updateOrInsert);
    }
}

//既存データのUpdate機能、Insert機能
function dataUpdateQuestionCatalog(switchingView) {
    var choiceClassMajor = document.getElementById('choiceClassMajor').value;
    var choiceClassMiddle = document.getElementById('choiceClassMiddle').value;
    var choiceClassMinor = document.getElementById('choiceClassMinor').value;
    var identificationNumberPersonal = document.getElementById('identificationNumberPersonal').value;
    var titlePersonal = document.getElementById('titlePersonal').value;
    var questionPersonal = document.getElementById('questionPersonal').value;
    var base64PictureBinary = document.getElementById('base64PictureBinaryQuestion').value;
    if (base64PictureBinary) {
        var fileName = document.getElementById('QuestionFileName').innerText;
    } else {
        var fileName = "";
    }
    var choiceClassLevel = document.getElementById('choiceClassLevel').value;
    var choiceScore = document.getElementById('choiceScore').value;
    var choiceClassQuestionType = document.getElementById('choiceClassQuestionType').value;
    var seqNoPersonal = document.getElementById('seqNoPersonal').value;
    var questionIdPersonal = document.getElementById('questionIdPersonal').value;
    var isFixedSortOrder = document.getElementById('sort_order_setting').checked;

    const fd = new FormData();

    let AnswerGroupViewTableData = extractDataFromTable();
    AnswerGroupViewTableData = AnswerGroupViewTableData.filter(item => {
        const isAnswerTextEmpty = item.answerText === '';
        const isExplanationTextEmpty = item.explanationText === '';
        const isAnswerIdEmpty = item.answerId === '';
        return !(isAnswerTextEmpty && isExplanationTextEmpty && isAnswerIdEmpty);
    });

    if (validateForQuestionContents()) {
        //if (switchingView === "update") {
        //reverseDeleteFlgChangeFunction(AnswerGroupViewTableData);
        //}
            return;
    }

    if (validateForAnswerContents(AnswerGroupViewTableData)) {
            //reverseDeleteFlgChangeFunction(AnswerGroupViewTableData);
        return;
    }
 
    const fixedAnswerGroupViewTableData = JSON.stringify(AnswerGroupViewTableData);
    fd.append('fixedAnswerGroupViewTableData', fixedAnswerGroupViewTableData);
    fd.append('choiceClassMajor', choiceClassMajor);
    fd.append('choiceClassMiddle', choiceClassMiddle);
    fd.append('choiceClassMinor', choiceClassMinor);
    fd.append('identificationNumberPersonal', identificationNumberPersonal);
    fd.append('titlePersonal', titlePersonal);
    fd.append('questionPersonal', questionPersonal);
    fd.append('base64PictureBinary', base64PictureBinary);
    fd.append('fileName', fileName);
    fd.append('choiceClassLevel', choiceClassLevel);
    fd.append('choiceScore', choiceScore);
    fd.append('choiceClassQuestionType', choiceClassQuestionType);
    fd.append('seqNoPersonal', seqNoPersonal);
    fd.append('questionIdPersonal', questionIdPersonal);
    fd.append('answerGroupTableChangeFlag', answerGroupTableChangeFlag);
    fd.append('fixedSortOrderSetting', isFixedSortOrder);

    if (switchingView === "update") {
        fetch('/AdminQuestions/UpdateQuestion', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(async response => {
                if (response.ok) {
                    const data = await response.json();

                    buttons = [{ type: "primary", text: "OK" }];
                    await messageBox('更新完了', "処理が正常に完了しました。", buttons);


                    // リダイレクト先のURLにdata.choiceClassMajorをクエリパラメータとして追加
                    location.href = `/AdminQuestions/SearchActionForQuestion?choiceClassMajor=${encodeURIComponent(data.choiceClassMajor)}`;

                    // AnswerGroup更新フラグをOFFにする
                    answerGroupTableChangeFlag = "0";
                } else {
                    //let errorMessage = await response.text();
                    //await messageBox("", errorMessage === "削除できませんでした" ? "削除できませんでした" : "削除に失敗しました", []);
                    this.messageBox("", "更新に失敗しました", []);
                }
            })
            .catch(error => {
                console.error('Fetchエラー:', error);
                messageBox(1, error);
            });
    } else if (switchingView === "insert") {
        fetch('/AdminQuestions/InsertQuestion', {
            method: 'POST',
            headers: {
                RequestVerificationToken: csrfToken,
            },
            body: fd,
        })
            .then(response => {
                if (response.ok) {

                    response.json().then(data => {

                        // リダイレクト先のURLにdata.choiceClassMajorをクエリパラメータとして追加
                        location.href = `/AdminQuestions/SearchActionForQuestion?choiceClassMajor=${encodeURIComponent(data.choiceClassMajor)}`;

                        // AnswerGroup更新フラグをOFFにする
                        answerGroupTableChangeFlag = "0";
                    });
                } else {
                    this.messageBox("", "更新に失敗しました", []);
                }
            })

            .catch(error => {
                this.messageBox(1, error);
            });

    }
}

//問題バリデーションチェック機能
//
//choiceClassMajor=大分類
//必須
//choiceClassMiddle=中分類
//必須
//choiceClassMinor=小分類
//必須
//titlePersonal=タイトル
//必須
//choiceClassLevel=難易度
//必須
//choiceScore=点数
//必須、0~255の範囲
function validateForQuestionContents() {

    let errorFlg = false;

    // エラーメッセージエリアが既にあれば削除
    var common_message_area = document.getElementById('common_message_area');
    if (common_message_area) {
        common_message_area.remove();
    }

    // フィールドの取得
    var fields = {
        choiceClassMajor: document.getElementById('choiceClassMajor'),
        choiceClassMiddle: document.getElementById('choiceClassMiddle'),
        choiceClassMinor: document.getElementById('choiceClassMinor'),
        titlePersonal: document.getElementById('titlePersonal'),  
        choiceClassLevel: document.getElementById('choiceClassLevel'),
        choiceScore: document.getElementById('choiceScore'),
    };

    // すべてのフィールドからエラースタイルをリムーブ
    for (var key in fields) {
        /*        fields[key].classList.remove("error");*/
        fields[key].title = "";  // エラーメッセージもクリア
        fields[key].style.border = '1px solid #ced4da';  // デフォルトのスタイルにリセット
    }

    // 必須フィールドのチェック
    var requiredFields = [
        'choiceClassMajor',
        'choiceClassMiddle',
        'choiceClassMinor',
        'titlePersonal',
        'choiceClassLevel',
        'choiceScore'
    ];

    requiredFields.forEach(function (field) {
        if (!fields[field].value) {
            fields[field].classList.add("error");
            fields[field].title = "この項目は必須です";
            fields[field].style.border = '2px solid red';  // エラースタイルを適用
            errorFlg = true;
        }
    });

    // choiceScoreの追加チェック
    if (fields.choiceScore.value > 255 || fields.choiceScore.value < 0) {
        fields.choiceScore.classList.add("error");
        fields.choiceScore.title = "この項目は0から255の間で入力してください";
        fields.choiceScore.style.border = '2px solid red';  // エラースタイルを適用
        errorFlg = true;
    }
    if (errorFlg) {
        displayErrorMessage(['入力値に異常があります、修正してください']);
    }

    return errorFlg;
}

//選択肢バリデーションチェック機能
function validateForAnswerContents(AnswerGroupViewTableData) {

    var validateError = false;
    var allDeleted = AnswerGroupViewTableData.every(item => item.DeletedFlg === 'True');
    if (!allDeleted) {
        var availableAnswers = AnswerGroupViewTableData.filter(function (value) {
            return (value.DeletedFlg !== 'True' && value.errataFlg)
        });
        if (availableAnswers.length < 1) {
            validateError = true;
            displayErrorMessage(['選択肢は1つ以上かつ正解を1つ以上作成した状態で登録してください']);
        }
    } else {
        validateError = true;
        displayErrorMessage(['選択肢は1つ以上かつ正解を1つ以上作成した状態で登録してください']);
    }
    return validateError;
}

//新規の解答を表に追加する。(DBへの登録は行われない)
function CreateNewTableInView() {
    var AnswerText = document.getElementById('modalAnswerText').value;
    var base64PictureBinary = document.getElementById('base64PictureBinaryAnswer').value;
    var ExplanationText = document.getElementById('modalexplanationText').value;
    var AnswerImageName = document.getElementById('AnswerFileName').innerText;

    // 入力データがすべて空白かどうかを確認
    if (
        (!AnswerText || AnswerText.trim() === "") &&
        (!base64PictureBinary || base64PictureBinary.trim() === "") &&
        (!ExplanationText || ExplanationText.trim() === "") &&
        (!AnswerImageName || AnswerImageName.trim() === "")
    ) {
        // データがすべて空白の場合、clearModalを実行
        clearModal("showModelNewQuestioninsert");
        return;  // 関数を終了
    }

    // 正解のラジオボタンがチェックされているかどうかを取得
    var correctAnswer = document.getElementById('correctAnswer').checked;
    // 不正解のラジオボタンがチェックされているかどうかを取得
    var incorrectAnswer = document.getElementById('incorrectAnswer').checked;

    answerGroupTableChangeFlag = "1";
    var ErrataFlg = correctAnswer ? "true" : "false";

    const AnswerId = generateUUID();
    let inputData = {
        AnswerId: AnswerId,
        AnswerText: AnswerText,
        AnswerImageData: base64PictureBinary,
        AnswerImageName: AnswerImageName,
        ExplanationText: ExplanationText,
        ErrataFlg: ErrataFlg,
    };
    addRowToTable(inputData);

    // 処理終了後、Modalをクリア
    clearModal("showModelNewQuestioninsert");
}

//GUIDを生成する機能
function generateUUID() {
    let dt = new Date().getTime();
    const uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = (dt + Math.random() * 16) % 16 | 0;
        dt = Math.floor(dt / 16);
        return (c === 'x' ? r : (r & 0x3) | 0x8).toString(16);
    });
    return uuid;
}

//反映を押下した後にモーダルの中身をクリアする機能
//resetModal="0"でリセットしてからモーダル終了
//resetModal="1"でリセットしてからモーダル立ち上げ
function clearModal(modalId, resetModal = "0") {
    // 指定されたModalのIDを使ってModal要素を取得
    var modal = document.getElementById(modalId);

    // Modalが正しく取得されているか確認
    if (!modal) {
        console.error("Modal element not found");
        return;
    }

    // Modal内の<textarea>要素をクリア
    var textareas = modal.querySelectorAll("textarea");
    textareas.forEach(function (textarea) {
        textarea.value = "";
    });

    // Modal内の<input>要素をクリア
    var inputs = modal.querySelectorAll("input");
    inputs.forEach(function (input) {
        if (input.type === "checkbox" || input.type === "radio") {
            input.checked = false;
            document.getElementById("incorrectAnswer").checked = true;
        } else if (input.type !== "hidden" && input.type !== "button") {
            input.value = "";
        }
    });

    // Modal内の<p>要素をクリア
    var answerFileName = modal.querySelector("#AnswerFileName");
    if (answerFileName) {
        answerFileName.textContent = "";
    }

    var base64PictureBinaryAnswer = modal.querySelector("#base64PictureBinaryAnswer");

    if (base64PictureBinaryAnswer) {
        base64PictureBinaryAnswer.value = "";
    }

    // dropZoneのバックグラウンドカラーを削除
    var dropZone = modal.querySelector("#dropZone");
    if (dropZone) {
        dropZone.style.backgroundColor = "";
    }

    if (resetModal === "1") {
        //モーダル表示時にリセットしてから表示する
        $(modal).modal('show');
    } else {
        // 反映押下後モーダルをリセットして閉じる
        $(modal).modal('hide');
    }
}

//モーダルにて入力した内容を画面の表に追加する機能
function addRowToTable(answerData) {
    // テーブルの<tbody>要素を取得
    const tbody = document.querySelector('.table-container table tbody');

    // answerDataがnullまたはundefinedでないか確認
    if (!answerData || !answerData.AnswerId) {
        console.error('Invalid answerData or missing AnswerId:', answerData);
        return;
    }

    // `tbody` が `null` でないことを確認
    if (!tbody) {
        console.error('The table body (tbody) element was not found.');
        return;
    }

    // 新しい<tr>要素を作成
    const newRow = document.createElement('tr');
    newRow.id = `tr_${answerData.AnswerId}`;
    newRow.style.height = '2.2rem';

    // 編集セルを作成
    const editCell = document.createElement('td');
    editCell.style.verticalAlign = 'middle';
    editCell.className = "centered";
    editCell.style.padding = 0;
    editCell.innerHTML = `
        <span class="span-icon" onclick="DeleteFlgChangeFunction('${answerData.AnswerId}')">
            <i class="bi bi-trash3"></i>
        </span>
        <span class="span-icon" onclick="showAnswerModalView('${answerData.AnswerId}')">
            <i class="bi bi-pencil-square"></i>
        </span>
    `;
    newRow.appendChild(editCell);

    // 正解セルを作成
    const correctAnswerCell = document.createElement('td');
    correctAnswerCell.style.verticalAlign = 'middle';
    correctAnswerCell.className = "centered";
    correctAnswerCell.style.padding = 0;


    correctAnswerCell.classList.add('text-center');
    correctAnswerCell.innerHTML = `
        <div class="form-check d-flex-center">
            <input type="checkbox" class="form-check-input" id="errataFlag_${answerData.AnswerId}" ${answerData.ErrataFlg == 'true' ? 'checked' : ''} disabled >
        </div>
    `;
    newRow.appendChild(correctAnswerCell);

    // 選択肢セルを作成
    const answerTextCell = document.createElement('td');
    answerTextCell.style.verticalAlign = 'middle';
    answerTextCell.className = "left whiteSpacePre ps-1";
    answerTextCell.style.padding = 0;


    answerTextCell.id = "answerText";
    answerTextCell.style.whiteSpace = 'normal';
    answerTextCell.textContent = answerData.AnswerText;
    newRow.appendChild(answerTextCell);

    // 画像セルを作成
    const imageCell = document.createElement('td');
    imageCell.style.verticalAlign = 'middle';
    imageCell.className = "centered";
    imageCell.style.padding = 0;

    imageCell.id = "explanationImageData2";
    if (answerData.AnswerImageData) {
        // <a>要素を作成
        const link = document.createElement('a');
        // IDとvalue属性を設定
        link.id = 'answerImageData';
        link.setAttribute('value', answerData.AnswerImageData);
        // 他の属性を設定
        link.href = '#';
        link.textContent = 'あり';
        link.setAttribute('onclick', `pictureViewForTable('${answerData.AnswerImageData}')`);
        // link要素をimageCellに追加
        imageCell.appendChild(link);
    } else {
        // なしの場合は単にテキストを表示
        imageCell.id = "answerImageData";
        const text = document.createTextNode('なし');
        imageCell.appendChild(text);
    }
    newRow.appendChild(imageCell);


    // answerImageNameの隠しセルを作成
    const hiddenCell = document.createElement('td');
    hiddenCell.className = "centered";
    hiddenCell.style.padding = 0;


    hiddenCell.style.display = 'none'; // セルを非表示に設定
    const hiddenInput = document.createElement('input');
    hiddenInput.type = 'hidden';
    hiddenInput.id = 'answerImageName'; // IDを'answerImageName'に設定
    hiddenInput.name = 'answerImageName';
    hiddenInput.value = answerData.AnswerImageName;
    hiddenCell.appendChild(hiddenInput);
    newRow.appendChild(hiddenCell);

    // answerData.AnswerIdを格納する隠し<input>要素を作成
    const answerIdInput = document.createElement('input');
    answerIdInput.type = 'hidden';
    answerIdInput.name = 'answerId'; // name属性を'sanswerId'に設定
    answerIdInput.value = answerData.AnswerId;
    answerIdInput.id = 'modalAnswerId';
    newRow.appendChild(answerIdInput);

    // 解説セルを作成
    const explanationCell = document.createElement('td');
    explanationCell.style.verticalAlign = 'middle';
    explanationCell.className = "left whiteSpacePre ps-1";
    explanationCell.style.padding = 0;


    explanationCell.id = "explanationText";
    explanationCell.textContent = answerData.ExplanationText;
    newRow.appendChild(explanationCell);

    //// DeleteFlgの隠しセルを作成
    const hiddenDeleteFlgCell = document.createElement('td');
    hiddenDeleteFlgCell.className = "centered";
    hiddenDeleteFlgCell.style.padding = 0;
    hiddenDeleteFlgCell.style.display = 'none'; // セルを非表示に設定

    const hiddenDeleteFlg = document.createElement('label');
    hiddenDeleteFlg.type = 'hidden';
    hiddenDeleteFlg.id = 'modalDeletedFlg';
    hiddenDeleteFlg.name = 'modalDeletedFlg';
    hiddenDeleteFlg.textContent = 'False';
    hiddenDeleteFlgCell.appendChild(hiddenDeleteFlg);
    newRow.appendChild(hiddenDeleteFlgCell);


    // 新しい行を<tbody>に追加
    tbody.appendChild(newRow);

    $(".span-icon").css({
        cursor: 'pointer',
        color: '#007bff'
    });

    $(".centered").css({
        textAlign: 'center'
    });

    $(".left").css({
        textAlign: 'left',
    });

    $(".whiteSpacePre").css({
        whiteSpace: 'pre'
    });

    $(".d-flex-center").css({
        display: 'flex',
        justifyContent: 'center'
    });

}

//解答の画像「あり」をクリックした際の画像表示機能
async function pictureViewForTable(hiddenField) {
    try {
        Base64ToImage(hiddenField, function (img) {
            // モーダルで画像をポップアップ表示
            showImageInModal(img);
        });
    } catch (error) {
        console.error(error);
    }
}

async function pictureViewForQuestion() {
    try {
        var hiddenField = document.getElementById('base64PictureBinaryQuestion').value;
        // Base64文字列を画像に変換
        Base64ToImage(hiddenField, function (img) {
            // モーダルで画像をポップアップ表示
            showImageInModal(img);
        });
    } catch (error) {
        console.error(error);
    }
}

// Base64文字列を画像に変換する関数
function Base64ToImage(base64img, callback) {
    const img = new Image();
    img.onload = function () {
        callback(img);
    };
    img.src = base64img;
}

// モーダルで画像を表示する関数
function showImageInModal(img) {
    // モーダル要素を取得または作成
    let modal = document.getElementById('imageModal');
    if (!modal) {
        modal = document.createElement('div');
        modal.id = 'imageModal';
        modal.style.display = 'none';

        // 背景のスタイルを追加
        modal.style.position = 'fixed';
        modal.style.top = '0';
        modal.style.left = '0';
        modal.style.width = '100%';
        modal.style.height = '100%';
        modal.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
        modal.style.zIndex = '1000';

        // モーダルのコンテンツを表示する要素を追加
        const modalContent = document.createElement('div');
        modalContent.style.position = 'absolute';
        modalContent.style.top = '50%';
        modalContent.style.left = '50%';
        modalContent.style.transform = 'translate(-50%, -50%)';
        modalContent.style.backgroundColor = 'white';
        modalContent.style.padding = '20px';
        modalContent.style.borderRadius = '10px';
        modalContent.style.boxShadow = '0 4px 8px rgba(0, 0, 0, 0.2)';

        // 画像のスタイリングを追加
        img.style.maxWidth = '80%';
        img.style.maxHeight = '80%';
        img.style.display = 'block';
        img.style.margin = '0 auto'; // 画像を中央に表示

        // モーダルに画像を追加
        modalContent.appendChild(img);
        modal.appendChild(modalContent);
        document.body.appendChild(modal);
    } else {
        // モーダルの既存コンテンツをクリアして画像を追加
        modal.innerHTML = '';
        const modalContent = document.createElement('div');
        modalContent.style.position = 'absolute';
        modalContent.style.top = '50%';
        modalContent.style.left = '50%';
        modalContent.style.transform = 'translate(-50%, -50%)';
        modalContent.style.backgroundColor = 'white';
        modalContent.style.padding = '20px';
        modalContent.style.borderRadius = '10px';
        modalContent.style.boxShadow = '0 4px 8px rgba(0, 0, 0, 0.2)';

        // 画像のスタイリングを追加
        img.style.maxWidth = '95%';
        img.style.maxHeight = '95%';
        img.style.display = 'block';
        img.style.margin = '0 auto'; // 画像を中央に表示

        // モーダルに画像を追加
        modalContent.appendChild(img);
        modal.appendChild(modalContent);
    }

    // モーダルを表示
    modal.style.display = 'block';

    // モーダルを閉じるためのクリックイベントを追加
    modal.addEventListener('click', function () {
        modal.style.display = 'none';
    });
}

//画面の表をリストに変換する機能
function extractDataFromTable() {
    let tableId = "AnswerGroupTableForView";
    const tableElement = document.getElementById(tableId);
    const rows = tableElement.querySelectorAll('tr');
    const extractedDataArray = [];

    rows.forEach(rowElement => {
        const rowId = rowElement.id;
        const answerId = rowId.replace('tr_', '');

        let answerText = '';
        const answerTextElement = rowElement.querySelector('#answerText');
        if (answerTextElement) {
            answerText = answerTextElement.textContent.trim();
        }

        let answerImageData = '';
        const answerImageDataElement = rowElement.querySelector('#answerImageData');
        if (answerImageDataElement) {
            answerImageData = answerImageDataElement.getAttribute('value');
        }

        let explanationText = '';
        const explanationTextElement = rowElement.querySelector('#explanationText');
        if (explanationTextElement) {
            explanationText = explanationTextElement.textContent.trim();
        }

        let modalDeletedFlg = '';
        const modalDeletedFlgElement = rowElement.querySelector('#modalDeletedFlg');
        if (modalDeletedFlgElement) {
            modalDeletedFlg = modalDeletedFlgElement.textContent.trim();
        }

        let answerImageName = '';
        const answerImageNameElement = rowElement.querySelector('#answerImageName');
        if (answerImageNameElement) {
            const value = answerImageNameElement.value;
            const answerImageDataElement = rowElement.querySelector('#answerImageData');
            if (answerImageDataElement) {
                answerImageName = value;
            } else {
                answerImageName = '';
            }
        }


        let errataFlg = false;
        const errataFlagElement = rowElement.querySelector(`#errataFlag_${answerId}`);
        if (errataFlagElement) {
            errataFlg = errataFlagElement.checked;
        }

        const extractedData = {
            answerId: answerId,
            answerText: answerText,
            answerImageData: answerImageData,
            explanationText: explanationText,
            answerImageName: answerImageName,
            errataFlg: errataFlg,
            DeletedFlg: modalDeletedFlg
        };

        extractedDataArray.push(extractedData);
    });

    return extractedDataArray;
}


//Modal操作、編集ボタンから開かれたモーダルなのか新規追加から開かれたModalなのかをgetElementByIdで判定
function changeOrInsert() {
    var changeOrinsertFlag = document.getElementById('changeOrinsertFlag').value
    if (changeOrinsertFlag === "change") {
        ModalViewForTableSentence();
    } else if (changeOrinsertFlag === "insert") {
        CreateNewTableInView();
    }
}

//大中小分類で検索した最大のSeqNo+1を取得する機能
//問題新規登録画面
async function GetMaxSeqNo() {
    var choiceClassMajor = document.getElementById('choiceClassMajor').value;
    var choiceClassMiddle = document.getElementById('choiceClassMiddle').value;
    var choiceClassMinor = document.getElementById('choiceClassMinor').value;
    const fd = new FormData();
    fd.append('choiceClassMajor', choiceClassMajor);
    fd.append('choiceClassMiddle', choiceClassMiddle);
    fd.append('choiceClassMinor', choiceClassMinor);
    try {
        // fetchリクエストを送信します
        const response = await fetch('/AdminQuestions/GetMaxSeqNo', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': csrfToken,
            },
            body: fd,
        });

        // レスポンスのステータスが正常であれば処理を行います
        if (response.ok) {
            const data = await response.json();
            return data.fixedNextSeqNo;
        } else {
            console.error('SeqNoの取得に失敗しました: ', response.statusText);
            throw new Error('SeqNoの取得に失敗しました: ' + response.statusText);
        }
    } catch (error) {
        console.error('エラーが発生しました:', error);
        this.messageBox(1, error.message);
    }
}

//問題詳細画面から解答の削除フラグをTrue(削除する）に変更する(画面上のみ))
async function DeleteFlgChangeFunction(answerId) {
    var buttons = [
        { type: "primary", text: "はい" },
        { type: "secondary", text: "いいえ" }
    ];
    // メッセージボックスの結果を取得
    var result = await messageBox('削除確認', 'データを削除します。よろしいですか?', buttons);
    // 'はい' が選択された場合の処理
    if (result === 'はい') {
        // 対象の行を取得
        var row = document.getElementById("tr_" + answerId);
        // 行が存在するか確認
        if (row) {
            // modalDeletedFlg の要素を取得
            var modalDeletedFlgElement = row.querySelector("#modalDeletedFlg");
            // modalDeletedFlg 要素が存在するか確認
            if (modalDeletedFlgElement) {
                // フラグを 'True' に変更
                modalDeletedFlgElement.textContent = 'True';
                // 対象の行を非表示
                row.style.display = 'none';
            }
        }
    }
    // 'いいえ' が選択された場合は何も行わない
}

//問題詳細画面から解答の削除フラグをTrue(削除取り消し）に変更する(画面上のみ))
async function reverseDeleteFlgChangeFunction(AnswerGroupViewTableData) {
    for (let item of AnswerGroupViewTableData) {
        // 対象の行を取得
        var row = document.getElementById("tr_" + item.answerId);
        // 行が存在するか確認
        if (row) {
            // modalDeletedFlg の要素を取得
            var modalDeletedFlgElement = row.querySelector("#modalDeletedFlg");
            // modalDeletedFlg 要素が存在するか確認
            if (modalDeletedFlgElement) {
                // フラグを 'False' に変更
                modalDeletedFlgElement.textContent = 'False';
                // 対象の行を表示に設定（display = '' で表示）
                row.style.display = '';
            }
        }
    }
}

function validateNumberInput(input) {
    // 数字以外の文字を削除
    input.value = input.value.replace(/[^0-9]/g, "");

    // 数値を0から255の範囲に制限
    if (input.value !== "") {
        let num = parseInt(input.value);
        if (num > 255 || num < 0) {
            input.setAttribute("data-bs-toggle", "tooltip");
            input.setAttribute("data-bs-placement", "top");
            //input.setAttribute("data-bs-original-title", "数値は0から255の範囲内で入力してください");
            var tooltip = new bootstrap.Tooltip(input);
            tooltip.show();
        } else {
            input.removeAttribute('data-bs-toggle');
            input.removeAttribute('data-bs-placement');
            input.removeAttribute('data-bs-original-title');
            input.removeAttribute('title');
            input.removeAttribute('aria-label');

            let tooltip = bootstrap.Tooltip.getInstance(input);
            if (tooltip) {
                tooltip.hide();
            }
        }
    } else {
        input.removeAttribute('data-bs-toggle');
        input.removeAttribute('data-bs-placement');
        input.removeAttribute('title');

        let tooltip = bootstrap.Tooltip.getInstance(input);
        if (tooltip) {
            tooltip.hide();
        }
    }
}