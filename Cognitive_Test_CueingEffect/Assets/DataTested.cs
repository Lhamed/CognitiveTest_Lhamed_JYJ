using System;

[System.Serializable]

public class DataTested
{
	public	int indexNumber;

	public	string responseTime;

	public	int conditionType;
}

public enum ConditionType
{
	None = 0, Cueing = 1, FakeCueing = 2
}