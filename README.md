# 🚀 TriApex – Desktop NFT Marketplace (C# .NET)

TriApex is a **full-featured Desktop NFT Marketplace application** built completely **from scratch** using **C# (.NET / Windows Forms)** and **SQL Server**. This project is not a demo or template clone — it is a **real-world system** designed with proper architecture, database relationships, and business logic.

It is ideal for:

* 🎓 **Computer Science students** (Semester Projects)
* 💼 **.NET Developers** building portfolio projects
* 🧠 Anyone wanting to understand **real desktop application architecture**

---

## ✨ Features

### 🔐 Authentication & Session Management

* Secure Login & Registration
* Centralized session handling (Singleton pattern)
* Auto session cleanup on logout

### 🖼️ NFT Management

* Create NFTs with **image upload** (stored as binary data)
* List NFTs for sale
* Browse all available NFTs
* View owned NFTs
* Real-time image rendering in UI

### 💰 Marketplace Logic

* Buy NFTs
* Bid on NFTs
* Ownership transfer
* Transaction history
* Balance validation

### 👤 User Profile

* Profile overview
* Recent activity
* Owned NFTs
* Transaction logs

### 🎨 UI / UX

* Modern Windows Forms UI
* Animated splash screen
* Hover effects & smooth layouts
* Custom NFT cards rendering

### 🗄️ Database

* SQL Server (Express / LocalDB)
* Fully normalized schema
* Foreign key constraints
* Safe parameterized queries (ADO.NET)

---

## 🛠️ Tech Stack

| Layer        | Technology                |
| ------------ | ------------------------- |
| Language     | C#                        |
| Framework    | .NET Framework (WinForms) |
| Database     | SQL Server                |
| Data Access  | ADO.NET                   |
| UI           | Windows Forms             |
| Architecture | Layered / Modular         |

---

## 🧱 Project Architecture

```
TriApex/
│
├── Forms/              # Login, Register, Dashboard, Splash
├── UserControls/       # Dashboard, Profile, My NFTs, Browse NFTs
├── Helpers/            # DBHelper, SessionManager, UI helpers
├── Models/             # Data models
├── Assets/             # Images & icons
├── Program.cs          # Application entry point
└── TriApexDB.sql       # Database schema
```

---

## 🗄️ Database Schema (Core Table)

**NFTs**

```
NFTID        INT (PK, Identity)
Title        VARCHAR
Description  VARCHAR
Price        DECIMAL
CurrentBid   DECIMAL
ImagePath    VARCHAR
ImageData    VARBINARY(MAX)
OwnerID      INT (FK)
IsSold       BIT
CreatedBy    VARCHAR
CreatedDate  DATETIME
Category     VARCHAR
Views        INT
```

---

## ▶️ How to Run

1. Clone the repository

```bash
git clone https://github.com/yourusername/TriApex.git
```

2. Open the solution in **Visual Studio**

3. Restore database

* Create database `TriApexDB`
* Execute the provided SQL script

4. Update connection string in `DBHelper.cs`

5. Build & Run 🚀

---

## 📸 Screenshots

<img width="984" height="695" alt="image" src="https://github.com/user-attachments/assets/78622d91-2c88-41de-91ef-289c6083ee6c" />


<img width="991" height="695" alt="image" src="https://github.com/user-attachments/assets/9281f533-6de6-48df-87a2-2c37fba625a5" />


<img width="1194" height="791" alt="image" src="https://github.com/user-attachments/assets/422790cf-eec7-4ca2-91ee-c3cb763807b5" />


<img width="1197" height="798" alt="image" src="https://github.com/user-attachments/assets/01902bd4-e244-463b-964f-3b8d7ed8a225" />


<img width="1172" height="797" alt="image" src="https://github.com/user-attachments/assets/3a1a2463-f37c-43be-96cc-9dd5c039f75b" />


<img width="1168" height="785" alt="image" src="https://github.com/user-attachments/assets/cb1d27f5-8af4-4d9a-9838-52c41021d566" />


<img width="1204" height="750" alt="image" src="https://github.com/user-attachments/assets/eb521f1e-62ba-4764-b25e-38628344b99c" />


---

## 🎯 Learning Outcomes

* Real-world WinForms application structure
* Handling binary image data in SQL Server
* Managing complex UI rendering
* Debugging real production-level errors
* Implementing marketplace logic

---

## 🚧 Known Improvements (Future Scope)

* Wallet integration
* Blockchain support
* API-based backend
* Role-based access control
* Cloud storage for images

---

## ⚠️ Disclaimer

This project is built **for educational and portfolio purposes**.
It does **not** interact with real blockchain networks.

---

## ⭐ Support

If you find this project useful:

* ⭐ Star the repository
* 🍴 Fork it
* 🧠 Learn from it

---

## 👤 Author

**Raja Abdul Sami**
.NET Developer | CS Student

📺 YouTube: *RajaAbdulSami-code*

---

> "Build real projects. Debug real problems. Become a real developer." 💻🔥
