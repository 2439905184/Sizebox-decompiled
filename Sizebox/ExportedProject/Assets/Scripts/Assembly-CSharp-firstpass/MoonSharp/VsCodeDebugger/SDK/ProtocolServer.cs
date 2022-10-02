using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization.Json;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public abstract class ProtocolServer
	{
		public bool TRACE;

		public bool TRACE_RESPONSE;

		protected const int BUFFER_SIZE = 4096;

		protected const string TWO_CRLF = "\r\n\r\n";

		protected static readonly Regex CONTENT_LENGTH_MATCHER = new Regex("Content-Length: (\\d+)");

		protected static readonly Encoding Encoding = Encoding.UTF8;

		private int _sequenceNumber;

		private Stream _outputStream;

		private ByteBuffer _rawData;

		private int _bodyLength;

		private bool _stopRequested;

		public ProtocolServer()
		{
			_sequenceNumber = 1;
			_bodyLength = -1;
			_rawData = new ByteBuffer();
		}

		public void ProcessLoop(Stream inputStream, Stream outputStream)
		{
			_outputStream = outputStream;
			byte[] array = new byte[4096];
			_stopRequested = false;
			while (!_stopRequested)
			{
				int num = inputStream.Read(array, 0, array.Length);
				if (num != 0)
				{
					if (num > 0)
					{
						_rawData.Append(array, num);
						ProcessData();
					}
					continue;
				}
				break;
			}
		}

		public void Stop()
		{
			_stopRequested = true;
		}

		public void SendEvent(Event e)
		{
			SendMessage(e);
		}

		protected abstract void DispatchRequest(string command, Table args, Response response);

		private void ProcessData()
		{
			while (true)
			{
				if (_bodyLength >= 0)
				{
					if (_rawData.Length >= _bodyLength)
					{
						byte[] bytes = _rawData.RemoveFirst(_bodyLength);
						_bodyLength = -1;
						Dispatch(Encoding.GetString(bytes));
						continue;
					}
					break;
				}
				string @string = _rawData.GetString(Encoding);
				int num = @string.IndexOf("\r\n\r\n");
				if (num != -1)
				{
					Match match = CONTENT_LENGTH_MATCHER.Match(@string);
					if (match.Success && match.Groups.Count == 2)
					{
						_bodyLength = Convert.ToInt32(match.Groups[1].ToString());
						_rawData.RemoveFirst(num + "\r\n\r\n".Length);
						continue;
					}
					break;
				}
				break;
			}
		}

		private void Dispatch(string req)
		{
			try
			{
				Table table = JsonTableConverter.JsonToTable(req);
				if (table != null && table["type"].ToString() == "request")
				{
					if (TRACE)
					{
						Console.Error.WriteLine(string.Format("C {0}: {1}", table["command"], req));
					}
					Response response = new Response(table);
					DispatchRequest(table.Get("command").String, table.Get("arguments").Table, response);
					SendMessage(response);
				}
			}
			catch
			{
			}
		}

		protected void SendMessage(ProtocolMessage message)
		{
			message.seq = _sequenceNumber++;
			if (TRACE_RESPONSE && message.type == "response")
			{
				Console.Error.WriteLine(string.Format(" R: {0}", JsonTableConverter.ObjectToJson(message)));
			}
			if (TRACE && message.type == "event")
			{
				Event @event = (Event)message;
				Console.Error.WriteLine(string.Format("E {0}: {1}", @event.@event, JsonTableConverter.ObjectToJson(@event.body)));
			}
			byte[] array = ConvertToBytes(message);
			try
			{
				_outputStream.Write(array, 0, array.Length);
				_outputStream.Flush();
			}
			catch (Exception)
			{
			}
		}

		private static byte[] ConvertToBytes(ProtocolMessage request)
		{
			string s = JsonTableConverter.ObjectToJson(request);
			byte[] bytes = Encoding.GetBytes(s);
			string s2 = string.Format("Content-Length: {0}{1}", bytes.Length, "\r\n\r\n");
			byte[] bytes2 = Encoding.GetBytes(s2);
			byte[] array = new byte[bytes2.Length + bytes.Length];
			Buffer.BlockCopy(bytes2, 0, array, 0, bytes2.Length);
			Buffer.BlockCopy(bytes, 0, array, bytes2.Length, bytes.Length);
			return array;
		}
	}
}
