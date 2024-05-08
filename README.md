ToDoApp
ToDoApp is a simple web application built using ASP.NET Core for the backend API and React for the frontend. It provides functionality to manage todo items and user authentication.

Features
Todo Management: Users can create, read, update, and delete todo items.
User Authentication: Users can register for an account, login, and logout.
Role-based Access Control: Different roles such as Member are supported, allowing for customized access to features.
Technologies Used
Backend:
ASP.NET Core
Entity Framework Core for database management
JWT Tokens for authentication
Microsoft Identity for user management
Serilog for logging
Frontend:
React
Redux for state management
Axios for HTTP requests
Setup Instructions
Prerequisites
.NET Core SDK installed
Node.js and npm installed
Backend Setup
Clone the repository:
bash
Copy code
git clone https://github.com/your-username/ToDoApp.git
Navigate to the ToDoApp directory:
bash
Copy code
cd ToDoApp
Navigate to the ToDoApp.Api directory:
bash
Copy code
cd ToDoApp.Api
Restore dependencies:
bash
Copy code
dotnet restore
Update database:
bash
Copy code
dotnet ef database update
Run the application:
bash
Copy code
dotnet run
The API server will start at https://localhost:5001.
Frontend Setup
Navigate to the client directory:
bash
Copy code
cd client
Install dependencies:
bash
Copy code
npm install
Run the application:
bash
Copy code
npm start
The React development server will start at http://localhost:3000.
Contributing
Contributions are welcome! If you have suggestions, feature requests, or find any issues, please open an issue or create a pull request.

Team
Backend Developer: Your Name (@Faskaa)
Frontend Developer: Senan Orujov (@SenanOrujov)
License
This project is licensed under the MIT License. See the LICENSE file for details.
