# NutriTrack API документація

Цей API дозволяє користувачам взаємодіяти з базою даних. Крім того він містить опис EndPoints і як правильно з ними взаємодіяти, а також моделі бази даних.

## Встановлення та налаштування

### Передумови

Для роботи проєкту необхідно мати встановлені такі інструменти:

- [Visual Studio](https://visualstudio.microsoft.com/) (для розробки та запуску проєкту)
- [.NET SDK](https://dotnet.microsoft.com/download) (версія 7.0 або новіша)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) саме MSSQL Server
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) для взаємодії з базою даних
- ASP.NET API for Web SDK (створити проект на цій платформі )

### Крок 1: Клонування репозиторію

Клонуйте репозиторій на свій локальний комп'ютер:

git clone https://github.com/yourusername/nutritrack-api.git
cd nutritrack-api

### Крок 2: Створення бази даних

Створіть власну локальну базу даних в MSSQL Server код для створення таблиць знаходиться за посиланням https://drive.google.com/file/d/1GWw7AxZR8vGSWrrromTWD4NHXH047aUS/view?usp=sharing 

### Крок 3: Запуск проекту

В клонованому проекті вам треба замінити в файлі appsettings.json "DefaultConnection": на свою строку для підключення з MSSQL Server
Також замінити в файлі lunchSettings.json "applicationUrl": "Замінити на свій локальний ip та port до якого підключений ваш ПК"
Після цього можна запустити проект в Swagger UI та перевірити чи всі конролери працюють

### Крок 4: Доступ до API

Доступ до API буде за посиланням "http://192.168.0.106:5182/" тільки вже ваш ip адрес та порт 

## Всі доступні контролери та як з ними працювати

# AuthController API

Цей контролер обробляє автентифікацію користувачів, консультантів та адміністраторів. Він містить маршрути для реєстрації та входу.

## Маршрути

### Login

**POST** `/api/auth/login`

#### Опис

Вхід користувача за допомогою унікального ідентифікатора.

#### Параметри запиту

- `LoginRequest`
  - `user_uid` (string): Унікальний ідентифікатор користувача.

#### Відповідь

- 200 OK:
  - `user_uid` (string): Унікальний ідентифікатор користувача.
  - `nickname` (string): Нікнейм користувача.
- 401 Unauthorized:
  - `message` (string): Повідомлення про помилку.

### Login Consultant

**POST** `/api/auth/login-consultant`

#### Опис

Вхід консультанта за допомогою унікального ідентифікатора.

#### Параметри запиту

- `ConsultantLoginRequest`
  - `consultant_uid` (string): Унікальний ідентифікатор консультанта.

#### Відповідь

- 200 OK:
  - `consultant_uid` (string): Унікальний ідентифікатор консультанта.
  - `nickname` (string): Нікнейм консультанта.
  - `profile_picture` (string): Посилання на профільну картинку.
  - `profile_description` (string): Опис профілю.
- 401 Unauthorized:
  - `message` (string): Повідомлення про помилку.

### Register User

**POST** `/api/auth/register/user`

#### Опис

Реєстрація нового користувача.

#### Параметри запиту

- `RegisterUserRequest`
  - `user_uid` (string): Унікальний ідентифікатор користувача.
  - `nickname` (string): Нікнейм користувача.
  - `profile_picture` (string): Посилання на профільну картинку.
  - `profile_description` (string): Опис профілю.
  - `gender` (string): Стать користувача.
  - `height` (int): Зріст користувача.
  - `current_weight` (float): Поточна вага користувача.

#### Відповідь

- 201 Created:
  - `user_uid` (string): Унікальний ідентифікатор користувача.
  - `nickname` (string): Нікнейм користувача.
  - `profile_picture` (string): Посилання на профільну картинку.
  - `profile_description` (string): Опис профілю.
  - `gender` (string): Стать користувача.
  - `height` (int): Зріст користувача.
  - `current_weight` (float): Поточна вага користувача.
- 400 Bad Request:
  - `message` (string): Повідомлення про помилку.

### Register Consultant

**POST** `/api/auth/register/consultant`

#### Опис

Реєстрація нового консультанта.

#### Параметри запиту

- `RegisterConsultantRequest`
  - `consultant_uid` (string): Унікальний ідентифікатор консультанта.
  - `nickname` (string): Нікнейм консультанта.
  - `profile_picture` (string): Посилання на профільну картинку.
  - `profile_description` (string): Опис профілю.
  - `experience_years` (int): Кількість років досвіду.
  - `max_clients` (int): Максимальна кількість клієнтів.

#### Відповідь

- 201 Created:
  - `consultant_uid` (string): Унікальний ідентифікатор консультанта.
  - `nickname` (string): Нікнейм консультанта.
  - `profile_picture` (string): Посилання на профільну картинку.
  - `profile_description` (string): Опис профілю.
  - `experience_years` (int): Кількість років досвіду.
  - `max_clients` (int): Максимальна кількість клієнтів.
- 400 Bad Request:
  - `message` (string): Повідомлення про помилку.

### Register Admin

**POST** `/api/auth/register/admin`

#### Опис

Реєстрація нового адміністратора.

#### Параметри запиту

- `RegisterAdminRequest`
  - `admin_uid` (string): Унікальний ідентифікатор адміністратора.
  - `name` (string): Ім'я адміністратора.
  - `email` (string): Email адміністратора.
  - `phone_number` (string): Номер телефону адміністратора.

#### Відповідь

- 201 Created:
  - `admin_uid` (string): Унікальний ідентифікатор адміністратора.
  - `name` (string): Ім'я адміністратора.
  - `email` (string): Email адміністратора.
  - `phone_number` (string): Номер телефону адміністратора.
- 400 Bad Request:
  - `message` (string): Повідомлення про помилку.

### Login Admin

**POST** `/api/auth/login/admin`

#### Опис

Вхід адміністратора за допомогою унікального ідентифікатора.

#### Параметри запиту

- `AdminLoginRequest`
  - `admin_uid` (string): Унікальний ідентифікатор адміністратора.

#### Відповідь

- 200 OK:
  - `admin_uid` (string): Унікальний ідентифікатор адміністратора.
  - `name` (string): Ім'я адміністратора.
  - `email` (string): Email адміністратора.
  - `phone_number` (string): Номер телефону адміністратора.
- 401 Unauthorized:
  - `message` (string): Повідомлення про помилку.

# AdminController API

Цей контролер обробляє запити для адміністратора щодо управління користувачами та консультантами, а також надає статистику про реєстрацію та активність.

## Маршрути

### Remove User

**DELETE** `/api/admin/remove-user/{userId}`

#### Опис

Видалення користувача та всіх пов'язаних з ним записів (цілі, стрики, запити на консультантів, зв'язки з консультантами тощо).

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача.

