using UnityEngine;

namespace YxModDll.Mod.Features;

public struct Side
{
	public Vector3 Item1;

	public Vector3 Item2;

	public Side(Vector3 item1, Vector3 item2)
	{
		if (item1.NewHashCode() > item2.NewHashCode())
		{
			Item1 = item2;
			Item2 = item1;
		}
		else
		{
			Item1 = item1;
			Item2 = item2;
		}
	}

	public override readonly bool Equals(object obj)
	{
		return obj is Side side && side.Item1 == Item1 && side.Item2 == Item2;
	}

	public override readonly int GetHashCode()
	{
		int num = -1030903623;
		num = num * -1521134295 + Item1.NewHashCode();
		return num * -1521134295 + Item2.NewHashCode();
	}
}
