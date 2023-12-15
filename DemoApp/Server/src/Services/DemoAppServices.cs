using FP.GitHubActions.DemoApp.Contract;
using FP.GitHubActions.DemoApp.Server.Business;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace FP.GitHubActions.DemoApp.Server.Services;

public class DemoAppServices : Contract.DemoAppServices.DemoAppServicesBase
{
    private readonly AccountRepository _accountRepository;

    public DemoAppServices(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    
    public override Task<GetVersionResponse> GetVersion(Empty request, ServerCallContext context)
    {
        var result = new GetVersionResponse
        {
            Timestamp = DateTime.UtcNow.ToTimestamp(),
            Version = "1.0.0"
        };
        return Task.FromResult(result);
    }

    public override async Task<Empty> SendDeployment(IAsyncStreamReader<DeploymentMessage> requestStream,
        ServerCallContext context)
    {
        var token = GetAuthenticationToken(context);

        if (!_accountRepository.TryAuthenticate(token, out var accountId))
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Permission denied"));
        }

        try
        {
            await Task.Run(
                async () =>
                {
                    var cancellationToken = context.CancellationToken;
                    using (var ms = new MemoryStream())
                    {
                        await foreach (var deploymentMessage in requestStream.ReadAllAsync(cancellationToken))
                        {
                            await ms.WriteAsync(deploymentMessage.Data.ToByteArray(), cancellationToken);
                        }

                        await ms.FlushAsync(cancellationToken);
                        await _accountRepository.SaveDeployment(accountId, ms.ToArray());
                    }
                }).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch
        {
            throw;
        }
        return new Empty();
    }

    private string? GetAuthenticationToken(ServerCallContext context)
    {
        var entry = context.RequestHeaders.FirstOrDefault(x =>
            string.Equals(x.Key, "authentication", StringComparison.OrdinalIgnoreCase));
        return entry?.Value;
    }
}