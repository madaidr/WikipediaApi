// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using System.Net;

HttpClient client = new HttpClient();

string url = "https://ru.wikipedia.org/wiki/%D0%90%D0%BA%D1%81%D1%91%D0%BD%D0%BE%D0%B2,_%D0%90%D0%BB%D0%B5%D0%BA%D1%81%D0%B0%D0%BD%D0%B4%D1%80_%D0%9D%D0%B8%D0%BA%D0%B8%D1%84%D0%BE%D1%80%D0%BE%D0%B2%D0%B8%D1%87"; // Замените на нужный URL
string htmlContent = await GetWikipediaPageAsync(url);
//Console.WriteLine(htmlContent);
var htmlDoc = new HtmlDocument();
htmlDoc.LoadHtml(htmlContent);

// Извлечение изображения
var imageNode = htmlDoc.DocumentNode.SelectSingleNode("//img");
string imageUrl = imageNode?.GetAttributeValue("src", null);
if (imageUrl != null && !imageUrl.StartsWith("http"))
{
    imageUrl = "https:" + imageUrl; // Приведение к полному URL
}

// Извлечение ФИО
var fioNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='mw-page-title-main']");
string fio = fioNode?.InnerText;

// Извлечение первого параграфа
var firstParagraphNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mw-content-ltr mw-parser-output']//p");
string firstParagraph = firstParagraphNode?.InnerText;
// Удаление нежелательных HTML-сущностей и пробелов
firstParagraph = WebUtility.HtmlDecode(firstParagraph);
firstParagraph = firstParagraph.Replace("&nbsp;", " ").Trim();

// Сохранение результатов в текстовый файл
string filePath = "output.txt";
using (StreamWriter writer = new StreamWriter(filePath))
{
    writer.WriteLine($"Изображение: {imageUrl}");
    writer.WriteLine($"ФИО: {fio}");
    writer.WriteLine($"Первый параграф: {firstParagraph}");
}

// Сообщение об успешном сохранении
Console.WriteLine($"Данные успешно сохранены в файл: {filePath}");
// Вывод результатов
Console.WriteLine($"Изображение: {imageUrl}");
Console.WriteLine($"ФИО: {fio}");
Console.WriteLine($"Первый параграф: {firstParagraph}");
async Task<string> GetWikipediaPageAsync(string url)
{
    try
    {
        // Отправляем GET-запрос
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode(); // Генерируем исключение для неуспешных статусов

        // Читаем контент страницы
        string responseBody = await response.Content.ReadAsStringAsync();

        return responseBody;
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"Ошибка при получении данных: {e.Message}");
        return string.Empty;
    }
}

