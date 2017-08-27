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

        //[Test]
        //public void TestResolveTypeNameDifferenceGeneric()
        //{
        //    var actual = new DummyTemplatedClass<Dummy>(new Dummy(1));
        //    var expected = new DummyTemplatedClass<Dummy>(new Dummy1(1));

        //    string actualStr, expectedStr;

        //    _differenceGetter.ResolveTypeNameDifference(
        //        expected, actual, out expectedStr, out actualStr);

        //    Assert.That(expectedStr, Is.EqualTo("DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy1]"));
        //    Assert.That(actualStr, Is.EqualTo("DummyTemplatedClass`1[TypeNameDifferenceTests+Dummy]"));
        //}

        [Test]
        public void TestIsObjectInstanceGeneric()
        {
            var notGeneric = new Dummy(1);

            Assert.False(_differenceGetter.IsObjectTypeGeneric(notGeneric));

            var generic = new DummyTemplatedClass<Dummy>(new Dummy(1));

            Assert.That(_differenceGetter.IsObjectTypeGeneric(generic));
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

        //TODO: create test for nested generics
    }
}
