using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Formats.Asn1.AsnWriter;

namespace ElsWebApp.Controllers
{
    /// <summary>
    /// 管理者用コントローラークラス
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public class AdminQuestionsController(
            ILogger<AdminQuestionsController> logger,
            ElsWebAppDbContext context,
            IWebHostEnvironment env,
            SignInManager<IdentityUser> sInMng,
            IElsService svcEls,
            IQuestionCatalogService svcAC,
           ISysCodeService svcSys
        ) : BaseController(logger, context, env, sInMng)
    {
        private IElsService _elsService = svcEls;
        private IQuestionCatalogService _QCservice = svcAC;
        private ISysCodeService _sysCodeService = svcSys;

        /// <summary>
        /// テスト問題一覧表示、初期表示時
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> ShowAllQuestions()
        {
            var model = await SearchQuestion("", "", "");
            return View(model);
        }

        /// <summary>
        /// 検索機能
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> SearchActionForQuestion(string choiceClassMajor, string choiceClassMiddle, string choiceClassMinor)
        {
            var model = await SearchQuestion(choiceClassMajor, choiceClassMiddle, choiceClassMinor);
            return View("ShowAllQuestions", model);
        }

        /// <summary>
        /// 検索、初期表示のデータ絞り込み機能
        ///<param name="choiceClassMajor"></param>
        /// <param name="choiceClassMiddle"></param>
        /// <param name="choiceClassMinor"></param>
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<AdminQuestionsViewModel> SearchQuestion(string choiceClassMajor = "", string choiceClassMiddle = "", string choiceClassMinor = "")
        {
            //全問題を取得
            var questionList = await _QCservice.GetQuestionListInUsedFlg();

            var sysClass_QuestionCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_QUESTION);
            var sysClass_LevelCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_LEVEL);

            //大分類、中分類、小分類の種類を取得。これをもとに日本語へ置き換える
            var selectLists = await GetSelectList(choiceClassMajor, choiceClassMiddle, choiceClassMinor);

            var sysClass_MajorCode = selectLists.majorList;
            var sysClass_MiddleCode = selectLists.middleList;
            var sysClass_MinorCode = selectLists.minorList;

            // questionListのQuestionTypeを日本語に置き換える
            questionList.ForEach(question =>
            {
                question.QuestionType = sysClass_QuestionCode
                    .Where(codeValuePair => codeValuePair.Code == question.QuestionType)
                    .Select(codeValuePair => codeValuePair.Value)
                    .FirstOrDefault() ?? question.QuestionType;

                question.Level = sysClass_LevelCode
                    .Where(codeValuePair => codeValuePair.Code == question.Level)
                    .Select(codeValuePair => codeValuePair.Value)
                    .FirstOrDefault() ?? question.Level;
            });

            foreach (var codeValuePair in sysClass_MajorCode)
            {
                codeValuePair.Text = codeValuePair.Value + " - " + codeValuePair.Text; // 各要素を追加していく
            }

            foreach (var codeValuePair in sysClass_MiddleCode)
            {
                codeValuePair.Text = codeValuePair.Value + " - " + codeValuePair.Text; // 各要素を追加していく
            }

            foreach (var codeValuePair in sysClass_MinorCode)
            {
                codeValuePair.Text = codeValuePair.Value + " - " + codeValuePair.Text; // 各要素を追加していく
            }

            // 検索内容を反映、フィルタリング
            if (!string.IsNullOrEmpty(choiceClassMajor))
            {
                questionList = questionList.Where(s => s.MajorCd.Contains(choiceClassMajor)).ToList();
            }

            if (!string.IsNullOrEmpty(choiceClassMiddle))
            {
                questionList = questionList.Where(s => s.MiddleCd.Contains(choiceClassMiddle)).ToList();
            }

            if (!string.IsNullOrEmpty(choiceClassMinor))
            {
                questionList = questionList.Where(s => s.MinorCd.Contains(choiceClassMinor)).ToList();
            }

