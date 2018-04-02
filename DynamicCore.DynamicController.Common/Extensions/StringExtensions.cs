namespace DynamicCore.DynamicController.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase( this string originalString )
        {
            if( string.IsNullOrEmpty( originalString ) == true )
            {
                return string.Empty;
            }

            string initial = originalString.Substring(0, 1).ToLower();

            return initial + originalString.Substring(1);
        }

    }
}
