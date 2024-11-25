using AuthService.Models;

public interface IUserService
    {
        String GetUser(string username);
        void RegisterUser(string username, string password, string role);
        User? Authenticate(string username, string password);
    }

