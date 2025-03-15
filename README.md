# Recipe Book

## Описание
Проект выполнен в рамках курса C# на ФКН ВШЭ (Программная инженерия).  
Это консольное приложение для работы с рецептами.

## Функциональность

- Импорт и экспорт рецептов в TXT, JSON, CSV (локально и через Яндекс.Диск).
- Добавление рецептов вручную через консоль.
- Добавление изображений к рецепту.
- Фильтрация и сортировка рецептов по названию, категории и ингредиентам.
- Просмотр списка рецептов с пагинацией.
- Просмотр детальной информации о рецепте (включая изображения).
- Генерация списка покупок на основе выбранных рецептов.
- Генерация рецептов по запросу через GigaChat.
- Выбор случайного рецепта дня.

## Используемые библиотеки

| Библиотека | Назначение |
|------------|------------|
| Spectre.Console | Консольный интерфейс |
| CsvHelper | Работа с CSV-файлами |
| dotenv.net | Работа с переменными окружения |
| System.Text.Json | Работа с JSON-файлами |
| Spectre.Console.ImageSharp | Работа с изображениями |
| System.Net.Http | Работа с HTTP-запросами (GigaChat, Яндекс.Диск) |

## Запуск проекта

1. Склонируйте репозиторий:
   ```sh
   git clone https://github.com/Kandler3/Recipe-Book.git
   cd Recipe-Book
   ```
2. Настройте переменные окружения в файле `.env`
    ```
    YANDEX_DISK_CLIENT_ID=<clientID>
    GIGACHAT_TOKEN=<token>
    ```
3. Запустите приложение:
   ```sh
   dotnet run --project RecipeBookApp
   ```

4. При первом запуске для работы с Яндекс.Диском выполните авторизацию.


## Формат хранения рецептов

### Формат TXT
```
Название рецепта: Блины
Категория: Завтрак
Ингредиенты:
- Мука - 200 г
- Яйцо - 2 шт
- Молоко - 500 мл
- Соль - 1 ч.л.
Инструкция:
- Взбить яйца с молоком
- Добавить муку и соль
- Жарить на сковороде
Изображения:
- images/blini.jpg
```

### JSON
```json
{
    "$schema": "http://json-schema.org/draft-07/schema#",
    "title": "Recipe",
    "type": "object",
    "properties": {
      "Title": { "type": "string" },
      "Category": { "type": ["string", "null"] },
      "Ingredients": {
        "type": "array",
        "items": {
          "type": "object",
          "properties": {
            "Name": { "type": "string" },
            "Quantity": { "type": ["integer", "null"] },
            "Measurement": { "type": ["string", "null"] }
          },
          "required": ["Name"],
          "additionalProperties": false
        }
      },
      "Instructions": {
        "type": "array",
        "items": { "type": "string" }
      },
      "Images": {
        "type": "array",
        "items": { "type": "string" }
      }
    },
    "required": ["Title"],
    "additionalProperties": false
}
```
### Формат CSV
Рецепты в CSV хранятся в следующем формате:
```csv
Title,Category,Ingredients,Instructions,Images
"Блины","Завтрак","[{""Name"":""Мука"",""Quantity"":200,""Measurement"":""г""},{""Name"":""Яйцо"",""Quantity"":2,""Measurement"":""шт""},{""Name"":""Молоко"",""Quantity"":500,""Measurement"":""мл""},{""Name"":""Соль"",""Quantity"":1,""Measurement"":""ч.л.""}]","[""Взбить яйца с молоком"",""Добавить муку и соль"",""Жарить на сковороде""]","[""/images/blini.jpg""]"
```
- Title – строка.
- Category – строка или null.
- Ingredients – массив объектов в формате JSON (каждый ингредиент содержит Name, Quantity и Measurement).
- Instructions – массив строк в формате JSON.
- Images – массив строк в формате JSON.
