using ElsWebApp.Models.Entitiy;
using ElsWebApp.Models;
using ElsWebApp.Services.IService;
using NuGet.Versioning;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsWebApp.Services
{
    public class CreateExamService(
        ElsWebAppDbContext ctx,
        ICourseService svcCourse,
        IChapterService svcChapter,
        IExamListService svcExamList,
        IQuestionCatalogService svcQCatalog,
        IAnswerGroupService svcAGroup,
        ITestContentsService svcTest,
        IUserChapterService svcUserChapter,
        IUserExamService svcUserExam,
        IUserScoreService svcUserScore
    ) : ICreateExamService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        protected ICourseService _courseService = svcCourse;
        protected IChapterService _chapterService = svcChapter;
        protected IExamListService _examListService = svcExamList;
        protected IQuestionCatalogService _qCatalogService = svcQCatalog;
        protected IAnswerGroupService _aGroupService = svcAGroup;
        protected ITestContentsService _testService = svcTest;
        protected IUserChapterService _userChapterService = svcUserChapter;
        protected IUserExamService _userExamService = svcUserExam;
        protected IUserScoreService _userScoreService = svcUserScore;

        /// <inheritdoc/>
        public async Task<ShowTestContentsViewModel> CreateStudentExamination(string userId, string courseId, string chapterId)
        {
            // 受講者講座データの取得
            var userChapter = await this._userChapterService.SelectByUniqueIndex(Guid.Parse(userId), Guid.Parse(courseId), Guid.Parse(chapterId));
            if (userChapter.UserChapterId == Guid.Empty)
            {
                throw new Exception("有効な受講者講座データがありません");
            }

            // コンテンツ(テスト)情報の取得
            var contents = await this._testService.SelectByChapterId(Guid.Parse(chapterId));

            // 実施回数が最大の受講者出題データ情報を取得
            var maxExam = await this._userExamService.GetMaxTimeExamInfo(userChapter.UserChapterId);
            var maxTime = (maxExam.Count != 0)? maxExam[0].NthTime : 0;

            // 出題リスト取得
            var examList = await this._examListService.SelectByContentsId(contents.ContentsId);

            // 並べ替え
            var ranQList = examList.OrderBy(x => Guid.NewGuid()).ToList()[..Math.Min(contents.Questions, examList.Count)];

            // 解答グループの取得
            var idx = 0;
            var queListArray = new List<AnswerGroup>[ranQList.Count];
            foreach (var item in ranQList)
            {
                var answer = await this._aGroupService.SelectByQuestionId(item.QuestionId);

                if (!item.QuestionCatalog? .FixedOrder?? false)
                {
                    // ランダムで並び替え
                    queListArray[idx++] = [.. answer.OrderBy(x => Guid.NewGuid())];
                }
                else
                {
                    // 表示順で並び替え
                    queListArray[idx++] = [.. answer.OrderBy(x => x.OrderNo)];
                }
            }

            // コンテンツ(テスト)表示Viewモデル生成
            var model = new ShowTestContentsViewModel
            {
                IsDisplayMode = false,
                UserId = userChapter.UserId,
                CourseId = userChapter.CourseId,
                ChapterId = userChapter.ChapterId,
                UserChapterId = userChapter.UserChapterId,
                Times = maxTime + 1,
                QuestionCount = ranQList.Count,
                LimitLime = contents.LimitTime,
            };

            // トランザクション開始
            await this._context.BeginTans();

            // 受講者-講座情報更新
            if ((maxTime ==0) && (userChapter.StartDatetime == null))
            {
                userChapter.Status = ConstService.SystemCode.SYSCODE_STS_STUDYING;
                userChapter.StartDatetime = DateTime.Now;
                userChapter.UpdatedBy = Guid.Parse(userId);
                await this._userChapterService.Update(userChapter);
            }

            // 受講者出題データ作成
            var qCount = 0;
            byte order = 0;
            foreach (var question in ranQList)
            {
                var userExam = new UserExam
                {
                    UserChapterId = userChapter.UserChapterId,
                    NthTime = (byte)(maxTime + 1),
                    QuestionId = question.QuestionId,
                    DisplayOrder = order++,
                    UpdatedBy = Guid.Parse(userId),
                    CreatedBy = Guid.Parse(userId),
                };
                qCount += await this._userExamService.Insert(userExam);

                model.Questions.Add(new QuestionInfo
                {
                    QId = question.QuestionId,
                    QText = question.QuestionCatalog!.QuestionText ?? "",
                    QImage = question.QuestionCatalog!.QuestionImageData ?? "",
                });
            }

            // 受講者採点データ作成
            var sCount = 0;
            for (var qIdx = 0; qIdx < queListArray.Length; qIdx++)
            {
                order = 0;
                var qInfo = model.Questions.Where(x => x.QId == queListArray[qIdx][0].QuestionId).FirstOrDefault();
                foreach (var ans in queListArray[qIdx])
                {
                    var userScore = new UserScore
                    {
                        UserChapterId = userChapter.UserChapterId,
                        NthTime = (byte)(maxTime + 1),
                        QuestionId = ans.QuestionId,
                        AnswerId = ans.AnswerId,
                        DisplayOrder = order++,
                        AnswerValue = false,
                        Result = 0,
                        UpdatedBy = Guid.Parse(userId),
                        CreatedBy = Guid.Parse(userId),
                    };
                    sCount += await this._userScoreService.Insert(userScore);

                    qInfo? .Answers.Add(new AnswerInfo
                    {
                        AId = ans.AnswerId,
                        AText = ans.AnswerText?? "",
                        AImage = ans.AnswerImageData?? "",
                        EText = string.Empty,
                        AValue = false,
                        Status = string.Empty,
                    });
                }
            }

            // トランザクション終了
            await this._context.EndTans(true);

            // 前講座情報設定
            var prev = await this._chapterService.GetPrevChapter(userChapter.ChapterId);
            model.PrevChapter.ChapterId = prev.ChapterId;
            model.PrevChapter.ContentsType = prev.ContentsType;

            // 次講座情報設定
            var next = await this._chapterService.GetNextChapter(userChapter.ChapterId);
            model.NextChapter.ChapterId = next.ChapterId;
            model.NextChapter.ContentsType = next.ContentsType;

            return model;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckCourseValid(string courseId)
        {
            var result = true;
            var course = await this._courseService.SelectById(courseId);

            // コースマスタ存在確認
            if (course == null)
            {
                result = false;
            }
            else
            {
                // 公開チェック
                if (course.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_PERIOD)
                {
                    // 有効期間チェック
                    if ((course.BegineDateTime > DateTime.Now) || (course.EndDateTime < DateTime.Now))
                    {
                        result = false;
                    }
                }
                else
                {
                    // 公開フラグチェック
                    if (!course.PublicFlg)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
    
        /// <inheritdoc/>
        public async Task<bool> GradeStudentExamination(Guid userId, ShowTestContentsViewModel model)
        {
            // 受講者講座データ取得
            var userChapter = await this._userChapterService.SelectById(model.UserChapterId.ToString());
            if (userChapter.UserChapterId == Guid.Empty)
            {
                throw new Exception("有効な受講者の講座情報がありません");
            }

            // 答え合わせ
            var scoreData = new Dictionary<Guid, List<UserScore>>();
            foreach (var que in model.Questions)
            {
                // 受講者採点データ取得
                var userScore = await this._userScoreService.GetUserScoreList(model.UserChapterId, model.Times, que.QId);
 
                // 解答グループ取得
                var ansGroupList = await this._aGroupService.SelectByQuestionId(que.QId);

                foreach (var ans in que.Answers)
                {
                    var score = userScore.Where(x => x.AnswerId == ans.AId).FirstOrDefault();
                    var collectAnswer = ansGroupList.Where(x => x.AnswerId == ans.AId).FirstOrDefault();
                    if ((collectAnswer != null) && (score != null))
                    {
                        // 解答、採点結果を設定
                        score.AnswerValue = ans.AValue;
                        score.Result = byte.Parse((collectAnswer.ErrataFlg == ans.AValue)? 
                            ConstService.SystemCode.SYSCODE_ANS_CORRECT : ConstService.SystemCode.SYSCODE_ANS_INCORRECT);
                        score.UpdatedBy = userId;
                    }
                }
                scoreData.Add(que.QId, userScore);
            }

            var efScore = 0;
            var efChapter = 0;
            var result = true;

            // トランザクション開始
            await this._context.BeginTans();

            try
            {
                // 受講者採点データ更新
                efScore = await this._userScoreService.UpdateFromUserScoreList(scoreData);

                // 受講講座データ更新
                if ((userChapter.UserChapterId != Guid.Empty) && (userChapter.EndDatetime == null))
                {
                    userChapter.EndDatetime = DateTime.Now;
                    userChapter.Status = ConstService.SystemCode.SYSCODE_STS_COMPLETE;
                    userChapter.UserId = userId;
                    efChapter = await this._userChapterService.Update(userChapter);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                result = false;
            }

            // トランザクションの終了
            await this._context.EndTans(result);

            return result;
        }

        public async Task<ShowExaminationHistoryViewModel> GetExaminationHistory(string userId, string courseId, string chapterId)
        {
            // 受講者講座データの取得
            var userChapter = await this._userChapterService.SelectByUniqueIndex(Guid.Parse(userId), Guid.Parse(courseId), Guid.Parse(chapterId));
            if (userChapter.UserChapterId == Guid.Empty)
            {
                throw new Exception("有効な受講者の講座情報がありません");
            }

            // 受講者出題データの取得
            var userExam = await this._userExamService.GetAllTimesExamInfo(userChapter.UserChapterId);
            // 実施回数の取得
            var times = userExam.Select(x => x.NthTime).FirstOrDefault();

            var examIdx = 0;
            List<QuestionInfo>? questionList = [];
            List<ExaminationInfo> history = [];
            history.Add(new ExaminationInfo
            {
                Times = times,
                Questions = questionList,
                CollectCount = 0
            });

            foreach (var exam in userExam)
            {
                if (exam.NthTime != times)
                {
                    examIdx++;
                    times = exam.NthTime;
                    questionList = [];
                    history.Add( new ExaminationInfo {
                        Times = times,
                        Questions = questionList,
                        CollectCount = 0
                    });
                }

                var qInfo = new QuestionInfo
                {
                    QId = exam.QuestionId,
                    QText = exam.QuestionCatalog!.QuestionText ?? "",
                    QImage = exam.QuestionCatalog!.QuestionImageData ?? "",
                };
                // 受講者採点データ取得
                var userScore = await this._userScoreService.GetUserScoreList(userChapter.UserChapterId, times, exam.QuestionId);
                var resunt = 0;
                foreach (var score in userScore)
                {
                    var ans = new AnswerInfo
                    {
                        AId = score.AnswerId,
                        AText = score.AnswerGroup!.AnswerText ?? "",
                        AImage = score.AnswerGroup!.AnswerImageData ?? "",
                        EText = score.AnswerGroup!.ExplanationText ?? "",
                        AValue = score.AnswerValue,
                        Status = (score.AnswerValue) ? score.Result.ToString() : string.Empty,
                    };
                    qInfo.Answers.Add(ans);

                    resunt += score.Result;
                }
                questionList.Add(qInfo);
                if (resunt >= userScore.Count) 
                {
                    history[examIdx].CollectCount++;
                }
            }

            var model = new ShowExaminationHistoryViewModel
            {
                UserChapterId = userChapter.UserChapterId,
                UserId = userChapter.UserId,
                CourseId = userChapter.CourseId,
                ChapterId = userChapter.ChapterId,
                ExamHistory = history,
            };
            // 前講座情報設定
            var prev = await this._chapterService.GetPrevChapter(userChapter.ChapterId);
            model.PrevChapter.ChapterId = prev.ChapterId;
            model.PrevChapter.ContentsType = prev.ContentsType;

            // 次講座情報設定
            var next = await this._chapterService.GetNextChapter(userChapter.ChapterId);
            model.NextChapter.ChapterId = next.ChapterId;
            model.NextChapter.ContentsType = next.ContentsType;

            return model;
        }


        /// <inheritdoc/>
        public async Task<UserChapter> GetUserChapterData(Guid userChapterId)
        {
            return await this._userChapterService.SelectById(userChapterId.ToString());
        }
    
        public async Task<ShowTestContentsViewModel> GetStudentExaminationResult(string userId, string courseId, string chapterId, string times)
        {
            // 受講者講座データの取得
            var userChapter = await this._userChapterService.SelectByUniqueIndex(Guid.Parse(userId), Guid.Parse(courseId), Guid.Parse(chapterId));
            if (userChapter.UserChapterId == Guid.Empty)
            {
                throw new Exception("有効な受講者の講座情報がありません");
            }

            var model = new ShowTestContentsViewModel
            {
                IsDisplayMode = true,
                UserId = userChapter.UserId,
                CourseId = userChapter.CourseId,
                ChapterId = userChapter.ChapterId,
                UserChapterId = userChapter.UserChapterId,
                Times = int.Parse(times),
            };

            // 受講者出題データの取得
            var userExam = await this._userExamService.GetNthTimeExamInfo(userChapter.UserChapterId, int.Parse(times));
        
            var cC = 0;
            foreach (var exam in userExam)
            {
                var qInfo = new QuestionInfo
                {
                    QId = exam.QuestionId,
                    QText = exam.QuestionCatalog!.QuestionText ?? "",
                    QImage = exam.QuestionCatalog!.QuestionImageData ?? "",
                };

                // 受講者採点データ取得
                var result = 0;
                var userScore = await this._userScoreService.GetUserScoreList(userChapter.UserChapterId, int.Parse(times), exam.QuestionId);
                foreach (var score in userScore)
                {
                    var ans = new AnswerInfo
                    {
                        AId = score.AnswerId,
                        AText = score.AnswerGroup!.AnswerText ?? "",
                        AImage = score.AnswerGroup!.AnswerImageData ?? "",
                        EText = score.AnswerGroup!.ExplanationText ?? "",
                        AValue = score.AnswerValue,
                        Status = (score.AnswerValue) ? score.Result.ToString() : string.Empty,
                    };
                    result += score.Result;
                    qInfo.Answers.Add(ans);
                }
                if (result >= userScore.Count)
                {
                    // 正解数のカウント
                    cC++;
                }
                model.Questions.Add(qInfo);
            }
            model.QuestionCount = model.Questions.Count;
            model.CollectCount = cC;
            model.LimitLime = 0;

            // 前講座情報設定
            var prev = await this._chapterService.GetPrevChapter(userChapter.ChapterId);
            model.PrevChapter.ChapterId = prev.ChapterId;
            model.PrevChapter.ContentsType = prev.ContentsType;
            model.PrevChapter.IsDelete = prev.DeletedFlg;

            // 次講座情報設定
            var next = await this._chapterService.GetNextChapter(userChapter.ChapterId);
            model.NextChapter.ChapterId = next.ChapterId;
            model.NextChapter.ContentsType = next.ContentsType;
            model.NextChapter.IsDelete = next.DeletedFlg;

            return model;
        }
    }
}
