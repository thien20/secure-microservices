public class FailedLoginTracker : IFailedLoginTracker
{
    private static readonly Dictionary<string, (int attempts, DateTime lastAttempt)> _failedLoginAttempts = new();
    private const int MaxAttempts = 5;
    private const int LockoutMinutes = 5;

    public bool IsLockedOut(string clientIp)
    {
        if (_failedLoginAttempts.TryGetValue(clientIp, out var record) &&
            record.attempts >= MaxAttempts &&
            (DateTime.UtcNow - record.lastAttempt).TotalMinutes < LockoutMinutes)
        {
            return true;
        }

        return false;
    }

    public void TrackFailedAttempt(string clientIp)
    {
        var now = DateTime.UtcNow;

        if (!_failedLoginAttempts.ContainsKey(clientIp))
        {
            _failedLoginAttempts[clientIp] = (1, now);
        }
        else
        {
            var attempts = _failedLoginAttempts[clientIp].attempts + 1;
            _failedLoginAttempts[clientIp] = (attempts, now);
        }
    }

    public void ClearFailedAttempts(string clientIp)
    {
        _failedLoginAttempts.Remove(clientIp);
    }
}
