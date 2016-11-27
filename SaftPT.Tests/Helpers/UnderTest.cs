using System;
using System.Linq.Expressions;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;

namespace SaftPT.Tests.Helpers
{
    /// <summary>
    /// Test helpers based on Moq.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UnderTest<T> where T : class
    {
        private IUnderTest<T> _underTest;

        [SetUp]
        public void Setup()
        {
            _underTest = new SubjectUnderTest<T>();
            SetDependencies(_underTest);
            Subject = MakeSubject();
        }

        protected virtual T MakeSubject()
        {
            return _underTest.MakeSubject();
        }

        public T Subject { get; private set; }
        // public Mock<T> SubjectMock => _underTest.SubjectMock;

        protected abstract void SetDependencies(IUnderTest<T> subject);

        protected Mock<TDependency> Dependency<TDependency>() where TDependency : class
        {
            return _underTest.Get<TDependency>();
        }

        //protected void CallBase<TResult>(Expression<Func<T, TResult>> expression)
        //{
        //    _underTest.SubjectMock.Setup(expression).CallBase();
        //}

        protected ISetup<TDependency, TResult> Setup<TDependency, TResult>(Expression<Func<TDependency, TResult>> expression) where TDependency : class
        {
            return Dependency<TDependency>().Setup(expression);
        }

        public void Verify<TDependency>(Expression<Action<TDependency>> expression, Func<Times> times) where TDependency : class
        {
            Dependency<TDependency>().Verify(expression, times);
        }
    }
}