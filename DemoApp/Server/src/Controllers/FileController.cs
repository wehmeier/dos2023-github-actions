using System.IO.Compression;
using FP.GitHubActions.DemoApp.Server.Business;
using Microsoft.AspNetCore.Mvc;

namespace FP.GitHubActions.DemoApp.Server.Controllers;

public class FileController
{
    private readonly AccountRepository _accountRepository;

    public FileController(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    
    [HttpGet("~/{account:guid}/{file}")]
    public async Task<IActionResult> GetFile([FromRoute] Guid account, [FromRoute] string file)
    {
        try
        {
            if (!_accountRepository.AccountExits(account))
            {
                return new BadRequestResult();
            }

            var data = await _accountRepository.GetFileAsync(account, file);
            var extension = Path.GetExtension(file).ToLowerInvariant()[1..];
            switch (extension)
            {
                case "html":
                case "htm":
                    return new FileContentResult(data, "text/html");
                case "png":
                    return new FileContentResult(data, "image/png");
                case "jpeg":
                case "jpg":
                    return new FileContentResult(data, "image/jpeg");
                case "js":
                    return new FileContentResult(data, "text/javascript ");
                case "css":
                    return new FileContentResult(data, "text/css");
                default:
                    return new FileContentResult(data, "application/octet-stream");
            }
        }
        catch (FileNotFoundException e)
        {
            return new NotFoundResult();
        }

    }
}
