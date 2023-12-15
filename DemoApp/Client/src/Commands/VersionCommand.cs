using FP.GitHubActions.DemoApp.Client.Options;
using FP.GitHubActions.DemoApp.Contract;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

namespace FP.GitHubActions.DemoApp.Client.Commands;

public class VersionCommand : ICommand
{
    private readonly VersionOptions _options;

    public VersionCommand(VersionOptions options)
    {
        _options = options;
    }
    
    public async Task ExecuteAsync()
    {
        var channel = GrpcChannel.ForAddress(_options.ServiceUrl);
        var client = new DemoAppServices.DemoAppServicesClient(channel);
        
        var version = await client.GetVersionAsync(new Empty());
        await Console.Out.WriteLineAsync($"Current server version is {version.Version}");
    }
}