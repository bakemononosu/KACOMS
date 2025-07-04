using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class ChapterService(ElsWebAppDbContext ctx, ILogger<ChapterService> logger) : IChapterService
    {
        private readonly ILogger<ChapterService> _logger = logger;
        private readonly ElsWebAppDbContext _context = ctx;

        /// <inheritdoc/>
        public async Task<MChapter> SelectById(string id)
        {
            MChapter mChapter = new();
            try
            {
                mChapter = await this._context.MChapter
                    .Where(x => x.ChapterId == Guid.Parse(id))
                    .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ChapterService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return mChapter;
        }

        /// <inheritdoc/>
        public async Task<int> Insert(MChapter data)
        {
            int result = 0;
            try
            {
                this._context.MChapter.Add(data);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ChapterService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> Update(MChapter data)
        {
            var result = 0;
            var mChapter = await this.SelectById(data.ChapterId.ToString());

            if (mChapter.ChapterId != Guid.Empty)
            {
                mChapter.ChapterName = data.ChapterName;
                mChapter.ContentsType = data.ContentsType;
                mChapter.CourseId = data.CourseId;
                mChapter.OrderNo = data.OrderNo;
                mChapter.DeletedFlg = data.DeletedFlg;
                mChapter.UpdatedBy = data.UpdatedBy;

                try
                {
                    result = await this._context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    CommonService.CriticalError<ChapterService>(this._logger, ex);
                    throw new Exception(ex.Message);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<MChapter>> SelectByCourseId(Guid courseId)
        {
            List<MChapter>? result = null;
            try
            {
                result = await this._context.MChapter
                    .Where(x => x.CourseId == courseId)
                    .Where(x => !x.DeletedFlg)
                    .OrderBy(x => x.OrderNo)
                    .Select(x => new MChapter
                    {
                        ChapterId = x.ChapterId,
                        ChapterName = x.ChapterName,
                        ContentsType = x.ContentsType,
                        CourseId = x.CourseId,
                        Course = x.Course,
                        OrderNo = x.OrderNo,
                        DeletedFlg = x.DeletedFlg,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy   = x.UpdatedBy,
                        CreatedAt   = x.CreatedAt,
                        CreatedBy   = x.CreatedBy
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ChapterService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result?? [];
        }

        /// <inheritdoc/>
        [HttpPost]
        [Authorize]
        public async Task<MChapter> GetNextChapter(Guid chapterId)
        {
            MChapter? findChapter = null;

            try
            {
                // 現在の講座情報を取得
                var currentChapter = await this._context.MChapter
                    .Where(x => x.ChapterId == chapterId)
                    .FirstOrDefaultAsync();

                if (currentChapter != null)
                {
                    // 次の講座を取得
                    var nextChapter = await this._context.MChapter
                        .Where(x => x.CourseId == currentChapter.CourseId)
                        .Where(x => x.Course != null)
                        .Where(x => x.OrderNo == (currentChapter.OrderNo + 1))
                        .Select(x => new MChapter
                        {
                            ChapterId = x.ChapterId,
                            ChapterName = x.ChapterName,
                            ContentsType = x.ContentsType,
                            CourseId = x.CourseId,
                            Course = x.Course,
                            OrderNo = x.OrderNo,
                            DeletedFlg = x.DeletedFlg,
                            UpdatedAt = x.UpdatedAt,
                            UpdatedBy = x.UpdatedBy,
                            CreatedAt = x.CreatedAt,
                            CreatedBy = x.CreatedBy,
                        })
                        .FirstOrDefaultAsync();

                    if (nextChapter != null)
                    {
                        if (nextChapter.ContentsType == ConstService.SystemCode.SYSCODE_CON_TEST)
                        {
                            // テストコンテンツの場合
                            findChapter = nextChapter;
                        }
                        else
                        {
                            // 動画コンテンツの場合
                            if (!nextChapter.DeletedFlg &&              // 講座:有効、かつ
                                IsCoursePublish(nextChapter.Course!))   // コース:有効 
                            {
                                findChapter = nextChapter;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ChapterService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return findChapter?? new();
        }

        /// <inheritdoc/>
        [HttpPost]
        [Authorize]
        public async Task<MChapter> GetPrevChapter(Guid chapterId)
        {
            MChapter? findChapter = null;

            try
            {
                // 現在の講座情報を取得
                var currentChapter = await this._context.MChapter
                    .Where(x => x.ChapterId == chapterId)
                    .FirstOrDefaultAsync();

                if (currentChapter != null)
                {
                    // 前の講座を取得
                    var prevChapter = await this._context.MChapter
                        .Where(x => x.CourseId == currentChapter.CourseId)
                        .Where(x => x.Course != null)
                        .Where(x => x.OrderNo == (byte)(currentChapter.OrderNo - 1))
                        .Select(x => new MChapter
                        {
                            ChapterId = x.ChapterId,
                            ChapterName = x.ChapterName,
                            ContentsType = x.ContentsType,
                            CourseId = x.CourseId,
                            Course = x.Course,
                            OrderNo = x.OrderNo,
                            DeletedFlg = x.DeletedFlg,
                            UpdatedAt = x.UpdatedAt,
                            UpdatedBy = x.UpdatedBy,
                            CreatedAt = x.CreatedAt,
                            CreatedBy = x.CreatedBy,
                        })
                        .FirstOrDefaultAsync();

                    if (prevChapter != null)
                    {
                        if (prevChapter.ContentsType == ConstService.SystemCode.SYSCODE_CON_TEST)
                        {
                            // テストコンテンツの場合
                            findChapter = prevChapter;
                        }
                        else
                        {
                            // 動画コンテンツの場合
                            if (!prevChapter.DeletedFlg &&                // 講座:有効、かつ
                                IsCoursePublish(prevChapter.Course!))     // コース:有効 
                            {
                                findChapter = prevChapter;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ChapterService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return findChapter?? new();
        }

        /// <summary>
        /// コースが有効かチェックする
        /// </summary>
        /// <param name="course">ko-su情報</param>
        /// <returns></returns>
        private bool IsCoursePublish(MCourse course)
        {
            return (!course.DeletedFlg &&                                            // コース:有効
                    (((course.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_PERIOD) &&            
                     ((course.BegineDateTime <= DateTime.Now) && (course.EndDateTime >= DateTime.Now))) ||
                                                                                    // コース:公開期間内
                    ((course.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_FLAG && course.PublicFlg)))
                                                                                    // 公開フラグ:ON
            );
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
    }
}
