using thebooking.Models;

namespace thebooking.DAL;

public class MessageRepository : IMessageRepository
{
    private readonly BookingDbContext _db;

    public MessageRepository(BookingDbContext db)
    {
        _db = db;
    }

    public async Task<Message?> GetMessageById(int id)
    {
        return await _db.Messages.FindAsync(id);
    }

    public async Task Create(Message message)
    {
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();
    }

    public async Task Update(Message message)
    {
        _db.Messages.Update(message);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var message = await _db.Messages.FindAsync(id);
        if (message == null)
        {
            return false;
        }

        _db.Messages.Remove(message);
        await _db.SaveChangesAsync();
        return true;
    }
}