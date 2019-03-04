namespace ShoppingCart.HttpClients.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class UnexpectedHttpResponseException : Exception
	{
		public UnexpectedHttpResponseException()
		{
		}

		public UnexpectedHttpResponseException(string message)
			: base(message)
		{
		}

		public UnexpectedHttpResponseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UnexpectedHttpResponseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}