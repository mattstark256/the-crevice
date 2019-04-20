public struct StringConversion
{
    public string typedString;
    public string displayedString;

    // Constructor for word that gets modified
    public StringConversion(string typedString, string displayedString)
    {
        this.typedString = typedString;
        this.displayedString = displayedString;
    }

    // Constructor for word that doesn't get modified
    public StringConversion(string s)
    {
        typedString = s;
        displayedString = s;
    }
}