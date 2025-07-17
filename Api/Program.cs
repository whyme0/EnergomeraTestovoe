using Api.Data;
using Api.Data.Seed;
using Api.Topology;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(o => {
    o.UseInMemoryDatabase("EnergomeraTestovoeDb");
});

builder.Services.AddSingleton<TopologyWorker>();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var topologyWroker = scope.ServiceProvider.GetRequiredService<TopologyWorker>();
    DbInitializer.Seed(context, topologyWroker);
}

app.Run();
