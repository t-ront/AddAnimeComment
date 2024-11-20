using System.Text;
using static System.Console;

namespace AddAnimeComment
{
    internal sealed class ProgressBar(int parMax)
    {
        /// <summary>
        /// 最大桁数
        /// </summary>
        internal int _columns = WindowWidth;

        /// <summary>
        /// プログレスバーの長さ
        /// </summary>
        internal int _width = 50;

        /// <summary>
        /// 進捗度
        /// </summary>
        internal int _compCnt = 0;

        /// <summary>
        /// 目標進捗度
        /// </summary>
        internal int _allCnt = parMax;

        /// <summary>
        /// 最後に出力したカーソルの行
        /// </summary>
        internal int _firstRow = CursorTop;

        /// <summary>
        /// プログレスバーを更新する
        /// </summary>
        /// <param name="message"></param>
        internal void Update(string message)
        {
            var compPer = (float)_compCnt / _allCnt;
            var widthNow = (int)Math.Floor(_width * compPer);

            var gauge = new string('#', widthNow) + new string(' ', _width - widthNow);
            var status = $"({compPer * 100:f1}% [{_compCnt}/{_allCnt}])";

            ClearScreenDown();
            Console.WriteLine($"[{gauge}] {status}");
            Console.WriteLine(message);

            _columns = WindowWidth;
            _compCnt++;
        }

        /// <summary>
        /// プログレスバーを完了表示にする
        /// </summary>
        /// <param name="doneAlert"></param>
        internal void Done(string doneAlert)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encShiftJis = Encoding.GetEncoding("Shift_JIS");
            var cnt1 = encShiftJis.GetByteCount(doneAlert);

            var sideLen = (int)Math.Floor((float)(_width - cnt1) / 2);

            var gauge = new string('=', sideLen) + doneAlert + new string('=', _width - sideLen - cnt1);
            var status = $"(100% [{_allCnt}/{_allCnt}])";

            ClearScreenDown();
            Console.WriteLine($"[{gauge}] {status}");
        }

        /// <summary>
        /// コンソール表示を初期化する
        /// </summary>
        private void ClearScreenDown()
        {
            SetCursorPosition(0, _firstRow);
            Console.Write(new string(' ', _columns * 5));
            SetCursorPosition(0, _firstRow);
        }
    }
}