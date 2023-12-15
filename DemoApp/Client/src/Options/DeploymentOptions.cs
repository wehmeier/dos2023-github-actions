using CommandLine;

namespace FP.GitHubActions.DemoApp.Client.Options;

[Verb("deploy", HelpText = "Deploy a zip archive as website")]
public class DeploymentOptions : ServiceOptions
{
    [Option('t',"token",  Required = true, HelpText = "Access token")]
    public string? Token { get; set; }
    
    [Option('p',"path",  Required = true, HelpText = "Path to zip archive")]
    public string? Path { get; set; }
}