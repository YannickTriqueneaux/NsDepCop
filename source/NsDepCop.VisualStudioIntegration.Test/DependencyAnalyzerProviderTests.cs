﻿using Codartis.NsDepCop.Core.Factory;
using Codartis.NsDepCop.Core.Interface.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Codartis.NsDepCop.VisualStudioIntegration.Test
{
    [TestClass]
    public class DependencyAnalyzerProviderTests
    {
        private Mock<IDependencyAnalyzerFactory> _dependencyAnalyzerFactoryMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _dependencyAnalyzerFactoryMock = new Mock<IDependencyAnalyzerFactory>();
        }

        [TestMethod]
        public void GetDependencyAnalyzer_RetrievedOnce_CallsFactory()
        {
            const string filePath = "myFilePath";

            var dependencyAnalyzerProvider = CreateDependencyAnalyzerProvider();

            dependencyAnalyzerProvider.GetDependencyAnalyzer(filePath);

            _dependencyAnalyzerFactoryMock.Verify(i => i.CreateFromMultiLevelXmlConfigFile(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GetDependencyAnalyzer_RetrievedTwice_CallsFactoryThenAnalyzerRefresh()
        {
            const string filePath = "myFilePath";

            var dependencyAnalyzerProvider = CreateDependencyAnalyzerProvider();

            var analyzerMock = new Mock<IDependencyAnalyzer>();
            _dependencyAnalyzerFactoryMock.Setup(i => i.CreateFromMultiLevelXmlConfigFile(It.IsAny<string>()))
                .Returns(analyzerMock.Object);

            dependencyAnalyzerProvider.GetDependencyAnalyzer(filePath);
            _dependencyAnalyzerFactoryMock.Verify(i => i.CreateFromMultiLevelXmlConfigFile(It.IsAny<string>()), Times.Once);
            analyzerMock.Verify(i => i.RefreshConfig(), Times.Never);

            dependencyAnalyzerProvider.GetDependencyAnalyzer(filePath);
            _dependencyAnalyzerFactoryMock.Verify(i => i.CreateFromMultiLevelXmlConfigFile(It.IsAny<string>()), Times.Once);
            analyzerMock.Verify(i => i.RefreshConfig(), Times.Once);
        }

        private IDependencyAnalyzerProvider CreateDependencyAnalyzerProvider()
        {
            return new DependencyAnalyzerProvider(_dependencyAnalyzerFactoryMock.Object);
        }
    }
}