
namespace Meowziq.Core {
    /// <summary>
    /// Message オブジェクトのファクトリインタフェース
    /// </summary>
    public interface IFactory<T1, T2> {
        IMessage<T1, T2> CreateMessage();
    }
}
