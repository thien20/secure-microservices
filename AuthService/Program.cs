using AuthService;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(5000); // HTTP
//     options.ListenAnyIP(5001, listenOptions =>
//     {
//         // take cert from https://letsencrypt.org/
//         listenOptions.UseHttps("path/to/certificate.pfx", "your_password");
//     });
// });


var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, builder.Environment);

app.Run();


