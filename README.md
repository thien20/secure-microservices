# Secure Microservices Project

This project demonstrates a microservices architecture using **C#** and **Python**, with authentication, role-based authorization, and data security. However, HTTPS enforcement is pending completion for both services.

## Getting Started

### Prerequisites
- **C# Service** requires [.NET SDK](https://dotnet.microsoft.com/download).
- **Python Service** requires [Python 3.9+](https://www.python.org/downloads/) and `uvicorn`.

### Installation
1. Clone the repository:
   ```bash
   git clone git@github.com:thien20/secure-microservices.git / https://github.com/thien20/secure-microservices.git
   cd secure-microservices
2. Open two terminal tabs or cmd tabs:
    
    Tab 1: Navigate to and run the C# service:

    ```bash
    cd AuthService
    dotnet build
    dotnet run
    ```
The service will run at http://localhost:5085.

   Tab 2: Navigate to and run the Python service:

    cd BusinessService
    uvicorn app.main:app --reload
    The service will run at http://127.0.0.1:8000.
    
# Usage
### Auth Service (C#)
1. Access the service: Open http://localhost:5085 in your browser.
2. Endpoints:
- Register a new user.
- Login to obtain a JWT token.
3. Role-based Authorization: Use the token for secure access to other endpoints.

### Business Service (Python)
1. Access the service: Open http://127.0.0.1:8000 in your browser.
2. JWT Validation: Use the JWT token obtained from the Auth Service to access protected endpoints.

## Testing
### Auth Service (C#)
1. Navigate to the test directory:
    ```bash
        cd AuthService.Tests
    ```
2. Run tests:
    ```bash
    dotnet test 
    ```
### Business Service (Python)
1. Navigate to the test directory:
    ```bash
    cd BusinessService/tests
    ```
2. Activate virtual enviorment:
    
    - For Windows
    ```bash
    business-env/Scripts/activate
    ```
    - For Mac
    ```bash
    source bussiness-env/bin/activate
    ```
3. Run tests:
    ```bash
    pytest test_endpoints.py
    ```

## HTTPS Integration
I have done this project for almost parts except the HTTPS requirements to be enforced in the C# and Python part

I tried to use the `Let's Encrypt` to get the certificate to redirect HTTP to HTTPS. But it seem ambiguous for me and i cannot finish this part.

