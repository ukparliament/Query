namespace Query
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class Resources
    {
        public static JObject OpenApiDocument
        {
            get
            {
                using (var stream = Resources.GetStream("Search.Resources.OpenApiDocumentTemplate.json"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var jsonReader = new JsonTextReader(reader))
                        {
                            dynamic document = JObject.Load(jsonReader);
                            //document.components.responses.searchResponse.content = new JObject(Configuration.Mappings.Select(m => new JProperty(m.MediaType, new JObject())));
                            //document.paths["/query.{extension}"].get.parameters[0].schema["enum"] = new JArray(Configuration.Mappings.Select(m => m.Extension));

                            return document;
                        }
                    }
                }
            }
        }

        public static Stream GetStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
    }
}
