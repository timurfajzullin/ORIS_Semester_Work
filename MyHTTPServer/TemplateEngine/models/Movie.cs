namespace MyHttttpServer.Models;

public class Movie
{
    public string PosterURl { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Genre { get; set; }
    public string Starring { get; set; }
    
    public string Country { get; set; }
    
    public string Production{get; set;}
    public string Privacy { get; set; }
    public string Year { get; set; }
}