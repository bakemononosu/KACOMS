const csrfToken = document.getElementsByName('__RequestVerificationToken')[0].value;

// Tableの高さ
$(window).on('load resize', function () {

    resizeTable();
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

    // table（スクロール領域）の高さセット
    $('.table-container').css('height', (window_height - pos.top + 50) + 'px');
}

// 検索条件の選択状態による画面遷移分岐
function showStudentCoursesStatus(userId) {
    document.getElementById('userId').value = userId;
    selectedCourse = document.getElementById('enteredCourse').value;
    var fm = document.getElementById("searchedKeyWords");
    if (selectedCourse) {
        fm.setAttribute('action', 'ShowStudentChaptersStatus');
    } else {
        fm.setAttribute('action', 'ShowStudentCoursesStatus');
    }

    fm.submit();
}

/**
 * CSVダウンロードクリックイベントハンドラ
 */
$('#download_csv').on('click', async function (e) {
    var fd = new FormData();
    // 法人名
    fd.append('corpName', $('#companyName').val());
    fd.append('userName', $('#userName').val());
    fd.append('courseId', $(`[id='courseId'] option:selected`).val());
    fd.append('pRateS', $(`[name='minRate']`).val());
    fd.append('pRateE', $(`[name='maxRate']`).val());

    try {
        var response = await fetch(`/AdminTaskStatus/DownloadCsv`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': csrfToken,
            },
            body: fd,
        });

        if (response.ok) {
            var dt = new Date();
            var today = `${dt.getFullYear()}${dt.getMonth() + 1}${dt.getDate()}${dt.getHours()}${dt.getMinutes()}${dt.getSeconds()}`;
            var data = await response.blob();
            let a = document.createElement('a');
            a.href = URL.createObjectURL(data);
            a.download = `講座進捗_${today}.csv`;
            a.click();
            a.remove();
        }
    }
    catch (e) {
        Console.log(e.message);
    }
});