using ElsWebApp.Data;
using ElsWebApp.Models.Entitiy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ElsWebApp.Controllers
{
    [ApiController]
    [Route("Bookmark")]
    public class BookmarkController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookmarkController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Save")]
        public IActionResult Save(
            [FromForm] string userId,
            [FromForm] string courseId,
            [FromForm] string chapterId,
            [FromForm] int bookmarkSeconds)
        {
            try
            {
                var userGuid = Guid.Parse(userId);
                var courseGuid = Guid.Parse(courseId);
                var chapterGuid = Guid.Parse(chapterId);

                var record = _context.UserChapters
                    .FirstOrDefault(x => x.UserId == userGuid && x.CourseId == courseGuid && x.ChapterId == chapterGuid);

                if (record == null)
                {
                    record = new UserChapter
                    {
                        UserChapterId = Guid.NewGuid(),
                        UserId = userGuid,
                        CourseId = courseGuid,
                        ChapterId = chapterGuid,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.UserChapters.Add(record);
                }

                record.BookmarkSeconds = bookmarkSeconds;
                record.UpdatedAt = DateTime.Now;

                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "保存に失敗しました", details = ex.Message });
            }
        }

        [HttpGet("Get")]
        public IActionResult Get(string userId, string courseId, string chapterId)
        {
            var userGuid = Guid.Parse(userId);
            var courseGuid = Guid.Parse(courseId);
            var chapterGuid = Guid.Parse(chapterId);

            var record = _context.UserChapters
                .FirstOrDefault(x => x.UserId == userGuid && x.CourseId == courseGuid && x.ChapterId == chapterGuid);

            if (record?.BookmarkSeconds != null && record.UpdatedAt != null)
            {
                var expired = DateTime.Now - record.UpdatedAt.Value;
                if (expired.TotalHours > 24)
                {
                    // しおりを無効にする
                    record.BookmarkSeconds = null;
                    record.UpdatedAt = DateTime.Now;
                    _context.SaveChanges();
                    return Ok(0);
                }
            }

            return Ok(record?.BookmarkSeconds ?? 0);
        }


        [HttpPost("Clear")]
        public IActionResult Clear(
            [FromForm] string userId,
            [FromForm] string courseId,
            [FromForm] string chapterId)
        {
            try
            {
                var userGuid = Guid.Parse(userId);
                var courseGuid = Guid.Parse(courseId);
                var chapterGuid = Guid.Parse(chapterId);

                var record = _context.UserChapters
                    .FirstOrDefault(x => x.UserId == userGuid && x.CourseId == courseGuid && x.ChapterId == chapterGuid);

                if (record != null)
                {
                    record.BookmarkSeconds = null;
                    record.UpdatedAt = DateTime.Now;
                    _context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "クリアに失敗しました", details = ex.Message });
            }
        }
    }
}
