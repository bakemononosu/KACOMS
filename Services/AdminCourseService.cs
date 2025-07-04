using ElsWebApp.Controllers;
using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SendGrid.Helpers.Mail;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using static ElsWebApp.Controllers.AdminCoursesController;
using static System.Net.Mime.MediaTypeNames;

namespace ElsWebApp.Services
{
    public class AdminCourseService(
        ICourseService course,
        IChapterService chapter,
        IMovieContentsService movie,
        ITestContentsService test,
        IQuestionCatalogService qcatalog,
        IAnswerGroupService agroup,
        ISysCodeService syscd,
        IExamListService elist,
        ElsWebAppDbContext ctx,
        IWebHostEnvironment env,
        IChapterService cptr,
        ILogger<AdminCourseService> logger
        )
        : IAdminCourseService
    {
        private readonly ICourseService _course = course;
        private readonly IChapterService _chapter = chapter;
        private readonly IMovieContentsService _movie = movie;
        private readonly ITestContentsService _test = test;
        private readonly IQuestionCatalogService _questCat = qcatalog;
        private readonly IAnswerGroupService _ansGroup = agroup;
        private readonly IExamListService _examList = elist;
        private readonly ISysCodeService _sysCd = syscd;

        private readonly ElsWebAppDbContext _ctx = ctx;
        private readonly IWebHostEnvironment _env = env;
        private readonly IChapterService _cptr = cptr;
        private readonly ILogger<AdminCourseService> _logger = logger;

        /// <inheritdoc/>
        public async Task<List<AdminCourse>> GetCourseList(bool isPublicOnly)
        {
            string sql = $@"
                        SELECT
	                        MCourse.CourseId
	                        , MAX(CourseName) 'CourseName'
	                        , CONCAT(
		                        CONVERT(nvarchar,MAX(BegineDateTime),111)
		                        , ' ～ '
		                        , CONVERT(nvarchar,MAX(EndDateTime),111)
	                        ) 'PublicPeriod'
	                        , COUNT(MChapter.ChapterId) 'ChapterCnt'
	                        , CONCAT(
		                        FORMAT((IsNull(SUM(TestContents.LimitTime), 0) + IsNull(SUM(MovieContents.PlaybackTime), 0)) / 60, '00')
		                        , ':'
		                        , FORMAT((IsNull(SUM(TestContents.LimitTime), 0) + IsNull(SUM(MovieContents.PlaybackTime), 0)) % 60, '00')
	                        ) 'StudyTime'
	                        , MAX(CAST(MCourse.PublicFlg AS int)) 'PublicFlg'
	                        , MAX(MCourse.PrimaryReference) 'PrimaryReference'
                            ,CASE WHEN MAX(MCourse.PrimaryReference) = @sysPriFlg
                                  THEN MAX(CAST(MCourse.PublicFlg AS int)) 
                                  WHEN MAX(MCourse.PrimaryReference) = @sysPriPeriod
                                  THEN CASE WHEN MAX(BegineDateTime) > GETDATE() OR MAX(EndDateTime) < GETDATE() THEN 0 ELSE 1 END
                                  ELSE 0
                             END AS IsPublic
                            ,CASE WHEN MAX(BegineDateTime) > GETDATE() OR MAX(EndDateTime) < GETDATE()
                                  THEN 0
                                  ELSE 1
                             END AS IsInAvalablePeriod
							 ,CASE WHEN COUNT(UserCourse.CourseId) > 0
								THEN 1
								ELSE 0
							END AS IsExistsUserCourse
                        FROM
	                        MCourse
                        LEFT JOIN MChapter
	                        ON MCourse.CourseId = MChapter.CourseId
	                        AND MChapter.DeletedFlg = @delflg
                        LEFT JOIN TestContents
	                        ON MChapter.ChapterId = TestContents.ChapterId
                            AND TestContents.DeletedFlg = @delflg
                        LEFT JOIN MovieContents
	                        ON MChapter.ChapterId = MovieContents.ChapterId
                            AND MovieContents.DeletedFlg = @delflg
                        LEFT JOIN (SELECT DISTINCT CourseId FROM UserCourse) UserCourse
							ON UserCourse.CourseId = MCourse.CourseId
                        WHERE
	                        MCourse.DeletedFlg = @delflg";

            if (isPublicOnly)
            {
                sql += $@"
                    AND
                        MCourse.PublicFlg = @pubyes";
            }

            sql += $@"
                    GROUP BY
	                    MCourse.CourseId
                    Order BY
                        CourseId";

            var taskAdminCourseList = await _ctx.Database.SqlQueryRaw<AdminCourse>(
               @sql,
                new SqlParameter("@delflg", ConstService.SystemCode.SYSCODE_DEL_NO),
                new SqlParameter("@pubyes", ConstService.SystemCode.SYSCODE_PUB_YES),
                new SqlParameter("@sysPriFlg", ConstService.SystemCode.SYSCODE_PRI_FLAG),
                new SqlParameter("@sysPriPeriod", ConstService.SystemCode.SYSCODE_PRI_PERIOD)
                ).ToListAsync();
            return taskAdminCourseList;
        }

        /// <inheritdoc/>
        public async Task<List<AdminCourseChapter>> GetCourseChapterList(Guid courseId)
        {

            // コース講座一覧を取得
            string sql = $@"
                    SELECT
                          ChapterId
                        , OrderNo
                        , ChapterName
                        , ContentsType
                        , CONCAT( FORMAT(LearningTime / 60, '00')
                            , ':'
                            , FORMAT(LearningTime % 60, '00')
                        ) AS LearningTime
                    FROM (
                        SELECT
                              MChapter.ChapterId
                            , MChapter.OrderNo
                            , MChapter.ChapterName
                            , MChapter.ContentsType
                            , CASE WHEN MChapter.ContentsType = {ConstService.SystemCode.SYSCODE_CON_MOVIE}
                                   THEN IsNull(MovieContents.PlaybackTime, 0)
                                   WHEN MChapter.ContentsType = {ConstService.SystemCode.SYSCODE_CON_TEST}
                                   THEN IsNull(TestContents.LimitTime, 0)
                                   ELSE 0
                              END AS LearningTime
                        FROM MChapter
                        LEFT JOIN MCourse 
                            ON MChapter.CourseId = MCourse.CourseId
                        LEFT JOIN TestContents
                            ON MChapter.ChapterId = TestContents.ChapterId
                           AND TestContents.DeletedFlg = {ConstService.SystemCode.SYSCODE_DEL_NO}
                        LEFT JOIN MovieContents
                            ON MChapter.ChapterId = MovieContents.ChapterId
                           AND MovieContents.DeletedFlg = {ConstService.SystemCode.SYSCODE_DEL_NO}
                        WHERE MChapter.DeletedFlg = {ConstService.SystemCode.SYSCODE_DEL_NO}
                          AND MCourse.CourseId = '{courseId}'
                    ) M
                    ORDER BY OrderNo
            ";

            var chapterList = await _ctx.Database.SqlQueryRaw<AdminCourseChapter>(sql).ToListAsync();
            return chapterList;
        }


        /// <inheritdoc/>
        public async Task<ShowChapterViewModel> GetChapterInfo(string courseId, string chapterId)
        {
            // システムコード(難易度区分)の取得
            var qLevel = await this._sysCd.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_LEVEL);
            // システムコード(出題形式)の取得
            var qType = await this._sysCd.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_QUESTION);

