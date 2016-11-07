﻿using System;
using System.Threading.Tasks;
using RawRabbit.Serialization;

namespace RawRabbit.Pipe.Middleware
{
	public class MessageSerializationOptions
	{
		public Func<IPipeContext, object> MessageFunc { get; set; }

		public static MessageSerializationOptions For(Func<IPipeContext, object> func)
		{
			return new MessageSerializationOptions
			{
				MessageFunc = func
			};
		}
	}

	public class MessageSerializationMiddleware : Middleware
	{
		private readonly ISerializer _serializer;
		private readonly Func<IPipeContext, object> _msgFunc;

		public MessageSerializationMiddleware(ISerializer serializer, MessageSerializationOptions options = null)
		{
			_serializer = serializer;
			_msgFunc = options?.MessageFunc ?? (context => context.GetMessage());
		}

		public override Task InvokeAsync(IPipeContext context)
		{
			var message = _msgFunc(context);
			var serialized = _serializer.Serialize(message);
			context.Properties.Add(PipeKey.SerializedMessage, serialized);
			return Next.InvokeAsync(context);
		}
	}
}
