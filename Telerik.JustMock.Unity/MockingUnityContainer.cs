using Microsoft.Practices.Unity;

namespace Telerik.JustMock.Unity
{
	/// <summary>
	/// Mocking container that works similarly to the built-in MockingContainer class.
	/// </summary>
	/// <typeparam name="T">The type whose dependencies will be mocked.</typeparam>
	public class MockingUnityContainer<T> : UnityContainer where T : class
	{
		private T instance;

		/// <summary>
		/// Constructs a new mocking container.
		/// </summary>
		public MockingUnityContainer()
		{
			this.RegisterType<T>();
		}

		/// <summary>
		/// The instance with satisfied dependencies. The instance is created on first access.
		/// </summary>
		public T Instance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = this.Resolve<T>();
				}
				return this.instance;
			}
		}
	}
}