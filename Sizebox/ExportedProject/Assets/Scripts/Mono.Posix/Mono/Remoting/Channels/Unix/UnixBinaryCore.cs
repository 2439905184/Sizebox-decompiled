using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixBinaryCore
	{
		private BinaryFormatter _serializationFormatter;

		private BinaryFormatter _deserializationFormatter;

		private bool _includeVersions = true;

		private bool _strictBinding;

		private IDictionary _properties;

		public static UnixBinaryCore DefaultInstance = new UnixBinaryCore();

		public BinaryFormatter Serializer
		{
			get
			{
				return _serializationFormatter;
			}
		}

		public BinaryFormatter Deserializer
		{
			get
			{
				return _deserializationFormatter;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return _properties;
			}
		}

		public UnixBinaryCore(object owner, IDictionary properties, string[] allowedProperties)
		{
			_properties = properties;
			foreach (DictionaryEntry property in properties)
			{
				string text = (string)property.Key;
				if (Array.IndexOf(allowedProperties, text) == -1)
				{
					throw new RemotingException(owner.GetType().Name + " does not recognize '" + text + "' configuration property");
				}
				if (!(text == "includeVersions"))
				{
					if (text == "strictBinding")
					{
						_strictBinding = Convert.ToBoolean(property.Value);
					}
				}
				else
				{
					_includeVersions = Convert.ToBoolean(property.Value);
				}
			}
			Init();
		}

		public UnixBinaryCore()
		{
			_properties = new Hashtable();
			Init();
		}

		public void Init()
		{
			RemotingSurrogateSelector selector = new RemotingSurrogateSelector();
			StreamingContext context = new StreamingContext(StreamingContextStates.Remoting, null);
			_serializationFormatter = new BinaryFormatter(selector, context);
			_deserializationFormatter = new BinaryFormatter(null, context);
			if (!_includeVersions)
			{
				_serializationFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
				_deserializationFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
			}
			if (!_strictBinding)
			{
				_serializationFormatter.Binder = SimpleBinder.Instance;
				_deserializationFormatter.Binder = SimpleBinder.Instance;
			}
		}
	}
}
