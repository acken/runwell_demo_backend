using RunwellDemo.Account;
using RunwellDemo.Account.Account;
using Uniscale.Core;

namespace Account {
    public class UsersInterceptors {
        private static Dictionary<Guid, UserFull> users = new Dictionary<Guid, UserFull>();
        
        public static void registerInterceptors(PlatformInterceptorBuilder builder) {
            var patterns = Patterns.Account;
            builder
                // Register an interceptor for the request/response feature GetOrRegister
                .InterceptRequest(
                    // Specify the AllMessageUsages pattern so that the implementation
                    // picks up features for all use case instances this feature
                    // is used in
                    patterns.GetOrRegister.AllRequestUsages,
                    // Define a handler for the feature
                    patterns.GetOrRegister.Handle((input, ctx) => {
                        // First check if the user handle is already registered
                        var existingUser = users.Values
                            .FirstOrDefault(u => u.Handle.Equals(input));
                        // If it is already registered return the registered user
                        if (existingUser != null)
                            return existingUser;

                        var user = new UserFull {
                            UserIdentifier = Guid.NewGuid(),
                            Handle = input
                        };
                        users.Add(user.UserIdentifier, user);
                        return user;
                    }))
                // Register an interceptor for the request/response feature LookupUsers
                .InterceptRequest(
                    patterns.LookupUsers.AllRequestUsages,
                    patterns.LookupUsers.Handle((input, ctx) => {
                        return users.Values
                            .Where(u => input.Contains(u.UserIdentifier))
                            .ToList();
                    })
                )
                // Register an interceptor for the request/response feature SearchAllUsers
                .InterceptRequest(
                    patterns.SearchAllUsers.AllRequestUsages,
                    patterns.SearchAllUsers.Handle((input, ctx) => {
                        return users.Values
                            .Where(u => u.Handle.ToLower().Contains(input.ToLower()))
                            .ToList();
                    })
                );
                    
        }
    }
}
