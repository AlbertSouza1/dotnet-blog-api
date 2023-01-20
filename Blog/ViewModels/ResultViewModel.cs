using System.Collections.Generic;

namespace Blog.ViewModels
{
    public class ResultViewModel<T>
    {
        public ResultViewModel(T data) => Data = data;

        public ResultViewModel(string error) => Errors.Add(error);

        public ResultViewModel(List<string> errors) => Errors.AddRange(errors);

        public ResultViewModel(T data, string error)
        {
            Data = data;
            Errors.Add(error);
        }

        public T Data { get; private set; }
        public List<string> Errors { get; private set; } = new();
    }
}