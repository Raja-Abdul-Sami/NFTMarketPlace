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

<img width="984" height="695" alt="image" src="https://github.com/user-attachments/assets/bda7b104-0dd1-4aa6-b1aa-4fb62b9df9ed" />

<img width="991" height="695" alt="image" src="https://github.com/user-attachments/assets/a201d72a-cced-4e72-b177-da5a2385b20b" />

<img width="1194" height="791" alt="image" src="https://github.com/user-attachments/assets/7d6029d4-b859-416c-a8bd-81636a4c3d3d" />

<img width="1197" height="798" alt="image" src="https://github.com/user-attachments/assets/b562e391-6289-4bfd-b7e2-b60245ac0682" />

<img width="1172" height="797" alt="image" src="https://github.com/user-attachments/assets/2b412266-f9f4-45f5-a85d-e0e51fd99a44" />

<img width="1168" height="785" alt="image" src="https://github.com/user-attachments/assets/946ca23a-e763-4a9e-b337-3f193ef326f7" />

<img width="1204" height="750" alt="image" src="https://github.com/user-attachments/assets/3df4d96e-d619-42a8-a31d-853feeb40889" />

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
