
namespace Meowziq.Core {
    /// <summary>
    /// Message オブジェクトのファクトリインタフェース
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface IFactory<T1, T2> {
        IMessage<T1, T2> CreateMessage();
    }
}
