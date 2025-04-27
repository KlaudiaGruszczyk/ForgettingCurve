# Plan API REST

## 1. Zasoby
- **Użytkownicy** - tabela AspNetUsers (zarządzana przez ASP.NET Core Identity)
- **Zakresy** - tabela Scopes
- **Tematy** - tabela Topics
- **Powtórki** - tabela Repetitions

## 2. Punkty końcowe

### Uwierzytelnianie i Zarządzanie Kontem

#### Rejestracja użytkownika
- Metoda: POST
- Ścieżka: `/api/auth/register`
- Opis: Rejestruje nowego użytkownika i wysyła email weryfikacyjny
- Struktura żądania:
  ```json
  {
    "email": "string",
    "password": "string",
    "confirmPassword": "string"
  }
  ```
- Struktura odpowiedzi:
  ```json
  {
    "success": true,
    "message": "Rejestracja udana. Sprawdź email, aby zweryfikować konto."
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 400 Bad Request (nieprawidłowe dane), 409 Conflict (email już istnieje)

#### Weryfikacja adresu email
- Metoda: GET
- Ścieżka: `/api/auth/verify-email`
- Opis: Weryfikuje adres email użytkownika poprzez unikalny token
- Parametry zapytania: `token` (string), `email` (string)
- Struktura odpowiedzi:
  ```json
  {
    "success": true,
    "message": "Email zweryfikowany pomyślnie."
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 400 Bad Request (nieprawidłowy token)

#### Logowanie
- Metoda: POST
- Ścieżka: `/api/auth/login`
- Opis: Loguje użytkownika do systemu
- Struktura żądania:
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
- Struktura odpowiedzi:
  ```json
  {
    "token": "string",
    "expiresAt": "2023-01-01T00:00:00Z",
    "userId": "guid"
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 400 Bad Request (nieprawidłowe dane), 401 Unauthorized (niewłaściwy email/hasło), 403 Forbidden (konto nie zweryfikowane)

### Zarządzanie Zakresami

#### Pobieranie wszystkich zakresów
- Metoda: GET
- Ścieżka: `/api/scopes`
- Opis: Pobiera wszystkie zakresy zalogowanego użytkownika
- Parametry zapytania: 
  - `sortBy` (string, opcjonalny): "creationDate" (domyślnie), "name", "nextRepetition"
  - `skip` (int, opcjonalny): do paginacji
  - `take` (int, opcjonalny): do paginacji
- Struktura odpowiedzi:
  ```json
  {
    "items": [
      {
        "scopeId": "guid",
        "name": "string",
        "creationDate": "2023-01-01T00:00:00Z",
        "lastModifiedDate": "2023-01-01T00:00:00Z",
        "nextRepetitionDate": "2023-01-01T00:00:00Z"
      }
    ],
    "totalCount": 10
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 401 Unauthorized (niezalogowany użytkownik)

#### Tworzenie zakresu
- Metoda: POST
- Ścieżka: `/api/scopes`
- Opis: Tworzy nowy zakres dla zalogowanego użytkownika
- Struktura żądania:
  ```json
  {
    "name": "string"
  }
  ```
- Struktura odpowiedzi:
  ```json
  {
    "scopeId": "guid",
    "name": "string",
    "creationDate": "2023-01-01T00:00:00Z"
  }
  ```
- Kody powodzenia: 201 Created
- Kody błędów: 400 Bad Request (nieprawidłowe dane), 401 Unauthorized

#### Pobieranie zakresu
- Metoda: GET
- Ścieżka: `/api/scopes/{scopeId}`
- Opis: Pobiera szczegóły pojedynczego zakresu
- Struktura odpowiedzi:
  ```json
  {
    "scopeId": "guid",
    "name": "string",
    "creationDate": "2023-01-01T00:00:00Z",
    "lastModifiedDate": "2023-01-01T00:00:00Z"
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 401 Unauthorized, 403 Forbidden (brak dostępu), 404 Not Found

#### Aktualizacja zakresu
- Metoda: PUT
- Ścieżka: `/api/scopes/{scopeId}`
- Opis: Aktualizuje nazwę zakresu
- Struktura żądania:
  ```json
  {
    "name": "string"
  }
  ```
- Struktura odpowiedzi:
  ```json
  {
    "scopeId": "guid",
    "name": "string",
    "lastModifiedDate": "2023-01-01T00:00:00Z"
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found

#### Usuwanie zakresu
- Metoda: DELETE
- Ścieżka: `/api/scopes/{scopeId}`
- Opis: Usuwa zakres i wszystkie powiązane tematy
- Struktura odpowiedzi:
  ```json
  {
    "success": true,
    "message": "Zakres usunięty pomyślnie."
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 401 Unauthorized, 403 Forbidden, 404 Not Found

### Zarządzanie Tematami

#### Pobieranie tematów w zakresie
- Metoda: GET
- Ścieżka: `/api/scopes/{scopeId}/topics`
- Opis: Pobiera wszystkie tematy w określonym zakresie
- Parametry zapytania: 
  - `includeMastered` (boolean, opcjonalny): true/false (domyślnie true)
  - `skip` (int, opcjonalny): do paginacji
  - `take` (int, opcjonalny): do paginacji
- Struktura odpowiedzi:
  ```json
  {
    "items": [
      {
        "topicId": "guid",
        "name": "string",
        "startDate": "2023-01-01T00:00:00Z",
        "notes": "string",
        "isMastered": false,
        "creationDate": "2023-01-01T00:00:00Z",
        "lastModifiedDate": "2023-01-01T00:00:00Z",
        "nextRepetitionDate": "2023-01-01T00:00:00Z"
      }
    ],
    "totalCount": 25
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 401 Unauthorized, 403 Forbidden, 404 Not Found

#### Tworzenie tematu
- Metoda: POST
- Ścieżka: `/api/scopes/{scopeId}/topics`
- Opis: Tworzy nowy temat w określonym zakresie i generuje harmonogram powtórek
- Struktura żądania:
  ```json
  {
    "name": "string",
    "startDate": "2023-01-01T00:00:00Z",
    "notes": "string"
  }
  ```
- Struktura odpowiedzi:
  ```json
  {
    "topicId": "guid",
    "name": "string",
    "startDate": "2023-01-01T00:00:00Z",
    "notes": "string",
    "isMastered": false,
    "creationDate": "2023-01-01T00:00:00Z",
    "repetitionSchedule": [
      {
        "repetitionId": "guid",
        "scheduledDate": "2023-01-01T00:00:00Z",
        "intervalDays": 1
      },
      {
        "repetitionId": "guid",
        "scheduledDate": "2023-01-03T00:00:00Z",
        "intervalDays": 3
      }
      // itd. dla wszystkich wygenerowanych powtórek
    ]
  }
  ```
- Kody powodzenia: 201 Created
- Kody błędów: 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found

#### Pobieranie tematu
- Metoda: GET
- Ścieżka: `/api/topics/{topicId}`
- Opis: Pobiera szczegóły pojedynczego tematu
- Struktura odpowiedzi:
  ```json
  {
    "topicId": "guid",
    "scopeId": "guid",
    "scopeName": "string",
    "name": "string",
    "startDate": "2023-01-01T00:00:00Z",
    "notes": "string",
    "isMastered": false,
    "creationDate": "2023-01-01T00:00:00Z",
    "lastModifiedDate": "2023-01-01T00:00:00Z",
    "repetitions": [
      {
        "repetitionId": "guid",
        "scheduledDate": "2023-01-01T00:00:00Z",
        "completedDate": "2023-01-01T00:00:00Z",
        "intervalDays": 1
      }
    ]
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 401 Unauthorized, 403 Forbidden, 404 Not Found

#### Aktualizacja tematu
- Metoda: PUT
- Ścieżka: `/api/topics/{topicId}`
- Opis: Aktualizuje temat i w razie potrzeby regeneruje harmonogram powtórek
- Struktura żądania:
  ```json
  {
    "name": "string",
    "startDate": "2023-01-01T00:00:00Z",
    "notes": "string"
  }
  ```
- Struktura odpowiedzi:
  ```json
  {
    "topicId": "guid",
    "name": "string",
    "startDate": "2023-01-01T00:00:00Z",
    "notes": "string",
    "isMastered": false,
    "lastModifiedDate": "2023-01-01T00:00:00Z"
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found

#### Usuwanie tematu
- Metoda: DELETE
- Ścieżka: `/api/topics/{topicId}`
- Opis: Usuwa temat i wszystkie powiązane powtórki
- Struktura odpowiedzi:
  ```json
  {
    "success": true,
    "message": "Temat usunięty pomyślnie."
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 401 Unauthorized, 403 Forbidden, 404 Not Found

#### Oznaczanie tematu jako przyswojony
- Metoda: POST
- Ścieżka: `/api/topics/{topicId}/master`
- Opis: Oznacza temat jako przyswojony
- Struktura odpowiedzi:
  ```json
  {
    "topicId": "guid",
    "isMastered": true,
    "lastModifiedDate": "2023-01-01T00:00:00Z"
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 401 Unauthorized, 403 Forbidden, 404 Not Found

#### Cofanie oznaczenia tematu jako przyswojony
- Metoda: POST
- Ścieżka: `/api/topics/{topicId}/unmaster`
- Opis: Cofa oznaczenie tematu jako przyswojony i planuje kolejną powtórkę
- Struktura odpowiedzi:
  ```json
  {
    "topicId": "guid",
    "isMastered": false,
    "lastModifiedDate": "2023-01-01T00:00:00Z",
    "nextRepetition": {
      "repetitionId": "guid",
      "scheduledDate": "2023-01-01T00:00:00Z",
      "intervalDays": 30
    }
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 401 Unauthorized, 403 Forbidden, 404 Not Found

### Zarządzanie Powtórkami

#### Pobieranie powtórek na dziś (Panel "Co na dziś?")
- Metoda: GET
- Ścieżka: `/api/repetitions/today`
- Opis: Pobiera wszystkie powtórki wymagane dzisiaj lub zaległe
- Struktura odpowiedzi:
  ```json
  {
    "items": [
      {
        "repetitionId": "guid",
        "topicId": "guid",
        "topicName": "string",
        "scopeId": "guid",
        "scopeName": "string",
        "scheduledDate": "2023-01-01T00:00:00Z",
        "isOverdue": true,
        "intervalDays": 7
      }
    ],
    "totalCount": 8,
    "overdueCount": 3
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 401 Unauthorized

#### Oznaczanie powtórki jako wykonanej
- Metoda: POST
- Ścieżka: `/api/repetitions/{repetitionId}/complete`
- Opis: Oznacza powtórkę jako wykonaną i rekalkuluje przyszłe powtórki (jeśli potrzeba)
- Struktura żądania:
  ```json
  {
    "completedDate": "2023-01-01T00:00:00Z" // opcjonalnie, domyślnie aktualny czas UTC
  }
  ```
- Struktura odpowiedzi:
  ```json
  {
    "repetitionId": "guid",
    "topicId": "guid",
    "completedDate": "2023-01-01T00:00:00Z",
    "daysLate": 2,
    "recalculatedRepetitions": [
      {
        "repetitionId": "guid",
        "oldScheduledDate": "2023-01-10T00:00:00Z",
        "newScheduledDate": "2023-01-12T00:00:00Z"
      }
    ],
    "isLastInSequence": false
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found, 409 Conflict (jeśli już oznaczona jako wykonana)

#### Cofanie oznaczenia powtórki jako wykonanej
- Metoda: POST
- Ścieżka: `/api/repetitions/{repetitionId}/uncomplete`
- Opis: Cofa oznaczenie powtórki jako wykonanej i przywraca poprzedni harmonogram
- Struktura odpowiedzi:
  ```json
  {
    "repetitionId": "guid",
    "topicId": "guid",
    "scheduledDate": "2023-01-01T00:00:00Z",
    "completedDate": null,
    "restoredRepetitions": [
      {
        "repetitionId": "guid",
        "newScheduledDate": "2023-01-10T00:00:00Z"
      }
    ]
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 400 Bad Request (nie jest ostatnią wykonaną powtórką), 401 Unauthorized, 403 Forbidden, 404 Not Found

#### Kontynuacja powtórek po zakończeniu standardowego cyklu
- Metoda: POST
- Ścieżka: `/api/topics/{topicId}/continue-repetitions`
- Opis: Planuje kolejne powtórki co 30 dni
- Struktura odpowiedzi:
  ```json
  {
    "topicId": "guid",
    "nextRepetition": {
      "repetitionId": "guid",
      "scheduledDate": "2023-01-01T00:00:00Z",
      "intervalDays": 30
    }
  }
  ```
- Kody powodzenia: 200 OK
- Kody błędów: 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found

## 3. Uwierzytelnianie i autoryzacja

### Mechanizm uwierzytelniania
- JWT (JSON Web Token) wykorzystywany dla uwierzytelniania API
- Token wydawany po pomyślnym logowaniu
- Token musi być dołączany w nagłówku `Authorization: Bearer {token}` do wszystkich chronionych punktów końcowych
- Czas życia tokenu: 1 godzina

### Autoryzacja
- Każde żądanie do chronionego punktu końcowego weryfikuje:
  1. Obecność i ważność tokenu JWT
  2. Czy użytkownik ma dostęp do żądanego zasobu (sprawdzenie OwnerUserId)
- Kontrolery wykorzystują autentykację ASP.NET Core Identity
- Wszystkie punkty końcowe, z wyjątkiem endpointów uwierzytelniania, wymagają uwierzytelniania

## 4. Walidacja i logika biznesowa

### Walidacja danych

#### Użytkownicy
- Email: musi być poprawnym adresem email, unikalnym w systemie
- Hasło: minimum 8 znaków, co najmniej jedna wielka litera, jedna mała litera i jedna cyfra

#### Zakresy
- Nazwa: wymagana, maksymalnie 150 znaków

#### Tematy
- Nazwa: wymagana, maksymalnie 400 znaków
- Data rozpoczęcia: wymagana, nie może być w przyszłości
- Notatki: opcjonalne, maksymalnie 3000 znaków

### Implementacja logiki biznesowej

#### Generowanie harmonogramu powtórek
- Logika zaimplementowana w TopicsController (POST, PUT)
- Automatyczne generowanie 5 powtórek z interwałami 1, 3, 7, 14, 30 dni od daty startowej

#### Rekalkulacja harmonogramu
- Logika zaimplementowana w RepetitionsController (POST `/api/repetitions/{id}/complete`)
- Jeśli powtórka jest wykonana z opóźnieniem, wszystkie przyszłe powtórki są przesuwane o liczbę dni opóźnienia

#### Obsługa stanu "Przyswojony"
- Logika zaimplementowana w TopicsController (POST `/api/topics/{id}/master` i `/api/topics/{id}/unmaster`)
- Po wykonaniu powtórki z 30-dniowym interwałem, opcja kontynuacji (POST `/api/topics/{id}/continue-repetitions`) lub oznaczenia jako przyswojony

#### Cofanie oznaczenia wykonania powtórki
- Logika zaimplementowana w RepetitionsController (POST `/api/repetitions/{id}/uncomplete`)
- Możliwe tylko dla ostatniej wykonanej powtórki dla danego tematu
- Cofanie wszystkich zmian w harmonogramie wywołanych przez to oznaczenie

#### Panel powtórek "Co na dziś?"
- Logika zaimplementowana w RepetitionsController (GET `/api/repetitions/today`)
- Zwraca wszystkie bieżące i zaległe powtórki
- Sortowanie: najpierw zaległe (najstarsze pierwsze), następnie dzisiejsze 