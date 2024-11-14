using Newtonsoft.Json.Linq;

namespace Cdp.Mapping.Model
{
    public class JTokenTransformResult
    {
        public JToken Token { get; }

        public bool IsOk { get; }
        public string Error { get; }
        public object Tags { get; }

        public JTokenTransformResult(JToken token, bool isOk, string error, object tags)
        {
            Token = token;
            IsOk = isOk;
            Error = error;
            Tags = tags;
        }

        public static JTokenTransformResult Fail(string error, object tags) => new (null, false, error, tags);

        public static JTokenTransformResult Ok(JToken token) => new (token, true, null, null);
    }
}