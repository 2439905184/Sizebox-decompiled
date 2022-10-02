namespace BulletXNA.BulletCollision
{
	public enum eStatus
	{
		Valid = 0,
		Touching = 1,
		Degenerated = 2,
		NonConvex = 3,
		InvalidHull = 4,
		OutOfFaces = 5,
		OutOfVertices = 6,
		AccuraryReached = 7,
		FallBack = 8,
		Failed = 9
	}
}
