namespace LanguageService.Shared
{
    public struct Range
    {
        public Range(int startFrom, int endAt)
        {
            this.Start = startFrom;
            this.End = endAt;
        }

        internal int Start { get; }
        internal int End { get; }
    }
}
