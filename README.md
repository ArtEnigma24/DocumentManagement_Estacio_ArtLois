# Document Management REST API

A simple API built with ASP.NET Core and SQL Server. You can upload, list, view, download and delete documents. File contents are saved on disk. Only the file details (metadata) are saved in the database. A small jQuery page is included to try it out.

## Requirements
- .NET SDK (TODO: version, e.g. 8.0)
- SQL Server (LocalDB, Express or full)

## Database Setup
Run these scripts from the `database` folder, in order:
1. `01_create_database_and_tables.sql` creates the `DocumentManagement` database and the `Documents` table.
2. `02_sp_document.sql` creates the `SP_Document` stored procedure.

**Documents table**

| Column | Type | Description |
|---|---|---|
| Id | INT IDENTITY | Primary key |
| OriginalFileName | NVARCHAR(255) | Name the user uploaded |
| StoredFileName | NVARCHAR(100) | Name on disk (GUID + extension) |
| ContentType | NVARCHAR(100) | File content type |
| FileSize | BIGINT | Size in bytes |
| UploadedAtUtc | DATETIME | Upload time in UTC |

`SP_Document` does all database work. Its `@Mode` parameter picks the action: `INSERT_DOCUMENT`, `GET_BY_ID`, `GET_ALL`, `DELETE_DOCUMENT`.

## Configuration
No passwords are stored in this repo. The connection string comes from the environment variable `DOCUMENTDB_CONNECTION`. If it is not set, the app uses local SQL Server with Windows login:

```
Server=localhost;Database=DocumentManagement;Trusted_Connection=True;TrustServerCertificate=True;
```

To use another server (PowerShell):
```
$env:DOCUMENTDB_CONNECTION = "Server=...;Database=DocumentManagement;User ID=...;Password=...;"
```

## Run the Application
```
cd src/DocumentManagementApi   # TODO: confirm folder name
dotnet run
```
Open the URL shown in the console. The web page loads at the root (`/`).

## Run the Tests
```
dotnet test
```
TODO: list what the tests cover once they are written.

## Endpoints

| Action | Request |
|---|---|
| Upload | `POST /api/documents/AddDocument` (form field `file`) |
| List | `GET /api/documents/GetAll` |
| Details | `GET /api/documents/GetById?id=1` |
| Download | `GET /api/documents/Download?id=1` |
| Delete | `POST /api/documents/Delete?id=1` |

Status codes: 200 success, 204 deleted, 400 empty file, 404 not found, 413 file too large, 415 file type not allowed. Errors return `{ "error": "message" }`. Responses never include file contents.

## How It Works
- **Structure:** Controller -> Service -> Repository -> SQL Server. The controller also saves and reads files on disk.
- **Data access:** Dapper with a stored procedure. It is lightweight and fits a one-table app.
- **File storage:** local `Documents` folder. Each file is saved as `{Guid}{extension}`, never with the user's file name. This prevents name collisions and path traversal. The original name is kept only in the database.
- **Upload order:** save the file, then insert the database row. If the insert fails, the file is deleted.
- **Delete order:** delete the database row, then the file. If the file delete fails, an orphan file is left, which is harmless. A row pointing to a missing file would be worse.

## Validation
- File is required and not empty.
- Maximum 10 MB.
- Allowed types, by extension: `.pdf .docx .xlsx .txt .png .jpg .jpeg`.
- File names are cleaned with `Path.GetFileName`.

## Assumptions and Limitations
- Endpoint URLs and the delete verb (`POST`) differ from the assignment (`DELETE /api/documents/{id}` and so on). With more time I would change them to match.
- File type is checked by extension only. I would add a file signature check.
- IDs are sequential numbers, so they can be guessed. No login is implemented, as allowed.
- No pagination, no virus scanning, local disk only (blob storage for production).
- Unexpected errors return a plain 500. I would add a global error handler with logging.
- TODO: add anything else that is incomplete.

## AI-Assisted Development
- **Tool used:** Claude (Anthropic).
- **How I used it:** TODO: in your own words, e.g. help with the table, stored procedure, controller, front end and this README.
- **What I reviewed or changed:** TODO: what you personally tested or changed.
- **Least confident about:** TODO: be honest, e.g. file error handling, request size limits.
