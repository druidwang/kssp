// -----------------------------------------------------------------------
// <copyright file="NHDaoBase.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace com.Sconit.Persistence
{
    using Castle.Facilities.NHibernateIntegration;
    using NHibernate;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NHDaoBase
    {
        public ISessionManager sessionManager { get; set; }
        public string sessionFactoryAlias { get; set; }

        #region protected methods
        protected ISession GetSession()
        {
            if (string.IsNullOrWhiteSpace(sessionFactoryAlias))
                return sessionManager.OpenSession();
            else
                return sessionManager.OpenSession(sessionFactoryAlias);
        }

        protected IStatelessSession GetStatelessSession()
        {
            if (string.IsNullOrWhiteSpace(sessionFactoryAlias))
                return sessionManager.OpenStatelessSession();
            else
                return sessionManager.OpenStatelessSession(sessionFactoryAlias);
        }
        #endregion
    }
}
