using FP.GitHubActions.DemoApp.Client.Options;
using FP.GitHubActions.DemoApp.Contract;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace FP.GitHubActions.DemoApp.Client.Commands;

public class DeploymentCommand : ICommand
{
    private readonly DeploymentOptions _options;

    public DeploymentCommand(DeploymentOptions options)
    {
        _options = options;
    }
    
    public async Task ExecuteAsync()
    {
        var channel = GrpcChannel.ForAddress(_options.ServiceUrl);
        var client = new DemoAppServices.DemoAppServicesClient(channel);
        
        var metadata = new Metadata { new("Authentication", _options.Token) };
        var stream = client.SendDeployment(metadata);
        var data = await File.ReadAllBytesAsync(_options.Path);
    
        var byteString = ByteString.CopyFrom(data);

        await stream.RequestStream.WriteAsync(new DeploymentMessage
            { Data = byteString, Timestamp = DateTime.UtcNow.ToTimestamp() });

        await stream.RequestStream.CompleteAsync();

        await Console.Out.WriteLineAsync("Deployment done");
    }
}