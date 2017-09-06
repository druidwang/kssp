using System.Threading;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.Exception;

namespace com.Sconit.Entity
{
    public class SecurityContextHolder
    {
        private static ThreadLocal<User> securityContext = new ThreadLocal<User>();

        public static User Get()
        {
            return securityContext.Value;
        }

        public static void Set(User user)
        {
            //if (user == null)
            //{
            //    throw new TechnicalException("User can't be null.");
            //}
            securityContext.Value = user;
        }
    }
}
