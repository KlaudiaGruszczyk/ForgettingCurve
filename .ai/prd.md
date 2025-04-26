# Dokument wymagań produktu (PRD) - Aplikacja do Planowania Powtórek
## 1. Przegląd produktu
Aplikacja do Planowania Powtórek to aplikacja webowa zaprojektowana, aby pomóc użytkownikom w efektywnym zarządzaniu długoterminowym procesem nauki. Umożliwia ona śledzenie i planowanie powtórek materiału w oparciu o naukowo potwierdzone metody, takie jak krzywa zapominania Ebbinghausa, automatyzując proces i zwiększając skuteczność nauki. Wersja MVP (Minimum Viable Product) koncentruje się na podstawowych funkcjonalnościach zarządzania materiałem do nauki i automatycznym planowaniu powtórek.

## 2. Problem użytkownika
Użytkownicy (np. studenci, osoby uczące się języków, profesjonaliści) mają trudności z efektywnym zarządzaniem procesem nauki długoterminowej. Ręczne śledzenie terminów powtórek materiału zgodnie z naukowo potwierdzonymi metodami (np. krzywa zapominania) jest uciążliwe, czasochłonne i podatne na błędy. Brak systematycznego podejścia prowadzi do nieregularnych powtórek, zapominania materiału lub całkowitej rezygnacji z systematycznej nauki, co znacząco obniża jej skuteczność i efektywność.

## 3. Wymagania funkcjonalne

### 3.1. Uwierzytelnianie i Zarządzanie Kontem Użytkownika
-   Rejestracja: Użytkownicy mogą tworzyć nowe konta za pomocą adresu e-mail i hasła.
    -   Wymagana jest weryfikacja adresu e-mail poprzez link wysłany na podany adres.
    -   Hasło musi spełniać minimalne wymagania: 8 znaków, co najmniej jedna wielka litera, jedna mała litera i jedna cyfra.
-   Logowanie: Zarejestrowani użytkownicy mogą logować się do aplikacji przy użyciu swojego adresu e-mail i hasła.
-   Prywatność: Dane użytkownika (Zakresy, Tematy, Notatki) są prywatne i dostępne tylko dla zalogowanego użytkownika.

### 3.2. Zarządzanie Zakresami (CRUD)
-   Tworzenie Zakresu: Użytkownik może tworzyć nowe Zakresy, podając ich nazwę (max 150 znaków).
-   Odczyt Zakresów: Użytkownik może przeglądać listę swoich Zakresów.
    -   Domyślne sortowanie listy Zakresów: według daty utworzenia (najnowsze na górze).
    -   Opcjonalne sortowanie: alfabetycznie według nazwy, według najwcześniejszej daty następnej powtórki Tematu w Zakresie (Zakresy bez zaplanowanych powtórek lub tylko z Tematami "Przyswojonymi" są na końcu listy).
-   Aktualizacja Zakresu: Użytkownik może edytować nazwę istniejącego Zakresu.
-   Usuwanie Zakresu: Użytkownik może usuwać Zakresy.
    -   Usunięcie Zakresu powoduje trwałe, kaskadowe usunięcie wszystkich zawartych w nim Tematów.
    -   Wymagane jest dwuetapowe potwierdzenie:
        -   Modal 1: Informuje o konsekwencjach (usunięcie Tematów) i pyta o świadomość użytkownika.
        -   Modal 2: Ostateczne potwierdzenie usunięcia.

### 3.3. Zarządzanie Tematami (CRUD) w ramach Zakresu
-   Tworzenie Tematu: W obrębie wybranego Zakresu, użytkownik może dodawać nowe Tematy, podając:
    -   Nazwę (max 400 znaków).
    -   Datę rozpoczęcia nauki (data, kiedy użytkownik zaczął się uczyć materiału).
    -   Opcjonalne Notatki (max 3000 znaków, plain text).
-   Odczyt Tematów: Użytkownik może przeglądać listę Tematów w ramach wybranego Zakresu.
    -   Dostępny filtr: "Ukryj/Pokaż Tematy Przyswojone".
-   Aktualizacja Tematu: Użytkownik może edytować nazwę, datę rozpoczęcia nauki oraz notatki istniejącego Tematu. Umożliwia również cofnięcie stanu "Przyswojony".
-   Usuwanie Tematu: Użytkownik może usuwać pojedyncze Tematy.

