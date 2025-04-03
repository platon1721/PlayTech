using Moq;
using Serilog;
using Services;
using ViewModels;
using ITimer = Services.ITimer;



namespace Tests

{
    [TestFixture]
    public class NotificationTimerTests
    {
        private Mock<ILogger> _loggerMock;
        private TestDispatcher _testDispatcher;
        private TestTimer _testTimer;
        private MainWindowViewModel _viewModel;
        
        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _testDispatcher = new TestDispatcher();
            _testTimer = new TestTimer();
            
            Func<ITimer> timerFactory = () => _testTimer;
            
            _viewModel = new MainWindowViewModel(_loggerMock.Object, _testDispatcher, timerFactory);
        }
        
        [Test]
        public void ShowNotification_MakesNotificationVisible()
        {
            // Check if notification is visible before command.
            Assert.That(_viewModel.IsNotificationVisible, Is.False);
            
            // Run the command.
            _viewModel.ShowNotificationCommand.Execute(null);
            
            // Check if notification is visible after command
            Assert.That(_viewModel.IsNotificationVisible, Is.True);
            Assert.That(_viewModel.NotificationText, Is.Not.Empty);
            
            // Check if timer is running
            Assert.That(_testTimer.IsRunning, Is.True);
            Assert.That(_testTimer.Interval, Is.EqualTo(TimeSpan.FromSeconds(5)));
        }
        
        [Test]
        public void NotificationTimer_HidesNotificationWhenFired()
        {
            // Run the command.
            _viewModel.ShowNotificationCommand.Execute(null);
            
            // Check if notification is visible after command.
            Assert.That(_viewModel.IsNotificationVisible, Is.True);
            
            // Time runs out
            _testTimer.FireTick();
            
            // Check if notification is visible after time out.
            Assert.That(_viewModel.IsNotificationVisible, Is.False);
            
            // Check if timer is stopped.
            Assert.That(_testTimer.IsRunning, Is.False);
        }
    }
}