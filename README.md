# Eventify

A comprehensive, web-based Event Management System built with .NET 9, tailored for the efficient planning and management of various events, such as conferences, trips, and others. This platform enables event organizers to connect with participants and sponsors seamlessly. It also provides secure online booking and payment features, alongside a suite of additional features designed to optimize and enhance the overall event experience.

#### Note: This project is still in progress.

## Features

### **Authentication & Security**
- **JWT-based authentication** with refresh tokens
- **Multi-role authorization** (Admin, Organizer, Participant, Sponsor)
- **Email verification** and password reset workflows
- **Two-factor authentication** support
- **Social login integration** (Google, Facebook, LinkedIn)
- **Account lockout protection** and audit trails

### **Event Management**
- **Complete event lifecycle** (Draft → Published → Completed)
- **Multi-session events** with detailed scheduling
- **Venue management** with capacity tracking
- **Event categorization** and advanced search
- **Public/Private events** with approval workflows
- **Event templates** and cloning for recurring events

### **Payment Processing**
- **Stripe integration** with secure tokenization
- **Multi-currency support** for global events
- **Automatic invoice generation** with PDF receipts
- **Refund processing** (partial and full)
- **Real-time webhook handling** for payment updates
- **PCI DSS compliance** through Stripe

### **Communication System**
- **SendGrid email integration** for notifications
- **Automated email workflows** (confirmations, reminders)
- **SMS notifications** via Twilio integration
- **Professional invoice and receipt generation**
- **Email templates** for various event communications

### **Booking & Ticketing**
- **Multiple ticket types** with pricing tiers
- **Group bookings** and bulk registration
- **Promotional codes** and discount system
- **Waitlist management** for sold-out events
- **QR code generation** for ticket validation
- **Mobile-friendly check-in** system

### **Sponsor Management**
- **Comprehensive sponsor profiles** and packages
- **Sponsorship level management** (Bronze, Silver, Gold, etc.)
- **ROI tracking** and analytics for sponsors
- **Lead generation tools** for networking
- **Sponsor approval workflows**

### **Analytics & Reporting**
- **Event performance metrics** and attendance tracking
- **Revenue analytics** and financial reporting
- **User behavior insights** and engagement metrics
- **Custom report generation** with data export
- **Real-time dashboards** for organizers and admins

## **Technology Stack**

### **Backend**
- **.NET 9** - Latest .NET framework with C# 12
- **ASP.NET Core Web API** - RESTful API development
- **Entity Framework Core 9** - Object-relational mapping
- **SQL Server 2022** - Primary database
- **MediatR** - CQRS and Mediator pattern implementation
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation and rules

### **Architecture & Patterns**
- **Clean Architecture** - Separation of concerns and maintainability
- **CQRS Pattern** - Command Query Responsibility Segregation
- **Repository Pattern** - Data access abstraction
- **Specification Pattern** - Complex query composition
- **Domain-Driven Design** - Rich domain models and business logic

### **Security & Authentication**
- **ASP.NET Core Identity** - User management and authentication
- **JWT Bearer Tokens** - Stateless authentication
- **Role-based Authorization** - Fine-grained access control
- **HTTPS Enforcement** - Secure communication
- **Rate Limiting** - API protection and throttling

### **Payment & External Services**
- **Stripe** - Payment processing and subscription management
- **SendGrid** - Email delivery and templates
- **Twilio** - SMS notifications (optional)
- **QuestPDF** - PDF generation for invoices and receipts

### **Development & DevOps**
- **Serilog** - Structured logging with multiple sinks
- **Swagger/OpenAPI** - Comprehensive API documentation
- **Health Checks** - Application and dependency monitoring
- **Response Compression** - Performance optimization

## **Getting Started**

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/event-management-system.git
   cd event-management-system
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure connection string**
   
   Update `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EventManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
     }
   }
   ```

4. **Set up external services (Optional for development)**
   
   **Stripe Configuration:**
   ```json
   {
     "Stripe": {
       "PublishableKey": "pk_test_your_stripe_publishable_key",
       "SecretKey": "sk_test_your_stripe_secret_key",
       "WebhookSecret": "whsec_your_webhook_secret"
     }
   }
   ```
   
   **SendGrid Configuration:**
   ```json
   {
     "SendGrid": {
       "ApiKey": "your_sendgrid_api_key",
       "FromEmail": "noreply@yourdomain.com",
       "FromName": "Event Management System"
     }
   }
   ```

5. **Run database migrations**
   ```bash
   dotnet ef database update --project src/Infrastructure/EventManagement.Persistence --startup-project src/Presentation/EventManagement.WebAPI
   ```

6. **Start the application**
   ```bash
   dotnet run --project src/Presentation/EventManagement.WebAPI
   ```

7. **Access the API**
   - **Swagger UI**: `https://localhost:7xxx/`
   - **Health Checks**: `https://localhost:7xxx/health`

### **Default Admin Account**
- **Email**: `admin@eventmanagement.com`
- **Password**: `Admin123!`