### 3.4. Automatyczne Planowanie Powtórek
-   Generowanie Harmonogramu: Po dodaniu Tematu z datą startową, system automatycznie generuje i zapisuje sekwencję przyszłych dat powtórek.
-   Algorytm MVP (Stały): Interwały powtórek wynoszą 1 dzień, 3 dni, 7 dni, 14 dni, 30 dni, liczone od daty startowej Tematu.
-   Rekalkulacja Harmonogramu: Jeśli powtórka zaplanowana na datę X zostanie oznaczona jako wykonana z opóźnieniem Y dni (w dniu X+Y), wszystkie kolejne, jeszcze niewykonane, zaplanowane daty powtórek dla tego Tematu zostaną przesunięte o Y dni.
-   Zakończenie Standardowego Cyklu: Po wykonaniu powtórki zaplanowanej po 30-dniowym interwale, system wyświetla modal:
    -   Opcja 1: Kontynuuj powtórki co 30 dni.
    -   Opcja 2: Oznacz Temat jako "Przyswojony" (Mastered).
-   Kontynuacja co 30 dni: Jeśli użytkownik wybierze tę opcję, system planuje kolejne powtórki co 30 dni od daty ostatniej wykonanej powtórki. Te powtórki podlegają tym samym zasadom (pojawiają się w Panelu Powtórek, stają się zaległe, ich opóźnienie powoduje rekalkulację).
-   Stan "Przyswojony": Jeśli użytkownik wybierze tę opcję (lub zostanie ona aktywowana), Temat przestaje pojawiać się w Panelu Powtórek.
    -   Wizualizacja: Tematy "Przyswojone" na liście Tematów w Zakresie są wyszarzone i oznaczone ikoną "ptaszka" (checkmark).
    -   Odwracalność: Stan "Przyswojony" można cofnąć w widoku edycji Tematu, przywracając go do cyklu powtórek (np. ostatni interwał 30-dniowy).

### 3.5. Panel Powtórek ("Co na dziś?")
-   Widok Główny: Centralny widok aplikacji, domyślnie wyświetlany po zalogowaniu.
-   Agregacja: Wyświetla listę wszystkich Tematów ze wszystkich Zakresów, które wymagają powtórki danego dnia lub są zaległe.
-   Sortowanie:
    1.  Tematy zaległe (najpierw najstarsza data wymagalności).
    2.  Tematy wymagane dzisiaj.
    3.  W ramach tej samej grupy i daty wymagalności: alfabetycznie wg nazwy Zakresu, następnie alfabetycznie wg nazwy Tematu.
-   Wyróżnienie Zaległych: Tematy zaległe są wizualnie odróżnione (np. inny kolor tła, ikona ostrzegawcza).

### 3.6. Zarządzanie Wykonaniem Powtórki
-   Oznaczanie Wykonania: Użytkownik może oznaczyć Temat na liście "Co na dziś?" jako powtórzony za pomocą checkboxa. System rejestruje wykonanie i datę.
-   Cofanie Oznaczenia: Użytkownik może cofnąć oznaczenie *tylko ostatniej* wykonanej powtórki dla danego Tematu poprzez ponowne kliknięcie checkboxa.
    -   Konsekwencje: Przywraca Temat na listę "Co na dziś?" (jeśli był wymagany lub zaległy) i cofa zmiany w harmonogramie (rekalkuluje daty do stanu sprzed oznaczenia).
    -   Ograniczenie: Nie można cofnąć wcześniejszych powtórek w sekwencji, jeśli kolejne zostały już oznaczone.

### 3.7. Interfejs Użytkownika i Doświadczenie
-   Stan Początkowy (Empty State): Nowym użytkownikom, którzy nie mają jeszcze żadnych Zakresów, wyświetlany jest komunikat powitalny i przycisk "+" z etykietą "Utwórz pierwszy Zakres".
-   Nawigacja: Prosta nawigacja między Panelem Powtórek a listą Zakresów/widokiem Tematów.
-   Obsługa Błędów: Podstawowe komunikaty dla użytkownika w przypadku błędów (np. nieudane logowanie, błąd zapisu danych, błąd walidacji formularza).

