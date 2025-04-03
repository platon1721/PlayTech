using Models;
using Moq;
using Serilog;
using Services;
using ViewModels;

namespace Tests
{
    [TestFixture]
    public class MainWindowStatisticsTests
    {
        private Mock<ILogger> _loggerMock;
        private MainWindowViewModel _viewModel;
        
        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _loggerMock.Setup(l => l.Information(It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();
            
            var testDispatcher = new TestDispatcher();
            
            _viewModel = new MainWindowViewModel(_loggerMock.Object, testDispatcher);
        }
        
        [Test]
        public void Constructor_InitializesStatisticsProperty()
        {
            Assert.That(_viewModel.Statistics, Is.Not.Null);
        }
        
        [Test]
        public void OnStatisticsReceived_UpdatesStatistics()
        {
            // New statistics for testing
            var testStats = new Statistics { ActivePlayers = 123, BiggestMultiplier = 456 };
            
            // Using reflection method for test
            var method = typeof(MainWindowViewModel).GetMethod("OnStatisticsReceived", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            method?.Invoke(_viewModel, new object[] { this, testStats });
            
            // CHeck if statistics is updated
            Assert.That(_viewModel.Statistics.ActivePlayers, Is.EqualTo(123));
            Assert.That(_viewModel.Statistics.BiggestMultiplier, Is.EqualTo(456));
        }
        
        [Test]
        public void OnStatisticsReceived_PreservesExistingValuesWhenZero()
        {
            // Sets normal statistics
            var initialStats = new Statistics { ActivePlayers = 100, BiggestMultiplier = 200 };
            _viewModel.Statistics = initialStats;
            
            // Sets statistics with one parameter
            var newStats = new Statistics {BiggestMultiplier = 300 };
            
            var method = typeof(MainWindowViewModel).GetMethod("OnStatisticsReceived", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            method?.Invoke(_viewModel, new object[] { this, newStats });
            
            // Check that only multiplier is updated.
            Assert.That(_viewModel.Statistics.ActivePlayers, Is.EqualTo(100));
            Assert.That(_viewModel.Statistics.BiggestMultiplier, Is.EqualTo(300));
        }
    }
}