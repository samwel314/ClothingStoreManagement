namespace ClothingStoreManagement.Application.ResultHelpers
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public ErrorType ErrorType { get; private set; }
        private Result(bool isSuccess, T? value, string? error, ErrorType errorType = ErrorType.Success)
        {
            IsSuccess = isSuccess;
            Data = value;
            ErrorMessage = error;
            ErrorType = errorType;
        }

        public static Result<T> Success(T value = default!) => new(true, value, string.Empty);
        public static Result<T> Failure(string error, ErrorType errorType) => new(false, default, error, errorType);

    }
    public enum ErrorType
    {
        Success,
        notFound,
        conflict,
        validation
    }
}
