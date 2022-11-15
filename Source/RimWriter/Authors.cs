using Verse;

namespace RimWriter;

public class Authors
{
    public static string Random => new[]
    {
        "Hemingway", "Austen", "Woolf", "Dickens", "Tolstoy", "Orwell", "Faulkner", "Tolkien", "Melville", "Kafka",
        "King", "Poe", "Conan", "Agatha", "Salinger", "Eliot", "Dante", "Proust", "Victor", "Verne", "Rowling",
        "Morrison", "Dumas", "Dahl", "Carrol", "Plato", "Giovanni", "Lovecraft", "Chekhov", "Aesop", "Ursula",
        "Balzac", "Neil", "Sartre", "Mark", "Arthur", "Jane", "Asimov", "Murakami", "Homer", "Frost", "Lee",
        "Lucas", "Spielberg", "Abrams"
    }.RandomElement();
}