## **API Documentation**

### **Authentication Endpoints**
```http
POST /api/v1/authentication/register     # User registration
POST /api/v1/authentication/login        # User login
POST /api/v1/authentication/refresh-token # Token refresh
POST /api/v1/authentication/forgot-password # Password reset request
POST /api/v1/authentication/reset-password  # Password reset
POST /api/v1/authentication/verify-email    # Email verification
```

### **Event Management**
```http
GET    /api/v1/events                    # Get all public events
POST   /api/v1/events                    # Create event (Organizer+)
GET    /api/v1/events/{id}               # Get event details
PUT    /api/v1/events/{id}               # Update event (Owner/Admin)
DELETE /api/v1/events/{id}               # Delete event (Owner/Admin)
POST   /api/v1/events/{id}/publish       # Publish event
GET    /api/v1/events/my-events          # Get user's organized events
```

### **Booking & Tickets**
```http
POST   /api/v1/bookings                  # Create booking
GET    /api/v1/bookings/{id}             # Get booking details
GET    /api/v1/bookings/my-bookings      # Get user's bookings
POST   /api/v1/bookings/{id}/cancel      # Cancel booking
POST   /api/v1/bookings/{id}/confirm     # Confirm booking (Organizer+)
```

### **Payment Processing**
```http
POST   /api/v1/payments                  # Create payment intent
POST   /api/v1/payments/confirm          # Confirm payment
GET    /api/v1/payments/{id}             # Get payment details
GET    /api/v1/payments/my-payments      # Get user's payments
POST   /api/v1/payments/{id}/refund      # Process refund
```

### **User Management**
```http
GET    /api/v1/userprofile               # Get current user profile
PUT    /api/v1/userprofile               # Update user profile
GET    /api/v1/admin/users               # Get all users (Admin only)
POST   /api/v1/admin/users/roles         # Manage user roles (Admin only)
```

For complete API documentation, visit the Swagger UI when running the application.

## **Configuration**

### **Environment Variables**
```bash
# Database
ConnectionStrings__DefaultConnection="Your SQL Server connection string"

# JWT Settings
JwtSettings__Key="Your 256-bit secret key"
JwtSettings__Issuer="EventManagementSystem"
JwtSettings__Audience="EventManagementSystem"

# Stripe
Stripe__SecretKey="sk_test_your_stripe_secret_key"
Stripe__PublishableKey="pk_test_your_stripe_publishable_key"
Stripe__WebhookSecret="whsec_your_webhook_secret"

# SendGrid
SendGrid__ApiKey="your_sendgrid_api_key"
SendGrid__FromEmail="noreply@yourdomain.com"
```

## 🏛️ **Architecture Overview**

```
EventManagementSystem/
├── src/
│   ├── Core/
│   │   ├── EventManagement.Domain/          # Enterprise Business Rules
│   │   └── EventManagement.Application/     # Application Business Rules
│   ├── Infrastructure/
│   │   ├── EventManagement.Infrastructure/  # External Services
│   │   └── EventManagement.Persistence/     # Database Implementation
│   ├── Presentation/
│   │   └── EventManagement.WebAPI/          # API Controllers
│   └── Shared/
│       └── EventManagement.Shared/          # Cross-cutting Concerns
├── tests/
│   ├── EventManagement.UnitTests/
│   ├── EventManagement.IntegrationTests/
└── docs/                                    # Documentation
```

### **Key Architectural Principles**
- **Dependency Inversion**: Dependencies point inward toward the domain
- **Separation of Concerns**: Each layer has a single responsibility
- **SOLID Principles**: Maintainable and extensible code
- **Clean Code**: Readable and well-documented implementation

## 🔒 **Security Features**

- **HTTPS Enforcement** - All communications encrypted
- **JWT Authentication** - Stateless and secure
- **Role-based Authorization** - Fine-grained access control
- **Input Validation** - Protection against malicious input
- **SQL Injection Protection** - Parameterized queries via EF Core
- **XSS Protection** - Input sanitization and output encoding
- **CSRF Protection** - Cross-site request forgery prevention
- **Rate Limiting** - API abuse prevention
- **Audit Logging** - Comprehensive security monitoring

## **Database Schema**

### **Core Tables**
- **Users** - User accounts and profiles
- **Events** - Event information and settings
- **Bookings** - Event registrations and reservations
- **Payments** - Payment transactions and status
- **TicketTypes** - Event ticket configurations
- **Venues** - Event location information

### **Supporting Tables**
- **UserRoles** - Role assignments with audit trails
- **EventCategories** - Event classification system
- **EventSponsors** - Sponsorship relationships
- **PaymentRefunds** - Refund transaction records
- **EventReviews** - Participant feedback system

## **Acknowledgments**

- **Microsoft** - For the excellent .NET ecosystem
- **Stripe** - For secure payment processing capabilities
- **SendGrid** - For reliable email delivery services
- **Community Contributors** - For their valuable feedback and contributions
