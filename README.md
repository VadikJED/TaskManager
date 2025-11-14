<img width="902" height="682" alt="image" src="https://github.com/user-attachments/assets/4940fb03-e9c8-4c49-9c55-1a58621c49fb" />

1. Скачивание PostgreSQL
Перейдите на официальный сайт:

https://www.postgresql.org/download/windows/

2. Установка PostgreSQL

Запустите установщик.

Выберите компоненты:

✅ PostgreSQL Server

✅ pgAdmin 4 (графический интерфейс)

✅ Command Line Tools

✅ Stack Builder (опционально)

Укажите папку установки:

По умолчанию: C:\Program Files\PostgreSQL\16\

Настройте пароль superuser:

Password: придумайте надежный пароль (запомните его!)

Confirm password: повторите пароль

Порт:

Оставьте по умолчанию: 5432

Локаль:

Russian_Russia.1251 или English_United States.1252

Завершите установку

В папке https://github.com/VadikJED/TaskManager/tree/master/TaskManager.Desktop/Export находим файл TaskManager.Desktop/Export/taskmanager_copy.sql

Создание бэкапа базы данных (Export)
Backup через pgAdmin
Откройте pgAdmin 4
Разверните дерево:
Servers → PostgreSQL → Databases → taskmanager
Правой кнопкой на taskmanager
Выберите Backup...
Настройки бэкапа:
Filename: C:\backup\taskmanager_backup.sql (или другая папка)
Format: Plain или Custom
Encoding: UTF8
Нажмите Backup

Как востоновить
Правой кнопкой на taskmanager → Backup (как выше)
Создайте новую базу:
Правой кнопкой на Databases → Create → Database
Name: taskmanager_copy
Восстановите бэкап в новую базу:
Правой кнопкой на taskmanager_copy → Restore...
Select file: выберите ваш .sql бэкап
Нажмите Restore


TaskManager.Desktop

├── 📄 TaskManager.Desktop.csproj

├── 📄 Program.cs

├── 📄 App.axaml

├── 📄 App.axaml.cs

├── 📄 appsettings.json

├── 📄 ViewLocator.cs

│

├── 📁 Models/

│   └── 📄 TaskItem.cs

│

├── 📁 Data/

│   ├── 📄 ApplicationDbContext.cs

│   └── 📁 Configurations/

│       └── 📄 TaskItemConfiguration.cs

│

├── 📁 Repositories/

│   ├── 📄 ITaskRepository.cs

│   └── 📄 TaskRepository.cs

│

├── 📁 Services/

│   ├── 📄 DatabaseService.cs

│   └── 📄 ServiceConfiguration.cs

│
├── 📁 ViewModels/

│   ├── 📄 ViewModelBase.cs

│   ├── 📄 MainWindowViewModel.cs

│   └── 📄 TaskItemViewModel.cs

│

├── 📁 Views/

│   ├── 📄 MainWindow.axaml

│   └── 📄 MainWindow.axaml.cs

│

└── 📁 Converters/
  └── 📄 BoolConverters.cs


TaskManager.Tests/

├── 📄 TaskManager.Tests.csproj

│

├── 📁 ViewModels/

│   ├── 📄 MainWindowViewModelTests.cs

│   └── 📄 TaskItemViewModelTests.cs

│
├── 📁 Repositories/

│   └── 📄 TaskRepositoryTests.cs

│

└── 📁 Helpers/
    
  └── 📄 TestDbContextFactory.cs

<img width="597" height="409" alt="image" src="https://github.com/user-attachments/assets/8a31eca3-7ad2-4808-840f-b368feb300b1" />
<img width="1007" height="709" alt="image" src="https://github.com/user-attachments/assets/38a8697b-2261-48a4-9dd4-473d7f10ce1b" />

# Опубликовать для Ubuntu 20.04 x64
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/linux

# Или для Ubuntu 22.04 x64
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish/linux
