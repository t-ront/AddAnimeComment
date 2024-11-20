using Microsoft.VisualBasic.FileIO;
using System.Reflection;

namespace AddAnimeComment
{
    /// <summary>
    /// 楽曲情報リストクラス
    /// </summary>
    internal sealed class SongInfoList
    {
        /// <summary>
        /// 楽曲情報リスト
        /// </summary>
        private List<SongInfo> songInfoList { get; set; }

        private readonly int _songInfoColumnCount;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal SongInfoList()
        {
            songInfoList = [];
            _songInfoColumnCount = CountSongInfoClassColumn();
        }

        /// <summary>
        /// CSVを読み込んでリストを追加する
        /// </summary>
        /// <param name="csvPath"></param>
        /// <returns></returns>
        internal void AddFromCsv(string csvPath)
        {
            var normalizer = new Normallizer();

            using (TextFieldParser txtParser = new(csvPath))
            {
                txtParser.SetDelimiters(",");
                while (!txtParser.EndOfData)
                {
                    var values = txtParser.ReadFields();
                    if (values == null)
                    {
                        continue;
                    }
                    if (values.Length != _songInfoColumnCount)
                    {
                        Console.WriteLine(_songInfoColumnCount);
                        Console.WriteLine($"Error! csvPath:{csvPath} linenumber:{txtParser.LineNumber}");
                        continue;
                    }

                    var songInfo = new SongInfo()
                    {
                        ProgramId = values[0],
                        ProgramType = SetProgramType(values[1]),
                        ProgramName = values[2],
                        Type = values[3],
                        TypeNum = values[4],
                        Id = values[5],
                        Title = normalizer.ToComparisonString(values[6]),
                        Artists = normalizer.ToComparisonArtistList(values[7])
                    };

                    songInfoList.Add(songInfo);
                }
            }
        }

        /// <summary>
        /// リスト内で曲名とアーティスト名が一致する情報を返す
        /// </summary>
        /// <param name="title"></param>
        /// <param name="artist"></param>
        /// <returns></returns>
        internal List<SongInfo> FilterByTitle(string title)
        {
            return songInfoList.Where(x => x.Title == title)
                               .ToList();
        }

        /// <summary>
        /// 楽曲情報クラスのフィールド数をカウントする
        /// </summary>
        /// <returns></returns>
        private int CountSongInfoClassColumn()
        {
            return typeof(SongInfo).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                                   .Length;
        }

        /// <summary>
        /// 番組分類を指定の書式に変換する
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string SetProgramType(string input)
        {
            return input switch
            {
                "TV" => "TVA",
                "TS" => "TVA",
                "VD" => "OVA",
                "MV" => "MVA",
                "GM" => "GM",
                "SF" => "SF",
                "SS" => "SS",
                "SV" => "SV",
                "SM" => "SM",
                "KK" => "KK",
                "DR" => "DR",
                "JD" => "JD",
                "RD" => "RD",
                "GE" => "GE",
                _ => input
            };
        }
    }
}
