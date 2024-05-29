using Ecommetriya.API.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ReportDetail> ReportDetails { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<CardTag> CardsTags { get; set; }
    public DbSet<CardPhoto> CardsPhotos { get; set; }
    public DbSet<CardSize> CardsSizes { get; set; }
    public DbSet<Feed> Feeds { get; set; }
    public DbSet<CardDispatched> CardsDispatched { get; set; }
    public DbSet<CardPurchased> CardsPurchaed { get; set; }
    public DbSet<CardReturn> CardsReturns { get; set; }
    public DbSet<CardNullStatus> CardsNullStatus { get; set; }
    public DbSet<Advert> Adverts { get; set; }
    public DbSet<AdvertStatistic> AdvertsStatistics { get; set; }
    public DbSet<Competitor> Competitors { get; set; }
    public DbSet<CompetitorPhoto> CompetitorsPhotos { get; set; }
    public DbSet<CompetitorStatistic> CompetitorsStatistics { get; set; }
}