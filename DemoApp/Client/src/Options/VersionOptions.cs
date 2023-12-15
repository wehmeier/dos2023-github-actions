using CommandLine;

namespace FP.GitHubActions.DemoApp.Client.Options;

[Verb("server-version", HelpText = "Request the service version")]
public class VersionOptions : ServiceOptions
{
    
}