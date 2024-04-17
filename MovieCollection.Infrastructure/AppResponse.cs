using System.Collections.Generic;

namespace MovieCollection.Infrastructure
{
    public class AppResponse
    {
        private readonly static AppResponse success = new AppResponse();

        public bool IsSucceed { get; protected set; } = true;
        public Dictionary<string, string> Messages { get; protected set; } = new Dictionary<string, string>();

        public static AppResponse Success() => success;
        public static AppResponse Success(string key, string value)
        {
            var result = success;
            result.Messages.Add(key, value);
            return result;
        }
        public static AppResponse Success(Dictionary<string, string> message)
        {
            var result = success;
            result.Messages = message;
            return result;
        }

        public static AppResponse Error(string key, string value)
        {
            var result = new AppResponse() { IsSucceed = false };
            result.Messages.Add(key, value);
            return result;
        }
        public static AppResponse Error(Dictionary<string, string> message)
        {
            var result = new AppResponse() { IsSucceed = false };
            result.Messages = message;
            return result;
        }
    }

    public class AppResponse<T> : AppResponse
    {
        private readonly static AppResponse<T> success = new AppResponse<T>();
        public T Data { get; private set; }

        public static AppResponse<T> Success(T data)
        {
            var result = success;
            result.Data = data;
            return result;
        }
        public static AppResponse<T> Success(T data, string key, string value)
        {
            var result = success;
            result.Data = data;
            result.Messages.Add(key, value);
            return result;
        }
        public static AppResponse<T> Success(T data, Dictionary<string, string> message)
        {
            var result = success;
            result.Data = data;
            result.Messages = message;
            return result;
        }

        public new static AppResponse<T> Error(string key, string value)
        {
            var result = new AppResponse<T>() { IsSucceed = false };
            result.Messages.Add(key, value);
            return result;
        }
        public new static AppResponse<T> Error(Dictionary<string, string> message)
        {
            var result = new AppResponse<T>() { IsSucceed = false };
            result.Messages = message;
            return result;
        }
    }
}
