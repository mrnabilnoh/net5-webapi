using System.Collections.Generic;
using UF.AssessmentProject.Model;

namespace UF.AssessmentProject.Repository
{
    /// <summary>
    ///     Repository to access partner information store in memory.
    /// </summary>
    /// <remarks>
    ///     Task requirement do not have any complexity for update/delete,
    ///     and partner information only contain fixed data.
    ///     So my decision to use in-memory is enough for this use-case.
    /// </remarks>
    public class InMemoryPartnersRepository : IDbPartnersRepository
    {
        private readonly List<Partner> _partners = new List<Partner>
        {
            new Partner(1, "FAKEGOOGLE", "FAKEPASSWORD1234"),
            new Partner(2, "FAKEPEOPLE", "FAKEPASSWORD4578")
        };

        public bool IsValid(string partnerkey, string partnerpassword)
        {
            return _partners
                .Exists(x => x.partnerkey == partnerkey
                             && x.partnerpassword == partnerpassword);
        }
    }
}
