# Система учета книг для библиотеки
Stack: C#, .net 10, Avalonia, ReactiveUI, Entity Framework, PostgreSQL

## Описание
Учет книг для библиотеки с возможностью добавлять книги, авторов, читателей, жанры и выдачи
<img width="1343" height="814" alt="1" src="https://github.com/user-attachments/assets/3e9a737c-98ef-4fb3-8b12-6db54ed962bc" />

## Структура проекта

```
BooksLibrary/
├── Domain/Entities/          — сущности БД (Book, Author, Genre, Reader, Loan)
├── Application/
│   ├── DTOs/                 — модели для UI
│   └── Interfaces/           — контракты репозиториев
├── Infrastructure/Data/
│   ├── AppDbContext.cs       — EF Core контекст
│   ├── Configurations/       — настройки таблиц (Fluent API)
│   └── Repositories/         — реализации репозиториев
├── Migrations/               — EF Core миграции
├── ViewModels/               — ReactiveUI ViewModels (по папкам на сущность)
├── Views/                    — Avalonia .axaml формы
│   ├── MainWindow            — главное окно с навигацией
│   ├── XxxListView           — таблица + кнопки CRUD
│   └── XxxEditView           — модальные окна добавления/редактирования
├── App.axaml(.cs)            — настройка DI и темы
└── Program.cs                — точка входа
```

Что где лежит:
- **Таблицы БД** — `Domain/Entities/` (один файл = одна таблица)
- **CRUD-операции** — `Infrastructure/Data/Repositories/`
- **Логика форм** — `ViewModels/`
- **Разметка форм** — `Views/`

## Запуск приложения
### 1. Установите зависимости
- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [PostgreSQL 13+](https://www.postgresql.org/download/)
- EF Core CLI:
  ```bash
  dotnet tool install --global dotnet-ef --version 10.0.7
  ```

### 2. Подключите базу данных
Перейдите в App.axaml.cs и AppDbContextFactory.cs и укажите там данные для подключения к вашей базе данных
```
Host=АДРЕС;Port=5432;Database=НАЗВАНИЕ_БАЗЫ_ДАННЫХ;Username=ИМЯ_ПОЛЬЗОВАТЕЛЯ;Password=ПАРОЛЬ
```

### 3. Создайте базу данных и применить миграицю
```bash
cd BooksLibrary
dotnet ef database update
```
Эта команда создаст базу `librarydb` и таблицы автоматически.

### 4. Запуск
```bash
dotnet run
```
