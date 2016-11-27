using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace SaftPT.Tests.Helpers
{
    internal class SubjectUnderTest<T> : IUnderTest<T> where T : class
    {
        private readonly IDictionary<string, Func<Mock>> _dependencyMockFactories;
        private readonly IDictionary<string, Mock> _dependencyMocks;

        public Mock<T> SubjectMock { get; private set; }

        public SubjectUnderTest()
        {
            _dependencyMockFactories = new Dictionary<string, Func<Mock>>();
            _dependencyMocks = new Dictionary<string, Mock>();
        }
         
        public void DependsOn<TDependency>() where TDependency : class
        {
            var typeName = GetTypeName<TDependency>();
            _dependencyMockFactories[typeName] = () => new Mock<TDependency>();
        }

        private static string GetTypeName<TDependency>()
        {
            return typeof (TDependency).FullName;
        }

        public T MakeSubject()
        {
            MakeDependencyMocks();

            SubjectMock = new Mock<T>(_dependencyMocks.Values.Select(e => e.Object).ToArray());

            return SubjectMock.Object;
        }

        public void MakeDependencyMocks()
        {
            if (_dependencyMocks.Any())
            {
                return;
            }

            foreach (var pair in _dependencyMockFactories)
            {
                _dependencyMocks[pair.Key] = pair.Value();
            }
        }

        public Mock<TDependency> Get<TDependency>() where TDependency : class
        {
            MakeDependencyMocks();
            return (Mock<TDependency>) _dependencyMocks[GetTypeName<TDependency>()];
        }
        
    }
    
}