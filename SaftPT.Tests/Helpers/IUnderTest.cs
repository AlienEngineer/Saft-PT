using Moq;

namespace SaftPT.Tests.Helpers
{
    public interface IUnderTest<T> where T : class
    {
        Mock<T> SubjectMock { get; }
        void DependsOn<TDependency>() where TDependency : class;
        T MakeSubject();
        Mock<TDependency> Get<TDependency>() where TDependency : class;
    }
}