#### Відповідь

- 200 OK:
  - `message` (string): Повідомлення про успішне видалення користувача.
- 404 Not Found:
  - `message` (string): Повідомлення про помилку, якщо користувач не знайдений.

### Remove Consultant

**DELETE** `/api/admin/remove-consultant/{consultantId}`

#### Опис

Видалення консультанта та всіх пов'язаних з ним записів (консультантські зауваження, запити на консультантів, зв'язки з користувачами тощо).

#### Параметри запиту

- `consultantId` (string): Унікальний ідентифікатор консультанта.

#### Відповідь

- 200 OK:
  - `message` (string): Повідомлення про успішне видалення консультанта.
- 404 Not Found:
  - `message` (string): Повідомлення про помилку, якщо консультант не знайдений.

### Get User Info

**GET** `/api/admin/get-user-info`

#### Опис

Отримання інформації про користувача за нікнеймом, датою реєстрації та датою останнього входу.

#### Параметри запиту

- `nickname` (string): Нікнейм користувача.
- `createdAt` (DateTime): Дата реєстрації користувача.
- `lastLogin` (DateTime): Дата останнього входу користувача.

#### Відповідь

- 200 OK:
  - `user_uid` (string): Унікальний ідентифікатор користувача.
  - `nickname` (string): Нікнейм користувача.
  - `created_at` (DateTime): Дата реєстрації користувача.
  - `last_login` (DateTime): Дата останнього входу користувача.
  - `gender` (string): Стать користувача.
  - `height` (int): Зріст користувача.
  - `current_weight` (float): Поточна вага користувача.
- 404 Not Found:
  - `message` (string): Повідомлення про помилку, якщо користувач не знайдений.

### Get Consultant Info

**GET** `/api/admin/get-consultant-info`

#### Опис

Отримання інформації про консультанта за нікнеймом, датою реєстрації та датою останнього входу.

#### Параметри запиту

- `nickname` (string): Нікнейм консультанта.
- `createdAt` (DateTime): Дата реєстрації консультанта.
- `lastLogin` (DateTime): Дата останнього входу консультанта.

#### Відповідь

- 200 OK:
  - `consultant_uid` (string): Унікальний ідентифікатор консультанта.
  - `nickname` (string): Нікнейм консультанта.
  - `created_at` (DateTime): Дата реєстрації консультанта.
  - `last_login` (DateTime): Дата останнього входу консультанта.
  - `current_clients` (int): Кількість поточних клієнтів.
  - `max_clients` (int): Максимальна кількість клієнтів.
  - `experience_years` (int): Кількість років досвіду консультанта.
- 404 Not Found:
  - `message` (string): Повідомлення про помилку, якщо консультант не знайдений.

### Get Statistics

**GET** `/api/admin/get-statistics`

#### Опис

Отримання статистики про кількість зареєстрованих користувачів та консультантів, а також кількість активних користувачів та консультантів.

#### Відповідь

- 200 OK:
  - `TotalUsers` (int): Загальна кількість користувачів.
  - `ActiveUsers` (int): Кількість активних користувачів.
  - `TotalConsultants` (int): Загальна кількість консультантів.
  - `ActiveConsultants` (int): Кількість активних консультантів.

# ConsultantController API

