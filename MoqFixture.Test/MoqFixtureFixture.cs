using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MoqFixture.Test
{
    [TestClass]
    public class MoqFixtureFixture
    {
        [TestMethod]
        public void Throws_Exception_If_Type_Consumes_Primative()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new PrimativeFixture());
        }

        [TestMethod]
        public void Throws_Exception_If_Type_Cannot_Be_Implemented()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new SealedFixture());
        }

        [TestMethod]
        public void Throws_Exception_If_Type_Has_Multiple_Constructors()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new MultiFixture());
        }

        [TestMethod]
        public void Throws_Exception_If_Type_Has_Duplicate_Constructor_Args()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new DuplicateFixture());
        }

        [TestMethod]
        public void Throws_Exception_If_Type_Has_Not_Asked_For_Dependency()
        {
            var fixture = new ValidFixture();
            Assert.ThrowsException<InvalidOperationException>(() => fixture.AskForWrongMock());
        }

        [TestMethod]
        public void Throws_Exception_If_Type_Constructor_Throws_An_Exception()
        {
            var actualException = Assert.ThrowsException<Exception>(() => new ConstructorExceptionFixture());
            Assert.AreEqual(ConstructorException.error, actualException.InnerException);
        }

        [TestMethod]
        public void Mocks_Dependencies_Correctly()
        {
            var fixture = new ValidFixture();

            var mockOne = fixture.GetMock<DependencyOne>();
            var mockTwo = fixture.GetMock<DependencyTwo>();

            Assert.AreEqual(mockOne.Object, ValidType.dependencyOne);
            Assert.AreEqual(mockTwo.Object, ValidType.dependencyTwo);
        }
    }

    class ValidFixture : MoqFixture<ValidType>
    {
        public void AskForWrongMock()
        {
            Mock<IConvertible>();
        }

        public Mock<T> GetMock<T>() where T : class
        {
            return Mock<T>();
        }
    }

    class DuplicateFixture : MoqFixture<Duplicate>
    {

    }

    class MultiFixture : MoqFixture<Multi>
    {

    }

    class PrimativeFixture : MoqFixture<TakesPrimative>
    {

    }

    class SealedFixture : MoqFixture<TakesSealed>
    {

    }

    class ConstructorExceptionFixture : MoqFixture<ConstructorException>
    {

    }

    public class ConstructorException
    {
        public static readonly Exception error = new Exception("This is a test error");

        public ConstructorException()
        {
            throw error;
        }
    }

    public interface DependencyTwo
    {
        string GetString(int thing);
    }

    public class DependencyOne
    {
        public object GetObject(Object param)
        {
            return new Object();
        }
    }

    class ValidType
    {
        public static DependencyOne dependencyOne;
        public static DependencyTwo dependencyTwo;

        public ValidType(DependencyOne one, DependencyTwo two)
        {
            dependencyOne = one;
            dependencyTwo = two;
        }        
    }

    class Duplicate
    {
        public Duplicate(Multi one, Multi two)
        {

        }
    }

    class Multi
    {
        public Multi()
        {

        }

        public Multi(int arg)
        {

        }
    }

    class TakesPrimative
    {
        public TakesPrimative(int primative, IAsyncResult type)
        {

        }
    }

    class TakesSealed
    {
        public TakesSealed(Sealed imSealed)
        {

        }
    }

    sealed class Sealed
    {

    }
}
