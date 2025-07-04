namespace ElsWebApp.Services
{
    public static class ConstService
    {
        public static class SystemCode
        {
            /// <summary> 利用者区分 </summary>
            public const string SYSCODE_CLASS_USER = "01";
            /// <summary> 受講可否フラグ </summary>
            public const string SYSCODE_CLASS_AVAILABLE = "02";
            /// <summary> 削除フラグ </summary>
            public const string SYSCODE_CLASS_DELETE = "03";
            /// <summary> 公開フラグ </summary>
            public const string SYSCODE_CLASS_PUBLIC = "04";
            /// <summary> 学習コンテンツ区分 </summary>
            public const string SYSCODE_CLASS_CONTENTS = "05";
            /// <summary> 難易度区分 </summary>
            public const string SYSCODE_CLASS_LEVEL = "06";
            /// <summary> 学習状況区分 </summary>
            public const string SYSCODE_CLASS_STATUS = "07";
            /// <summary> 正誤フラグ </summary>
            public const string SYSCODE_CLASS_ANSWER = "08";
            /// <summary> 出題形式区分 </summary>
            public const string SYSCODE_CLASS_QUESTION = "09";
            /// <summary> 優先公開参照先区分 </summary>
            public const string SYSCODE_CLASS_PRIMARY = "10";


            /// <summary> 大分類区分 </summary>
            public const string SYSCODE_CLASS_MAJOR = "11";
            /// <summary> 中分類区分 </summary>
            public const string SYSCODE_CLASS_MIDDLE = "12";
            /// <summary> 小分類区分 </summary>
            public const string SYSCODE_CLASS_MINOR = "13";


            /// <summary> 利用者区分:システム管理者 </summary>
            public const string SYSCODE_USR_ADMIN = "0";
            /// <summary> 利用者区分:法人代用管理者 </summary>
            public const string SYSCODE_USR_CORPO = "1";
            /// <summary> 利用者区分:受講者者 </summary>
            public const string SYSCODE_USR_USERS = "9";


            /// <summary> 受講可否フラグ：受講不可 </summary>
            public const string SYSCODE_AVA_NO = "0";
            /// <summary> 受講可否フラグ：受講可 </summary>
            public const string SYSCODE_AVA_YES = "1";

            /// <summary> 削除フラグ:有効</summary>
            public const string SYSCODE_DEL_NO = "0";
            /// <summary> 削除フラグ:削除</summary>
            public const string SYSCODE_DEL_YES = "1";

            /// <summary> 公開フラグ:非公開 </summary>
            public const string SYSCODE_PUB_NO = "0";
            /// <summary> 公開フラグ:公開 </summary>
            public const string SYSCODE_PUB_YES = "1";

            /// <summary> 学習コンテンツ区分:動画 </summary>
            public const string SYSCODE_CON_MOVIE = "1";
            /// <summary> 学習コンテンツ区分:テスト </summary>
            public const string SYSCODE_CON_TEST = "9";

            /// <summary> 難易度区分:低 </summary>
            public const string SYSCODE_LVL_LOW = "1";
            /// <summary> 難易度区分:中 </summary>
            public const string SYSCODE_LVL_MIDDLE = "5";
            /// <summary> 難易度区分:高 </summary>
            public const string SYSCODE_LVL_HEIGH = "9";

            /// <summary> 学習状況区分:未学習 </summary>
            public const string SYSCODE_STS_WAITING = "0";
            /// <summary> 学習状況区分:学習中 </summary>
            public const string SYSCODE_STS_STUDYING = "1";
            /// <summary> 学習状況区分:完了 </summary>
            public const string SYSCODE_STS_COMPLETE = "9";

            /// <summary> 正誤フラグ:正解 </summary>
            public const string SYSCODE_ANS_CORRECT = "1";
            /// <summary> 正誤フラグ:不正解 </summary>
            public const string SYSCODE_ANS_INCORRECT = "0";

            /// <summary> 出題形式区分:選択式 </summary>
            public const string SYSCODE_QUE_SELECT = "1";
            /// <summary> 出題形式区分:記述式 </summary>
            public const string SYSCODE_QUE_DESCRIPT = "2";

            /// <summary> 優先公開参照先区分:公開期間 </summary>
            public const string SYSCODE_PRI_PERIOD = "0";
            /// <summary> 優先公開参照先区分:フラグ </summary>
            public const string SYSCODE_PRI_FLAG = "1";
        }

        /// <summary>
        /// CSVアップロード関連
        /// </summary>
        public static class CsvUpload
        {
            /// <summary> アップロード動作:追加 </summary>
            public const string UPLOAD_ACT_INSERT = "I";
            /// <summary> アップロード動作:更新 </summary>
            public const string UPLOAD_ACT_UPDATA = "U";
            /// <summary> アップロード動作:削除 </summary>
            public const string UPLOAD_ACT_DELETE = "D";

            /// <summary> 取込みCSV項目数 </summary>
            public const int CSV_NUMBER_OF_ENTRIES = 13;

            /// <summary> CSVカラム名:アクション </summary>
            public const int CSV_COL_NAME_ACTION = 0;
            /// <summary> CSV:カラム名ユーザ識別子 </summary>
            public const int CSV_COL_NAME_USER_IDENTIFIER = 1;
            /// <summary> アップロード動作:カラム名利用者ID </summary>
            public const int CSV_COL_NAME_USER_ID = 2;
            /// <summary> アップロード動作:カラム名利用者名 </summary>
            public const int CSV_COL_NAME_USER_NAME = 3;
            /// <summary> アップロード動作:カラム名法人名 </summary>
            public const int CSV_COL_NAME_CORPORATE_NAME = 4;
            /// <summary> アップロード動作:カラム名所属部署名 </summary>
            public const int CSV_COL_NAME_DEPARTMENT_NAME = 5;
            /// <summary> アップロード動作:カラム名メールアドレス </summary>
            public const int CSV_COL_NAME_EMAIL_ADDRESS = 6;
            /// <summary> アップロード動作:カラム名社員番号 </summary>
            public const int CSV_COL_NAME_EMPLOYEE_NUMBER = 7;
            /// <summary> アップロード動作:カラム名備考1 </summary>
            public const int CSV_COL_NAME_NOTE1 = 8;
            /// <summary> アップロード動作:カラム名備考2 </summary>
            public const int CSV_COL_NAME_NOTE2 = 9;
            /// <summary> アップロード動作:カラム名利用者区分 </summary>
            public const int CSV_COL_NAME_USER_CLASSIFICATION = 10;
            /// <summary> アップロード動作:カラム名受講可否フラグ </summary>
            public const int CSV_COL_NAME_AVAILABLE_FLG = 11;
            /// <summary> アップロード動作:カラム名削除フラグ </summary>
            public const int CSV_COL_NAME_DELETED_FLG = 12;


        }

        /// <summary>
        /// パス情報
        /// </summary>
        public static class PathInfo
        {
            /// <summary>動画コンテンツのパス</summary>
            public const string PATH_FOLDER_VIDEO = @"\video";
            /// <summary>一時ファイル保存フォルダパス</summary>
            public const string PATH_FOLDER_TEMP = @"\tmp";

            public const string PATH_FFMPEG_EXE = @"\lib\ffmpeg\bin\ffmpeg.exe";

            #region ログインページ
            /// <summary>管理者用ログイン</summary>
            public const string PATH_LOGIN_ADMIN = "/Login/Admin";
            /// <summary>受講者用ログイン</summary>
            public const string PATH_LOGIN_STUDENT = "/Login/Student";
            #endregion

            #region 受講者ページ
            /// <summary>受講者:マイコースコントローラー</summary>
            public const string PATH_CONTROLLER_MY_COURSE = "/StudentMyCourse";
            /// <summary>受講者ページ：マイコース一覧</summary>
            public const string PATH_ST_PAGE_SHOW_MY_COURSE = $"{PATH_CONTROLLER_MY_COURSE}/ShowMyCourse";

            /// <summary>受講者:コースコントローラー</summary>
            public const string PATH_CONTROLLER_STUDENT_COURSES = "/StudentCourses";
            /// <summary>受講者ページ：コース一覧</summary>
            public const string PATH_ST_PAGE_SHOW_COURSES = $"{PATH_CONTROLLER_STUDENT_COURSES}/ShowCourses";

            /// <summary>受講者ページ：講座受講</summary>
            public const string PATH_AD_PAGE_SHOW_VIDEO = $"{PATH_CONTROLLER_MY_COURSE}/ShowVideoContents";
            /// <summary>受講者ページ：テスト実施確認</summary>
            public const string PATH_AD_PAGE_CONFIRM_EXAM = $"{PATH_CONTROLLER_MY_COURSE}/ConfirmExamination";
            /// <summary>受講者ページ：テスト実施</summary>
            public const string PATH_AD_PAGE_SHOW_TEST = $"{PATH_CONTROLLER_MY_COURSE}/ShowTestContents";
            /// <summary>受講者ページ：過去の問題の参照</summary>
            public const string PATH_AD_PAGE_EXAM_HISTORY = $"{PATH_CONTROLLER_MY_COURSE}/ShowExaminationHistory";
            #endregion

            #region 管理者ページ
            #region 受講者管理
            /// <summary>管理者用:受講者管理コントローラー</summary>
            public const string PATH_CONTROLLER_MANAGE_STUDENTS = "/AdminStudents";
            /// <summary>管理者ページ:受講者一覧</summary>
            public const string PATH_AD_PAGE_SHOW_STUDENTS = $"{PATH_CONTROLLER_MANAGE_STUDENTS}/ShowStudents";
            #endregion

            #region コース講座管理
            /// <summary>管理者用:コース講座管理コントローラー</summary>
            public const string PATH_CONTROLLER_MANAGE_COURSES = "/AdminCourses";
            /// <summary>管理者ページ:コース一覧</summary>
            public const string PATH_AD_PAGE_SHOW_COURSES = $"{PATH_CONTROLLER_MANAGE_COURSES}/ShowCourses";

            #endregion
            #region 進捗管理
            /// <summary>管理者用:進捗管理コントローラー</summary>
            public const string PATH_CONTROLLER_MANAGE_TASK = "/AdminTaskStatus";
            /// <summary>管理者ページ:コース進捗一覧</summary>
            public const string PATH_AD_PAGE_SHOW_COURSES_STATUS = $"{PATH_CONTROLLER_MANAGE_TASK}/ShowStudentsAnyCoursesStatus";

            #endregion

            #region テスト実施状況
            /// <summary>管理者用:テスト実施状況コントローラー</summary>
            public const string PATH_CONTROLLER_MANAGE_TEST_STATUS = "/AdminTestStatus";
            /// <summary>管理者ページ:コース進捗一覧</summary>
            public const string PATH_AD_PAGE_SHOW_COURSES_TEST_STATUS = $"{PATH_CONTROLLER_MANAGE_TEST_STATUS}/ShowCourseTestStatus";

            #endregion

            #region テスト管理
            /// <summary>管理者用:テスト管理コントローラー</summary>
            public const string PATH_CONTROLLER_MANAGE_QUESTIONS = "/AdminQuestions";
            /// <summary>管理者ページ:テスト一覧</summary>
            public const string PATH_AD_PAGE_SHOW_ALL_QUESTIONS = $"{PATH_CONTROLLER_MANAGE_QUESTIONS}/ShowAllQuestions";

            #endregion
            #endregion

            #region 共通ページ
            /// <summary>共通:アカウント管理コントローラー</summary>
            public const string PATH_CONTROLLER_MANAGE_ACCOUNT = "/AccountInfo";

            /// <summary>管理者用:アカウント一覧</summary>
            public const string PATH_AD_PAGE_SHOW_ACCOUNTS = $"{PATH_CONTROLLER_MANAGE_ACCOUNT}/ShowAccounts";
            /// <summary>受講者用:アカウント情報</summary>
            public const string PATH_ST_PAGE_SHOW_ACCOUNT = $"{PATH_CONTROLLER_MANAGE_ACCOUNT}/ShowStudentAccount";
            #endregion

            #region ロゴ
            public const string PATH_SYSTEM_LOGO = "/logo/logo_KACOMS_GROUP.png";
            #endregion
        }

        /// <summary>
        /// 動画関連
        /// </summary>
        public static class MovieContens
        {
            /// <summary>MIMEタイプ</summary>
            public const string TARGET_MIME_TYPE = "video/mp4";
            /// <summary>動画ファイルサイズ上限</summary>
            public const long UPLOAD_LIMIT_SIZE = 50 * 1024 * 1024;
            /// <summary>HLSファイル拡張子</summary>
            public const string HLS_FILE_EXT = "m3u8";
        }

        /// <summary>
        /// コース講座更新画面用
        /// 受講者コースデータ存在チェック結果
        /// </summary>
        public static class ResultCheckUserCourse
        {
            /// <summary> 受講者コースデータなし </summary>
            public const int RESULT_NOT_EXISTS = 0;
            /// <summary> 受講者コースデータあり </summary>
            public const int RESULT_EXISTS = 1;
            /// <summary> システムエラー </summary>
            public const int RESULT_ERROR = 2;
        }
    }
}
