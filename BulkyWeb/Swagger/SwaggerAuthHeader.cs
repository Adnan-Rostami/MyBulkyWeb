//using Microsoft.OpenApi;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using Microsoft.OpenApi.Models;

//namespace BulkyWeb.Swagger
//{
//    public class SwaggerAuthHeader : IOperationFilter
//    {
//        public void Apply(OpenApiOperation operation, OperationFilterContext context)
//        {
//            operation.Parameters ??= new List<OpenApiParameter>();

//            operation.Parameters.Add(new OpenApiParameter
//            {
//                Name = "Authorization",
//                In = ParameterLocation.Header,
//                Description = "Enter 'Bearer {token}'",
//                Required = false,
//                Schema = new OpenApiSchema
//                {
//                    Type = "string"
//                }
//            });
//        }
//    }