namespace ElsWebApp.Services.IService
{
    public interface IBaseEntity<T> : IDisposable
    {
        /// <summary>
        /// idに該当するレコード<T>を取得する
        /// </summary>
        /// <param name="id">識別子</param>
        /// <returns></returns>
        public Task<T> SelectById(string id);

        /// <summary>
        /// レコード<T>を登録する
        /// </summary>
        /// <param name="id">識別子</param>
        /// <returns></returns>
        public Task<int> Insert(T data);

        // public int Delete(string id);

        /// <summary>
        /// レコード<T>を更新する
        /// </summary>
        /// <param name="id">識別子</param>
        /// <returns></returns>
        public Task<int> Update(T data);
    }
}
