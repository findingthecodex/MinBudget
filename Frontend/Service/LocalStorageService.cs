// Importerar nödvändiga bibliotek för JSON-serialisering och JavaScript-interaktion.
using System.Text.Json;
using Microsoft.JSInterop;

namespace MinBudget.Service
{
    // Denna klass, LocalStorageService, tillhandahåller metoder för att interagera med webbläsarens lokala lagring (localStorage).
    // Det gör att applikationen kan spara och hämta data som finns kvar även efter att webbläsaren har stängts.
    public class LocalStorageService
    {
        // En privat referens till IJSRuntime, som används för att anropa JavaScript-funktioner från C#-kod.
        private readonly IJSRuntime _js;

        // Konstruktorn tar emot en instans av IJSRuntime (via dependency injection) och lagrar den.
        public LocalStorageService(IJSRuntime js)
        {
            _js = js;
        }

        // Asynkron metod för att spara en lista av objekt till localStorage.
        // 'T' är en generisk typ, vilket innebär att metoden kan spara listor av vilken typ som helst (t.ex. Income, Expense).
        public async Task SaveListAsync<T>(string key, List<T> list)
        {
            // Serialiserar listan till en JSON-sträng.
            var json = JsonSerializer.Serialize(list);
            // Anropar JavaScript-funktionen `localStorage.setItem` för att spara JSON-strängen med den angivna nyckeln.
            await _js.InvokeVoidAsync("localStorage.setItem", key, json);
        }

        // Asynkron metod för att läsa en lista av objekt från localStorage.
        public async Task<List<T>> ReadListAsync<T>(string key)
        {
            // Anropar JavaScript-funktionen `localStorage.getItem` för att hämta JSON-strängen som är associerad med nyckeln.
            var json = await _js.InvokeAsync<string>("localStorage.getItem", key);
            
            // Kontrollerar om en sträng faktiskt hämtades.
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    // Försöker deserialisera JSON-strängen tillbaka till en lista av den specificerade typen 'T'.
                    // Om deserialiseringen misslyckas eller returnerar null, returneras en ny, tom lista.
                    return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                }
                catch
                {
                    // Om ett fel uppstår under deserialiseringen (t.ex. om datan är korrupt), returnera en tom lista för att undvika krascher.
                    return new List<T>();
                }
            }
            // Om ingen data hittades för nyckeln, returnera en ny, tom lista.
            return new List<T>();
        }
    }
}
