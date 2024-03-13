class City
{
    public string Name { get; set; }
    public double LocationX { get; set; }
    public double LocationY { get; set; }

    public City(string name, double locationX, double locationY)
    {
        Name = name;
        LocationX = locationX;
        LocationY = locationY;
    }
}