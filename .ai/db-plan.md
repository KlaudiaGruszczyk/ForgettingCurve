```markdown
### 1. Lista Tabel z Kolumnami, Typami Danych i Ograniczeniami (MSSQL)

```sql
-- Tabela użytkowników (zakładamy, że jest zarządzana przez ASP.NET Core Identity)
-- Należy dostosować nazwę tabeli i kolumny PK do rzeczywistej konfiguracji Identity.
-- Przykład standardowej struktury Identity z Guid PK:
/*
CREATE TABLE dbo.AspNetUsers (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET(7) NULL,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL
);
CREATE INDEX EmailIndex ON dbo.AspNetUsers (NormalizedEmail);
CREATE UNIQUE INDEX UserNameIndex ON dbo.AspNetUsers (NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
*/

-- Tabela Zakresów (Scopes)
CREATE TABLE dbo.Scopes (
    ScopeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    OwnerUserId UNIQUEIDENTIFIER NOT NULL, -- Klucz obcy do tabeli użytkowników (AspNetUsers.Id)
    Name NVARCHAR(150) NOT NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastModifiedDate DATETIME2 NULL -- Aktualizowana przez logikę aplikacji lub trigger
);
GO

-- Tabela Tematów (Topics)
CREATE TABLE dbo.Topics (
    TopicId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ScopeId UNIQUEIDENTIFIER NOT NULL, -- Klucz obcy do tabeli Scopes
    OwnerUserId UNIQUEIDENTIFIER NOT NULL, -- Klucz obcy do tabeli użytkowników
    Name NVARCHAR(400) NOT NULL,
    StartDate DATETIME2 NOT NULL, -- Data rozpoczęcia nauki tematu (UTC)
    Notes NVARCHAR(MAX) NULL,
    IsMastered BIT NOT NULL DEFAULT 0, -- Czy temat został oznaczony jako przyswojony
    CreationDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastModifiedDate DATETIME2 NULL -- Aktualizowana przez logikę aplikacji lub trigger
);
GO

-- Tabela Powtórek (Repetitions)
CREATE TABLE dbo.Repetitions (
    RepetitionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TopicId UNIQUEIDENTIFIER NOT NULL, -- Klucz obcy do tabeli Topics
    ScheduledDate DATETIME2 NOT NULL, -- Data, na którą powtórka jest zaplanowana (UTC)
    CompletedDate DATETIME2 NULL, -- Data faktycznego wykonania powtórki (UTC). NULL oznacza niewykonaną.
    IntervalDays INT NULL -- Interwał (w dniach, np. 1, 3, 7, 14, 30), który doprowadził do tej ScheduledDate
);
GO
```

### 2. Relacje Między Tabelami (Klucze Obce - MSSQL)

```sql
-- Relacja: Użytkownik -> Zakresy
ALTER TABLE dbo.Scopes
ADD CONSTRAINT FK_Scopes_OwnerUserId
FOREIGN KEY (OwnerUserId) REFERENCES dbo.AspNetUsers(Id)
ON DELETE NO ACTION; -- Zapobiega usunięciu użytkownika, jeśli ma zakresy
GO

-- Relacja: Zakres -> Tematy
ALTER TABLE dbo.Topics
ADD CONSTRAINT FK_Topics_ScopeId
FOREIGN KEY (ScopeId) REFERENCES dbo.Scopes(ScopeId)
ON DELETE CASCADE; -- Usunięcie zakresu usuwa jego tematy
GO

-- Relacja: Użytkownik -> Tematy
ALTER TABLE dbo.Topics
ADD CONSTRAINT FK_Topics_OwnerUserId
FOREIGN KEY (OwnerUserId) REFERENCES dbo.AspNetUsers(Id)
ON DELETE NO ACTION; -- Dodatkowe zabezpieczenie
GO