### 3.8. Wymagania Niefunkcjonalne (MVP)
-   Platforma: Aplikacja webowa.
-   Wsparcie Przeglądarek: Ostatnie dwie stabilne główne wersje Google Chrome, Mozilla Firefox, Apple Safari, Microsoft Edge.
-   Obsługa Stref Czasowych: Operacje związane z datami (np. określenie "dziś" dla Panelu Powtórek) bazują na ustawieniach strefy czasowej przeglądarki użytkownika.
-   Limity Danych: Brak limitów liczby Zakresów/Tematów. Limity długości pól tekstowych (Zakres: 150, Temat: 400, Notatki: 3000 znaków).
-   Trwałość Ustawień: Preferencje sortowania nie są zapamiętywane między sesjami.
-   Dostępność (A11y): Brak specyficznych wymagań WCAG dla MVP.
-   Wydajność: Brak specyficznych wymagań dotyczących czasu ładowania dla MVP.
-   Bezpieczeństwo: Standardowe praktyki (HTTPS, hashowanie haseł, weryfikacja email), brak dodatkowych wymagań dla MVP.

## 4. Granice produktu

Następujące funkcjonalności i cechy są celowo wyłączone z zakresu MVP:
-   Możliwość modyfikacji algorytmu powtórek przez użytkownika.
-   Import/eksport danych (np. z/do CSV).
-   Funkcje społecznościowe (współdzielenie, współpraca).
-   Integracje z zewnętrznymi narzędziami (kalendarze, platformy e-learningowe).
-   Dedykowane aplikacje mobilne (natywne).
-   Powiadomienia (email, push) o nadchodzących powtórkach.
-   Zaawansowane statystyki i wizualizacje postępów w nauce.
-   System tagowania lub dodatkowej kategoryzacji Tematów.
-   Tryb nauki/quizu/fiszek bezpośrednio w aplikacji.
-   Gamifikacja (punkty, odznaki, rankingi).
-   Wsparcie dla różnych typów treści w Notatkach (np. obrazy, audio, formatowanie RTF).
-   Zapamiętywanie preferencji sortowania między sesjami.
-   Szczegółowe wymagania dotyczące dostępności (WCAG).
-   Specyficzne cele wydajnościowe.
-   Zaawansowane funkcje bezpieczeństwa poza podstawowym uwierzytelnianiem i ochroną danych.

## 5. Historyjki użytkowników

### Sekcja 5.1: Uwierzytelnianie i Zarządzanie Kontem

-   ID: US-001
-   Tytuł: Rejestracja nowego użytkownika
-   Opis: Jako nowy użytkownik, chcę móc utworzyć konto w aplikacji używając mojego adresu e-mail i hasła, aby móc korzystać z jej funkcjonalności.
-   Kryteria akceptacji:
    -   Mogę przejść do formularza rejestracji.
    -   Mogę wprowadzić adres e-mail i hasło.
    -   System waliduje format adresu e-mail.
    -   System waliduje złożoność hasła (min. 8 znaków, 1 wielka, 1 mała litera, 1 cyfra).
    -   System sprawdza, czy adres e-mail nie jest już zajęty.
    -   Po pomyślnym przesłaniu formularza, otrzymuję informację o konieczności weryfikacji e-mail.
    -   Otrzymuję e-mail weryfikacyjny z unikalnym linkiem.
    -   Kliknięcie linku weryfikacyjnego aktywuje moje konto.
    -   Nie mogę zalogować się przed aktywacją konta.

-   ID: US-002
-   Tytuł: Logowanie użytkownika
-   Opis: Jako zarejestrowany i zweryfikowany użytkownik, chcę móc zalogować się do aplikacji używając mojego adresu e-mail i hasła, aby uzyskać dostęp do moich danych i funkcjonalności.
-   Kryteria akceptacji:
    -   Mogę przejść do formularza logowania.
    -   Mogę wprowadzić zarejestrowany adres e-mail i hasło.
    -   Po pomyślnym zalogowaniu jestem przekierowany do Panelu Powtórek.
    -   W przypadku błędnych danych (e-mail lub hasło) widzę stosowny komunikat błędu.
    -   W przypadku próby logowania na niezweryfikowane konto widzę stosowny komunikat.

-   ID: US-003
-   Tytuł: Zabezpieczenie dostępu
-   Opis: Jako zalogowany użytkownik, chcę mieć pewność, że tylko ja mam dostęp do moich Zakresów i Tematów.
-   Kryteria akceptacji:
    -   Dane (Zakresy, Tematy, Notatki) utworzone przez jednego użytkownika nie są widoczne dla innego zalogowanego użytkownika.
    -   Dostęp do funkcjonalności zarządzania danymi wymaga bycia zalogowanym.

