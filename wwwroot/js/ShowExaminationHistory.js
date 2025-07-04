// テスト実施回数
const nTimes = $('#total_times').val();
pageControllerInit(nTimes, 1, showPage);

function showPage(page) {
    console.log(page);

    // 全てのタブを一旦、非表示にする
    $(`[id^='myTab_p']`).each(function (index, element) {
        $(element).addClass('d-none');
    });
    $(`[id^='myContent_p']`).each(function (index, element) {
        $(element).addClass('d-none');
    });


    $(`[id^='myTab_p${page}']`).removeClass('d-none');
    $(`[id^='myContent_p${page}']`).removeClass('d-none');
}

