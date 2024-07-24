using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace GGS.Models
{
    public class Result
    {
        public string Message { get; set; } = "OK";
        public int Status { get; set; } = 200;
        public object? Data { get; set; } = null;

        public static IActionResult Create(HttpResponse response, int status, string message, object? data)
        {
            response.StatusCode = status;

            var result = new Result()
            {
                Status = status,
                Message = message,
                Data = data
            };

            var settings = new JsonSerializerOptions();
            settings.ReferenceHandler = ReferenceHandler.IgnoreCycles;

            return new JsonResult(result, settings);
        }

        public static IActionResult FromCreated(HttpResponse response) => Create(response, 201, "OK", null);
        public static IActionResult FromData(HttpResponse response, object data) => Create(response, 200, "OK", data);
        public static IActionResult FromException(HttpResponse response, Exception exception) => Create(response, 500, exception.Message, null);
        public static IActionResult FromOk(HttpResponse response) => Create(response, 200, "OK", null);
    }
}
