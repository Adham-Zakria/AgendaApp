using BusinessLogicLayer.Services;
using BusinessLogicLayer.ServicesAbstraction;
using DataAccessLayer.Contracts;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


#region Add services to the container

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SwaiqatAgendaAppContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();
builder.Services.AddScoped<IBranchesRepository, BranchesRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUserGroupsRepository, UserGroupsRepository>();

builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IDailyBalanceService, DailyBalanceService>();
builder.Services.AddScoped<IBranchService, BranchService>();


builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
