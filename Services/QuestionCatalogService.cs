using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using static System.Formats.Asn1.AsnWriter;

namespace ElsWebApp.Services
{
    public class QuestionCatalogService(ElsWebAppDbContext ctx, ILogger<QuestionCatalogService> logger) : IQuestionCatalogService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<QuestionCatalogService> _logger = logger;

        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        /// <inheritdoc/>
        public async Task<QuestionCatalog> SelectById(string id)
        {
            QuestionCatalog mQuest = new();
            try
            {
                mQuest = await this._context.QuestionCatalog
                    .Where(x => x.QuestionId == Guid.Parse(id))
                    .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return mQuest;
        }

        /// <inheritdoc/>
        public async Task<List<AnswerGroup>> SelectByIdForAnswerGroup(string id)
        {
            List<AnswerGroup> answerList = new List<AnswerGroup>();
            try
            {
                answerList = await this._context.AnswerGroup
                     .Where(x => x.QuestionId == Guid.Parse(id))
                     .Where(x => x.DeletedFlg == false)
                     .OrderBy(q => q.OrderNo) //解答の降順
                     .ToListAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return answerList;
        }

        /// <inheritdoc/>
        public async Task<AnswerGroup> SelectByIdForAnswerGroupQuestionDetailDelFlg(string id)
        {
            AnswerGroup answerList = new();
            try
            {
                answerList = await this._context.AnswerGroup
                     .Where(x => x.AnswerId == Guid.Parse(id))
                      .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return answerList;
        }

        /// <inheritdoc/>
        public async Task<AnswerGroup> SelectByIdForAnswerGroupCheckAnswerId(string id)
        {
            AnswerGroup answerList = new();
            try
            {
                answerList = await this._context.AnswerGroup
                     .Where(x => x.AnswerId == Guid.Parse(id))
                    .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return answerList;
        }

        /////////////////////////////////////////

        public async Task<int> InsertQuestionCatalogAndAnswerGroup(AdminQuestionsViewModel data)
        {
            int result = 0;
            var questionList = await GetQuestionListInUsedFlg(data.QuestionList[0].QuestionId.ToString());
            var usedFlgNum = questionList.Count > 0 ? questionList[0].UsedFlg : 0;

            // トランザクションを開始
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // `data` が null でない場合
                    if (data != null)
                    {
                        var questionCatalogsQuestionId = "";
                        // QuestionListがnullでないか確認
                        if (data.QuestionList != null)
                        {
                            foreach (var QuestionCatalogItem in data.QuestionList)
                            {
                                var question = await SelectById(QuestionCatalogItem.QuestionId.ToString());
                                if (question.QuestionId == Guid.Empty)
                                {
                                    this._context.QuestionCatalog.Add(QuestionCatalogItem);
                                    questionCatalogsQuestionId = QuestionCatalogItem.QuestionId.ToString();
                                    //result = await this._context.SaveChangesAsync();
                                    //result = await _context.SaveChangesAsync();

                                }
                                else
                                {
                                    //question.QuestionId = QuestionCatalogItem.QuestionId;
                                    if (usedFlgNum != 1)
                                    {
                                        question.MajorCd = QuestionCatalogItem.MajorCd;
                                        question.MiddleCd = QuestionCatalogItem.MiddleCd;
                                        question.MinorCd = QuestionCatalogItem.MinorCd;
                                        question.QuestionType = QuestionCatalogItem.QuestionType;
                                        question.Level = QuestionCatalogItem.Level;
                                        question.Score = QuestionCatalogItem.Score;
                                        question.DeletedFlg = QuestionCatalogItem.DeletedFlg;
                                        question.SeqNo = QuestionCatalogItem.SeqNo;
                                    }
                                    question.QuestionTitle = QuestionCatalogItem.QuestionTitle;
                                    question.QuestionText = QuestionCatalogItem.QuestionText;
                                    question.QuestionImageName = QuestionCatalogItem.QuestionImageName;
                                    question.QuestionImageData = QuestionCatalogItem.QuestionImageData;
                                    question.FixedOrder = QuestionCatalogItem.FixedOrder;


                                    question.UpdatedBy = QuestionCatalogItem.UpdatedBy;
                                    questionCatalogsQuestionId = question.QuestionId.ToString();
                                }
                            }
                        }
                        //AnswerGroupがnullでないか確認
                        // `AnswerGroup` の処理
                        if (data.AnswerGroup != null)
                        {
                            var orderNo = 0;
                            foreach (var answerGroupItem in data.AnswerGroup)
                            {
                                // 既存の AnswerGroup を検索
                                var existingAnswerGroupList = await SelectByIdForAnswerGroupCheckAnswerId(answerGroupItem.AnswerId.ToString());
                                if (existingAnswerGroupList.AnswerId == Guid.Empty)
                                {
                                    if (!answerGroupItem.DeletedFlg)
                                    {
                                        if (usedFlgNum != 1)
                                        {
                                            answerGroupItem.OrderNo = (byte)orderNo;
                                            answerGroupItem.CreatedBy = answerGroupItem.UpdatedBy ?? answerGroupItem.CreatedBy;
                                            answerGroupItem.UpdatedBy = null;
                                            answerGroupItem.QuestionId = new Guid(questionCatalogsQuestionId);
                                            this._context.AnswerGroup.Add(answerGroupItem);
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else// 存在する場合は更新
                                {
                                    //var existingAnswerGroup = existingAnswerGroupList[0];
                                    answerGroupItem.QuestionId = Guid.Parse(questionCatalogsQuestionId);
                                    if (usedFlgNum != 1)
                                    {
                                        existingAnswerGroupList.AnswerId = answerGroupItem.AnswerId;
                                        existingAnswerGroupList.QuestionId = answerGroupItem.QuestionId;
                                        existingAnswerGroupList.ErrataFlg = answerGroupItem.ErrataFlg;
                                        existingAnswerGroupList.DeletedFlg = answerGroupItem.DeletedFlg;
                                        existingAnswerGroupList.UpdatedBy = answerGroupItem.CreatedBy ?? answerGroupItem.UpdatedBy;
                                    }
                                    existingAnswerGroupList.AnswerText = answerGroupItem.AnswerText;
                                    existingAnswerGroupList.AnswerImageName = answerGroupItem.AnswerImageName;
                                    existingAnswerGroupList.AnswerImageData = answerGroupItem.AnswerImageData;
                                    existingAnswerGroupList.ExplanationText = answerGroupItem.ExplanationText;
                                    existingAnswerGroupList.ExplanationImageName = answerGroupItem.ExplanationImageName;
                                    existingAnswerGroupList.ExplanationImageData = answerGroupItem.ExplanationImageData;
                                    existingAnswerGroupList.OrderNo = (byte)orderNo;
                                }
                                orderNo++;
                            }
                            await transaction.CommitAsync();
                            result = await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // エラーが発生した場合、トランザクションは自動的にロールバックされます
                    await transaction.RollbackAsync();
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<int> InsertAnswerGroup(AnswerGroup data)
        {
            int result = 0;
            try
            {
                this._context.AnswerGroup.Add(data);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<(int result, QuestionCatalog data)> UpdateQuestionCatalog(QuestionCatalog data)
        {
            var result = 0;
            var mQuest = await this.SelectById(data.QuestionId.ToString());

            try
            {
                mQuest.MajorCd = data.MajorCd;
                mQuest.MiddleCd = data.MiddleCd;
                mQuest.MinorCd = data.MinorCd;
                mQuest.SeqNo = data.SeqNo;
                mQuest.QuestionTitle = data.QuestionTitle;
                mQuest.QuestionText = data.QuestionText;
                mQuest.QuestionImageName = data.QuestionImageName;
                mQuest.QuestionImageData = data.QuestionImageData;
                mQuest.Level = data.Level;
                mQuest.Score = data.Score;
                mQuest.DeletedFlg = data.DeletedFlg;

                result = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return (result, data);
        }

        /// <inheritdoc/>
        public async Task<int> Update(QuestionCatalog data)
        {
            var result = 0;
            var mQuest = await this.SelectById(data.QuestionId.ToString());

            try
            {
                mQuest.MajorCd = data.MajorCd;
                mQuest.MiddleCd = data.MiddleCd;
                mQuest.MinorCd = data.MinorCd;
                mQuest.SeqNo = data.SeqNo;
                mQuest.QuestionTitle = data.QuestionTitle;
                mQuest.QuestionText = data.QuestionText;
                mQuest.QuestionImageName = data.QuestionImageName;
                mQuest.QuestionImageData = data.QuestionImageData;
                mQuest.Level = data.Level;
                mQuest.Score = data.Score;
                mQuest.DeletedFlg = data.DeletedFlg;

                result = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        public async Task<int> UpdateQuestionDetail(AnswerGroup data)
        {
            var result = 0;
            var mQuest = await this.SelectByIdForAnswerGroupQuestionDetailDelFlg(data.AnswerId.ToString());

            try
            {
                mQuest.DeletedFlg = data.DeletedFlg;



                result = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> Insert(QuestionCatalog data)
        {
            int result = 0;
            try
            {
                this._context.QuestionCatalog.Add(data);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 必要な場合以下に記載
            }
        }

        /// <inheritdoc/>
        public async Task<List<QuestionCatalog>> GetAllQuestionList() //全権取得
        {
            List<QuestionCatalog> questionList = new List<QuestionCatalog>();
            try
            {
                questionList = await this._context.QuestionCatalog
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return questionList;
        }

        /// <inheritdoc/>
        public async Task<List<CodeValuePair>> GetClassCodeListForViewModel(string classId)
        {
            List<CodeValuePair> pairList = [];
            try
            {
                pairList = await this._context.MSysCode
                    .Where(x => x.ClassId == classId)
                    .Where(x => x.ClassCd != "--")
                    .OrderBy(x => x.ClassCd)
                    .Select(x => new CodeValuePair
                    {
                        Code = x.ClassCd,
                        Value = x.ClassName
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return pairList;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateDeleteFlg(string QuestionId, Guid mineUserId)
        {
            var user = await this.SelectById(QuestionId);

            user.DeletedFlg = true;
            user.UpdatedBy = mineUserId;
            var result = await this.Update(user);

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuestionDetailDeleteFlg(string AnswerId)
        {
            AnswerGroup user = new AnswerGroup();
            // AnswerGroup 型のオブジェクトを取得
            user = await this.SelectByIdForAnswerGroupQuestionDetailDelFlg(AnswerId);

            // DeletedFlg を true に設定
            user.DeletedFlg = true;

            // QuestionCatalog 型のオブジェクトを更新
            var result = await this.UpdateQuestionDetail(user);

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private QuestionCatalog ConvertToQuestionCatalog(AnswerGroup answerGroup)
        {
            // 新しい QuestionCatalog オブジェクトを作成
            QuestionCatalog questionCatalog = new QuestionCatalog();
            // 必要なプロパティを設定（例：DeletedFlg）
            questionCatalog.DeletedFlg = answerGroup.DeletedFlg;
            // 他のプロパティもコピーする必要があれば追加
            // questionCatalog.プロパティ = answerGroup.プロパティ;

            return questionCatalog;
        }

        /// <inheritdoc/>
        public async Task<List<QuestionCatalog>> SelectByClassCd(string majorCd, string middleCd, string minorCd)
        {
            List<QuestionCatalog>? qList = null;
            try
            {
                var query = this._context.QuestionCatalog.
                    Where(x => !x.DeletedFlg);
                // 大分類
                if ((majorCd ?? "") != "")
                {
                    query = query.Where(x => x.MajorCd == majorCd);
                }
                // 中分類
                if ((middleCd ?? "") != "")
                {
                    query = query.Where(x => x.MiddleCd == middleCd);
                }
                // 小分類
                if ((minorCd ?? "") != "")
                {
                    query = query.Where(x => x.MinorCd == minorCd);
                }

                qList = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return qList ?? [];
        }

        public async Task<List<CustomQuestionCatalog>> GetQuestionListInUsedFlg(string questionId = "1", string IsGetDelFlg = "false") // questionId="1"全件取得_IsGetDelFlg="false"DeletedFlgがFalseのみ取得

        {
            try
            {
                string sql = @"
            WITH userTestContentIist AS ( 
                SELECT DISTINCT
                    el.QuestionId
                FROM
                    UserChapter uc
                INNER JOIN TestContents tc ON (uc.ChapterId = tc.ChapterId)
                INNER JOIN ExamList el ON (el.ContentsId = tc.ContentsId)
            )
            SELECT
                CASE WHEN uc.QuestionId IS NULL
                    THEN 0
                    ELSE 1
                END AS UsedFlg,
                qc.*
            FROM
                QuestionCatalog qc
            LEFT JOIN userTestContentIist uc ON (qc.QuestionId = uc.QuestionId)
            @WHERE 
                @IsGetDelFlg
                @isAND
                @isQuestionId
            ORDER BY
                qc.MajorCd,
                qc.MiddleCd,
                qc.MinorCd,
                qc.SeqNo;
        ";

                string isQuestionIdCondition = questionId == "1" ? "" : "qc.QuestionId = @questionId";
                string DelFlg = IsGetDelFlg == "false" ? "qc.DeletedFlg = 'false'" : "";
                string isAnd = "";
                if (isQuestionIdCondition != "" && DelFlg != "")
                {
                    isAnd = "AND";
                }
                string isWhere = "";
                if (isQuestionIdCondition != "" && DelFlg != "" || questionId != "1")
                {
                    isWhere = "WHERE";
                }
                sql = sql.Replace("@IsGetDelFlg", DelFlg);
                sql = sql.Replace("@isQuestionId", isQuestionIdCondition);
                sql = sql.Replace("@WHERE", isWhere);
                sql = sql.Replace("@isAND", isAnd);
                // パラメータを設定
                var parameters = new List<SqlParameter>();
                if (questionId != "1")
                {
                    parameters.Add(new SqlParameter("@questionId", questionId));
                }
                // SQL クエリを非同期で実行して結果を取得する
                var taskStatusList = await _context.Database.SqlQueryRaw<CustomQuestionCatalog>(sql, parameters.ToArray()).ToListAsync();
                return taskStatusList;
            }
            catch (Exception ex)
            {
                CriticalError(ex); // エラーハンドリング
                throw;
            }
        }

    }
}
