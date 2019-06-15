using Bogus;
using NUnit.Framework;

namespace Discans.Tests
{
    public class BaseTests
    {
        protected Faker Faker;

        [SetUp]
        public void BaseSetUp() => 
            Faker = new Faker();
    }
}
