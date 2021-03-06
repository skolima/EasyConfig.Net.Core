using System;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;
using NUnit.Framework;
#pragma warning disable 649

namespace EasyConfig.UnitTests
{
    [TestFixture]
    public class ConfigTests
    {
        private const string EnvironmentVariableTestKey = "EasyConfig_this_environment_variable_should_only_exist_during_these_tests";

        [SetUp]
        public void SetUp()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableTestKey, Guid.NewGuid().ToString());
        }

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableTestKey, null);
        }

        [Test]
        public void Populate_Object_GivenValidArgs_DoesntThrow()
        {
            Assert.DoesNotThrow(() => Config.Populate<object>());
        }

        [Test]
        public void Populate_UriRequired_GivenDgApiEndpoint_DoesntThrow()
        {
            Assert.DoesNotThrow(() => Config.Populate<UriRequired>("endpoint=http://www.google.com"));
        }

        [Test]
        public void Populate_UriRequired_GivenDgApiEndpoint_SetsApiEndpoint()
        {
            var parameters = Config.Populate<UriRequired>("endpoint=http://www.google.com");

            Assert.That(parameters.Test, Is.Not.Null);
        }

        [Test]
        public void Populate_UriRequired_GivenAUri_SetsUriCorrectly()
        {
            var endpoint = new Uri("http://www.google.com");
            var parameters = Config.Populate<UriRequired>($"endpoint={endpoint}");

            Assert.That(parameters.Test, Is.EqualTo(endpoint));
        }

        [Test]
        public void Populate_UriRequired_GivenNotAUri_Throws()
        {
            Assert.Throws<ConfigurationTypeException>(() => Config.Populate<UriRequired>("endpoint=notanendpoint"));
        }

        [Test]
        public void Populate_UriRequired_NotGivenRequiredParameter_Throws()
        {
            Assert.Throws<ConfigurationMissingException>(() => Config.Populate<UriRequired>());
        }

        [Test]
        public void Populate_InEnvironmentVariabless_RequiredParamInEnvironment_DoesntThrow()
        {
            Assert.DoesNotThrow(() => Config.Populate<InEnvironmentVariables>());
        }

        [Test]
        public void Populate_NotInEnvironmentVariables_RequiredParamNotInEnvironment_Throws()
        {
            Assert.Throws<ConfigurationMissingException>(() => Config.Populate<NotInEnvironmentVariables>());
        }

        [Test]
        public void Populate_NotInEnvironmentVariables_RequiredParamNotInEnvironmentButInArgs_Throws()
        {
            Assert.Throws<ConfigurationMissingException>(() => Config.Populate<NotInEnvironmentVariables>("defninitely_not_in_environment_variables=shouldstillthrow"));
        }

        [Test]
        public void Populate_NotRequired_ParamNotInEnvironmentButInArgs_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => Config.Populate<NotRequired>());
        }

        [Test]
        public void Populate_HasDefault_UsesDefault()
        {
            var config = Config.Populate<HasDefault>();

            Assert.That(config.Test, Is.EqualTo("defaulttest"));
        }

        [Test]
        public void Populate_IntRequired_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => Config.Populate<IntRequired>("number=1"));
        }

        [Test]
        public void Populate_IntRequired_SetsNumber()
        {
            var num = new Random().Next();
            var config = Config.Populate<IntRequired>("number=" + num);

            Assert.That(config.Test, Is.EqualTo(num));
        }

        [Test]
        public void Populate_IntRequired_GivenNotAnInt_Throws()
        {
            Assert.Throws<ConfigurationTypeException>(() => Config.Populate<IntRequired>("number=notaninteger"));
        }

        private class UriRequired
        {
            [EnvironmentOrCommandLine("endpoint"), Required]
            public Uri Test;
        }

        private class IntRequired
        {
            [EnvironmentOrCommandLine("number"), Required]
            public int Test;
        }

        private class InEnvironmentVariables
        {
            [Environment(EnvironmentVariableTestKey), Required]
            public string Test;
        }

        private class NotInEnvironmentVariables
        {
            [Environment("shouldnt_be_in_environment_variables"), Required]
            public string Test;
        }

        private class NotRequired
        {
            [Environment("shouldnt_be_in_environment_variables")]
            public string Test;
        }

        private class HasDefault
        {
            [Environment("shouldnt_be_in_environment_variables"), Default("defaulttest")]
            public string Test;
        }
    }
}
