namespace Query
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.OpenApi.Models;
    using Microsoft.OpenApi.Writers;

    internal class OpenApiFormatter : TextOutputFormatter
    {
        private readonly Type writer;

        internal OpenApiFormatter(string mediaType, Type writer)
        {
            this.SupportedMediaTypes.Add(mediaType);
            this.SupportedEncodings.Add(Encoding.UTF8);

            this.writer = writer;
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(OpenApiDocument).IsAssignableFrom(type);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding encoding)
        {
            return new TaskFactory().StartNew(() =>
            {
                var document = context.Object as OpenApiDocument;
                using (var writer = context.WriterFactory(context.HttpContext.Response.Body, encoding))
                {
                    document.SerializeAsV3(Activator.CreateInstance(this.writer, writer) as IOpenApiWriter);
                }
            });
        }
    }
}
