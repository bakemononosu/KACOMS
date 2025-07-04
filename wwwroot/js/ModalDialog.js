let elmMessageBox = undefined;

function processingMessage(message) {
    var dialog = createProcessingModal(message);
    document.querySelector('.main').insertAdjacentHTML('beforebegin', dialog);
    var elmMessageBox = document.getElementById('message_box');
    var msgbox = new bootstrap.Modal(elmMessageBox);
    msgbox.show();
    elmMessageBox.remove();
}

/**
 * メッセージボックスを表示する
 * 
 * @param {any} title
 * @param {any} message
 * @param {any} buttons
 * @returns
 */
async function messageBox(title, message, buttons) {

    var dialog = createModalMessage(title, message, buttons);
    document.querySelector('.main').insertAdjacentHTML('beforebegin', dialog);
    elmMessageBox = document.getElementById('message_box');
    var msgbox = new bootstrap.Modal(elmMessageBox);

    var promise = new Promise(function (callback) {
        document.querySelector('#modal_close_btn').addEventListener('click', { resolve: callback, handleEvent: buttonOnClickEventHandler });
        document.querySelectorAll(`[id^='modal_sel_btn_']`).forEach((elm) => {
            elm.addEventListener('click', { resolve : callback, handleEvent: buttonOnClickEventHandler });
        });
        msgbox.show();
    });

    return await promise;
}


/**
 * モーダルダイアログ(画像表示用)を表示する
 * 
 * @param {any} imagePath   画像ファイルのパス
 */
function modalImageDialog(imagePath) {
    //console.log(`imagePath:${imagePath}`);

    var img = new Image();
    img.src = imagePath;

    // イメージがダウンロードされるまで待つ
    img.onload = () => {
        var cv = document.createElement('canvas');
        var cx = cv.getContext('2d');
        cv.width = img.naturalWidth;
        cv.height = img.naturalHeight;
        cx.drawImage(img, 0, 0, img.naturalWidth, img.naturalHeight);
        var imageData = cv.toDataURL();
        cx = null;
        cv = null;

        var size = '';
        switch (true) {
            case img.naturalWidth < 300:
                size = 'modal-sm';
                break;
            case img.naturalWidth < 500:
                break;
            case img.naturalWidth < 800:
                size = 'modal-lg';
                break;
            default:
                size = 'modal-xl';
                break;
        }

        var dialog = createModalImage(size, imageData);
        document.querySelector('.main').insertAdjacentHTML('beforebegin', dialog);
        var md = new bootstrap.Modal(document.getElementById('modal_image'));

        md.show();
        document.getElementById('modal_image').remove();
    };
}

/**
 * メッセージボックスHTMLの生成
 * 
 * @param {any} title
 * @param {any} message
 * @param {any} buttons
 * @returns
 */
function createModalMessage(title, message, buttons) {
    let dialog = '';
    dialog = `<div id='message_box' class='modal fade' data-bs-backdrop='static' tabindex='-1'>`;
    dialog += `<div class='modal-dialog modal-dialog-centered'>`;
    dialog += `<div class='modal-content'>`;
    dialog += `<div class='modal-header'>`;
    dialog += `<h5 class='modal-title'>${title}</h5>`;
    dialog += `<button type='button' class='btn-close' data-bs-dismiss='modal' aria-label='Close' id='modal_close_btn'></button>`;
    dialog += `</div>`;
    dialog += `<div class='modal-body'>`;
    dialog += `<p>${message}</p>`;
    dialog += `</div>`;
    dialog += `<div class="modal-footer">`;
    for (var idx = 0; idx < buttons.length; idx++) {
        dialog += `<button type="button" class="btn btn-${buttons[idx].type}" data-bs-dismiss="modal" id="modal_sel_btn_${idx}">${buttons[idx].text}</button>`;
    }
    dialog += `</div>`;
    dialog += `</div>`;
    dialog += `</div>`;
    dialog += `</div>`;

    return dialog;
}

/**
 * 画像表示要モーダルダイアログHTMLの生成
 * 
 * @param {any} title
 * @param {any} message
 * @param {any} buttons
 * @returns
 */
function createModalImage(size, data) {
    let dialog = '';
    dialog = `<div id='modal_image' class='modal fade' tabindex='-1'>`;
    dialog += `<div class='modal-dialog ${size} modal-dialog-centered'>`;
    dialog += `<div class='modal-content'>`;
    dialog += `<div style='width:100%; display:flex; justify-content:end;'>`;
    dialog += `<button type='button' class='btn-close' data-bs-dismiss='modal' aria-label='Close' id='modal_close_btn'></button>`;
    dialog += `</div>`;
    dialog += `<div class='modal-body' style='text-align:center'>`;
    dialog += `<img src='${data}'/>`;
    dialog += `</div>`;
    dialog += `</div>`;
    dialog += `</div>`;
    dialog += `</div>`;

    return dialog;
}

function createProcessingModal(message) {
    let dialog = '';
    dialog = `<div id='message_box' class='modal fade' data-bs-backdrop='static' tabindex='-1'>`;
    dialog += `<div class='modal-dialog modal-dialog-centered'>`;
    dialog += `<div class='modal-content'>`;
    dialog += `<div class='modal-body'>`;
    dialog += `<h3 class='text-center'>${message}</h3>`;
    dialog += `</div>`;
    dialog += `</div>`;
    dialog += `</div>`;
    dialog += `</div>`;

    return dialog;
}


/**
 * ボタンのクリックイベントハンドラ
 * 
 * @param {any} e
 */
function buttonOnClickEventHandler(e) {
    if (e.currentTarget.id.match(/^modal_sel_btn_\d+/)) {
        this.resolve(e.currentTarget.innerHTML);
    } else {
        this.resolve('');
    }
    elmMessageBox.remove();
}