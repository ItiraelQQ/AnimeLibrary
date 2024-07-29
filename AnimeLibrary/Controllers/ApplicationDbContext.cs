using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AnimeLibrary.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<News> News { get; set; }
}

public class ApplicationUser : IdentityUser
{
    public string ProfilePictureUrl { get; set; }
    public string Bio { get; set; }
    public List<int> ViewedAnimes { get; set; } = new List<int>();
    public List<int> ViewedMangas { get; set; } = new List<int>();

}
