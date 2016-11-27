using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SaftPT.Tests.Helpers
{
    /// <summary>
    /// Assertion helper for complex types
    /// <code>
    /// AssertComplex.That(subject,
    ///     has => has.Field("_data", Is.EqualTo(23123)),
    ///     has => has.Field("_data1", Is.EqualTo("test"))
    /// );
    /// </code>
    /// </summary>
    public static class AssertComplex
    {
        public class AggregateException : AssertionException
        {
            private readonly ICollection<Exception> _exceptions;

            public AggregateException() : base(string.Empty)
            {
                _exceptions = new List<Exception>();
            }

            public bool HasInnerExceptions => _exceptions.Any();

            public override string ToString()
            {
                return string.Join("\n", _exceptions.Select(e => e.Message));
            }

            public override string Message => ToString();

            public void AddException(Exception exception)
            {
                _exceptions.Add(exception);
            }
        }

        public static void That<T>(T subject, params Action<ModelAssertion<T>>[] assertions)
        {
            var exception = new AggregateException();
            foreach (var assertion in assertions)
            {
                try
                {
                    assertion(new ModelAssertion<T>(subject));
                }
                catch (AssertionException ex)
                {
                    exception.AddException(ex);
                }
            }

            if (exception.HasInnerExceptions)
            {
                throw exception;
            }
        }
    }
}