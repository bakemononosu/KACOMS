// ページ制御
const MAX_SELECTABLE_PAGE = 5;
let selectablePageNum = undefined;
let displayPage = undefined;
let pageChangeEventCallback = undefined;

// イベントハンドラ登録
document.querySelectorAll('.page-item').forEach((elm) => {
    elm.addEventListener('click', pageControllOnClickEventHandler);
});

/**
 * 現在表示されているページを取得する。
 * 
 * @returns ページ
 */
function getDisplayPage() {
    return displayPage;
}

function pageControllOnClickEventHandler(e) {
    //console.log(`==>pageControllOnClickEventHandler():${this.id}`);

    // 無効な要素がクリックされた場合は、何もしない
    if (document.getElementById(this.id).classList.contains('disabled')) {
        return;
    }

    if (this.id.match(/^p{1}/)) {
        targetPage = parseInt(document.getElementById(this.id).childNodes[0].innerHTML);
        //console.log(targetPage);
    } else {
        switch (this.id) {
            case 'start':
                targetPage = 1;
                break;
            case 'back':
                targetPage = displayPage - 1;
                break;
            case 'forward':
                targetPage = displayPage + 1;
                break;
            case 'end':
                targetPage = displayLastPage;
                break;
            default:
                break;
        }
    }

    if (displayPage != targetPage) {
        if (pageChangeEventCallback != undefined) {
            pageChangeEventCallback(targetPage);
        }
        displayPage = targetPage;
    }

    var num = parseInt(selectablePageNum / 2);

    //console.log(`displayPage(${displayPage}) + num(${num}) <= displayLastPage(${displayLastPage})`);
    var displayTopNum = 1;
    if (displayPage == 1) {
        displayTopNum = 1;
    } else if ((displayPage - num) <= 1) {
        displayTopNum = 1;
    } else {
        if ((displayPage + num) <= displayLastPage) {
            displayTopNum = displayPage - num;
        } else {
            displayTopNum = displayLastPage - selectablePageNum + 1;
        }
    }

    pageAccessControl(displayTopNum);
}


/**
 * 
function pagerInt(count) {

    // 全ページ数取得
    var totalPage = parseInt(count / MAX_OF_PAGE) + (((count % MAX_OF_PAGE) > 0) ? 1 : 0);
    selectablePageNum = 1;
    if (totalPage >= 5) {
        selectablePageNum = 5;
    } else if (totalPage >= 3) {
        selectablePageNum = 3;
    } else {
        selectablePageNum = totalPage;
    }

    for (var idx = 0; idx < MAX_SELECTABLE_PAGE; idx++) {
        var elm = document.getElementById(`p${(idx + 1)}`);
        if (idx < selectablePageNum) {
            elm.innerHTML = `${(idx + 1)}`;
            elm.classList.remove('invisible');
        } else {
            elm.classList.add('invisible');
        }
    }

    displayPage = 1;
    displayLastPage = totalPage;

    pageAccessControl(1);
}
 */

function pageControllerInit(count, numOfPage, callback=undefined) {

    // 全ページ数取得
    var totalPage = parseInt(count / numOfPage) + (((count % numOfPage) > 0) ? 1 : 0);
    selectablePageNum = 1;
    if (totalPage >= 5) {
        selectablePageNum = 5;
    } else if (totalPage >= 3) {
        selectablePageNum = 3;
    } else {
        selectablePageNum = totalPage;
    }

    for (var idx = 0; idx < MAX_SELECTABLE_PAGE; idx++) {
        var elm = document.getElementById(`p${(idx + 1)}`);
        if (idx < selectablePageNum) {
            elm.innerHTML = `${(idx + 1)}`;
            elm.classList.remove('d-none');
        } else {
            elm.classList.add('d-none');
        }
    }

    displayPage = 1;
    displayLastPage = totalPage;
    pageChangeEventCallback = callback;

    pageAccessControl(1);
}

function pageAccessControl(displayTopNum) {
    for (var idx = 0; idx < selectablePageNum; idx++) {
        var pageNumElm = document.getElementById(`p${(idx + 1)}`);
        pageNumElm.classList.remove('active');
        pageNumElm.innerHTML = `<a class="page-link" href="#!">${(displayTopNum + idx)}</a>`;
        if ((displayTopNum + idx) == displayPage) {
            pageNumElm.classList.add('active');
        }
    }

    document.getElementById('start').classList.remove('disabled');
    document.getElementById('back').classList.remove('disabled');
    document.getElementById('forward').classList.remove('disabled');
    document.getElementById('end').classList.remove('disabled');

    if (displayPage == 1) {
        document.getElementById('start').classList.add('disabled');
        document.getElementById('back').classList.add('disabled');
    }
    if (displayPage == displayLastPage) {
        document.getElementById('forward').classList.add('disabled');
        document.getElementById('end').classList.add('disabled');
    }
}
