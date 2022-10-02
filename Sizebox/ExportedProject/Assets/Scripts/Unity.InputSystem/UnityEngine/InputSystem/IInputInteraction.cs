using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[Preserve]
	public interface IInputInteraction
	{
		void Process(ref InputInteractionContext context);

		void Reset();
	}
	[Preserve]
	public interface IInputInteraction<TValue> : IInputInteraction where TValue : struct
	{
	}
}
