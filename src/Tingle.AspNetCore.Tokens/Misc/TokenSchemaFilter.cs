using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Tokens.Misc
{
    /// <summary>
    /// Modifies the OpenAPI schema for types implementing <see cref="IToken"/> (special purpose tokens) such as:
    /// <list type="number">
    /// <item><see cref="ContinuationToken{T}"/></item> and
    /// <item><see cref="TimedContinuationToken{T}"/></item>
    /// </list>
    /// </summary>
    internal class TokenSchemaFilter : ISchemaFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Type = "string";

            // this format in unofficial and is meant to convince the reader that the value here is not human-readable
            schema.Format = "opaque";
        }
    }
}
