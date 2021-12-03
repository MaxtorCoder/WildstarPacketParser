using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace WildstarPacketParser.Network.Message;

public delegate void MessageHandlerDelegate(Packet message);

public static class MessageManager
{
    private static ImmutableDictionary<Opcodes, MessageHandlerDelegate> clientMessageHandlers;

    public static void Initialise()
    {
        InitialiseMessages();
    }

    private static void InitialiseMessages()
    {
        var messageFactories = new Dictionary<Opcodes, MessageHandlerDelegate>();

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            foreach (var method in type.GetMethods())
            {
                if (method.DeclaringType != type)
                    continue;

                var attribute = method.GetCustomAttribute<MessageAttribute>();
                if (attribute == null)
                    continue;

                var readerParameter = Expression.Parameter(typeof(Packet));
                var paramInfo = method.GetParameters();

                if (method.IsStatic)
                {
                    var call = Expression.Call(method, Expression.Convert(readerParameter, paramInfo[0].ParameterType));
                    var lambda = Expression.Lambda<MessageHandlerDelegate>(call, readerParameter);
                    messageFactories.Add(attribute.Opcode, lambda.Compile());
                }
                else
                {
                    var call = Expression.Call(Expression.Convert(readerParameter, type), method);
                    var lambda = Expression.Lambda<MessageHandlerDelegate>(call, readerParameter);
                    messageFactories.Add(attribute.Opcode, lambda.Compile());
                }
            }
        }

        clientMessageHandlers = messageFactories.ToImmutableDictionary();
    }

    public static MessageHandlerDelegate GetMessageHandler(Opcodes opcode)
    {
        return clientMessageHandlers.TryGetValue(opcode, out MessageHandlerDelegate handler)
            ? handler : null;
    }
}
