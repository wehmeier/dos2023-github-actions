namespace FP.GitHubActions.DemoApp.Client.Commands;

public interface ICommand
{
    Task ExecuteAsync();
}