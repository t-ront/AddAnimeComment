namespace AddAnimeComment
{
    /// <summary>
    /// 楽曲情報クラス
    /// </summary>
    internal sealed class SongInfo
    {
        /// <summary>
        /// 番組ID
        /// </summary>
        internal string ProgramId { get; set; }
        /// <summary>
        /// 番組分類
        /// </summary>
        internal string ProgramType { get; set; }
        /// <summary>
        /// 番組名
        /// </summary>
        internal string ProgramName { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        internal string Type { get; set; }
        /// <summary>
        /// 放映順
        /// </summary>
        internal string TypeNum { get; set; }
        /// <summary>
        /// 楽曲ID
        /// </summary>
        internal string Id { get; set; }
        /// <summary>
        /// 楽曲名
        /// </summary>
        internal string Title { get; set; }
        /// <summary>
        /// 歌手名
        /// </summary>
        internal List<string> Artists { get; set; }
    }
}
