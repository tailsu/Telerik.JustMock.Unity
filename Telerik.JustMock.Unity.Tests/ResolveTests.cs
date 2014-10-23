using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Telerik.JustMock.Unity.Tests
{
	[TestClass]
	public class ResolveTests
	{
		[TestMethod]
		public void ResolveMocksWithoutRegistration_ImplicitMocksCreated()
		{
			var container = new UnityContainer();
			container.EnableMocking();
			var greeter = container.RegisterType<Greeter>().Resolve<Greeter>();

			greeter.Greet();

			var logger = container.Resolve<ILogger>();
			Mock.Assert(() => logger.Log("0: "), Occurs.Once());
		}

		[TestMethod]
		public void ResolveMocksByName_ImplicitMocksCreated()
		{
			var container = new UnityContainer();
			container.EnableMocking();

			var fatal = container.Resolve<ILogger>("fatal");
			var trace = container.Resolve<ILogger>("trace");
			Assert.AreNotSame(fatal, trace);
		}

		[TestMethod]
		public void ArrangeMocks_MocksCreatedAndArranged()
		{
			var container = new UnityContainer();
			string result = null;
			container.Arrange<ILogger>(x => x.Log(Arg.AnyString)).DoInstead((string msg) => result = msg).MustBeCalled();
			container.ArrangeLike<IMessage>(x => x.Message == "foo");
			container.ArrangeLike<ICounter>(x => x.Next == 5);

			var greeter = container.RegisterType<Greeter>().Resolve<Greeter>();
			greeter.Greet();
			Assert.AreEqual("5: foo", result);
		}

		[TestMethod]
		public void AssertContainer_ExceptionThrownForUnmetExplicitExpectations()
		{
			var container = new UnityContainer();
			container.Arrange<IMessage>(x => x.Message).MustBeCalled();

			AssertEx.Throws<AssertFailedException>(() => container.Assert());
		}

		[TestMethod]
		public void AssertContainer_ExceptionThrownForUnmetImplicitExpectations()
		{
			var container = new UnityContainer();
			container.Arrange<IMessage>(x => x.Message).Returns("abc");

			AssertEx.Throws<AssertFailedException>(() => container.AssertAll());
		}

		public interface ILogger
		{
			void Log(string message);
		}

		public interface IMessage
		{
			string Message { get; }
		}

		public interface ICounter
		{
			int Next { get; }
		}

		public class Greeter
		{
			private ILogger logger;
			private IMessage message;
			private ICounter counter;

			public Greeter(ILogger logger, IMessage message, ICounter counter)
			{
				this.logger = logger;
				this.message = message;
				this.counter = counter;
			}

			public void Greet()
			{
				this.logger.Log(string.Format("{0}: {1}", this.counter.Next, this.message.Message));
			}
		}
	}
}
