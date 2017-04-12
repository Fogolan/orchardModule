namespace SimpleCommerce.Helpers
{
    public static class StringExtensions
    {
        public static string TrimSafe(this string s) {
            return s?.Trim() ?? string.Empty;
        }
    }
}