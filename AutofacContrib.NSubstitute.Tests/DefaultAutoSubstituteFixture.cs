using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacContrib.NSubstitute.Tests
{
    public class DefaultAutoSubstituteFixture
    {
        [Test]
        public void CanLock()
        {
            var builder = new DefaultAutoSubstituteBuilder()
                .ConfigureDefault(_ => { });

            builder.Lock();

            Assert.Throws<InvalidOperationException>(() => builder.ConfigureDefault(_ => { }));
        }

        [Test]
        public void OptionSet()
        {
            var defaultOptions = new AutoSubstituteOptions();
            var handler = Substitute.For<MockHandler>();
            var defaultBuilder = new DefaultAutoSubstituteBuilder()
                .ConfigureDefault(b => b.ConfigureOptions(options =>
                {
                    options.MockHandlers.Add(handler);
                }));

            var handlers = default(ICollection<MockHandler>);

            var builder = new AutoSubstituteBuilder(defaultBuilder)
                .ConfigureOptions(options =>
                {
                    handlers = options.MockHandlers;
                });

            Assert.That(handlers, Contains.Item(handler));
        }

        [Test]
        public void DefaultIsSameWithMultipleInvocations()
        {
            Assert.AreSame(AutoSubstitute.Default, AutoSubstitute.Default);
        }
    }
}
