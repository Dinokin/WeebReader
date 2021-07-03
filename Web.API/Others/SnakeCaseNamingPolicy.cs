using System.Text;
using System.Text.Json;

namespace WeebReader.Web.API.Others
{
    internal class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        private const char Underscore = '_';
        private const char Space = ' ';

        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < name.Length; i++)
            {
                if (name[i] == Space || i == 0 && name[i] == Underscore)
                    continue;

                if (char.IsUpper(name[i]) && i > 0 && (name[i - 1] != Underscore || !char.IsUpper(name[i - 1])))
                    stringBuilder.Append(Underscore);

                stringBuilder.Append(char.ToLowerInvariant(name[i]));
            }

            return stringBuilder.ToString();
        }
    }
}