namespace NUnit.Framework.Internal
{
    public class TypeNameDifferenceTests
    {
        #region Mock types

        class Dummy
        {
            internal readonly int value;

            public Dummy(int value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return "Dummy " + value;
            }
        }

        class Dummy1
        {
            internal readonly int value;

            public Dummy1(int value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return "Dummy " + value;
            }
        }

        class DummyTemplatedClass<T>
        {
            private object _obj;
            public DummyTemplatedClass(object obj)
            {
                _obj = obj;
            }

            public override string ToString()
            {
                return _obj.ToString();
            }
        }

        #endregion

        TypeNameDifference _differenceGetter;

        [SetUp]
        public void TestSetup()
        {
            _differenceGetter = new TypeNameDifference();
        }

        [Test]
        public void TestResolveTypeNameDifferenceNonGeneric()
        {
            var actual = new Dummy(1);
            var expected = new Dummy1(1);

            string actualStr, expectedStr;

            _differenceGetter.ResolveTypeNameDifference(
                expected, actual, out expectedStr, out actualStr);

            Assert.That(expectedStr, Is.EqualTo("TypeNameDifferenceTests+Dummy1"));
            Assert.That(actualStr, Is.EqualTo("TypeNameDifferenceTests+Dummy"));
        }
    }
}
