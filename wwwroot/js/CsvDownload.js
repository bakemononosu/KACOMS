function handleDownloadButtonClick(controllerName) {
    var downloadButton = document.getElementById('downloadButton');
    downloadButton.disabled = true; // ボタンを無効化する

    var userName = document.getElementById('userName').value;
    var email = document.getElementById('email').value;
    var companyName = document.getElementById('companyName').value;
    var departmentName = document.getElementById('departmentName').value;
    var employeeNo = document.getElementById('employeeNo').value;
    var remarks1 = document.getElementById('remarks1').value;
    var remarks2 = document.getElementById('remarks2').value;
    // 受講者管理画面用////////////////////////////////////////
    var userRole;
    var elmUserRole = document.getElementById('userRole');
    if (elmUserRole) {
        userRole = elmUserRole.value;
    }
    var available;
    var elmAvailable = document.getElementById('available');
    if (elmAvailable) {
        // 要素が存在する場合の処理
        available = elmAvailable.value;
    }
    ///////////////////////////////////////////////////////////

    var fd = new FormData();
    fd.append('userName', userName);
    fd.append('email', email);
    fd.append('companyName', companyName);
    fd.append('departmentName', departmentName);
    fd.append('employeeNo', employeeNo);
    fd.append('remarks1', remarks1);
    fd.append('remarks2', remarks2);
    // 受講者管理画面用///////////////////////////////
    if (userRole) fd.append('userRole', userRole);
    if (available) fd.append('available', available);
    //////////////////////////////////////////////////

    fetch(`/${controllerName}/SearchActionCSV`, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': csrfToken,
        },
        body: fd,
    })
        .then(response => {
            if (!response.ok) {
                this.messageBox("", "ファイルダウンロードに失敗しました", []);
                downloadButton.disabled = false; // エラーが発生した場合はボタンを再度有効化する
                throw new Error('Fetchエラー');
            }
            return response.json();
        })
        .then(data => {
            downloadCSV(data);
            this.messageBox("", "ファイルダウンロードが完了しました", []);
            downloadButton.disabled = false; // 正常に完了した場合はボタンを再度有効化する
        })
        .catch(error => {
            console.error('Fetchエラー: ', error);
            downloadButton.disabled = false; // エラーが発生した場合はボタンを再度有効化する
        });
}

function downloadCSV(data) {
    // 指定されたカラムのみを含むCSVヘッダーを作成
    //const headers = ['action', 'userId', 'loginId', 'userName', 'companyName', 'departmentName', 'email', 'employeeNo', 'remarks1', 'remarks2', 'userRole', 'availableFlg', 'deletedFlg'].join(',');
    const headers = ['アクション', 'ユーザ識別子', '利用者ID', '利用者名', '法人名', '所属部署名', 'メールアドレス', '社員番号', '備考1', '備考2', '利用者区分', '受講可否フラグ', '削除フラグ'].join(',');

    const csvData = data.map(item => {
        const row = [
            item.action || '',
            item.userId || '',
            item.loginId || '',
            item.userName || '',
            item.companyName || '',
            item.departmentName || '',
            item.email || '',
            item.employeeNo || '',
            item.remarks1 || '',
            item.remarks2 || '',
            item.userRole || '',
            item.availableFlg ? '1' : '0',
            item.deletedFlg ? '1' : '0',
        ].map(value => {
            // ダブルクオートをエスケープし、文字列内のカンマを囲みます
            return typeof value === 'string' && value.includes(',') ? `"${value.replace(/"/g, '""')}"` : value;
        });
        return row.join(',');
    }).join('\n');

    // CSVデータをファイルとしてダウンロード
    const csvContent = `${headers}\n${csvData}`;
    const bom = new Uint8Array([0xEF, 0xBB, 0xBF]);
    const blob = new Blob([bom, csvContent], { type: 'text/csv;' });
    const link = document.createElement('a');
    const fileName = `MUser_${getFormattedDateTime()}.csv`;
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
}

function getFormattedDateTime() {
    const now = new Date();
    const year = now.getFullYear();
    const month = ('0' + (now.getMonth() + 1)).slice(-2);
    const day = ('0' + now.getDate()).slice(-2);
    const hours = ('0' + now.getHours()).slice(-2);
    const minutes = ('0' + now.getMinutes()).slice(-2);
    const seconds = ('0' + now.getSeconds()).slice(-2);
    return `${year}${month}${day}${hours}${minutes}${seconds}`;
}
