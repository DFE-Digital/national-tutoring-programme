namespace Application.Common.Interfaces;

public interface ITuitionPartnerShortlistStorageService
{
    /// <summary>
    /// Adds the specified Shortlisted Tuition Partners to the implemented storage.
    /// </summary>
    /// <param name="shortlistedTuitionPartnersSeoUrl"></param>
    bool AddTuitionPartners(IEnumerable<string> shortlistedTuitionPartnersSeoUrl);

    /// <summary>
    /// Gets all Shortlisted Tuition Partners present in the implemented storage.
    /// Note: It does not remove the Shortlisted Tuition Partners from the storage.
    /// </summary>
    /// <returns>A list of Shortlisted Tuition Partners</returns>
    IEnumerable<string> GetAllTuitionPartners();

    /// <summary>
    /// Removes the specified Shortlisted Tuition Partner stored in the implemented storage
    /// </summary>
    /// <param name="shortlistedTuitionPartnerSeoUrl"></param>
    bool RemoveTuitionPartner(string shortlistedTuitionPartnerSeoUrl);

    /// <summary>
    /// Removes every Shortlisted Tuition Partner present in the implemented storage.
    /// </summary>
    bool RemoveAllTuitionPartners();

    /// <summary>
    /// Check if a tuition partner is shortlisted.
    /// </summary>
    /// <param name="tuitionPartnerSeoUrl"></param>
    /// <returns>Returns true if present or false if not</returns>
    bool IsTuitionPartnerShortlisted(string tuitionPartnerSeoUrl);
}