Цей контролер обробляє взаємодію між консультантами та користувачами, включаючи надсилання запрошень, управління клієнтами та оновлення профілю консультанта.

## Маршрути

### Надіслати запрошення користувачу

**POST** `/api/consultant/send-invite/{consultantId}`

#### Опис

Надсилання запрошення користувачу для встановлення зв'язку з консультантом.

#### Параметри запиту

- `consultantId` (string): Унікальний ідентифікатор консультанта
- `InviteUserRequest`:
  - `user_uid` (string): Унікальний ідентифікатор користувача

#### Відповідь

- 200 OK:
  - `message` (string): "Invite sent successfully"
- 404 Not Found:
  - `message` (string): "Consultant not found" або "User not found"
- 400 Bad Request:
  - `message` (string): "No available slots for new clients"

### Відповісти на запрошення

**POST** `/api/consultant/respond-invite/{userId}`

#### Опис

Користувач може прийняти або відхилити запрошення від консультанта.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `RespondToInviteRequest`:
  - `consultant_uid` (string): Унікальний ідентифікатор консультанта
  - `is_accepted` (boolean): Прийняття чи відхилення запрошення

#### Відповідь

- 200 OK:
  - `message` (string): "Invite response recorded"
- 404 Not Found:
  - `message` (string): "Invite not found"

### Видалити користувача

**DELETE** `/api/consultant/remove-user/{consultantId}/{userId}`

#### Опис

Видалення зв'язку між консультантом та користувачем.

#### Параметри запиту

- `consultantId` (string): Унікальний ідентифікатор консультанта
- `userId` (string): Унікальний ідентифікатор користувача

#### Відповідь

- 200 OK:
  - `message` (string): "User removed successfully and associated requests deleted"
- 404 Not Found:
  - `message` (string): "User is not assigned to this consultant"

### Оновити нікнейм консультанта

**PUT** `/api/consultant/update-nickname/{consultantId}`

#### Опис

Оновлення нікнейму консультанта.

#### Параметри запиту

- `consultantId` (string): Унікальний ідентифікатор консультанта
- `UpdateConsultantNicknameRequest`:
  - `new_nickname` (string): Новий нікнейм консультанта

#### Відповідь

- 200 OK:
  - `message` (string): "Nickname updated successfully"
- 404 Not Found:
  - `message` (string): "Consultant not found"
- 400 Bad Request:
  - `message` (string): "New nickname is required"

### Оновити фото профілю

**PUT** `/api/consultant/update-profile-picture/{consultantId}`

#### Опис

Оновлення фотографії профілю консультанта.

#### Параметри запиту

- `consultantId` (string): Унікальний ідентифікатор консультанта
- `UpdateConsultantProfilePictureRequest`:
  - `new_profile_picture` (string): Нове посилання на фото профілю

#### Відповідь

- 200 OK:
  - `message` (string): "Profile picture updated successfully"
- 404 Not Found:
  - `message` (string): "Consultant not found"
- 400 Bad Request:
  - `message` (string): "New profile picture is required"

### Оновити опис профілю

**PUT** `/api/consultant/update-profile-description/{consultantId}`

#### Опис

Оновлення опису профілю консультанта.

#### Параметри запиту

- `consultantId` (string): Унікальний ідентифікатор консультанта
- `UpdateConsultantProfileDescriptionRequest`:
  - `new_profile_description` (string): Новий опис профілю

#### Відповідь

- 200 OK:
  - `message` (string): "Profile description updated successfully"
- 404 Not Found:
  - `message` (string): "Consultant not found"
- 400 Bad Request:
  - `message` (string): "New profile description is required"

### Оновити максимальну кількість клієнтів

**PUT** `/api/consultant/update-max-clients/{consultantId}`

#### Опис

Оновлення максимальної кількості клієнтів консультанта.

#### Параметри запиту

- `consultantId` (string): Унікальний ідентифікатор консультанта
- `UpdateConsultantMaxClientsRequest`:
  - `new_max_clients` (int): Нова максимальна кількість клієнтів

#### Відповідь

- 200 OK:
  - `message` (string): "Max clients updated successfully"
- 404 Not Found:
  - `message` (string): "Consultant not found"
- 400 Bad Request:
  - `message` (string): "New max clients count is required"

# ConsultantNoteController API

Цей контролер обробляє створення, оновлення, отримання та видалення нотаток консультанта щодо цілей користувачів.

## Маршрути

### Додати нотатку

**POST** `/api/consultantnote/add-note`

#### Опис

Створення нової нотатки консультанта для конкретної цілі користувача.

#### Параметри запиту