### Sekcja 5.2: Zarządzanie Zakresami

-   ID: US-004
-   Tytuł: Tworzenie nowego Zakresu
-   Opis: Jako zalogowany użytkownik, chcę móc utworzyć nowy Zakres (np. przedmiot, dziedzinę nauki), aby móc w nim grupować powiązane Tematy.
-   Kryteria akceptacji:
    -   Mogę zainicjować tworzenie nowego Zakresu (np. z widoku listy Zakresów).
    -   Mogę wprowadzić nazwę dla nowego Zakresu (max 150 znaków).
    -   System waliduje, czy nazwa nie jest pusta.
    -   Po pomyślnym utworzeniu, nowy Zakres pojawia się na mojej liście Zakresów.

-   ID: US-005
-   Tytuł: Przeglądanie listy Zakresów
-   Opis: Jako zalogowany użytkownik, chcę móc zobaczyć listę wszystkich moich utworzonych Zakresów, aby móc nawigować do ich zawartości.
-   Kryteria akceptacji:
    -   Mogę przejść do widoku listy moich Zakresów.
    -   Lista wyświetla nazwy wszystkich moich Zakresów.
    -   Domyślnie lista jest posortowana według daty utworzenia (najnowsze pierwsze).
    -   Mogę zmienić sortowanie na alfabetyczne wg nazwy.
    -   Mogę zmienić sortowanie na według najwcześniejszej daty następnej powtórki Tematu w Zakresie.

-   ID: US-006
-   Tytuł: Edycja nazwy Zakresu
-   Opis: Jako zalogowany użytkownik, chcę móc zmienić nazwę istniejącego Zakresu, jeśli popełniłem błąd lub chcę ją uaktualnić.
-   Kryteria akceptacji:
    -   Mogę zainicjować edycję nazwy dla wybranego Zakresu na liście.
    -   Mogę wprowadzić nową nazwę (max 150 znaków).
    -   System waliduje, czy nowa nazwa nie jest pusta.
    -   Po zapisaniu zmian, zaktualizowana nazwa jest widoczna na liście Zakresów.

-   ID: US-007
-   Tytuł: Usuwanie Zakresu
-   Opis: Jako zalogowany użytkownik, chcę móc usunąć Zakres, którego już nie potrzebuję, wraz ze wszystkimi zawartymi w nim Tematami.
-   Kryteria akceptacji:
    -   Mogę zainicjować usuwanie dla wybranego Zakresu na liście.
    -   Wyświetlany jest pierwszy modal z ostrzeżeniem, że usunięcie Zakresu spowoduje usunięcie wszystkich jego Tematów, pytający o świadomość.
    -   Jeśli potwierdzę świadomość, wyświetlany jest drugi modal wymagający ostatecznego potwierdzenia usunięcia.
    -   Jeśli ostatecznie potwierdzę, Zakres i wszystkie jego Tematy są trwale usuwane z systemu.
    -   Usunięty Zakres znika z listy Zakresów.
    -   Jeśli anuluję w którymkolwiek kroku, Zakres i jego Tematy pozostają nienaruszone.

### Sekcja 5.3: Zarządzanie Tematami

-   ID: US-008
-   Tytuł: Tworzenie nowego Tematu w Zakresie
-   Opis: Jako zalogowany użytkownik, chcę móc dodać nowy Temat (jednostkę nauki) w ramach wybranego Zakresu, podając jego nazwę, datę rozpoczęcia nauki i opcjonalnie notatki, aby system mógł zaplanować dla niego powtórki.
-   Kryteria akceptacji:
    -   Mogę przejść do widoku wybranego Zakresu.
    -   Mogę zainicjować tworzenie nowego Tematu.
    -   Mogę wprowadzić nazwę Tematu (max 400 znaków).
    -   Mogę wybrać datę rozpoczęcia nauki (data z przeszłości lub teraźniejsza).
    -   Mogę opcjonalnie dodać notatki (max 3000 znaków, plain text).
    -   System waliduje, czy nazwa i data startowa są podane.
    -   Po pomyślnym utworzeniu, nowy Temat pojawia się na liście Tematów w danym Zakresie.
    -   System automatycznie generuje harmonogram powtórek dla nowego Tematu (1, 3, 7, 14, 30 dni od daty startowej).

