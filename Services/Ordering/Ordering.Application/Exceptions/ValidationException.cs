using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public Dictionary<string, string[]> Errors { get; }
        public ValidationException() : base("One or more validation error(s) occurred")
        {
            Errors = new Dictionary<string, string[]>();
        }
        public ValidationException(IEnumerable<ValidationFailure> failure) : this()
        {
            Errors = failure
                .GroupBy(x => x.PropertyName, e => e.ErrorMessage)
                .ToDictionary(fail => fail.Key, fail => fail.ToArray());
        }
    }
}
