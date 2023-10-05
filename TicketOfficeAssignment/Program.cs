


using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
enum Place
{
    Seated,
    Standing
}
class Ticket
{
    public int Age { get; set; }
    public Place Place { get; set; }
    public int Number { get; private set; }
    public Ticket(int age, Place place)
    {
        Age = age;
        Place = place;
        Number = TicketSalesManager.Instance.NextTicketNumber();
    }
    public void SetTicketNumber(int number)
    {
        Number = number;
    }
    public decimal Price()
    {
        decimal price = 0;
        if (Age <= 11)
        {
            price = (Place == Place.Seated) ? 50 : 25;
        }
        else if (Age >= 12 && Age <= 64)
        {
            price = (Place == Place.Seated) ? 170 : 110;
        }
        else if (Age >= 65)
        {
            price = (Place == Place.Seated) ? 100 : 60;
        }
        return price;
    }
    public decimal Tax()
    {
        decimal price = Price();
        decimal tax = Math.Round((1 - 1 / 1.06m) * price, 2);
        return tax;
    }
}
class TicketSalesManager
{
    private List<Ticket> tickets;
    private static TicketSalesManager instance;
    public static TicketSalesManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TicketSalesManager();
            }
            return instance;
        }
    }
    private TicketSalesManager()
    {
        tickets = new List<Ticket>();
    }
    public int NextTicketNumber()
    {
        int nextNumber = tickets.Count + 1;
        return nextNumber;
    }
    public Ticket AddTicket(Ticket ticket)
    {
        tickets.Add(ticket);
        return ticket;
    }
    public bool RemoveTicket(int ticketNumber)
    {
        Ticket ticketToRemove = tickets.FirstOrDefault(t => t.Number == ticketNumber);
        if (ticketToRemove != null)
        {
            tickets.Remove(ticketToRemove);
            return true;
        }
        return false;
    }
    public decimal SalesTotal()
    {
        decimal total = tickets.Sum(ticket => ticket.Price() + ticket.Tax());
        return total;
    }
    public int AmountOfTickets()
    {
        return tickets.Count;
    }
    public List<int> GetUsedPlaceNumbers()
    {
        return tickets.Select(ticket => ticket.Number).ToList();
    }
}
class TicketOffice
{
    static TicketSalesManager salesManager = TicketSalesManager.Instance;
    static void Main(string[] args)
    {
        System.Globalization.CultureInfo swedishCulture = new
        System.Globalization.CultureInfo("sv-SE");
        System.Threading.Thread.CurrentThread.CurrentCulture = swedishCulture;
        Console.WriteLine("Ticket Office - Calculate Ticket Price");
        Console.WriteLine("--------------------------------------");
        bool continueProgram = true;
        while (continueProgram)
        {
            Console.WriteLine("\nPlease select an option:");
            Console.WriteLine("1. Calculate Ticket");
            Console.WriteLine("2. Check Ticket Availability");
            Console.WriteLine("3. Display Used Ticket Numbers");
            Console.WriteLine("4. Remove Ticket");
            Console.WriteLine("5. Check Total Number of Tickets Issued");
            Console.WriteLine("6. Check Total Sales Amount");
            Console.WriteLine("7. Exit");
            Console.Write("\nEnter your choice (1/2/3/4/5/6/7): ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("\nEnter your age: ");
                    int age;
                    if (!int.TryParse(Console.ReadLine(), out age) || age < 0 || age > 110)
                    {
                        Console.WriteLine(" Invalid age input.");
                        continue;
                    }
                    Console.Write("Enter ticket type (Seated or Standing): ");
                    string placeInput = Console.ReadLine().ToLower();
                    Place place;
                    if (placeInput == "seated")
                    {
                        place = Place.Seated;
                    }
                    else if (placeInput == "standing")
                    {
                        place = Place.Standing;
                    }
                    else
                    {
                        Console.WriteLine(" Invalid place preference.");
                        continue;
                    }
                    Ticket ticket = new Ticket(age, place);
                    Console.WriteLine("\nReceipt:");
                    Console.WriteLine($" Age: {ticket.Age}");
                    Console.WriteLine($" Ticket Type: {ticket.Place}");
                    Console.WriteLine($" Ticket Price: {ticket.Price():C2}");
                    Console.WriteLine($" Tax (6%): {ticket.Tax():C2}");
                    Console.WriteLine($" Total Price: {(ticket.Price() +ticket.Tax()):C2}");
                    Console.WriteLine($" Ticket Number: {ticket.Number}");
                    salesManager.AddTicket(ticket);
                    break;
                case "2":
                    Console.Write("\nEnter the ticket number to check availability: ");
                    int ticketNumberToCheck;
                    if (int.TryParse(Console.ReadLine(), out ticketNumberToCheck))
                    {
                        if
                        (salesManager.GetUsedPlaceNumbers().Contains(ticketNumberToCheck))
                        {
                            Console.WriteLine($"\nTicket {ticketNumberToCheck} is already taken.\n");
                        }
                        else
                        {
                            Console.WriteLine($"\nTicket {ticketNumberToCheck} is available.\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine(" Invalid ticket number input.");
                    }
                    break;
                case "3":
                    DisplayUsedPlaceNumbers();
                    break;
                case "4":
                    Console.Write("\nEnter the ticket number to remove: ");
                    int ticketNumberToRemove;
                    if (int.TryParse(Console.ReadLine(), out ticketNumberToRemove))
                    {
                        if (salesManager.RemoveTicket(ticketNumberToRemove))
                        {
                            Console.WriteLine($"\nTicket {ticketNumberToRemove} has been removed.\n");
                        }
                        else
                        {
                            Console.WriteLine($"\nTicket {ticketNumberToRemove} not found.\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine(" Invalid ticket number input.");
                    }
                    break;
                case "5":
                    int totalTicketsIssued = salesManager.AmountOfTickets();
                    Console.WriteLine($"\nTotal Number of Tickets Issued: {totalTicketsIssued}\n");
                    break;
                case "6":
                    decimal totalSalesAmount = salesManager.SalesTotal();
                    Console.WriteLine($"\nTotal Sales Amount: {totalSalesAmount:C2}");
                    break;
                case "7":
                    continueProgram = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option (1/2/3/4/5/6/7).\n");
                    break;
            }
        }
        Console.WriteLine("Thank you for using the Ticket Office. Goodbye!");
    }
    static void DisplayUsedPlaceNumbers()
    {
        List<int> usedPlaceNumbers = salesManager.GetUsedPlaceNumbers();
        Console.WriteLine("\nUsed Ticket Numbers:");
        if (usedPlaceNumbers.Count == 0)
        {
            Console.WriteLine(" There are no used ticket numbers yet.\n");
        }
        else
        {
            foreach (int placeNumber in usedPlaceNumbers)
            {
                Console.WriteLine($"Ticket Number: {placeNumber}");
            }
            Console.WriteLine();
        }
    }
}