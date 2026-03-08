using System.Diagnostics.Metrics;

namespace Users.Application;

public interface UserMetrics
{
    void UserCreated();
}

public class DiagnosticUserMetrics : UserMetrics
{
    private const string meterName = "Identity.Users";

    private readonly Counter<long> usersCreated;

    public DiagnosticUserMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(meterName);

        usersCreated = meter.CreateCounter<long>(
            "users.created",
            description: "Number of users created"
        );
    }

    public void UserCreated()
    {
        usersCreated.Add(1);
    }
}