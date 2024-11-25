public interface IFailedLoginTracker
{
    bool IsLockedOut(string clientIp);
    void TrackFailedAttempt(string clientIp);
    void ClearFailedAttempts(string clientIp);
}