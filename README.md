# 💈 Sistema de Gestión de Turnos - LUCK BARBER

Sistema web completo de gestión de turnos para barberías, desarrollado en ASP.NET Core 8.

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![C#](https://img.shields.io/badge/C%23-12.0-blue)
![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-green)

## 🎯 Características

### Para Clientes
- ✅ Reserva de turnos online 24/7
- ✅ Selección de servicios disponibles
- ✅ Visualización de horarios disponibles en tiempo real
- ✅ Confirmación automática por WhatsApp
- ✅ Interfaz responsive (mobile-first)

### Para Administradores
- ✅ Panel de administración completo
- ✅ Gestión de turnos (Confirmar, Cancelar, Completar)
- ✅ Vista de calendario mensual
- ✅ Estadísticas e ingresos
- ✅ Gestión de servicios y precios
- ✅ Dark mode

## 🛠️ Tecnologías

- **Backend:** ASP.NET Core 8 (Web API)
- **Frontend:** HTML5, CSS3, JavaScript (Vanilla)
- **Base de Datos:** SQL Server / LocalDB
- **ORM:** Entity Framework Core 8
- **Autenticación:** Basic Auth (Admin)
- **Calendario:** FullCalendar.js
- **Notificaciones:** WhatsApp Business API

## 📋 Requisitos

- .NET 8 SDK
- SQL Server 2019+ o LocalDB
- Visual Studio 2022 o VS Code
- (Opcional) Cuenta de Gmail para envío de emails

## 🚀 Instalación

### 1. Clonar el repositorio
git clone https://github.com/nijinsky14/barberia-turnos.git cd barberia-turnos

### 2. Configurar la base de datos
- Modificar la cadena de conexión en `appsettings.json`:
```json

"ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BarberiaDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Restaurar paquetes
dotnet restore

Aplicar migraciones
dotnet ef database update


### 3. Configurar appsettings.json
Copiar archivo de ejemplo
cp appsettings.example.json appsettings.json
Editar con tus credenciales
- Contraseña de admin
- Credenciales de email (opcional)


### 4. Ejecutar la aplicación
dotnet run


La aplicación estará disponible en: `https://localhost:5001`

## 📱 Uso

### Cliente
1. Acceder a la página principal
2. Seleccionar servicio
3. Elegir fecha y hora disponible
4. Completar datos de contacto
5. Confirmar reserva

### Administrador
1. Acceder a `/login.html`
2. Usuario: `admin`
3. Contraseña: (configurada en appsettings.json)
4. Gestionar turnos desde el panel

## 📁 Estructura del Proyecto
BarberiaWeb/ ├── Controllers/          # API Controllers │   ├── AuthController.cs │   ├── ServiciosController.cs │   └── TurnosController.cs ├── Data/                 # DbContext │   └── BarberiaDbContext.cs ├── DTOs/                 # Data Transfer Objects ├── Models/              # Entidades │   ├── Cliente.cs │   ├── Servicio.cs │   └── Turno.cs ├── Services/            # Lógica de negocio │   ├── TurnoService.cs │   └── EmailService.cs ├── Migrations/          # Migraciones de EF └── wwwroot/            # Frontend ├── index.html      # Landing page ├── admin.html      # Panel admin ├── calendario.html # Vista calendario └── login.html      # Login admin


## 🗄️ Base de Datos

### Tablas Principales
- **Clientes:** Información de clientes
- **Servicios:** Catálogo de servicios
- **Turnos:** Reservas de turnos

### Diagrama ER
Cliente (1) ──────< (N) Turno (N) >────── (1) Servicio


## 🔒 Seguridad

- ✅ Autenticación básica para panel admin
- ✅ Validación de datos en backend
- ✅ Protección contra SQL Injection (EF Core)
- ✅ CORS configurado
- ✅ HTTPS habilitado

## 📊 Características Avanzadas

- **Estados de turno:** Pendiente, Confirmado, Completado, Cancelado
- **Disponibilidad inteligente:** Bloqueo automático de horarios ocupados
- **Estadísticas:** Ingresos totales y estimados
- **Calendario interactivo:** Vista mensual con FullCalendar
- **Responsive design:** Optimizado para móviles

## 🤝 Contribuir

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crear una rama (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add: nueva característica'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## 📄 Licencia

Este proyecto es de código abierto bajo la licencia MIT.

## 👨‍💻 Autor

**Franco Gaibazzi**
- GitHub: [@nijinsky14](https://github.com/nijinsky14)
- Email: francogaibazzi14@gmail.com

## 🙏 Agradecimientos

- [FullCalendar](https://fullcalendar.io/) - Librería de calendario
- [Unsplash](https://unsplash.com/) - Imágenes
- Comunidad .NET

---

⭐ Si te gustó el proyecto, dale una estrella en GitHub!