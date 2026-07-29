namespace ProjectBase.Services;

public interface IPracticeCreationFaultInjector
{
    Task AfterPracticeSavedAsync(CancellationToken cancellationToken);
}

public sealed class NoOpPracticeCreationFaultInjector : IPracticeCreationFaultInjector
{
    public Task AfterPracticeSavedAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
