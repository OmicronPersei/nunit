namespace NUnit.Framework.Internal.Filters
{
    public class MultipleFilterTests : TestFilterTests
    {
        [Test]
        public void TestExplicitNotSelectedCompoundFilter()
        {
            //where cat==Dummy || cat!=Dummy
            var filter = new OrFilter(new NotFilter(new CategoryFilter("Dummy")), new CategoryFilter("Dummy"));

            Assert.False(filter.IsExplicitMatch(_explicitFixture));
            Assert.False(filter.Pass(_explicitFixture));
        }
    }
}
