using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MessengerLib.DTOs;

public class CreateBookDTO
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Author { get; set; } = string.Empty;

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public int Year { get; set; }

    [Required]
    public string Publisher { get; set; } = string.Empty;

    [Required]
    public string ISBN { get; set; } = string.Empty;

    [Required]
    public int Pages { get; set; }

    [Required]
    public string Language { get; set; } = string.Empty;

    public IFormFile? File { get; set; }
}