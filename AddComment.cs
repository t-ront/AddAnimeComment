using log4net;
using System.Configuration;

namespace AddAnimeComment
{
    internal sealed class AddComment
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 非同期処理時ロックオブジェクト
        /// </summary>
        private readonly object _lockObj = new();

        private readonly string _currentDirectory = Path.GetDirectoryName(Environment.ProcessPath);
        private readonly string _csvFileDirectory = "csv";

        private readonly List<string> _csvList =
        [
            "anison.csv",
            "sf.csv",
            "game.csv"
        ];
        private readonly string _targetMusicDirectory = ConfigurationManager.AppSettings["TargetDirectory"];
        private readonly string[] _targetMusicExtension =
        [
            "*.mp3",
            "*.m4a"
        ];


        /// <summary>
        /// プログラム開始
        /// </summary>
        internal void Execute()
        {
            if (!Directory.Exists(Path.Combine(_currentDirectory, "log")))
            {
                Directory.CreateDirectory(Path.Combine(_currentDirectory, "log"));
            }
            _logger.Info("==========================");
            _logger.Info("=========処理開始=========");
            _logger.Info("==========================");


            Console.WriteLine("Step1 CSVの読み込み 開始");
            var songInfoList = new SongInfoList();
            foreach (var csv in _csvList)
            {
                var csvPath = Path.Combine(_currentDirectory, _csvFileDirectory, csv);
                if (!File.Exists(csvPath))
                {
                    _logger.Error($"csvファイルが見つかりませんでした path:{csvPath}");
                    return;
                }
                songInfoList.AddFromCsv(csvPath);
            }
            Console.WriteLine($"Step1 CSVの読み込み 完了{Environment.NewLine}");


            Console.WriteLine("Step2 音楽ファイルの読み込み 開始");
            if (!Directory.Exists(_targetMusicDirectory))
            {
                _logger.Error($"音楽フォルダが見つかりませんでした path:{_targetMusicDirectory}");
                return;
            }
            var targetMusicList = GetMusicList(_targetMusicDirectory);
            Console.WriteLine($"Step2 音楽ファイルの読み込み 完了{Environment.NewLine}");


            Console.WriteLine("Step3 コメント入力処理 開始");
            var prg = new ProgressBar(targetMusicList.Count());

            var normalizer = new Normallizer();
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = int.TryParse(ConfigurationManager.AppSettings["ParaMax"], out var num) ? num : 1,
            };
            Parallel.ForEach(targetMusicList, parallelOptions, (targetMusic) =>
            {
                lock (_lockObj)
                {
                    prg.Update($"『{Path.GetFileName(targetMusic.FullName)}』のデータを処理中...");
                }

                using (var musicTag = TagLib.File.Create(targetMusic.FullName))
                {
                    var musicComment = musicTag.Tag.Comment;
                    if (musicComment != null)
                    {
                        _logger.Info($"すでにコメント入力済みのためスキップ ファイル:{targetMusic.FullName} コメント:{musicComment}");

                        return;
                    }

                    var musicArtist = musicTag.Tag.Performers?.FirstOrDefault();
                    if (musicArtist == null)
                    {
                        return;
                    }

                    var musicTitle = musicTag.Tag.Title;
                    if (musicTitle == null)
                    {
                        return;
                    }

                    var normalizedMusicTitle = normalizer.ToComparisonString(musicTitle);
                    var titleMatchMusicList = songInfoList.FilterByTitle(normalizedMusicTitle);
                    if (titleMatchMusicList.Count == 0)
                    {
                        return;
                    }

                    var normalizedMusicArtist = normalizer.ToComparisonString(musicArtist);
                    var newCommentList = new HashSet<string>();
                    foreach (var titleMatchMusic in titleMatchMusicList)
                    {
                        foreach (var artist in titleMatchMusic.Artists)
                        {
                            if (artist == normalizedMusicArtist)
                            {
                                newCommentList.Add(GenerateAnimeComment(titleMatchMusic));
                            }
                        }
                    }

                    if (newCommentList.Count > 0)
                    {
                        var newComment = string.Join(", ", newCommentList);
                        _logger.Info($"マッチする曲が見つかりました! {musicTitle} / {musicArtist} => {newComment}");
                        musicTag.Tag.Comment = newComment;
                        musicTag.Save();
                    }
                }
            });

            prg.Done("コメント書き込み完了");
            Console.WriteLine($"Step3 コメント入力処理 完了{Environment.NewLine}");

            _logger.Info("==========================");
            _logger.Info("=========処理終了=========");
            _logger.Info("==========================");


            Console.WriteLine("ダウンロード完了!");
            Console.ReadLine();
        }


        /// <summary>
        /// 指定ディレクトリに存在する音楽ファイルのリストを取得する
        /// </summary>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        private IEnumerable<FileInfo> GetMusicList(string targetDirectory)
        {
            return _targetMusicExtension.SelectMany(filter => new DirectoryInfo(targetDirectory).EnumerateFiles(filter, System.IO.SearchOption.AllDirectories));
        }

        /// <summary>
        /// 検索結果からコメントを生成する
        /// </summary>
        /// <param name="songInfo"></param>
        /// <returns></returns>
        private string GenerateAnimeComment(SongInfo songInfo)
        {
            return songInfo.TypeNum == string.Empty
                ? $"{songInfo.ProgramType} {songInfo.ProgramName} {songInfo.Type}"
                : int.TryParse(songInfo.TypeNum, out var num)
                    ? $"{songInfo.ProgramType} {songInfo.ProgramName} {songInfo.Type}{num}"
                    : $"{songInfo.ProgramType} {songInfo.ProgramName} {songInfo.Type} {songInfo.TypeNum}";
        }
    }
}


