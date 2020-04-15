using Xunit;

namespace Fast.Activator.Tests
{
    public class BasicTests
    {
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
            var Result = FastActivator.CreateInstance<int>();
            Assert.Equal(0, Result);
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
    }
}