-   ID: US-009
-   Tytuł: Przeglądanie listy Tematów w Zakresie
-   Opis: Jako zalogowany użytkownik, chcę móc zobaczyć listę wszystkich Tematów w ramach wybranego Zakresu, aby zarządzać nimi lub przejrzeć ich szczegóły.
-   Kryteria akceptacji:
    -   Po wybraniu Zakresu widzę listę zawartych w nim Tematów.
    -   Lista domyślnie pokazuje wszystkie Tematy (aktywne i przyswojone).
    -   Mogę użyć filtra, aby ukryć Tematy oznaczone jako "Przyswojony".
    -   Mogę użyć filtra, aby ponownie pokazać Tematy "Przyswojone".
    -   Tematy "Przyswojone" są wizualnie odróżnione (wyszarzone, ikona ptaszka).

-   ID: US-010
-   Tytuł: Edycja Tematu
-   Opis: Jako zalogowany użytkownik, chcę móc edytować szczegóły istniejącego Tematu (nazwę, datę startową, notatki), aby poprawić błędy lub zaktualizować informacje.
-   Kryteria akceptacji:
    -   Mogę zainicjować edycję dla wybranego Tematu na liście.
    -   Mogę zmodyfikować nazwę Tematu (max 400 znaków).
    -   Mogę zmodyfikować datę rozpoczęcia nauki.
    -   Mogę zmodyfikować notatki (max 3000 znaków, plain text).
    -   System waliduje, czy nazwa i data startowa nie są puste.
    -   Po zapisaniu zmian, zaktualizowane dane są widoczne.
    -   Zmiana daty startowej powoduje przeliczenie całego harmonogramu powtórek od nowa.

-   ID: US-011
-   Tytuł: Usuwanie Tematu
-   Opis: Jako zalogowany użytkownik, chcę móc usunąć pojedynczy Temat, którego już nie potrzebuję.
-   Kryteria akceptacji:
    -   Mogę zainicjować usuwanie dla wybranego Tematu na liście.
    -   Wyświetlane jest potwierdzenie (np. prosty modal "Czy na pewno chcesz usunąć ten Temat?").
    -   Po potwierdzeniu, Temat jest trwale usuwany.
    -   Usunięty Temat znika z listy Tematów w Zakresie i z Panelu Powtórek.

### Sekcja 5.4: Planowanie i Wykonywanie Powtórek

-   ID: US-012
-   Tytuł: Wyświetlanie Panelu Powtórek
-   Opis: Jako zalogowany użytkownik, chcę widzieć w jednym miejscu listę wszystkich Tematów, które wymagają powtórki dzisiaj lub są zaległe, abym wiedział, co mam do zrobienia.
-   Kryteria akceptacji:
    -   Po zalogowaniu domyślnie widzę Panel Powtórek.
    -   Panel wyświetla Tematy z datą powtórki przypadającą na dzisiaj (wg strefy czasowej przeglądarki).
    -   Panel wyświetla Tematy z przeszłymi datami powtórek, które nie zostały oznaczone jako wykonane (zaległe).
    -   Tematy zaległe są na górze listy, posortowane od najstarszej daty wymagalności.
    -   Tematy wymagane dzisiaj są poniżej zaległych.
    -   W ramach tej samej daty wymagalności, sortowanie jest alfabetyczne wg nazwy Zakresu, a potem Tematu.
    -   Tematy zaległe są wizualnie wyróżnione (inny kolor tła, ikona ostrzegawcza).
    -   Dla każdego Tematu widoczna jest jego nazwa i nazwa Zakresu, do którego należy.

-   ID: US-013
-   Tytuł: Oznaczanie powtórki jako wykonanej
-   Opis: Jako zalogowany użytkownik, chcę móc oznaczyć Temat z Panelu Powtórek jako powtórzony, aby system wiedział, że zadanie zostało wykonane i mógł zaplanować kolejną powtórkę lub zakończyć cykl.
-   Kryteria akceptacji:
    -   Mogę oznaczyć Temat na liście w Panelu Powtórek jako wykonany za pomocą checkboxa.
    -   Po oznaczeniu, Temat znika z listy "Co na dziś" (chyba że natychmiast przypada kolejna powtórka, co jest mało prawdopodobne przy danych interwałach).
    -   System zapisuje datę wykonania powtórki.
    -   System oblicza i zapisuje datę następnej powtórki zgodnie z algorytmem (uwzględniając ewentualne opóźnienie i rekalkulację).