            var majorPair = await this._sysCd.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_MAJOR);
            var middlePair = await this._sysCd.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_MIDDLE);
            var minorPair = await this._sysCd.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_MINOR);


            var model = new ShowChapterViewModel();

            model.CourseId = Guid.Parse(courseId);

            model.MajorList = CommonService.ConvCVPairListToSIList(majorPair, true);
            model.MajorList.Insert(0, new SelectListItem("", ""));

            model.MiddleList = CommonService.ConvCVPairListToSIList(middlePair, true);
            model.MiddleList.Insert(0, new SelectListItem("", ""));

            model.MinorList = CommonService.ConvCVPairListToSIList(minorPair, true);
            model.MinorList.Insert(0, new SelectListItem("", ""));

            model.IsExistsUserCourse = Convert.ToInt32(await IsExistUserCourse(model.CourseId));

            if ((chapterId ?? "") != "")
            {
                // コンテンツ情報(動画)の取得
                var video = await this._movie.SelectByChapterId(Guid.Parse(chapterId!));

                if (video.ContentsId != Guid.Empty)
                {
                    model.ContentsType = ConstService.SystemCode.SYSCODE_CON_MOVIE;
                    model.ChapterId = video.ChapterId;
                    model.ChapterName = video.Chapter?.ChapterName ?? "";
                    model.OrderNo = video.Chapter?.OrderNo ?? 0;
                    model.ContentsId = video.ContentsId;
                    model.VideoOld.ContentsName = video.ContentsName;
                    model.VideoOld.ContentsPath = $"{video.ContentsPath}?{DateTime.Now.ToString()}";
                    model.VideoOld.PlaybackTime = video.PlaybackTime;
                }

                // コンテンツ情報(テスト)の取得
                var exam = await this._test.SelectByChapterId(Guid.Parse(chapterId!));

                if (exam.ContentsId != Guid.Empty)
                {
                    model.ContentsType = ConstService.SystemCode.SYSCODE_CON_TEST;
                    model.ChapterId = exam.ChapterId;
                    model.ChapterName = exam.Chapter?.ChapterName ?? "";
                    model.OrderNo = exam.Chapter?.OrderNo ?? 0;
                    model.ContentsId = exam.ContentsId;
                    model.Exam.ContentsName = exam.ContentsName;
                    model.Exam.Questions = exam.Questions;
                    model.Exam.LimitTime = exam.LimitTime;

                    // 出題リストの取得
                    var questions = await this._examList.SelectByContentsId(exam.ContentsId);
                    foreach (var quest in questions)
                    {
                        if (quest.QuestionCatalog == null)
                        {
                            // 問題カタログが存在しない場合
                            continue;
                        }
                        model.Exam.QuestionList.Add(new QuestionData
                        {
                            QId = quest.QuestionId.ToString(),
                            QNo = CommonService.MakeQuetionNo(
                                quest.QuestionCatalog!.MajorCd,
                                quest.QuestionCatalog!.MiddleCd,
                                quest.QuestionCatalog!.MinorCd,
                                quest.QuestionCatalog!.SeqNo),
                            QType = CommonService.GetValueByCode(qType, quest.QuestionCatalog!.QuestionType),
                            QTitle = quest.QuestionCatalog!.QuestionTitle,
                            QLevel = CommonService.GetValueByCode(qLevel, quest.QuestionCatalog!.Level),
                            QScore = quest.QuestionCatalog.Score
                        });
                    }
                    // 問題識別番号でソート
                    model.Exam.QuestionList.Sort((x, y) => x.QNo.CompareTo(y.QNo));
                }
            }
            else
            {
                var mChapterList = await _cptr.SelectByCourseId(Guid.Parse(courseId));
                var fixedOrderNo = 1;

                if (mChapterList.Count > 0)
                {
                    var getMaxOrderNo = mChapterList.Max(s => s.OrderNo);
                    if (getMaxOrderNo > 0)
                    {
                        fixedOrderNo = getMaxOrderNo + 1;
                    }
                }
                model.OrderNo = fixedOrderNo;
            }
            return model;
        }

        /// <inheritdoc/>
        public async Task<List<QuestionData>> SearchQuestionCatalog(string majorCd, string middleCd, string minorCd)
        {
            // システムコード(難易度区分)の取得
            var qLevel = await this._sysCd.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_LEVEL);
            // システムコード(出題形式)の取得
            var qType = await this._sysCd.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_QUESTION);

            // 問題カタログリストの取得
            var qcList = await this._questCat.SelectByClassCd(majorCd, middleCd, minorCd);

            List<QuestionData>? dataList = [];

            foreach (var qc in qcList)
            {
                dataList.Add(new QuestionData
                {
                    QId = qc.QuestionId.ToString(),
                    QNo = CommonService.MakeQuetionNo(qc.MajorCd, qc.MiddleCd, qc.MinorCd, qc.SeqNo),
                    QTitle = qc.QuestionTitle,
                    QType = CommonService.GetValueByCode(qType, qc.QuestionType),
                    QLevel = CommonService.GetValueByCode(qLevel, qc.Level),
                    QScore = qc.Score,
                });

                dataList.Sort((x, y) => x.QNo.CompareTo(y.QNo));
            }

            return dataList;
        }

        /// <inheritdoc/>
        public async Task<bool> RegisterChapterInfo(Guid userId, ShowChapterViewModel model)
        {
            var result = true;
            var isExistsUserCourse = await IsExistUserCourse(model.CourseId);
            try
            {
                // トランザクションの開始
                await this._ctx.BeginTans();
                // 講座マスターの登録/更新
                MChapter? chapterRec = null;
                var chapterId = model.ChapterId;
                if (model.ChapterId == Guid.Empty)
                {
                    // 受講者コースデータに情報がある場合は、登録しない
                    if(!isExistsUserCourse)
                    {
                        // 新規
                        chapterRec = new MChapter
                        {
                            CourseId = model.CourseId,
                            ChapterName = model.ChapterName,
                            ContentsType = model.ContentsType,
                            OrderNo = (byte)model.OrderNo,
                            DeletedFlg = false,
                            UpdatedBy = userId,
                            CreatedBy = userId,
                        };

                        var ret = await this._ctx.MChapter.AddAsync(chapterRec);
                        chapterId = ret.Entity.ChapterId;
                    }
                }
                else
                {
                    // 更新
                    chapterRec = await this._chapter.SelectById(model.ChapterId.ToString());
                    chapterRec.ChapterName = model.ChapterName;
                    // 受講者コースデータに情報がある場合は、講座名のみ更新
                    if(!isExistsUserCourse)
                    {
                        chapterRec.ContentsType = model.ContentsType;
                        chapterRec.OrderNo = (byte)model.OrderNo;
                    }
                    chapterRec.UpdatedBy = userId;
                }
                var count = await this._ctx.SaveChangesAsync();

                // 受講者コースデータに情報がある場合は、以降は処理しない
                if (!isExistsUserCourse)
                {
                    // 学習コンテンツ情報の更新
                    if (model.ContentsType == ConstService.SystemCode.SYSCODE_CON_MOVIE)
                    {
                        // 動画コンテンツ
                        MovieContents? videoRec = null;
                        if (model.ContentsId == Guid.Empty)
                        {
                            // 新規
                            videoRec = new MovieContents
                            {
                                ChapterId = chapterId,
                                ContentsName = model.VideoNew.ContentsName,
                                ContentsPath = $@"{ConstService.PathInfo.PATH_FOLDER_VIDEO.Replace('\\', '/')}/{model.CourseId}/{chapterId}/{Path.GetFileName(model.VideoNew.ContentsPath)}.{ConstService.MovieContens.HLS_FILE_EXT}",
                                PlaybackTime = (short)model.VideoNew.PlaybackTime,
                                DeletedFlg = false,
                                UpdatedBy = userId,
                                CreatedBy = userId,
                            };

                            await this._ctx.MovieContents.AddAsync(videoRec);
                        }
                        else
                        {
                            // 更新
                            videoRec = await this._movie.SelectById(model.ContentsId.ToString());
                            if (model.VideoNew.ContentsPath != "")
                            {
                                videoRec.ContentsName = model.VideoNew.ContentsName;
                                videoRec.ContentsPath = $@"{ConstService.PathInfo.PATH_FOLDER_VIDEO.Replace('\\', '/')}/{model.CourseId}/{chapterId}/{Path.GetFileName(model.VideoNew.ContentsPath)}.{ConstService.MovieContens.HLS_FILE_EXT}";
                                videoRec.PlaybackTime = (short)model.VideoNew.PlaybackTime;
                                videoRec.UpdatedBy = userId;
                            }
                        }
                        count = await this._ctx.SaveChangesAsync();
                    }
                    else if (model.ContentsType == ConstService.SystemCode.SYSCODE_CON_TEST)
                    {
                        // テストコンテンツ
                        TestContents? testsRec = null;
                        Guid contentsId = model.ContentsId;
                        if (model.ContentsId == Guid.Empty)
                        {
                            // 新規
                            testsRec = new TestContents
                            {
                                ChapterId = chapterId,
                                ContentsName = model.Exam.ContentsName,
                                Questions = (byte)model.Exam.Questions,
                                LimitTime = (short)model.Exam.LimitTime,
                                DeletedFlg = false,
                                UpdatedBy = userId,
                                CreatedBy = userId,
                            };
                            var ret = await this._ctx.TestContents.AddAsync(testsRec);
                            contentsId = ret.Entity.ContentsId;
                        }
                        else
                        {
                            // 更新
                            testsRec = await this._test.SelectById(model.ContentsId.ToString());
                            testsRec.ContentsName = model.Exam.ContentsName;
                            testsRec.Questions = (byte)model.Exam.Questions;
                            testsRec.LimitTime = (short)model.Exam.LimitTime;
                            testsRec.UpdatedBy = userId;
                        }
                        count = await this._ctx.SaveChangesAsync();

                        // 出題リスト削除
                        if (!await this._examList.DeleteByContentsId(contentsId))
                        {
                            throw new Exception("Delete Error");
                        }
                        // 出題リスト再作成
                        var qlist = model.Exam.QuestionList.Select(x => Guid.Parse(x.QId)).ToList();
                        if (!await this._examList.InsertFromQuestionIdList(userId, contentsId, qlist))
                        {
                            throw new Exception("Insert Error");
                        }
                    }

                    // 動画ファイルのコピー
                    var srcPath = model.VideoNew.ContentsPath.Replace("/", @"\");    // e.g. /wwwroot/tmp/r4dbm4x4zou/r4dbm4x4zou.m3u8
                    var desPath = $@"{this._env.WebRootPath}{ConstService.PathInfo.PATH_FOLDER_VIDEO}\{model.CourseId}\{chapterId}";
                    if (!CopyNewVideoContents(srcPath, desPath))
                    {
                        throw new Exception("File I/O Error");
                    }
                }
            }
            catch (Exception ex)
            {
                // ログ出力
                CommonService.CriticalError<AdminCourseService>(this._logger, ex);
                result = false;
            }

            await this._ctx.EndTans(result);


            return result;

        }

        /// <inheritdoc/>
        public async Task<QAnswerData> SearchAnswerGroup(Guid questionId)
        {
            var ansGList = await this._ansGroup.SelectByQuestionId(questionId);
            var qaData = new QAnswerData();

            if (ansGList.Count > 0)
            {
                qaData.QNo = CommonService.MakeQuetionNo(
                    ansGList[0].Question!.MajorCd,
                    ansGList[0].Question!.MiddleCd,
                    ansGList[0].Question!.MinorCd,
                    ansGList[0].Question!.SeqNo);
                qaData.QText = ansGList[0].Question!.QuestionText ?? "";
                qaData.QTitle = ansGList[0].Question!.QuestionTitle;
                qaData.QImage = ansGList[0].Question!.QuestionImageData ?? "";
            }
            foreach (var ans in ansGList)
            {
                qaData.AList.Add(new AnswerData
                {
                    AnswerText = ans.AnswerText ?? "",
                    ExplanationText = ans.ExplanationText,
                    AnswerImage = ans.AnswerImageData ?? "",
                    ErrataFlg = ans.ErrataFlg
                });
            }

            return qaData;
        }


        /// <summary>
        /// アップロードされた動画ファイルを動画コンテンツファイルにリネームする
        /// </summary>
        /// <param name="src">アップロードファイルパス    e.g. "\wwwroot\tmp\r4dbm4x4zou" </param>
        /// <param name="des">動画コンテンツファイルパス  e.g. "\wwwroot\video\{CourseId}\{ChapterId}"</param>
        /// <returns></returns>
        private bool CopyNewVideoContents(string src, string des)
        {
            var result = true;
            if (!string.IsNullOrEmpty(src))
            {
                var savePath = string.Empty;
                try
                {
                    // HLSファイル保存フォルダが存在しない場合は作成する。
                    if (!Directory.Exists(des))
                    {
                        Directory.CreateDirectory(des);
                    }

                    if (Directory.GetFiles(des).Length > 0)
                    {
                        // コピー先ファイルの存在確認：あり ⇒
                        savePath = $@"{des}\save";
                        Directory.CreateDirectory($@"{des}\save");
                        FileMove(des, savePath, false);
                    }

                    // tmpフォルダから講座識別子のフォルダへHLSファイルをコピーする
                    FileMove(src, des);

                }
                catch (Exception ex)
                {
                    // ログ出力
                    CommonService.CriticalError<AdminCourseService>(this._logger, ex);

                    // コピー元ファイル(tmpファイル)の削除
                    if (Directory.Exists(src))
                    {
                        Directory.Delete(src, true);
                    }

                    result = false;
                }
                finally
                {
                    if (savePath != "")
                    {
                        if (result)
                        {
                            // 一時ファイルの削除
                            Directory.Delete(savePath, true);
                        }
                        else
                        {
                            // 一時ファイルが存在する場合は復活
                            FileMove(savePath, des);
                        }
                    }
                }
            }
            return result;

            // HLSファイルの移動
            static void FileMove(string fromPath, string toPath, bool remove = true)
            {
                // ファイル移動
                foreach (var f in Directory.GetFiles(fromPath))
                {
                    File.Move(f, $@"{toPath}\{Path.GetFileName(f)}");
                }

                if (remove)
                {
                    // 移動元フォルダの削除
                    Directory.Delete(fromPath);
                }
            }
        }

        /// <inheritdoc/>
        public async Task<bool> HandlRegisterCourseInfo(string courseId, string primaryReference, bool publicFlg)
        {
            // コース識別子からコースマスタ情報を取得
            var course = await this._course.SelectById(courseId);
            if (string.IsNullOrEmpty(primaryReference))
            {
                // 公開フラグを更新
                course.PublicFlg = publicFlg;
            }
            else
            {
                // 優先参照先区分を更新
                course.PrimaryReference = primaryReference;
            }


            var count = await this._ctx.SaveChangesAsync();

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateDeleteFlg(string courseId, Guid userId)
        {
            var result = true;
            // 削除対象動画コンテンツファイルパス    // e.g /video/{courseId}/{chapterId}/***********.m3u8         
            List<string> videoContentsPath = [];
            try
            {
                // トランザクションの開始
                await this._ctx.BeginTans();
                // コース識別子からコースマスタ情報を取得
                var course = await this._course.SelectById(courseId);
                // 削除フラグを更新
                course.DeletedFlg = true;
                course.UpdatedBy = userId;
                var count = await this._ctx.SaveChangesAsync();
                if (count < 0) result = false;

                // コース識別子から講座リストを取得
                var chapterList = await this._chapter.SelectByCourseId(Guid.Parse(courseId));

                // 講座識別子からコンテンツ情報(動画・テスト)を取得し、削除フラグを更新
                foreach (var chapter in chapterList)
                {
                    // 動画コンテンツ
                    var movieContent = await this._movie.SelectByChapterId(chapter.ChapterId);
                    if(movieContent.ContentsId != Guid.Empty)
                    {
                        // 削除対象の動画コンテンツパスを一時保存
                        videoContentsPath.Add(movieContent.ContentsPath);
                        // 削除フラグ更新
                        movieContent.DeletedFlg = true;
                        movieContent.UpdatedBy = userId;
                        count = await this._movie.Update(movieContent);
                        if (count < 0) result = false;

                    }

                    // テストコンテンツ
                    var testContent = await this._test.SelectByChapterId(chapter.ChapterId);
                    if (testContent.ContentsId != Guid.Empty)
                    {
                        testContent.DeletedFlg = true;
                        testContent.UpdatedBy = userId;
                        count = await this._test.Update(testContent);
                        if (count < 0) result = false;
                    }
                }

                // 講座リストの削除フラグを更新
                chapterList.ForEach(x => x.DeletedFlg = true);
                foreach (var chapter in chapterList)
                {
                    chapter.UpdatedBy = userId;
                    count = await this._chapter.Update(chapter);
                    if (count < 0) result = false;
                }

                if(!result)
                {
                    throw new Exception("Update_Fail");
                }
            }
            catch (Exception ex)
            {
                // ログ出力
                CommonService.CriticalError<AdminCourseService>(this._logger, ex);
                result = false;
            }

            await this._ctx.EndTans(result);

            if (result)
            {
                if (videoContentsPath.Count > 0)
                {
                    // 全講座動画ファイル削除
                    var deleteFolderPath = $@"{this._env.WebRootPath}{ConstService.PathInfo.PATH_FOLDER_VIDEO}\{courseId}";
                    Directory.Delete(deleteFolderPath, true);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<MCourse> GetCourseById(string courseId)
        {
            return await this._course.SelectById(courseId);
        }

        /// <inheritdoc/>
        public async Task<bool> InsertCourse(MCourse course)
        {
            var results = await this._course.Insert(course);
            return (results == 1);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCourse(MCourse course)
        {
            var result = false;
            // 更新対象データを検索
            var mCourse = await this._course.SelectById(course.CourseId.ToString());
            if (mCourse.CourseId != Guid.Empty)
            {
                // 画面から更新する項目をセット
                mCourse.CourseName = course.CourseName;
                mCourse.CourseExplaination = course.CourseExplaination;
                mCourse.BegineDateTime = course.BegineDateTime;
                mCourse.EndDateTime = course.EndDateTime;
                mCourse.PublicFlg = course.PublicFlg;
                mCourse.PrimaryReference = course.PrimaryReference;
                mCourse.UpdatedBy = course.UpdatedBy;

                await this._ctx.SaveChangesAsync();
                result = true;
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteChapterById(string chapterId, Guid updatedBy)
        {
            var result = true;
            List<string> contentsList = [];
            try
            {
                // 講座マスタから削除
                MChapter mChapter = await this._chapter.SelectById(chapterId);
                if (mChapter.ChapterId != Guid.Empty)
                {
                    // 画面から更新する項目をセット
                    mChapter.DeletedFlg = true;
                    mChapter.UpdatedBy = updatedBy;
                }

                // コンテンツ情報（動画）
                List<MovieContents> movieContents = await this._ctx.MovieContents
                    .Where(x => x.ChapterId == Guid.Parse(chapterId))
                    .ToListAsync();
                if (movieContents.Count > 0)
                {
                    foreach (MovieContents mc in movieContents)
                    {
                        mc.DeletedFlg = true;
                        mc.UpdatedBy = updatedBy;

                        // 削除対象の動画コンテンツパスを保持
                        if (mc.ContentsPath != null)
                        {
                            contentsList.Add(mc.ContentsPath);
                        }
                    }
                }

                // コンテンツ情報（テスト）
                List<TestContents> testContents = await this._ctx.TestContents
                    .Where(x => x.ChapterId == Guid.Parse(chapterId))
                    .ToListAsync();
                if (testContents.Count > 0)
                {
                    List<Guid> testIdList = []; // コンテンツ識別子を確保
                    foreach (TestContents tc in testContents)
                    {
                        tc.DeletedFlg = true;
                        tc.UpdatedBy = updatedBy;
                        testIdList.Add(tc.ContentsId);
                    }
                    // 出題リスト
                    List<ExamList> examList = await this._ctx.ExamList
                        .Where(x => testIdList.Contains(x.ContentsId))
                        .ToListAsync();
                    if (examList != null)
                    {
                        this._ctx.RemoveRange(examList);
                    }
                }

                // 動画コンテンツファイルがある場合は削除
                if (contentsList.Count > 0)
                {
                    foreach (string path in contentsList)
                    {
                        CommonService.DeleteVideoContents(GetPhysicalVideoPath(path));
                    }
                }

                // 更新を反映
                await this._ctx.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // ファイル削除は戻せないのでログ出力して処理継続
                CommonService.CriticalError<AdminCourseService>(this._logger, ex);
                result = false;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateChapterOrderNo(string fixedTableCourseChapterData, Guid updatedBy)
        {
            try
            {
                // デシリアライズして List<Person> に変換
                var data = JsonSerializer.Deserialize<List<Person>>(fixedTableCourseChapterData);
                var result = false;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.ChapterId != null)
                        {
                            MChapter mChapter = await this._chapter.SelectById(item.ChapterId);
                            if (mChapter.ChapterId != Guid.Empty)
                            {
                                // 画面から更新する項目をセット
                                mChapter.OrderNo = Convert.ToByte(item.OrderNo);
                                mChapter.UpdatedBy = updatedBy;
                                await this._ctx.SaveChangesAsync();
                                result = true;
                            }
                        }
                        else { result = false; }
                    }
                }
                else { result = false; }
                return result;
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<AdminCourseService>(this._logger, ex);
                var result = false;
                return result;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsExistUserCourse(Guid courseId)
        {
            string sql = $@"SELECT COUNT (CourseID) FROM UserCourse WHERE CourseId = @courseId";
            var existsCourseId = await _ctx.Database.SqlQueryRaw<int>(
                @sql,
                new SqlParameter("@courseId", courseId)
                ).ToListAsync();

            return existsCourseId.FirstOrDefault() != 0;
        }

        /// <summary>
        /// HLSファイルへの相対パスを物理パスに変換する
        /// </summary>
        /// <param name="rerativePath">相対パス e.g. /video/{courseId}/{chapterId}/***********.m3u8</param>
        /// <returns></returns>
        private string GetPhysicalVideoPath(string rerativePath)
        {
            return $"{this._env.WebRootPath}{rerativePath.Replace('/', '\\')}";
        }
    }
}
