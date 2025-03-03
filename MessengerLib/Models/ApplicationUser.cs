using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MessengerLib.Models;

[Table("AspNetUsers", Schema = "user")] // Set scheme "user"
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}