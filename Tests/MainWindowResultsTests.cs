using Models;
using Moq;
using Serilog;
using ViewModels;

namespace Tests
{
    [TestFixture]
    public class ResultsTests
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
        public void Constructor_InitializesResultsProperty()
        {
            Assert.That(_viewModel.Results, Is.Not.Null);
            Assert.That(_viewModel.AddRandomResultCommand, Is.Not.Null);
            Assert.That(_viewModel.ShowNotificationCommand, Is.Not.Null);
            Assert.That(_viewModel.IsNotificationVisible, Is.False);
        }
        
        [Test]
        public void AddRandomResultCommand_AddsNewResult()
        {
            var resultTracker = new ResultTracker();
            
            // Reflection method to use private function
            var vmType = typeof(MainWindowViewModel);
            var resultTrackerField = vmType.GetField("_resultTracker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            resultTrackerField?.SetValue(_viewModel, resultTracker);
            
            // Check if after initialisation parameter results is empty.
            Assert.That(_viewModel.Results.Count, Is.EqualTo(0));
            
            // Use AddRandomResult to add new result.
            var methodInfo = vmType.GetMethod("AddRandomResult", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo?.Invoke(_viewModel, null);
            
            // Check if after AddRandomResult command there is 1 new result added
            Assert.That(_viewModel.Results.Count, Is.EqualTo(1), "After first result added must be only one result");
            
            // Use AddRandomResultCommand to add new result.
            _viewModel.AddRandomResultCommand.Execute(null);
            
            // Check if one more new result is being added.
            Assert.That(_viewModel.Results.Count, Is.EqualTo(2), "After second result added must be only two result");
        }
        
        [Test]
        public void ResultTrackerInitialization_LimitsToTenResults()
        {
            // Add 15 new results
            for (int i = 0; i < 15; i++)
            {
                _viewModel.AddRandomResultCommand.Execute(null);
            }
            
            // Check if max results are 10
            Assert.That(_viewModel.Results.Count, Is.LessThanOrEqualTo(10));
        }
    }
}