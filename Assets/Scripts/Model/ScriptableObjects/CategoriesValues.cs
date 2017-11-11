public enum Category {
	Unknown, Religion, Drugs, Gays, Money, Sexism, Racism, Alcohol
}

[System.Serializable]
public struct CategoryValue
{
	public Category CategoryName;
	public int Value;
}