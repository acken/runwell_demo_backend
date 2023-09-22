using RunwellDemo.Streams;
using RunwellDemo.Streams.Streams.Timeline;
using Uniscale.Core;

namespace Streams {
    public class TimelineInterceptors {
        private static Dictionary<Guid, MessageFull> messages = new Dictionary<Guid, MessageFull>();

        public static void registerInterceptors(PlatformInterceptorBuilder builder) {
            var patterns = Patterns.Streams.Timeline;
            builder
                // Register an interceptor for the message feature SendMessage
                .InterceptMessage(
                    // Specify the AllMessageUsages pattern so that the implementation
                    // picks up features for all use case instances this feature
                    // is used in
                    patterns.SendMessage.AllMessageUsages,
                    // Define a handler for the feature
                    patterns.SendMessage.Handle((input, ctx) => {
                        var msg = new MessageFull {
                            MessageIdentifier = Guid.NewGuid(),
                            Message = input.Message,
                            Created = new RunwellDemo.Streams.Streams.UserTag { By = input.By, At = DateTime.Now }
                        };
                        messages.Add(msg.MessageIdentifier, msg);
                    }))
                // Register an interceptor for the request/response feature GetMessageList
                .InterceptRequest(
                    patterns.GetMessageList.AllRequestUsages,
                    patterns.GetMessageList.Handle((input, ctx) => {
                        return messages.Values
                            .OrderByDescending(m => m.Created.At)
                            .Skip(input)
                            .Take(20)
                            .ToList();
                    }));
        }
    }
}
