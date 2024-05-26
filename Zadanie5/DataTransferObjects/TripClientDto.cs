namespace Zadanie5.DataTransferObjects;

public class TripClientDto
{
    public int TripId { get; set; }
    public TripDto Trip { get; set; }

    public int ClientId { get; set; }
    public ClientDto Client { get; set; }

    public DateTime RegisteredAt { get; set; }
}