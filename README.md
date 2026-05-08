# 🛍️ Smart Clothing Store POS & Inventory
### **Enterprise-Grade WPF + Blazor Hybrid Solution | .NET 10**

[![Framework](https://img.shields.io/badge/.NET-10-purple.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
[![UI Framework](https://img.shields.io/badge/UI-Blazor--WebView-blue.svg)](#)
[![Persistence](https://img.shields.io/badge/DB-SQLite-green.svg)](#)

A high-performance management system for retail clothing stores, bridging the gap between native Windows capabilities and modern web flexibility using **Blazor Hybrid**.

---

## 🏗️ Architecture & Advanced Design Patterns

The project is built with a focus on **Clean Architecture** and **Domain-Driven Design (DDD)** to ensure scalability and maintainability:

* **Hybrid Hosting:** Blazor components are hosted within a **WPF container**, allowing full access to local hardware (Printers/Scanners) while maintaining a high-fidelity Web UI.
* **Encapsulated Domain Entities:** Logic-rich entities (like `Product`, `Invoice`, `StockMovement`) manage their own state and validation, ensuring data integrity at the core.
* **Unit of Work & Generic Repository:** A robust data access layer that orchestrates complex transactions across multiple services.
* **Service Layer Orchestration:** Dedicated services (`InvoiceService`, `ProductService`, etc.) handle business workflows and DTO mapping.

---

## 🛡️ Robust Business Logic & Queries

This system isn't just a CRUD application; it implements strict **Business Rules**:

### 1. Invoice Lifecycle & State Machine
* **Strict State Transitions:** Invoices manage states (`Pending`, `Completed`, `Returned`) with logic that prevents invalid reversals (e.g., a `Completed` invoice cannot revert to `Pending`).
* **Atomic Stock Sync:** Completing an invoice triggers an automated, synchronized withdrawal from the `ProductVariant` inventory.

### 2. Intelligent Inventory Management
* **Variant SKU Logic:** Automated generation of specialized SKUs based on parent attributes: `{ParentSKU}-{SizeCode}-{ColorCode}`.
* **Audit-Ready Movements:** Every inventory change—whether through a sale, return, or manual adjustment—is logged in the `StockMovements` audit trail.
- **CanWithdraw Guard:** A proactive check prevents stock-outs by validating availability before any transaction.

### 3. High-Performance Data Access
* **AutoMapper ProjectTo:** Leveraging EF Core's projection to fetch only required data from SQLite, significantly reducing memory footprint and improving query speed.
* **Advanced Filtering:** Server-side search and filtering mechanisms for products and invoices.

---

## 🛠️ Tech Stack

| Component | Technology |
| :--- | :--- |
| **Framework** | .NET 10 (C#) |
| **Host** | WPF (Windows Presentation Foundation) |
| **UI Environment** | Blazor Hybrid (Razor Components) |
| **Database** | SQLite + Entity Framework Core |
| **DTO Mapping** | AutoMapper |

---

## 🚀 Getting Started

1.  **Environment:** Ensure [Visual Studio 2022](https://visualstudio.microsoft.com/) is installed with .NET 10 SDK.
2.  **Clone & Build:**
    ```bash
    git clone [https://github.com/your-username/clothing-store-management.git](https://github.com/your-username/clothing-store-management.git)
    ```
3.  **Run:** Set the WPF project as your Startup Project and hit `F5`.

> [!NOTE]  
> For more details on Blazor Hybrid integration, refer to the [Official Microsoft Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/tutorials/wpf?view=aspnetcore-10.0).

---
**Crafted with precision using modern .NET architectures.**
