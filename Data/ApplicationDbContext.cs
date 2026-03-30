using ApiNotificaciones.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiTalentoHumano.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public DbSet<NotificacionRabbitModel> NOTIFICACIONRABBIT { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificacionRabbitModel>(entity =>
            {
                entity.ToTable("NOTIFICACION_RABBIT", schema: "DATA_USR");
                entity.HasKey(x => new { x.NOTR_ID });

            });

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
            optionsBuilder.UseOracle(_configuration.GetConnectionString("TESPIG11G")); // OracleConnexion  TESPIG11G
            
        }
    }
}
