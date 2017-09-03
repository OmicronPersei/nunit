// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    namespace DifferingNamespace1
    {
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

        class DummyGeneric<T>
        {
            public DummyGeneric(T obj)
            { }
        }
    }

    namespace DifferingNamespace2
    {
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
    }

    public class TypeNameDifferenceTestBase
    {

    }

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

        private void TestShortenedNameDifference(object objA, object objB, string expectedA, string expectedB)
        {
            string actualA, actualB;

            _differenceGetter.ResolveTypeNameDifference(
                 objA, objB, out actualA, out actualB);

            Assert.That(actualA, Is.EqualTo(expectedA));
            Assert.That(actualB, Is.EqualTo(expectedB));
        }

        [Test]
        public void TestResolveTypeNameDifferenceNonGenericDifferingTypes()
        {
            TestShortenedNameDifference(
                new Dummy(1),
                new Dummy1(1),
                "TypeNameDifferenceTests+Dummy",
                "TypeNameDifferenceTests+Dummy1");
        }

        [Test]
        public void TestResolveTypeNameDifferenceNonGenericNonDifferingTypes()
        {
            TestShortenedNameDifference(
                new Dummy(1),
                new Dummy(1),
                "TypeNameDifferenceTests+Dummy",
                "TypeNameDifferenceTests+Dummy");
        }

        [Test]
        public void TestResolveTypeNameDifferenceNonGenericNonDifferingTypesSingularDiffNamespace()
        {
            TestShortenedNameDifference(
                new DifferingNamespace1.Dummy(1),
                new Dummy(1),
                "Dummy",
                "TypeNameDifferenceTests+Dummy");
        }

        [Test]
        public void TestResolveTypeNameDifferenceNonGenericNonDifferingTypesBothDiffNamespace()
        {
            TestShortenedNameDifference(
                new DifferingNamespace1.Dummy(1),
                new DifferingNamespace2.Dummy(1),
                "DifferingNamespace1.Dummy",
                "DifferingNamespace2.Dummy");
        }

        [Test]
        public void TestResolveTypeNameDifferenceGeneric()
        {
            TestShortenedNameDifference(
                new DummyTemplatedClass<Dummy1>(new Dummy(1)),
                new DummyTemplatedClass<Dummy>(new Dummy(1)),
                "TypeNameDifferenceTests+DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy1]",
                "TypeNameDifferenceTests+DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy]");
        }

        [Test]
        public void TestResolveTypeNameDifferenceGenericDifferingNamespaces()
        {
            TestShortenedNameDifference(
                new DummyTemplatedClass<Dummy>(new Dummy(1)),
                new DummyTemplatedClass<DifferingNamespace1.Dummy>(new DifferingNamespace1.Dummy(1)),
                "TypeNameDifferenceTests+DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy]",
                "TypeNameDifferenceTests+DummyTemplatedClass`1[Dummy]");

            TestShortenedNameDifference(
                new DummyTemplatedClass<DifferingNamespace1.Dummy>(new DifferingNamespace1.Dummy(1)),
                new DummyTemplatedClass<Dummy>(new Dummy(1)),
                "TypeNameDifferenceTests+DummyTemplatedClass`1[Dummy]",
                "TypeNameDifferenceTests+DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy]");

            TestShortenedNameDifference(
                new DummyTemplatedClass<DifferingNamespace1.Dummy>(new DifferingNamespace1.Dummy(1)),
                new DummyTemplatedClass<DifferingNamespace2.Dummy>(new DifferingNamespace2.Dummy(1)),
                "TypeNameDifferenceTests+DummyTemplatedClass`1[DifferingNamespace1.Dummy]",
                "TypeNameDifferenceTests+DummyTemplatedClass`1[DifferingNamespace2.Dummy]");
        }

        [Test]
        public void TestResolveTypeNameDifferenceGenericDifferentAmountGenericParams()
        {
            TestShortenedNameDifference(
                new DummyTemplatedClass<Dummy>(new Dummy(1)),
                new KeyValuePair<int, string>(1, ""),
                "TypeNameDifferenceTests+DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy]",
                "KeyValuePair`2[Int32,String]");

            TestShortenedNameDifference(
                new KeyValuePair<int, string>(1, ""),
                new DummyTemplatedClass<Dummy>(new Dummy(1)),
                "KeyValuePair`2[Int32,String]",
                "TypeNameDifferenceTests+DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy]");
        }

        [Test]
        public void TestResolveNameDifferenceOneIsGenericOtherIsNot()
        {
            TestShortenedNameDifference(
                new DummyTemplatedClass<Dummy>(new Dummy(1)),
                new Dummy(1),
                "TypeNameDifferenceTests+DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy]",
                "TypeNameDifferenceTests+Dummy");

            TestShortenedNameDifference(
                new Dummy(1),
                new DummyTemplatedClass<Dummy>(new Dummy(1)),
                "TypeNameDifferenceTests+Dummy",
                "TypeNameDifferenceTests+DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy]");

            TestShortenedNameDifference(
                new KeyValuePair<string, int>("str", 0),
                new Dummy(1),
                "KeyValuePair`2[String,Int32]",
                "TypeNameDifferenceTests+Dummy");

            TestShortenedNameDifference(
                new Dummy(1),
                new KeyValuePair<string, int>("str", 0),
                "TypeNameDifferenceTests+Dummy",
                "KeyValuePair`2[String,Int32]");
        }

        [Test]
        public void TestNestedGenericsNonDifferingNamespace()
        {
            TestShortenedNameDifference(
                new DifferingNamespace1.DummyGeneric<List<string>>(new List<string>()),
                new DifferingNamespace1.DummyGeneric<IEnumerable<string>>(new List<string>()),
                "DummyGeneric`1[List`1[String]]",
                "DummyGeneric`1[IEnumerable`1[String]]");

            TestShortenedNameDifference(
                new DifferingNamespace1.DummyGeneric<IEnumerable<string>>(new List<string>()),
                new DifferingNamespace1.DummyGeneric<List<string>>(new List<string>()),
                "DummyGeneric`1[IEnumerable`1[String]]",
                "DummyGeneric`1[List`1[String]]");

            TestShortenedNameDifference(
                new DifferingNamespace1.DummyGeneric<KeyValuePair<DifferingNamespace1.Dummy, DifferingNamespace2.Dummy>>(new KeyValuePair<DifferingNamespace1.Dummy, DifferingNamespace2.Dummy>()),
                new DifferingNamespace1.DummyGeneric<KeyValuePair<DifferingNamespace2.Dummy, DifferingNamespace1.Dummy>>(new KeyValuePair<DifferingNamespace2.Dummy, DifferingNamespace1.Dummy>()),
                "DummyGeneric`1[KeyValuePair`2[DifferingNamespace1.Dummy,DifferingNamespace2.Dummy]]",
                "DummyGeneric`1[KeyValuePair`2[DifferingNamespace2.Dummy,DifferingNamespace1.Dummy]]");
        }

        [Test]
        public void TestIsObjectInstanceGeneric()
        {
            var notGeneric = new Dummy(1);

            Assert.False(_differenceGetter.IsObjectTypeGeneric(notGeneric.GetType().ToString()));

            var generic = new DummyTemplatedClass<Dummy>(new Dummy(1));

            Assert.That(_differenceGetter.IsObjectTypeGeneric(generic.GetType().ToString()));
        }

        [Test]
        public void TestGetTopLevelGenericName()
        {
            var generic = new DummyTemplatedClass<Dummy>(new Dummy(1)).GetType().ToString();

            var expected = "NUnit.Framework.Internal.TypeNameDifferenceTests+DummyTemplatedClass`1";

            var actual = _differenceGetter.GetTopLevelGenericType(generic);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetTopLevelGenericNameThrowsWhenNotGeneric()
        {
            var notGeneric = new Dummy(1).GetType().ToString();

            Assert.Throws<ArgumentException>(() => _differenceGetter.GetTopLevelGenericType(notGeneric));
        }

        [Test]
        public void TestGetFullyQualifiedGenericParametersOfObjectSingleGenericParam()
        {
            var generic = new DummyTemplatedClass<Dummy>(new Dummy(1));

            var expected = new List<string>() { "NUnit.Framework.Internal.TypeNameDifferenceTests+Dummy" };
            var actual = _differenceGetter.GetTopLevelFullyQualifiedGenericParameters(generic.GetType().ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetFullyQualifiedGenericParametersOfObjectDoubleGenericParam()
        {
            var generic = new KeyValuePair<string, int>();

            var expected = new List<string>() { "System.String", "System.Int32"};
            var actual = _differenceGetter.GetTopLevelFullyQualifiedGenericParameters(generic.GetType().ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetFullyQualifiedGenericParametersOfObjectNestedGenerics()
        {
            var generic = new KeyValuePair<int, KeyValuePair<int, string>>();

            var expected = new List<string>() { "System.Int32", "System.Collections.Generic.KeyValuePair`2[System.Int32,System.String]" };

            var actual = _differenceGetter.GetTopLevelFullyQualifiedGenericParameters(generic.GetType().ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestReconstructShortenedGenericTypeName()
        {
            var expected = "KeyValuePair`2[String,Int32]";

            var actual = _differenceGetter.ReconstructShortenedGenericTypeName(
                "KeyValuePair`2",
                new List<string>() { "String", "Int32" });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestShortenTypeNamesDifferingNamespace()
        {
            var fullyQualifiedA = "DifferingNamespaceA.Class";
            var fullyQualifiedB = "DifferingNamespaceB.Class";

            var expectedA = "DifferingNamespaceA.Class";
            var expectedB = "DifferingNamespaceB.Class";

            string shortenedA, shortenedB;

            _differenceGetter.ShortenTypeNames(fullyQualifiedA, fullyQualifiedB, out shortenedA, out shortenedB);

            Assert.AreEqual(expectedA, shortenedA);
            Assert.AreEqual(expectedB, shortenedB);
        }

        [Test]
        [TestCase("NamespaceA.NamespaceB.Type", "Type")]
        [TestCase("NamespaceA.Type", "Type")]
        [TestCase("Type", "Type")]
        public void TestGetOnlyTypeName(string input, string expectedOutput)
        {
            string actual = _differenceGetter.GetOnlyTypeName(input);

            Assert.AreEqual(expectedOutput, actual);
        }

        [Test]
        [TestCase("A.GenericType`1[B.Type]", "GenericType`1[Type]")]
        [TestCase("A.GenericType`2[B.TypeA,C.D.E.TypeB]", "GenericType`2[TypeA,TypeB]")]
        public void TestShortenFullyQualifiedGenericType(string FullyQualifiedGenericType, string expectedOutput)
        {
            string actual = _differenceGetter.ShortenFullyQualifiedGenericType(FullyQualifiedGenericType);

            Assert.AreEqual(expectedOutput, actual);
        }

        //TODO: create test for nested generics
    }
}
