using System.IO.Compression;

namespace FP.GitHubActions.DemoApp.Server.Business;

public class AccountRepository
{
    private AccountConfiguration[] Accounts { get; init; }
    
    private Dictionary<Guid, byte[]> DeploymentCache { get; } = new();

    private string DeploymentPath { get; init; }
    

    public AccountRepository(IConfiguration configuration)
    {
        var accounts =configuration.GetSection("Accounts").Get<AccountConfiguration[]>();
        Accounts = accounts ?? Array.Empty<AccountConfiguration>();
        DeploymentPath = configuration["DeploymentPath"];
    }

    public bool AccountExits(Guid accountId)
    {
        return Accounts.Any(x => x.Id == accountId);
    }

    public bool TryAuthenticate(string token, out Guid accountId)
    {
        var account = Accounts.FirstOrDefault(x => x.Token == token);

        if (account == null)
        {
            accountId = Guid.Empty;
            return false;
        }

        accountId = account.Id;
        return true;
    }

    public async Task SaveDeployment(Guid account, byte[] data)
    {
        if (!Directory.Exists(DeploymentPath))
        {
            Directory.CreateDirectory(DeploymentPath);
        }

        var fileName = $"{account:N}-{DateTime.UtcNow:yyyyMMdd-hhmmss}.zip";
        var savePath = Path.Combine(DeploymentPath, fileName);
        await File.WriteAllBytesAsync(savePath, data);
        DeploymentCache[account] = data;
    }

    public async Task<byte[]> GetFileAsync(Guid accountId, string file)
    {
        if (!DeploymentCache.ContainsKey(accountId))
        {
            await LoadDeploymentByAccount(accountId);
        }

        if (!DeploymentCache.ContainsKey(accountId))
        {
            throw new FileNotFoundException($"File {file} not found");
        }

        var data = DeploymentCache[accountId];

        using var msZipData = new MemoryStream(data);
        using var zip = new ZipArchive(msZipData);
        foreach (var entry in zip.Entries)
        {
            if (entry.Name != file)
            {
                continue;
            }

            using var msTarget = new MemoryStream();
            await using (var zipEntryStream = entry.Open())
            {
                await zipEntryStream.CopyToAsync(msTarget);
            }

            await msTarget.FlushAsync();
            return msTarget.ToArray();

        }
        throw new FileNotFoundException($"File {file} not found");
    }

    private async Task LoadDeploymentByAccount(Guid accountId)
    {
        var deploymentDirectory = new DirectoryInfo(DeploymentPath);
        if (!deploymentDirectory.Exists)
        {
            return;
        }

        var files = deploymentDirectory.GetFiles();
        var filesByAccount = files.Where(x => x.Name.StartsWith(accountId.ToString("N"))).OrderByDescending(x=>x.LastWriteTime);
        var latestFile = filesByAccount.FirstOrDefault();
        if (latestFile != null)
        {
            DeploymentCache[accountId] = await File.ReadAllBytesAsync(latestFile.FullName);
        }
    }
}