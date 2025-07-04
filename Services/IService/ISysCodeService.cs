using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface ISysCodeService : IBaseEntity<MSysCode>
    {
        /// <summary>
        /// 指定されたクラスIDに該当する、クラスCDとクラス名の
        /// ペアリストを作成する。
        /// </summary>
        /// <param name="classId">クラス識別子</param>
        /// <returns></returns>
        public Task<List<CodeValuePair>> GetClassCodeList(string classId);
    }
}