            var adminQuestionsViewModel = new AdminQuestionsViewModel
            {
                CustomQuestionCatalog = questionList,
                ClassMajor = sysClass_MajorCode,
                ClassMiddle = sysClass_MiddleCode,
                ClassMinor = sysClass_MinorCode,
                UserEnteredchoiceClassMajor = choiceClassMajor, //検索内容：大分類
                UserEnteredchoiceClassMiddle = choiceClassMiddle, //検索内容：中分類
                UserEnteredchoiceClassMinor = choiceClassMinor //検索内容：小分類
            };
            return adminQuestionsViewModel;


        }

        ///// <summary>
        ///// SystemCodeからSELECTBOXの中身を抽出する
        ///// </summary>
        ///// <returns></returns>
        private async Task<(List<SelectListItem> majorList, List<SelectListItem> middleList, List<SelectListItem> minorList, List<SelectListItem> levelList, List<SelectListItem> questionTypeList)> GetSelectList(string selectedMajor = "", string selectedMiddle = "", string selectedMinor = "", string selectedLevel = "", string selectedQuestionType = "")
        {
            // 大分類
            var sysMajorCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_MAJOR);
            var majorList = sysMajorCode.Select(code => new SelectListItem
            {
                Value = code.Code,
                Text = code.Value,
                Selected = code.Code == selectedMajor
            }).ToList();

            // 中分類
            var sysMiddleCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_MIDDLE);
            var middleList = sysMiddleCode.Select(code => new SelectListItem
            {
                Value = code.Code,
                Text = code.Value,
                Selected = code.Code == selectedMiddle
            }).ToList();
            // 小分類
            var sysMinorCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_MINOR);
            var minorList = sysMinorCode.Select(code => new SelectListItem
            {
                Value = code.Code,
                Text = code.Value,
                Selected = code.Code == selectedMinor
            }).ToList();

            // 難易度
            var sysLevelCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_LEVEL);
            var levelList = sysLevelCode.Select(code => new SelectListItem
            {
                Value = code.Code,
                Text = code.Value,
                Selected = code.Code == selectedLevel
            }).ToList();

            // 出題型
            var sysQuestionLevelCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_QUESTION);
            var questionTypeList = sysQuestionLevelCode.Select(code => new SelectListItem
            {
                Value = code.Code,
                Text = code.Value,
                Selected = code.Code == selectedQuestionType
            }).ToList();

            return (majorList, middleList, minorList, levelList, questionTypeList);
        }

        //詳細表示画面の表示
        [HttpPost]
        public async Task<IActionResult> ShowQuestionsView(string userEnteredchoiceClassMajor, string userEnteredchoiceClassMiddle, string userEnteredchoiceClassMinor, string questionId)
        {
            var questionList = await _QCservice.GetQuestionListInUsedFlg(questionId); // リストとして取得する

            var answerGroup = await _QCservice.SelectByIdForAnswerGroup(questionId);

            var selectLists = await GetSelectList();
            foreach (var question in questionList)
            {
                selectLists = await GetSelectList(question.MajorCd, question.MiddleCd, question.MinorCd, question.Level, question.QuestionType);
            }

            var sysClass_MajorCode = selectLists.majorList;
            var sysClass_MiddleCode = selectLists.middleList;
            var sysClass_MinorCode = selectLists.minorList;

            foreach (var codeValuePair in sysClass_MajorCode)
            {
                codeValuePair.Text = codeValuePair.Value + " - " + codeValuePair.Text; // 各要素を追加していく
            }

            foreach (var codeValuePair in sysClass_MiddleCode)
            {
                codeValuePair.Text = codeValuePair.Value + " - " + codeValuePair.Text; // 各要素を追加していく
            }

            foreach (var codeValuePair in sysClass_MinorCode)
            {
                codeValuePair.Text = codeValuePair.Value + " - " + codeValuePair.Text; // 各要素を追加していく
            }

            var questionCatalogData = new CustomQuestionCatalog();
            if (questionList != null && questionList.Count > 0)
            {
                var singleQuestion = questionList.FirstOrDefault(); // リストから最初の要素を取得する、適宜変更する

                questionCatalogData = new CustomQuestionCatalog()
                {
                    QuestionId = singleQuestion!.QuestionId,
                    CreatedAt = singleQuestion.CreatedAt,
                    CreatedBy = singleQuestion.CreatedBy,
                    DeletedFlg = singleQuestion.DeletedFlg,
                    Level = singleQuestion.Level,
                    QuestionType = singleQuestion.QuestionType,
                    MajorCd = singleQuestion.MajorCd,
                    MiddleCd = singleQuestion.MiddleCd,
                    MinorCd = singleQuestion.MinorCd,
                    QuestionImageData = singleQuestion.QuestionImageData,
                    QuestionImageName = singleQuestion.QuestionImageName,
                    QuestionText = singleQuestion.QuestionText,
                    QuestionTitle = singleQuestion.QuestionTitle,
                    Score = singleQuestion.Score,
                    SeqNo = singleQuestion.SeqNo,
                    UpdatedAt = singleQuestion.UpdatedAt,
                    UpdatedBy = singleQuestion.UpdatedBy,
                    UsedFlg = singleQuestion.UsedFlg,
                    FixedOrder = singleQuestion.FixedOrder,
                };
            }

            var adminQuestionsViewModel = new AdminQuestionsViewModel
            {
                CustomQuestionCatalog = questionList,
                AnswerGroup = answerGroup,
                UserEnteredchoiceClassMajor = userEnteredchoiceClassMajor, // 検索内容：大分類
                UserEnteredchoiceClassMiddle = userEnteredchoiceClassMiddle, // 検索内容：中分類
                UserEnteredchoiceClassMinor = userEnteredchoiceClassMinor, // 検索内容：小分類
                ClassMajor = selectLists.majorList,
                ClassMiddle = selectLists.middleList,
                ClassMinor = selectLists.minorList,
                ClassLevel = selectLists.levelList,
                ClassQuestionType = selectLists.questionTypeList,
                QuestionCatalogData = questionCatalogData,
            };

            return View("showQuestion", adminQuestionsViewModel);
        }


        //新規登録画面の表示
        public async Task<IActionResult> ShowQuestionsNewView(string userEnteredchoiceClassMajor, string userEnteredchoiceClassMiddle, string userEnteredchoiceClassMinor)
        {
            // SelectList取得
            var selectLists = await GetSelectList();

            var sysClass_MajorCode = selectLists.majorList;
            var sysClass_MiddleCode = selectLists.middleList;
            var sysClass_MinorCode = selectLists.minorList;

            foreach (var codeValuePair in sysClass_MajorCode)
            {
                codeValuePair.Text = codeValuePair.Value + " - " + codeValuePair.Text; // 各要素を追加していく
            }

            foreach (var codeValuePair in sysClass_MiddleCode)
            {
                codeValuePair.Text = codeValuePair.Value + " - " + codeValuePair.Text; // 各要素を追加していく
            }

            foreach (var codeValuePair in sysClass_MinorCode)
            {
                codeValuePair.Text = codeValuePair.Value + " - " + codeValuePair.Text; // 各要素を追加していく
            }

            var adminQuestionsViewModel = new AdminQuestionsViewModel
            {
                UserEnteredchoiceClassMajor = userEnteredchoiceClassMajor, //検索内容：大分類
                UserEnteredchoiceClassMiddle = userEnteredchoiceClassMiddle, //検索内容：中分類
                UserEnteredchoiceClassMinor = userEnteredchoiceClassMinor, //検索内容：小分類
                ClassMajor = selectLists.majorList,
                ClassMiddle = selectLists.middleList,
                ClassMinor = selectLists.minorList,
                ClassLevel = selectLists.levelList,
                ClassQuestionType = selectLists.questionTypeList
            };

            return View("showQuestion", adminQuestionsViewModel);
        }

        /// <summary>
        /// 削除フラグ更新
        /// 問題
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> DeleteAccount(string questionId)
        {
            var result = false;

            var questionList = await _QCservice.GetQuestionListInUsedFlg(questionId); // リストとして取得する

            if (questionList[0].UsedFlg == 1)
            {
                return BadRequest("削除できませんでした");
            }

            try
            {
                result = await _QCservice.UpdateDeleteFlg(questionId, _loginUser!.UserId);
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
            }

            return result ? Ok() : BadRequest("削除に失敗しました");
        }


        /// <summary>
        /// 削除フラグ更新
        /// 解答
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> DeleteQuestionDetail(string answerId)
        {
            var result = false;
            try
            {
                result = await this._QCservice.UpdateQuestionDetailDeleteFlg(answerId);

            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);

            }
            return (result) ? Ok() : BadRequest();
        }

        /// <summary>
        /// 新規データ登録機能
        /// </summary>
        /// <param name="choiceClassMajor">大分類</param>
        /// <param name="choiceClassMiddle">中分類</param>
        /// <param name="choiceClassMinor">小分類</param>
        /// <param name="titlePersonal">タイトル</param>
        /// <param name="questionPersonal">問題文</param>
        /// <param name="base64PictureBinary">問題画像データ</param>
        /// <param name="fileName">画像ファイル名</param>
        /// <param name="choiceClassLevel">難易度</param>
        /// <param name="choiceScore">点数</param>
        /// <param name="choiceClassQuestionType">出題形式</param>
        /// <param name="fixedAnswerGroupViewTableData"></param>
        /// <param name="fixedSortOrderSetting">並び順固定フラグ</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> InsertQuestion(
            string choiceClassMajor,
            string choiceClassMiddle,
            string choiceClassMinor,
            string titlePersonal,
            string questionPersonal,
            string base64PictureBinary,
            string fileName,
            string choiceClassLevel,
            byte choiceScore,
            string choiceClassQuestionType,
            string fixedAnswerGroupViewTableData,
            bool fixedSortOrderSetting)
        {
            try
            {
                var questionList = await _QCservice.GetQuestionListInUsedFlg("1","true");

                // 検索内容を繁栄、フィルタリング
                if (!string.IsNullOrEmpty(choiceClassMajor))
                {
                    questionList = questionList.Where(s => s.MajorCd.Contains(choiceClassMajor)).ToList();
                }

                if (!string.IsNullOrEmpty(choiceClassMiddle))
                {
                    questionList = questionList.Where(s => s.MiddleCd.Contains(choiceClassMiddle)).ToList();
                }

                if (!string.IsNullOrEmpty(choiceClassMinor))
                {
                    questionList = questionList.Where(s => s.MinorCd.Contains(choiceClassMinor)).ToList();
                }

                // questionListからSeqNoの最大値を取得します              
                var maxSeqNoString = questionList.Max(s => s.SeqNo);
                string fixedNextSeqNo = "";
                if (maxSeqNoString != null)
                {
                    // maxSeqNoStringを整数に変換する際に、変換が成功したかどうかを確認します
                    int maxSeqNo;

                    if (int.TryParse(maxSeqNoString, out maxSeqNo))
                    {
                        // 最大値に1を加えて次のSeqNoを計算します
                        var nextSeqNo = maxSeqNo + 1;

                        // 計算した次のSeqNoを5桁のフォーマットに整え、AAAに格納します
                        fixedNextSeqNo = nextSeqNo.ToString("D5");
                    }
                    else
                    {
                        // maxSeqNoStringがnullまたは変換に失敗した場合のエラー処理
                        Console.WriteLine("Error: Invalid SeqNoString");
                    }
                }
                else
                {
                    fixedNextSeqNo = "00001";
                }

                // 問題カタログオブジェクトの作成と挿入
                var DeletedFlg = false;
                var QuestionCatalog = new QuestionCatalog
                {
                    MajorCd = choiceClassMajor,
                    MiddleCd = choiceClassMiddle,
                    MinorCd = choiceClassMinor,
                    SeqNo = fixedNextSeqNo,
                    QuestionTitle = titlePersonal,
                    QuestionText = questionPersonal,
                    QuestionImageName = fileName,
                    QuestionImageData = base64PictureBinary,
                    QuestionType = choiceClassQuestionType,
                    Level = choiceClassLevel,
                    Score = choiceScore,
                    FixedOrder = fixedSortOrderSetting,
                    DeletedFlg = DeletedFlg,
                    CreatedBy = this._loginUser?.UserId,
                };
                List<AnswerGroup>? answerGroupList = JsonConvert.DeserializeObject<List<AnswerGroup>>(fixedAnswerGroupViewTableData);

                // 現在のログインユーザーのUserIdを取得
                Guid? createByUserId = this._loginUser?.UserId;                  

                // 各要素にCreateBy_UserIdを設定
                foreach (var answerGroup in answerGroupList!)
                {
                    answerGroup.CreatedBy = createByUserId;                  
                }

                var AnswerAndQuestionlist = new AdminQuestionsViewModel
                {
                    QuestionList = new List<QuestionCatalog> { QuestionCatalog },
                    AnswerGroup = answerGroupList,
                
                };
                var result = await _QCservice.InsertQuestionCatalogAndAnswerGroup(AnswerAndQuestionlist);

                // 挿入結果の確認
                if (result > 0)
                {
                    var myObject = new { result, choiceClassMajor };

                    // 挿入が成功した場合
                    return Ok(myObject);
                }
                else
                {
                    // 挿入が失敗した場合
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
                return BadRequest();
            }
        }

        /// <summary>
        /// 大中小の分類で検索し、その検索結果の最大SeqNo+1を取得する機能
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetMaxSeqNo(string choiceClassMajor, string choiceClassMiddle, string choiceClassMinor)
        {
            try
            {
                var questionList = await _QCservice.GetQuestionListInUsedFlg("1","true");

                // 検索内容を反映、フィルタリング
                if (!string.IsNullOrEmpty(choiceClassMajor))
                {
                    questionList = questionList.Where(s => s.MajorCd.Contains(choiceClassMajor)).ToList();
                }

                if (!string.IsNullOrEmpty(choiceClassMiddle))
                {
                    questionList = questionList.Where(s => s.MiddleCd.Contains(choiceClassMiddle)).ToList();
                }

                if (!string.IsNullOrEmpty(choiceClassMinor))
                {
                    questionList = questionList.Where(s => s.MinorCd.Contains(choiceClassMinor)).ToList();
                }

                // questionListからSeqNoの最大値を取得します
                var maxSeqNoString = questionList.Max(s => s.SeqNo);
                string fixedNextSeqNo = "";

                if (maxSeqNoString != null)
                {
                    // maxSeqNoStringを整数に変換する際に、変換が成功したかどうかを確認します
                    if (int.TryParse(maxSeqNoString, out int maxSeqNo))
                    {
                        // 最大値に1を加えて次のSeqNoを計算します
                        var nextSeqNo = maxSeqNo + 1;

                        // 計算した次のSeqNoを5桁のフォーマットに整えます
                        fixedNextSeqNo = nextSeqNo.ToString("D5");
                    }
                    else
                    {
                        // maxSeqNoStringがnullまたは変換に失敗した場合のエラー処理
                        Console.WriteLine("Error: Invalid SeqNoString");
                        throw new InvalidOperationException("Invalid SeqNoString: " + maxSeqNoString);
                    }
                }
                else
                {
                    fixedNextSeqNo = "00001";
                }

                var myObject = new { fixedNextSeqNo };
                return Ok(myObject);
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// 既存データ更新機能
        /// </summary>
        /// <param name="choiceClassMajor">大分類</param>
        /// <param name="choiceClassMiddle">中分類</param>
        /// <param name="choiceClassMinor">小分類</param>
        /// <param name="titlePersonal">タイトル</param>
        /// <param name="questionPersonal">問題文</param>
        /// <param name="base64PictureBinary">問題画像データ</param>
        /// <param name="fileName">画像ファイル名</param>
        /// <param name="choiceClassLevel">難易度</param>
        /// <param name="choiceScore">点数</param>
        /// <param name="choiceClassQuestionType">出題形式</param>
        /// <param name="seqNoPersonal">連番</param>
        /// <param name="questionIdPersonal">問題ID</param>
        /// <param name="questionTbodyList"></param>
        /// <param name="fixedAnswerGroupViewTableData"></param>
        /// <param name="fixedSortOrderSetting">並び順固定フラグ</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateQuestion(
            string choiceClassMajor,
            string choiceClassMiddle,
            string choiceClassMinor,
            string titlePersonal,
            string questionPersonal,
            string base64PictureBinary,
            string fileName,
            string choiceClassLevel,
            byte choiceScore,
            string choiceClassQuestionType,
            string seqNoPersonal,
            Guid questionIdPersonal,
            List<AnswerGroup> questionTbodyList,
            string fixedAnswerGroupViewTableData,
            bool fixedSortOrderSetting)
        {
            try
            {
                var questionList = await _QCservice.GetQuestionListInUsedFlg(questionIdPersonal.ToString()); // リストとして取得する
                var usedFlg = questionList[0].UsedFlg;

                List<AnswerGroup> answerGroupList = questionTbodyList;

                var DeletedFlg = false;
                var QuestionCatalog = new QuestionCatalog
                {
                    QuestionId = questionIdPersonal,
                    MajorCd = choiceClassMajor,
                    MiddleCd = choiceClassMiddle,
                    MinorCd = choiceClassMinor,
                    SeqNo = seqNoPersonal,
                    QuestionTitle = titlePersonal,
                    QuestionText = questionPersonal,
                    QuestionImageName = fileName,
                    QuestionImageData = base64PictureBinary,
                    QuestionType = choiceClassQuestionType,
                    Level = choiceClassLevel,
                    Score = choiceScore,
                    FixedOrder = fixedSortOrderSetting,
                    DeletedFlg = DeletedFlg,
                    UpdatedBy = this._loginUser?.UserId,
                };
                //////////////////////////////////////////
                List<AnswerGroup>? answerGroupViewTableDataForList = JsonConvert.DeserializeObject<List<AnswerGroup>>(fixedAnswerGroupViewTableData);

                // 現在のログインユーザーのUserIdを取得
                Guid? createByUserId = this._loginUser?.UserId;

                // 各要素にCreateBy_UserIdを設定
                foreach (var item in answerGroupViewTableDataForList!)
                {
                    item.UpdatedBy = createByUserId;
                }

                /////////////////////////////////////////
                var answerAndQuestionlist = new AdminQuestionsViewModel
                {
                    QuestionList = new List<QuestionCatalog> { QuestionCatalog },
                    AnswerGroup = answerGroupViewTableDataForList
                };

                var result = await _QCservice.InsertQuestionCatalogAndAnswerGroup(answerAndQuestionlist);
                // 挿入結果の確認
                if (result >= 0)
                {
                    var myObject = new { result, choiceClassMajor, usedFlg };

                    // 挿入が成功した場合
                    return Ok(myObject);
                }
                else
                {
                    // 挿入が失敗した場合
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
                return BadRequest();
            }
        }

    }
}


