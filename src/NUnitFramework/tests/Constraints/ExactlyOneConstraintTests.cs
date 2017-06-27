﻿using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.Framework.Tests.Constraints
{
    public class ExactlyOneConstraintTests
    {
        private static IEnumerable<string> testCollectionLen0 = new List<string>();
        private static IEnumerable<string> testCollectionLen1 = new List<string> { "item" };
        private static IEnumerable<string> testCollectionLen2 = new List<string> { "item", "otherItem" };
        private static IEnumerable<string> testCollectionLen3 = new List<string> { "item", "item", "otherItem" };

        [Test]
        public void ExactlyOneThrowsWhenNotIEnumberable()
        {
            Assert.Throws<ArgumentException>(() =>
                Assert.That(new int(), new ExactlyOneConstraint(Is.EqualTo("item"))));
        }

        [Test]
        public void ExactlyOneThrowsWhenConstraintIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var blah = new ExactlyOneConstraint(null);
            });
        }

        [Test]
        public void ExactlyOneItemInCollection()
        {
            Assert.That(testCollectionLen1, new ExactlyOneConstraint());
            Assert.That(testCollectionLen1, Has.One().Items);
        }

        [Test]
        public void ExactlyOneItemMatches()
        {
            Assert.That(testCollectionLen1, new ExactlyOneConstraint(Is.EqualTo("item")));
            Assert.That(testCollectionLen1, Has.One().EqualTo("item"));
        }

        [Test]
        public void ExactlyOneItemDoesNotMatch()
        {
            Assert.That(testCollectionLen1, new ExactlyOneConstraint(Is.Not.EqualTo("notItem")));
            Assert.That(testCollectionLen1, Has.One().Not.EqualTo("notItem"));
        }

        [Test]
        public void ExactlyOneItemMustMatchButDoesNotMatchTestMessage()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "one item equal to \"notItem\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"item\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen1, new ExactlyOneConstraint(Is.EqualTo("notItem"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyOneItemInCollectionButNone()
        {
            Assert.IsFalse(new ExactlyOneConstraint().ApplyTo(testCollectionLen0).IsSuccess);
        }

        [Test]
        public void ExactlyOneItemMustMatchButNoneInCollection()
        {
            Assert.IsFalse(new ExactlyOneConstraint(Is.EqualTo("item")).ApplyTo(testCollectionLen0).IsSuccess);
            Assert.IsFalse(Has.One().EqualTo("item").ApplyTo(testCollectionLen0).IsSuccess);
        }

        [Test]
        public void ExactlyOneItemMustMatchButNoneInCollectionTestMessage()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "one item equal to \"item\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "<empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen0, new ExactlyOneConstraint(Is.EqualTo("item"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyOneBlankConstraintWhereCollectionIsTwo()
        {
            Assert.IsFalse(new ExactlyOneConstraint().ApplyTo(testCollectionLen2).IsSuccess);
            Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen2, Has.One().Items));
        }

        [Test]
        public void ExactlyOneBlankConstraintWhereCollectionIsTwoTestMessage()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "length of 1" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "2" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen2, new ExactlyOneConstraint()));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyOneItemDoesNotMatchFromTwoElements()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen2, new ExactlyOneConstraint(Is.EqualTo("notItem"))));
            Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen2, Has.One().EqualTo("notItem")));
        }

        [Test]
        public void ExactlyOneItemDoesNotMatchFromTwoElementsTestMessage()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "one item equal to \"notItem\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"item\", \"otherItem\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen2, new ExactlyOneConstraint(Is.EqualTo("notItem"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyOneItemMustMatchButMultipleMatch()
        {
            Assert.IsFalse(new ExactlyOneConstraint(Is.EqualTo("item")).ApplyTo(testCollectionLen3).IsSuccess);
            Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen3, Has.One().EqualTo("item")));
        }

        [Test]
        public void ExactlyOneItemMustMatchButMultipleMatchTestMessage()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "one item equal to \"notItem\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"item\", \"item\", \"otherItem\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen3, new ExactlyOneConstraint(Is.EqualTo("notItem"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyOneBlankConstraintWhereCollectionIsOne()
        {
            Assert.That(testCollectionLen1, new ExactlyOneConstraint());
            Assert.That(testCollectionLen1, Has.One().Items);
        }

        [Test]
        public void ExactlyOneConstraintMatchingWhereCollectionIsTwo()
        {
            Assert.That(testCollectionLen2, new ExactlyOneConstraint(Is.EqualTo("item")));
            Assert.That(testCollectionLen2, Has.One().EqualTo("item"));
        }

        [Test]
        public void ExactlyOneConstraintNotMatchingWhereCollectionIsTwo()
        {
            Assert.IsFalse(new ExactlyOneConstraint(Is.EqualTo("blah")).ApplyTo(testCollectionLen2).IsSuccess);
            Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen2, Has.One().EqualTo("blah")));
        }
    }
}
