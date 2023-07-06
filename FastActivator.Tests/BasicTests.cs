using Xunit;

namespace Fast.Activator.Tests
{
    public class BasicTests
    {
        [Fact]
        public void Enums()
        {
            Assert.Equal(Status.Start, FastActivator.CreateInstance<Status>());
            Assert.Equal(Status.Start, FastActivator.CreateInstance(typeof(Status)));
        }

        [Fact]
        public void Nullable()
        {
            Assert.Null(FastActivator.CreateInstance<int?>());
            Assert.Null(FastActivator.CreateInstance(typeof(int?)));
            Assert.Null(FastActivator.CreateInstance<double?>());
            Assert.Null(FastActivator.CreateInstance(typeof(double?)));
        }

        [Fact]
        public void Simple()
        {
            var Result = FastActivator.CreateInstance<SimpleTestClass>();
            Assert.Null(Result.A);
            Assert.False(Result.B);
        }

        [Fact]
        public void SimpleValue()
        {
            Assert.Equal(0, FastActivator.CreateInstance<int>());
            Assert.Equal(0, FastActivator.CreateInstance(typeof(int)));
            Assert.Null(FastActivator.CreateInstance<string>());
            Assert.Null(FastActivator.CreateInstance(typeof(string)));
        }

        [Fact]
        public void StructCreation()
        {
            var Result = FastActivator.CreateInstance<StructTest>(1, 2);
            Assert.Equal(1, Result.A);
            Assert.Equal(2, Result.B);
        }

        [Fact]
        public void WithParams()
        {
            var Result = FastActivator.CreateInstance<ParamsTestClass>("A", true);
            Assert.Equal("A", Result.A);
            Assert.True(Result.B);
        }

        [Fact]
        public void WithParamsMultipleConstructors()
        {
            var Result = FastActivator.CreateInstance<ParamsMultipleTestClass>("A", true);
            Assert.Equal("A", Result.A);
            Assert.True(Result.B);
        }

        private struct StructTest
        {
            public StructTest(int a, int b)
            {
                A = a;
                B = b;
            }

            public int A;
            public int B;
        }

        private class ParamsMultipleTestClass
        {
            public ParamsMultipleTestClass()
            {
                A = "C";
                B = false;
            }

            public ParamsMultipleTestClass(string a, bool b)
            {
                A = a;
                B = b;
            }

            public ParamsMultipleTestClass(string a, int b)
            {
                A = a;
                B = false;
            }

            public string A { get; set; }
            public bool B { get; set; }
        }

        private class ParamsTestClass
        {
            public ParamsTestClass(string a, bool b)
            {
                A = a;
                B = b;
            }

            public string A { get; set; }
            public bool B { get; set; }
        }

        private class SimpleTestClass
        {
            public string A { get; set; }
            public bool B { get; set; }
        }

        private enum Status
        {
            Start = 0,
            Stop
        }
    }
}