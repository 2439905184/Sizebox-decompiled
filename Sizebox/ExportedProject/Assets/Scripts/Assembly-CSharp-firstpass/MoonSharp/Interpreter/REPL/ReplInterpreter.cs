using System;

namespace MoonSharp.Interpreter.REPL
{
	public class ReplInterpreter
	{
		private Script m_Script;

		private string m_CurrentCommand = string.Empty;

		public bool HandleDynamicExprs { get; set; }

		public bool HandleClassicExprsSyntax { get; set; }

		public virtual bool HasPendingCommand
		{
			get
			{
				return m_CurrentCommand.Length > 0;
			}
		}

		public virtual string CurrentPendingCommand
		{
			get
			{
				return m_CurrentCommand;
			}
		}

		public virtual string ClassicPrompt
		{
			get
			{
				if (!HasPendingCommand)
				{
					return ">";
				}
				return ">>";
			}
		}

		public ReplInterpreter(Script script)
		{
			m_Script = script;
		}

		public virtual DynValue Evaluate(string input)
		{
			bool flag = !HasPendingCommand;
			bool flag2 = input == "";
			m_CurrentCommand += input;
			if (m_CurrentCommand.Length == 0)
			{
				return DynValue.Void;
			}
			m_CurrentCommand += "\n";
			try
			{
				DynValue dynValue = null;
				if (flag && HandleClassicExprsSyntax && m_CurrentCommand.StartsWith("="))
				{
					m_CurrentCommand = "return " + m_CurrentCommand.Substring(1);
				}
				if (flag && HandleDynamicExprs && m_CurrentCommand.StartsWith("?"))
				{
					string code = m_CurrentCommand.Substring(1);
					dynValue = m_Script.CreateDynamicExpression(code).Evaluate();
				}
				else
				{
					DynValue function = m_Script.LoadString(m_CurrentCommand, null, "stdin");
					dynValue = m_Script.Call(function);
				}
				m_CurrentCommand = "";
				return dynValue;
			}
			catch (SyntaxErrorException ex)
			{
				if (flag2 || !ex.IsPrematureStreamTermination)
				{
					m_CurrentCommand = "";
					ex.Rethrow();
					throw;
				}
				return null;
			}
			catch (ScriptRuntimeException ex2)
			{
				m_CurrentCommand = "";
				ex2.Rethrow();
				throw;
			}
			catch (Exception)
			{
				m_CurrentCommand = "";
				throw;
			}
		}
	}
}
