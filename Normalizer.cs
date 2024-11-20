using System.Text;
using System.Text.RegularExpressions;
using Umayadia.Kana;

namespace AddAnimeComment
{
    internal sealed class Normallizer
    {
        /// <summary>
        /// アーティスト名を比較用の文字列リストに変換する
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal List<string> ToComparisonArtistList(string input)
        {
            var resultList = new List<string>();

            var halfWidthBlacketsInput = ToHalfWidthBrackets(input);
            var splitByBracketsInputs = SplitByBrackets(halfWidthBlacketsInput);
            foreach (var splitByBracketsInput in splitByBracketsInputs)
            {
                if (splitByBracketsInput == null || splitByBracketsInput == string.Empty)
                {
                    continue;
                }
                var removeBlackInput = RemoveBlank(splitByBracketsInput);
                resultList.Add(ToComparisonString(removeBlackInput));
            }

            return resultList;
        }

        /// <summary>
        /// 全角のカッコを半角のカッコに変換する
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ToHalfWidthBrackets(string input)
        {
            return input.Replace("（", "(")
                        .Replace("）", ")");
        }

        /// <summary>
        /// 半角カッコで文字列を分割する
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private List<string> SplitByBrackets(string input)
        {
            return [.. input.Split('(')];
        }

        /// <summary>
        /// 空白を削除する
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoveBlank(string input)
        {
            return input.Replace(" ", "")
                        .Replace("　", "");
        }

        /// <summary>
        /// 比較用の文字列に変換する
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal string ToComparisonString(string input)
        {
            var unicodeNormalizeInput = ToUnicodeNormalizeString(input);
            var removedNeedlessCharactersInput = ToRemovedNeedlessCharactersString(unicodeNormalizeInput);
            var lowerInput = removedNeedlessCharactersInput.ToLower();
            var removeBlankInput = RemoveBlank(lowerInput);
            return ToHalfWidth(removeBlankInput);
        }

        /// <summary>
        /// Unicode正規化した文字列に変換する
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ToUnicodeNormalizeString(string input)
        {
            return input.Normalize(NormalizationForm.FormKC);
        }

        /// <summary>
        /// 英語、日本語の漢字・カタカナ・ひらがな以外を除去した文字列に変換する
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ToRemovedNeedlessCharactersString(string input)
        {
            var pattern = @"[^\p{IsBasicLatin}\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}\p{Nd}]";
            return Regex.Replace(input, pattern, "");
        }

        /// <summary>
        /// 半角に変換する
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ToHalfWidth(string input)
        {
            return KanaConverter.ToNarrow(input);
        }
    }
}