-   ID: US-014
-   Tytuł: Obsługa opóźnionej powtórki
-   Opis: Jako zalogowany użytkownik, jeśli oznaczyłem powtórkę z opóźnieniem, chcę, aby system odpowiednio dostosował harmonogram przyszłych powtórek dla tego Tematu.
-   Kryteria akceptacji:
    -   Gdy oznaczam zaległą powtórkę (planowaną na datę X) jako wykonaną w dniu X+Y, system rejestruje wykonanie.
    -   Wszystkie przyszłe, jeszcze nie wykonane, zaplanowane daty powtórek dla tego Tematu zostają przesunięte o Y dni.
    -   Data następnej powtórki jest obliczana na podstawie interwału liczonego od daty faktycznego wykonania (X+Y).

-   ID: US-015
-   Tytuł: Cofanie oznaczenia wykonania powtórki
-   Opis: Jako zalogowany użytkownik, chcę móc cofnąć omyłkowe oznaczenie ostatniej powtórki jako wykonanej, aby przywrócić poprzedni stan harmonogramu.
-   Kryteria akceptacji:
    -   Mogę kliknąć ponownie checkbox przy ostatnio oznaczonej powtórce (np. w widoku Tematu lub jeśli jest tam dostępny).
    -   Checkbox przy wcześniejszych wykonanych powtórkach w sekwencji jest nieaktywny (lub jego kliknięcie nie daje efektu).
    -   Po cofnięciu oznaczenia ostatniej powtórki, system przywraca poprzednią zaplanowaną datę dla tej powtórki.
    -   System cofa ewentualne zmiany w harmonogramie przyszłych powtórek spowodowane tym oznaczeniem.
    -   Temat ponownie pojawia się w Panelu Powtórek, jeśli jego przywrócona data wymagalności jest dzisiejsza lub przeszła.

-   ID: US-016
-   Tytuł: Zakończenie cyklu powtórek i decyzja o kontynuacji
-   Opis: Jako zalogowany użytkownik, po wykonaniu ostatniej powtórki ze standardowego cyklu (30 dni), chcę móc zdecydować, czy kontynuować powtórki co 30 dni, czy uznać Temat za przyswojony.
-   Kryteria akceptacji:
    -   Po oznaczeniu powtórki wynikającej z interwału 30-dniowego, wyświetla się modal z pytaniem o dalsze kroki.
    -   Modal oferuje dwie opcje: "Kontynuuj powtórki co 30 dni" i "Oznacz jako Przyswojony".
    -   Jeśli wybiorę "Kontynuuj", system planuje następną powtórkę za 30 dni od daty wykonania tej ostatniej.
    -   Jeśli wybiorę "Oznacz jako Przyswojony", Temat otrzymuje status "Mastered", jest odpowiednio oznaczany wizualnie i znika z Panelu Powtórek.

-   ID: US-017
-   Tytuł: Zarządzanie Tematami Przyswojonymi
-   Opis: Jako zalogowany użytkownik, chcę móc filtrować Tematy Przyswojone w widoku Zakresu oraz mieć możliwość przywrócenia ich do aktywnego cyklu powtórek.
-   Kryteria akceptacji:
    -   W widoku listy Tematów w Zakresie mogę użyć filtra "Ukryj/Pokaż Tematy Przyswojone".
    -   Tematy Przyswojone są wizualnie odróżnione (wyszarzone, ikona checkmark).
    -   Mogę przejść do edycji Tematu Przyswojonego.
    -   W formularzu edycji istnieje opcja (np. checkbox lub przycisk) pozwalająca cofnąć stan "Przyswojony".
    -   Po cofnięciu stanu "Przyswojony", Temat wraca do aktywnego cyklu (np. system planuje kolejną powtórkę za 30 dni od daty ostatniej faktycznie wykonanej powtórki przed oznaczeniem jako mastered).

### Sekcja 5.5: Pierwsze Użycie i Stany Brzegowe

-   ID: US-018
-   Tytuł: Stan początkowy aplikacji (Empty State)
-   Opis: Jako nowy, zalogowany użytkownik, który nie utworzył jeszcze żadnych Zakresów ani Tematów, chcę zobaczyć pomocny ekran powitalny i wskazówkę, jak zacząć.
-   Kryteria akceptacji:
    -   Po pierwszym zalogowaniu (lub gdy użytkownik nie ma Zakresów), Panel Powtórek jest pusty.
    -   Wyświetlany jest komunikat powitalny wyjaśniający cel aplikacji.
    -   Widoczny jest wyraźny przycisk akcji (np. "+") z etykietą "Utwórz pierwszy Zakres", który inicjuje proces tworzenia Zakresu.

