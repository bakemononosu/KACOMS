const topPath = document.getElementById('top_url').value;
const offcanvasMenu = document.getElementById('offcanvas_menu');
const navbarToggler = document.querySelector('.navbar-toggler');
let offcanvas = undefined;

offcanvasMenu.addEventListener('show.bs.offcanvas', (e) => {
    offcanvas = e.target;
})

offcanvasMenu.addEventListener('hide.bs.offcanvas', (e) => {
    offcanvas = undefined;
})

window.addEventListener('resize', () => {
    if (window.innerWidth > 1199) {
        if (offcanvas != undefined) {
            navbarToggler.click();
        }
    }
});

document.querySelectorAll('#menu').forEach((elm) => {
    //console.log(elm.childNodes[0].getAttribute('href').replace(/\?[\W\w]+$/, ''));
    if (elm.childNodes[0].getAttribute('href').replace(/\?[\W\w]+$/, '') == topPath) {
        elm.classList.add('active');
        elm.classList.add('menu-selected');
    }
});

$('#logout, .link-logout').on('click', () => {
    location.href = `${location.protocol}//${location.host}/Login/Logout`;
});

/**
 * ファイル選択で選択されたイメージファイルをURL文字列に変換する
 * @param {any} e
 * @returns
 */
async function getImageUrl(e) {
    var promise = new Promise(function (callback) {
        var reader = new FileReader();
        reader.onload = function (e) {
            //console.log(e.target.result);
            callback(e.target.result);
        }
        reader.readAsDataURL(e.target.files[0]);
    });

    return await promise;
}

/**
 * HTMLエスケープ処理
 * @param {any} html
 * @returns
 */
function htmlEscape(html) {
    return $('<div>').text(html).html();
}