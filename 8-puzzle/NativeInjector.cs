using Application.Services;

namespace _8_puzzle;

public static class NativeInjector
{
    public static void Register(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPuzzleSolverService, PuzzleSolverService>();
    }
}