/**
 * パンくずリスト表示スクリプト 
 */
const elm = document.getElementById('breadcrumb') as HTMLInputElement;
const subContainer = document.querySelector('.sub-container');
let urls = elm.value.split(' > ');

let level = 1;
let breadcrumb = []; 
urls.forEach((data) => {
    var items = data.split(/(?<=^[^:]+):/);
    var jsonString = items[1].match(/\{[\s\S]*?\}/);
    if (jsonString != null) {
        var obj = JSON.parse(jsonString.input);
        var html = '<div class="d-inline-block col-5 col-lg-3"><select class="form-select d-inline-block w-100" id="breadcrumb_chapter">';
        for (var idx = 0; idx < obj.length; idx++) {
            var selected = obj[idx].selected? 'selected' : '';
            html += `<option value=${obj[idx].pathName}/${obj[idx].userId}/${obj[idx].courseId}/${obj[idx].chapterId} ${selected}>${obj[idx].chapterName}</option>`;
        }
        html += `</select></div>`;

        breadcrumb.push(html);
    }
    else
    {
        breadcrumb.push((level < urls.length) ? `<a href='${items[0]}'>${items[1]}</a>` : `${items[1]}`);
    }
    level++;
})

subContainer.insertAdjacentHTML('afterbegin', `<div class='row breadcrumb mt-1 ms-1 me-0 mb-1'><h5>${breadcrumb.join(' > ') }</h5></div>`)

/**
 * チャプタリスト変更イベントハンドラ
 */
const breadcrumbChapter = document.getElementById('breadcrumb_chapter') as HTMLSelectElement;
if (breadcrumbChapter != undefined) {
    breadcrumbChapter.addEventListener('change', (e) => {

        //console.log(breadcrumbChapter.value);
        location.href = breadcrumbChapter.value;

    });
}


/**
 * エラーメッセージをパンくずリストの下に表示する
 * 
 * @param {any} messages
 */
function displayErrorMessage(messages) {
    var html = ''
    html += `<div class="row justify-content-between" id="common_message_area">`;
    html += `<div class="col-11" style="color:red;">`;
    html += `<ul>`;
    messages.forEach((message) => {
        html += `<li>${message}</li>`;
    });
    html += `</ul>`;
    html += `</div>`;
    html += `<div class="col-1">`;
    html += `<i class="bi bi-x" onclick="closeMessage();" style="cursor:pointer;"></i>`;
    html += `</div>`;
    html += `</row>`;

    document.querySelector('.breadcrumb').insertAdjacentHTML('afterend', html);
}

/**
 * ×アイコンクリックイベントハンドラ
 */
function closeMessage() {
    document.getElementById('common_message_area').remove();
}
