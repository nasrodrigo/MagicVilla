using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string>? ErrorMessage { get; set; }
        public object? Result { get; set; }

        public ApiResponse NotFoundResponse(string message)
        {
            this.ErrorMessage = new List<string> {
                message
                    };
            this.StatusCode = HttpStatusCode.NotFound;
            this.IsSuccess = false;
            return this;
        }

        public ApiResponse BadRequestResponse(string message)
        {
            this.ErrorMessage = new List<string> {
                message
                    };
            this.StatusCode = HttpStatusCode.BadRequest;
            this.IsSuccess = false;
            return this;
        }

        public ApiResponse InternalServerErrorResponse(List<string> message)
        {
            this.ErrorMessage = message;
            this.StatusCode = HttpStatusCode.InternalServerError;
            this.IsSuccess = false;
            return this;
        }

    }

}
