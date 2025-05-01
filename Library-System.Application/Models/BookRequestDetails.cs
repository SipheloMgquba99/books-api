using System;

namespace Library_System.Application.Models;

public class BookRequestDetails
{
    public string Requestor { get; set; }
    public string ContactNumber { get; set; }
    public string BookTitle { get; set; }
    public string Author { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime ReturnDate { get; set; }
}