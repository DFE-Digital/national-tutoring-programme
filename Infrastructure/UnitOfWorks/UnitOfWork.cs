using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly NtpDbContext _context;
    public UnitOfWork(NtpDbContext context)
    {
        _context = context;
        EnquiryRepository = new EnquiryRepository(_context);
        LocalAuthorityDistrictRepository = new LocalAuthorityDistrictRepository(_context);
        SubjectRepository = new SubjectRepository(_context);
        TuitionPartnerRepository = new TuitionPartnerRepository(_context);
        TuitionTypeRepository = new TuitionTypeRepository(_context);
        SchoolRepository = new SchoolRepository(_context);
    }
    public IEnquiryRepository EnquiryRepository { get; private set; }

    public ILocalAuthorityDistrictRepository LocalAuthorityDistrictRepository { get; private set; }

    public ISubjectRepository SubjectRepository { get; private set; }

    public ITuitionPartnerRepository TuitionPartnerRepository { get; private set; }

    public ITuitionTypeRepository TuitionTypeRepository { get; private set; }

    public ISchoolRepository SchoolRepository { get; private set; }

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _context.Dispose();
    }
}