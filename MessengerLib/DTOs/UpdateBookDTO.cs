using Microsoft.AspNetCore.Http;

namespace MessengerLib.DTOs;

public class UpdateBookDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Genre { get; set; }
    public int? Year { get; set; }
    public string? Publisher { get; set; }
    public string? ISBN { get; set; }
    public int? Pages { get; set; }
    public string? Language { get; set; }
    public IFormFile? File { get; set; }
}