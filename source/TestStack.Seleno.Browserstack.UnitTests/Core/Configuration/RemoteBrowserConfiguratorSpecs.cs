﻿using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OpenQA.Selenium;
using TestStack.Seleno.BrowserStack.Core.Capabilities;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Enums;
using TestStack.Seleno.BrowserStack.Core.Exceptions;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class RemoteBrowserConfiguratorSpecs
    {
        private RemoteBrowserConfigurator _sut;
        private IBrowserHostFactory _browserHostFactory;
        private IBrowserConfigurationParser _parser;
        private ICapabilitiesBuilder _capabilitiesBuilder;
        private IConfigurationProvider _configurationProvider;

        [SetUp]
        public void SetUp()
        {
            _browserHostFactory = Substitute.For<IBrowserHostFactory>();
            _parser = Substitute.For<IBrowserConfigurationParser>();
            _capabilitiesBuilder = Substitute.For<ICapabilitiesBuilder>();
            _configurationProvider = Substitute.For<IConfigurationProvider>();

            _capabilitiesBuilder.WithTestSpecification(Arg.Any<TestSpecification>()).Returns(_capabilitiesBuilder);
            _capabilitiesBuilder.WithRunTestLocally(Arg.Any<bool>()).Returns(_capabilitiesBuilder);
            _sut = new RemoteBrowserConfigurator(_browserHostFactory,_parser, _capabilitiesBuilder, _configurationProvider);
        }

        [TestCase(null)]
        [TestCase("configuration")]
        public void CreateAndConfigure_ShouldAlwaysConfigureTestSpecifications(string browser)
        {
            // Arrange
            var testSpecification = new TestSpecification("Fancy scenario", "178wq76essf");

            // Act
            _sut.CreateAndConfigure(testSpecification, browser);

            // Assert

            _capabilitiesBuilder.DidNotReceive().WithCredentials(Arg.Any<string>(), Arg.Any<string>());
            _capabilitiesBuilder.Received().WithTestSpecification(testSpecification);
            _capabilitiesBuilder.Received().WithRunTestLocally(false);

        }

        [Test]
        public void CreateAndConfigure_ShouldNotConfigureBrowserWhenNoBrowserConfigurationSpecified()
        {
            // Arrange
            var testSpecification =new TestSpecification("Fancy scenario","178wq76essf");
            var browserHost = Substitute.For<IBrowserHost>();
            var capabilities = Substitute.For<ICapabilities>();

            _capabilitiesBuilder.Build().Returns(capabilities);
            _browserHostFactory.CreateWithCapabilities(capabilities).Returns(browserHost);

            // Act

            var result = _sut.CreateAndConfigure(testSpecification);

            // Assert
            result.Should().BeSameAs(browserHost);
            _capabilitiesBuilder.DidNotReceive().WithBrowserConfiguration(Arg.Any<BrowserConfiguration>());
            _capabilitiesBuilder.Received().WithRunTestLocally(false);
        }

        [Test]
        public void CreateAndConfigure_ShouldCreateAndBrowserHostWithSpecifiedCapabilities()
        {
            // Arrange
            var testSpecification = new TestSpecification("Fancy scenario", "178wq76essf");
            var browserHost = Substitute.For<IBrowserHost>();
            var capabilities = Substitute.For<ICapabilities>();
            const string browser = "SomeBrowserConfiguration";

            _capabilitiesBuilder.Build().Returns(capabilities);
            _browserHostFactory.CreateWithCapabilities(capabilities).Returns(browserHost);

            // Act

            var result = _sut.CreateAndConfigure(testSpecification, browser);

            // Assert
            result.Should().BeSameAs(browserHost);
            _capabilitiesBuilder.Received().WithBrowserConfiguration(Arg.Any<BrowserConfiguration>());
            _capabilitiesBuilder.Received().WithRunTestLocally(false);
        }

        [Test]
        public void CreateAndConfigure_ShouldThrowExceptionWhenParserFailsToExtractBrowserConfiguration()
        {
            // Arrange
            var testSpecification = new TestSpecification("Fancy scenario", "178wq76essf");
            var capabilities = Substitute.For<ICapabilities>();
            const string unsupportedBrowser = "jsdhfjsg";

            var expectedException = new InvalidBrowserConfigurationException("Unsupported browser");

            _parser.When(x => x.Parse(unsupportedBrowser)).Do(x => { throw expectedException; });

            _capabilitiesBuilder.Build().Returns(capabilities);

            Action unsupportedActionBrowser = () => _sut.CreateAndConfigure(testSpecification, unsupportedBrowser);

            // Act && Assert
            unsupportedActionBrowser
                .ShouldThrow<InvalidBrowserConfigurationException>()
                .And.Should().BeSameAs(expectedException);

            _browserHostFactory
                .DidNotReceive()
                .CreateWithCapabilities(capabilities);
        }
        
        [TestCase(BrowserEnum.Chrome)]
        [TestCase(BrowserEnum.Firefox)]
        [TestCase(BrowserEnum.InternetExplorer)]
        [TestCase(BrowserEnum.PhantomJs)]
        [TestCase(BrowserEnum.Safari)]
        public void CreateAndConfigure_ShouldCallCreateLocalBrowser(BrowserEnum browserType)
        {
            // Arrange
            var testSpecification = new TestSpecification("Fancy scenario", "178wq76essf");           
            _configurationProvider.LocalBrowser.Returns(browserType);

            // Act
            _sut.CreateAndConfigure(testSpecification, "configuration");

            // Assert
            _browserHostFactory.Received(1).CreateLocalWebDriver(browserType);
            _capabilitiesBuilder.Received().WithRunTestLocally(false);
        }
       
    }
}