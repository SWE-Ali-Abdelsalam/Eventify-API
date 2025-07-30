namespace Eventify.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string Error { get; private set; } = string.Empty;
        public string[] Errors { get; private set; } = Array.Empty<string>();

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        protected Result(bool isSuccess, string[] errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);
        public static Result Failure(string[] errors) => new(false, errors);
    }

    public class Result<T> : Result
    {
        public T? Value { get; private set; }

        private Result(bool isSuccess, T? value, string error) : base(isSuccess, error)
        {
            Value = value;
        }

        private Result(bool isSuccess, T? value, string[] errors) : base(isSuccess, errors)
        {
            Value = value;
        }

        public static Result<T> Success(T? value) => new(true, value, string.Empty);
        public new static Result<T> Failure(string error) => new(false, default, error);
        public new static Result<T> Failure(string[] errors) => new(false, default, errors);
    }
}
