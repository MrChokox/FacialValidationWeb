using ValidacaoFacialWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IPessoaRepository>(
    new PessoaRepositoryPasta(Path.Combine(AppContext.BaseDirectory, "fotos")));

builder.Services.AddSingleton(sp => new ServicoReconhecimento(
    Path.Combine(AppContext.BaseDirectory, "modelos_dlib"),
    sp.GetRequiredService<IPessoaRepository>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Reconhecimento}/{action=Index}/{id?}");


app.Run();
