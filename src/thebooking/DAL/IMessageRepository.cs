using thebooking.Models;

namespace thebooking.DAL;

// Repository for messages in a study session. All methods are async.
public interface IMessageRepository
{
    Task<Message?> GetMessageById(int id);
    Task Create(Message message);
    Task Update(Message message);
    Task<bool> Delete(int id);
}