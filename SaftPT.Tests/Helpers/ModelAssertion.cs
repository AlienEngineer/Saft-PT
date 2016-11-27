using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SaftPT.Tests.Helpers
{
    /// <summary>
    /// Assertion model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelAssertion<T>
    {
        private readonly T _subject;

        public ModelAssertion(T subject)
        {
            _subject = subject;
        }

        public void Field(string fieldName, IResolveConstraint constraint)
        {
            AssertPrivate.That(_subject, fieldName, constraint);
        }
        public void Field(Func<T, object> propGetter, IResolveConstraint constraint)
        {
            Assert.That(propGetter(_subject), constraint);
        }

    }
}