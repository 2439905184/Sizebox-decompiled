using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ProceduralCityGenerator
{
	public class CityDestructionManager : MonoBehaviour
	{
		private readonly LinkedList<CityBuilding> mTrackingBuildings = new LinkedList<CityBuilding>();

		private const int cTreshHold = 8;

		public CityPopulationManager CityManager;

		public void RegisterBuilding(CityBuilding building)
		{
			mTrackingBuildings.AddLast(building);
			if (mTrackingBuildings.Count > 8)
			{
				mTrackingBuildings.First.Value.Collapse();
				mTrackingBuildings.RemoveFirst();
			}
			if (CityManager != null)
			{
				CityManager.RemoveBuilding(building);
			}
		}

		public void UnregisterBuilding(CityBuilding building)
		{
			mTrackingBuildings.Remove(building);
		}
	}
}