-   ID: US-019
-   Tytuł: Podstawowa obsługa błędów
-   Opis: Jako użytkownik, w przypadku wystąpienia błędu (np. problem z zapisem, nieudana walidacja), chcę otrzymać zrozumiały komunikat zwrotny.
-   Kryteria akceptacji:
    -   Przy próbie zapisu formularza z niepoprawnymi danymi (np. pusta nazwa), wyświetlane są komunikaty walidacyjne przy odpowiednich polach.
    -   W przypadku ogólnego błędu serwera podczas zapisu, wyświetlany jest generyczny komunikat o błędzie.
    -   Przy nieudanym logowaniu wyświetlany jest komunikat o błędnych danych logowania lub konieczności weryfikacji email.


## 6. Metryki sukcesu

Kryteria sukcesu dla MVP aplikacji będą mierzone poprzez weryfikację kluczowych funkcjonalności i przepływów użytkownika:

1.  Realizacja Pełnego Cyklu Życia Tematu:
    -   Opis: Aplikacja umożliwia bezbłędne przejście przez cały zdefiniowany dla MVP cykl życia Tematu: stworzenie Zakresu, dodanie Tematu z datą startową, poprawne wygenerowanie i wyświetlenie pierwszej daty powtórki w Panelu Powtórek, oznaczenie powtórki jako wykonanej oraz poprawne obliczenie i wskazanie kolejnej daty powtórki zgodnie z algorytmem (w tym obsługa stanu "Przyswojony" lub kontynuacji co 30 dni).
    -   Mierzalność: 100% udanych prób przeprowadzenia pełnego cyklu dla co najmniej 3 różnych Tematów z różnymi datami startowymi podczas testów manualnych lub demonstracji.

2.  Poprawność Logiki Harmonogramu (w tym Rekalkulacji):
    -   Opis: Mechanizm generowania Harmonogramu Powtórek działa zgodnie z predefiniowanym, niezmiennym algorytmem (interwały 1, 3, 7, 14, 30 dni od daty startowej) oraz poprawnie obsługuje rekalkulację dat w przypadku opóźnionego wykonania powtórki (przesunięcie wszystkich przyszłych dat o czas opóźnienia).
    -   Mierzalność: Weryfikacja wygenerowanych sekwencji dat dla co najmniej 5 różnych scenariuszy (różne daty startowe, symulacja oznaczania powtórek w różnych terminach, w tym z opóźnieniami) potwierdza zgodność z założonym algorytmem. Weryfikacja manualna i/lub poprzez testy jednostkowe/integracyjne.

3.  Kompletność Funkcjonalności CRUD:
    -   Opis: Wszystkie podstawowe operacje zarządzania danymi (Tworzenie, Odczyt, Aktualizacja, Usuwanie) dla Zakresów (w tym podwójne potwierdzenie usunięcia) i Tematów działają zgodnie z oczekiwaniami, walidacją i bez błędów krytycznych.
    -   Mierzalność: Pomyślne wykonanie i weryfikacja efektów każdej operacji CRUD dla Zakresów i Tematów (np. utworzenie zakresu, dodanie tematu, edycja nazwy tematu, edycja notatek, usunięcie tematu, usunięcie zakresu wraz z tematami, filtrowanie tematów przyswojonych).

4.  Stabilność Podstawowych Ścieżek Użytkownika:
    -   Opis: Kluczowe interakcje zdefiniowane w MVP (rejestracja, weryfikacja email, logowanie, nawigacja między Panelem Powtórek a widokiem Zakresów/Tematów, korzystanie z Panelu Powtórek – sortowanie, oznaczanie, cofanie oznaczenia) działają płynnie i bez krytycznych błędów uniemożliwiających korzystanie z podstawowej funkcjonalności aplikacji. Stan początkowy jest poprawnie wyświetlany.
    -   Mierzalność: Możliwość przeprowadzenia pełnej demonstracji kluczowych ścieżek użytkownika bez napotkania błędów krytycznych lub nieoczekiwanych zachowań aplikacji.