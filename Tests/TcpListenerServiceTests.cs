using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Models;
using Moq;
using Serilog;
using Services;

namespace Tests
{
    [TestFixture]
    public class TcpListenerServiceTests
    {
        private Mock<ILogger> _loggerMock;
        private TcpListenerService _service;
        private TestDispatcher _testDispatcher;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _testDispatcher = new TestDispatcher();
            _service = new TcpListenerService(_loggerMock.Object, _testDispatcher);
        }

        /// <summary>
        /// Test TCP message processing, if activePlayers and biggestMultiplier json is valid.
        /// </summary>
        [Test]
        public void ProcessMessage_ValidJson_InvokesStatisticsReceived()
        {
            bool eventRaised = false;
            var testStats = new Statistics { ActivePlayers = 10, BiggestMultiplier = 5 };
            string jsonMessage = JsonSerializer.Serialize(testStats);
            
            _service.StatisticsReceived += (sender, stats) => {
                eventRaised = true;
                Assert.That(stats.ActivePlayers, Is.EqualTo(10));
                Assert.That(stats.BiggestMultiplier, Is.EqualTo(5));
            };
            
            // Using reflection method to test event in private method
            var method = typeof(TcpListenerService).GetMethod("ProcessMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            method.Invoke(_service, new object[] { jsonMessage });
            
            Assert.That(eventRaised, Is.True, "StatisticsReceived event was not raised");
        }
        
        /// <summary>
        /// Test TCP message processing, if activePlayers and biggestMultiplier json wrong (string not int).
        /// </summary>
        [Test]
        public void ProcessMessage_InvalidJson_DoesNotRaiseEvent()
        {
            bool eventRaised = false;
            _service.StatisticsReceived += (sender, stats) => eventRaised = true;
    
            string invalidJson = "{\"ActivePlayers\": \"not a number\", \"BiggestMultiplier\": \"not a number\"}";
    
            // Using reflection method to test event in private method
            var method = typeof(TcpListenerService).GetMethod("ProcessMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    
            method.Invoke(_service, new object[] { invalidJson });
            
            Assert.That(eventRaised, Is.False, "StatisticsReceived event should not be raised for invalid JSON");
        }
        
        /// <summary>
        /// Test TCP message processing. Message is empty json.
        /// </summary>
        [Test]
        public void ProcessMessage_EmptyString_DoesNotRaiseEvent()
        {
            bool eventRaised = false;
            _service.StatisticsReceived += (sender, stats) => eventRaised = true;
    
            string emptyJson = "{}";
    
            // Using reflection method to test event in private method
            var method = typeof(TcpListenerService).GetMethod("ProcessMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    
            method.Invoke(_service, new object[] { emptyJson });
            
            Assert.That(eventRaised, Is.False, "StatisticsReceived event should not be raised for invalid JSON");
        }

        /// <summary>
        /// Test TCP message processing. Only players statistics is given.
        /// </summary>
        [Test]
        public void ProcessMessage_OnlyPlayersStatistic_InvokesStatisticsReceived()
        {
            bool eventRaised = false;
            var testStats = new Statistics { ActivePlayers = 10};
            string jsonMessage = JsonSerializer.Serialize(testStats);
            
            _service.StatisticsReceived += (sender, stats) => {
                eventRaised = true;
                Assert.That(stats.ActivePlayers, Is.EqualTo(10));
            };
            
            // Using reflection method to test event in private method
            var method = typeof(TcpListenerService).GetMethod("ProcessMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            method.Invoke(_service, new object[] { jsonMessage });
            
            Assert.That(eventRaised, Is.True, "StatisticsReceived event was not raised");
        }
        
        /// <summary>
        /// Test TCP message processing. Only multiplier statistics is given.
        /// </summary>
        [Test]
        public void ProcessMessage_OnlyMultiplierStatistic_InvokesStatisticsReceived()
        {
            bool eventRaised = false;
            var testStats = new Statistics {BiggestMultiplier = 5};
            string jsonMessage = JsonSerializer.Serialize(testStats);
            
            _service.StatisticsReceived += (sender, stats) => {
                eventRaised = true;
                Assert.That(stats.BiggestMultiplier, Is.EqualTo(5));
            };
            
            // Using reflection method to test event in private method
            var method = typeof(TcpListenerService).GetMethod("ProcessMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            method.Invoke(_service, new object[] { jsonMessage });
            
            Assert.That(eventRaised, Is.True, "StatisticsReceived event was not raised");
        }
        
        /// <summary>
        /// Test TCP message processing. Players statistics and some wrong json are given.
        /// </summary>
        [Test]
        public void ProcessMessage_PlayerStatisticsAndWrongJsonParameterAreSent_InvokesPlayerStatisticsReceived()
        {
            bool eventRaised = false;
            
            var mixedObject = new
            {
                ActivePlayers = 5,
                UnknownProperty = "test",
                RandomValue = 42,
                AnotherObject = new { Name = "Test" }
            };
    
            string jsonMessage = JsonSerializer.Serialize(mixedObject);
    
            _service.StatisticsReceived += (sender, stats) => {
                eventRaised = true;
                Assert.That(stats.ActivePlayers, Is.EqualTo(5));
            };
    
            // Using reflection method to test event in private method
            var method = typeof(TcpListenerService).GetMethod("ProcessMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    
            method.Invoke(_service, new object[] { jsonMessage });
    
            Assert.That(eventRaised, Is.True, "StatisticsReceived event was not raised");
        }
        
        /// <summary>
        /// Test TCP message processing. Multiplier statistics and some wrong json are given.
        /// </summary>
        [Test]
        public void ProcessMessage_MultiplierStatisticsAndWrongJsonParameterAreSent_InvokesPlayerStatisticsReceived()
        {
            bool eventRaised = false;
            
            var mixedObject = new
            {
                BiggestMultiplier = 5,
                UnknownProperty = "test",
                RandomValue = 42,
                AnotherObject = new { Name = "Test" }
            };
    
            string jsonMessage = JsonSerializer.Serialize(mixedObject);
    
            _service.StatisticsReceived += (sender, stats) => {
                eventRaised = true;
                Assert.That(stats.BiggestMultiplier, Is.EqualTo(5));
            };
    
            // Using reflection method to test event in private method
            var method = typeof(TcpListenerService).GetMethod("ProcessMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    
            method.Invoke(_service, new object[] { jsonMessage });
    
            Assert.That(eventRaised, Is.True, "StatisticsReceived event was not raised");
        }
        
        /// <summary>
        /// Test TCP message processing. Multiplier and players statistics with some wrong json are given.
        /// </summary>
        [Test]
        public void ProcessMessage_FullStatisticsAndWrongJsonParameterAreSent_InvokesPlayerStatisticsReceived()
        {
            bool eventRaised = false;
            
            var mixedObject = new
            {
                BiggestMultiplier = 1000,
                ActivePlayers = 10,
                UnknownProperty = "test",
                RandomValue = 42,
                AnotherObject = new { Name = "Test" }
            };
    
            string jsonMessage = JsonSerializer.Serialize(mixedObject);
    
            _service.StatisticsReceived += (sender, stats) => {
                eventRaised = true;
                Assert.That(stats.BiggestMultiplier, Is.EqualTo(1000));
                Assert.That(stats.ActivePlayers, Is.EqualTo(10));
            };
    
            // Using reflection method to test event in private method
            var method = typeof(TcpListenerService).GetMethod("ProcessMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    
            method.Invoke(_service, new object[] { jsonMessage });
    
            Assert.That(eventRaised, Is.True, "StatisticsReceived event was not raised");
        }
        
        
        /// <summary>
        /// Test TCP message receiving throw port 4948.
        /// </summary>
        [Test]
        [Ignore("Ignored because makes conflict with other tests")]
        public async Task TcpListener_ReceivesValidMessage_RaisesEvent()
        {
            // Arrange
            var testStats = new Statistics { ActivePlayers = 42, BiggestMultiplier = 8 };
            var eventReceived = new TaskCompletionSource<Statistics>();
            var testDispatcher = new TestDispatcher();
            var service = new TcpListenerService(dispatcher: testDispatcher);
    
            // Listener
            service.StatisticsReceived += (sender, stats) => {
                eventReceived.SetResult(stats);
            };
    
            // Service
            service.Start();
            await Task.Delay(500);
    
            try
            {
                // Act - send message TCP
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync("localhost", 4948);
                    using (var stream = client.GetStream())
                    {
                        string jsonMessage = JsonSerializer.Serialize(testStats);
                        byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
                        await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                    }
                }
                
                var timeoutTask = Task.Delay(5000);
                var completedTask = await Task.WhenAny(eventReceived.Task, timeoutTask);
        
                // Assert
                Assert.That(completedTask, Is.Not.EqualTo(timeoutTask), "Timeout waiting for event");
        
                if (completedTask != timeoutTask)
                {
                    var receivedStats = await eventReceived.Task;
                    Assert.That(receivedStats.ActivePlayers, Is.EqualTo(testStats.ActivePlayers));
                    Assert.That(receivedStats.BiggestMultiplier, Is.EqualTo(testStats.BiggestMultiplier));
                }
            }
            finally
            {
                // END
                service.Stop();
            }
        }
    }
}