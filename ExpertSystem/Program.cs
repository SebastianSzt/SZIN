using System;
using System.Collections.Generic;
using System.Linq;

// Definicja faktów
public class Fact
{
    public string Name { get; set; }
    public bool Value { get; set; }
}

// Definicja reguł
public class Rule
{
    public string Name { get; set; }
    public List<string> Conditions { get; set; }
    public string Conclusion { get; set; }

    // Metoda do sprawdzania spełnienia warunków reguły
    public bool Evaluate(Dictionary<string, bool> facts)
    {
        foreach (var condition in Conditions)
            if (!facts.ContainsKey(condition) || !facts[condition])
                return false;
        return true;
    }
}

// System ekspercki
public class ExpertSystem
{
    private List<Fact> _facts = new List<Fact>();
    private List<Rule> _rules = new List<Rule>();

    // Dodawanie faktów
    public void AddFact(string name, bool value)
    {
        _facts.Add(new Fact { Name = name, Value = value });
    }

    // Dodawanie reguł
    public void AddRule(string name, List<string> conditions, string conclusion)
    {
        _rules.Add(new Rule { Name = name, Conditions = conditions, Conclusion = conclusion });
    }

    // Wnioskowanie w przód
    public void ForwardChaining()
    {
        bool newFactAdded = true;
        while (newFactAdded)
        {
            newFactAdded = false;
            foreach (var rule in _rules)
                if (!_facts.Exists(fact => fact.Name == rule.Conclusion) && rule.Evaluate(_facts.ToDictionary(fact => fact.Name, fact => fact.Value)))
                {
                    _facts.Add(new Fact { Name = rule.Conclusion, Value = true });
                    newFactAdded = true;
                }
        }
    }

    // Wnioskowanie wstecz
    public bool BackwardChaining(string goal)
    {
        if (_facts.Exists(fact => fact.Name == goal && fact.Value))
            return true;

        foreach (var rule in _rules)
        {
            if (rule.Conclusion == goal)
            {
                bool conditionsMet = true;
                foreach (var condition in rule.Conditions)
                {
                    if (!BackwardChaining(condition))
                    {
                        conditionsMet = false;
                        break;
                    }
                }
                if (conditionsMet)
                {
                    _facts.Add(new Fact { Name = goal, Value = true });
                    return true;
                }
            }
        }

        return false;
    }

    // Zapytanie użytkownika o fakt
    public bool AskUser(string factName)
    {
        Console.Write($"{factName} (Tak/Nie): ");
        string userInput = Console.ReadLine();
        return userInput.ToLower() == "tak";
    }
}

class Program
{
    static void Main(string[] args)
    {
        ExpertSystem expertSystem = new ExpertSystem();

        // Dodawanie reguł
        expertSystem.AddRule("Rule1", new List<string> { "Rain" }, "WetGround");
        expertSystem.AddRule("Rule2", new List<string> { "WetGround" }, "Puddles");

        // Zapytanie użytkownika o fakty
        Console.WriteLine("Odpowiedz na pytania: ");
        expertSystem.AddFact("Rain", expertSystem.AskUser("deszcz"));
        expertSystem.AddFact("WetGround", expertSystem.AskUser("mokra ziemia"));

        // Wywołanie wnioskowania w przód
        expertSystem.ForwardChaining();

        // Sprawdzenie czy wnioskowanie wstecz znajdzie odpowiedź
        bool result = expertSystem.BackwardChaining("Puddles");
        Console.WriteLine("\nWynik:");
        Console.WriteLine("Czy są kałuże na ziemi? " + (result ? "Tak" : "Nie"));
    }
}