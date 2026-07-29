namespace ProjectBase.Models;

public static class RegistrationStatuses
{
    public const string Submitted = "Submitted";

    // Keep the legacy database value to avoid a destructive data migration.
    public const string Registered = "Registrated";

    public const string Cancelled = "Cancelled";
}
