using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class MovieContentsService(ElsWebAppDbContext ctx, ILogger<MovieContentsService> logger) : IMovieContentsService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<MovieContentsService> _logger = logger;

        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        /// <inheritdoc/>
        public async Task<MovieContents> SelectById(string id)
        {
            MovieContents mMovie = new();
            try
            {
                mMovie = await this._context.MovieContents
                    .Where(x => x.ContentsId == Guid.Parse(id))
                    .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return mMovie;
        }

        /// <inheritdoc/>
        public async Task<int> Insert(MovieContents data)
        {
            var result = 0;
            try
            {
                await this._context.MovieContents.AddAsync(data);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        public async Task<int> Update(MovieContents data)
        {
            var result = 0;
            var mMovie = await this.SelectById(data.ContentsId.ToString());

            if (mMovie.ContentsId != Guid.Empty)
            {
                mMovie.ChapterId = data.ChapterId;
                mMovie.ContentsName = data.ContentsName;
                mMovie.ContentsPath = data.ContentsPath;
                mMovie.PlaybackTime = data.PlaybackTime;
                mMovie.DeletedFlg = data.DeletedFlg;
                mMovie.UpdatedBy = data.UpdatedBy;

                try
                {
                    result = await this._context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    CriticalError(ex);
                }
            }

            return result;
        }

        public async Task<MovieContents> SelectByChapterId(Guid chapterId)
        {
            MovieContents? mMovie = null;
            try
            {
                mMovie = await this._context.MovieContents
                    .Where(x => x.ChapterId == chapterId)
                    .Where(x => !x.DeletedFlg)
                    .Select(x => new MovieContents
                    {
                        ContentsId = x.ContentsId,   
                        ChapterId = x.ChapterId,
                        Chapter = x.Chapter,
                        ContentsName = x.ContentsName,
                        ContentsPath = x.ContentsPath,
                        PlaybackTime = x.PlaybackTime,
                        DeletedFlg = x.DeletedFlg,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy =x.UpdatedBy,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return mMovie?? new();
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
