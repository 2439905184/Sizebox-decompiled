using System;

namespace SizeboxUI
{
	public struct CommandAction
	{
		public string Text;

		public Action Action;

		public CommandAction(string text, Action action)
		{
			Text = text;
			Action = action;
		}
	}
}