-- Relacja: Temat -> Powtórki
ALTER TABLE dbo.Repetitions
ADD CONSTRAINT FK_Repetitions_TopicId
FOREIGN KEY (TopicId) REFERENCES dbo.Topics(TopicId)
ON DELETE CASCADE; -- Usunięcie tematu usuwa jego powtórki
GO
```

### 3. Indeksy (MSSQL)

```sql
-- Indeksy dla kluczy obcych
CREATE NONCLUSTERED INDEX IX_Scopes_OwnerUserId ON dbo.Scopes(OwnerUserId);
CREATE NONCLUSTERED INDEX IX_Topics_ScopeId ON dbo.Topics(ScopeId);
CREATE NONCLUSTERED INDEX IX_Topics_OwnerUserId ON dbo.Topics(OwnerUserId); -- Ważne dla filtrowania w aplikacji
CREATE NONCLUSTERED INDEX IX_Repetitions_TopicId ON dbo.Repetitions(TopicId);
GO

-- Indeksy wspierające zapytania
CREATE NONCLUSTERED INDEX IX_Topics_IsMastered ON dbo.Topics(IsMastered);
CREATE NONCLUSTERED INDEX IX_Scopes_CreationDate ON dbo.Scopes(CreationDate DESC); -- Dla domyślnego sortowania zakresów
CREATE NONCLUSTERED INDEX IX_Scopes_Name ON dbo.Scopes(Name); -- Dla sortowania zakresów po nazwie
GO

-- Indeks kluczowy dla "Panelu Powtórek" (znajduje niewykonane powtórki posortowane wg daty)
-- Używa filtrowanego indeksu
CREATE NONCLUSTERED INDEX IX_Repetitions_ScheduledDate_NotCompleted
ON dbo.Repetitions(ScheduledDate)
INCLUDE (TopicId) -- Dołączamy TopicId dla JOIN w aplikacji
WHERE CompletedDate IS NULL;
GO

-- Indeks do szybkiego znajdowania ostatniej wykonanej powtórki dla tematu
CREATE NONCLUSTERED INDEX IX_Repetitions_TopicId_CompletedDate_Desc
ON dbo.Repetitions(TopicId, CompletedDate DESC)
WHERE CompletedDate IS NOT NULL;
GO
```


### 4. Dodatkowe Uwagi

1.  **Egzekwowanie Bezpieczeństwa w Aplikacji:** **Krytycznie ważne** jest, aby logika aplikacji .NET **zawsze** filtrowała dane na podstawie `OwnerUserId` zalogowanego użytkownika. Każde zapytanie `SELECT`, `UPDATE`, `DELETE` dotyczące `Scopes`, `Topics` lub `Repetitions` musi zawierać warunek `WHERE OwnerUserId = @CurrentUserId` (lub odpowiedni JOIN i warunek dla `Repetitions`). Zaniedbanie tego kroku spowoduje, że użytkownicy będą mogli widzieć i modyfikować dane innych użytkowników.
2.  **Typy Danych Czasowych:** Użyto `DATETIME2`, zgodnie z ustaleniami dla MSSQL. Funkcja `GETUTCDATE()` zapewnia zapis aktualnego czasu w UTC.
3.  **Generowanie UUID:** Użyto `NEWSEQUENTIALID()` jako wartości domyślnej dla kluczy głównych typu `UNIQUEIDENTIFIER`, aby zminimalizować fragmentację indeksu klastrowanego.
4.  **Obsługa `LastModifiedDate`:** Schemat zawiera kolumnę `LastModifiedDate`, ale nie definiuje automatycznej aktualizacji. Zaleca się implementację aktualizacji tej kolumny w logice aplikacji lub za pomocą triggerów bazodanowych.
5.  **Integralność Danych:** Klucze obce z odpowiednimi akcjami `ON DELETE` (CASCADE lub NO ACTION) zapewniają integralność referencyjną. `NO ACTION` na `OwnerUserId` zapobiega przypadkowemu usunięciu użytkownika, jeśli posiada on jeszcze jakieś dane.
6.  **Struktura Użytkownika:** Schemat zakłada użycie standardowej tabeli `AspNetUsers` z ASP.NET Core Identity z kluczem głównym typu `UNIQUEIDENTIFIER`. Należy to zweryfikować i dostosować nazwy tabel/kolumn, jeśli konfiguracja Identity jest inna.
7.  **Logika Aplikacji:** Kluczowe aspekty funkcjonalności (generowanie harmonogramu, rekalkulacja dat, obsługa cofania powtórki, logika stanu "Przyswojony") są realizowane w logice aplikacji, a baza danych dostarcza jedynie strukturę do przechowywania danych.
```