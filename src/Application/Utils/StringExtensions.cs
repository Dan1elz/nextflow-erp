namespace dotnet_api_erp.src.Application.Utils
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            string.IsNullOrEmpty(input) ? input : char.ToUpper(input[0]) + input.Substring(1);
    }

}