using CommandLine;
using FP.GitHubActions.DemoApp.Client.Commands;
using FP.GitHubActions.DemoApp.Client.Options;

async Task<int> ExecuteCommandAsync(ICommand command)
{
    try
    {
        await command.ExecuteAsync();

        return 0;
    }
    catch (Exception ex)
    {
        await Console.Error.WriteLineAsync(ex.ToString());
        return 1;
    }
}

await Task.Delay(2500);

return await Parser.Default
    .ParseArguments<VersionOptions, DeploymentOptions>(args)
    .MapResult(
        (VersionOptions options) => ExecuteCommandAsync(new VersionCommand(options)),
        (DeploymentOptions options) => ExecuteCommandAsync(new DeploymentCommand(options)),
        err => Task.FromResult(1)
    );
