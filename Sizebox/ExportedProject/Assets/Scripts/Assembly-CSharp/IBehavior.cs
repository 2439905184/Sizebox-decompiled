using System.Collections.Generic;
using UnityEngine;

public interface IBehavior
{
	IBehaviorInstance CreateInstance(EntityBase agent, EntityBase target, Vector3 cursorPoint);

	bool CanAppearInBehaviorManager();

	bool IsReactive();

	bool IsHidden();

	string GetText();

	string GetName();

	bool IsSecondary();

	bool IsEnabled();

	bool IsAI();

	bool CanUseAI();

	List<string> GetFlags();

	EntityDef GetAgentDef();

	EntityDef GetTargetDef();
}
