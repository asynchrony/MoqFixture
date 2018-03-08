using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters;
using Moq;

namespace MoqFixture
{
    public interface IMoqFixture
    {
        Mock<TMock> Mock<TMock>() where TMock : class;
    }

    public class MoqFixture<T> : IMoqFixture where T : class
    {
        private class GloballyConfigurableFixture : IMoqFixture
        {
            private readonly IMoqFixture _fixture;

            public GloballyConfigurableFixture(IMoqFixture fixture)
            {
                _fixture = fixture;
            }

            public Mock<TMock> Mock<TMock>() where TMock : class
            {
                try
                {
                    return _fixture.Mock<TMock>();
                }
                catch (InvalidOperationException)
                {
                    return new Mock<TMock>();
                }
            }
        }

        private readonly Dictionary<string, Mock> _mocks;
        private readonly Type _testObjectType = typeof(T);
        private readonly ConstructorInfo _testObjectConstructor;

        public MoqFixture()
        {
            var constructors = _testObjectType.GetConstructors();
            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"Expected the test object {_testObjectType.Name} to have exactly one constructor, but it has {constructors.Length}.");
            }

            _testObjectConstructor = constructors.Single();

            _mocks = new Dictionary<string, Mock>();

            InitMocks();
            InitTestObject();
            DefaultMocks.SetupFixture(new GloballyConfigurableFixture(this));
        }

        protected T TestObject { get; set; }

        public Mock<TMock> Mock<TMock>() where TMock : class
        {
            if (_mocks.TryGetValue(typeof(TMock).FullName, out var mock))
            {
                return (Mock<TMock>)mock;
            }

            throw new InvalidOperationException($"The no mock of type {typeof(TMock).Name} is available, because test object's constructor did not request a dependency of type {typeof(TMock).Name}.");
        }

        private void InitMocks()
        {
            var mockTypes = _testObjectConstructor.GetParameters().Select(x => x.ParameterType);
            foreach (var argType in mockTypes)
            {
                if (_mocks.ContainsKey(argType.FullName))
                {
                    throw new InvalidOperationException($"Constructor for {_testObjectType.Name} has duplicate dependency {argType.Name}.");
                }
                var mock = ResolveMock(argType);
                _mocks.Add(argType.FullName, mock);
            }
        }

        private void InitTestObject()
        {
            var paramaters = _mocks.Values.Select(x => x.Object).ToArray();
            try
            {
                TestObject = (T)_testObjectConstructor.Invoke(paramaters);
            }
            catch (TargetInvocationException e)
            {
                throw new Exception($"Construction of TestObject of type {_testObjectType.Name} threw an exception.", e.InnerException);
            }
        }

        private Mock ResolveMock(Type argType)
        {
            try
            {
                var mockType = typeof(Mock<>).MakeGenericType(argType);
                var mockConstructor = mockType.GetConstructors().Single(x => x.GetParameters().Length == 0);

                var mock = (Mock) mockConstructor.Invoke(new object[] {});
                return mock;
            }
            catch (Exception e) when (e is ArgumentException || e is TargetInvocationException)
            {
                var msg = $"Cannot set up mocks for {_testObjectType.Name}, because the dependency type {argType.Name} requested by its constructor cannot be mocked. Dependencies must be interfaces or non-sealed classes.";
                throw new InvalidOperationException(msg, e);
            }
        }
    }
}
