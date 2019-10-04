using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WildstarPacketParser.Network.Message
{
    public static class MessageManager
    {
        private delegate IReadable MessageFactoryDelegate();
        private static ImmutableDictionary<Opcodes, MessageFactoryDelegate> clientMessageFactories;

        public static void Initialise()
        {
            InitialiseMessages();
        }

        private static void InitialiseMessages()
        {
            var messageFactories = new Dictionary<Opcodes, MessageFactoryDelegate>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                MessageAttribute attribute = type.GetCustomAttribute<MessageAttribute>();
                if (attribute == null)
                    continue;

                if (typeof(IReadable).IsAssignableFrom(type))
                {
                    NewExpression @new = Expression.New(type.GetConstructor(Type.EmptyTypes));
                    messageFactories.Add(attribute.Opcode, Expression.Lambda<MessageFactoryDelegate>(@new).Compile());
                }
            }

            clientMessageFactories = messageFactories.ToImmutableDictionary();
        }

        public static IReadable GetMessage(Opcodes opcode)
        {
            return clientMessageFactories.TryGetValue(opcode, out MessageFactoryDelegate factory)
                ? factory.Invoke() : null;
        }
    }
}
