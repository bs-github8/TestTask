namespace TestTask
{
    public static class Extension
    {
        public static bool IsVowel(this LetterStats ls)
        {
            const string vowels = "аеёиоуыэюяАЕЁИОУЫЭЮЯaeiouyAEIOUY";
            return vowels.IndexOf(ls.Letter[0]) != -1;
        }
    }
}