- `CreateConsultantNoteRequest`:
  - `consultant_uid` (string, обов'язковий): Унікальний ідентифікатор консультанта
  - `goal_id` (int, обов'язковий): Ідентифікатор цілі
  - `content` (string, обов'язковий): Текст нотатки (максимум 1000 символів)

#### Відповідь

- 200 OK:
  - `message` (string): "Note added successfully"
- 404 Not Found:
  - `message` (string): "Goal not found"
- 400 Bad Request:
  - `message` (string): "Consultation request must be accepted"

### Оновити нотатку

**PUT** `/api/consultantnote/update-note/{noteId}`

#### Опис

Оновлення існуючої нотатки консультанта.

#### Параметри запиту

- `noteId` (int): Ідентифікатор нотатки
- `UpdateConsultantNoteRequest`:
  - `consultant_uid` (string, обов'язковий): Унікальний ідентифікатор консультанта
  - `content` (string, обов'язковий): Новий текст нотатки (максимум 1000 символів)

#### Відповідь

- 200 OK:
  - `message` (string): "Note updated successfully"
- 404 Not Found:
  - `message` (string): "Note not found"
- 400 Bad Request:
  - `message` (string): "Consultation request must be accepted"
- 401 Unauthorized:
  - `message` (string): "You are not authorized to update this note"

### Отримати нотатки

**GET** `/api/consultantnote/get-notes/{goalId}`

#### Опис

Отримання всіх нотаток для конкретної цілі.

#### Параметри запиту

- `goalId` (int): Ідентифікатор цілі

#### Відповідь

- 200 OK:
  - Масив нотаток, кожна містить:
    - `note_id` (int): Ідентифікатор нотатки
    - `consultant_nickname` (string): Нікнейм консультанта
    - `consultant_uid` (string): Унікальний ідентифікатор консультанта
    - `goal_id` (int): Ідентифікатор цілі
    - `content` (string): Текст нотатки
    - `created_at` (DateTime): Дата створення нотатки
- 404 Not Found:
  - `message` (string): "No notes found for this goal"

### Видалити нотатку

**DELETE** `/api/consultantnote/delete-note/{noteId}`

#### Опис

Видалення нотатки консультанта.

#### Параметри запиту

- `noteId` (int): Ідентифікатор нотатки
- Query параметри:
  - `consultantUid` (string): Унікальний ідентифікатор консультанта

#### Відповідь

- 200 OK:
  - `message` (string): "Note deleted successfully"
- 404 Not Found:
  - `message` (string): "Note not found"
- 400 Bad Request:
  - `message` (string): "Consultation request must be accepted"
- 401 Unauthorized:
  - `message` (string): "You are not authorized to delete this note"

# ExerciseController API
Цей контролер обробляє створення, оновлення, отримання та видалення записів про фізичні вправи користувачів.

## Маршрути

### Додати вправу

**POST** `/api/exercise/add-exercise/{userId}`
#### Опис

Створення нового запису про виконану вправу для конкретного користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `AddExerciseRequest`:
  - `exercise_name` (string, обов'язковий): Назва вправи
  - `duration_minutes` (int, обов'язковий): Тривалість вправи в хвилинах
  - `calories_burned` (float, обов'язковий): Кількість спалених калорій
  - `exercise_type` (string, обов'язковий): Тип вправи (кардіо, силові тощо)
  - `entry_date` (DateTime, обов'язковий): Дата виконання вправи
  
#### Відповідь

- 200 OK:
  - `message` (string): "Exercise entry added successfully"
- 404 Not Found:
  - `message` (string): "User not found"

### Оновити вправу

**PUT** `/api/exercise/update-exercise/{userId}/{exerciseId}`

#### Опис

Оновлення існуючого запису про вправу.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `exerciseId` (int): Ідентифікатор запису про вправу
- `UpdateExerciseRequest`:
  - `exercise_name` (string, обов'язковий): Назва вправи
  - `duration_minutes` (int, обов'язковий): Тривалість вправи в хвилинах
  - `calories_burned` (float, обов'язковий): Кількість спалених калорій
  - `exercise_type` (string, обов'язковий): Тип вправи
  - `entry_date` (DateTime, обов'язковий): Дата виконання вправи
  
#### Відповідь

- 200 OK:
  - `message` (string): "Exercise entry updated successfully"
- 404 Not Found:
  - `message` (string): "User not found" або "Exercise entry not found"

### Отримати вправи

**GET** `/api/exercise/get-exercises/{userId}`

#### Опис

Отримання всіх записів про вправи для конкретного користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача

#### Відповідь

- 200 OK:
  - Масив записів про вправи
- 404 Not Found:
  - `message` (string): "User not found"

### Видалити вправу

**DELETE** `/api/exercise/delete-exercise/{userId}/{exerciseId}`

#### Опис

Видалення запису про вправу.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `exerciseId` (int): Ідентифікатор запису про вправу

#### Відповідь

- 200 OK:
  - `message` (string): "Exercise entry deleted successfully"
- 404 Not Found:
  - `message` (string): "User not found" або "Exercise entry not found"

# GoalController API

Цей контролер обробляє створення, отримання та управління цілями користувачів щодо їхньої ваги та харчування.

## Маршрути

### Створити ціль

**POST** `/api/goal/create`

#### Опис

Створення нової цілі для користувача з розрахунком необхідного харчування.

#### Параметри запиту

- `CreateGoalRequest`:
  - `user_uid` (string, обов'язковий): Унікальний ідентифікатор користувача
  - `consultant_uid` (string, необов'язковий): Ідентифікатор консультанта
  - `goal_type` (GoalType, обов'язковий): Тип цілі (Loss/Gain/Maintain)
  - `target_weight` (double, обов'язковий): Цільова вага
  - `duration_weeks` (int, обов'язковий): Тривалість у тижнях
  
#### Відповідь

- 201 Created:
  - Об'єкт `GoalResponse` з усіма деталями створеної цілі
- 404 Not Found:
  - `message` (string): "User not found"
- 400 Bad Request:
  - `message` (string): "User data is incomplete for goal creation"

### Отримати ціль

**GET** `/api/goal/{id}`

#### Опис

Отримання детальної інформації про конкретну ціль.

#### Параметри запиту

- `id` (int): Ідентифікатор цілі

#### Відповідь

- 200 OK:
  - Об'єкт `GoalResponse`
- 404 Not Found:
  - `message` (string): "Goal not found"

### Отримати цілі користувача

**GET** `/api/goal/user/{uid}`

#### Опис

Отримання всіх цілей конкретного користувача.

#### Параметри запиту

- `uid` (string): Унікальний ідентифікатор користувача

#### Відповідь

- 200 OK:
  - Масив об'єктів `GoalResponse`
- 404 Not Found:
  - `message` (string): "No goals found for this user"

### Оновити вагу та перерахувати ціль

**PUT** `/api/goal/update-weight/{userId}`

#### Опис

Оновлення поточної ваги користувача та перерахунок активної цілі.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `UpdateWeightRequest`:
  - `new_weight` (double, обов'язковий): Нова вага користувача
  
#### Відповідь
- 200 OK:
  - `message` (string): "Weight updated and goal recalculated successfully"
- 404 Not Found:
  - `message` (string): "User not found" або "Goal not found for the user"

### Затвердити ціль

**PUT** `/api/goal/approve-goal/{goalId}`

#### Опис

Затвердження цілі консультантом.

#### Параметри запиту

- `goalId` (int): Ідентифікатор цілі
- `ApproveGoalRequest`:
  - `consultant_uid` (string, обов'язковий): Ідентифікатор консультанта
  
#### Відповідь

- 200 OK:
  - `message` (string): "Goal successfully approved"
- 404 Not Found:
  - `message` (string): "Goal not found or consultant not authorized to approve this goal"

# MealController API

Цей контролер обробляє створення, отримання та видалення записів про прийоми їжі користувачів.

## Маршрути

### Додати прийом їжі

**POST** `/api/meal/add-meal/{userId}`

#### Опис

Створення нового запису про прийом їжі з переліком продуктів.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `CreateMealRequest`:
  - `meal_type` (string, обов'язковий): Тип прийому їжі (сніданок, обід, вечеря, перекус)
  - `products` (array, обов'язковий): Список продуктів
    - Кожен продукт містить:
      - `product_name` (string): Назва продукту
      - `quantity_grams` (double): Кількість у грамах
      - `calories` (double): Калорій на 100г
      - `protein` (double): Білків на 100г
      - `carbs` (double): Вуглеводів на 100г
      - `fats` (double): Жирів на 100г
      
#### Відповідь

- 200 OK:
  - `message` (string): "Meal and products added successfully"
- 404 Not Found:
  - `message` (string): "User not found"

### Отримати прийом їжі

**GET** `/api/meal/get-meal/{userId}/{entryDate}/{mealType}`

#### Опис

Отримання всіх продуктів для конкретного прийому їжі за вказану дату.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `entryDate` (DateTime): Дата прийому їжі
- `mealType` (string): Тип прийому їжі

#### Відповідь

- 200 OK:
  - Масив продуктів, кожен містить:
    - `entry_id` (int): Ідентифікатор запису
    - `product_name` (string): Назва продукту
    - `quantity_grams` (double): Кількість у грамах
    - `calories` (double): Калорії
    - `protein` (double): Білки
    - `carbs` (double): Вуглеводи
    - `fats` (double): Жири
    - `created_at` (DateTime): Дата створення запису
- 404 Not Found:
  - `message` (string): "User not found" або "No meal entries found for the specified date and type"

### Видалити прийом їжі

**DELETE** `/api/meal/delete-meal/{userId}`

#### Опис

Видалення записів про прийом їжі за різними критеріями.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- Query параметри (потрібно вказати або `entryId`, або обидва `entryDate` та `mealType`):
  - `entryId` (int, необов'язковий): Ідентифікатор конкретного запису
  - `entryDate` (DateTime, необов'язковий): Дата прийому їжі
  - `mealType` (string, необов'язковий): Тип прийому їжі
  
#### Відповідь

- 200 OK:
  - `message` (string): "Meal entries deleted successfully"
  - `deletedCount` (int): Кількість видалених записів
- 404 Not Found:
  - `message` (string): "User not found" або "No meal entries found matching the provided criteria"
- 400 Bad Request:
  - `message` (string): "Either entryId or both entryDate and mealType must be provided"

# StreakController API

Цей контролер обробляє створення, оновлення, отримання та видалення записів про стрики для користувачів.

## Маршрути

### Додати новий стрик

**POST** `/api/streak/add-streak/{userId}`

#### Опис

Створення нового запису стрика для користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `AddStreakRequest`:
  - `current_streak` (int, обов'язковий): Початковий стрик (наприклад, 1)

#### Відповідь

- 200 OK:
  - `message` (string): "New streak started successfully."
- 404 Not Found:
  - `message` (string): "User not found."

### Оновити стрик

**PUT** `/api/streak/update-streak/{userId}`

#### Опис

Оновлення інформації про поточний стрик для користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `UpdateStreakRequest`:
  - `current_streak` (int, обов'язковий): Поточний стрик
  - `is_active` (bool, обов'язковий): Статус активності стрика

#### Відповідь

- 200 OK:
  - `message` (string): "Streak updated successfully."
- 404 Not Found:
  - `message` (string): "User not found" або "No active streak found."

### Отримати історію стриків

**GET** `/api/streak/get-streaks/{userId}`

#### Опис

Отримання історії стриків для користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача

#### Відповідь

- 200 OK:
  - Масив стриків, кожен містить:
    - `streak_id` (int): Ідентифікатор стрика
    - `streak_date` (DateTime): Дата стрика
    - `current_streak` (int): Поточний стрик
    - `is_active` (bool): Статус активності
- 404 Not Found:
  - `message` (string): "User not found"

### Видалити стрик

**DELETE** `/api/streak/delete-streak/{userId}`

#### Опис

Видалення (деактивація) активного стрика для користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача

#### Відповідь

- 200 OK:
  - `message` (string): "Streak deleted successfully."
- 404 Not Found:
  - `message` (string): "User not found" або "No active streak found."

# UserController API

Цей контролер обробляє оновлення інформації про користувача, зокрема нікнейму, фото профілю, опису профілю, ваги та видалення консультанта.

## Маршрути

### Оновлення нікнейму користувача

**PUT** `/api/user/update-nickname/{userId}`

#### Опис

Оновлення нікнейму користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `UpdateNicknameRequest`:
  - `new_nickname` (string, обов'язковий): Новий нікнейм користувача

#### Відповідь

- 200 OK:
  - `message` (string): "Nickname updated successfully."
- 404 Not Found:
  - `message` (string): "User not found."

---

### Оновлення фото профілю користувача

**PUT** `/api/user/update-profile-picture/{userId}`

#### Опис

Оновлення фото профілю користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `UpdateProfilePictureRequest`:
  - `new_profile_picture` (string, обов'язковий): Нове фото профілю користувача

#### Відповідь

- 200 OK:
  - `message` (string): "Profile picture updated successfully."
- 404 Not Found:
  - `message` (string): "User not found."

---

### Оновлення опису профілю користувача

**PUT** `/api/user/update-profile-description/{userId}`

#### Опис

Оновлення опису профілю користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `UpdateProfileDescriptionRequest`:
  - `new_profile_description` (string, обов'язковий): Новий опис профілю користувача

#### Відповідь

- 200 OK:
  - `message` (string): "Profile description updated successfully."
- 404 Not Found:
  - `message` (string): "User not found."

---

### Оновлення ваги користувача (current_weight)

**PUT** `/api/user/update-current-weight/{userId}`

#### Опис

Оновлення поточної ваги користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `UpdateCurrentWeightRequest`:
  - `new_current_weight` (double, обов'язковий): Нова вага користувача

#### Відповідь

- 200 OK:
  - `message` (string): "User current weight updated successfully."
- 404 Not Found:
  - `message` (string): "User not found."

---

### Видалити консультанта з користувача

**DELETE** `/api/user/remove-consultant/{userId}`

#### Опис

Видалення консультанта з користувача та оновлення кількості клієнтів консультанта.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `RemoveConsultantRequest`:
  - `consultant_uid` (string, обов'язковий): Унікальний ідентифікатор консультанта

#### Відповідь

- 200 OK:
  - `message` (string): "Consultant removed successfully and pending requests deleted."
- 404 Not Found:
  - `message` (string): "Consultant not found or not assigned to this user."

# WaterController API

Цей контролер обробляє операції, пов'язані з водоспоживанням користувача, включаючи додавання, оновлення, видалення записів про водоспоживання та отримання історії водоспоживання.

## Маршрути

### Додати запис про водоспоживання

**POST** `/api/water/add-water/{userId}`

#### Опис

Додає новий запис про водоспоживання для користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `AddWaterRequest`:
  - `amount_ml` (double, обов'язковий): Кількість води в мілілітрах
  - `entry_date` (DateTime, обов'язковий): Дата запису водоспоживання

#### Відповідь

- 200 OK:
  - `message` (string): "Water intake added successfully."
- 404 Not Found:
  - `message` (string): "User not found."

---

### Оновити запис про водоспоживання

**PUT** `/api/water/update-water/{userId}/{intakeId}`

#### Опис

Оновлює запис про водоспоживання користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `intakeId` (int): Унікальний ідентифікатор запису водоспоживання
- `UpdateWaterRequest`:
  - `amount_ml` (double, обов'язковий): Кількість води в мілілітрах
  - `entry_date` (DateTime, обов'язковий): Дата запису водоспоживання

#### Відповідь

- 200 OK:
  - `message` (string): "Water intake updated successfully."
- 404 Not Found:
  - `message` (string): "User not found."
  - `message` (string): "Water intake entry not found."

---

### Видалити запис про водоспоживання

**DELETE** `/api/water/delete-water/{userId}/{intakeId}`

#### Опис

Видаляє запис про водоспоживання для користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача
- `intakeId` (int): Унікальний ідентифікатор запису водоспоживання

#### Відповідь

- 200 OK:
  - `message` (string): "Water intake entry deleted successfully."
- 404 Not Found:
  - `message` (string): "User not found."
  - `message` (string): "Water intake entry not found."

---

### Отримати історію водоспоживання

**GET** `/api/water/get-water/{userId}`

#### Опис

Отримує історію водоспоживання для користувача.

#### Параметри запиту

- `userId` (string): Унікальний ідентифікатор користувача

#### Відповідь

- 200 OK:
  - Список записів водоспоживання для користувача:
    - `amount_ml` (double): Кількість води в мілілітрах
    - `entry_date` (DateTime): Дата запису водоспоживання
- 404 Not Found:
  - `message` (string): "User not found."

# WeightMeasurementsController API

Цей контролер обробляє операції, пов'язані з вимірюваннями ваги користувачів, включаючи додавання, отримання вимірювань та перегляд історії вимірювань для користувача.

## Маршрути

### Додати вимірювання ваги

**POST** `/api/WeightMeasurements`

#### Опис

Додає нове вимірювання ваги для користувача.

#### Параметри запиту

- `WeightMeasurementRequest`:
  - `UserUid` (string, обов'язковий): Унікальний ідентифікатор користувача
  - `Weight` (double, обов'язковий): Вага користувача
  - `MeasuredAt` (DateTime, обов'язковий): Дата і час вимірювання
  - `DeviceId` (string, обов'язковий): Ідентифікатор пристрою, що використовувався для вимірювання
  - `IsSynced` (bool, обов'язковий): Статус синхронізації даних

#### Відповідь

- 201 Created:
  - `Location` (string): Шлях до створеного ресурсу
  - `WeightMeasurement` (WeightMeasurement): Дані про додане вимірювання ваги
- 400 Bad Request:
  - `message` (string): "Invalid data."
- 404 Not Found:
  - `message` (string): "User not found."

---

### Отримати вимірювання ваги для користувача

**GET** `/api/WeightMeasurements/user/{userUid}`

#### Опис

Отримує всі вимірювання ваги для заданого користувача.

#### Параметри запиту

- `userUid` (string): Унікальний ідентифікатор користувача

#### Відповідь

- 200 OK:
  - Список вимірювань ваги для користувача:
    - `user_uid` (string): Унікальний ідентифікатор користувача
    - `weight` (double): Вага користувача
    - `measured_at` (DateTime): Дата і час вимірювання
    - `device_id` (string): Ідентифікатор пристрою, що використовувався для вимірювання
    - `is_synced` (bool): Статус синхронізації
- 404 Not Found:
  - `message` (string): "No weight measurements found for this user."

---

### Отримати вимірювання ваги за ідентифікатором

**GET** `/api/WeightMeasurements/{id}`

#### Опис

Отримує вимірювання ваги за вказаним ідентифікатором.

#### Параметри запиту

- `id` (int): Унікальний ідентифікатор вимірювання

#### Відповідь

- 200 OK:
  - Дані про вимірювання ваги:
    - `user_uid` (string): Унікальний ідентифікатор користувача
    - `weight` (double): Вага користувача
    - `measured_at` (DateTime): Дата і час вимірювання
    - `device_id` (string): Ідентифікатор пристрою, що використовувався для вимірювання
    - `is_synced` (bool): Статус синхронізації
- 404 Not Found:
  - `message` (string): "Weight measurement not found."

# NutriTrack API - Моделі даних

Цей проект реалізує API для обробки даних користувачів, консультацій, вимірювань ваги та інших аспектів для платформи NutriTrack.

## Моделі даних

### Admin
Модель для адміністратора системи.

- `admin_uid` - Унікальний ідентифікатор адміністратора
- `registration_date` - Дата реєстрації адміністратора
- `name` - Ім'я адміністратора
- `email` - Електронна пошта адміністратора
- `phone_number` - Номер телефону адміністратора (формат +380)

### User
Модель користувача.

- `user_uid` - Унікальний ідентифікатор користувача
- `nickname` - Нікнейм користувача
- `profile_picture` - Фото профілю користувача
- `profile_description` - Опис профілю користувача
- `gender` - Стать користувача
- `height` - Висота користувача
- `current_weight` - Поточна вага користувача
- `created_at` - Дата створення користувача
- `last_login` - Дата останнього входу
- `is_active` - Статус активності користувача

Колекції:
- `WeightMeasurements` - Вимірювання ваги користувача
- `UserGoals` - Цілі користувача
- `MealEntries` - Записи про прийоми їжі
- `ExerciseEntries` - Записи про тренування
- `WaterIntakes` - Споживання води
- `StreakHistories` - Історія досягнень
- `UserConsultants` - Консультанти, що працюють з користувачем
- `ConsultantNotes` - Нотатки консультантів
- `ConsultantRequests` - Запити на консультації

### Consultant
Модель консультанта.

- `consultant_uid` - Унікальний ідентифікатор консультанта
- `nickname` - Нікнейм консультанта
- `profile_picture` - Фото профілю консультанта
- `profile_description` - Опис профілю консультанта
- `experience_years` - Кількість років досвіду консультанта
- `is_active` - Статус активності консультанта
- `created_at` - Дата створення консультанта
- `last_login` - Дата останнього входу
- `max_clients` - Максимальна кількість клієнтів
- `current_clients` - Поточна кількість клієнтів

Колекції:
- `UserGoals` - Цілі користувачів
- `UserConsultants` - Зв'язки користувачів і консультантів
- `ConsultantNotes` - Нотатки консультантів
- `ConsultantRequests` - Запити на консультації

### WeightMeasurement
Модель вимірювання ваги користувача.

- `measurement_id` - Унікальний ідентифікатор вимірювання
- `user_uid` - Унікальний ідентифікатор користувача
- `weight` - Вага користувача
- `measured_at` - Дата вимірювання
- `device_id` - Ідентифікатор пристрою
- `is_synced` - Статус синхронізації

### UserGoal
Модель цілі користувача.

- `goal_id` - Унікальний ідентифікатор мети
- `user_uid` - Унікальний ідентифікатор користувача
- `consultant_uid` - Унікальний ідентифікатор консультанта
- `goal_type` - Тип мети (Gain, Loss, Maintain)
- `target_weight` - Цільова вага
- `duration_weeks` - Тривалість в тижнях
- `daily_calories` - Щоденне споживання калорій
- `daily_protein` - Щоденне споживання білка
- `daily_carbs` - Щоденне споживання вуглеводів
- `daily_fats` - Щоденне споживання жирів
- `start_date` - Дата початку
- `is_approved_by_consultant` - Чи затверджено консультантом

### MealEntry
Модель запису про прийом їжі.

- `entry_id` - Унікальний ідентифікатор запису
- `user_uid` - Унікальний ідентифікатор користувача
- `meal_type` - Тип їжі (сніданок, обід, вечеря, перекус)
- `entry_date` - Дата запису
- `product_name` - Назва продукту
- `quantity_grams` - Кількість продукту в грамах
- `calories` - Кількість калорій
- `protein` - Кількість білка
- `carbs` - Кількість вуглеводів
- `fats` - Кількість жирів

### ExerciseEntry
Модель запису про тренування.

- `exercise_id` - Унікальний ідентифікатор тренування
- `user_uid` - Унікальний ідентифікатор користувача
- `exercise_name` - Назва тренування
- `duration_minutes` - Тривалість в хвилинах
- `calories_burned` - Витрачені калорії
- `exercise_type` - Тип тренування
- `entry_date` - Дата тренування

### WaterIntake
Модель запису про споживання води.

- `intake_id` - Унікальний ідентифікатор запису
- `user_uid` - Унікальний ідентифікатор користувача
- `amount_ml` - Кількість води в мілілітрах
- `entry_date` - Дата споживання води

### StreakHistory
Модель історії досягнень.

- `streak_id` - Унікальний ідентифікатор історії
- `user_uid` - Унікальний ідентифікатор користувача
- `streak_date` - Дата досягнення
- `current_streak` - Поточна кількість досягнень
- `is_active` - Статус активності

### UserConsultant
Модель зв'язку між користувачем і консультантом.

- `user_consultant_id` - Унікальний ідентифікатор зв'язку
- `user_uid` - Унікальний ідентифікатор користувача
- `consultant_uid` - Унікальний ідентифікатор консультанта
- `assignment_date` - Дата призначення
- `is_active` - Статус активності

### ConsultantNote
Модель нотатки консультанта.

- `note_id` - Унікальний ідентифікатор нотатки
- `consultant_uid` - Унікальний ідентифікатор консультанта
- `goal_id` - Унікальний ідентифікатор мети
- `content` - Зміст нотатки
- `created_at` - Дата створення нотатки
- `user_uid` - Унікальний ідентифікатор користувача

### ConsultantRequest
Модель запиту на консультацію.

- `request_id` - Унікальний ідентифікатор запиту
- `consultant_uid` - Унікальний ідентифікатор консультанта
- `user_uid` - Унікальний ідентифікатор користувача
- `status` - Статус запиту (наприклад, "pending", "accepted", "rejected")
- `created_at` - Дата створення запиту
- `responded_at` - Дата відповіді на запит


