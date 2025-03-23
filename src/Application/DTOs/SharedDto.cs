﻿namespace dotnet_api_erp.src.Application.DTOs
{
    public class SharedDto
    {
        public class ApiResponse<T>
        {
            public int Status { get; set; }
            public required string Message { get; set; }
            public required T Data { get; set; }
        }

        public class ApiResponseMessage
        {
            public int Status { get; set; }
            public required string Message { get; set; }
            public List<string>? Errors { get; set; }
        }

        public class ApiResponseTable<T>
        {
            public int TotalItems { get; set; }
            public required List<T> Data { get; set; }

            internal object Select(Func<object, object> value)
            {
                throw new NotImplementedException();
            }
        }

        public class OptionDto
        {
            public required string Value { get; set; }
            public required string Label { get; set; }
        }

        public class ListIdsGuidDto
        {
            public required List<Guid>? Ids { get; set; }
        }

        public class ListIdsIntDto
        {
            public required List<int>? Ids { get; set; }
        }
    }

    public interface IReadOnlyList
    {
    }
}
