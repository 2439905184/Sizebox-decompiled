using System.Runtime.CompilerServices;
using SizeboxUI;
using UnityEngine;
using UnityEngine.UI;

public class CommandButton : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_0
	{
		public CommandAction command;

		internal void _003CAssignCommand_003Eb__0()
		{
			command.Action();
		}
	}

	[Header("Required References")]
	[SerializeField]
	private Text text;

	[SerializeField]
	private Button button;

	public void AssignCommand(CommandAction command)
	{
		_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = new _003C_003Ec__DisplayClass2_0();
		_003C_003Ec__DisplayClass2_.command = command;
		text.text = _003C_003Ec__DisplayClass2_.command.Text;
		button.onClick.AddListener(_003C_003Ec__DisplayClass2_._003CAssignCommand_003Eb__0);
	}
}
