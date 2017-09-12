namespace Parliament.Data.Api.FixedQuery
{
    using Newtonsoft.Json;
    using System.IO;
    using System.Reflection;

    public static class Resources
    {
        private const string BaseName = "Parliament.Data.Api.FixedQuery";

        public static DB DB
        {
            get
            {
                return JsonConvert.DeserializeObject<DB>(Resources.EndpointsJson);
            }
        }

        public static string EndpointsJson
        {
            get
            {
                return Resources.GetFile($"{BaseName}.Endpoints.json");
            }
        }

        public static string EndpointsSchema
        {
            get
            {
                return Resources.GetFile($"{BaseName}.EndpointsSchema.json");
            }
        }

        public static string GetSparql(string name)
        {
            return Resources.GetFile($"{BaseName}.Sparql.{name}.sparql");
        }

        private static string GetFile(string resourceName)
        {
            using (var sparqlResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(sparqlResourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}