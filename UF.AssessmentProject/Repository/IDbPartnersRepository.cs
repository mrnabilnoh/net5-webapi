namespace UF.AssessmentProject.Repository
{
    /// <summary>
    ///     Repository interface to access partner information
    /// </summary>
    /// <remarks>
    ///     Currently, only InMemoryPartnersRepository using this interface.
    ///     Decision to use interface rather than direct import is to reduce
    ///     redundant modification in controller if we decide to replace in-memory
    ///     database to other type of database later.
    /// </remarks>
    public interface IDbPartnersRepository
    {
        bool IsValid(string partnerkey, string partnerpassword);
    }
}
