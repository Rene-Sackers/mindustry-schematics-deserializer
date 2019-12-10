using System;

namespace MindustrySchematics.Deserializer.Models
{
	public enum DeserializationExceptionReason
	{
		InvalidBase64,
		MissingMschHeader
	}

	public class DeserializationException : Exception
	{
		public DeserializationExceptionReason ExceptionReason { get; }

		public DeserializationException(
			DeserializationExceptionReason deserializationExceptionReason,
			string message = null,
			Exception innerException = null) : base(message, innerException)
		{
			ExceptionReason = deserializationExceptionReason;
		}
	}
}
