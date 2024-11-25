# THE SECURE MICROSERVICES

I have done this project for almost parts except the HTTPS requirements to be enforced in the C# and Python part

I tried to use the `Let's Encrypt` to get the certificate to redirect HTTP to HTTPS. But it seem ambiguous for me and i cannot finish this part

To run this project, you need to *download / git clone* to your local machine:
1. `cd your-local-path/secure-micoservices`
2. To use the C# service: `cd AuthService`. Parrallelly use the Python Service to get data through JWT key validation `cd BusinessService`
-> this should be 2 tabs on your *terminal / cmd*
3. To run C# service on local machine: `dotnet build` -> `dotnet run` click to the http://localhost:5085 to use the use the service
4. To run Python Service: `uvicorn app.main:app --reload`
5. To use the get data from C# Serivce. You should use **POSTMAN** and take the **JWT TOKEN** from the **LOGIN SESSION** after **REGISTER** on http://localhost:5085
6. To use the get data from Python Service. You should do the same as step 5 but different port  http://127.0.0.1:8000



## Auth Service (C#)
- Recording to the requirements. I did `Endpoints`, `Role-based Authorization`, `Security` - not included HTTPS, and `Tests`

- To test the Auth Service:
1. `cd your-local-path/secure-micoservices`
2. `dotnet test \AuthService.Tests`

## Business Service (Python)
- Recording to the requirements. I did `Endpoints`, `Role-based Authorization`, `Security` - not included HTTPS, and `Tests`

- To test the Business Service:
1. `cd your-local-path/BusinessService`
2. `pytest tests/test_endpoints.py`






