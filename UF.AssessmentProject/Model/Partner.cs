using System.Diagnostics.CodeAnalysis;

namespace UF.AssessmentProject.Model
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class Partner
    {
        public Partner(int id, string partnerkey, string partnerpassword)
        {
            this.id = id;
            this.partnerkey = partnerkey;
            this.partnerpassword = partnerpassword;
        }

        public int id { get; }
        public string partnerkey { get; }
        public string partnerpassword { get; }
    }
}
